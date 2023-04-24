#define _WINSOCK_DEPRECATED_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <fcntl.h>
#include <errno.h>
#ifdef VTWIN32
#include <winsock2.h>
#include <windows.h>
#include <ws2tcpip.h>
#else
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <sys/time.h>
#endif

#include "libY.h"
#include "libssh2.h"

#include "vtssh.h"

#pragma comment(lib, "Ws2_32.lib")

#ifdef VTWIN32
#define VTSSH_RETRY_INTERVAL			2000
#else
#define VTSSH_RETRY_INTERVAL			2000
#endif

struct vtssh
{
	vtssh_options *options;
	VTSOCK sock;
	vtssh_status_enum status;
	int notify;
	LIBSSH2_SESSION *session;
	LIBSSH2_CHANNEL *channel;
	Ythread *ssh_thread;
};

static void ssh_thread_proc(void *userdata)
{
	vtssh *ssh = (vtssh *)userdata;
	vtssh_options *options = ssh->options;
	LIBSSH2_CHANNEL *channel = ssh->channel;
	char buffer[8192] = { '\0' };

	while(ssh->status == VTSSH_STATUS_CONNECTED)
	{
		int rc = libssh2_channel_read(channel, buffer, sizeof(buffer));
		if(rc < 0)
		{
			// 读取失败，断开连接
			ssh->status = VTSSH_STATUS_DISCONNECTED;
			YLOGE("libssh2_channel_read failed, %d", rc);
			if(ssh->notify)
			{
				options->on_status_changed(ssh, ssh->status);
			}
			break;
		}
		else if(rc == 0)
		{
			// 继续读
			continue;
		}
		else
		{
			// 读到了一些数据
			options->on_data_received(ssh, buffer, rc);
		}
	}
}

/// <summary>
/// 连接ssh服务器
/// 连接成功后填充ssh->sock字段
/// </summary>
/// <param name="ssh"></param>
/// <returns></returns>
static int connect_ssh_server(vtssh *ssh)
{
	struct sockaddr_in sin;
	//socklen_t sinlen;
	vtssh_options *options = ssh->options;

#ifdef VTWIN32
	//char sockopt;
	SOCKET sock = INVALID_SOCKET;
	SOCKET listensock = INVALID_SOCKET, forwardsock = INVALID_SOCKET;
	WSADATA wsadata;
	int err;

	err = WSAStartup(MAKEWORD(2, 0), &wsadata);
	if(err != 0) {
		YLOGE("WSAStartup failed with error: %s", strerror(err));
		return VTSSH_ERR_SYSERR;
	}
#else
	int sockopt, sock = -1;
	int listensock = -1, forwardsock = -1;
#endif

	/* Connect to SSH server */
	sock = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
	if(sock == -1)
	{
		YLOGE("failed to open socket, %s", strerror(errno));
		return VTSSH_ERR_SYSERR;
	}

	sin.sin_family = AF_INET;
	sin.sin_addr.s_addr = inet_addr(options->serverip);
	if(INADDR_NONE == sin.sin_addr.s_addr) {
		YLOGE("inet_addr failed, %s", strerror(errno));
		return VTSSH_ERR_SYSERR;
	}
	sin.sin_port = htons(options->serverport);

	if(connect(sock, (struct sockaddr *)(&sin), sizeof(struct sockaddr_in)) != 0) {
		YLOGE("failed to connect, %s", strerror(errno));
		return VTSSH_ERR_SYSERR;
	}

	ssh->sock = sock;
	
	YLOGI("connect ssh server success");

	return VTSSH_ERR_OK;
}

/// <summary>
/// 初始化sshChannel
/// 成功后填充ssh->channel字段
/// </summary>
/// <param name="ssh"></param>
/// <returns></returns>
static int initialize_ssh_channel(vtssh *ssh)
{
	vtssh_options *options = ssh->options;
	VTSOCK sock = ssh->sock;
	const char *fingerprint;
	char *userauthlist;
	vtssh_auth_enum supported_auth = 0;
	vtssh_auth_enum auth = options->auth;
	LIBSSH2_SESSION *session = NULL;
	int rc = 0;

	/* Create a session instance */
	session = libssh2_session_init();
	if(!session) {
		YLOGE("Could not initialize SSH session!");
		return VTSSH_ERR_SYSERR;
	}

	/* ... start it up. This will trade welcome banners, exchange keys,
	 * and setup crypto, compression, and MAC layers
	 */
	rc = libssh2_session_handshake(session, sock);
	if(rc) {
		YLOGE("Error when starting up SSH session: %d", rc);
		return VTSSH_ERR_SYSERR;
	}

	/* At this point we havn't yet authenticated.  The first thing to do
	 * is check the hostkey's fingerprint against our known hosts Your app
	 * may have it hard coded, may go to a file, may present it to the
	 * user, that's your call
	 */
	fingerprint = libssh2_hostkey_hash(session, LIBSSH2_HOSTKEY_HASH_SHA1);
	fprintf(stderr, "Fingerprint: ");
	for(int i = 0; i < 20; i++)
		fprintf(stderr, "%02X ", (unsigned char)fingerprint[i]);
	fprintf(stderr, "\n");

	/* check what authentication methods are available */
	userauthlist = libssh2_userauth_list(session, options->username, strlen(options->username));
	YLOGI("Authentication methods: %s", userauthlist);
	if(strstr(userauthlist, "password"))
	{
		supported_auth |= VTSSH_AUTH_PASSWORD;
	}
	if(strstr(userauthlist, "publickey"))
	{
		supported_auth |= VTSSH_AUTH_PUBLICKEY;
	}

#pragma region 登录逻辑
	switch(auth)
	{
	case VTSSH_AUTH_NONE:
	{
		YLOGI("ssh_auth_none");
		break;
	}

	case VTSSH_AUTH_PASSWORD:
	{
		// 使用密码登录
		if(libssh2_userauth_password(session, options->username, options->password)) {
			YLOGE("Authentication by password failed");
			return VTSSH_ERR_AUTH_FAILED;
		}
		else {
			YLOGI("Authentication by password succeeded");
			break;
		}
	}

	case VTSSH_AUTH_PUBLICKEY:
	{
		if(libssh2_userauth_publickey_fromfile(session, options->username, options->keyfile1, options->keyfile2, options->password)) {
			YLOGE("Authentication by public key failed!");
			return VTSSH_ERR_AUTH_FAILED;
		}
		else {
			YLOGI("Authentication by public key succeeded.");
			break;
		}
	}

	default:
		YLOGE("No supported authentication methods found!");
		break;
	}
#pragma endregion

#pragma region 创建SSH Shell通道
	/* Request a shell */
	LIBSSH2_CHANNEL *channel = libssh2_channel_open_session(session);
	if(channel == NULL)
	{
		YLOGE("libssh2_channel_open_session failed");
		return VTSSH_ERR_SYSERR;
	}

	/* Request a terminal with 'vanilla' terminal emulation
	 * See /etc/termcap for more options
	 */
	rc = libssh2_channel_request_pty_ex(channel, options->term, strlen(options->term), NULL, 0, options->term_columns, options->term_rows, 0, 0);
	if(rc != 0)
	{
		YLOGE("libssh2_channel_request_pty_ex failed, %d", rc);
		return VTSSH_ERR_SYSERR;
	}

	/* Open a SHELL on that pty */
	if(libssh2_channel_shell(channel)) {
		YLOGE("Unable to request shell on allocated pty");
		return VTSSH_ERR_SYSERR;
	}
#pragma endregion

	/* At this point the shell can be interacted with using
	 * libssh2_channel_read()
	 * libssh2_channel_read_stderr()
	 * libssh2_channel_write()
	 * libssh2_channel_write_stderr()
	 *
	 * Blocking mode may be (en|dis)abled with: libssh2_channel_set_blocking()
	 * If the server send EOF, libssh2_channel_eof() will return non-0
	 * To send EOF to the server use: libssh2_channel_send_eof()
	 * A channel can be closed with: libssh2_channel_close()
	 * A channel can be freed with: libssh2_channel_free()
	 */

	ssh->session = session;
	ssh->channel = channel;

	return VTSSH_ERR_OK;
}




int vtssh_create(vtssh **_ssh, vtssh_options *ssh_options)
{
	vtssh *ssh = (vtssh *)calloc(1, sizeof(vtssh));
	if(ssh == NULL)
	{
		return VTSSH_ERR_NO_MEM;
	}

	ssh->options = ssh_options;

	*_ssh = ssh;

	return VTSSH_ERR_OK;
}

int vtssh_connect(vtssh *ssh)
{
	vtssh_options *options = ssh->options;
	int rc = 0;

	// 1. 连接ssh服务器
	if((rc = connect_ssh_server(ssh)) != VTSSH_ERR_OK)
	{
		return rc;
	}

	// 2. 初始化ssh通道
	if((rc = initialize_ssh_channel(ssh)) != VTSSH_ERR_OK)
	{
		return rc;
	}

	ssh->notify = 1;

	Ythread *thread = Y_create_thread(ssh_thread_proc, ssh);
	ssh->ssh_thread = thread;
	return VTSSH_ERR_OK;
}

int vtssh_send(vtssh *ssh, char *bytes, uint32_t bytesize)
{
	if(ssh->channel == NULL)
	{
		return VTSSH_ERR_OK;
	}
	libssh2_channel_write(ssh->channel, bytes, bytesize);
	return VTSSH_ERR_OK;
}

void vtssh_disconnect(vtssh *ssh)
{
	ssh->status = VTSSH_STATUS_DISCONNECTED;
	ssh->notify = 0;

	if(ssh->channel)
	{
		libssh2_channel_close(ssh->channel);
		libssh2_channel_free(ssh->channel);
		ssh->channel = NULL;
	}

	if(ssh->session)
	{
		libssh2_session_disconnect(ssh->session, "Normal Shutdown, Thank you for playing");
		libssh2_session_free(ssh->session);
		ssh->session = NULL;
	}

#ifdef WIN32
	closesocket(ssh->sock);
#else
	close(ssh->sock);
#endif
	Y_delete_thread(ssh->ssh_thread);
}

void vtssh_delete(vtssh *ssh)
{
	free(ssh);
}


/***********************************************************************************
 * @ file    : vtssh.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2023.04.03 19:10
 * @ brief   : 封装不同平台通用的ssh客户端
 * 代码参考自：https://www.libssh2.org/examples/direct_tcpip.html
 ************************************************************************************/

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
	LIBSSH2_SESSION *session;
};

static void ssh_thread_proc(void *userdata)
{
	vtssh *ssh = (vtssh *)userdata;
	vtssh_options *options = ssh->options;
	VTSOCK sock = ssh->sock;
	LIBSSH2_SESSION *session = ssh->session;
	fd_set fds;
	struct timeval tv;
	ssize_t len, wr;
	char buf[16384];
	const char *shost;
	unsigned int sport;
	int rc = 0;

	while(ssh->status == VTSSH_STATUS_CONNECTED)
	{
		FD_ZERO(&fds);
		FD_SET(sock, &fds);
		tv.tv_sec = 0;
		tv.tv_usec = 100000;
		rc = select(sock + 1, &fds, NULL, NULL, &tv);
		if(-1 == rc) {
			YLOGE("select failed, %s", strerror(errno));
			ssh->status = VTSSH_STATUS_DISCONNECTED;
			break;
		}
		if(rc && FD_ISSET(sock, &fds)) {
			len = recv(sock, buf, sizeof(buf), 0);
			if(len < 0) {
				YLOGE("read failed, %s", strerror(errno));
				ssh->status = VTSSH_STATUS_DISCONNECTED;
				break;
			}
			else if(0 == len) {
				ssh->status = VTSSH_STATUS_DISCONNECTED;
				YLOGE("The client at %s:%d disconnected!", shost, sport);
				break;
			}
			wr = 0;
			while(wr < len) {
				i = libssh2_channel_write(channel, buf + wr, len - wr);

				if(LIBSSH2_ERROR_EAGAIN == i) {
					continue;
				}
				if(i < 0) {
					fprintf(stderr, "libssh2_channel_write: %d\n", i);
					goto shutdown;
				}
				wr += i;
			}
		}
		while(1) {
			len = libssh2_channel_read(channel, buf, sizeof(buf));

			if(LIBSSH2_ERROR_EAGAIN == len)
				break;
			else if(len < 0) {
				fprintf(stderr, "libssh2_channel_read: %d", (int)len);
				goto shutdown;
			}
			wr = 0;
			while(wr < len) {
				i = send(forwardsock, buf + wr, len - wr, 0);
				if(i <= 0) {
					perror("write");
					goto shutdown;
				}
				wr += i;
			}
			if(libssh2_channel_eof(channel)) {

				fprintf(stderr, "The server at %s:%d disconnected!\n",
					remote_desthost, remote_destport);
				goto shutdown;
			}
		}
	}
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

int vtssh_start(vtssh *ssh)
{
	struct sockaddr_in sin;
	socklen_t sinlen;
	vtssh_options *options = ssh->options;
	const char *fingerprint;
	char *userauthlist;
	vtssh_auth_enum supported_auth = 0;
	vtssh_auth_enum auth = options->auth;
	LIBSSH2_SESSION *session = NULL;
	int rc = 0;

#ifdef VTWIN32
	char sockopt;
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
		return;
	}

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
		return;
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
			return;
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
			return;
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
	//libssh2_channel_open_ex(session, )
	LIBSSH2_CHANNEL *channel = libssh2_channel_open_session(session);

#pragma endregion



	ssh->sock = sock;
	ssh->options = options;
	ssh->session = session;

	/* Must use non-blocking IO hereafter due to the current libssh2 API */
	libssh2_session_set_blocking(session, 0);

	Ythread *thread = Y_create_thread(ssh_thread_proc, ssh);
	return VTSSH_ERR_OK;
}

void vtssh_stop(vtssh *ssh)
{

}

void vtssh_delete(vtssh *ssh)
{
	free(ssh);
}


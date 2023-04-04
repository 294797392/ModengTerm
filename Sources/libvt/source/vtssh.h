/***********************************************************************************
 * @ file    : vtssh.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2023.04.03 19:10
 * @ brief   : 封装不同平台通用的ssh客户端
 * 代码参考自：https://www.libssh2.org/examples/direct_tcpip.html
 ************************************************************************************/

#ifndef __VTSSH_H__
#define __VTSSH_H__

#include "vt.h"

#define VTSSH_ERR_OK			0
#define VTSSH_ERR_NO_MEM		1
#define VTSSH_ERR_SYSERR		2
#define VTSSH_ERR_AUTH_FAILED	3

typedef struct vtssh vtssh;
typedef struct vtssh_options vtssh_options;
typedef enum vtssh_status_enum vtssh_status_enum;
typedef enum vtssh_auth_enum vtssh_auth_enum;

typedef void(*vtssh_data_received_callback)(vtssh *ssh, char *data, uint32_t datasize);
typedef void(*vtssh_status_changed_callback)(vtssh *ssh, vtssh_status_enum status);

struct vtssh_options
{
	vtssh_data_received_callback on_data_received;
	vtssh_status_changed_callback on_status_changed;
	char serverip[128];
	int serverport;

	// 身份验证方式
	vtssh_auth_enum auth;

	// 登录用户名
	char username[512];

	// 如果auth方式为PASSWORD，那么存储登录密码
	char password[256];

	char keyfile1[256];
	char keyfile2[256];

	// 要请求的终端类型，vt100,xterm,xterm-256color...etc
	// 在linux里使用echo $TERM可以查看当前终端类型
	char term[64];

	int term_columns;		// 终端的列数
	int term_rows;			// 终端的行数
};

enum vtssh_status_enum
{
	VTSSH_STATUS_CONNECTING,
	VTSSH_STATUS_CONNECTED,
	VTSSH_STATUS_DISCONNECTED
};

enum vtssh_auth_enum {
	VTSSH_AUTH_NONE = 0,
	VTSSH_AUTH_PASSWORD,
	VTSSH_AUTH_PUBLICKEY
};

VTAPI int vtssh_create(vtssh **_ssh, vtssh_options *ssh_options);
VTAPI int vtssh_connect(vtssh *ssh);
VTAPI int vtssh_send(vtssh *ssh, char *bytes, uint32_t bytesize);
VTAPI void vtssh_disconnect(vtssh *ssh);
VTAPI void vtssh_delete(vtssh *ssh);

#endif

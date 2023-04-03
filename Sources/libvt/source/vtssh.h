#ifndef __VTSSH_H__
#define __VTSSH_H__

#include "vtbase.h"

#define VTSSH_ERR_OK			0
#define VTSSH_ERR_NO_MEM		1
#define VTSSH_ERR_SYSERR		2


typedef struct vtssh vtssh;
typedef struct vtssh_options vtssh_options;
typedef enum vtssh_status_enum vtssh_status_enum;
typedef enum vtssh_auth_enum vtssh_auth_enum;

typedef void(*on_vtssh_data_received)(vtssh *ssh, char *data, size_t datasize);
typedef void(*on_vtssh_status_changed)(vtssh *ssh, vtssh_status_enum status);

struct vtssh_options
{
	on_vtssh_data_received data_callback;
	on_vtssh_status_changed status_callback;
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
};

enum vtssh_status_enum
{
	VTSSH_STATUS_CONNECTING,
	VTSSH_STATUS_CONNECTED,
	// 断线了，但是会自动重连
	VTSSH_STATUS_DISCONNECTED,
	// 停止工作了，不会重连
	VTSSH_STATUS_STOPPED
};

enum vtssh_auth_enum {
	VTSSH_AUTH_NONE = 0,
	VTSSH_AUTH_PASSWORD,
	VTSSH_AUTH_PUBLICKEY
};

VTAPI int vtssh_create(vtssh **_ssh, vtssh_options *ssh_options);
VTAPI int vtssh_start(vtssh *ssh);
VTAPI void vtssh_stop(vtssh *ssh);
VTAPI void vtssh_delete(vtssh *ssh);

#endif

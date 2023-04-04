#ifndef __LIBVT_EXPORT_H__
#define __LIBVT_EXPORT_H__

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

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

#define VTAPI

#ifdef VTWIN32
typedef SOCKET VTSOCK;
#define sleep_ms(ms) Sleep(ms)
#else
typedef int VTSOCK;
#define sleep_ms(ms) sleep(ms / 1000)
#endif

#endif

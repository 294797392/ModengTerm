#ifndef __LIBVT_H__
#define __LIBVT_H__

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

// ���嵼���͵������
// ע�⣬��������Ӿ�̬�⣬��ô����Ҫ__declspec(dllimport)
#if (defined(LIBVT_WIN32))
#ifdef LIBVT_EXPORT
#define LIBVTAPI __declspec(dllexport)
#else
#define LIBVTAPI __declspec(dllimport)
#endif
#else
#define LIBVTAPI
#endif

#endif

#ifndef __VTCOMMON_H__
#define __VTCOMMON_H__

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

// ���嵼���͵������
// ע�⣬��������Ӿ�̬�⣬��ô����Ҫ__declspec(dllimport)
#if (defined(LIBVT_WIN32))
#ifdef LIBVT_EXPORT
#define VTAPI __declspec(dllexport)
#else
#define VTAPI __declspec(dllimport)
#endif
#else
#define VTAPI
#endif

#define VTCOLOR_SIZE                32

#endif

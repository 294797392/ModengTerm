#ifndef __VTCOMMON_H__
#define __VTCOMMON_H__

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

// 定义导出和导入符号
// 注意，如果是链接静态库，那么不需要__declspec(dllimport)
#if (defined(LIBVT_WIN32))
#ifdef LIBVT_EXPORT
#define VTAPI __declspec(dllexport)
#else
#define VTAPI __declspec(dllimport)
#endif
#else
#define VTAPI
#endif

#endif

#ifndef __VTCORE_H__
#define __VTCORE_H__

// 定义导出和导入符号
// 注意，如果是链接静态库，那么不需要__declspec(dllimport)
#if (defined(VTCORE_WIN32))
#ifdef VTCORE_EXPORT
#define VTCOREAPI __declspec(dllexport)
#else
#define VTCOREAPI __declspec(dllimport)
#endif
#else
#define VTCOREAPI
#endif

#endif
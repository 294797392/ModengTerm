#ifndef __VTCORE_H__
#define __VTCORE_H__

// ���嵼���͵������
// ע�⣬��������Ӿ�̬�⣬��ô����Ҫ__declspec(dllimport)
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
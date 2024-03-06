#ifndef __VTCHARACTER_H__
#define __VTCHARACTER_H__

#include "VTCommon.h"
#include "VTColor.h"

typedef struct VTCharacter VTCharacter;

struct VTCharacter
{
    // 需要指针类型，因为有可能一个字符占用多个字节
    char *character;

    // 字符大小，单位是字节
    int size;

    // 一个字符占几列
    int columnSize;

    int attribute;

    VTColor *background;
    VTColor *foreground;

    VTCharacter *prev;
    VTCharacter *next;
};

#endif
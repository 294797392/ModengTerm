#ifndef __VTEXTLINE_H__
#define __VTEXTLINE_H__

#include "VTCommon.h"
#include "VTCharacter.h"
#include "VTextMetrics.h"

typedef struct VTextLine VTextLine;

struct VTextLine
{
    int physicsRow;
    VTextLine *previous;
    VTextLine *next;
    VTCharacter *character;
    int columns;
    VTextMetrics *metrics;
};

#endif

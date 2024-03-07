#ifndef __VTEXTLINE_H__
#define __VTEXTLINE_H__

#include "VTCommon.h"
#include "VTCharacter.h"
#include "VTextMetrics.h"
#include "VTypeface.h"

#ifdef __cplusplus 
extern "C" {
#endif

    typedef struct VTextLine VTextLine;

    struct VTextLine
    {
        int physicsRow;
        VTextLine *previous;
        VTextLine *next;
        VTCharacter *character;
        int columns;
        VTextMetrics *metrics;
        double offsetX;
        double offsetY;
        VTypeface *typeface;
    };

    VTAPI VTextLine *VTextLine_new();
    VTAPI void VTextLine_free(VTextLine *textLine);
    VTAPI void VTextLine_initialize(VTextLine *textLine);
    VTAPI void VTextLine_release(VTextLine *textLine);

    VTAPI void VTextLine_append(VTextLine *textLine, VTextLine *toAppend);

#ifdef __cplusplus 
}
#endif

#endif

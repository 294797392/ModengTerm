#ifndef __VTCOLOR_H__
#define __VTCOLOR_H__

#include "VTCommon.h"

typedef struct VTColor
{
    char key[32];
    char r;
    char g;
    char b;
    char html[32];
}VTColor;

VTAPI VTColor *VTColor_createFromRgb(char r, char g, char b);

VTAPI VTColor *VTColor_createFromRgbKey(char *rgbKey);

#endif
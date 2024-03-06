#ifndef __VTCURSOR_H__
#define __VTCURSOR_H__

#include "VTDocument.h"

typedef enum VTCursorStyles
{
    VTCURSOR_STYLES_NONE,
    VTCURSOR_STYLES_LINE,
    VTCURSOR_STYLES_BLOCK,
    VTCURSOR_STYLES_UNDERSCORE
}VTCursorStyles;

typedef struct VTCursor 
{
    int column;
    int row;
    int blinkState;
    int blinkAllowed;
    VTCursorStyles style;
    char color[32];
    int interval;
    VTDocument *ownerDocument;
}VTCursor;

#endif

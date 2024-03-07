#ifndef __VTCURSOR_H__
#define __VTCURSOR_H__

#include "VTDocument.h"
#include "VTypeface.h"

#ifdef __cplusplus 
extern "C" {
#endif

    typedef enum VTCursorStyles
    {
        VTCURSOR_STYLES_NONE,
        VTCURSOR_STYLES_LINE,
        VTCURSOR_STYLES_BLOCK,
        VTCURSOR_STYLES_UNDERSCORE
    }VTCursorStyles;

    typedef enum VTCursorSpeeds
    {
        VTCURSOR_SPEEDS_HIGH = 300,
        VTCURSOR_SPEEDS_NORMAL = 600,
        VTCURSOR_SPEEDS_LOW = 900
    }VTCursorSpeeds;

    typedef struct VTCursor 
    {
        int column;
        int row;
        int blinkState;
        int blinkAllowed;
        VTCursorStyles style;
        char color[VTCOLOR_SIZE];
        int interval;
        VTDocument *ownerDocument;
        double offsetX;
        double offsetY;
        int isVisible;
        VTypeface *typeface;
    }VTCursor;

    VTAPI VTCursor *VTCursor_new();
    VTAPI void VTCursor_free(VTCursor *cursor);
    VTAPI void VTCursor_initialize(VTCursor *cursor);
    VTAPI void VTCursor_release(VTCursor *cursor);

#ifdef __cplusplus 
}
#endif


#endif

#ifndef __VTDOCUMENT_H__
#define __VTDOCUMENT_H__

#include "VTCommon.h"
#include "VTypeface.h"
#include "VTCursor.h"
#include "VTCharacter.h"

#ifdef __cplusplus 
extern "C" {
#endif

    typedef struct VTDocument VTDocument;
    typedef struct VTDocumentOptions
    {
        int viewportRow;
        int viewportColumn;
        VTypeface *typeface;
        int scrollDelta;
        int scrollbackMax;
        int autoWrapMode;
        VTCursorStyles cursorStyle;
        VTCursorSpeeds cursorSpeed;
        char cursorColor[VTCOLOR_SIZE];
        char selectionColor[VTCOLOR_SIZE];
    }VTDocumentOptions;

    VTAPI VTDocument *VTDocument_new();
    VTAPI void VTDocument_free(VTDocument *document);

    VTAPI void VTDocument_initialize(VTDocument *document, VTDocumentOptions *options);
    VTAPI void VTDocument_release(VTDocument *document);

    VTAPI void VTDocument_printCharacter(VTCharacter *character);

#ifdef __cplusplus 
}
#endif

#endif


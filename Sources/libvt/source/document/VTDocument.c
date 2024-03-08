#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "VTDocument.h"
#include "VTextSelection.h"
#include "VTCursor.h"
#include "VTextLine.h"
#include "VTScrollInfo.h"

struct VTDocument
{
    VTCursor *cursor;
    VTextSelection *selection;
    VTScrollInfo *scrollInfo;
    int scrollMarginTop;
    int scrollMarginBottom;

    VTextLine *activeLine;
    VTextLine *firstLine;
    VTextLine *lastLine;
};



static VTextLine *createTextLine(int physicsRow, VTDocumentOptions *options)
{
    VTextLine *textLine = VTextLine_new();
    textLine->offsetX = 0;
    textLine->offsetY = 0;
    textLine->physicsRow = physicsRow;
    textLine->typeface = options->typeface;
    VTextLine_initialize(textLine);
    return textLine;
}





VTDocument *VTDocument_new()
{
    VTDocument *document = (VTDocument*)calloc(1, sizeof(VTDocument));
    return document;
}

void VTDocument_free(VTDocument *document)
{
    free(document);
}

void VTDocument_initialize(VTDocument *document, VTDocumentOptions *options)
{
    VTCursor *cursor = VTCursor_new();
    cursor->offsetX = 0;
    cursor->offsetY = 0;
    cursor->row = 0;
    cursor->column = 0;
    cursor->interval = (int)options->cursorSpeed;
    cursor->blinkAllowed = 1;
    cursor->isVisible = 1;
    strncpy(cursor->color, options->cursorColor, VTCOLOR_SIZE);
    cursor->style = options->cursorStyle;
    cursor->typeface = options->typeface;
    VTCursor_initialize(cursor);
    document->cursor = cursor;

    VTextSelection *selection = VTextSelection_new();
    strncpy(selection->color, options->selectionColor, VTCOLOR_SIZE);
    VTextSelection_initialize(selection);
    document->selection = selection;

    VTScrollInfo *scrollInfo = VTScrollInfo_new();
    scrollInfo->viewportRow = options->viewportRow;
    scrollInfo->scrollbackMax = options->scrollbackMax;
    VTScrollInfo_initialize(scrollInfo);
    document->scrollInfo = scrollInfo;

    VTextLine *firstLine = createTextLine(0, options);
    document->firstLine = firstLine;
    document->lastLine = firstLine;
    document->activeLine = firstLine;
    for (int i = 1; i < options->viewportRow; i++)
    {
        VTextLine *textLine = createTextLine(i, options);
        VTextLine_append(document->lastLine, textLine);
    }
}

void VTDocument_release(VTDocument *document)
{

}

void VTDocument_printCharacter(VTDocument *document, VTCharacter *character)
{
    VTextLine *activeLine = document->activeLine;
}

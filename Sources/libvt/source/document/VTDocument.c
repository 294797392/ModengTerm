#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "VTextSelection.h"
#include "VTCursor.h"
#include "VTextLine.h"
#include "VTDocument.h"

struct VTDocument
{
    VTCursor *cursor;
    VTextSelection *selection;
    int scrollMarginTop;
    int scrollMarginBottom;

    VTextLine *activeLine;
    VTextLine *firstLine;
    VTextLine *nextLine;
};

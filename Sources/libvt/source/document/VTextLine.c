#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "VTextLine.h"

VTextLine *VTextLine_new()
{
    VTextLine *textLine = (VTextLine*)calloc(1, sizeof(VTextLine));
    return textLine;
}

void VTextLine_free(VTextLine *textLine)
{
    free(textLine);
}

void VTextLine_initialize(VTextLine *textLine)
{}

void VTextLine_release(VTextLine *textLine)
{}

void VTextLine_append(VTextLine *textLine, VTextLine *toAppend)
{}
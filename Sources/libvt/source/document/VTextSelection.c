#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "VTextSelection.h"

VTextSelection *VTextSelection_new()
{
    VTextSelection *selection = (VTextSelection*)calloc(1, sizeof(VTextSelection));
    return selection;
}

void VTextSelection_free(VTextSelection *selection)
{
    free(selection);
}

void VTextSelection_initialize(VTextSelection *selection)
{
    
}

void VTextSelection_release(VTextSelection *selection)
{}



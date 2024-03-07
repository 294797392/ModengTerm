#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "VTCursor.h"

VTCursor *VTCursor_new()
{
    VTCursor *cursor = (VTCursor*)calloc(1, sizeof(VTCursor));
    return cursor;
}

void VTCursor_free(VTCursor *cursor)
{
    free(cursor);
}

void VTCursor_initialize(VTCursor *cursor)
{
    
}
  
void VTCursor_release(VTCursor *cursor)
{

}

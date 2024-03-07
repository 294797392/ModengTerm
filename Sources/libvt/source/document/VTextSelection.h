#ifndef __VTEXTSELECTION_H__
#define __VTEXTSELECTION_H__

#include "VTCommon.h"

#ifdef __cplusplus 
extern "C" {
#endif

    typedef struct VTextSelection 
    {
        int firstRow;
        int lastRow;
        int firstRowCharacterIndex;
        int lastRowCharacterIndex;
        char color[VTCOLOR_SIZE];
    }VTextSelection;

    VTAPI VTextSelection *VTextSelection_new();
    VTAPI void VTextSelection_free(VTextSelection *selection);
    VTAPI void VTextSelection_initialize(VTextSelection *selection);
    VTAPI void VTextSelection_release(VTextSelection *selection);

#ifdef __cplusplus 
}
#endif

#endif
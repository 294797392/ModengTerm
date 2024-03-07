#ifndef __VTSCROLLINFO_H__
#define __VTSCROLLINFO_H__

#include "VTCommon.h"

#ifdef __cplusplus 
extern "C" {
#endif

    typedef struct VTScrollInfo
    {
        int scrollbackMax;
        int scrollMax;
        int scrollMin;
        int scrollValue;
        int viewportRow;
    }VTScrollInfo;

    VTAPI VTScrollInfo *VTScrollInfo_new();
    VTAPI void VTScrollInfo_free(VTScrollInfo *scrollInfo);
    VTAPI void VTScrollInfo_initialize(VTScrollInfo *scrollInfo);
    VTAPI void VTScrollInfo_release(VTScrollInfo *scrollInfo);

    VTAPI void VTScrollInfo_setScrollMax(VTScrollInfo *scrollInfo, int scrollMax);
    VTAPI void VTScrollInfo_hasScroll(VTScrollInfo *scrollInfo);
    VTAPI void VTScrollInfo_scrollAtBottom(VTScrollInfo *scrollInfo);
    VTAPI void VTScrollInfo_scrollAtTop(VTScrollInfo *scrollInfo);

#ifdef __cplusplus 
}
#endif

#endif
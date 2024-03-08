#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "VTScrollInfo.h"

VTScrollInfo *VTScrollInfo_new()
{
	VTScrollInfo *scrollInfo = (VTScrollInfo *)calloc(1, sizeof(VTScrollInfo));
	return scrollInfo;
}

void VTScrollInfo_free(VTScrollInfo *scrollInfo)
{
	free(scrollInfo);
}

void VTScrollInfo_initialize(VTScrollInfo *scrollInfo)
{}

void VTScrollInfo_release(VTScrollInfo *scrollInfo)
{}

void VTScrollInfo_setScrollMax(VTScrollInfo *scrollInfo, int scrollMax)
{}

void VTScrollInfo_hasScroll(VTScrollInfo *scrollInfo)
{}

void VTScrollInfo_scrollAtBottom(VTScrollInfo *scrollInfo)
{}

void VTScrollInfo_scrollAtTop(VTScrollInfo *scrollInfo)
{}


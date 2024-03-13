#ifndef __VTPARSER_H__
#define __VTPARSER_H__

#include "VTCommon.h"

#ifdef __cplusplus 
extern "C" {
#endif

	typedef struct VTParser VTParser;

	VTAPI VTParser *VTParser_new();
	VTAPI void VTParser_free(VTParser *parser);
	VTAPI void VTParser_porcess(VTParser *parser, const char *bytes, size_t size);

#ifdef __cplusplus 
}
#endif

#endif
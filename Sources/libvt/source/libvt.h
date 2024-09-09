#include "VTCommon.h"

#define VTCODE_FAILED					0
#define VTCODE_SUCCESS					1

#ifdef __cplusplus 
extern "C" {
#endif


	typedef struct libvt_pseudo_console libvt_pseudo_console;

	VTAPI int libvt_create_pseudo_console(libvt_pseudo_console **console);
	VTAPI void libvt_free_pseudo_console(libvt_pseudo_console *console);
	VTAPI int libvt_read_pseudo_console(libvt_pseudo_console *console);
	VTAPI int libvt_write_pseudo_console(libvt_pseudo_console *console);


#ifdef __cplusplus 
}
#endif


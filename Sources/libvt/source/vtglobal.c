#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include <libY.h>
#include <libssh2.h>

#include "vtglobal.h"

void vtglobal_initialize()
{
	Y_log_init(NULL);
	libssh2_init(0);
}

void vtglobal_release()
{
}

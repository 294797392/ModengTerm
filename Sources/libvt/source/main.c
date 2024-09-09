#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "libvt.h"

int main(int argc, char **argv)
{
	libvt_pseudo_console *console = NULL;
	libvt_create_pseudo_console(&console);

	return 0;
}

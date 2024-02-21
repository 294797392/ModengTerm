#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "vtcore.h"

#ifndef __VTPARSER_H__
#define __VTPARSER_H__

#ifdef __cplusplus 
extern "C" {
#endif

	typedef struct vtparser_s vtparser;
	typedef struct vtparser_config_s
	{
		void *p;
	}vtparser_config;

    /*
	 * ������
	 *
	 * ������
	 * @config���������������ļ�
	 *
	 * ����ֵ��
	 * vtparserʵ��
	 */
	VTCOREAPI vtparser *vtparser_new(vtparser_config *config);

	VTCOREAPI void vtparser_free(vtparser *parser);

	VTCOREAPI void vtparser_init(vtparser *parser);

	VTCOREAPI void vtparser_release(vtparser *parser);

	VTCOREAPI void vtparser_process_characters(vtparser *parser, char *bytes, int size);

#ifdef __cplusplus 
}
#endif

#endif
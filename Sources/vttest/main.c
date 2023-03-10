#include <stdlib.h>
#include <stdio.h>
#include <string.h>

static char CUU[3] = { 0x1b, '[','A' };// �������
static char CUD[3] = { 0x1b, '[','B' };// �������
static char CUB[3] = { 0x1b, '[','D' };// �������
static char CUF[3] = { 0x1b, '[','C' };// �������
static char CUP00[6] = { 0x1b, '[', '0',';','0', 'H' };// ����ƶ���00

static char ReverseLineFeed[2] = {0x1b, 'I'};

int main()
{
	system("mode con cols=80 lines=24");

	for(size_t i = 0; i < 25; i++)
	{
		char line[1024] = { '\0' };
		snprintf(line, sizeof(line), "%d\n", i);
		printf(line);
	}

	printf(CUP00);
	//printf(ReverseLineFeed);
	//printf(ReverseLineFeed);
	printf("ABC");

	char read[1024];
	fgets(read, sizeof(read), stdin);
}

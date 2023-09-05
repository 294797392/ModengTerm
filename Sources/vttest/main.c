#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <wchar.h>

#include <Windows.h>

static char CUU[3] = { 0x1b, '[','A' };// �������
static char CUD[3] = { 0x1b, '[','B' };// �������
static char CUB[3] = { 0x1b, '[','D' };// �������
static char CUF[3] = { 0x1b, '[','C' };// �������
static char CUP00[6] = { 0x1b, '[', '0',';','0', 'H' };// ����ƶ���00

static char ReverseLineFeed[2] = {0x1b, 'I'};

static char DCH[4] = { 0x1b, '[','2','P' };


static char SGR[4] = { 0x1b, '[', (char)33, 'm' };


static void TestDCH()
{
	printf("1234567");
	printf(CUB);
	printf(CUB);
	printf(CUB);
	printf(CUB);
	//printf(DCH);
}

static void TestSGR()
{
	printf(SGR);
	printf("123");
}

int main()
{
    HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
    if (hOut == INVALID_HANDLE_VALUE)
    {
        return GetLastError();
    }

    DWORD dwMode = 0;
    if (!GetConsoleMode(hOut, &dwMode))
    {
        return GetLastError();
    }

    dwMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
    if (!SetConsoleMode(hOut, dwMode))
    {
        return GetLastError();
    }

    wprintf(L"\x1b[31m          This text has a red foreground using SGR.31.\r\n");

	//TestDCH();
	//TestSGR();

	char read[1024];
	fgets(read, sizeof(read), stdin);
}

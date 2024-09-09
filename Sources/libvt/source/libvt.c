#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include <Windows.h>

#include "libvt.h"

struct libvt_pseudo_console
{
	HANDLE inputReadSide;
	HANDLE outputWriteSide;
	HANDLE outputReadSide;
	HANDLE inputWriteSide;

	HPCON hPC;

	STARTUPINFOEX si;

	PROCESS_INFORMATION pi;
};


static int SetUpPseudoConsole(COORD size, libvt_pseudo_console *console)
{
	HRESULT hr = S_OK;

	// Create communication channels

	// - Close these after CreateProcess of child application with pseudoconsole object.
	HANDLE inputReadSide, outputWriteSide;

	// - Hold onto these and use them for communication with the child through the pseudoconsole.
	HANDLE outputReadSide, inputWriteSide;

	if (!CreatePipe(&inputReadSide, &inputWriteSide, NULL, 0))
	{
		return VTCODE_FAILED;
	}

	if (!CreatePipe(&outputReadSide, &outputWriteSide, NULL, 0))
	{
		return VTCODE_FAILED;
	}

	HPCON hPC;
	hr = CreatePseudoConsole(size, inputReadSide, outputWriteSide, 0, &hPC);
	if (FAILED(hr))
	{
		return hr;
	}

	// ...

	console->inputReadSide = inputReadSide;
	console->outputWriteSide = outputWriteSide;
	console->outputReadSide = outputReadSide;
	console->inputWriteSide = inputWriteSide;
	console->hPC = hPC;

	return VTCODE_SUCCESS;
}

static int PrepareStartupInformation(libvt_pseudo_console *console)
{
	STARTUPINFOEX *si = &console->si;
	// Prepare Startup Information structure
	ZeroMemory(si, sizeof(STARTUPINFOEX));
	si->StartupInfo.cb = sizeof(STARTUPINFOEX);

	// Discover the size required for the list
	size_t bytesRequired;
	InitializeProcThreadAttributeList(NULL, 1, 0, &bytesRequired);

	// Allocate memory to represent the list
	si->lpAttributeList = (PPROC_THREAD_ATTRIBUTE_LIST)calloc(1, bytesRequired);
	if (!si->lpAttributeList)
	{
		return VTCODE_FAILED;
	}

	// Initialize the list memory location
	if (!InitializeProcThreadAttributeList(si->lpAttributeList, 1, 0, &bytesRequired))
	{
		free(si->lpAttributeList);
		return VTCODE_FAILED;
	}

	// Set the pseudoconsole information into the list
	if (!UpdateProcThreadAttribute(si->lpAttributeList,
		0,
		PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE,
		console->hPC,
		sizeof(console->hPC),
		NULL,
		NULL))
	{
		free(si->lpAttributeList);
		return VTCODE_FAILED;
	}

	return VTCODE_SUCCESS;
}

static int SetUpProcess(COORD size, libvt_pseudo_console *console)
{
	PCWSTR childApplication = L"C:\\windows\\system32\\cmd.exe";

	// Call CreateProcess
	if (!CreateProcessW(NULL,
		childApplication,
		NULL,
		NULL,
		FALSE,
		EXTENDED_STARTUPINFO_PRESENT | CREATE_UNICODE_ENVIRONMENT,
		NULL,
		NULL,
		&console->si.StartupInfo,
		&console->pi))
	{
		int error = GetLastError();
		return VTCODE_FAILED;
	}

	return VTCODE_SUCCESS;
}


int libvt_create_pseudo_console(libvt_pseudo_console **pconsole)
{
	libvt_pseudo_console *console = (libvt_pseudo_console *)calloc(1, sizeof(libvt_pseudo_console));

	COORD size = { .X = 80, .Y = 24 };
	int code = SetUpPseudoConsole(size, console);
	if (code != VTCODE_SUCCESS)
	{
		libvt_free_pseudo_console(console);
		return VTCODE_FAILED;
	}

	if ((code = PrepareStartupInformation(console)) != VTCODE_SUCCESS)
	{
		libvt_free_pseudo_console(console);
		return VTCODE_FAILED;
	}

	if ((code = SetUpProcess(size, console)) != VTCODE_SUCCESS)
	{
		libvt_free_pseudo_console(console);
		return VTCODE_FAILED;
	}

	*pconsole = console;

	return VTCODE_SUCCESS;
}

void libvt_free_pseudo_console(libvt_pseudo_console *console)
{
	if (console->hPC)
	{
		ClosePseudoConsole(console->hPC);
	}

	if (console->inputReadSide)
	{
		CloseHandle(console->inputReadSide);
	}

	if (console->inputWriteSide)
	{
		CloseHandle(console->inputWriteSide);
	}

	if (console->outputReadSide)
	{
		CloseHandle(console->outputReadSide);
	}

	if (console->outputWriteSide)
	{
		CloseHandle(console->outputWriteSide);
	}

	if (console->si.lpAttributeList)
	{
		free(console->si.lpAttributeList);
	}

	free(console);
}

int libvt_read_pseudo_console(libvt_pseudo_console *console)
{}

int libvt_write_pseudo_console(libvt_pseudo_console *console)
{}


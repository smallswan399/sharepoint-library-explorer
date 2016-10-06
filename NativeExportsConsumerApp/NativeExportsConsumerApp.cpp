// NativeExportsConsumerApp.cpp : Defines the entry point for the console application.
//
#include "stdafx.h"

extern "C"
{
	__declspec(dllimport) void config(int key, int handle);
}

#ifdef _DEBUG
#pragma comment(lib, "../Main/bin/Debug/lcp_dmsstexp.lib")
#else
#pragma comment(lib, "../Main/bin/Release/lcp_dmsstexp.lib")
#endif

int _tmain(int argc, _TCHAR* argv[])
{
	//NE_Rainbow();
	//configtest();
	config(123, 0);
	return 0;
}

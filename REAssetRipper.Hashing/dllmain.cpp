// dllmain.cpp : Define el punto de entrada de la aplicación DLL.
#include "pch.h"
#include "MurmurHash3.h"
#include "Hashing.h"

#define EXPORTED_METHOD extern "C" __declspec(dllexport)

unsigned int ConvertFilePathToMurmurHash(char* filePath, bool lower, bool changeCase, bool keepNullTerminator)
{
	//We'll need a UTF-16 variant of the filepath for this
	unsigned int wideLength = (unsigned int)((strlen(filePath) + 1) * 2);
	char* wideString = new char[wideLength];
	memset(wideString, 0, wideLength);
	unsigned int oldPos = 0, newPos = 0;
	while (1)
	{
		if (filePath[oldPos] == '\\') filePath[oldPos] = '/';
		if (changeCase)
		{
			if (lower) wideString[newPos] = tolower(filePath[oldPos]);
			else wideString[newPos] = toupper(filePath[oldPos]);
		}
		else
			wideString[newPos] = filePath[oldPos];
		wideString[newPos + 1] = 0;
		if (filePath[oldPos] == 0)
			break;
		newPos += 2;
		oldPos++;
	}
	unsigned int murmurHash = 0;
	if (keepNullTerminator == 0)
		MurmurHash3_x86_32(wideString, wideLength - 2, -1, &murmurHash);
	else
		MurmurHash3_x86_32(wideString, wideLength, -1, &murmurHash);
	delete[]wideString;
	return murmurHash;
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}


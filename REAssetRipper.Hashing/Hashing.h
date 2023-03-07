#pragma once

#ifdef MYFUNCTIONS_EXPORTS
#define MYFUNCTIONS_API __declspec(dllexport)
#else
#define MYFUNCTIONS_API __declspec(dllimport)
#endif

MYFUNCTIONS_API unsigned int ConvertFilePathToMurmurHash(char* filePath, bool lower, bool changeCase, bool keepNullTerminator);
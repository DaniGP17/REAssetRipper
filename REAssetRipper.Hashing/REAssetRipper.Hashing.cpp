// REAssetRipper.Hashing.cpp : Este archivo contiene la función "main". La ejecución del programa comienza y termina ahí.
//

#include <iostream>
#include "murmurhash3.h"

unsigned int ConvertFilePathToMurmurHash(char* filePath, bool lower = 1, bool changeCase = 1, bool keepNullTerminator = 0)
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

int main()
{
	//lower 4194362085
	//upper 843311338
	char *hashName = ReturnFilenameFromHashList(4194362085, 843311338);
    std::cout << hashName;
}

// Ejecutar programa: Ctrl + F5 o menú Depurar > Iniciar sin depurar
// Depurar programa: F5 o menú Depurar > Iniciar depuración

// Sugerencias para primeros pasos: 1. Use la ventana del Explorador de soluciones para agregar y administrar archivos
//   2. Use la ventana de Team Explorer para conectar con el control de código fuente
//   3. Use la ventana de salida para ver la salida de compilación y otros mensajes
//   4. Use la ventana Lista de errores para ver los errores
//   5. Vaya a Proyecto > Agregar nuevo elemento para crear nuevos archivos de código, o a Proyecto > Agregar elemento existente para agregar archivos de código existentes al proyecto
//   6. En el futuro, para volver a abrir este proyecto, vaya a Archivo > Abrir > Proyecto y seleccione el archivo .sln

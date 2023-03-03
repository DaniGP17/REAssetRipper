using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAssetRipper.Core.Logs
{
    public static class Log
    {
        private static bool isFileCreated = false;
        private static StreamWriter writer;

        private static void CreateLogFile(string firstLog)
        {
            isFileCreated = true;
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Crear una carpeta para los archivos de registro
            string logFolderPath = Path.Combine(appDataPath, "Logs");
            Directory.CreateDirectory(logFolderPath);

            // Crear el nombre del archivo de registro basado en la fecha y hora actual
            string logFileName = $"log_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log";
            string logFilePath = Path.Combine(logFolderPath, logFileName);

            // Crear el archivo de registro con el contenido inicial
            writer = new StreamWriter(logFilePath);
            writer.WriteLine(firstLog);
        }

        public static void InsertNewLog(string logText)
        {
            if(isFileCreated){
                CreateLogFile(logText);
                return;
            }
            writer.WriteLine(logText);
        }
    }
}

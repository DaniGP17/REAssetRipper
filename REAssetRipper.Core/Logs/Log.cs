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
        private static string logPath = null;
        private static StreamWriter writer;

        private static void CreateLogFile(string firstLog)
        {
            isFileCreated = true;
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string logFolderPath = Path.Combine(appDataPath, "Logs");
            Directory.CreateDirectory(logFolderPath);

            string logFileName = $"log_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log";
            string logFilePath = Path.Combine(logFolderPath, logFileName);
            logPath = logFilePath;
            using (StreamWriter writer = new StreamWriter(logFilePath))
            {
                writer.WriteLine(firstLog);
            }
        }

        public static void InsertNewLog(string logText)
        {
            Console.WriteLine(logText);
            if(!isFileCreated){
                CreateLogFile(logText);
                return;
            }
            using (StreamWriter writer = File.AppendText(logPath))
            {
                writer.WriteLine(logText);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace REAssetRipper.Core.Handlers
{
	public static class List
	{
        public static Dictionary<string, string> hashList = new Dictionary<string, string>();
		public static void ReadList()
		{
            string fileName = "re7.list";
            string executablePath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(executablePath, "HashList", fileName);
            StreamReader reader = new StreamReader(filePath);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                int spaceIndex = line.IndexOf(' ');
                string hash = line.Substring(0, spaceIndex);
                string path = line.Substring(spaceIndex + 1);
                hashList.Add(hash, path);
            }

            reader.Close();
        }

        public static string GetNameFromHash(string hash)
        {
            if (hashList.Count == 0)
            {
                ReadList();
            }
            string path;
            if (hashList.TryGetValue(hash, out path))
            {
                return path;
            }
            return "Undefined-" + hash;
        }
	}
}


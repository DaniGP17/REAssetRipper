using REAssetRipper.Core.Handlers;
using REAssetRipper.Core.Constants;
using System;
using System.Text.RegularExpressions;
using REAssetRipper.Core.Logs;
using System.IO;
using PhilLibX.IO;
using PhilLibX.Imaging;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Server;
using System.Drawing;
using static REAssetRipper.Core.Constants.Structures;

namespace REAssetRipper.Testing
{
    public class Program
    {
        private static REAssetRipper.Core.Handlers.Pak pak;
        static void Main(string[] args)
        {
            pak = new REAssetRipper.Core.Handlers.Pak("\\\\Mac\\Home\\Desktop\\re_chunk_000.pak");
            List<Asset> results = new List<Asset>(pak.PakAssets.Count);
            REAssetRipper.Core.Handlers.List.ReadList();
            int asd = 0;
            string lastName = "";

            foreach (var entry in pak.PakAssets)
            {
                string path = List.GetNameFromHash(entry.Value.LowerCaseHash.ToString());
                AssetTypes.types type = AssetClasificator.GetType(path);

                //Console.WriteLine(type + "   " + path);
                if (AssetTypes.types.Texture == type && path.Contains("props"))
                {
                    Console.WriteLine(path);
                    int ultimoSeparador = path.LastIndexOf('/');
                    string fileName = path.Substring(ultimoSeparador + 1);
                    if (fileName != lastName)
                    {
                        asd++;
                        //ExtractAsset(entry.Value, "\\\\Mac\\Home\\Desktop\\" + fileName);
                        File.WriteAllBytes("\\\\Mac\\Home\\Desktop\\" + fileName, pak.LoadAsset(entry.Value));
                        lastName = fileName;
                    }
                }
                if (asd > 5)
                    break;
            }
            Console.WriteLine("Finished" + asd.ToString());
        }

        private static void ExtractAsset(PakAssets asset, string path)
        {
            AssetTypes.types type = AssetClasificator.GetType(path);
            switch (type)
            {
                case AssetTypes.types.Texture:
                    var image = Texture.Convert(pak.LoadAsset(asset));
                    if(REAssetRipper.Core.Helpers.Settings.textureExport != TextureTypes.exportTypes.dds)
                        image.ConvertImage(ScratchImage.DXGIFormat.R8G8B8A8UNORM);

                    image.Save(path + "." + REAssetRipper.Core.Helpers.Settings.textureExport.ToString());
                    break;
            }
        }
    }
}

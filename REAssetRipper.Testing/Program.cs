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
using System.Diagnostics;
using System.Xml.Linq;

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
            int lastid = 0;

            foreach (var entry in pak.PakAssets)
            {
                string path = List.GetNameFromAsset(entry.Value);
                if (AssetClasificator.GetType(path) == AssetTypes.types.Texture && path.Contains(".mdf."))
                {
                    ExtractAsset(entry.Value, "\\\\Mac\\Home\\Desktop\\test.mdf.8");
                    //File.WriteAllBytes("\\\\Mac\\Home\\Desktop\\sm2080_cardkey01a.tex.8", pak.LoadAsset(entry.Value));
                    Debug.WriteLine(entry.Value.DecompressedSize + "  " + entry.Value.CompressedSize);
                    break;
                }
            }
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

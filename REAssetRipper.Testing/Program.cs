using REAssetRipper.Core.Handlers;
using REAssetRipper.Core.Constants;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Text.RegularExpressions;
using REAssetRipper.Core.Logs;
using System.IO;
using SixLabors.ImageSharp;
using PhilLibX.IO;
using PhilLibX.Imaging;

Pak pak = new Pak("/Users/danielgallegopinilla/Desktop/re_chunk_000.pak");
List<Asset> results = new List<Asset>(pak.PakAssets.Count);
List.ReadList();
int asd = 0;
string lastName = "";
foreach (var entry in pak.PakAssets)
{
    string path = List.GetNameFromHash(entry.Value.LowerCaseHash.ToString()) + "\n";
    AssetTypes.types type = AssetClasificator.GetType(path);

    //Console.WriteLine(type + "   " + name);
    if (AssetTypes.types.Texture == type && path.Contains("sm2451_smallbottle01a_a_a"))
    {
        Console.WriteLine(path);
        string exportName = Regex.Replace(Path.GetFileNameWithoutExtension(path), @"\.\d+$", "");
        if (exportName != lastName)
        {
            asd++;
            byte[] imageData = pak.LoadAsset(entry.Value);
            using (var image = Texture.Convert(pak.LoadAsset(entry.Value)))
            {
                image.ConvertImage(ScratchImage.DXGIFormat.R8G8B8A8UNORM);
            }
            /*Texture texture = new Texture(imageData, path, exportName);
            lastName = exportName;*/
            break;
        }
    }
}

Console.WriteLine("Finished" + asd.ToString());
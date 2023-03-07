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
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace REAssetRipper.Testing
{
    public class Program
    {
        private static REAssetRipper.Core.Handlers.Pak pak;
        static void Main(string[] args)
        {
            pak = new REAssetRipper.Core.Handlers.Pak("D:\\Games\\Resident Evil VII Biohazard\\re_chunk_000.pak");
            List<Asset> results = new List<Asset>(pak.PakAssets.Count);
            REAssetRipper.Core.Handlers.List.ReadList();

            foreach (var entry in pak.PakAssets)
            {
                string path = List.GetNameFromAsset(entry.Value);
                if (AssetClasificator.GetType(path) == AssetTypes.types.AudioBnk)
                {
                    Console.WriteLine(path);
                    byte[] bnkBytes = pak.LoadAsset(entry.Value);
                    BNKHeaderRE7 bnkHeader;
                    MemoryStream memoryStream = new MemoryStream(bnkBytes);
                    BinaryReader binaryReader = new BinaryReader(memoryStream);
                    bnkHeader = binaryReader.ReadStruct<BNKHeaderRE7>();
                    byte[] magicBytes = BitConverter.GetBytes(bnkHeader.Magic);
                    int magicInt = BitConverter.ToInt32(magicBytes, 0);
                    string magicStr = new string(new[] { (char)(magicInt & 0xff), (char)((magicInt >> 8) & 0xff), (char)((magicInt >> 16) & 0xff), (char)((magicInt >> 24) & 0xff) });
                    if (magicStr != "BKHD")
                        return;

                    binaryReader.BaseStream.Seek(bnkHeader.TableOffset, SeekOrigin.Begin);
                    BNKIndexEntryRE7[] sounds = new BNKIndexEntryRE7[bnkHeader.TableCount];
                    for (int i = 0; i < bnkHeader.TableCount; i++)
                    {
                        sounds[i].Offset = binaryReader.ReadInt32();
                        sounds[i].Size = binaryReader.ReadInt32();
                        sounds[i].Flags = binaryReader.ReadInt32();

                        int nameLength = binaryReader.ReadInt32();
                        sounds[i].Name = Encoding.ASCII.GetString(binaryReader.ReadBytes(nameLength));
                    }

                    // Imprimir nombres de archivo de sonido
                    Console.WriteLine("Archivos de sonido encontrados en el archivo .bnk:");
                    foreach (BNKIndexEntryRE7 sound in sounds)
                    {
                        Console.WriteLine(sound.Name);
                    }

                    // Cerrar archivo y liberar recursos
                    binaryReader.Close();
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
                    if (REAssetRipper.Core.Helpers.Settings.textureExport != TextureTypes.exportTypes.dds)
                        image.ConvertImage(ScratchImage.DXGIFormat.R8G8B8A8UNORM);

                    image.Save(path + "." + REAssetRipper.Core.Helpers.Settings.textureExport.ToString());
                    break;
            }
        }
    }
}

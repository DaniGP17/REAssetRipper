using REAssetRipper.Core.Constants;
using REAssetRipper.Core.Logs;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace REAssetRipper.Core.Handlers
{
    public class Pak
    {
        public Dictionary<uint, Structures.PakAssets> PakAssets { get; set; }

        private BinaryReader br;

        public Pak(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    this.br = br;
                    var magic = br.ReadUInt32();
                    var version = br.ReadUInt32();
                    var count = br.ReadInt32();
                    var checksum = br.ReadUInt32();

                    if(!Magics.pak.Any(x => x == magic))
                    {
                        ErrorHandler.NewError(Errors.Types.InvalidMagicNumber, new string[] {"Probably invalid pak file", "Probably pak file is corrupted"});
                        return;
                    }

                    Log.InsertNewLog("Magic number is valid");

                    if (!Versions.pak.Any(x => x == version))
                    {
                        ErrorHandler.NewError(Errors.Types.InvalidVersionNumber);
                        return;
                    }

                    Log.InsertNewLog("Version number is valid");

                    PakAssets = new Dictionary<uint, Structures.PakAssets>();

                    for (int i = 0; i < count; i++)
                    {
                        Structures.PakAssets asset = new Structures.PakAssets();
                        asset.LowerCaseHash = br.ReadUInt32();
                        asset.UpperCaseHash = br.ReadUInt32();
                        asset.Offset = br.ReadInt64();
                        asset.CompressedSize = br.ReadInt64();
                        asset.DecompressedSize = br.ReadInt64();
                        asset.Flags = br.ReadBytes(8);
                        asset.Checksum = br.ReadInt64();

                        PakAssets[asset.LowerCaseHash] = asset;
                    }
                }
            }
        }
    }
}

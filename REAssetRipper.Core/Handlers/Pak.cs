using REAssetRipper.Core.Constants;
using REAssetRipper.Core.Logs;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using zlib;
using Zstandard;
using Zstandard.Net;
using PhilLibX.IO;

namespace REAssetRipper.Core.Handlers
{
    public class Pak
    {
        public Dictionary<uint, Structures.PakAssets> PakAssets { get; set; }

        private BinaryReader br;

        public Pak(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            br = new BinaryReader(fs);
            var magic = br.ReadUInt32();
            var version = br.ReadUInt32();
            var count = br.ReadInt32();
            var checksum = br.ReadUInt32();

            if (!Magics.pak.Any(x => x == magic))
            {
                ErrorHandler.NewError(Errors.Types.InvalidMagicNumber, new string[] {
                    "Probably invalid pak file",
                    "Probably pak file is corrupted"
                });
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

            var entries = br.ReadArray<Structures.PakAssets>(count);
            foreach (var entry in entries)
                PakAssets[entry.LowerCaseHash] = entry;

            /*for (int i = 0; i < count; i++)
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
            }*/
        }

        public byte[] LoadAsset(Structures.PakAssets asset)
        {
            lock (this.br)
            {
                br.BaseStream.Position = asset.Offset;

                if ((asset.Flags[0] & 0x0F) == 0)
                {
                    Log.InsertNewLog("A new entry has been loaded by BinaryReader: " + asset.LowerCaseHash);
                    return br.ReadBytes((int)asset.CompressedSize);
                }
                else if ((asset.Flags[0] & 0x0F) == 1)
                {
                    using (var compressedStream = new MemoryStream(br.ReadBytes((int)asset.CompressedSize)))
                    using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                    using (var outputStream = new MemoryStream())
                    {
                        deflateStream.CopyTo(outputStream);
                        Log.InsertNewLog("A new entry has been loaded with Zlib: " + asset.LowerCaseHash);
                        return outputStream.ToArray();
                    }
                }
                else if ((asset.Flags[0] & 0x0F) == 2)
                {
                    using (var compressedStream = new MemoryStream(br.ReadBytes((int)asset.CompressedSize)))
                    using (var zstdStream = new ZstandardStream(compressedStream, CompressionMode.Decompress))
                    using (var outputStream = new MemoryStream())
                    {
                        zstdStream.CopyTo(outputStream);
                        Log.InsertNewLog("A new entry has been loaded with ZStandard lib: " + asset.LowerCaseHash);
                        return outputStream.ToArray();
                    }
                }
                else
                {
                    ErrorHandler.NewError(Errors.Types.InvalidEntryCompression);
                    return null;
                }
            }
        }
    }
}

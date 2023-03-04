using System;
using REAssetRipper.Core.Constants;

namespace REAssetRipper.Core.Handlers
{
	public class Asset
	{
        public string Name { get; set; }

        public string Type { get; set; }

        public string DisplaySize
        {
            get
            {
                return ByteSize.FromBytes(PackageEntry.CompressedSize).ToString();
            }
        }

        public Structures.PakAssets PackageEntry { get; set; }

        public Asset(string name, string type, Structures.PakAssets entry)
        {
            Name = name;
            Type = type;
            PackageEntry = entry;
        }
    }
}


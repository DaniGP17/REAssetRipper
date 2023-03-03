using REAssetRipper.Core.Handlers;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

Pak pak = new Pak("/Users/danielgallegopinilla/Desktop/re_chunk_000.pak");
List<Asset> results = new List<Asset>(pak.PakAssets.Count);
List.ReadList();
foreach (var entry in pak.PakAssets)
{
    Console.Write(List.GetNameFromHash(entry.Value.LowerCaseHash.ToString()) + "\n");
}
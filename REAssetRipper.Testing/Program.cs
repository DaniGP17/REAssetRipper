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

foreach (var entry in pak.PakAssets)
{
    Console.Write(entry.Value.LowerCaseHash + "  " + entry.Value.UpperCaseHash + "\n");
}
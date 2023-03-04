using System;

namespace REAssetRipper.Core.Handlers
{
    public static class ByteSize
    {
        static string[] SizeSuffixes = { "Bytes", "KB", "MB", "GB"};

        public static string FromBytes(long bytes)
        {
            if (bytes == 0)
            {
                return "0 Bytes";
            }

            var magnitude = (int)Math.Floor(Math.Log(bytes, 1024));
            var value = (double)bytes / Math.Pow(1024, magnitude);
            var suffix = SizeSuffixes[magnitude];

            return $"{value:##.##} {suffix}";
        }
    }
}


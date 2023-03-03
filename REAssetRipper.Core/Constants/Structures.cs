﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace REAssetRipper.Core.Constants
{
    public static class Structures
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Pak
        {
            public UInt32 magic { get; set; }
            public UInt32 version { get; set; }
            public Int32 count { get; set; }
            public UInt32 checkSum { get; set; }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PakAssets
        {
            public uint LowerCaseHash { get; set; }

            public uint UpperCaseHash { get; set; }

            public long Offset { get; set; }

            public long CompressedSize { get; set; }

            public long DecompressedSize { get; set; }

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x8)]
            public byte[] Flags;

            public long Checksum { get; set; }
        }
    }
}
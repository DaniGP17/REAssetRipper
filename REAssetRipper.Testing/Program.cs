using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using PhilLibX.Mathematics;
using PhilLibX.IO;
using PhilLibX;
using System.Text;

namespace REAssetRipper.Testing
{
    public class Program
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct MeshHeaderRE7
        {
            public uint Magic;
            public uint Version;
            public int FileSize;
            public ushort Unk;
            public ushort StringCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public long[] ModelPointers;
            public long BoneDataHeaderPointer;
            public long UnkPointer01;
            public long UnkPointer02;
            public long GeometryPointer;
            public long UnkPointer03;
            public long MaterialNamesPointer;
            public long BoneNamesPointer;
            public long UnkPointer04;
            public long StringTablePointer;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct BoneDataHeaderRE7
        {
            public short BoneCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public byte[] Padding;
            public long BoneTablePointer;
            public long MatricesPointer;
            public long UnkMatricesPointer01;
            public long UnkMatricesPointer02;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
        internal struct BoneDataRE7
        {
            public short BoneIndex;
            public short ParentIndex;
            public short NextSibling;
            public short FirstChild;
            public short Unk; // Sometimes == BoneIndex, Sometimes -1
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct ModelHeaderRE7
        {
            public byte LODCount;
            public byte MaterialCount;
            public byte UVCount;
            public byte Unk01;
            public int Unk02;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct LODHeaderRE7
        {
            public ushort MeshCount;
            public byte Flags;
            public byte BoneCount;
            public float LODDistance;
            public long MeshesPointer;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct LODMeshRE7
        {
            public byte MeshIndex;
            public byte SubMeshCount;
            public byte Unk01;
            public byte Unk02;
            public int Unk03;
            public int VertexCount;
            public int FaceCount;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct LODSubMeshRE7
        {
            public int MaterialIndex;
            public int FaceCount;
            public int FaceIndex;
            public int VertexIndex;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct GeometryHeaderRE7
        {
            public uint VertexBufferSize;
            public long UnkPointer;
            public int Padding;
            public int FaceBufferSize;
            public long UnkPointer01;
            public int Padding01;
            public long Unk01;
            public long FaceBufferOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct Matrix4x4
        {
            public Vector4 X;
            public Vector4 Y;
            public Vector4 Z;
            public Vector4 W;

           
        }


        private static REAssetRipper.Core.Handlers.Pak pak;
        public static void MurmurHash3_x86_32(byte[] key, int len, uint seed, out uint result)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            uint h1 = seed;
            int nblocks = len / 4;

            // body
            for (int i = 0; i < nblocks; i++)
            {
                uint k1 = BitConverter.ToUInt32(key, i * 4);

                k1 *= c1;
                k1 = rotl32(k1, 15);
                k1 *= c2;

                h1 ^= k1;
                h1 = rotl32(h1, 13);
                h1 = h1 * 5 + 0xe6546b64;
            }

            // tail
            uint k2 = 0;
            int tailIndex = nblocks * 4;
            switch (len & 3)
            {
                case 3:
                    k2 ^= (uint)key[tailIndex + 2] << 16;
                    goto case 2;
                case 2:
                    k2 ^= (uint)key[tailIndex + 1] << 8;
                    goto case 1;
                case 1:
                    k2 ^= key[tailIndex];
                    k2 *= c1;
                    k2 = rotl32(k2, 15);
                    k2 *= c2;
                    h1 ^= k2;
                    break;
            }

            // finalization
            h1 ^= (uint)len;

            // fmix32
            h1 ^= h1 >> 16;
            h1 *= 0x85ebca6b;
            h1 ^= h1 >> 13;
            h1 *= 0xc2b2ae35;
            h1 ^= h1 >> 16;

            result = h1;
        }

        private static uint rotl32(uint x, byte r)
        {
            return (x << r) | (x >> (32 - r));
        }

        public static uint ConvertFilePathToMurmurHash(string filePath, bool lower, bool changeCase, bool keepNullTerminator)
        {
            // We'll need a UTF-16 variant of the filepath for this
            filePath = filePath.Replace('\\', '/');
            if (changeCase)
            {
                if (lower) filePath = filePath.ToLower();
                else filePath = filePath.ToUpper();
            }

            byte[] wideString = Encoding.Unicode.GetBytes(filePath);
            uint murmurHash = 0;
            if (keepNullTerminator == false)
                MurmurHash3_x86_32(wideString, wideString.Length - 2, 0xFFFFFFFF, out murmurHash);
            else
                MurmurHash3_x86_32(wideString, wideString.Length, 0xFFFFFFFF, out murmurHash);

            return murmurHash;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(ConvertFilePathToMurmurHash("natives/x64/aaa/ambassadortrial_tu/animation/player/pl0000/motfsm/pl0000_interact_lookmole_v2.motfsm.17", true, true, false));
        }


        /*private static void ExtractAsset(PakAssets asset, string path)
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
        }*/
    }
}

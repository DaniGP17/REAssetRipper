using System;
using REAssetRipper.Core.Constants;
using REAssetRipper.Core.Helpers;
using static REAssetRipper.Core.Constants.Structures;
using PhilLibX.Imaging;
using PhilLibX.IO;
using PhilLibX;
using PhilLibX.Compression;

namespace REAssetRipper.Core.Handlers
{
	public class Texture
	{
        public static ScratchImage Convert(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return Convert(stream);
            }
        }

        public static ScratchImage Convert(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                // Only grab the highest mip
                var header = reader.ReadStruct<TextureHeaderRE7>();
                var mip = reader.ReadArray<MipMapRE7>(header.MipMapCount).OrderBy(x => x.Size).ToArray()[header.MipMapCount - 1];

                reader.BaseStream.Position = mip.Offset;

                // Switch SRGB
                switch (header.Format)
                {
                    case ScratchImage.DXGIFormat.BC7UNORMSRGB:
                        header.Format = ScratchImage.DXGIFormat.BC7UNORM;
                        break;
                    case ScratchImage.DXGIFormat.BC3UNORMSRGB:
                        header.Format = ScratchImage.DXGIFormat.BC3UNORM;
                        break;
                    case ScratchImage.DXGIFormat.BC2UNORMSRGB:
                        header.Format = ScratchImage.DXGIFormat.BC2UNORM;
                        break;
                    case ScratchImage.DXGIFormat.BC1UNORMSRGB:
                        header.Format = ScratchImage.DXGIFormat.BC1UNORM;
                        break;
                }

                Console.WriteLine("Convertidor");

                return new ScratchImage(new ScratchImage.Metadata()
                {
                    Width = header.Width,
                    Height = header.Height,
                    Depth = 1,
                    ArraySize = 1,
                    MiscFlags = ScratchImage.TexMiscFlags.NONE,
                    MiscFlags2 = ScratchImage.TexMiscFlags2.NONE,
                    Dimension = ScratchImage.TexDimension.TEXTURE2D,
                    MipLevels = 1,
                    Format = header.Format
                }, reader.ReadBytes(mip.Size));
            }
        }

        /*public Texture(byte[] imageData, string path, string name)
		{
            MemoryStream stream = new MemoryStream(imageData);
            BinaryReader reader = new BinaryReader(stream);

            Structures.TextureHeaderRE7 asset = new Structures.TextureHeaderRE7();
            asset.Magic = reader.ReadInt32();
            asset.Version = reader.ReadInt32();
            asset.Width = reader.ReadUInt16();
            asset.Height = reader.ReadUInt16();
            asset.Depth = reader.ReadUInt16();
            asset.MipMapCount = reader.ReadByte();
            asset.Unknown1 = reader.ReadByte();
            asset.Format = reader.ReadInt32();
            asset.Unknown2 = reader.ReadUInt32();
            asset.Padding = reader.ReadInt64();

            List<uint[]> mipData = new List<uint[]>();
            for (int i = 0; i < asset.MipMapCount; i++)
            {
                mipData.Add(new uint[] { reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32() });
            }

            reader.BaseStream.Seek(mipData[0][0], SeekOrigin.Begin);
            byte[] texData = reader.ReadBytes((int)mipData[0][3]);
            TextureTypes.types textType;

            if(Settings.textureExport != TextureTypes.exportTypes.dds) {
                switch (asset.Format)
                {
                    case 0x1C: //
                        textType = TextureTypes.types.ATI1;
                        break;

                    case 0x1D: // PCX (Personal Computer eXchange)
                        textType = TextureTypes.types.BC1;
                        break;

                    case 0x47: // GIF (Graphics Interchange Format)
                        textType = TextureTypes.types.DXT1;
                        break;

                    case 0x48: // HDR (High Dynamic Range)
                        textType = TextureTypes.types.DXT1;
                        break;

                    case 0x50: // PNG (Portable Network Graphics)
                        textType = TextureTypes.types.BC4;
                        break;

                    case 0x5F: // DDS (DirectDraw Surface)
                        textType = TextureTypes.types.BC6H;
                        break;

                    case 0x62: // JPEG 2000
                        textType = TextureTypes.types.BC7;
                        break;
                    case 0x63: // JPEG
                        textType = TextureTypes.types.BC7;
                        break;
                    default:
                        ErrorHandler.NewError(Errors.Types.CantFindTextureFormat, new string[] {
                        "Finded format is: " + asset.Format + " , from asset called: " + path
                    });
                        break;
                }
                return;
            }
            File.WriteAllBytes("/Users/danielgallegopinilla/Desktop/" + name, imageData);
        }*/
	}
}


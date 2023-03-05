using System;
using REAssetRipper.Core.Constants;
using REAssetRipper.Core.Helpers;
using static REAssetRipper.Core.Constants.Structures;
using PhilLibX.Imaging;
using PhilLibX.IO;
using PhilLibX;
using PhilLibX.Compression;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace REAssetRipper.Core.Handlers
{
    public class Texture
    {
        public static ScratchImage Convert(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            var reader = new BinaryReader(stream);
            var header = reader.ReadStruct<TextureHeaderRE7>();
            var mip = reader.ReadArray<MipMapRE7>(header.MipMapCount).OrderBy(x => x.Size).ToArray()[header.MipMapCount - 1];

            reader.BaseStream.Position = mip.Offset;

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
}
using System;
using REAssetRipper.Core.Constants;
namespace REAssetRipper.Core.Handlers
{
	public class AssetClasificator
	{
		public static AssetTypes.types GetType(string name)
		{
            if (name.Contains(".tex."))
            {
                return AssetTypes.types.Texture;
            }
            else if (name.Contains(".mesh."))
            {
                return AssetTypes.types.Mesh;
            }
            else if (name.Contains(".motlist."))
            {
                return AssetTypes.types.AudioBnk;
            }
            else if (name.Contains(".mot."))
            {
                return AssetTypes.types.Motion;
            }
            else if (name.Contains(".bnk."))
            {
                return AssetTypes.types.AudioBnk;
            }
            else if (name.Contains(".pck."))
            {
                return AssetTypes.types.AudioPck;
            }
            else if (name.Contains(".scn."))
            {
                return AssetTypes.types.Scene;
            }
            else if (name.Contains(".pfb."))
            {
                return AssetTypes.types.Prefab;
            }
            else if (name.Contains(".gui."))
            {
                return AssetTypes.types.Gui;
            }
            else
            {
                return AssetTypes.types.Unknown;
            }
        }
	}
}


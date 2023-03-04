using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAssetRipper.Core.Constants
{
    public static class AssetTypes
    {
        public enum types
        {
            Mesh,
            Texture,
            Material,
            MotionList,
            Motion,
            Scene,
            Prefab,
            AudioBnk,
            AudioPck,
            Gui,
            Unknown
        }

        public static types GetType(string name)
        {
            if (name.Contains(".tex."))
            {
                return types.Texture;
            }
            else if (name.Contains(".mesh."))
            {
                return types.Mesh;
            }
            else if (name.Contains(".motlist."))
            {
                return types.AudioBnk;
            }
            else if (name.Contains(".mot."))
            {
                return types.Motion;
            }
            else if (name.Contains(".bnk."))
            {
                return types.AudioBnk;
            }
            else if (name.Contains(".pck."))
            {
                return types.AudioPck;
            }
            else if (name.Contains(".scn."))
            {
                return types.Scene;
            }
            else if (name.Contains(".pfb."))
            {
                return types.Prefab;
            }
            else if (name.Contains(".gui."))
            {
                return types.Gui;
            }
            else
            {
                return types.Unknown;
            }
        }
    }
}

using REAssetRipper.Core.Handlers;
using REAssetRipper.Core.Constants;
using PhilLibX.Imaging;
using System.IO;
using static REAssetRipper.Core.Constants.Structures;
using System;
using System.CodeDom;
using System.Text;
using PhilLibX;
using System.Runtime.Remoting.Messaging;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace REAssetRipper.Testing
{
    public class GameObjectInfoStruct
    {
        public byte[] bytesGuid;
        public int objectID;
        public int parentID;
        public ushort componentCount;
        public short ukn;
        public int prefabID;
        public GameObjectInfoStruct(byte[] bytesGuid, int objectID, int parentID, ushort componentCount, short ukn, int prefabID)
        {
            this.bytesGuid = bytesGuid;
            this.objectID = objectID;
            this.parentID = parentID;
            this.componentCount = componentCount;
            this.ukn = ukn;
            this.prefabID = prefabID;
        }
    }

    public class FolderInfoStruct
    {
        public int objectID;
        public int parentID;
        public FolderInfoStruct(int objectID, int parentID)
        {
            this.objectID = objectID;
            this.parentID = parentID;
        }
    }

    public class ObjectParser
    {
        public string Key { get; set; }
        public string Crc { get; set; }
        public List<object> Fields { get; set; }
        public string Name { get; set; }
    }

    public class Program
    {
        private static GameObjectInfoStruct[] gameObjectInfo;
        private static FolderInfoStruct[] folderInfo;
        private static Tuple<string, int>[] resourceInfos;
        private static Tuple<string, UInt32>[] prefabInfos;
        private static UInt32[] objectTable;

        private static REAssetRipper.Core.Handlers.Pak pak;
        static void Main(string[] args)
        {
            FileStream fileStream = new FileStream("E:\\Program Files (x86)\\Steam\\steamapps\\common\\RESIDENT EVIL 7 biohazard\\Retool\\RESteam\\re_chunk_000\\natives\\stm\\environment\\scene\\chapter4\\c04_cottageswamp01.scn.20", FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            uint magic = binaryReader.ReadUInt32();
            int infoCount = binaryReader.ReadInt32();
            int resourceCount = binaryReader.ReadInt32();
            int folderCount = binaryReader.ReadInt32();
            int prefabCount = binaryReader.ReadInt32();
            int userdataCount = binaryReader.ReadInt32();
            UInt64 folderInfoTbl = binaryReader.ReadUInt64();
            UInt64 resourceInfoTbl = binaryReader.ReadUInt64();
            UInt64 prefabInfoTbl = binaryReader.ReadUInt64();
            UInt64 userdataInfoTbl = binaryReader.ReadUInt64();
            UInt64 dataOffset = binaryReader.ReadUInt64();
            Console.WriteLine("magic: {0}", magic);
            Console.WriteLine("infoCount: {0}", infoCount);
            Console.WriteLine("resourceCount: {0}", resourceCount);
            Console.WriteLine("folderCount: {0}", folderCount);
    
            Console.WriteLine("prefabCount: {0}", prefabCount);
            Console.WriteLine("userdataCount: {0}", userdataCount);
            Console.WriteLine("folderInfoTbl: {0}", folderInfoTbl);
            Console.WriteLine("resoureceInfoCount: {0}", resourceInfoTbl);
            Console.WriteLine("prefabInfoTbl: {0}", prefabInfoTbl);
            Console.WriteLine("userdataInfoTbl: {0}", userdataInfoTbl);
            Console.WriteLine("dataOffset: {0}", dataOffset);
            Console.WriteLine("__________________________________________");

            //Read GameObjectInfos
            Console.WriteLine("Reading GameObject Infos");
            gameObjectInfo = new GameObjectInfoStruct[infoCount];
            for (int i = 0; i < infoCount; i++)
            {
                byte[] bytesGuid = binaryReader.ReadBytes(16);
                int objectID = binaryReader.ReadInt32();
                int parentId = binaryReader.ReadInt32();
                ushort componentCount = binaryReader.ReadUInt16();
                short ukn = binaryReader.ReadInt16();
                int prefabID = binaryReader.ReadInt32();
                gameObjectInfo[i] = new GameObjectInfoStruct(bytesGuid, objectID, parentId, componentCount, ukn, prefabID);
            }

            Console.WriteLine("Reading Folder Infos");
            //Read FolderInfos
            folderInfo = new FolderInfoStruct[folderCount];
            for (int i = 0; i < folderCount; i++)
            {
                int objectID = binaryReader.ReadInt32();
                int parentID = binaryReader.ReadInt32();

                folderInfo.Append(new FolderInfoStruct(objectID, parentID));
            }

            //Skip 8 empty bytes
            binaryReader.ReadUInt64();

            Console.WriteLine("Reading Resources Infos");
            resourceInfos = new Tuple<string, int>[resourceCount];
            long currentSeekPosition = binaryReader.BaseStream.Position;
            for (int i = 0; i < resourceCount; i++)
            {
                currentSeekPosition = binaryReader.BaseStream.Position;
                UInt64 strOffset = binaryReader.ReadUInt64();
                binaryReader.BaseStream.Seek((int)strOffset, SeekOrigin.Begin);
                string pathStr = "";
                while (true)
                {
                    int charCodified = binaryReader.ReadInt16();
                    if(charCodified == 0)
                    {
                        break;
                    }
                    char charConverted = Encoding.ASCII.GetChars(new byte[] { (byte)charCodified })[0];
                    pathStr += charConverted;
                }
                resourceInfos[i] = Tuple.Create(pathStr, (int)strOffset);
                binaryReader.BaseStream.Seek(currentSeekPosition + 8, SeekOrigin.Begin);
            }

            //Skip 8 bytes because are empty
            binaryReader.ReadUInt64();

            Console.WriteLine("Reading Prefab Infos");
            prefabInfos = new Tuple<string, UInt32>[prefabCount];
            //Read FolderInfos
            for (int i = 0; i < prefabCount; i++)
            {
                UInt32 strOffset = binaryReader.ReadUInt32();
                int parentID = binaryReader.ReadInt32();

                binaryReader.BaseStream.Seek((int)strOffset, SeekOrigin.Begin);
                string pathStr = "";
                while (true)
                {
                    int charCodified = binaryReader.ReadInt16();
                    if (charCodified == 0)
                    {
                        break;
                    }
                    char charConverted = Encoding.ASCII.GetChars(new byte[] { (byte)charCodified })[0];
                    pathStr += charConverted;
                }
                prefabInfos[i] = Tuple.Create(pathStr, strOffset);
                binaryReader.BaseStream.Seek(currentSeekPosition + 24, SeekOrigin.Begin);
            }


            //Move binary reader position to data
            binaryReader.BaseStream.Seek((long)dataOffset, SeekOrigin.Begin);

            Console.WriteLine("Reading Data Header");
            //Read Data Header
            uint magicData = binaryReader.ReadUInt32();
            uint versionData = binaryReader.ReadUInt32();
            int objectCountData = binaryReader.ReadInt32();
            int instanceCountData = binaryReader.ReadInt32();
            binaryReader.ReadUInt32();
            int userdataCountData = binaryReader.ReadInt32();
            uint instanceOffsetData= binaryReader.ReadUInt32() + 18160;
            uint dataOffsetData = binaryReader.ReadUInt32() + 18160;
            uint userdataOffsetData = binaryReader.ReadUInt32() + 18160;


            //Skip 12 ukn bytes
            binaryReader.ReadBytes(12);


            Console.WriteLine("Reading Object Table");
            objectTable = new UInt32[objectCountData];
            //Read FolderInfos
            for (int i = 0; i < objectCountData; i++)
            {
                objectTable[i] = binaryReader.ReadUInt32();
            }

            //Move binary reader position to instance info data
            binaryReader.BaseStream.Seek((long)instanceOffsetData, SeekOrigin.Begin);

            string parserData = System.IO.File.ReadAllText("DataParser/rszre7rt.json");
            Dictionary<string, ObjectParser> objectParsers = JsonConvert.DeserializeObject<Dictionary<string, ObjectParser>>(parserData);
            /*foreach(KeyValuePair<string, ObjectParser> pair in objectParsers)
            {
                Console.WriteLine("Key: {0}", pair.Key);
                Console.WriteLine("CRC: {0}", pair.Value.Crc);
                Console.WriteLine("Name: {0}", pair.Value.Name);
                Console.WriteLine("Fields:");

                foreach (object field in pair.Value.Fields)
                {
                    Console.WriteLine("- {0}", field);
                }

                Console.WriteLine();
            }*/


            Console.WriteLine("Reading Instance Infos");
            for (int i = 0; i < instanceCountData; i++)
            {
                string instanceHash = binaryReader.ReadUInt32().ToString("X").PadLeft(8, '0').ToLower();
                uint instanceCRC = binaryReader.ReadUInt32();
                /*Console.WriteLine("instanceHash: " + instanceHash);
                Console.WriteLine("instanceCRC: " + instanceCRC);*/
                //Console.WriteLine(instanceHash.ToString("X").PadLeft(8, '0').ToLower());
                if (objectParsers.ContainsKey(instanceHash))
                {
                    //Console.WriteLine(objectParsers[instanceHash].Name);
                }
                else
                {
                    Console.WriteLine("Can't find game object of type: " + instanceHash);
                }
            }
        }


        private static void ExtractAsset(PakAssets asset, string path)
        {
            AssetTypes.types type = AssetClasificator.GetType(path);
            switch (type)
            {
                case AssetTypes.types.Texture:
                    var image = Texture.Convert(pak.LoadAsset(asset));
                    if (REAssetRipper.Core.Helpers.Settings.textureExport != TextureTypes.exportTypes.dds)
                        image.ConvertImage(ScratchImage.DXGIFormat.R8G8B8A8UNORM);

                    image.Save(path + "." + REAssetRipper.Core.Helpers.Settings.textureExport.ToString());
                    break;
            }
        }
    }
}

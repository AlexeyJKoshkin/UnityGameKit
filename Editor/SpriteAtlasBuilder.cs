using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using EditorUtils = GameKit.Editor.EditorUtils;


namespace GameKit
{
    public static class SpriteAtlasBuilder
    {
        private static string yaml = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!687078895 &4343727234628468602
SpriteAtlas:
  m_ObjectHideFlags: 0
  m_PrefabParentObject: {fileID: 0}
  m_PrefabInternal: {fileID: 0}
  m_Name: New Sprite Atlas
  m_EditorData:
    textureSettings:
      serializedVersion: 2
      anisoLevel: 1
      compressionQuality: 50
      maxTextureSize: 2048
      textureCompression: 0
      filterMode: 1
      generateMipMaps: 0
      readable: 1
      crunchedCompression: 0
      sRGB: 1
    platformSettings: []
    packingParameters:
      serializedVersion: 2
      padding: 4
      blockOffset: 1
      allowAlphaSplitting: 0
      enableRotation: 1
      enableTightPacking: 1
    variantMultiplier: 1
    packables: []
    bindAsDefault: 1
  m_MasterAtlas: {fileID: 0}
  m_PackedSprites: []
  m_PackedSpriteNamesToIndex: []
  m_Tag: New Sprite Atlas
  m_IsVariant: 0
";


        public static SpriteAtlas CreateAtlas(string atlasName, string sptDesDir)
        {
            var filePath = GetFilePath(atlasName, sptDesDir);
            EditorUtils.CreateAssetFolder(sptDesDir);
            if (File.Exists(filePath))
            {
                return LoadOld(filePath);
            }
            return GenerateNewAtlas(filePath);
        }

        private static SpriteAtlas LoadOld(string filePath)
        {
            SpriteAtlas atlas   = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(filePath);
            var         sprites = atlas.GetPackables();
            atlas.Remove(sprites);
            return atlas;
        }

        private static SpriteAtlas GenerateNewAtlas(string filePath)
        {
            CreateSpriteAtlas(filePath);
            return AssetDatabase.LoadAssetAtPath<SpriteAtlas>(filePath);
        }

        private static string GetFilePath(string atlasName, string sptDesDir)
        {
            if (!sptDesDir.EndsWith("/"))
                sptDesDir += "/";
            string filePath = sptDesDir + atlasName;

            if (!filePath.EndsWith(".spriteatlas"))
                filePath += ".spriteatlas";
            return filePath;
        }

        private static void CreateSpriteAtlas(string filePath)
        {
            using ( FileStream fs    = new FileStream(filePath, FileMode.CreateNew))
            {
                byte[] bytes = new UTF8Encoding().GetBytes(yaml);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();      
            }
            //await Task.Delay(50); // чтобы юнити одуплилось и успело заимпортить файл
            AssetDatabase.Refresh();
        }

        private static void CheckExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                AssetDatabase.Refresh();
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace net.fiveotwo.pyxelImporter
{
    public class CreateSprite 
    {
        private Texture2D Merge(RenderTexture baseTexture, List<Texture2D> textures)
        {
            Graphics.Blit(textures[0], baseTexture);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = baseTexture;
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, baseTexture.width, baseTexture.height, 0);
            for (int x = 1; x < textures.Count; x++)
            {
                Graphics.DrawTexture(new Rect(0, 0, baseTexture.width, baseTexture.height), textures[x]);
            }
            GL.PopMatrix();
            Texture2D readableText = new Texture2D(baseTexture.width, baseTexture.height);
            readableText.ReadPixels(new Rect(0, 0, baseTexture.width, baseTexture.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(baseTexture);

            return readableText;
        }

        public Texture2D CreateImage(DocData data, string path, string destinationPath)
        {
            Canvas canvas = data.canvas;
            RenderTexture renderTex = RenderTexture.GetTemporary(canvas.width, canvas.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            ICollection<int> keys = canvas.layers.Keys;
            List<Texture2D> textures = new();
            foreach (int key in keys)
            {
                canvas.layers.TryGetValue(key, out Layer layer);
                if (!layer.hidden)
                {
                    byte[] fileData;
                    fileData = File.ReadAllBytes($"{path}/layer{key}.png");
                    Texture2D tex = new Texture2D(canvas.width, canvas.height);
                    tex.LoadImage(fileData);
                    textures.Add(tex);
                }
            }
            textures.Reverse();
            Texture2D texture = Merge(renderTex, textures);
            byte[] encoded = texture.EncodeToPNG();

            File.WriteAllBytes($"{destinationPath}/{data.name}.png", encoded);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            ProcessTexture(data, $"{destinationPath}/{data.name}.png");
            Texture2D newTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(HelperClass.GetRelativePath($"{destinationPath}/{data.name}.png"), typeof(Texture2D));

            return newTexture;
        }

        private void ProcessTexture(DocData data, string destinationPath)
        {
            string importerPath = HelperClass.GetRelativePath(destinationPath);

            Vector2Int tile = new(data.canvas.tileWidth, data.canvas.tileHeight);
            int spriteSize = tile.x > tile.y ? tile.x : tile.y;

            AssetDatabase.ImportAsset($"{importerPath}", ImportAssetOptions.ForceUpdate);
            TextureImporter textureImporter = AssetImporter.GetAtPath($"{importerPath}") as TextureImporter;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.wrapMode = TextureWrapMode.Repeat;
            textureImporter.spritePixelsPerUnit = spriteSize;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.spritesheet = GetAnimationMetadata(data.animations, data.canvas);
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.SaveAndReimport();
        }

        private SpriteMetaData[] GetAnimationMetadata(IDictionary<int, Animation> animationInfo, Canvas canvas)
        {
            List<SpriteMetaData> metas = new();
            Vector2Int tile = new(canvas.tileWidth, canvas.tileHeight);
            Vector2Int sizeInTiles = HelperClass.GetSizeInTiles(canvas);

            foreach (int key in animationInfo.Keys)
            {
                animationInfo.TryGetValue(key, out Animation animation);
                int baseTile = animation.baseTile;
                Vector2Int startPosition = HelperClass.GetIndexOfAnimation(baseTile, sizeInTiles);
                int totalFrames = animation.length;
                int currentFrame = 0;
                for (int spriteSheetY = startPosition.y; spriteSheetY < sizeInTiles.y; spriteSheetY++)
                {
                    for (int spriteSheetX = startPosition.x; spriteSheetX < sizeInTiles.x; spriteSheetX++)
                    {
                        if (currentFrame < totalFrames)
                        {
                            SpriteMetaData meta = new();
                            meta.rect = new Rect(spriteSheetX * tile.x, canvas.height - (spriteSheetY + 1) * tile.y, tile.x, tile.y);
                            meta.name = $"{animation.name}_{currentFrame}";
                            metas.Add(meta);
                            currentFrame++;
                        }
                    }
                }
            }
            return metas.ToArray();
        }
    }
}
using System.Text.RegularExpressions;
using UnityEngine;

namespace net.fiveotwo.pyxelImporter
{
    public static class HelperClass
    {
        public static Vector2Int GetSizeInTiles(Canvas canvas)
        {
            return new(canvas.width / canvas.tileWidth, canvas.height / canvas.tileHeight);
        }

        public static string GetRelativePath(string destinationPath)
        {
            return Regex.Replace(destinationPath, "^.*Assets", "Assets");
        }

        public static Vector2Int GetIndexOfAnimation(int baseTile, Vector2Int sizeInTiles)
        {
            Vector2Int itemIndex = new();
            int internalCount = 0;
            for (int spriteSheetY = 0; spriteSheetY < sizeInTiles.y; spriteSheetY++)
            {
                for (int spriteSheetX = 0; spriteSheetX < sizeInTiles.x; spriteSheetX++)
                {
                    if (internalCount == baseTile)
                    {
                        itemIndex.x = spriteSheetX;
                        itemIndex.y = spriteSheetY;
                        return itemIndex;
                    }
                    internalCount++;
                }
            }

            return itemIndex;
        }
    }
}
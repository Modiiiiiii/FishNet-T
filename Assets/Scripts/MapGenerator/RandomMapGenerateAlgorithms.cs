using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public class RandomMapGenerateAlgorithms 
    {
        public static BoundsInt[,] GeneratorRegionPoint(int regionSizeX, int regionSizeY, int regoinWidth, int regoinHeight)
        {
            BoundsInt[,] regionPoints = new BoundsInt[regionSizeX, regionSizeY];
            for (int i = 0; i < regionPoints.GetLength(0); i++)
            {
                for (int j = 0; j < regionPoints.GetLength(1); j++)
                {
                    regionPoints[i, j] = new BoundsInt(
                        new Vector3Int(i * regoinWidth, j * regoinHeight),
                        new Vector3Int(regoinWidth, regoinHeight));
                }
            }
            return regionPoints;
        }

        public static HashSet<Vector2Int> GenerateFloorPoints(BoundsInt regionPoints, HashSet<Vector2Int> checkAllFloor, int mapIterations,int mapSize)
        {
            int count = 0;
            int randomDir = 0;
            int maxSize = mapIterations * mapSize;
            HashSet<Vector2Int> points = new HashSet<Vector2Int>();
            List<Vector2Int> tempPoints = new List<Vector2Int>();
            
            var center  =  regionPoints.center;
            Vector2Int currentPoint = new Vector2Int(Mathf.RoundToInt(center.x), Mathf.RoundToInt(center.y));
            System.Random random = new System.Random();
            for (int i = 0; i < mapIterations; i++)
            {
                while (count<mapSize)
                {
                    var currentDir = random.Next(0, 4);
                    if (currentDir != randomDir)
                    {
                        randomDir = currentDir;
                        currentPoint += GetDir(randomDir);
                        if (!checkAllFloor.Contains(currentPoint) && !points.Contains(currentPoint))
                        {
                            checkAllFloor.Add(currentPoint);
                            points.Add(currentPoint);
                            tempPoints.Add(currentPoint);
                            if (points.Count >= maxSize)
                            {
                                SpreadFloorPoints(points, checkAllFloor, tempPoints);
                                return points;
                            }

                            count++;
                        }
                    }
                }

                count = 0;
                currentPoint = tempPoints[random.Next(0,points.Count)];
                
            }
            SpreadFloorPoints(points,checkAllFloor, tempPoints);
            return points;
        }

        /// <summary>
        /// 平滑坐标
        /// </summary>
        private static void SpreadFloorPoints(HashSet<Vector2Int> points, HashSet<Vector2Int> checkAllFloor, List<Vector2Int> tempPoints)
        {
            
        }

        #region 地图生成方向
        private enum EightDir
        {
            Left,
            Right,
            Top,
            Bottom,
            LeftTop,
            RightTop,
            LeftBottom,
            RightBottom,
        }
        
        private static Vector2Int GetDir(int randomDir)
        {
            return randomDir switch
            {
                (int)EightDir.Left => new Vector2Int(-1, 0),
                (int)EightDir.Right => new Vector2Int(1, 0),
                (int)EightDir.Top => new Vector2Int(0, 1),
                (int)EightDir.Bottom => new Vector2Int(0, -1),
                (int)EightDir.LeftTop => new Vector2Int(-1, 1),
                (int)EightDir.RightTop => new Vector2Int(1, 1),
                (int)EightDir.LeftBottom => new Vector2Int(-1, -1),
                (int)EightDir.RightBottom => new Vector2Int(1, -1),
                _ => new Vector2Int(0, 0)
            };
        }
        #endregion

        
    }
}

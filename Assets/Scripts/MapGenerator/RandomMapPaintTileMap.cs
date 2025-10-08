using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator
{
    public class RandomMapPaintTileMap : MonoBehaviour
    {
        [Header("地图瓷砖")]
        [SerializeField] private TileBase[] floorTiles;
        [SerializeField] private TileBase wallColliderTile;
        
        [SerializeField] private Tilemap[] floorTileMap;
        [SerializeField] private Tilemap wallColliderTileMap;
        

        public void ClearTile()
        {
            foreach (var map in floorTileMap)
            {
                map.ClearAllTiles();
            }
            wallColliderTileMap.ClearAllTiles();
        }

        /// <summary>
        /// 绘制地图瓷砖
        /// </summary>
        private async UniTask PaintTile(HashSet<Vector2Int> points,Tilemap tilemap,TileBase tile)
        {
            int count = 0;
            foreach (var point in points)
            {
                var tilePoint = tilemap.WorldToCell((Vector3Int)point);
                count++;
                tilemap.SetTile(tilePoint,tile);
                if (count >= 500)
                {
                    count = 0;
                    await UniTask.NextFrame();
                }
            }
        }

        /// <summary>
        /// 绘制地面瓷砖
        /// </summary>
        public UniTask PaintFloorTile(HashSet<Vector2Int> points, int tileIndex)
        {
            return PaintTile(points, floorTileMap[tileIndex], floorTiles[tileIndex]);
        }
        
        /// <summary>
        /// 绘制墙体地面瓷砖
        /// </summary>
        public UniTask PaintWallTile(HashSet<Vector2Int> points)
        {
            return PaintTile(points, wallColliderTileMap, wallColliderTile);
        }
    }
}

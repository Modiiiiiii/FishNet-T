using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MapGenerator
{
    public class RandomMapGenerator : MonoBehaviour
    {
        [Header("地图种子")]
        public int mapSeed;
        [Header("地图大小")]
        public int mapSize;
        [Header("地图迭代次数")]
        public int mapIterations; //根据地图大小的迭代值
        
        [Header("地图绘制")]
        [SerializeField] private RandomMapPaintTileMap paintTileMap;
        [SerializeField] private RandomMapPaintTileProp paintTileProp;
        [Header("地图区域大小与范围")]
        [SerializeField] private Vector2Int regionSize;
        [SerializeField] private Vector2Int regionArea;

        private HashSet<Vector2Int>[,] _floorPoint; //地面坐标点
        private HashSet<Vector2Int>[,] _propsPoint; //道具坐标点
        private HashSet<Vector2Int>[,] _wallColliderPoint; //墙壁碰撞坐标点
        private HashSet<Vector2Int>[,] _wallDecorationPoint; //地面坐标点


        private void Start()
        {
            GenerateMap();
        }
        public async void GenerateMap()
        {
            ResetMapData();
            var regionPoints = InitRegion();
            var checkAllFloor = GenerateFloorPoints(regionPoints);

            await UniTask.WhenAll(PaintTileMap(0,0),PaintTileMap(0,1),PaintTileMap(1,0));
            await UniTask.WhenAll(PaintTileMap(1,1),PaintTileMap(2,0),PaintTileMap(2,1));
            await UniTask.WhenAll(PaintTileMap(0,2),PaintTileMap(1,2),PaintTileMap(2,2));
        }

        private UniTask PaintTileMap(int v1, int v2)
        {
            int index = v1 * regionSize.y + v2;
            return paintTileMap.PaintFloorTile(_floorPoint[v1, v2], index);
        }

        #region 区域生成

        /// <summary>
        /// 生成地面坐标点
        /// </summary>
        private HashSet<Vector2Int> GenerateFloorPoints(BoundsInt[,] regionPoints)
        {
            _floorPoint =  new HashSet<Vector2Int>[regionSize.x, regionSize.y];
            _propsPoint = new HashSet<Vector2Int>[regionSize.x, regionSize.y];
            
            Vector2Int[,] regionCenters   = new Vector2Int[regionSize.x, regionSize.y];
            HashSet<Vector2Int> checkFloor = new HashSet<Vector2Int>();
            GenerateRegionPoint(regionPoints,regionCenters,checkFloor);
            return checkFloor;
        }

        
        /// <summary>
        /// 生成区域坐标点
        /// </summary>
        private void GenerateRegionPoint(BoundsInt[,] regionPoints, Vector2Int[,] regionCenters, HashSet<Vector2Int> checkFloor)
        {
            for (int i = 0; i < regionPoints.GetLength(0); i++)
            {
                for (int j = 0; j < regionPoints.GetLength(1); j++)
                {
                    _floorPoint[i,j] =  new HashSet<Vector2Int>();
                    _propsPoint[j,i] =  new HashSet<Vector2Int>();
                    
                    var region = regionPoints[i,j];
                    var center = region.center;

                    _floorPoint[i, j] = RandomMapGenerateAlgorithms.GenerateFloorPoints(regionPoints[i,j],checkFloor,mapIterations,mapSize);
                    _propsPoint[i, j].UnionWith(_floorPoint[i, j]);
                    regionCenters[i,j] = (Vector2Int)Vector3Int.RoundToInt(center);
                }
            }
        }

        #endregion
        


        #region 初始化
        private BoundsInt[,] InitRegion()
        {
            return RandomMapGenerateAlgorithms.GeneratorRegionPoint(regionSize.x,regionSize.y,regionArea.x,regionArea.y);
        }
        
        
        /// <summary>
        /// 重置地图数据
        /// </summary>
        public void ResetMapData()
        {
            InitMapSeed();
            InitMapData();
            InitMapPaint();
            GC.Collect();
        }

        private void InitMapPaint()
        {
            paintTileMap.ClearTile();
            paintTileProp.ClearProp();
        }

        private void InitMapData()
        {
            _floorPoint = null;
            _propsPoint = null;
            _wallColliderPoint = null;
            _wallDecorationPoint = null;
        }

        /// <summary>
        /// 初始化地图随机种子
        /// </summary>
        private void InitMapSeed()
        {
            if (mapSeed == 0)
            {
                Random.InitState(Random.Range(-10000, 10000));
                return;
            }

            Random.InitState(mapSeed);
        }
        
        

        #endregion

        
    }
}

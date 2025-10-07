using UnityEngine;

namespace MapGenerator
{
    public class SimplePerlinTerrain : MonoBehaviour
    {
        [Header("地形设置")]
        public int width = 100;          // 地形宽度
        public int height = 100;         // 地形长度
        public float scale = 10f;        // 噪声缩放（控制地形起伏频率）
        public float heightMultiplier = 10f; // 高度乘数（控制地形起伏幅度）
    
        [Header("噪声设置")]
        public float xOffset = 0f;       // X轴偏移（用于地形滚动）
        public float yOffset = 0f;       // Y轴偏移（用于地形滚动）
        public int octaves = 1;          // 噪声 octave 数量（未来用于分形噪声）

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        void Start()
        {
            // 确保有必要的组件
            _meshFilter = GetComponent<MeshFilter>();
            if (_meshFilter == null)
                _meshFilter = gameObject.AddComponent<MeshFilter>();
            
            _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshRenderer == null)
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();

            // 生成地形
            GenerateTerrain();
        
            // 创建简单材质
            CreateSimpleMaterial();
        }

        void Update()
        {
            // 按空格键重新生成地形
            if (Input.GetKeyDown(KeyCode.Space))
            {
                xOffset = Random.Range(0f, 100f);
                yOffset = Random.Range(0f, 100f);
                GenerateTerrain();
            }
        }

        void GenerateTerrain()
        {
            // 创建网格
            Mesh mesh = new Mesh();
        
            // 生成顶点
            Vector3[] vertices = new Vector3[width * height];
            Vector2[] uv = new Vector2[width * height];
        
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 计算柏林噪声高度
                    float xCoord = (float)x / width * scale + xOffset;
                    float yCoord = (float)z / height * scale + yOffset;
                    float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);
                
                    // 应用高度
                    vertices[z * width + x] = new Vector3(x, noiseValue * heightMultiplier, z);
                    uv[z * width + x] = new Vector2((float)x / width, (float)z / height);
                }
            }

            // 生成三角形
            int[] triangles = new int[(width - 1) * (height - 1) * 6];
            int triIndex = 0;
        
            for (int z = 0; z < height - 1; z++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int bottomLeft = z * width + x;
                    int bottomRight = bottomLeft + 1;
                    int topLeft = (z + 1) * width + x;
                    int topRight = topLeft + 1;
                
                    // 第一个三角形
                    triangles[triIndex] = bottomLeft;
                    triangles[triIndex + 1] = topLeft;
                    triangles[triIndex + 2] = bottomRight;
                
                    // 第二个三角形
                    triangles[triIndex + 3] = bottomRight;
                    triangles[triIndex + 4] = topLeft;
                    triangles[triIndex + 5] = topRight;
                
                    triIndex += 6;
                }
            }

            // 设置网格数据
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
        
            // 重新计算法线和边界
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        
            _meshFilter.mesh = mesh;
        }

        void CreateSimpleMaterial()
        {
            // 创建简单材质
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        
            // 根据高度设置颜色渐变
            Texture2D texture = new Texture2D(256, 1);
            for (int i = 0; i < 256; i++)
            {
                float heightF = i / 255f;
                Color color;
            
                if (heightF < 0.3f) color = Color.blue;           // 水
                else if (heightF < 0.4f) color = Color.yellow;    // 沙滩
                else if (heightF < 0.6f) color = Color.green;     // 草地
                else if (heightF < 0.8f) color = Color.gray;      // 岩石
                else color = Color.white;                        // 雪地
                
                texture.SetPixel(i, 0, color);
            }
            texture.Apply();
        
            material.mainTexture = texture;
            _meshRenderer.material = material;
        }

        // 在Inspector中显示生成按钮
        [ContextMenu("重新生成地形")]
        void RegenerateTerrain()
        {
            GenerateTerrain();
        }
    }
}
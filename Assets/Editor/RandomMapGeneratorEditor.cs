using System;
using MapGenerator;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(RandomMapGenerator),true)]
public class RandomMapGeneratorEditor : UnityEditor.Editor
{
    private RandomMapGenerator _generator;

    private void Awake()
    {
        _generator = (RandomMapGenerator)(target);
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("生成地图"))
        {
            _generator.GenerateMap();
        }
        
        if (GUILayout.Button("重置地图"))
        {
            _generator.ResetMapData();
        }
    }
    
}
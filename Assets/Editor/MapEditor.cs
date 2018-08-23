using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EarthSimulator.MapEditor
{
    public class MapEditor : EditorWindow
    {
        #region private变量
        private static bool isLoadData = false;
        private static GameObject mapObject = null;
        #endregion

        #region ediotr修改的private变量
        private static int mapSizeX;
        private static int mapSizeY;
        #endregion


        [MenuItem(@"Tools/MapEditor")]
        private static void Init()
        {
            EditorWindow.GetWindow(typeof(MapEditor), false, "地图编辑器");
        }

        void OnGUI()
        {
            GetMapColor();
            GetGenerateSpeed();
            GetMapData();
        }

        /// <summary>
        /// 获取地图的颜色
        /// </summary>
        private static void GetMapColor()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地图颜色:");
            MapData.MapColor = EditorGUILayout.ColorField(MapData.MapColor);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("改变颜色:");
            MapData.ChangeColor = EditorGUILayout.ColorField(MapData.ChangeColor);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        /// <summary>
        /// 获取生成地图的速度
        /// </summary>
        private static void GetGenerateSpeed()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("改变边界点速度:");
            MapData.BoarderGenerateSpeed = EditorGUILayout.Slider(MapData.BoarderGenerateSpeed, 1,100);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("改变地图生成速度:");
            MapData.MapGenerateSpeed = EditorGUILayout.Slider(MapData.MapGenerateSpeed, 1, 10);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// 获取地图的相关资源
        /// </summary>
        private static void GetMapData()
        {
            if (GUILayout.Button("加载地图相关数据"))
            {
                Object mapPrefab = Resources.Load("MapData/MapObject");
                if (mapPrefab != null)
                {
                    if(mapObject!=null)
                    {
                        GameObject.DestroyImmediate(mapObject);
                        mapObject = null;
                    }
                    mapObject = GameObject.Instantiate(mapPrefab) as GameObject;
                    MapData.Image = null;
                    MapData.Image = (mapObject).GetComponentInChildren<RawImage>();
                    if (MapData.Image != null)
                    {
                        Object mapTexture = Resources.Load("MapData/map");
                        if (mapTexture != null)
                        {
                            MapData.Map = GameObject.Instantiate<Texture2D>(mapTexture as Texture2D);
                            mapSizeX = MapData.Map.width;
                            mapSizeY = MapData.Map.height;
                            MapData.Image.texture = MapData.Map;
                            MapData.Image.SetNativeSize();
                            isLoadData = true;
                        }
                    }
                }
            }
            if (isLoadData)
            {
                if (GUILayout.Button("生成数据"))
                {
                    EditorCoroutineRunner.StartEditorCoroutine(GetCitys());
                }

                if (GUILayout.Button("强制生成单个三角面"))
                {
                    MapData.IsForceContinue = true;
                }
            }
        }

        private static IEnumerator GetCitys()
        {
            for (int i = mapSizeX; i >= 0; i -= 16)
            {
                for (int j = mapSizeY; j >= 0; j -= 16)
                {
                    if (MapData.Map.GetPixel(i, j) == MapData.MapColor)
                    {
                        yield return EditorCoroutineRunner.StartEditorCoroutine(new Province().GetCityFromPoint(i, j));
                    }
                }
            }
        }
    }
}

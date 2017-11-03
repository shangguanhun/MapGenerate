using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMap : MonoBehaviour {

    public Color mapColor = Color.red;
    public Color changeColor = Color.grey;
    public int mapSizeX;
    public int mapSizeY;
    public Texture2D map = null;
    public RawImage image;
    public int cityNum = 0;

    public List<Vector2> pointList = new List<Vector2>();
    public List<Vector2> borderPointList = new List<Vector2>();

    bool isGetPointOver = true;
    bool isMakeMeshOver = true;

    void Start () {

        map = Resources.Load("map") as Texture2D;
        map=GameObject.Instantiate(map);
        image.texture = map;
        image.SetNativeSize();
        mapSizeX = map.width;
        mapSizeY = map.height;
        StartCoroutine(GetCitys());
    }

    void SetPixel(int x,int y)
    {
        map.SetPixel(x, y, changeColor);
    }

    void TryAddPoint(int x, int y)
    {
        if (map.GetPixel(x, y) == mapColor)
        {
            pointList.Add(new Vector2(x, y));
        }
    }

    void TrySetPixel(int x,int y)
    {
        if (map.GetPixel(x, y) == mapColor)
        {
            SetPixel(x, y);
            TryAddPoint(x - 1, y);
            TryAddPoint(x + 1, y);
            TryAddPoint(x, y + 1);
            TryAddPoint(x, y - 1);
        }
        else
        {
            borderPointList.Add(new Vector2(x, y));
        }
    }

    IEnumerator GetCityFromPoint(int x, int y)
    {
        pointList.Add(new Vector2(x, y));
        borderPointList.Clear();
        int timer = 0;
        while (pointList.Count > 0)
        {
            Vector2 point = pointList[0];
            TrySetPixel((int)point.x, (int)point.y);
            pointList.RemoveAt(0);
            if (timer > 100)
            {
                timer = 0;
                map.Apply();
                yield return new WaitForFixedUpdate();
            }
            timer++;
        }
        isGetPointOver = true;
    }

    IEnumerator makeCityMesh(int x,int y)
    {
        Mesh mesh = new Mesh();
        Vector3[] m_vertices;
        Vector2[] m_uv;
        int[] m_triangles;
        //Color[] m_color;
        //Vector3[] m_normals;
        m_vertices = new Vector3[borderPointList.Count+1];
        m_uv = new Vector2[borderPointList.Count * 2];
        m_triangles = new int[borderPointList.Count*3];

        m_vertices[0] = new Vector3(x, y, 0);

        yield return new WaitForFixedUpdate();

        int i = 0;
        foreach (Vector2 vec in borderPointList)
        {
            m_vertices[i + 1] = new Vector3(vec.x, vec.y, 0);
            m_uv[i * 2] = Vector2.zero;
            m_uv[i * 2 + 1] = new Vector2(1, i / borderPointList.Count);
            m_triangles[i * 3] = 0;
            m_triangles[i * 3 + 1] = i + 1;
            m_triangles[i * 3 + 2] = i + 2;
            if (i == borderPointList.Count)
                m_triangles[i * 3 + 2] = 0;
            i++;
            if(i%100==0)
                yield return new WaitForFixedUpdate();
        }

        mesh.Clear();
        mesh.vertices = m_vertices;
        mesh.uv = m_uv;
        mesh.triangles = m_triangles;
        //mesh.colors = m_color;
        //mesh.normals = m_normals;
        mesh.RecalculateNormals();
        isMakeMeshOver = true;
    }
    
    IEnumerator GetCitys()
    {
        for(int i = 0; i < mapSizeX; i+=16)
        {
            for (int j = 0; j < mapSizeY; j += 16)
            {
                if (map.GetPixel(i, j) == mapColor)
                {
                    isGetPointOver = false;
                    isMakeMeshOver = false;
                    StartCoroutine(GetCityFromPoint(i, j));
                    while (!isGetPointOver)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    if (borderPointList.Count > 0)
                    {
                        StartCoroutine(makeCityMesh(i, j));
                        while (!isMakeMeshOver)
                        {
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                }
            }
        }
    }
}

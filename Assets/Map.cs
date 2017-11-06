using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map{
    private static Map map = null;
    private static GameObject BaseMeshObject;
    public static Map GetMap
    {
        get
        {
            if (map == null)
            {
                BaseMeshObject = Resources.Load("BaseMeshObject") as GameObject;
                map = new Map();
            }
            return map;
        }
    }

    public IEnumerator normalizeBorderPoint()
    {
        for (int i = 1; i < GenerateMap.GetGenerateMap.borderPointList.Count - 1; i++)
        {
            Vector2 firstPoint = GenerateMap.GetGenerateMap.borderPointList[i - 1];
            Vector2 nowPoint = GenerateMap.GetGenerateMap.borderPointList[i];
            Vector2 lastPoint = GenerateMap.GetGenerateMap.borderPointList[i + 1];
            if (Vector2.Angle(nowPoint - firstPoint, lastPoint - nowPoint) < 0.0001f)
            {
                GenerateMap.GetGenerateMap.borderPointList.RemoveAt(i);
                i--;
            }
            if (i % 100 == 0)
                yield return new WaitForFixedUpdate();
        }
        GenerateMap.GetGenerateMap.isnormalizeBorderPointOver = true;
    }

    private void LoadMesh(Mesh mesh)
    {
        
    }

    public IEnumerator makeCityMesh(int x, int y)
    {
        GameObject go = GameObject.Instantiate(BaseMeshObject, GenerateMap.GetGenerateMap.transform);
        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        Vector3[] m_vertices;
        Vector2[] m_uv;
        int[] m_triangles;
        //Color[] m_color;
        //Vector3[] m_normals;
        m_vertices = new Vector3[GenerateMap.GetGenerateMap.borderPointList.Count + 1];
        m_uv = new Vector2[GenerateMap.GetGenerateMap.borderPointList.Count + 1];
        m_triangles = new int[GenerateMap.GetGenerateMap.borderPointList.Count * 3];

        m_uv[0] = Vector2.zero;
        m_vertices[0] = new Vector3(x, y, 0);

        yield return new WaitForFixedUpdate();

        int i = 0;
        foreach (Vector2 vec in GenerateMap.GetGenerateMap.borderPointList)
        {
            m_vertices[i + 1] = new Vector3(vec.x, vec.y, 0);
            m_uv[i + 1] = new Vector2(1, i / GenerateMap.GetGenerateMap.borderPointList.Count);
            if (i < GenerateMap.GetGenerateMap.borderPointList.Count)
            {
                m_triangles[i * 3] = 0;
                m_triangles[i * 3 + 1] = i + 1;
                m_triangles[i * 3 + 2] = i + 2;
                if (i == GenerateMap.GetGenerateMap.borderPointList.Count-1)
                    m_triangles[i * 3 + 2] = 1;
            }
            
            i++;
            if (i % 100 == 0)
                yield return new WaitForFixedUpdate();
        }

        mesh.Clear();
        mesh.vertices = m_vertices;
        mesh.uv = m_uv;
        mesh.triangles = m_triangles;
        mesh.name = GenerateMap.GetGenerateMap.provinceNum.ToString() + "_mesh";
        //mesh.colors = m_color;
        //mesh.normals = m_normals;
        mesh.RecalculateNormals();
        LoadMesh(mesh);
        GenerateMap.GetGenerateMap.isMakeMeshOver = true;
    }
}

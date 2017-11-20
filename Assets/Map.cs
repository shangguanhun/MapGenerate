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

    //删除多余顶点
    public void NormalizeBorderPoint()
    {
        Debug.Log("删除多余点前点的数量："+GenerateMap.GetGenerateMap.borderPointList.Count);
        for (int i = 1; i < GenerateMap.GetGenerateMap.borderPointList.Count - 1; i++)
        {
            Vector2 firstPoint = GenerateMap.GetGenerateMap.borderPointList[i - 1];
            Vector2 nowPoint = GenerateMap.GetGenerateMap.borderPointList[i];
            Vector2 lastPoint = GenerateMap.GetGenerateMap.borderPointList[i + 1];
            if (Vector2.Angle(nowPoint - firstPoint, lastPoint - nowPoint) < 1)
            {
                GenerateMap.GetGenerateMap.borderPointList.RemoveAt(i);
                i--;
            }
        }
        Debug.Log("删除多余点后点的数量："+GenerateMap.GetGenerateMap.borderPointList.Count);
    }

    public void MakeCityMesh(int x, int y)
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
        }

        mesh.Clear();
        mesh.vertices = m_vertices;
        mesh.uv = m_uv;
        mesh.triangles = m_triangles;
        mesh.name = GenerateMap.GetGenerateMap.provinceNum.ToString() + "_mesh";
        //mesh.colors = m_color;
        //mesh.normals = m_normals;
        mesh.RecalculateNormals();
    }
    
    public void MakeCityMesh()
    {
        List<Vector3> m_vertices = new List<Vector3>();
        List<int> m_triangles = new List<int>();

        List<PointF> polygon = new List<PointF>();
        foreach (Vector2 vec in GenerateMap.GetGenerateMap.borderPointList)
        {
            polygon.Add(new PointF(vec.x, vec.y));
        }
        while (polygon.Count > 0)
        {
            List<PointF> nPolygon = new List<PointF>();
            m_vertices.Clear();
            m_triangles.Clear();
            for (int j = 1; j < polygon.Count - 1; j++)
            {
                PointF point = new PointF(polygon[j + 1].X - polygon[j - 1].X, polygon[j + 1].Y - polygon[j - 1].Y);
                Line line = new Line(new PointF(polygon[j - 1].X+point.X/10, polygon[j - 1].Y+ point.Y / 10), new PointF(polygon[j + 1].X- point.X / 10, polygon[j + 1].Y- point.Y / 10));
                Debug.Log(GeometryHelper.IntersectionOf(line, polygon));
                if (GeometryHelper.IntersectionOf(line,polygon)!= GeometryHelper.Intersection.None)
                {
                    nPolygon.Add(polygon[j]);
                    polygon.Remove(polygon[j]);
                    j--;
                }
                else
                {
                    Vector3 fVec = new Vector3(polygon[j - 1].X, polygon[j - 1].Y, 0);
                    Vector3 nVec = new Vector3(polygon[j].X, polygon[j].Y, 0);
                    Vector3 lVec = new Vector3(polygon[j + 1].X, polygon[j + 1].Y, 0);
                    if (!m_vertices.Contains(fVec))
                        m_vertices.Add(fVec);
                    if (!m_vertices.Contains(nVec))
                        m_vertices.Add(nVec);
                    if (!m_vertices.Contains(lVec))
                        m_vertices.Add(lVec);
                    for (int i = 0; i < m_vertices.Count; i++)
                        if (Vector3.Equals(fVec, m_vertices[i]))
                            m_triangles.Add(i);
                    for (int i = 0; i < m_vertices.Count; i++)
                        if (Vector3.Equals(nVec, m_vertices[i]))
                            m_triangles.Add(i);
                    for (int i = 0; i < m_vertices.Count; i++)
                        if (Vector3.Equals(lVec, m_vertices[i]))
                            m_triangles.Add(i);

                    polygon.Remove(polygon[j]);
                    j--;
                }
            }
            
            if (m_vertices.Count != 0)
            {
                GameObject go = GameObject.Instantiate(BaseMeshObject, GenerateMap.GetGenerateMap.transform);
                Mesh mesh = go.GetComponent<MeshFilter>().mesh;
                mesh.Clear();
                mesh.vertices = m_vertices.ToArray();
                mesh.triangles = m_triangles.ToArray();
                mesh.RecalculateNormals();
            }
            polygon.ToArray();
            polygon = nPolygon;
        }
    }

    public void MakeCityMesh1()
    {
        List<Vector3> m_vertices = new List<Vector3>();
        List<int> m_triangles = new List<int>();
        GameObject go = GameObject.Instantiate(BaseMeshObject, GenerateMap.GetGenerateMap.transform);
        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        m_vertices.Clear();
        m_triangles.Clear();
        List<PointF> polygon = new List<PointF>();
        foreach (Vector2 vec in GenerateMap.GetGenerateMap.borderPointList)
        {
            polygon.Add(new PointF(vec.x, vec.y));
        }
        while (polygon.Count > 0)
        {
            List<PointF> nPolygon = new List<PointF>();
            for (int j = 1; j < polygon.Count - 1; j++)
            {
                PointF point = new PointF(polygon[j + 1].X - polygon[j - 1].X, polygon[j + 1].Y - polygon[j - 1].Y);
                Line line = new Line(new PointF(polygon[j - 1].X + point.X / 10, polygon[j - 1].Y + point.Y / 10), new PointF(polygon[j + 1].X - point.X / 10, polygon[j + 1].Y - point.Y / 10));
                //Debug.Log(GeometryHelper.IntersectionOf(line, polygon));
                if (GeometryHelper.IntersectionOf(line, polygon) != GeometryHelper.Intersection.None)
                {
                    nPolygon.Add(polygon[j]);
                    polygon.Remove(polygon[j]);
                    j--;
                }
                else
                {
                    Vector3 fVec = new Vector3(polygon[j - 1].X, polygon[j - 1].Y, 0);
                    Vector3 nVec = new Vector3(polygon[j].X, polygon[j].Y, 0);
                    Vector3 lVec = new Vector3(polygon[j + 1].X, polygon[j + 1].Y, 0);
                    if (!m_vertices.Contains(fVec))
                        m_vertices.Add(fVec);
                    if (!m_vertices.Contains(nVec))
                        m_vertices.Add(nVec);
                    if (!m_vertices.Contains(lVec))
                        m_vertices.Add(lVec);
                    for (int i = 0; i < m_vertices.Count; i++)
                        if (Vector3.Equals(fVec, m_vertices[i]))
                            m_triangles.Add(i);
                    for (int i = 0; i < m_vertices.Count; i++)
                        if (Vector3.Equals(nVec, m_vertices[i]))
                            m_triangles.Add(i);
                    for (int i = 0; i < m_vertices.Count; i++)
                        if (Vector3.Equals(lVec, m_vertices[i]))
                            m_triangles.Add(i);

                    polygon.Remove(polygon[j]);
                    j--;
                }
            }

            if (m_vertices.Count != 0)
            {
                mesh.Clear();
                mesh.vertices = m_vertices.ToArray();
                mesh.triangles = m_triangles.ToArray();
                mesh.RecalculateNormals();
            }
            polygon.ToArray();
            polygon = nPolygon;
        }
    }
}

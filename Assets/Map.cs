using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
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

    //判断是否为“死”点，上下左右四个方向，均非地图内部点则为“死”点
    private bool IsDeathPoint(int x, int y)
    {
        Color color;

        color = GenerateMap.GetGenerateMap.map.GetPixel(x - 1, y);
        if (color == GenerateMap.GetGenerateMap.mapColor)
            return false;

        color = GenerateMap.GetGenerateMap.map.GetPixel(x, y + 1);
        if (color == GenerateMap.GetGenerateMap.mapColor)
            return false;

        color = GenerateMap.GetGenerateMap.map.GetPixel(x + 1, y);
        if (color == GenerateMap.GetGenerateMap.mapColor)
            return false;

        color = GenerateMap.GetGenerateMap.map.GetPixel(x, y - 1);
        if (color == GenerateMap.GetGenerateMap.mapColor)
            return false;

        return true;
    }

    //删除多余顶点
    public void NormalizeBorderPoint()
    {
        for (int i = 1; i < GenerateMap.GetGenerateMap.borderPointList.Count - 1; i++)
        {
            Vector2 firstPoint = GenerateMap.GetGenerateMap.borderPointList[i - 1];
            Vector2 nowPoint = GenerateMap.GetGenerateMap.borderPointList[i];
            Vector2 lastPoint = GenerateMap.GetGenerateMap.borderPointList[i + 1];
            if (Vector2.Angle(nowPoint - firstPoint, lastPoint - nowPoint) < 0.1f && Vector2.Angle(nowPoint - firstPoint, lastPoint - nowPoint) > -0.1f)
            {
                //GenerateMap.GetGenerateMap.map.SetPixel((int)GenerateMap.GetGenerateMap.borderPointList[i].x, (int)GenerateMap.GetGenerateMap.borderPointList[i].y, Color.green);
                GenerateMap.GetGenerateMap.borderPointList.RemoveAt(i);
                i--;
            }
        }
        GenerateMap.GetGenerateMap.map.Apply();
        Debug.Log("删除多余点后点的数量：" + GenerateMap.GetGenerateMap.borderPointList.Count);
    }

    public void DeleteDeathPoints()
    {
        Debug.Log("删除死点前点的数量：" + GenerateMap.GetGenerateMap.borderPointList.Count);
        for (int i = 1; i < GenerateMap.GetGenerateMap.borderPointList.Count - 1; i++)
        {
            if (IsDeathPoint((int)GenerateMap.GetGenerateMap.borderPointList[i].x, (int)GenerateMap.GetGenerateMap.borderPointList[i].y))
            {
                GenerateMap.GetGenerateMap.map.SetPixel((int)GenerateMap.GetGenerateMap.borderPointList[i].x, (int)GenerateMap.GetGenerateMap.borderPointList[i].y, Color.gray);
                GenerateMap.GetGenerateMap.borderPointList.Remove(GenerateMap.GetGenerateMap.borderPointList[i]);
                i--;
            }
        }
        Debug.Log("删除死点后点的数量：" + GenerateMap.GetGenerateMap.borderPointList.Count);
        GenerateMap.GetGenerateMap.map.Apply();
    }

    //弃用！！！此方法无法处理凹多边形
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
                if (i == GenerateMap.GetGenerateMap.borderPointList.Count - 1)
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

    //弃用！！！因其无法判断w形的凹多边形
    public void MakeCityMeshByLineAndPolygon()
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
            m_vertices.Add(new Vector3(vec.x, vec.y, 0));
        }
        while (polygon.Count > 2)
        {
            for (int j = 1; j < polygon.Count - 1; j++)
            {
                PointF point = new PointF(polygon[j + 1].X - polygon[j - 1].X, polygon[j + 1].Y - polygon[j - 1].Y);
                Line line = new Line(new PointF(polygon[j - 1].X + point.X / 10, polygon[j - 1].Y + point.Y / 10), new PointF(polygon[j + 1].X - point.X / 10, polygon[j + 1].Y - point.Y / 10));
                if (GeometryHelper.IntersectionOf(line, polygon) != GeometryHelper.Intersection.None || polygon.Count == 3)
                {
                    Vector3 fVec = new Vector3(polygon[j - 1].X, polygon[j - 1].Y, 0);
                    Vector3 nVec = new Vector3(polygon[j].X, polygon[j].Y, 0);
                    Vector3 lVec = new Vector3(polygon[j + 1].X, polygon[j + 1].Y, 0);
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
                else
                {
                    Debug.Log("233");
                }
            }
        }

        mesh.Clear();
        mesh.vertices = m_vertices.ToArray();
        mesh.triangles = m_triangles.ToArray();
        mesh.RecalculateNormals();
    }

    private Vector3 ComputeBestFitNormal(List<Vector3> pointList, int num)
    {
        //和置零
        Vector3 reslut = Vector3.zero;
        //从最后一个点开始，避免循环中进行if判断
        Vector3 p = pointList[num - 1];
        for (int i = 0; i < num; i++)
        {
            Vector3 c = pointList[i];
            reslut.x += (p.z + c.z) * (p.y - c.y);
            reslut.y += (p.x + c.x) * (p.z - c.z);
            reslut.z += (p.y + c.y) * (p.x - c.x);
            p = c;
        }
        reslut.Normalize();
        return reslut;
    }

    private List<Vector3> GetHollowPointList(List<Vector3> curveloopPoints)
    {
        //假设传进来的顶点数组都是按照顺时针或者逆时针遍历的，且没有重复点  
        //使用法向量判断凹凸性，检测多边形上是否有凸点，每个顶点的转向都应该一致，若不一致则为凹点  
        List<Vector3> HollowPoints = new List<Vector3>();
        int num = curveloopPoints.Count;
        Vector3 HollowNor = ComputeBestFitNormal(curveloopPoints, num);
        Vector3 Nor;
        for (int i = 0; i < num; i++)
        {
            if (i == 0)//第一个点  
            {
                Nor = Vector3.Cross((curveloopPoints[0] - curveloopPoints[num - 1]), (curveloopPoints[1] - curveloopPoints[0]));
                if (Vector3.Dot(Nor, HollowNor) < 0.0f)
                {
                    HollowPoints.Add(curveloopPoints[i]);
                }
            }
            else if (i == num - 1)//最后一个点  
            {
                Nor = Vector3.Cross((curveloopPoints[i] - curveloopPoints[i - 1]), (curveloopPoints[0] - curveloopPoints[i]));
                if (Vector3.Dot(Nor, HollowNor) < 0.0f)
                {
                    HollowPoints.Add(curveloopPoints[i]);
                }
            }
            else//中间点  
            {
                Nor = Vector3.Cross((curveloopPoints[i] - curveloopPoints[i - 1]), (curveloopPoints[i + 1] - curveloopPoints[i]));
                if (Vector3.Dot(Nor, HollowNor) < 0.0f)
                {
                    HollowPoints.Add(curveloopPoints[i]);
                }
            }
        }
        return HollowPoints;
    }

    public IEnumerator MakeCityMesh()
    {
        List<Vector3> m_vertices = new List<Vector3>();
        List<int> m_triangles = new List<int>();
        m_vertices.Clear();
        m_triangles.Clear();
        GameObject go = GameObject.Instantiate(BaseMeshObject, GenerateMap.GetGenerateMap.transform);
        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        List<Vector3> polygon = new List<Vector3>();
        List<Vector3> hollowPointList;
        foreach (Vector2 vec in GenerateMap.GetGenerateMap.borderPointList)
        {
            if (!polygon.Contains(new Vector3(vec.x, vec.y, 0)))
            {
                polygon.Add(new Vector3(vec.x, vec.y, 0));
                m_vertices.Add(new Vector3(vec.x, vec.y, 0));
            }
        }
        while (polygon.Count > 2)
        {
            hollowPointList = GetHollowPointList(polygon);
            for (int j = 0; j < polygon.Count; j++)
            {
                if (!hollowPointList.Contains(polygon[j]) && polygon.Count > 2)
                {
                    Vector3 fVec;
                    Vector3 nVec;
                    Vector3 lVec;
                    if (j > 0 && j < polygon.Count - 1)
                    {
                        fVec = new Vector3(polygon[j - 1].x, polygon[j - 1].y, 0);
                        nVec = new Vector3(polygon[j].x, polygon[j].y, 0);
                        lVec = new Vector3(polygon[j + 1].x, polygon[j + 1].y, 0);
                    }
                    else
                    if (j == 0)
                    {
                        fVec = new Vector3(polygon[polygon.Count - 1].x, polygon[polygon.Count - 1].y, 0);
                        nVec = new Vector3(polygon[j].x, polygon[j].y, 0);
                        lVec = new Vector3(polygon[j + 1].x, polygon[j + 1].y, 0);
                    }
                    else
                    {
                        fVec = new Vector3(polygon[j - 1].x, polygon[j - 1].y, 0);
                        nVec = new Vector3(polygon[j].x, polygon[j].y, 0);
                        lVec = new Vector3(polygon[0].x, polygon[0].y, 0);
                    }

                    for (int i = 0; i < m_vertices.Count; i++)
                        if (Vector3.Equals(fVec, m_vertices[i]))
                        {
                            m_triangles.Add(i);
                            break;
                        }
                    for (int i = 0; i < m_vertices.Count; i++)
                        if (Vector3.Equals(nVec, m_vertices[i]))
                        {
                            m_triangles.Add(i);
                            break;
                        }
                    for (int i = 0; i < m_vertices.Count; i++)
                        if (Vector3.Equals(lVec, m_vertices[i]))
                        {
                            m_triangles.Add(i);
                            break;
                        }

                    polygon.Remove(polygon[j]);
                    j--;
                }
            }
            hollowPointList.Clear();

            mesh.Clear();
            mesh.vertices = m_vertices.ToArray();
            mesh.triangles = m_triangles.ToArray();
            mesh.RecalculateNormals();
            yield return new WaitForSeconds(0.1f);
        }
        GenerateMap.GetGenerateMap.isDrawMeshOver = true;
    }
}

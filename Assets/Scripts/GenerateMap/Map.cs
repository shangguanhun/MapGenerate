using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EarthSimulator.MapEditor
{
    public class Map
    {
        private GameObject BaseMeshObject;
        public static int ProvinceNum = 0;
        public Map()
        {
            BaseMeshObject = Resources.Load("BaseMeshObject") as GameObject;
        }

        //判断是否为“死”点，上下左右四个方向，均非地图内部点则为“死”点
        private bool IsDeathPoint(int x, int y)
        {
            Color color;
            color = MapData.Map.GetPixel(x - 1, y);
            if (MapData.ColorNear(color , MapData.MapColor))
                return false;

            color = MapData.Map.GetPixel(x, y + 1);
            if (MapData.ColorNear(color , MapData.MapColor))
                return false;

            color = MapData.Map.GetPixel(x + 1, y);
            if (MapData.ColorNear(color , MapData.MapColor))
                return false;

            color = MapData.Map.GetPixel(x, y - 1);
            if (MapData.ColorNear(color , MapData.MapColor))
                return false;

            return true;
        }

        //删除多余顶点
        public void NormalizeBorderPoint(List<Vector2> BorderPointList)
        {
            for (int i = 1; i < BorderPointList.Count - 1; i++)
            {
                Vector2 firstPoint = BorderPointList[i - 1];
                Vector2 nowPoint = BorderPointList[i];
                Vector2 lastPoint = BorderPointList[i + 1];
                if (Vector2.Angle(nowPoint - firstPoint, lastPoint - nowPoint) < 0.1f && Vector2.Angle(nowPoint - firstPoint, lastPoint - nowPoint) > -0.1f)
                {
                    //MapData.Map.SetPixel((int)BorderPointList[i].x, (int)BorderPointList[i].y, Color.green);
                    BorderPointList.RemoveAt(i);
                    i--;
                }
            }
            MapData.Map.Apply();
            Debug.Log("删除多余点后点的数量：" + BorderPointList.Count);
        }

        public void DeleteDeathPoints(List<Vector2> BorderPointList)
        {
            Debug.Log("删除死点前点的数量：" + BorderPointList.Count);
            for (int i = 1; i < BorderPointList.Count - 1; i++)
            {
                if (IsDeathPoint((int)BorderPointList[i].x, (int)BorderPointList[i].y))
                {
                    MapData.Map.SetPixel((int)BorderPointList[i].x, (int)BorderPointList[i].y, Color.gray);
                    BorderPointList.Remove(BorderPointList[i]);
                    i--;
                }
            }
            Debug.Log("删除死点后点的数量：" + BorderPointList.Count);
            MapData.Map.Apply();
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

        private bool IsPolygonContainLine(List<PointF> nPolygon, Vector2 point1, Vector2 point2)
        {
            Vector2 vector;
            bool contain = true;
            float distance = Vector2.Distance(point1, point2);
            for (int i = 1; i < distance; i++)
            {
                vector = Vector2.Lerp(point1, point2, i / distance);
                if (!GeometryHelper.IsInPolygon(new PointF(vector.x, vector.y), nPolygon))
                {
                    contain = false;
                    break;
                }
            }
            return contain;
        }

        public IEnumerator MakeCityMesh(GameObject go, List<Vector2> BorderPointList)
        {
            if (go != null)
            {
                go.transform.parent = MapData.BaseGameObject.transform;

                DeleteDeathPoints(BorderPointList);
                yield return new WaitForSeconds(0.1f);
                NormalizeBorderPoint(BorderPointList);

                List<Vector3> m_vertices = new List<Vector3>();
                List<int> m_triangles = new List<int>();
                List<PointF> nPolygon = new List<PointF>();
                m_vertices.Clear();
                m_triangles.Clear();
                MeshFilter meshFilter = go.GetComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                meshFilter.mesh = mesh;
                List<Vector3> polygon = new List<Vector3>();
                List<Vector3> hollowPointList;
                foreach (Vector2 vec in BorderPointList)
                {
                    if (!polygon.Contains(new Vector3(vec.x, vec.y, 0)))
                    {
                        polygon.Add(new Vector3(vec.x, vec.y, 0));
                        m_vertices.Add(new Vector3(vec.x, vec.y, 0));
                        nPolygon.Add(new PointF(vec.x, vec.y));
                    }
                }
                int mapGenerateCount = 0;
                while (polygon.Count > 2)
                {
                    for (int j = 0; j < polygon.Count; j++)
                    {
                        hollowPointList = GetHollowPointList(polygon);
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
                            if (hollowPointList.Count == 0 || MapData.IsForceContinue || IsPolygonContainLine(nPolygon, fVec, lVec))
                            {
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
                                MapData.IsForceContinue = false;
                                polygon.Remove(polygon[j]);
                                nPolygon.Remove(nPolygon[j]);
                                j--;
                            }
                        }
                        hollowPointList.Clear();

                        mapGenerateCount++;
                        if (mapGenerateCount >= MapData.MapGenerateSpeed)
                        {
                            mapGenerateCount = 0;
                            mesh.Clear();
                            mesh.vertices = m_vertices.ToArray();
                            mesh.triangles = m_triangles.ToArray();
                            mesh.RecalculateNormals();
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                }

                mesh.Clear();
                mesh.vertices = m_vertices.ToArray();
                mesh.triangles = m_triangles.ToArray();
                List<Vector2> uvs = new List<Vector2>();
                for(int i = 0; i < m_vertices.Count; i++)
                {
                    uvs.Add(new Vector2(m_vertices[i].x / MapData.Map.width, m_vertices[i].y / MapData.Map.height));
                }
                mesh.SetUVs(0,uvs);
                mesh.RecalculateNormals();
                yield return new WaitForEndOfFrame();
                ProvinceNum++;
                mesh.name = ProvinceNum.ToString() + "_mesh";
                go.name = mesh.name;
                SerializeMesh.MeshToFile(meshFilter, mesh.name, 1);
            }
        }
    }
}

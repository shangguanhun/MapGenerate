using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EarthSimulator.MapEditor
{
    public class Province
    {
        void SetPixel(int x, int y)
        {
            MapData.Map.SetPixel(x, y, MapData.ChangeColor);
        }

        bool HaveMore(Vector2 from, Vector2 to)
        {
            if (Vector2.Distance(from, to) > 1)
                return true;
            else
                return false;
        }

        bool AddMorePoint(List<Vector2> BorderPointList)
        {
            int x, y;
            for (int i = BorderPointList.Count - 1; i > -1; i--)
            {
                if (BorderPointList.Count > 10 && !HaveMore(BorderPointList[0], BorderPointList[i]))
                    return false;
                x = (int)BorderPointList[i].x - 1;
                y = (int)BorderPointList[i].y;
                if (IsBorderPoint(x, y))
                {
                    BorderPointList.Add(new Vector2(x, y));
                    SetPixel(x, y);
                    return true;
                }
                x = (int)BorderPointList[i].x;
                y = (int)BorderPointList[i].y + 1;
                if (IsBorderPoint(x, y))
                {
                    BorderPointList.Add(new Vector2(x, y));
                    SetPixel(x, y);
                    return true;
                }
                x = (int)BorderPointList[i].x + 1;
                y = (int)BorderPointList[i].y;
                if (IsBorderPoint(x, y))
                {
                    BorderPointList.Add(new Vector2(x, y));
                    SetPixel(x, y);
                    return true;
                }
                x = (int)BorderPointList[i].x;
                y = (int)BorderPointList[i].y - 1;
                if (IsBorderPoint(x, y))
                {
                    BorderPointList.Add(new Vector2(x, y));
                    SetPixel(x, y);
                    return true;
                }




                x = (int)BorderPointList[i].x - 1;
                y = (int)BorderPointList[i].y + 1;
                if (IsBorderPoint(x, y))
                {
                    BorderPointList.Add(new Vector2(x, y));
                    SetPixel(x, y);
                    return true;
                }
                x = (int)BorderPointList[i].x + 1;
                y = (int)BorderPointList[i].y + 1;
                if (IsBorderPoint(x, y))
                {
                    BorderPointList.Add(new Vector2(x, y));
                    SetPixel(x, y);
                    return true;
                }
                x = (int)BorderPointList[i].x + 1;
                y = (int)BorderPointList[i].y - 1;
                if (IsBorderPoint(x, y))
                {
                    BorderPointList.Add(new Vector2(x, y));
                    SetPixel(x, y);
                    return true;
                }
                x = (int)BorderPointList[i].x - 1;
                y = (int)BorderPointList[i].y - 1;
                if (IsBorderPoint(x, y))
                {
                    BorderPointList.Add(new Vector2(x, y));
                    SetPixel(x, y);
                    return true;
                }
            }
            return false;
        }

        bool IsBorderPoint(int x, int y)
        {
            Color color;
            color = MapData.Map.GetPixel(x, y);
            if (MapData.ColorNear(color , MapData.MapColor))
            {
                color = MapData.Map.GetPixel(x - 1, y);
                if (!MapData.ColorNear(color , MapData.MapColor) && !MapData.ColorNear(color , MapData.ChangeColor))
                    return true;

                color = MapData.Map.GetPixel(x, y + 1);
                if (!MapData.ColorNear(color , MapData.MapColor) && !MapData.ColorNear(color , MapData.ChangeColor))
                    return true;

                color = MapData.Map.GetPixel(x + 1, y);
                if (!MapData.ColorNear(color , MapData.MapColor) && !MapData.ColorNear(color , MapData.ChangeColor))
                    return true;

                color = MapData.Map.GetPixel(x, y - 1);
                if (!MapData.ColorNear(color , MapData.MapColor) && !MapData.ColorNear(color , MapData.ChangeColor))
                    return true;
            }

            return false;
        }

        Vector2 GetBorderPoint(int x, int y)
        {
            while (!IsBorderPoint(x, y))
            {
                x--;
                if (x <= 0)
                    return Vector2.zero;
            }
            return new Vector2(x, y);
        }

        public IEnumerator GetCityFromPoint(int x, int y)
        {
            List<Vector2> BorderPointList = new List<Vector2>();
            Vector2 vector = GetBorderPoint(x, y);
            if (!vector.Equals(Vector2.zero))
            {
                if (vector.x >= 0)
                {
                    BorderPointList.Add(vector);
                    bool isMore = true;
                    int timer = 0;
                    while (isMore)
                    {
                        isMore = false;

                        if (AddMorePoint(BorderPointList))
                        {
                            isMore = true;
                        }

                        if (timer >= MapData.BoarderGenerateSpeed)
                        {
                            timer = 0;
                            MapData.Map.Apply();
                            yield return new WaitForFixedUpdate();
                        }
                        timer++;
                    }
                }
            }
            if (BorderPointList.Count > 2)
                yield return EditorCoroutineRunner.StartEditorCoroutine(new Map().MakeCityMesh(GameObject.Instantiate(Resources.Load("MapData/BaseMeshObject")) as GameObject, BorderPointList));
        }
    }
}

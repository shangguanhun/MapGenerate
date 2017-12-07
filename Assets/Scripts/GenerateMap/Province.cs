using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Province{

    private static Province province = null;
    public static Province GetProvince
    {
        get
        {
            if (province == null)
            {
                province = new Province();
            }
            return province;
        }
    }
    
    void SetPixel(int x, int y)
    {
        GenerateMap.GetGenerateMap.map.SetPixel(x, y, GenerateMap.GetGenerateMap.changeColor);
    }

    bool HaveMore(Vector2 from,Vector2 to)
    {
        if (Vector2.Distance(from, to) > 1)
            return true;
        else
            return false;
    }

    bool AddMorePoint()
    {
        int x, y;
        for (int i = GenerateMap.GetGenerateMap.borderPointList.Count - 1; i > -1; i--)
        {
            if (GenerateMap.GetGenerateMap.borderPointList.Count > 10 && !HaveMore(GenerateMap.GetGenerateMap.borderPointList[0], GenerateMap.GetGenerateMap.borderPointList[i]))
                return false;
            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x - 1;
            y = (int)GenerateMap.GetGenerateMap.borderPointList[i].y;
            if (IsBorderPoint(x, y))
            {
                GenerateMap.GetGenerateMap.borderPointList.Add(new Vector2(x, y));
                SetPixel(x, y);
                return true;
            }
            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x;
            y = (int)GenerateMap.GetGenerateMap.borderPointList[i].y + 1;
            if (IsBorderPoint(x, y))
            {
                GenerateMap.GetGenerateMap.borderPointList.Add(new Vector2(x, y));
                SetPixel(x, y);
                return true;
            }
            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x + 1;
            y = (int)GenerateMap.GetGenerateMap.borderPointList[i].y;
            if (IsBorderPoint(x, y))
            {
                GenerateMap.GetGenerateMap.borderPointList.Add(new Vector2(x, y));
                SetPixel(x, y);
                return true;
            }
            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x;
            y = (int)GenerateMap.GetGenerateMap.borderPointList[i].y - 1;
            if (IsBorderPoint(x, y))
            {
                GenerateMap.GetGenerateMap.borderPointList.Add(new Vector2(x, y));
                SetPixel(x, y);
                return true;
            }
            



            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x - 1;
            y = (int)GenerateMap.GetGenerateMap.borderPointList[i].y + 1;
            if (IsBorderPoint(x, y))
            {
                GenerateMap.GetGenerateMap.borderPointList.Add(new Vector2(x, y));
                SetPixel(x, y);
                return true;
            }
            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x + 1;
            y = (int)GenerateMap.GetGenerateMap.borderPointList[i].y + 1;
            if (IsBorderPoint(x, y))
            {
                GenerateMap.GetGenerateMap.borderPointList.Add(new Vector2(x, y));
                SetPixel(x, y);
                return true;
            }
            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x + 1;
            y = (int)GenerateMap.GetGenerateMap.borderPointList[i].y - 1;
            if (IsBorderPoint(x, y))
            {
                GenerateMap.GetGenerateMap.borderPointList.Add(new Vector2(x, y));
                SetPixel(x, y);
                return true;
            }
            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x - 1;
            y = (int)GenerateMap.GetGenerateMap.borderPointList[i].y - 1;
            if (IsBorderPoint(x, y))
            {
                GenerateMap.GetGenerateMap.borderPointList.Add(new Vector2(x, y));
                SetPixel(x, y);
                return true;
            }
        }
        return false;
    }

    bool IsBorderPoint(int x, int y)
    {
        Color color;
        color = GenerateMap.GetGenerateMap.map.GetPixel(x, y);
        if (color == GenerateMap.GetGenerateMap.mapColor)
        {
            color = GenerateMap.GetGenerateMap.map.GetPixel(x - 1, y);
            if (color != GenerateMap.GetGenerateMap.mapColor && color != GenerateMap.GetGenerateMap.changeColor)
                return true;

            color = GenerateMap.GetGenerateMap.map.GetPixel(x, y + 1);
            if (color != GenerateMap.GetGenerateMap.mapColor && color != GenerateMap.GetGenerateMap.changeColor)
                return true;

            color = GenerateMap.GetGenerateMap.map.GetPixel(x + 1, y);
            if (color != GenerateMap.GetGenerateMap.mapColor && color != GenerateMap.GetGenerateMap.changeColor)
                return true;

            color = GenerateMap.GetGenerateMap.map.GetPixel(x, y - 1);
            if (color != GenerateMap.GetGenerateMap.mapColor && color != GenerateMap.GetGenerateMap.changeColor)
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
        GenerateMap.GetGenerateMap.borderPointList.Clear();
        Vector2 vector = GetBorderPoint(x, y);
        if (!vector.Equals(Vector2.zero))
        {
            if (vector.x >= 0)
            {
                GenerateMap.GetGenerateMap.borderPointList.Add(vector);
                bool isMore = true;
                int timer = 0;
                while (isMore)
                {
                    isMore = false;

                    if (AddMorePoint())
                    {
                        isMore = true;
                    }

                    if (timer >= 100)
                    {
                        timer = 0;
                        GenerateMap.GetGenerateMap.map.Apply();
                        yield return new WaitForFixedUpdate();
                    }
                    timer++;
                }
            }
        }
        GenerateMap.GetGenerateMap.isGetPointOver = true;
    }
}

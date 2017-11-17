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

    bool AddMorePoint()
    {
        int x, y;
        for (int i = GenerateMap.GetGenerateMap.borderPointList.Count - 1; i > -1; i--)
        {
            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x - 1;
            y = (int)GenerateMap.GetGenerateMap.borderPointList[i].y;
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

            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x;
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
            y = (int)GenerateMap.GetGenerateMap.borderPointList[i].y;
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

            x = (int)GenerateMap.GetGenerateMap.borderPointList[i].x;
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

    Vector2 GetBorderPoint(int x,int y)
    {
        while (!IsBorderPoint(x, y))
        {
            x--;
        }
        return new Vector2(x, y);
    }

    public IEnumerator GetCityFromPoint(int x, int y)
    {
        GenerateMap.GetGenerateMap.borderPointList.Clear();
        GenerateMap.GetGenerateMap.borderPointList.Add(GetBorderPoint(x, y));
        bool isMore = true;
        int timer = 0;
        while (isMore)
        {
            isMore = false;

            if (AddMorePoint())
            {
                isMore = true;
            }

            if (timer > 1)
            {
                timer = 0;
                GenerateMap.GetGenerateMap.map.Apply();
                yield return new WaitForFixedUpdate();
            }
            timer++;
        }
        GenerateMap.GetGenerateMap.isGetPointOver = true;
    }
}

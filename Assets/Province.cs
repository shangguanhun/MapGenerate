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

    void TryAddPoint(int x, int y)
    {
        if (GenerateMap.GetGenerateMap.map.GetPixel(x, y) == GenerateMap.GetGenerateMap.mapColor)
        {
            GenerateMap.GetGenerateMap.pointList.Add(new Vector2(x, y));
        }
    }

    void TrySetPixel(int x, int y)
    {
        if (GenerateMap.GetGenerateMap.map.GetPixel(x, y) == GenerateMap.GetGenerateMap.mapColor)
        {
            SetPixel(x, y);
            TryAddPoint(x - 1, y);
            TryAddPoint(x + 1, y);
            TryAddPoint(x, y + 1);
            TryAddPoint(x, y - 1);
        }
        else
        {
            GenerateMap.GetGenerateMap.borderPointList.Add(new Vector2(x, y));
        }
    }

    public IEnumerator GetCityFromPoint(int x, int y)
    {
        GenerateMap.GetGenerateMap.pointList.Add(new Vector2(x, y));
        GenerateMap.GetGenerateMap.borderPointList.Clear();
        int timer = 0;
        while (GenerateMap.GetGenerateMap.pointList.Count > 0)
        {
            Vector2 point = GenerateMap.GetGenerateMap.pointList[0];
            TrySetPixel((int)point.x, (int)point.y);
            GenerateMap.GetGenerateMap.pointList.RemoveAt(0);
            if (timer > 100)
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

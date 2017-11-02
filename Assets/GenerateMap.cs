using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMap : MonoBehaviour {

    public Color borderColor = Color.white;
    public Color changeColor = Color.red;
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
        map.SetPixel(x, y, Color.red);
        map.Apply();
    }

    void TryAddPoint(int x,int y)
    {
        if(map.GetPixel(x,y) != changeColor)
        {
            pointList.Add(new Vector2(x, y));
        }
    }

    void TrySetPixel(int x,int y)
    {
        if (map.GetPixel(x, y) != changeColor)
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
        while (pointList.Count > 0)
        {
            Vector2 point = pointList[0];
            pointList.RemoveAt(0);
            TrySetPixel((int)point.x, (int)point.y);
            yield return new WaitForFixedUpdate();
        }
        isGetPointOver = true;
    }

    IEnumerator makeCityMesh(int x,int y)
    {
        Mesh mesh = new Mesh();
        foreach (Vector2 vec in borderPointList)
        {
            yield return new WaitForFixedUpdate();
        }
        isMakeMeshOver = true;
    }

    IEnumerator GetCitys()
    {
        for(int i = 0; i < mapSizeX; i+=10)
        {
            for (int j = 0; j < mapSizeY; j += 10)
            {
                if (map.GetPixel(i, j) != changeColor && map.GetPixel(i, j) != borderColor)
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

using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 与几何相关的辅助类
/// </summary>
/// 
internal class PointF
{
    public double X, Y;
    public PointF(double x, double y)
    {
        X = x;
        Y = y;
    }
}

internal class GeometryHelper
{
    public static bool IsInPolygon(PointF checkPoint, List<PointF> polygonPoints)
    {
        bool inside = false;
        int pointCount = polygonPoints.Count;
        PointF p1, p2;
        for (int i = 0, j = pointCount - 1; i < pointCount; j = i, i++) 
        {
            p1 = polygonPoints[i];
            p2 = polygonPoints[j];
            if (checkPoint.Y < p2.Y)
            {//p2在射线之上  
                if (p1.Y <= checkPoint.Y)
                {//p1正好在射线中或者射线下方  
                    if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) > (checkPoint.X - p1.X) * (p2.Y - p1.Y))//斜率判断,在P1和P2之间且在P1P2右侧  
                    {
                        //射线与多边形交点为奇数时则在多边形之内，若为偶数个交点时则在多边形之外。  
                        //由于inside初始值为false，即交点数为零。所以当有第一个交点时，则必为奇数，则在内部，此时为inside=(!inside)  
                        //所以当有第二个交点时，则必为偶数，则在外部，此时为inside=(!inside)  
                        inside = (!inside);
                    }
                }
            }
            else if (checkPoint.Y < p1.Y)
            {
                //p2正好在射线中或者在射线下方，p1在射线上  
                if ((checkPoint.Y - p1.Y) * (p2.X - p1.X) < (checkPoint.X - p1.X) * (p2.Y - p1.Y))//斜率判断,在P1和P2之间且在P1P2右侧  
                {
                    inside = (!inside);
                }
            }
        }
        return inside;
    }
}
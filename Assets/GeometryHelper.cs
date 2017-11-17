using System;
using System.Collections.Generic;
/// <summary>
/// 与几何相关的辅助类
/// </summary>
/// 
internal class PointF
{
    public float X, Y;
    public PointF(float x, float y)
    {
        X = x;
        Y = y;
    }
}

internal class Line
{
    public PointF p1, p2;
    public float X1, X2, Y1, Y2;
    public Line(PointF p1, PointF p2)
    {
        this.p1 = p1;
        this.p2 = p2;
        X1 = p1.X;
        X2 = p2.X;
        Y1 = p1.Y;
        Y2 = p2.Y;
    }
}

internal class GeometryHelper
{

    /// <summary>
    /// 判断线段与多边形的关系
    /// </summary>
    /// <param name="line"></param>
    /// <param name="polygon"></param>
    /// <returns></returns>
    public static Intersection IntersectionOf(Line line, List<PointF> polygon)
    {
        if (polygon.Count == 0)
        {
            return Intersection.None;
        }
        if (polygon.Count == 1)
        {
            return IntersectionOf(polygon[0], line);
        }
        bool tangent = false;
        for (int index = 0; index < polygon.Count; index++)
        {
            int index2 = (index + 1) % polygon.Count;
            Intersection intersection = IntersectionOf(line, new Line(polygon[index], polygon[index2]));
            if (intersection == Intersection.Intersection)
            {
                return intersection;
            }
            if (intersection == Intersection.Tangent)
            {
                tangent = true;
            }
        }
        return tangent ? Intersection.Tangent : IntersectionOf(line.p1, polygon);
    }
    /// <summary>
    /// 判断点与多边形的关系
    /// </summary>
    /// <param name="point"></param>
    /// <param name="polygon"></param>
    /// <returns></returns>
    public static Intersection IntersectionOf(PointF point, List<PointF> polygon)
    {
        switch (polygon.Count)
        {
            case 0:
                return Intersection.None;
            case 1:
                if (polygon[0].X == point.X && polygon[0].Y == point.Y)
                {
                    return Intersection.Tangent;
                }
                else
                {
                    return Intersection.None;
                }
            case 2:
                return IntersectionOf(point, new Line(polygon[0], polygon[1]));
        }

        int counter = 0;
        int i;
        PointF p1;
        int n = polygon.Count;
        p1 = polygon[0];
        if (point == p1)
        {
            return Intersection.Tangent;
        }

        for (i = 1; i <= n; i++)
        {
            PointF p2 = polygon[i % n];
            if (point == p2)
            {
                return Intersection.Tangent;
            }
            if (point.Y > Math.Min(p1.Y, p2.Y))
            {
                if (point.Y <= Math.Max(p1.Y, p2.Y))
                {
                    if (point.X <= Math.Max(p1.X, p2.X))
                    {
                        if (p1.Y != p2.Y)
                        {
                            double xinters = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                            if (p1.X == p2.X || point.X <= xinters)
                                counter++;
                        }
                    }
                }
            }
            p1 = p2;
        }

        return (counter % 2 == 1) ? Intersection.Containment : Intersection.None;
    }
    /// <summary>
    /// 判断点与直线的关系
    /// </summary>
    /// <param name="point"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    public static Intersection IntersectionOf(PointF point, Line line)
    {
        float bottomY = Math.Min(line.Y1, line.Y2);
        float topY = Math.Max(line.Y1, line.Y2);
        bool heightIsRight = point.Y >= bottomY &&
                             point.Y <= topY;
        //Vertical line, slope is divideByZero error!
        if (line.X1 == line.X2)
        {
            if (point.X == line.X1 && heightIsRight)
            {
                return Intersection.Tangent;
            }
            else
            {
                return Intersection.None;
            }
        }
        float slope = (line.X2 - line.X1) / (line.Y2 - line.Y1);
        bool onLine = (line.Y1 - point.Y) == (slope * (line.X1 - point.X));
        if (onLine && heightIsRight)
        {
            return Intersection.Tangent;
        }
        else
        {
            return Intersection.None;
        }
    }
    /// <summary>
    /// 判断直线与直线的关系
    /// </summary>
    /// <param name="line1"></param>
    /// <param name="line2"></param>
    /// <returns></returns>
    public static Intersection IntersectionOf(Line line1, Line line2)
    {
        //  Fail if either line segment is zero-length.
        if (line1.X1 == line1.X2 && line1.Y1 == line1.Y2 || line2.X1 == line2.X2 && line2.Y1 == line2.Y2)
            return Intersection.None;

        if (line1.X1 == line2.X1 && line1.Y1 == line2.Y1 || line1.X2 == line2.X1 && line1.Y2 == line2.Y1)
            return Intersection.Intersection;
        if (line1.X1 == line2.X2 && line1.Y1 == line2.Y2 || line1.X2 == line2.X2 && line1.Y2 == line2.Y2)
            return Intersection.Intersection;

        //  (1) Translate the system so that point A is on the origin.
        line1.X2 -= line1.X1; line1.Y2 -= line1.Y1;
        line2.X1 -= line1.X1; line2.Y1 -= line1.Y1;
        line2.X2 -= line1.X1; line2.Y2 -= line1.Y1;

        //  Discover the length of segment A-B.
        double distAB = Math.Sqrt(line1.X2 * line1.X2 + line1.Y2 * line1.Y2);

        //  (2) Rotate the system so that point B is on the positive X axis.
        double theCos = line1.X2 / distAB;
        double theSin = line1.Y2 / distAB;
        double newX = line2.X1 * theCos + line2.Y1 * theSin;
        line2.Y1 = (float)(line2.Y1 * theCos - line2.X1 * theSin); line2.X1 = (float)newX;
        newX = line2.X2 * theCos + line2.Y2 * theSin;
        line2.Y2 = (float)(line2.Y2 * theCos - line2.X2 * theSin); line2.X2 = (float)newX;

        //  Fail if segment C-D doesn't cross line A-B.
        if (line2.Y1 < 0 && line2.Y2 < 0 || line2.Y1 >= 0 && line2.Y2 >= 0)
            return Intersection.None;

        //  (3) Discover the position of the intersection point along line A-B.
        double posAB = line2.X2 + (line2.X1 - line2.X2) * line2.Y2 / (line2.Y2 - line2.Y1);

        //  Fail if segment C-D crosses line A-B outside of segment A-B.
        if (posAB < 0 || posAB > distAB)
            return Intersection.None;

        //  (4) Apply the discovered position to line A-B in the original coordinate system.
        return Intersection.Intersection;
    }

    /// <summary>
    /// 几何体之间的关系类型
    /// </summary>
    public enum Intersection
    {
        None,
        Tangent,
        Intersection,
        Containment
    }
}
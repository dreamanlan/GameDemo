﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GameLibrary;

namespace GameLibrary
{
    public partial class Geometry
    {
        public const float c_MinDistance = 0.0001f;
        public const float c_FloatPrecision = 0.0001f;
        public const float c_DoublePrecision = 0.0000001f;
        public static float RadianToDegree(float dir)
        {
            return (float)(dir * 180 / Math.PI);
        }
        public static float DegreeToRadian(float dir)
        {
            return (float)(dir * Math.PI / 180);
        }
        public static bool IsInvalid(float v)
        {
            return float.IsNaN(v) || float.IsInfinity(v);
        }
        public static bool IsInvalid(Vector2 pos)
        {
            return IsInvalid(pos.x) || IsInvalid(pos.y);
        }
        public static bool IsInvalid(Vector3 pos)
        {
            return IsInvalid(pos.x) || IsInvalid(pos.y) || IsInvalid(pos.z);
        }
        public static float Max(float a, float b)
        {
            return (a > b ? a : b);
        }
        public static float Min(float a, float b)
        {
            return (a < b ? a : b);
        }
        public static float DistanceSquare(float x1, float y1, float x2, float y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }
        public static float DistanceSquare(Vector2 p1, Vector2 p2)
        {
            return (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y);
        }
        public static float Distance(Vector2 p1, Vector2 p2)
        {
            return (float)Math.Sqrt(DistanceSquare(p1, p2));
        }
        public static float DistanceSquare(Vector3 p1, Vector3 p2)
        {
            return (p1.x - p2.x) * (p1.x - p2.x) + (p1.z - p2.z) * (p1.z - p2.z);
        }
        public static float DistanceSquare3D(Vector3 p1, Vector3 p2)
        {
            return (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y) + (p1.z - p2.z) * (p1.z - p2.z);
        }
        public static float Distance(Vector3 p1, Vector3 p2)
        {
            return (float)Math.Sqrt(DistanceSquare(p1, p2));
        }
        public static float Distance3D(Vector3 p1, Vector3 p2)
        {
            return (float)Mathf.Sqrt(DistanceSquare3D(p1, p2));
        }
        public static float CalcLength(IList<Vector2> pts)
        {
            float len = 0;
            for (int i = 0; i < pts.Count - 1; ++i) {
                len += Distance(pts[i], pts[i + 1]);
            }
            return len;
        }
        public static float CalcLength(IList<Vector3> pts)
        {
            float len = 0;
            for (int i = 0; i < pts.Count - 1; ++i) {
                len += Distance(pts[i], pts[i + 1]);
            }
            return len;
        }

        public static bool IsNearZero(float checkZero)
        {
            return IsSameFloat(checkZero,0);
        }

        public static bool IsSameFloat(float v1, float v2)
        {
            return Math.Abs(v1 - v2) < c_FloatPrecision;
        }
        public static bool IsSameDouble(double v1, double v2)
        {
            return Math.Abs(v1 - v2) < c_DoublePrecision;
        }
        public static bool IsSamePoint(Vector2 p1, Vector2 p2)
        {
            return IsSameFloat(p1.x, p2.x) && IsSameFloat(p1.y, p2.y);
        }
        public static bool IsSamePoint(Vector3 p1, Vector3 p2)
        {
            return IsSameFloat(p1.x, p2.x) && IsSameFloat(p1.y, p2.y) && IsSameFloat(p1.z, p2.z);
        }

        public static float DistanceIn2D(Vector3 p1, Vector3 p2)
        {
            float sqr = (p1.x - p2.x) * (p1.x - p2.x) + (p1.z - p2.z) * (p1.z - p2.z);
            return (float)Math.Sqrt(sqr);
        }
    }
    public partial class Geometry
    {
        #region miloyip "A Brief Analysis of Sector and Disk Intersection Test"
        //-------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------The following functions come from miloyip's "A Brief Analysis of Sector and Disk Intersection Tests"----------------------------------------
        /// <summary>
        /// Calculate the shortest square distance between a line segment and a point
        /// x0 starting point of line segment
        /// u  Line segment direction to end point
        /// x  any point
        /// </summary>
        /// <returns></returns>
        public static float SegmentPointSqrDistance(Vector2 x0, Vector2 u, Vector2 x)
        {
            float t = Vector2.Dot(x - x0, u) / u.sqrMagnitude;
            return (x - (x0 + Mathf.Clamp(t, 0, 1) * u)).sqrMagnitude;
        }
        /// <summary>
        /// x0 Starting point of capsule line segment
        /// u  Capsule line segment direction to end point
        /// cr capsule radius
        /// c Disk center
        /// r Disk radius
        /// </summary>
        /// <returns></returns>
        public static bool IsCapsuleDiskIntersect(Vector2 x0, Vector2 u, float cr, Vector2 c, float r)
        {
            return SegmentPointSqrDistance(x0, u, c) <= (cr + r) * (cr + r);
        }
        /// <summary>
        /// c AABB Center
        /// h AABB half length
        /// p center of disc
        /// r radius of disk
        /// </summary>
        public static bool IsAabbDiskIntersect(Vector2 c, Vector2 h, Vector2 p, float r) {
            Vector2 v = Vector2.Max(p - c, c - p); // = Abs(p - c);
            Vector2 u = Vector2.Max(v - h, Vector2.zero);
            return u.sqrMagnitude<= r * r;
        }
        /// <summary>
        /// Intersection of circle and rectangle
        /// </summary>
        /// <param name="c">center of rectangle</param>
        /// <param name="h">Rectangle half length</param>
        /// <param name="angle">Rectangular direction</param>
        /// <param name="p">center of circle</param>
        /// <param name="r">circle radius</param>
        /// <returns></returns>
        public static bool IsObbDiskIntersect(Vector2 c, Vector2 h, float angle, Vector2 p, float r)
        {
            Vector2 nc = GetRotate(c, -angle);
            Vector2 np = GetRotate(p, -angle);
            return IsAabbDiskIntersect(nc, h, np, r);
        }
        ///<summary>
        /// Sector and disk intersection test
        /// a Sector center
        /// u Sector direction (unit vector)
        /// theta Sector Sweep Half Angle
        /// l Sector side length
        /// c Disk center
        /// r Disk radius
        ///</summary>
        public static bool IsSectorDiskIntersect(Vector2 a, Vector2 u, float theta, float l, Vector2 c, float r)
        {
            //UnityEngine.LogSystem.Warn("IsSectorDiskIntersect,u:" + u);
            //UnityEngine.LogSystem.Warn("a:" + a + ",u:" + u + ",theta:" + theta + ",l" + l + ",c" + c + ",r:" + r);
            // 1. If the directions of the center of the sector and the center of the disk can be separated, the two shapes will not intersect.
            Vector2 d = c - a;
            float rsum = l + r;
            if (d.sqrMagnitude > rsum * rsum)
            {
                //UnityEngine.LogSystem.Warn("IsSectorDiskIntersect return false:" + d.sqrMagnitude + "," + rsum + "*" + rsum + "," + (rsum* rsum));
                return false;
            }

            // 2. Calculate p of the sector local space
            float px = Vector2.Dot(d, u);
            float py = Mathf.Abs(Vector2.Dot(d, new Vector2(-u.y, u.x)));

            // 3. If p_x > ||p|| cos theta, the two shapes intersect
            if (px > d.magnitude * Mathf.Cos(theta))
            {
                //UnityEngine.LogSystem.Warn("IsSectorDiskIntersect return true,px > d.magnitude * Mathf.Cos(theta)");
                return true;
            }

            // 4. Find whether the left line segment intersects the disk
            Vector2 q = l * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            Vector2 p = new Vector2(px, py);
            
            if(SegmentPointSqrDistance(Vector2.zero, q, p) <= r * r)
            {
                //UnityEngine.LogSystem.Warn("SegmentPointSqrDistance(Vector2.zero, q, p) <= r * r true," + SegmentPointSqrDistance(Vector2.zero, q, p) + " <= " + (r * r));
            }
            else
            {
                //UnityEngine.LogSystem.Warn("SegmentPointSqrDistance(Vector2.zero, q, p) <= r * r false," + SegmentPointSqrDistance(Vector2.zero, q, p) + " > " + (r * r));
            }

            return SegmentPointSqrDistance(Vector2.zero, q, p) <= r * r;
        }
        //-------------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
    //The coordinate system is a left-handed coordinate system.
    //The angle of 0 degrees is the positive direction of the z-axis.
    //The rotation direction is the positive direction of the z-axis and the positive
    //direction of the x-axis
    //(counterclockwise rotation when viewed along the positive y-axis).
    public partial class Geometry
    {
        public static float GetYRadian(Vector2 fvPos1, Vector2 fvPos2)
        {
            float dDistance = (float)Vector2.Distance(fvPos1, fvPos2);
            if (dDistance <= 0.0f)
                return 0.0f;

            float fACos = (fvPos2.y - fvPos1.y) / dDistance;
            if (fACos > 1.0)
                fACos = 0.0f;
            else if (fACos < -1.0)
                fACos = (float)Math.PI;
            else
                fACos = (float)Math.Acos(fACos);

            // [0~180]
            if (fvPos2.x >= fvPos1.x)
                return (float)fACos;
            //(180, 360)
            else
                return (float)(Math.PI * 2 - fACos);
        }
        public static Vector2 GetRotate(Vector2 fvPos1, float radian)
        {
            radian = -radian;
            // pos1 -> pos2
            Vector2 retV = new Vector2(
                (float)(fvPos1.x * Math.Cos(radian) - fvPos1.y * Math.Sin(radian)),
                (float)(fvPos1.x * Math.Sin(radian) + fvPos1.y * Math.Cos(radian))
                );
            return retV;
        }
        public static float GetYRadian(UnityEngine.Vector3 pos1, UnityEngine.Vector3 pos2)
        {
            return GetYRadian(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        }
        public static Vector3 GetRotate(Vector3 pos, float radian)
        {
            Vector2 newPos = GetRotate(new Vector2(pos.x, pos.z), radian);
            return new Vector3(newPos.x, 0, newPos.y);
        }
        public static Vector3 TransformPoint(Vector3 pos, Vector3 offset, float radian)
        {
            Vector3 newOffset = Geometry.GetRotate(offset, radian);
            newOffset.y = offset.y;
            Vector3 result = pos + newOffset;
            return result;
        }
    }
    public partial class Geometry
    {
        public static bool IsOnLeft(Vector2 v, Vector2 p)
        {
            return Vector3.Dot(p, new Vector2(-v.y, v.x)) > 0;
        }

        /// <summary>
        /// Compute vector cross product
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="ep"></param>
        /// <param name="op"></param>
        /// <returns>
        /// ret>0 ep in opsp vector counterclockwise direction
        /// ret=0 collinear
        /// ret<0 ep in opsp vector clockwise direction
        /// </returns>
        public static float Multiply(Vector2 sp, Vector2 ep, Vector2 op)
        {
            return ((sp.x - op.x) * (ep.y - op.y) - (ep.x - op.x) * (sp.y - op.y));
        }
        /// <summary>
        /// r=DotMultiply(p1,p2,p0), get the dot product of vectors (p1-p0) and (p2-p0).
        /// Note: Both vectors must be non-zero vectors
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p0"></param>
        /// <returns>
        /// r>0:The angle between two vectors is an acute angle;
        /// r=0:The angle between the two vectors is a right angle;
        /// r<0:The angle between two vectors is an obtuse angle
        /// </returns>
        public static float DotMultiply(Vector2 p1, Vector2 p2, Vector2 p0)
        {
            return ((p1.x - p0.x) * (p2.x - p0.x) + (p1.y - p0.y) * (p2.y - p0.y));
        }
        /// <summary>
        /// Determine the relationship between p and line segment p1p2
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns>
        /// r=0 p = p1
        /// r=1 p = p2
        /// r<0 p is on the backward extension of p1p2
        /// r>1 p is on the forward extension of p1p2
        /// 0<r<1 p is interior to p1p2
        /// </returns>
        public static float Relation(Vector2 p, Vector2 p1, Vector2 p2)
        {
            return DotMultiply(p, p2, p1) / DistanceSquare(p1, p2);
        }
        /// <summary>
        /// Ask for a foothold
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector2 Perpendicular(Vector2 p, Vector2 p1, Vector2 p2)
        {
            float r = Relation(p, p1, p2);
            Vector2 tp = new Vector2();
            tp.x = p1.x + r * (p2.x - p1.x);
            tp.y = p1.y + r * (p2.y - p1.y);
            return tp;
        }
        /// <summary>
        /// Finds the minimum distance from a point to a line segment and
        /// returns the closest point.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="np"></param>
        /// <returns></returns>
        public static float PointToLineSegmentDistance(Vector2 p, Vector2 p1, Vector2 p2, out Vector2 np)
        {
            float r = Relation(p, p1, p2);
            if (r < 0) {
                np = p1;
                return Distance(p, p1);
            }
            if (r > 1) {
                np = p2;
                return Distance(p, p2);
            }
            np = Perpendicular(p, p1, p2);
            return Distance(p, np);
        }
        public static float PointToLineSegmentDistanceSquare(Vector2 p, Vector2 p1, Vector2 p2, out Vector2 np)
        {
            float r = Relation(p, p1, p2);
            if (r < 0) {
                np = p1;
                return DistanceSquare(p, p1);
            }
            if (r > 1) {
                np = p2;
                return DistanceSquare(p, p2);
            }
            np = Perpendicular(p, p1, p2);
            return DistanceSquare(p, np);
        }
        /// <summary>
        /// Find the distance from a point to a straight line
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float PointToLineDistance(Vector2 p, Vector2 p1, Vector2 p2)
        {
            return Math.Abs(Multiply(p, p2, p1)) / Distance(p1, p2);
        }
        public static float PointToLineDistanceInverse(Vector2 p, Vector2 p1, Vector2 p2)
        {
            return Math.Abs(Multiply(p, p2, p1)) / Distance(p1, p2);
        }
        /// <summary>
        /// Find the minimum distance from the point to the polyline and return the minimum distance point.
        /// Note: If the given point is not enough to form a polyline, the value of q will not be modified.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="pts"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static float PointToPolylineDistance(Vector2 p, IList<Vector2> pts, int start, int len, ref Vector2 q)
        {
            float ret = float.MaxValue;
            for (int i = start; i < start + len - 1; ++i) {
                Vector2 tq;
                float d = PointToLineSegmentDistance(p, pts[i], pts[i + 1], out tq);
                if (d < ret) {
                    ret = d;
                    q = tq;
                }
            }
            return ret;
        }
        public static float PointToPolylineDistanceSquare(Vector2 p, IList<Vector2> pts, int start, int len, ref Vector2 q)
        {
            float ret = float.MaxValue;
            for (int i = start; i < start + len - 1; ++i) {
                Vector2 tq;
                float d = PointToLineSegmentDistanceSquare(p, pts[i], pts[i + 1], out tq);
                if (d < ret) {
                    ret = d;
                    q = tq;
                }
            }
            return ret;
        }
        public static bool Intersect(Vector2 us, Vector2 ue, Vector2 vs, Vector2 ve)
        {
            return ((Max(us.x, ue.x) >= Min(vs.x, ve.x)) &&
            (Max(vs.x, ve.x) >= Min(us.x, ue.x)) &&
            (Max(us.y, ue.y) >= Min(vs.y, ve.y)) &&
            (Max(vs.y, ve.y) >= Min(us.y, ue.y)) &&
            (Multiply(vs, ue, us) * Multiply(ue, ve, us) >= 0) &&
            (Multiply(us, ve, vs) * Multiply(ve, ue, vs) >= 0));
        }

        //Returns 1 when there is no collision, returns a value of 0~1 when there is a collision
        //The collision position is lerp(from,to,t)
        //https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282
        public static float Intersect2(Vector2 from, Vector2 to, Vector2 a, Vector2 b) { 
            float s_numer, t_numer, denom, t;
            Vector2 s10 = to - from;
            Vector2 sba = b - a;

            denom = s10.x * sba.y - sba.x * s10.y;
            if (denom == 0)
                return 1; // Collinear
            bool denomPositive = denom > 0;

            Vector2 s02 = from - a;
            s_numer = s10.x * s02.y - s10.y * s02.x;
            if ((s_numer < 0) == denomPositive)
                return 1; // No collision

            t_numer = sba.x * s02.y - sba.y * s02.x;
            if ((t_numer < 0) == denomPositive)
                return 1; // No collision

            if (((s_numer > denom) == denomPositive) || ((t_numer > denom) == denomPositive))
                return 1; // No collision
                          // Collision detected
            t = t_numer / denom;
            return t;
        }

        public static bool LineIntersectRectangle(Vector2 lines, Vector2 linee, float left, float top, float right, float bottom)
        {
            if (Max(lines.x, linee.x) >= left && right >= Min(lines.x, linee.x) && Max(lines.y, linee.y) >= top && bottom >= Min(lines.y, linee.y)) {
                Vector2 lt = new Vector2(left, top);
                Vector2 rt = new Vector2(right, top);
                Vector2 rb = new Vector2(right, bottom);
                Vector2 lb = new Vector2(left, bottom);
                if (Multiply(lines, lt, linee) * Multiply(lines, rt, linee) <= 0)
                    return true;
                if (Multiply(lines, lt, linee) * Multiply(lines, lb, linee) <= 0)
                    return true;
                if (Multiply(lines, rt, linee) * Multiply(lines, rb, linee) <= 0)
                    return true;
                if (Multiply(lines, lb, linee) * Multiply(lines, rb, linee) <= 0)
                    return true;
                /*
                if (Intersect(lines, linee, lt, rt))
                  return true;
                if (Intersect(lines, linee, rt, rb))
                  return true;
                if (Intersect(lines, linee, rb, lb))
                  return true;
                if (Intersect(lines, linee, lt, lb))
                  return true;
                */
            }
            return false;
        }
        public static bool PointOnLine(Vector2 pt, Vector2 pt1, Vector2 pt2)
        {
            float dx, dy, dx1, dy1;
            bool retVal;
            dx = pt2.x - pt1.x;
            dy = pt2.y - pt1.y;
            dx1 = pt.x - pt1.x;
            dy1 = pt.y - pt1.y;
            if (pt.x == pt1.x && pt.y == pt1.y || pt.x == pt2.x && pt.y == pt2.y) {//on the vertex
                retVal = true;
            } else {
                if (Math.Abs(dx * dy1 - dy * dx1) < c_FloatPrecision) {//Slopes are equal//Consider calculation errors
                    //equals zero at the vertex
                    if (dx1 * (dx1 - dx) < 0 | dy1 * (dy1 - dy) < 0) {
                        retVal = true;
                    } else {
                        retVal = false;
                    }
                } else {
                    retVal = false;
                }
            }
            return retVal;
        }
        /// <summary>
        /// Determine whether two points coincide with each other
        /// </summary>
        /// <param name="pt0"></param>
        /// <param name="pt1"></param>
        /// <returns></returns>
        public static bool PointOverlapPoint(Vector2 pt0, Vector2 pt1)
        {
            if (Distance(pt0, pt1) < c_MinDistance)
                return true;
            else
                return false;
        }
        public static bool RectangleOverlapRectangle(float _x1, float _y1, float _x2, float _y2, float x1, float y1, float x2, float y2)
        {
            if (_x1 > x2 + c_FloatPrecision)
                return false;
            if (_x2 < x1 - c_FloatPrecision)
                return false;
            if (_y1 > y2 + c_FloatPrecision)
                return false;
            if (_y2 < y1 - c_FloatPrecision)
                return false;
            return true;
        }
        /// <summary>
        /// Determine whether a is to the left of line bc
        /// </summary>
        public static bool PointIsLeft(Vector2 a, Vector2 b, Vector2 c)
        {
            return Multiply(c, a, b) > 0;
        }
        /// <summary>
        /// Determine whether a is to the left or on the line bc
        /// </summary>
        public static bool PointIsLeftOn(Vector2 a, Vector2 b, Vector2 c)
        {
            return Multiply(c, a, b) >= 0;
        }
        /// <summary>
        /// Determine whether the three points a, b, and c are collinear
        /// </summary>
        public static bool PointIsCollinear(Vector2 a, Vector2 b, Vector2 c)
        {
            return Multiply(c, a, b) == 0;
        }
        public static bool IsCounterClockwise(IList<Vector2> pts, int start, int len)
        {
            bool ret = false;
            if (len >= 3) {
                float maxx = pts[start].x;
                float maxy = pts[start].y;
                float minx = maxx;
                float miny = maxy;
                int maxxi = 0;
                int maxyi = 0;
                int minxi = 0;
                int minyi = 0;
                for (int i = 1; i < len; ++i) {
                    int ix = start + i;
                    float x = pts[ix].x;
                    float y = pts[ix].y;
                    if (maxx < x) {
                        maxx = x;
                        maxxi = i;
                    } else if (minx > x) {
                        minx = x;
                        minxi = i;
                    }
                    if (maxy < y) {
                        maxy = y;
                        maxyi = i;
                    } else if (miny > y) {
                        miny = y;
                        minyi = i;
                    }
                }
                int val = 0;
                for (int delt = 0; delt < len; delt++) {
                    int ix0 = start + (len * 2 + maxxi - delt - 1) % len;
                    int ix1 = start + maxxi;
                    int ix2 = start + (maxxi + delt + 1) % len;

                    float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
                    if (r > 0) {
                        val++;
                        break;
                    } else if (r < 0) {
                        val--;
                        break;
                    }
                }
                for (int delt = 0; delt < len; delt++) {
                    int ix0 = start + (len * 2 + maxyi - delt - 1) % len;
                    int ix1 = start + maxyi;
                    int ix2 = start + (maxyi + delt + 1) % len;

                    float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
                    if (r > 0) {
                        val++;
                        break;
                    } else if (r < 0) {
                        val--;
                        break;
                    }
                }
                for (int delt = 0; delt < len; delt++) {
                    int ix0 = start + (len * 2 + minxi - delt - 1) % len;
                    int ix1 = start + minxi;
                    int ix2 = start + (minxi + delt + 1) % len;

                    float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
                    if (r > 0) {
                        val++;
                        break;
                    } else if (r < 0) {
                        val--;
                        break;
                    }
                }
                for (int delt = 0; delt < len; delt++) {
                    int ix0 = start + (len * 2 + minyi - delt - 1) % len;
                    int ix1 = start + minyi;
                    int ix2 = start + (minyi + delt + 1) % len;

                    float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
                    if (r > 0) {
                        val++;
                        break;
                    } else if (r < 0) {
                        val--;
                        break;
                    }
                }
                if (val > 0)
                    ret = true;
                else
                    ret = false;
            }
            return ret;
        }
        /// <summary>
        /// Calculate polygon centroid and radius
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public static float CalcPolygonCentroidAndRadius(IList<Vector2> pts, int start, int len, out Vector2 centroid)
        {
            float ret = 0;
            if (len > 0) {
                float x = 0;
                float y = 0;
                for (int i = 1; i < len; ++i) {
                    int ix = start + i;
                    x += pts[ix].x;
                    y += pts[ix].y;
                }
                x /= len;
                y /= len;
                centroid = new Vector2(x, y);
                float distSqr = 0;
                for (int i = 1; i < len; ++i) {
                    int ix = start + i;
                    float tmp = Geometry.DistanceSquare(centroid, pts[ix]);
                    if (distSqr < tmp)
                        distSqr = tmp;
                }
                ret = (float)Math.Sqrt(distSqr);
            } else {
                centroid = new Vector2();
            }
            return ret;
        }
        /// <summary>
        /// Compute polygonal axis-aligned rectangular bounding box
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="minXval"></param>
        /// <param name="maxXval"></param>
        /// <param name="minZval"></param>
        /// <param name="maxZval"></param>
        public static void CalcPolygonBound(IList<Vector2> pts, int start, int len, out float minXval, out float maxXval, out float minYval, out float maxYval)
        {
            maxXval = pts[start].x;
            minXval = pts[start].x;
            maxYval = pts[start].y;
            minYval = pts[start].y;
            for (int i = 1; i < len; ++i) {
                int ix = start + i;
                float xv = pts[ix].x;
                float yv = pts[ix].y;
                if (maxXval < xv)
                    maxXval = xv;
                else if (minXval > xv)
                    minXval = xv;
                if (maxYval < yv)
                    maxYval = yv;
                else if (minYval > yv)
                    minYval = yv;
            }
        }
        /// <summary>
        /// Determine whether a point is within a polygon
        /// </summary>
        /// <typeparam name="PointT"></typeparam>
        /// <param name="pt"></param>
        /// <param name="pts"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns>
        /// 1  -- within polygon
        /// 0  -- on polygon edge
        /// -1 -- outside the polygon 
        /// </returns>
        public static int PointInPolygon(Vector2 pt, IList<Vector2> pts, int start, int len)
        {
            float maxXval;
            float minXval;
            float maxYval;
            float minYval;
            CalcPolygonBound(pts, start, len, out minXval, out maxXval, out minYval, out maxYval);
            return PointInPolygon(pt, pts, start, len, minXval, maxXval, minYval, maxYval);
        }

        public static int PointInPolygon(Vector2 pt, IList<Vector2> pts, int start, int len, float minXval, float maxXval, float minYval, float maxYval)
        {
            if ((pt.x > maxXval) | (pt.x < minXval) | pt.y > maxYval | pt.y < minYval) {
                return -1;//outside the polygon
            } else {
                int cnt = 0;
                int lastStatus;
                Vector2 pt0 = pts[start];
                if (pt0.y - pt.y > c_FloatPrecision)
                    lastStatus = 1;
                else if (pt0.y - pt.y < c_FloatPrecision)
                    lastStatus = -1;
                else
                    lastStatus = 0;

                for (int i = 0; i < len; ++i) {
                    int ix1 = start + i;
                    int ix2 = start + (i + 1) % len;
                    Vector2 pt1 = pts[ix1];
                    Vector2 pt2 = pts[ix2];

                    if (PointOnLine(pt, pt1, pt2))
                        return 0;

                    int status;
                    if (pt2.y - pt.y > c_FloatPrecision)
                        status = 1;
                    else if (pt2.y - pt.y < c_FloatPrecision)
                        status = -1;
                    else
                        status = 0;

                    int temp = status - lastStatus;
                    lastStatus = status;
                    if (temp > 0) {
                        if (!PointIsLeftOn(pt, pt1, pt2))
                            cnt += temp;
                    } else if (temp < 0) {
                        if (PointIsLeft(pt, pt1, pt2))
                            cnt += temp;
                    }
                }
                if (cnt == 0)
                    return -1;
                else
                    return 1;
            }
        }

        /// <summary>
        /// Determine the relationship between a point and the triangle formed by three other points
        /// Note: The three points A B C of the triangle must be arranged counterclockwise
        /// -1-outside the triangle 0-inside the triangle 1-on the triangle side ab 2-on the triangle side bc 3-on the triangle side ca 4-on the triangle vertex/// </summary>
        /// <param name="pn">point</param>
        /// <param name="pa">triangle vertex A</param>
        /// <param name="pb">triangle vertex B</param>
        /// <param name="pc">triangle vertex C</param>
        /// <returns>-1-Outside the triangle 0-Inside the triangle 1-On the triangle side ab 2-On the triangle side bc 3-On the triangle side ca 4-On the triangle vertex</returns>
        public static int PointInTriangle(Vector2 pn, Vector2 pa, Vector2 pb, Vector2 pc)
        {
            //First, determine whether it is on the edge
            //Note: The three points A B C of the triangle must be arranged counterclockwise
            int num = 0;
            int side = 0;
            if (PointOnLine(pn, pa, pb)) {
                side = 1;
                num++;
            }
            if (PointOnLine(pn, pb, pc)) {
                side = 2;
                num++;
            }
            if (PointOnLine(pn, pc, pa)) {
                side = 3;
                num++;
            }
            if (num > 1)
                return 4;
            else if (num == 1)
                return side;
            else {
                if (SignArea(pn, pa, pb) >= 0.0f && SignArea(pn, pb, pc) >= 0.0f && SignArea(pn, pc, pa) >= 0.0f)
                    return 0;
                else
                    return -1;
            }
        }
        /// <summary>
        /// To calculate the signed area of a triangle, enter the coordinates of the three vertices of the triangle.
        /// </summary>
        /// <param name="na"></param>
        /// <param name="nb"></param>
        /// <param name="nc"></param>
        /// <returns></returns>
        public static float SignArea(Vector2 na, Vector2 nb, Vector2 nc)
        {
            return ((nb.x - na.x) * (nc.y - na.y) - (nb.y - na.y) * (nc.x - na.x)) / 2;
        }
        /// <summary>
        /// Thin out a polygon or polyline.
        /// </summary>
        /// <typeparam name="PointT"></typeparam>
        /// <param name="pts"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="delta">Distance to determine if three specified points are collinear</param>
        /// <returns></returns>
        public static IList<Vector2> Sparseness(IList<Vector2> pts, int start, int len, float delta)
        {
            Vector2 lastPt = pts[0];
            LinkedList<Vector2> links = new LinkedList<Vector2>();
            links.AddLast(lastPt);
            for (int i = 1; i < len; i++) {
                int ix = start + i;
                Vector2 pt = pts[ix];
                if (!PointOverlapPoint(pt, lastPt)) {
                    links.AddLast(pt);
                }
                lastPt = pt;
            }
            for (int count = 1; count > 0; ) {
                count = 0;
                LinkedListNode<Vector2> node = links.First;
                while (node != null) {
                    LinkedListNode<Vector2> node2 = node.Next;
                    LinkedListNode<Vector2> node3 = null;
                    if (node2 != null)
                        node3 = node2.Next;
                    if (node3 != null) {
                        double d = PointToLineDistance(node2.Value, node.Value, node3.Value);
                        if (d <= delta) {
                            links.Remove(node2);
                            count++;
                        }
                    }
                    //Regardless of whether to delete a node, always remove the next node, which can avoid some situations where arcs are drawn into straight lines.
                    node = node.Next;
                }
            }
            Vector2[] rpts = new Vector2[links.Count];
            links.CopyTo(rpts, 0);
            return rpts;
        }
    }
    public partial class Geometry
    {
        /// <summary>
        /// Compute vector cross product
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="ep"></param>
        /// <param name="op"></param>
        /// <returns>
        /// ret>0 ep in opsp vector counterclockwise direction
        /// ret=0 collinear
        /// ret<0 ep in opsp vector clockwise direction
        /// </returns>
        public static float Multiply(Vector3 sp, Vector3 ep, Vector3 op)
        {
            return ((sp.x - op.x) * (ep.z - op.z) - (ep.x - op.x) * (sp.z - op.z));
        }
        /// <summary>
        /// r=DotMultiply(p1,p2,p0), get the dot product of vectors (p1-p0) and (p2-p0).
        /// Note: Both vectors must be non-zero vectors
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p0"></param>
        /// <returns>
        /// r>0: The angle between the two vectors is an acute angle;
        /// r=0: The angle between the two vectors is a right angle;
        /// r<0: The angle between the two vectors is an obtuse angle
        /// </returns>
        public static float DotMultiply(Vector3 p1, Vector3 p2, Vector3 p0)
        {
            return ((p1.x - p0.x) * (p2.x - p0.x) + (p1.z - p0.z) * (p2.z - p0.z));
        }
        /// <summary>
        /// Determine the relationship between p and line segment p1p2
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns>
        /// r=0 p = p1
        /// r=1 p = p2
        /// r<0 p is on the backward extension of p1p2
        /// r>1 p is on the forward extension of p1p2
        /// 0<r<1 p is interior to p1p2
        /// </returns>
        public static float Relation(Vector3 p, Vector3 p1, Vector3 p2)
        {
            return DotMultiply(p, p2, p1) / DistanceSquare(p1, p2);
        }
        /// <summary>
        /// Ask for a foothold
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector3 Perpendicular(Vector3 p, Vector3 p1, Vector3 p2)
        {
            float r = Relation(p, p1, p2);
            Vector3 tp = new Vector3();
            tp.x = p1.x + r * (p2.x - p1.x);
            tp.z = p1.z + r * (p2.z - p1.z);
            return tp;
        }
        /// <summary>
        /// Finds the minimum distance from a point to a line segment and returns the closest point.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="np"></param>
        /// <returns></returns>
        public static float PointToLineSegmentDistance(Vector3 p, Vector3 p1, Vector3 p2, out Vector3 np)
        {
            float r = Relation(p, p1, p2);
            if (r < 0) {
                np = p1;
                return Distance(p, p1);
            }
            if (r > 1) {
                np = p2;
                return Distance(p, p2);
            }
            np = Perpendicular(p, p1, p2);
            return Distance(p, np);
        }
        public static float PointToLineSegmentDistanceSquare(Vector3 p, Vector3 p1, Vector3 p2, out Vector3 np)
        {
            float r = Relation(p, p1, p2);
            if (r < 0) {
                np = p1;
                return DistanceSquare(p, p1);
            }
            if (r > 1) {
                np = p2;
                return DistanceSquare(p, p2);
            }
            np = Perpendicular(p, p1, p2);
            return DistanceSquare(p, np);
        }
        /// <summary>
        /// Find the distance from a point to a straight line
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float PointToLineDistance(Vector3 p, Vector3 p1, Vector3 p2)
        {
            return Math.Abs(Multiply(p, p2, p1)) / Distance(p1, p2);
        }
        public static float PointToLineDistanceInverse(Vector3 p, Vector3 p1, Vector3 p2)
        {
            return Math.Abs(Multiply(p, p2, p1)) / Distance(p1, p2);
        }
        /// <summary>
        /// Find the minimum distance from the point to the polyline and return the minimum distance point.
        /// Note: If the given point is not enough to form a polyline, the value of q will not be modified.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="pts"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static float PointToPolylineDistance(Vector3 p, IList<Vector3> pts, int start, int len, ref Vector3 q)
        {
            float ret = float.MaxValue;
            for (int i = start; i < start + len - 1; ++i) {
                Vector3 tq;
                float d = PointToLineSegmentDistance(p, pts[i], pts[i + 1], out tq);
                if (d < ret) {
                    ret = d;
                    q = tq;
                }
            }
            return ret;
        }
        public static float PointToPolylineDistanceSquare(Vector3 p, IList<Vector3> pts, int start, int len, ref Vector3 q)
        {
            float ret = float.MaxValue;
            for (int i = start; i < start + len - 1; ++i) {
                Vector3 tq;
                float d = PointToLineSegmentDistanceSquare(p, pts[i], pts[i + 1], out tq);
                if (d < ret) {
                    ret = d;
                    q = tq;
                }
            }
            return ret;
        }
        public static bool Intersect(Vector3 us, Vector3 ue, Vector3 vs, Vector3 ve)
        {
            return ((Max(us.x, ue.x) >= Min(vs.x, ve.x)) &&
            (Max(vs.x, ve.x) >= Min(us.x, ue.x)) &&
            (Max(us.z, ue.z) >= Min(vs.z, ve.z)) &&
            (Max(vs.z, ve.z) >= Min(us.z, ue.z)) &&
            (Multiply(vs, ue, us) * Multiply(ue, ve, us) >= 0) &&
            (Multiply(us, ve, vs) * Multiply(ve, ue, vs) >= 0));
        }
        public static bool LineIntersectRectangle(Vector3 lines, Vector3 linee, float left, float top, float right, float bottom)
        {
            if (Max(lines.x, linee.x) >= left && right >= Min(lines.x, linee.x) && Max(lines.z, linee.z) >= top && bottom >= Min(lines.z, linee.z)) {
                Vector3 lt = new Vector3(left, 0, top);
                Vector3 rt = new Vector3(right, 0, top);
                Vector3 rb = new Vector3(right, 0, bottom);
                Vector3 lb = new Vector3(left, 0, bottom);
                if (Multiply(lines, lt, linee) * Multiply(lines, rt, linee) <= 0)
                    return true;
                if (Multiply(lines, lt, linee) * Multiply(lines, lb, linee) <= 0)
                    return true;
                if (Multiply(lines, rt, linee) * Multiply(lines, rb, linee) <= 0)
                    return true;
                if (Multiply(lines, lb, linee) * Multiply(lines, rb, linee) <= 0)
                    return true;
                /*
                if (Intersect(lines, linee, lt, rt))
                  return true;
                if (Intersect(lines, linee, rt, rb))
                  return true;
                if (Intersect(lines, linee, rb, lb))
                  return true;
                if (Intersect(lines, linee, lt, lb))
                  return true;
                */
            }
            return false;
        }

        //3D version: https://answers.unity.com/questions/869869/method-of-finding-point-in-3d-space-that-is-exactl.html
        public static int LineCircleIntersection(Vector2 center, float radius, Vector2 rayStart, Vector2 rayEnd,ref Vector2[] intersects)
        {
            Vector2 directionRay = rayEnd - rayStart;
            Vector2 centerToRayStart = rayStart - center;

            float a = Vector2.Dot(directionRay, directionRay);
            float b = 2 * Vector2.Dot(centerToRayStart, directionRay);
            float c = Vector2.Dot(centerToRayStart, centerToRayStart) - (radius * radius);

            float discriminant = (b * b) - (4 * a * c);
            if (discriminant >= 0)
            {
                //Ray did not miss
                discriminant = Mathf.Sqrt(discriminant);

                //How far on ray the intersections happen
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
                {
                    //total intersection, return both points
                    intersects[0] = rayStart + (directionRay * t1);
                    intersects[1] = rayStart + (directionRay * t2);
                    return 2;
                }
                else
                {
                    //Only one intersected, return one point
                    if (t1 >= 0 && t1 <= 1)
                    {
                        intersects[0] = rayStart + (directionRay * t1);
                        return 1;
                    }
                    else if (t2 >= 0 && t2 <= 1)
                    {
                        intersects[0] = rayStart + (directionRay * t2);
                        return 1;
                    }
                    return 0;
                }
            }
            //No hits
            return 0;
        }

        public static bool PointOnLine(Vector3 pt, Vector3 pt1, Vector3 pt2)
        {
            float dx, dy, dx1, dy1;
            bool retVal;
            dx = pt2.x - pt1.x;
            dy = pt2.z - pt1.z;
            dx1 = pt.x - pt1.x;
            dy1 = pt.z - pt1.z;
            if (pt.x == pt1.x && pt.z == pt1.z || pt.x == pt2.x && pt.z == pt2.z) {//on the vertex
                retVal = true;
            } else {
                if (Math.Abs(dx * dy1 - dy * dx1) < c_FloatPrecision) {//Slopes are equal//Consider calculation errors
                    //equals zero at the vertex
                    if (dx1 * (dx1 - dx) < 0 | dy1 * (dy1 - dy) < 0) {
                        retVal = true;
                    } else {
                        retVal = false;
                    }
                } else {
                    retVal = false;
                }
            }
            return retVal;
        }
        /// <summary>
        /// Determine whether two points coincide with each other
        /// </summary>
        /// <param name="pt0"></param>
        /// <param name="pt1"></param>
        /// <returns></returns>
        public static bool PointOverlapPoint(Vector3 pt0, Vector3 pt1)
        {
            if (Distance(pt0, pt1) < c_MinDistance)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Determine whether a is to the left of line bc
        /// </summary>
        public static bool PointIsLeft(Vector3 a, Vector3 b, Vector3 c)
        {
            return Multiply(c, a, b) > 0;
        }
        /// <summary>
        /// Determine whether a is to the left or on the line bc
        /// </summary>
        public static bool PointIsLeftOn(Vector3 a, Vector3 b, Vector3 c)
        {
            return Multiply(c, a, b) >= 0;
        }
        /// <summary>
        /// Determine whether the three points a, b, and c are collinear
        /// </summary>
        public static bool PointIsCollinear(Vector3 a, Vector3 b, Vector3 c)
        {
            return Multiply(c, a, b) == 0;
        }
        public static bool IsCounterClockwise(IList<Vector3> pts, int start, int len)
        {
            bool ret = false;
            if (len >= 3) {
                float maxx = pts[start].x;
                float maxy = pts[start].z;
                float minx = maxx;
                float miny = maxy;
                int maxxi = 0;
                int maxyi = 0;
                int minxi = 0;
                int minyi = 0;
                for (int i = 1; i < len; ++i) {
                    int ix = start + i;
                    float x = pts[ix].x;
                    float y = pts[ix].z;
                    if (maxx < x) {
                        maxx = x;
                        maxxi = i;
                    } else if (minx > x) {
                        minx = x;
                        minxi = i;
                    }
                    if (maxy < y) {
                        maxy = y;
                        maxyi = i;
                    } else if (miny > y) {
                        miny = y;
                        minyi = i;
                    }
                }
                int val = 0;
                for (int delt = 0; delt < len; delt++) {
                    int ix0 = start + (len * 2 + maxxi - delt - 1) % len;
                    int ix1 = start + maxxi;
                    int ix2 = start + (maxxi + delt + 1) % len;

                    float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
                    if (r > 0) {
                        val++;
                        break;
                    } else if (r < 0) {
                        val--;
                        break;
                    }
                }
                for (int delt = 0; delt < len; delt++) {
                    int ix0 = start + (len * 2 + maxyi - delt - 1) % len;
                    int ix1 = start + maxyi;
                    int ix2 = start + (maxyi + delt + 1) % len;

                    float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
                    if (r > 0) {
                        val++;
                        break;
                    } else if (r < 0) {
                        val--;
                        break;
                    }
                }
                for (int delt = 0; delt < len; delt++) {
                    int ix0 = start + (len * 2 + minxi - delt - 1) % len;
                    int ix1 = start + minxi;
                    int ix2 = start + (minxi + delt + 1) % len;

                    float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
                    if (r > 0) {
                        val++;
                        break;
                    } else if (r < 0) {
                        val--;
                        break;
                    }
                }
                for (int delt = 0; delt < len; delt++) {
                    int ix0 = start + (len * 2 + minyi - delt - 1) % len;
                    int ix1 = start + minyi;
                    int ix2 = start + (minyi + delt + 1) % len;

                    float r = Multiply(pts[ix1], pts[ix2], pts[ix0]);
                    if (r > 0) {
                        val++;
                        break;
                    } else if (r < 0) {
                        val--;
                        break;
                    }
                }
                if (val > 0)
                    ret = true;
                else
                    ret = false;
            }
            return ret;
        }
        /// <summary>
        /// Calculate polygon centroid and radius
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public static float CalcPolygonCentroidAndRadius(IList<Vector3> pts, int start, int len, out Vector3 centroid)
        {
            float ret = 0;
            if (len > 0) {
                float x = 0;
                float y = 0;
                for (int i = 1; i < len; ++i) {
                    int ix = start + i;
                    x += pts[ix].x;
                    y += pts[ix].z;
                }
                x /= len;
                y /= len;
                centroid = new Vector3(x, 0, y);
                float distSqr = 0;
                for (int i = 1; i < len; ++i) {
                    int ix = start + i;
                    float tmp = Geometry.DistanceSquare(centroid, pts[ix]);
                    if (distSqr < tmp)
                        distSqr = tmp;
                }
                ret = (float)Math.Sqrt(distSqr);
            } else {
                centroid = new Vector3();
            }
            return ret;
        }
        /// <summary>
        /// Compute polygonal axis-aligned rectangular bounding box
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="minXval"></param>
        /// <param name="maxXval"></param>
        /// <param name="minZval"></param>
        /// <param name="maxZval"></param>
        public static void CalcPolygonBound(IList<Vector3> pts, int start, int len, out float minXval, out float maxXval, out float minYval, out float maxYval)
        {
            maxXval = pts[start].x;
            minXval = pts[start].x;
            maxYval = pts[start].z;
            minYval = pts[start].z;
            for (int i = 1; i < len; ++i) {
                int ix = start + i;
                float xv = pts[ix].x;
                float yv = pts[ix].z;
                if (maxXval < xv)
                    maxXval = xv;
                else if (minXval > xv)
                    minXval = xv;
                if (maxYval < yv)
                    maxYval = yv;
                else if (minYval > yv)
                    minYval = yv;
            }
        }
        /// <summary>
        /// Determine whether a point is within a polygon
        /// </summary>
        /// <typeparam name="PointT"></typeparam>
        /// <param name="pt"></param>
        /// <param name="pts"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns>
        /// 1  -- within polygon
        /// 0  -- on polygon edge
        /// -1 -- outside the polygon 
        /// </returns>
        public static int PointInPolygon(Vector3 pt, IList<Vector3> pts, int start, int len)
        {
            float maxXval;
            float minXval;
            float maxYval;
            float minYval;
            CalcPolygonBound(pts, start, len, out minXval, out maxXval, out minYval, out maxYval);
            return PointInPolygon(pt, pts, start, len, minXval, maxXval, minYval, maxYval);
        }

        public static int PointInPolygon(Vector3 pt, IList<Vector3> pts, int start, int len, float minXval, float maxXval, float minYval, float maxYval)
        {
            if ((pt.x > maxXval) | (pt.x < minXval) | pt.z > maxYval | pt.z < minYval) {
                return -1;//outside the polygon
            } else {
                int cnt = 0;
                int lastStatus;
                Vector3 pt0 = pts[start];
                if (pt0.z - pt.z > c_FloatPrecision)
                    lastStatus = 1;
                else if (pt0.z - pt.z < c_FloatPrecision)
                    lastStatus = -1;
                else
                    lastStatus = 0;

                for (int i = 0; i < len; ++i) {
                    int ix1 = start + i;
                    int ix2 = start + (i + 1) % len;
                    Vector3 pt1 = pts[ix1];
                    Vector3 pt2 = pts[ix2];

                    if (PointOnLine(pt, pt1, pt2))
                        return 0;

                    int status;
                    if (pt2.z - pt.z > c_FloatPrecision)
                        status = 1;
                    else if (pt2.z - pt.z < c_FloatPrecision)
                        status = -1;
                    else
                        status = 0;

                    int temp = status - lastStatus;
                    lastStatus = status;
                    if (temp > 0) {
                        if (!PointIsLeftOn(pt, pt1, pt2))
                            cnt += temp;
                    } else if (temp < 0) {
                        if (PointIsLeft(pt, pt1, pt2))
                            cnt += temp;
                    }
                }
                if (cnt == 0)
                    return -1;
                else
                    return 1;
            }
        }

        /// <summary>
        /// Determine the relationship between a point and the triangle formed by three other points
        /// Note: The three points A B C of the triangle must be arranged counterclockwise
        /// -1-outside the triangle 0-inside the triangle 1-on the triangle side ab 2-on the triangle side bc 3-on the triangle side ca 4-on the triangle vertex
        /// </summary>
        /// <param name="pn">point</param>
        /// <param name="pa">triangle vertex A</param>
        /// <param name="pb">triangle vertex B</param>
        /// <param name="pc">triangle vertex C</param>
        /// <returns>-1-Outside the triangle 0-Inside the triangle 1-On the triangle side ab 2-On the triangle side bc 3-On the triangle side ca 4-On the triangle vertex</returns>
        public static int PointInTriangle(Vector3 pn, Vector3 pa, Vector3 pb, Vector3 pc)
        {
            //First, determine whether it is on the edge
            //Note: The three points A B C of the triangle must be arranged counterclockwise
            int num = 0;
            int side = 0;
            if (PointOnLine(pn, pa, pb)) {
                side = 1;
                num++;
            }
            if (PointOnLine(pn, pb, pc)) {
                side = 2;
                num++;
            }
            if (PointOnLine(pn, pc, pa)) {
                side = 3;
                num++;
            }
            if (num > 1)
                return 4;
            else if (num == 1)
                return side;
            else {
                if (SignArea(pn, pa, pb) >= 0.0f && SignArea(pn, pb, pc) >= 0.0f && SignArea(pn, pc, pa) >= 0.0f)
                    return 0;
                else
                    return -1;
            }
        }
        /// <summary>
        /// To calculate the signed area of a triangle, enter the coordinates of the three vertices of the triangle.
        /// </summary>
        /// <param name="na"></param>
        /// <param name="nb"></param>
        /// <param name="nc"></param>
        /// <returns></returns>
        public static float SignArea(Vector3 na, Vector3 nb, Vector3 nc)
        {
            return ((nb.x - na.x) * (nc.z - na.z) - (nb.z - na.z) * (nc.x - na.x)) / 2;
        }
        /// <summary>
        /// Thin out a polygon or polyline.
        /// </summary>
        /// <typeparam name="PointT"></typeparam>
        /// <param name="pts"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="delta">Distance to determine if three specified points are collinear</param>
        /// <returns></returns>
        public static IList<Vector3> Sparseness(IList<Vector3> pts, int start, int len, float delta)
        {
            Vector3 lastPt = pts[0];
            LinkedList<Vector3> links = new LinkedList<Vector3>();
            links.AddLast(lastPt);
            for (int i = 1; i < len; i++) {
                int ix = start + i;
                Vector3 pt = pts[ix];
                if (!PointOverlapPoint(pt, lastPt)) {
                    links.AddLast(pt);
                }
                lastPt = pt;
            }
            for (int count = 1; count > 0; ) {
                count = 0;
                LinkedListNode<Vector3> node = links.First;
                while (node != null) {
                    LinkedListNode<Vector3> node2 = node.Next;
                    LinkedListNode<Vector3> node3 = null;
                    if (node2 != null)
                        node3 = node2.Next;
                    if (node3 != null) {
                        double d = PointToLineDistance(node2.Value, node.Value, node3.Value);
                        if (d <= delta) {
                            links.Remove(node2);
                            count++;
                        }
                    }
                    //Regardless of whether to delete a node, always remove the next node,
                    //which can avoid some situations where arcs are drawn into straight lines.
                    node = node.Next;
                }
            }
            Vector3[] rpts = new Vector3[links.Count];
            links.CopyTo(rpts, 0);
            return rpts;
        }
    }
    public struct SRT
    {
        public SRT(Vector3 scale, Quaternion rotation, Vector3 pos)
        {
            this.scale = scale;
            this.rotation = rotation;
            this.pos = pos;
        }
        public SRT(Vector3 pos)
        {
            this.scale = Vector3.one;
            this.rotation = Quaternion.identity;
            this.pos = pos;
        }
        public static SRT identity = new SRT(Vector3.one, Quaternion.identity, Vector3.zero);
        public Vector3 scale;
        public Quaternion rotation;
        public Vector3 pos;
    }
}

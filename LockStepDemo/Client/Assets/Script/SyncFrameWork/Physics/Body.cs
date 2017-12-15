﻿using Lockstep;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body
{
    public BodyType bodyType = BodyType.Circle;
    public Vector2d position;
    public Vector2d direction;
     
    public long length;
    public long width;

    public long radius;
    public long angle;

    public bool isStandard = false;

    #region 构造函数

    public Body()
    {

    }

    /// <summary>
    /// 标准矩形的构造函数
    /// </summary>
    /// <param name="leftBound"></param>
    /// <param name="rightBound"></param>
    /// <param name="upBound"></param>
    /// <param name="downBound"></param>
    public Body(int leftBound,int rightBound,int downBound,int upBound) : this(FixedMath.Create(leftBound), FixedMath.Create(rightBound), FixedMath.Create(downBound), FixedMath.Create(upBound))
    {

    }

    public Body(long leftBound, long rightBound, long downBound, long upBound)
    {
        //Debug.Log(" " + leftBound.ToFloat() + " " + rightBound.ToFloat() + " " + downBound.ToFloat() + " " + upBound.ToFloat());

        bodyType = BodyType.Rectangle;
        isStandard = true;

        direction = new Vector2d(1, 0);
        length = rightBound - leftBound ;
        width  = upBound - downBound;

        position = new Vector2d(leftBound + (length).Div (FixedMath.Create(2)), downBound + (width).Div(FixedMath.Create( 2)));

        //Debug.Log(" pos " + position + " l " + length.ToFloat() + " w " + width.ToFloat());
    }

    #endregion

    #region 赋值方法

    public float Length
    {
        get
        {
            return length.ToFloat();
        }

        set
        {
            length = FixedMath.Create( value);
        }
    }

    public float Width
    {
        get
        {
            return width.ToFloat();
        }

        set
        {
            width = FixedMath.Create(value);
        }
    }

    public float Radius
    {
        get
        {
            return radius.ToFloat();
        }

        set
        {
            radius = FixedMath.Create(value);
        }
    }

    public float Angle
    {
        get
        {
            return angle.ToFloat();
        }

        set
        {
            angle = FixedMath.Create(value);
        }
    }

    public Vector3 Position
    {
        get
        {
            return new Vector3(position.x.ToFloat(),0,position.y.ToFloat());
        }

        set
        {
            position = new Vector2d( FixedMath.Create( value.x),FixedMath.Create( value.z));
        }
    }

    public Vector3 Direction
    {
        get
        {
            return new Vector3(direction.x.ToFloat(), 0, direction.y.ToFloat());
        }

        set
        {
            direction = new Vector2d(FixedMath.Create(value.x), FixedMath.Create(value.z));
        }
    }

    public AreaType AreaType
    {
        get
        {
            switch(bodyType)
            {
                case BodyType.Circle:return AreaType.Circle;
                case BodyType.Rectangle: return AreaType.Rectangle;
                default:return AreaType.Circle;
            }
        }

        set
        {
            switch (value)
            {
                case AreaType.Circle: bodyType = BodyType.Circle;break ;
                case AreaType.Rectangle: bodyType = BodyType.Rectangle; break;
                case AreaType.Sector: bodyType = BodyType.Sector; break;
                default: bodyType = BodyType.Circle;break;
            }
        }
    }



    #endregion

    #region 取值方法

    //标准矩形使用
    public long LeftBound
    {
        get
        {
            if(isStandard)
            {
                return position.x - length.Mul(FixedMath.Half);
            }
            else
            {
                Vector2d forward = direction * length.Mul(FixedMath.Half);
                return (position + forward).x;
            }

        }
    }

    public long RightBound
    {
        get
        {
            if (isStandard)
            {
                return position.x + length.Mul(FixedMath.Half);
            }
            else
            {
                Vector2d forward = direction * length.Mul(FixedMath.Half);
                return (position + forward).x;
            }
        }
    }

    public long UpBound
    {
        get
        {
            if (isStandard)
            {
                return position.y + width.Mul(FixedMath.Half);
            }
            else
            {
                Vector2d left = direction.Vector2dRotateInXZ(FixedMath.Create(90)) * length.Mul(FixedMath.Half);

                return (position + left).y;
            }
        }
    }

    public long DownBound
    {
        get
        {
            if (isStandard)
            {
                return position.y - width.Mul(FixedMath.Half);
            }
            else
            {
                Vector2d left = direction.Vector2dRotateInXZ(FixedMath.Create(90)) * length.Mul(FixedMath.Half);

                return (position - left).y;
            }
        }
    }

    #endregion

    #region 碰撞接口

    public bool CheckCollide(Body body)
    {
        switch (bodyType)
        {
            case BodyType.Circle: return Circle(body);
            case BodyType.Rectangle: return Rectangle(body);
                //case BodyType.Sector: return Sector(area);
        }

        return true;
    }

    /// <summary>
    /// 判断是否包含后者
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public bool CheckIsInnner(Body body)
    {
        switch (bodyType)
        {
            case BodyType.Circle: return Inner_Circle(body);
            case BodyType.Rectangle: return Inner_Rectangle(body);
                //case BodyType.Sector: return Sector(area);
        }

        return true;
    }

    public Vector2d GetOffset(Body body)
    {
        switch (bodyType)
        {
            case BodyType.Circle: return Offset_Circle(body);
            case BodyType.Rectangle: return Offset_Rectangle(body);
                //case BodyType.Sector: return Offset_Sector(body);
        }

        return new Vector2d();
    }

    #endregion

    #region 形状判断
    //自己是圆形
    private bool Circle(Body body)
    {
        switch (body.bodyType)
        {
            case BodyType.Circle: return Circle_Circle(this, body);
            case BodyType.Rectangle: return Circle_Rectangle(this, body);
                //case BodyType.Sector: return Circle_Sector(this, area);
        }
        return true;

    }

    //自己是矩形
    private bool Rectangle(Body body)
    {
        //Debug.Log(area.position + "长 ：" + area.length + "宽： " + area.width + "半径"+ area.radius);
        //Debug.Log(this.position + "长 ：" + this.length + "宽： " + this.width + "forward" + direction);
        switch (body.bodyType)
        {
            case BodyType.Circle: return Circle_Rectangle(body, this);
            case BodyType.Rectangle: return Rectangle_Rectangle(body, this);
                //case BodyType.Sector: return Sector_Rectangle(area, this);
        }

        return true;
    }

    private bool Standard_Rectangle(Body body)
    {
        switch (body.bodyType)
        {
            case BodyType.Circle: return Circle_Rectangle(body, this);
            case BodyType.Rectangle: return Rectangle_Rectangle(body, this);
                //case BodyType.Sector: return Sector_Rectangle(area, this);
        }

        return true;
    }

        ////自己是扇形
        //private bool Sector(Body body)
        //{
        //    switch (area.bodyType)
        //    {
        //        case BodyType.Circle: return Circle_Sector(area, this);
        //        case BodyType.Rectangle: return Sector_Rectangle(this, area);
        //        case BodyType.Sector: return Sector_Sector(area, this);
        //    }
        //    return true;
        //}
        #endregion

    #region 相交判断

     private bool Circle_Circle(Body Body1, Body Body2)
    {
        long CacheSqrDistance = Body1.radius + Body2.radius;
        CacheSqrDistance *= CacheSqrDistance;

        long DistX = Body1.position.x - Body2.position.x;
        long DistY = Body1.position.y - Body2.position.y;
        if ((DistX * DistX + DistY * DistY) <= CacheSqrDistance)
        {
            return true;
        }

        return false;
    }

    private bool Circle_Rectangle(Body circle, Body rectangle)
    {
        Vector2d newCirclePos = circle.position;
        //标准矩形
        if (!isStandard)
        {
            //先进行一次剪枝
            long x1 = circle.position.x - rectangle.position.x;
            long y1 = circle.position.y - rectangle.position.y;
            long d1 = x1.Mul(x1) + y1.Mul(y1);

            long r1 = circle.radius + FixedMath.Create(1.42f).Mul(rectangle.width + rectangle.length);
            long d2 = r1.Mul(r1);

            if (d1 > d2)
            {
                return false;
            }

            //先把圆形归位
            long angle = rectangle.direction.GetRotationAngle(new Vector2d(1, 0));
            newCirclePos = circle.position.PostionRotateInXZ(rectangle.position, angle);
        }

        long cx, cy;

        if (newCirclePos.x < rectangle.position.x - rectangle.length.Mul(FixedMath.Half))
        {
            cx = rectangle.position.x - rectangle.length.Mul(FixedMath.Half);
        }
        else if (newCirclePos.x > rectangle.position.x + rectangle.length.Mul(FixedMath.Half))
        {
            cx = rectangle.position.x + rectangle.length.Mul(FixedMath.Half);
        }
        else
        {
            cx = newCirclePos.x;
        }

        if (newCirclePos.y < rectangle.position.y - rectangle.width.Mul(FixedMath.Half))
        {
            cy = rectangle.position.y - rectangle.width.Mul(FixedMath.Half);
        }
        else if (newCirclePos.y > rectangle.position.y + rectangle.width.Mul(FixedMath.Half))
        {
            cy = rectangle.position.y + rectangle.width.Mul(FixedMath.Half);
        }
        else
        {
            cy = newCirclePos.y;
        }

        long d = (cx - newCirclePos.x). Mul (cx - newCirclePos.x) + (cy - newCirclePos.y). Mul (cy - newCirclePos.y);

        if (d < (circle.radius .Mul( circle.radius)))
        {
            return true;
        }

        return false;
    }

    private bool Rectangle_Rectangle(Body area1, Body area2)
    {
        List<Vector2d> list = GetRectPoint(area1);

        bool result = false;
        for (int i = 0; i < list.Count; i++)
        {
            if(CheckPointInRect(list[i], area2))
            {
                result |= true;
            }
        }

        return result;
    }

    #endregion

    #region 包含判断

    public bool Inner_Circle(Body body)
    {
        //Debug.LogError("Inner_Circle 未实现");
        //未实现
        return false;
    }

    public bool Inner_Rectangle(Body body)
    {
        switch (body.bodyType)
        {
            case BodyType.Circle: return Inner_Rectangle_Circle(body);
            case BodyType.Rectangle: return Inner_Rectangle_Rectangle(body);
                //case BodyType.Sector: return Offset_Sector(body);
        }

        return false;
    }

    public bool Inner_Rectangle_Circle(Body circle)
    {
        Body rectangle = this;
        Vector2d newCirclePos = circle.position;
        //标准矩形
        if (!isStandard)
        {
            //先把圆形归位
            long angle = rectangle.direction.GetRotationAngle(new Vector2d(1, 0));
            newCirclePos = circle.position.PostionRotateInXZ(rectangle.position, angle);
        }

        long cx, cy;

        if (newCirclePos.x < rectangle.position.x - rectangle.length.Mul(FixedMath.Half))
        {
            cx = rectangle.position.x - rectangle.length.Mul(FixedMath.Half);
        }
        else if (newCirclePos.x > rectangle.position.x + rectangle.length.Mul(FixedMath.Half))
        {
            cx = rectangle.position.x + rectangle.length.Mul(FixedMath.Half);
        }
        else
        {
            cx = newCirclePos.x;
        }

        if (newCirclePos.y < rectangle.position.y - rectangle.width.Mul(FixedMath.Half))
        {
            cy = rectangle.position.y - rectangle.width.Mul(FixedMath.Half);
        }
        else if (newCirclePos.y > rectangle.position.y + rectangle.width.Mul(FixedMath.Half))
        {
            cy = rectangle.position.y + rectangle.width.Mul(FixedMath.Half);
        }
        else
        {
            cy = newCirclePos.y;
        }

        //Debug.Log(newCirclePos + " " + new Vector2d(cx,cy));

        if (cx != newCirclePos.x
            || cy != newCirclePos.y)
        {
            return false;
        }
        else
        {
            Vector2d offset = newCirclePos - rectangle.position;

            if(offset.x > 0 && offset.x + circle.radius > rectangle.length.Mul(FixedMath.Half))
            {
                return false;
            }

            if (offset.x > 0 && offset.x - circle.radius < -rectangle.length.Mul(FixedMath.Half))
            {
                return false;
            }

            if (offset.y > 0 && offset.y + circle.radius > rectangle.width.Mul(FixedMath.Half))
            {
                return false;
            }

            if (offset.y < 0 && offset.y - circle.radius < -rectangle.width.Mul(FixedMath.Half))
            {
                return false;
            }
        }

        return true;
    }

    public bool Inner_Rectangle_Rectangle(Body rect)
    {
        List<Vector2d> list = GetRectPoint(rect);

        for (int i = 0; i < list.Count; i++)
        {
            if (!CheckPointInRect(list[i], this))
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region 碰撞偏移

    Vector2d Offset_Circle(Body body)
    {
        switch (body.bodyType)
        {
            case BodyType.Circle: return Offset_Circle_Circle(body);
            case BodyType.Rectangle: return Offset_Circle_Rectangle(body);
            //case BodyType.Sector: return Offset_Sector(area);
        }

        return new Vector2d();
    }

    Vector2d Offset_Circle_Circle(Body body)
    {
        long distance = position.Distance(body.position);

        long r = radius + body.radius;

        //Debug.Log("distance " + distance + " r " + r + " ");

        if (distance < r)
        {
            Vector2d offset = new Vector2d();

            offset = body.position - position;
            offset.Normalize();
            offset = offset * (r - distance);

            return offset;
        }
        else
        {
            return new Vector2d();
        }
    }

    Vector2d Offset_Rectangle(Body body)
    {
        switch (body.bodyType)
        {
            case BodyType.Circle: return Offset_Rectangle_Circle(body);
            case BodyType.Rectangle: return Offset_Rectangle_Rectangle(body);
            //case BodyType.Sector: return Offset_Sector(area);
        }

        return new Vector2d();
    }

    //弹走矩形
    Vector2d Offset_Circle_Rectangle(Body body)
    {
        return new Vector2d();
    }

    //弹走圆形
    Vector2d Offset_Rectangle_Circle(Body circle)
    {
        Body rectangle = this;
        Vector2d newCirclePos = circle.position;

        long rot = FixedMath.Create(0);
        if (!isStandard)
        {
            //先把圆形归位
            rot = direction.GetRotationAngle(new Vector2d(1, 0));
            newCirclePos = circle.position.PostionRotateInXZ(position, rot);
        }

        
        long cx, cy;

        if (newCirclePos.x < rectangle.position.x - rectangle.length.Mul(FixedMath.Half))
        {
            cx = rectangle.position.x - rectangle.length.Mul(FixedMath.Half);
        }
        else if (newCirclePos.x > rectangle.position.x + rectangle.length.Mul(FixedMath.Half))
        {
            cx = rectangle.position.x + rectangle.length.Mul(FixedMath.Half);
        }
        else
        {
            cx = newCirclePos.x;
        }

        if (newCirclePos.y < rectangle.position.y - rectangle.width.Mul(FixedMath.Half))
        {
            cy = rectangle.position.y - rectangle.width.Mul(FixedMath.Half);
        }
        else if (newCirclePos.y > rectangle.position.y + rectangle.width.Mul(FixedMath.Half))
        {
            cy = rectangle.position.y + rectangle.width.Mul(FixedMath.Half);
        }
        else
        {
            cy = newCirclePos.y;
        }

        Vector2d oc =  newCirclePos - position;

        //Debug.DrawLine(position.ToVector3(), (position + oc.Vector2dRotateInXZ(rot)).ToVector3(),Color.yellow);

        Vector2d offset = new Vector2d();

        if (oc.x > FixedMath.Create( 0) && oc.x < length.Mul(FixedMath.Half) + circle.radius)
        {
            offset.x = (length.Mul(FixedMath.Half) - oc.x) + circle.radius;
        }
        else if (oc.x < FixedMath.Create(0) && oc.x > -length.Mul(FixedMath.Half) - circle.radius)
        {
            offset.x = (-length.Mul( FixedMath.Half) - oc.x) - circle.radius;
        }
        else
        {
            offset.x = 0;
        }

        if (oc.y > FixedMath.Create(0) && oc.y < width.Mul(FixedMath.Half) + circle.radius)
        {
            offset.y = (width.Mul(FixedMath.Half) - oc.y) + circle.radius;
        }
        else if (oc.y < FixedMath.Create(0) && oc.y > -width.Mul(FixedMath.Half) - circle.radius)
        {
            offset.y = (-width.Mul(FixedMath.Half) - oc.y) - circle.radius;
        }
        else
        {
            offset.y = 0;
        }

        if (Math.Abs(offset.x) > Math.Abs(offset.y))
        {
            offset.x = 0;
        }
        else
        {
            offset.y = 0;
        }

        offset = offset.Vector2dRotateInXZ(rot);
        return offset;

    }

    Vector2d Offset_Rectangle_Rectangle(Body body)
    {
        return new Vector2d();
    }

    #endregion

    #region 工具函数

    List<Vector2d> listCache = new List<Vector2d>();
    public List<Vector2d> GetRectPoint(Body rect)
    {
        listCache.Clear();

        if(rect.isStandard)
        {
            listCache.Add(new Vector2d(rect.position.x + rect.length.Mul(FixedMath.Half), rect.position.y + rect.width.Mul(FixedMath.Half)));
            listCache.Add(new Vector2d(rect.position.x + rect.length.Mul(FixedMath.Half), rect.position.y - rect.width.Mul(FixedMath.Half)));
            listCache.Add(new Vector2d(rect.position.x - rect.length.Mul(FixedMath.Half), rect.position.y + rect.width.Mul(FixedMath.Half)));
            listCache.Add(new Vector2d(rect.position.x - rect.length.Mul(FixedMath.Half), rect.position.y - rect.width.Mul(FixedMath.Half)));
        }
        else
        {
            Vector2d forward = rect.direction * rect.length.Mul(FixedMath.Half);
            Vector2d left = rect.direction.Vector2dRotateInXZ(FixedMath.Create(90)) * rect.length.Mul(FixedMath.Half);

            listCache.Add(rect.position + forward + left);
            listCache.Add(rect.position + forward - left);
            listCache.Add(rect.position - forward + left);
            listCache.Add(rect.position - forward - left);
        }

        return listCache;
    }

    /// <summary>
    /// 判断一个点是否在矩形内
    /// </summary>
    /// <param name="point"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    bool CheckPointInRect(Vector2d point,Body rect)
    {
        Vector2d newPos = point;
        if(!rect.isStandard)
        {
            long angle = rect.direction.GetRotationAngle(new Vector2d(1, 0));
            newPos = newPos.PostionRotateInXZ(rect.position, angle);
        }

        Vector2d offset = newPos - rect.position;

        if(offset.x > 0 &&  offset.x > rect.length.Mul(FixedMath.Half))
        {
            return false;
        }

        if (offset.x < 0 && offset.x < - rect.length.Mul(FixedMath.Half))
        {
            return false;
        }

        if (offset.y > 0 && offset.y > rect.width.Mul(FixedMath.Half))
        {
            return false;
        }

        if (offset.y < 0 && offset.y < -rect.width.Mul(FixedMath.Half))
        {
            return false;
        }

        return true;
    }

    #endregion

    #region 绘制和打印

    public void Draw()
    {
        if(bodyType == BodyType.Rectangle)
        {
            Vector3 a = new Vector2d(LeftBound,UpBound).ToVector3(10);
            Vector3 b = new Vector2d(LeftBound, DownBound).ToVector3(10);
            Vector3 c = new Vector2d(RightBound, UpBound).ToVector3(10);
            Vector3 d = new Vector2d(RightBound, DownBound).ToVector3(10);

            Debug.DrawLine(a, c, Color.green, 60);
            Debug.DrawLine(c, d, Color.green, 60);
            Debug.DrawLine(d, b, Color.green, 60);
            Debug.DrawLine(b, a, Color.green, 60);
        }
    }

    public override string ToString()
    {
        if(bodyType == BodyType.Rectangle)
        {
            return "Rectangle -> Pos:" + position + " Dir: " + direction + " Length: " + Length + " Width " + Width;
        }
        else
        {
            return "Circle -> Pos:" + position + " Dir: " + direction + " Radius: " + radius;
        }
    }

    public void PrintBound()
    {
        Debug.Log("LeftBound " + LeftBound.ToFloat() + " RightBound " + RightBound.ToFloat() + " UpBound " + UpBound.ToFloat() + " DownBound " + DownBound.ToFloat());
    }

    #endregion
}

public enum BodyType
{
    Circle,
    Rectangle,
    Sector,
}

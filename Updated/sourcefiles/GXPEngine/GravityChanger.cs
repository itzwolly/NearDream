using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;

public class GravityChanger : Canvas
{
    public Vec2 changedGravity;
    private string _Name;
    private int _direction;

    public string Name {
        get { return _Name; }
        set { _Name = value; }
    }
    public int Direction { get; set; }

    public GravityChanger(float pX, float pY, int pWidth, int pHeight, int pDirection):base(pWidth,pHeight)
    {
        Direction = pDirection;
        //SetOrigin(width / 2, height / 2);
        x = pX;
        y = pY;
        
        graphics.Clear(Color.Yellow);
        alpha = 0.2f;

        if (pDirection == 3) // down
        {
            changedGravity = new Vec2(0, 1.5f);
        }
        if (pDirection == 1) // up
        {
            changedGravity = new Vec2(0, -2.5f); //to counteract the actual gravity
        }
        if (pDirection == 4) // left
        {
            changedGravity = new Vec2(-1.5f, 0);
        }
        if (pDirection == 2) // right
        {
            changedGravity = new Vec2(1.5f, 0);
        }
        if (pDirection == 0) {
            changedGravity = new Vec2(0, 0);
        }

        alpha = 0.5f;
    }
}


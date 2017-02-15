using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Plank : Sprite
{
    //public bool there;
    public NLineSegment plankLine;
    public Vec2 position;
    public Plank() : base("assets\\sprites\\plank.png")
    {
        SetOrigin(width / 2, height / 2);
        position = new Vec2();
    }
    public NLineSegment GetLine()
    {
        return plankLine = new NLineSegment(new Vec2(position.x - width / 2, position.y), new Vec2(position.x + width / 2, position.y), 0xffffff00, 4);
    }
}
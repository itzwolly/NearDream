using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Plank : Sprite
{
    //public bool there;
    private NLineSegment _plankLine;
    private Vec2 _position;

    public NLineSegment PlankLine {
        get { return _plankLine; }
        set { _plankLine = value; }
    }

    public Vec2 Position {
        get { return _position; }
        set { _position = value; }
    }

    public Plank() : base("assets\\sprites\\plank.png")
    {
        SetOrigin(width / 2, height / 2);
        _position = new Vec2();
       }

    public NLineSegment GetLine()
    {
        _plankLine = new NLineSegment(new Vec2(Position.x - width / 2, Position.y), new Vec2(Position.x + width / 2, Position.y), 0xffffff00, 4);
        return _plankLine;
    }
}
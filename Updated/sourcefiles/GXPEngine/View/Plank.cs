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
        if (SpriteName == "hell_plank") {
            return _plankLine = new NLineSegment(new Vec2((Position.x - width / 2) - 32, Position.y), new Vec2((Position.x + width / 2) - 32, Position.y), 0x00ffff00, 4);
        } else if (SpriteName == "plank_30") {
            return _plankLine = new NLineSegment(new Vec2(Position.x, Position.y - height * 3), new Vec2(Position.x, Position.y + height * 3), 0x00ffff00, 4);
        } else {
            return _plankLine = new NLineSegment(new Vec2(Position.x - width / 2, Position.y), new Vec2(Position.x + width / 2, Position.y), 0x00ffff00, 4);
        }
    }
}
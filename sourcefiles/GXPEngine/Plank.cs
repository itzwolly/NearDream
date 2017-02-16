using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Plank : Sprite
{
    public NLineSegment collider;
    public Vec2 position;
    public Plank() : base("assets\\sprites\\plank.png") {
        collider = new NLineSegment(Vec2.zero,Vec2.zero, 0xffffff00,4);
        SetOrigin(width/2, height/2);
        scale = 2;
        position = new Vec2();
    }
}
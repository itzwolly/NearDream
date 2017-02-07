using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class SimpleRock:Sprite
{
    public Vec2 position;
    public Vec2 velocity;
    public SimpleRock(Vec2 pPosition, Vec2 pVelocity) : base("assets\\sprites\\circle.png")
    {
        SetOrigin(width / 2, height / 2);
        position = pPosition;
        velocity = pVelocity;
        Step();
    }

    public void Step()
    {
        position.Add(velocity);
        x = position.x;
        y = position.y;
    }
}


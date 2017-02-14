using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class BallAnim:AnimationSprite
{
    private Ball _ball;
    private int _wait;

    public BallAnim(Ball pBall):base("assets\\sprites\\dragonanim.png", 8,12)
    {
        _ball = pBall;
        SetOrigin(width / 2, height / 2);
    }

    public void Update()
    {
        NextFrame();
    }
}


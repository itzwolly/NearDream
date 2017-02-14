using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class BallAnim:AnimationSprite
{
    private Ball _ball;
    private int _wait;
    private float _velocityLenght;

    public BallAnim(Ball pBall):base("assets\\sprites\\dragonanim.png", 8,12)
    {
        _ball = pBall;
        SetOrigin(width / 2, height / 2);
    }

    public void Update()
    {
        _velocityLenght = _ball.Velocity.Length();
        if(_ball.IsExploding&&(currentFrame<47||currentFrame>67))
        {
            SetFrame(49);
            if(_ball.chargeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee)
            {
                if (currentFrame >= 87)
                    currentFrame = 87;
            }
        }
        else if(!_ball.IsExploding)
        {
            //Console.WriteLine(_ball.chargeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee +" || "+_ball.Velocity.Length());
            //SetFrame(0);
            if (_ball.chargeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee)
            {
                // if (currentFrame >= 38)
                currentFrame = 38;
            }
            else if (_velocityLenght > 0 && (currentFrame > 46 ||currentFrame <38))
            {
                currentFrame = 38;
            }
            else if ((currentFrame > 18 || currentFrame < 0)&&_velocityLenght==0)
            {
                currentFrame = 0;
            }
            Console.WriteLine(currentFrame);
        }
        NextFrame();
    }
}


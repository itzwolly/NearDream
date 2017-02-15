using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class BallAnim:AnimationSprite
{
    private Ball _ball;
    private float _wait;
    private float _velocityLenght;
    private float _waitTime;

    public BallAnim(Ball pBall):base("assets\\sprites\\dragonanim.png", 8,12)
    {
        _ball = pBall;
        SetOrigin(width / 2, height / 2);
    }

    public void Update()
    {
       
        //_waitTime = 10 -_ball.Velocity.Length();
        //if (_wait > _waitTime)
        //{
            _velocityLenght = _ball.Velocity.Length();
            if (_ball.IsExploding)
            {
                if (_ball.charge)
                {
                    if (currentFrame >= 86)
                        currentFrame = 86;
                }
                else if (_velocityLenght > 0 && (currentFrame > 94 || currentFrame < 86))
                {
                    currentFrame = 86;
                }
                else if (_ball.StartedTimer)
                {
                    currentFrame = 86;
                }
                else if ((currentFrame > 66 || currentFrame < 49) && _velocityLenght == 0)
                {
                    currentFrame = 48;
                }

            }
            else if (!_ball.IsExploding)
            {

                if (_ball.charge)
                {
                    if (currentFrame >= 38)
                        currentFrame = 38;
                }
                else if (_velocityLenght > 0 && (currentFrame > 46 || currentFrame < 38))
                {
                    currentFrame = 38;
                }
                else if ((currentFrame > 18 || currentFrame < 0) && _velocityLenght == 0)
                {
                    currentFrame = 0;
                }
            }
            NextFrame();
            //_wait = 0;
        }
    //    _wait++;
    //}
}


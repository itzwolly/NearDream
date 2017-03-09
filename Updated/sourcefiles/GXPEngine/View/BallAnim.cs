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

    public BallAnim(Ball pBall):base("assets\\sprites\\dragonfinal.png", 4,51)
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
                if (currentFrame >= 175)
                        currentFrame = 175;
                    else if (currentFrame < 137)
                        currentFrame = 137;
                }
                else if (_velocityLenght > 0 && (currentFrame > 187 || currentFrame < 175))
                {
                    currentFrame = 175;
                }
                else if (_ball.StartedTimer)
                {
                    currentFrame = 176;
                }
                else if ((currentFrame > 137 || currentFrame < 99) && _velocityLenght == 0)
                {
                    currentFrame = 99;
                }

            }
            else if (!_ball.IsExploding)
            {

                if (_ball.charge)
                {
                if (currentFrame >= 75)
                    currentFrame = 75;
                else if (currentFrame < 39)
                    currentFrame = 39;
                }
                else if (_velocityLenght > 0 && (currentFrame > 97 || currentFrame < 75))
                {
                    currentFrame = 75;
                }
                else if ((currentFrame > 38 || currentFrame < 0) && _velocityLenght == 0)
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


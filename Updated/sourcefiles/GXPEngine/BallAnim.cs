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

    public BallAnim(Ball pBall):base("assets\\sprites\\dragon_anim1.png", 4,44)
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
                if (currentFrame >= 163)
                        currentFrame = 163;
                    else if (currentFrame < 127)
                        currentFrame = 127;
                }
                else if (_velocityLenght > 0 && (currentFrame > 175 || currentFrame < 163))
                {
                    currentFrame = 163;
                }
                else if (_ball.StartedTimer)
                {
                    currentFrame = 164;
                }
                else if ((currentFrame > 127 || currentFrame < 86) && _velocityLenght == 0)
                {
                    currentFrame = 86;
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
                else if (_velocityLenght > 0 && (currentFrame > 86 || currentFrame < 75))
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


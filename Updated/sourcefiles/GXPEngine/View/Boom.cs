using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Boom : AnimationSprite
{
    public Boom():base("assets//sprites//boom.png",2,5)
    {
        SetOrigin(width / 2, height / 2);
        SetFrame(0);
    }
    public void Update()
    {
        NextFrame();
        if(currentFrame==0)
        {
            Destroy();
        }
    }

}


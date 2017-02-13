using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class BridgeCollider:Canvas
{
    public BridgeCollider(int pWidth, int pHeight) : base(pWidth, pHeight)
    {
        SetOrigin(width / 2, height / 2);
    }

}


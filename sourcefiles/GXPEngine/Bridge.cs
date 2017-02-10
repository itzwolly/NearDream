using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Bridge : Sprite
{
    string _bridgeName;
    public bool Down;

    public string BridgeName {
        get { return _bridgeName; }
        set { _bridgeName = value; }
    }

    public Bridge(int pRotation) : base("assets\\sprites\\bridge_concept.png") {
        SetOrigin(0, height/2);
        scale = 2f;
        rotation = pRotation;
    }
}

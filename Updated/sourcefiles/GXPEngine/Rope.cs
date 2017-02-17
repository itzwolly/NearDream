using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;


public class Rope : Sprite {
    private string _bridgeToDrop;
    private string _pathBlockName;

    public string BridgeToDrop {
        get { return _bridgeToDrop; }
        set { _bridgeToDrop = value; }
    }

    public string PathBlockName {
        get { return _pathBlockName; }
        set { _pathBlockName = value; }
    }

    public Rope(string pFileName) : base(pFileName) {
        SetOrigin(width / 2, 0);
    }
}

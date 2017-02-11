using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class PressurePlate:Sprite
{
    private string _pressurePlateName;
    private string _opensThis;
    private bool _open;

    public bool Open
    {
        get { return _open; }
        set { _open = value; }
    }

    public string PressurePlateName
    {
        get { return _pressurePlateName; }
        set { _pressurePlateName = value; }
    }

    public PressurePlate(float pX, float pY,string pOpensThis):base("assets\\sprites\\pressureplate.png")
    {
        
        _opensThis = pOpensThis;
        SetOrigin(width / 2, height / 2);
        x = pX;
        y = pY;
    }

    public void OpenCoresponding()
    {
        Console.WriteLine(_opensThis);
    }
}


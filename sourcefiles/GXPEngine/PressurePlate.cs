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
    public NLineSegment coverLine;
    public bool cover;

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

    public PressurePlate(float pX, float pY,string pOpensThis, bool pCover,int coverHight):base("assets\\sprites\\pressureplate.png")
    {
        SetOrigin(width / 2, height / 2);
        x = pX;
        y = pY;
        coverLine = new NLineSegment(new Vec2(x-width/2,y-coverHight), new Vec2(x + width / 2, y - coverHight), 0xffffff00, 4);
        cover = pCover;
        _opensThis = pOpensThis;
    }

    public void OpenCoresponding()
    {
        Console.WriteLine(_opensThis);
    }
}


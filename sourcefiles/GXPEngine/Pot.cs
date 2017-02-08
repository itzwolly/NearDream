using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;

public class Pot : Sprite
{
    private Canvas _canvas;

    public Canvas Canvas {
        get { return _canvas; }
        set { _canvas = value; }
    }

    public Pot() : base("assets\\sprites\\triangle.png") {
        SetOrigin(width / 2, height / 2);

        _canvas = new Canvas(width, 40);
        //_canvas.graphics.Clear(Color.Red);
        //_canvas.alpha = .33f;
    }
}

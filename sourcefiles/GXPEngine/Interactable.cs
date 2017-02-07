using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class Interactable:Sprite
{
    private String _type;

    public Interactable(String pType):base("assets\\sprites\\checkers.png")
    {
        _type = pType;
    }
}


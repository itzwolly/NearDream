using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class PressurePlate : Sprite
{
    private string _pressurePlateName;
    private string _opensThis;
    private bool _open;
    public NLineSegment coverLine;
    public bool cover;
    private Level _level;

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

    public PressurePlate(Level pLevel, float pX, float pY,string pOpensThis, bool pCover,int coverHight, int coverWidth) : base(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\pressureplate.png")
    {
        _level = pLevel;
        SetOrigin(width / 2, height / 2);
        x = pX;
        y = pY;
        coverLine = new NLineSegment(new Vec2(x-coverWidth/2,y-coverHight+height-1), new Vec2(x + coverWidth / 2, y - coverHight+height-1), 0xffffff00, 4);
        cover = pCover;
        _opensThis = pOpensThis;
    }

    public void OpenCorresponding(Sprite pSprite) {
        if (pSprite.SpriteName == _opensThis) {
            pSprite.Destroy();
            if (pSprite is Plank) {
                Plank plank = (pSprite as Plank);
                _level.GetDestroyables().Remove(plank);
            }
        }
    }

    public string GetOpensThis() {
        return _opensThis;
    }
}


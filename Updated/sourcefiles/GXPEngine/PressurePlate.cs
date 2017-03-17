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
    float _minX, _maxX;

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

    public PressurePlate(Level pLevel, float pX, float pY,string pOpensThis, bool pCover,int coverHeight, int coverWidth, string pName) : base(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\pressureplate.png")
    {
        _level = pLevel;
        _pressurePlateName = pName;

        SetOrigin(width / 2, height / 2);
        x = pX;
        y = pY;

        if (pName == "Pressureplate_2") {
            if (pLevel.CurrentLevel != 10) {
                _minX = x - coverWidth / 2;
                _maxX = x - coverWidth / 2;
            } else {
                _minX = (x - coverWidth / 2) + 68;
                _maxX = (x + coverWidth / 2) + 35;
            }
            coverLine = new NLineSegment(new Vec2(_minX, y - coverHeight + height - 1), new Vec2(_maxX, y - coverHeight + height - 1), 0xffffff00, 4);
        } else if (pName == "Pressureplate_3") {
            if (pLevel.CurrentLevel != 10) {
                _minX = x - coverWidth / 2;
                _maxX = x - coverWidth / 2;
            } else {
                _minX = (x - coverWidth / 2) - 150;
                _maxX = (x + coverWidth / 2) - 130;
            }
            coverLine = new NLineSegment(new Vec2(_minX, y - coverHeight + height - 1), new Vec2(_maxX, y - coverHeight + height - 1), 0xffffff00, 4);
        } else {
            coverLine = new NLineSegment(new Vec2(x - coverWidth / 2, y - coverHeight + height - 1), new Vec2(x + coverWidth / 2, y - coverHeight + height - 1), 0xffffff00, 4);
        }

        Console.WriteLine(pName);

        cover = pCover;
        _opensThis = pOpensThis;
    }

    public void OpenCorresponding(Sprite pSprite) {
        if (pSprite.SpriteName == _opensThis) {
            pSprite.Destroy();
            if (pSprite is Plank) {
                Plank plank = (pSprite as Plank);
                _level.GetLines().Remove(plank.PlankLine);
                plank.PlankLine.Destroy();
                _level.GetDestroyables().Remove(plank);

            }
        }
    }

    public string GetOpensThis() {
        return _opensThis;
    }
}


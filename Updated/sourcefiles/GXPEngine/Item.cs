using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Item : AnimationSprite
{
    public Item(string pFileName, int pSpriteSheetCol, int pSpriteSheetRow) : base(pFileName, pSpriteSheetCol, pSpriteSheetRow) {
        //SetOrigin(width / 2, height / 2);
    }
}

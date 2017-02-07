﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Unmovable : GameTile
{
    //Level pLevel, string fileName, uint pTile, int pSpriteSheetCol, int pSpriteSheetRow
    public Unmovable(Level pLevel, string pFileName, uint pTile, int pSpriteSheetCol, int pSpriteSheetRow) : base(pLevel, pFileName, pTile, pSpriteSheetCol, pSpriteSheetRow)
    {
        SetOrigin(width / 2, height / 2);
    }
}


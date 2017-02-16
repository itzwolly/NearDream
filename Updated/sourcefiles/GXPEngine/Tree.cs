using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Tree : Sprite
{
    public Tree(string pFileName) : base (pFileName) {
        SetOrigin(width, height);
        if (pFileName == MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\tree_1.png") {
            width = 600;
            height = 1200;
        } else {
            width = 1000;
            height = 1350;
        }
        
    }

    public void MoveTree(float pAmount) {
        x -= pAmount;
    }
}

using System;
using GXPEngine;
using System.Collections.Generic;

public class GameTile : AnimationSprite {
    private Map _map;
    private uint _tileId;
    private List<int> test = new List<int>();
    private Level _level;
    public Vec2 position;

    public GameTile(Level pLevel, Layer pLayer, string fileName, uint pTile, int pSpriteSheetCol, int pSpriteSheetRow)
        : base(fileName, pSpriteSheetCol, pSpriteSheetRow) {
        _level = pLevel;
        SetOrigin(width / 2, height / 2);
        TMXParser tmxParser = new TMXParser();
        _map = tmxParser.ParseFile("assets\\level_" + _level.CurrentLevel + ".tmx");
        
        _tileId = pTile;

        currentFrame = (int)pLayer.Data.GetGId((uint)_tileId);
        rotation = pLayer.Data.GetRotation((uint)_tileId);
    }

    public uint GetTileId() {
        return _tileId;
    }
}

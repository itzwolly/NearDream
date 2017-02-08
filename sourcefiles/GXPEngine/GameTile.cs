using System;
using GXPEngine;
using System.Collections.Generic;

public class GameTile : AnimationSprite {
    Map _map;
    uint _tileId;
    List<int> test = new List<int>();
    Level _level;
    public Vec2 position;

    public GameTile(Level pLevel, string fileName, uint pTile, int pSpriteSheetCol, int pSpriteSheetRow)
        : base(fileName, pSpriteSheetCol, pSpriteSheetRow) {
        _level = pLevel;
        SetOrigin(width / 2, height / 2);
        TMXParser tmxParser = new TMXParser();
        _map = tmxParser.ParseFile("assets\\level_" + _level.CurrentLevel + ".tmx");
        
        _tileId = pTile;

        currentFrame = (int)_map.Layer[0].Data.GetGId((uint)_tileId) - 1;
        rotation = _map.Layer[0].Data.GetRotation((uint)_tileId);
    }

    public uint GetTileId() {
        return _tileId;
    }
}

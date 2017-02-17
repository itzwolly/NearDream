using System;
using System.Collections.Generic;
using System.Linq;
using GXPEngine;

public class Layer : CustomRenderer {

    /* Fields */
    private GameTile _tile;
    private Level _level;
    private List<GameTile> _tiles = new List<GameTile>();
    private uint[,] _layerLayout;

    public enum Direction {
        NONE, UP,
        RIGHT, DOWN,
        LEFT
    }

    //Constructor
    public Layer(Level pLevel, TiledLayer pTiledLayer) {
        _level = pLevel;
        BuildLayer(pTiledLayer);
    }

    /* Methods */
    private void Update() {
        //empty
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pTiledLayer"></param>
    private void BuildLayer(TiledLayer pTiledLayer) {
        pTiledLayer.Data.SetLevelArray(_level.GetMap().Height, _level.GetMap().Width);
        _layerLayout = pTiledLayer.Data.GetLevelArray();
        for (int row = 0; row < _layerLayout.GetLength(0); row++) {
            for (int col = 0; col < _layerLayout.GetLength(1); col++) {
                uint tile = _layerLayout[row, col];
                CreateTile(pTiledLayer, row, col, tile);
            }
        }
    }

    private void CreateTile(TiledLayer pLayer, int pRow, int pCol, uint pTile) {
        if (pTile != 0) {
            foreach (TileSet tileSet in _level.GetMap().TileSet) {
                if (pLayer.Name.ToLower() == tileSet.Name) {
                    _tile = new GameTile(_level, pLayer, MyGame.GetAssetFilePath(MyGame.Asset.ROOT) + "\\" + tileSet.Image.Source, pTile - (uint) tileSet.FirstGId, tileSet.Columns, tileSet.TileCount / tileSet.Columns);
                    _tile.x = (pCol * _level.GetMap().TileWidth) + (_tile.width / 2);
                    _tile.y = (pRow * _level.GetMap().TileHeight) + (_tile.height / 2);

                    _tiles.Add(_tile);

                    AddChild(_tile);
                    AddScenery(_tile);
                }
            }
        }
    }


    // For now we only need them to move in one direction.
    public void MoveLayer(Direction pDirection, float pAmount) {
        switch (pDirection) {
            case Direction.UP:
                y -= pAmount;
                break;
            case Direction.RIGHT:
                x += pAmount;
                break;
            case Direction.DOWN:
                y += pAmount;
                break;
            case Direction.LEFT:
                x -= pAmount;
                break;
            // in both cases just break
            case Direction.NONE:
            default:
                break;
        }
    }

    public List<GameTile> GetTiles() {
        return _tiles;
    }
}

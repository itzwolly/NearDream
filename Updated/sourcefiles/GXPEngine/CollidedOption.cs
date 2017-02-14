using System;
using System.Collections.Generic;
using GXPEngine;

public struct CollidedOption {
    public enum Direction {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public Direction dir;
    public Sprite obj;
}

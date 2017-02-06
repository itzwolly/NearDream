using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;


    class Level:GameObject
    {

        public enum direction
        {
            none, middle, left, right,below,above
        }

        public struct CollidedOption
        {
            public direction dir;
            public Sprite obj;
        }

        private List<Unmovable> _colidables;

        public Level()
        {
            _colidables = new List<Unmovable>();
        }

    public CollidedOption CheckCollision(Sprite who)
    {
        CollidedOption co = new CollidedOption();
        co.dir = direction.none;
        co.obj = null;

        float _distanceX,_distanceY;

        foreach (Unmovable unmovable in _colidables)
        {
            _distanceX = unmovable.width / 2 + who.width / 2;
            _distanceY = unmovable.height / 2 + who.height / 2;
            if (who.x + _distanceX < unmovable.x||
                who.x - _distanceX > unmovable.x ||
                who.y + _distanceY < unmovable.y ||
                who.y - _distanceY > unmovable.y)
                break;
            if (who.x < unmovable.x)
            {
                co.dir = direction.left;
                co.obj = unmovable;
                return co;
            }
            if (who.x > unmovable.x)
            {
                co.dir = direction.right;
                co.obj = unmovable;
                return co;
            }
            if (who.y < unmovable.y)
            {
                co.dir = direction.above;
                co.obj = unmovable;
                return co;
            }
            if (who.y > unmovable.y)
            {
                co.dir = direction.below;
                co.obj = unmovable;
                return co;
            }

        }

        return co;
    }
    }


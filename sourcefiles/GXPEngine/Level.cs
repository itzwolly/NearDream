using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;


    class Level:GameObject
    {
    private List<Unmovable> _colidables;
        public Level()
        {
            _colidables = new List<Unmovable>();
        }
    }


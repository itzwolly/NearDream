using System;
using System.Collections.Generic;
using GXPEngine;

public class Fan : AnimationButton {
    private int _timer;

    public Fan(string pFileName, int pCol, int pRow) : base(pFileName, pCol, pRow) {
        SetOrigin(width / 2, height / 2);
    }

    private void Update() {
        _timer++;
        if (_timer == 3) {
            NextFrame();
            _timer = 0;
        }
    }
}

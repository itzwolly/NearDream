using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StoneAnim : Pausable {
    private Stone _stone;
    private int _wait;

    public StoneAnim(Stone pStone) : base("assets\\sprites\\rocksprite.png", 2, 5) {
        _stone = pStone;
        SetOrigin(width / 2, height / 2);
    }

    private void Update() {
        if (_stone.active) {
            if (_wait > 2) {
                NextFrame();
                _wait = 0;
            }
            _wait++;
        }
    }
}

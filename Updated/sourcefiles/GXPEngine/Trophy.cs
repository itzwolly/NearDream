using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Trophy : Item {
    int index = 0;
    private int _id;

    public int Id {
        get { return _id; }
        set { _id = value; }
    }

    public Trophy(int pTrophyNumber, string pFileName, int pSpriteSheetCol, int pSpriteSheetRow) : base(pFileName, pSpriteSheetCol, pSpriteSheetRow) {
        _id = pTrophyNumber;
        scale = 0.65f;
    }

    private void Update() {
        index++;

        if (index == 3) {
            NextFrame();
            index = 0;
        }

        if (currentFrame > 58) {
            currentFrame = 0;
        }
    }
}

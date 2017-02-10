using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Trophy : Item {
    int index = 0;

    public Trophy(string pFileName, int pSpriteSheetCol, int pSpriteSheetRow) : base(pFileName, pSpriteSheetCol, pSpriteSheetRow) {
        //currentFrame = 1;
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

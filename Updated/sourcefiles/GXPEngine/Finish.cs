using System;
using System.Collections.Generic;

public class Finish : Item {

    int index = 0;
    private int _id;

    public Finish(string pFileName, int pSpriteSheetCol, int pSpriteSheetRow) : base(pFileName, pSpriteSheetCol, pSpriteSheetRow) {
        SetOrigin(width / 2, height * 0.32f);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Trophy : Item {
    public Trophy(string pFileName, int pSpriteSheetCol, int pSpriteSheetRow) : base(pFileName, pSpriteSheetCol, pSpriteSheetRow) {
        currentFrame = 1;
    }

    private void Update() {
        //NextFrame();
    }
}

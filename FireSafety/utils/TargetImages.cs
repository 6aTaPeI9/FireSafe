using System;
using System.Drawing;
using System.Collections.Generic;

namespace FireSafety.utils
{
    [Serializable]
    class TargetImages
    {
        public Image img;
        public Dictionary<string, string> coord_blockers;

        public TargetImages(Image img, Dictionary<string, string> coord)
        {
            this.img = img;
            this.coord_blockers = coord;
        }
    }
}

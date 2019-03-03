using System;
using System.Drawing;
using System.Collections.Generic;

namespace FireSafety.utils
{
    [Serializable]
    class GameImages
    {
        public string img_name { get; set; }
        public Image img { get; set; }

        public GameImages(string image_name, Image img)
        {
            this.img_name = image_name;
            this.img = img;
        }
    }
}

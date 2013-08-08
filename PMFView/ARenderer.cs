using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceballView
{
    public abstract class ARenderer
    {
        private Image _img;

        public Image Img
        {
            get { return _img; }
            set { _img = value; }
        }

        public int Width { get; set; }
    }
}

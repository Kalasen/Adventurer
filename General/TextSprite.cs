using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventurer
{
    public class TextSprite : ISprite
    {
        Graphics graphics;
        string text;
        Vector2 pos;
        Color color;

        public TextSprite(Graphics graphics, string text, Vector2 pos, Color color)
        {
            this.text = text;
            this.pos = pos;
            this.color = color;
        }

        public void Draw()
        {
            
        }
    }
}

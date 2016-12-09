using System.Drawing;

namespace Adventurer
{
    //This is sort of just a bundle of stored arguments for DrawText, representing a drawable sprite of text
    public class TextSprite : IDrawable
    {
        string text;
        Vector2 pos;
        Color color;
        Fonts font;

        public TextSprite(string text, Vector2 pos, Color color = default(Color), Fonts font = Fonts.VeraSmall)
        {
            this.text = text;
            this.pos = pos;
            this.color = color;
            this.font = font;
        }

        public void Draw(Graphics graphics)
        {
            graphics.DrawText(text, pos, color, font);
        }
    }
}

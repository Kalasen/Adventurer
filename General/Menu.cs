using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventurer
{
    //Represents a hierarchical menu structure
    public class Menu
    {
        Menu parent;
        List<Menu> children = new List<Menu>();
        List<string> menuItems = new List<string>();

        Vector2 pos;
        string title;
        Color color;

        public Menu(string title, Vector2 pos, Color color = default(Color))
        {
            this.title = title;
            this.pos = pos;
            this.color = color;
        }

        public void Draw(Graphics graphics)
        {
            graphics.DrawText(title, new Vector2(pos.X, pos.Y), color, Fonts.Vera);
            graphics.DrawText("Child menus and menu items go here", new Vector2(pos.X + 30, pos.Y + 30), color, Fonts.Vera);
        }
    }
}

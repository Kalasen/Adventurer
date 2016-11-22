using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    public class Fixture
    {
        public string type; //The type of fixture this is. TODO: YO PAST-ME, this is dumb, you can check types.
        public int imageIndex; //The image to draw this as
        public Color color; //The color of the fixture

        public Fixture(string type, int imageIndex, Color color)
        {
            this.type = type;
            this.imageIndex = imageIndex;
            this.color = color;
        }
    } //A semi-permanent dungeon fixture, like a door or stairs
}

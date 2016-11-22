using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    public class Door : Fixture
    {
        public bool isOpen {get;set;} //Change to flags enum
		public bool isVertical {get;set;}

        public Door(Tile tile, bool isVertical)
            : base("door", 43, Color.FromArgb(200,200,200))
        {
			this.isVertical = isVertical;
            this.isOpen = false;
            tile.isTransparent = false;
            tile.isPassable = false;
            tile.isDoor = true;
        }

        public void Close(Tile tile, Level currentLevel)
        {
            foreach (Creature c in currentLevel.creatures)
                if (currentLevel.tileArray[(int)tile.pos.X, (int)tile.pos.Y].pos == c.pos)
                    return; //Stop if a creature's blocking it.

            this.isOpen = false;
            base.imageIndex = 43;
            tile.isTransparent = false;
            tile.isPassable = false;
        } //Closes the door
        public void Open(Tile tile, Level currentLevel)
        {
            this.isOpen = true;

            if (isVertical)
                base.imageIndex = 45; // "|"
            else
                base.imageIndex = 124; // "-"

            tile.isTransparent = true;
            tile.isPassable = true;
        } //Opens the door
    } //It opens and closes
}

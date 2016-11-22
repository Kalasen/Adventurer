using System;
using System.Net.Mime;
using System.Collections.Generic;
using System.Text;

namespace Adventurer
{
    public class Room
    {
        public int roomNumber, x, y, width, height, doorCount;
        public bool isIsolated;

        public Room(int roomNumber, int x, int y, int width, int height)
        {
            this.roomNumber = roomNumber;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.doorCount = 0;
        }
		
		public bool IsInRoom(Vector2 pos)
		{
			return (pos.X >= x &&
			    	pos.X <= x + width &&
			    	pos.Y >= y &&
			    	pos.Y <= y + height); //If within room, return true, else false.
		}
    } //Defines a room in the dungeon
}
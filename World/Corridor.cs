using System;
using System.Collections.Generic;
using System.Text;

namespace Adventurer
{
    public class Corridor
    {
        public Vector2 pointA {get;protected set;}
        public Vector2 pointB {get;protected set;}

		public Corridor():this(new Vector2(), new Vector2()){}
        public Corridor(Vector2 a, Vector2 b)
        {
            this.pointA = a;
            this.pointB = b;
        }
		public Corridor(Corridor c)
		{
			this.pointA = c.pointA;
			this.pointB = c.pointB;
		}
    } //A hallway through the dungeon
}

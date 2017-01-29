using KalaGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adventurer
{
    public class Corridor
    {
        public Point2D pointA {get;protected set;}
        public Point2D pointB {get;protected set;}

		public Corridor():this(new Point2D(), new Point2D()){}
        public Corridor(Point2D a, Point2D b)
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

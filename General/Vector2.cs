using System;

namespace Adventurer
{
    // Represents an (x,y) coordinate
    public struct Vector2
    {
        // Variables
        public short X;
        public short Y;

        // Constructors
        public Vector2(int X, int Y):this((short)X, (short)Y) { }
        public Vector2(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }

        // Handle usual operators for structs
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            if (a.X == b.X && a.Y == b.Y) //TODO: Simplify into one-liner
                return true;
            else
                return false;
		}
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            if (a.X == b.X && a.Y == b.Y) //TODO: Simplify into one-liner
                return false;
            else
                return true;
		}
        public override bool Equals(object b)
        {
            return false; //Bluh to stop warnings
        }
        public override int GetHashCode()
        {
            return -1; //Bluh to stop warnings
        }

        //Converts a relative direction into an absolute position
        public Vector2 AdjacentVector(Directions dir)
        {
            switch (dir)
            {
                case Directions.SW:
                    return new Vector2(X - 1, Y + 1);
                case Directions.S:
                    return new Vector2(X    , Y + 1);
                case Directions.SE:
                    return new Vector2(X + 1, Y + 1);
                case Directions.W:
                    return new Vector2(X - 1, Y    );
                case Directions.E:
                    return new Vector2(X + 1, Y    );
                case Directions.NW:
                    return new Vector2(X - 1, Y - 1);
                case Directions.N:
                    return new Vector2(X    , Y - 1);
                case Directions.NE:
                    return new Vector2(X + 1, Y - 1);
                default:
                    throw new Exception("Received unhandled Direction");
            }
        }
    }
}

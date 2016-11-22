using System;
using System.Collections.Generic;
using System.Text;

namespace Adventurer
{
    public struct Vector3
    {
        public int X;
        public int Y;
        public int Z;

        public Vector3(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            if (a.X == b.X && a.Y == b.Y && a.Z == b.Z) //TODO: Simplify to one-liner
                return true;
            else
                return false;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            if (a.X == b.X && a.Y == b.Y && a.Z == b.Z) //TODO: Simplify to one-liner
                return false;
            else
                return true;
        }

        public override bool Equals(Object b)
        {
            return false; //Bluh to stop warnings //TODO: implement properly
        }

        public override int GetHashCode()
        {
            return -1; //Bluh to stop warnings
        }
    }
}

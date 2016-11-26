using System;

namespace Adventurer
{
    public static class Direction
    {
        private static Directions ConvertNumPadToDirection(byte numPadDir)
        {
            switch(numPadDir)
            {
                case 1:
                    return Directions.SW;
                case 2:
                    return Directions.S;
                case 3:
                    return Directions.SE;
                case 4:
                    return Directions.W;
                case 6:
                    return Directions.E;
                case 7:
                    return Directions.NW;
                case 8:
                    return Directions.N;
                case 9:
                    return Directions.NE;
                default:
                    throw new Exception("Unhandled direction " + numPadDir);
            }
        }
        public static Directions Parse(string obj)
        {
            return ConvertNumPadToDirection(byte.Parse(obj));
        }
    }
}
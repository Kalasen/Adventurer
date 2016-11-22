using System;

namespace Adventurer
{
    // Represents a dice based RNG
    public class Dice
    {
        //Underlying number-based RNG
        Random rng = new Random();

        //Constructors
        public Dice() : this((int)DateTime.Now.Ticks) { }        
        public Dice(int seed)
        {
            rng = new Random(seed);
        }

        //Roll some dice in the form number of dice * number of sides per die + bonus to be added after
        public int Roll() { return Roll(6); }
        public int Roll(int sides) { return Roll(1, sides); }
        public int Roll(int nDie, int sides) { return Roll(nDie, sides, 0); }
        public int Roll(int nDie, int sides, int bonus) { return Roll(new DNotation(nDie, sides, bonus)); }
        public int Roll(DNotation nds)
        {
            int result = 0;

            for (int i = 0; i < nds.nDie; i++)
            {
                result += rng.Next(0, nds.sides);
                result++; //Because rng.Next(x,y) is inclusive on x but exclusive on y. YYYYYYYYYYYY
            }

            result += nds.bonus;

            return result;
        }
    }
}

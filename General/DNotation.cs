namespace Adventurer
{
    //Represents notation in the form xDy+z, where x is number of dice, y is sides per die, and z is a bonus added after
    public class DNotation
    {
        //Key variables
        public int nDie {get;set;} //Number of individual dice to be rolled
        public int sides {get;set;} //Sides per die
        public int bonus {get;set;} //Bonus to add after the roll
		
        //Properties based on local variables
		public int lower
		{
			get
			{
				return nDie + bonus;
			}
		}
		public int average 
		{
			get
			{
				return (lower + upper) / 2;
			}
		}
		public int upper
		{
			get
			{
				return nDie * sides + bonus;
			}
		}
		
        //Constructors
		public DNotation():this(6){}
        public DNotation(int sides):this(1,sides){}
        public DNotation(int nDie, int sides):this(nDie, sides, 0){}
        public DNotation(int nDie, int sides, int bonus)
        {
            this.nDie = nDie;
            this.sides = sides;
            this.bonus = bonus;
        }
		public DNotation(DNotation d)
        {
            this.nDie = d.nDie;
            this.sides = d.sides;
            this.bonus = d.bonus;
        }
    }
}

using System;
using System.Drawing;

namespace Adventurer
{
	public class Currency : Item
	{
		public int worth {get; protected set;}
		
		public Currency() :this(1){}
		public Currency(int worth) :this(1f, 1, "coin", Color.Gold, worth){}
		public Currency(float mass, int volume, string name, Color color, int worth)
			:base(mass, volume, name, color)
		{
			this.worth = worth;
			base.itemImage = 36;
		}
		public Currency(Currency c)
			:base(c)
		{
			this.worth = c.worth;
		}		
	} //Coin, paper, etc.
}


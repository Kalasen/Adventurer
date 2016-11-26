using System;
using System.Collections.Generic;
using System.Drawing;

namespace Adventurer
{
	public class Currency : Item
	{
		public int worth {get; protected set;}
		
		public Currency(float mass, int volume, string name, Color color, int worth, List<Item> components, List<string> uses)
			:base(mass, volume, name, color, components, uses)
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


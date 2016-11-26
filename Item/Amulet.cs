using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Adventurer
{
    public class Amulet : Item
    {
        //TODO: Move this over to base Item
        public Effect effect {get;set;}
        
        public Amulet(float mass, float volume, string name, Color color, List<Item> components, List<string> uses)
            : base(mass, volume, name, color, components, uses)
        {
            base.itemImage = 34; //The " symbol
            this.effect = effect;
        }
		public Amulet(Amulet a):base(a)
		{
			this.effect = a.effect;
		}
    } //MANLY necklace
}

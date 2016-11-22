using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Adventurer
{
    public class Amulet : Item
    {
        public Effect effect {get;set;}
		
		public Amulet():this("missingmulet"){} //TODO: Should be able to avoid this error value
		public Amulet(string name):this(name, Color.White){}
		public Amulet(string name, Effect effect):this(name, Color.White, effect){}
		public Amulet(string name, Color color):this(name, color, new Effect()){}
		public Amulet(string name, Color color, Effect effect):this(1f,1,name,color, effect){}
        public Amulet(float mass, float volume, string name, Color color, Effect effect)
            : base(mass, volume, name, color)
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

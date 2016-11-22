using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace Adventurer
{
    public class Scroll : Item
    {
		public Effect effect {get;set;}
		
		public Scroll():this("missingscroll"){}
		public Scroll(string name):this(name, new List<Item>()){}
		public Scroll(string name, List<Item> componentList):this(name, Color.White, componentList){}
		public Scroll(string name, Color color):this(name, color, new List<Item>()){}
		public Scroll(string name, Effect effect):this(name, new List<Item>(), effect){}
		public Scroll(string name, List<Item> componentList, Effect effect):this(name, Color.White, componentList, effect){}		              
        public Scroll(string name, Color color, List<Item> componentList):this(name, color, componentList, new Effect()){}
		public Scroll(string name, Color color, List<Item> componentList, Effect effect):this(1f, 1, name, color, componentList, effect){}
		public Scroll(float mass, int volume, string name, Color color, List<Item> componentList, Effect effect)
			:base(mass, volume, name, color, componentList, new List<string>())
        {
			base.itemImage = 63; // The ? symbol			
			this.effect = effect;            
        }
		public Scroll(Scroll s):base(s)
		{
			this.effect = s.effect;
		}
    } //A scroll is an item that can be read
}
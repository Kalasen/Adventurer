using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    public class Armor : Item //Armor is an item that can be worn
    {
        public int aC {get;set;}
        public string shape {get;protected set;}
        public List<string> covers {get;protected set;}

		public Armor():this(1, "missingno", "m[s[i[g[o"){}
		public Armor(int aC, string shape, string name):this(aC, shape, name, Color.White){}			
		public Armor(int aC, string shape, string name, Color color):this(1f, 1, aC, shape, new List<Item>(), name, new List<string>(), color){}
        public Armor(float mass, float volume, int aC, string shape, List<Item> component, string name,
            List<string> covers, Color color)
            : base(mass, volume, name, color)
        {
            this.aC = aC;
            this.covers = covers;
            this.shape = shape;
            base.componentList = component;
            base.itemImage = 91; //The armor image '['
        }
		public Armor(Armor a):base(a)
		{
			this.aC = a.aC;
			this.shape = a.shape;
			this.covers = new List<string>();
			foreach(string s in a.covers)
				this.covers.Add(s);
		}
    }
}

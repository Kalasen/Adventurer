using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    public class Atom //The fundamental material structure
    {
        public byte protonCount {get;set;}
        public string name {get;set;}

		public Atom():this("missingnium"){}
		public Atom(string name):this(name, 111){}
        public Atom(string name, byte protonCount)
        {
            this.name = name;
            this.protonCount = protonCount;
        }
		public Atom(Atom a)
		{
			this.protonCount = a.protonCount;
			this.name = a.name;
		}

        //String representation of an atom should be its name
        public override string ToString()
        {
            return name;
        }
    }
}

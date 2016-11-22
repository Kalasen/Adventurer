using System;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace Adventurer
{
    //Made of a bunch of atoms
    public class Molecule
    {
        public string name {get;set;}
		public float meltPoint {get;set;}
		public float boilPoint {get;set;}
        public IEnumerable<Atom> atomList {get;set;}

		public Molecule():this("missingmo"){}
		public Molecule(string name):this(name, new List<Atom>()){}
		public Molecule(string name, IEnumerable<Atom> atomList):this(name, 0f, 100f, atomList){}
        public Molecule(string name, float meltPoint, float boilPoint, IEnumerable<Atom> atomList)
        {
            this.name = name;
            this.meltPoint = meltPoint;
            this.boilPoint = boilPoint;
            this.atomList = atomList;
        }
		public Molecule(Molecule m)
		{
			this.name = m.name;
			this.meltPoint = m.meltPoint;
			this.boilPoint = m.boilPoint;
			this.atomList = m.atomList;
		}

        //String representation of an atom should be its name
        public override string ToString()
        {
            return name;
        }
    }
}

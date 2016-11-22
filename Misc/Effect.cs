using System;
using System.Collections.Generic;
using System.Text;

namespace Adventurer
{
    public class Effect
    {
        public int magnitude {get;set;} //How powerful the effect is
        public string type {get;set;} //What the effect does

		public Effect():this(1,"missingtype"){}
        public Effect(int magnitude, String type)
        {
            this.magnitude = magnitude;
            this.type = type;
        }
		public Effect(Effect e)
		{
			this.magnitude = e.magnitude;
			this.type = e.type;
		}
    } //A (semi?, not?) magical effect
}

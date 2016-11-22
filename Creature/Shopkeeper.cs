using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Adventurer
{
    class Shopkeeper : Creature //A creature that gives quests. What?
    {
		Room shop; //The room the shopkeeper patrols
		
        public Shopkeeper(byte speed, DNotation attack, int creatureImage,
            string name, Color color, List<BodyPart> anatomy, int mass, int rngSeed, List<Item> giveWantOptions)
            :base(speed, attack, creatureImage, name, color, anatomy, mass, rngSeed)
        {                  
        }

        public Shopkeeper(Shopkeeper s)
            : base(s)
        {
			this.shop = s.shop;
        }
    }
}

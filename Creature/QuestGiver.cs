using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Adventurer
{
    class QuestGiver : Creature //A creature that gives quests. What?
    {
        public string wantObject, giveObject;

        public QuestGiver(byte speed, DNotation attack, int creatureImage,
            string name, Color color, List<BodyPart> anatomy, int mass, int rngSeed, List<Item> giveWantOptions)
            :base(speed, attack, creatureImage, name, color, anatomy, mass, rngSeed)
        {
            rng = new Random(rngSeed); //Persistence possible
            wantObject = giveWantOptions[rng.Next(0, giveWantOptions.Count)].name;

            giveObject = wantObject;

            while (wantObject == giveObject && giveWantOptions.Count > 1)
            {
                Item giveItem = giveWantOptions[rng.Next(0, giveWantOptions.Count)];
                giveObject = giveItem.name;
                inventory.Add(giveItem); //Make sure s/he actually has the item.
            }                    
        }

        public QuestGiver(QuestGiver c)
            : base(c)
        {
            this.wantObject = c.wantObject;
            this.giveObject = c.giveObject;
        }

        public void CycleWantGiveItem(List<Item> giveWantOptions)
        {
            wantObject = giveWantOptions[rng.Next(0, giveWantOptions.Count)].name;

            giveObject = wantObject;

            while (wantObject == giveObject && giveWantOptions.Count > 1)
            {
                Item giveItem = inventory[rng.Next(0, inventory.Count)];
                giveObject = giveItem.name;
            }
        }
    }
}

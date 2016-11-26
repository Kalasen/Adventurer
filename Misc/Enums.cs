using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventurer
{
    [Flags]
    public enum BodyPartFlags
    {
        None = 0,
        UsedForBreathing = 1 << 0,
        CanUseWeapon = 1 << 1,
        CanHoldItem = 1 << 2,
        CanWearJewelry = 1 << 3,
        CanPickUpItem = 1 << 4,
        CanHear = 1 << 5,
        CanSee = 1 << 6,
        CanSmell = 1 << 7,
        LifeCritical = 1 << 8
    }

    public enum Directions
    {
        SW = 1,
        S = 2,
        SE = 3,
        W = 4,
        NONE = 5,
        E = 6,
        NW = 7,
        N = 8,
        NE = 9
    }

    public enum GameState
    {
        None,
        OpeningMenu,
        MainGame,
        CreatureSelect,
        HelpMenu,
        EscapeMenu,
        HealthMenu,
        InventoryMenu,
        NameSelect,
        WaitForPosition
    }

    public enum InjuryLevel
    {
        Healthy,
        Minor,
        Broken,
        Mangled,
        Destroyed
    }
}

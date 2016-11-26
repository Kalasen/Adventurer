using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventurer.General
{
    //Represents a hierarchical menu structure
    public class Menu
    {
        Menu parent;
        List<Menu> children = new List<Menu>();

        public Menu()
        {

        }
    }
}

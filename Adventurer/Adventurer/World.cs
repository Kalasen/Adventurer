// -----------------------------------------------------------------------
// <copyright file="World.cs" company="Kalasen Games">
// GNU GPL
// </copyright>
// -----------------------------------------------------------------------

namespace Adventurer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a universe. Seriously.
    /// </summary>
    public class World
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class. 
        /// </summary>
        public World()
        {
            this.creatures = new List<Creature>();
        }

        /// <summary>
        /// Gets or sets. A list of creatures in the world.
        /// </summary>
        public List<Creature> creatures { get; set; }
    }
}

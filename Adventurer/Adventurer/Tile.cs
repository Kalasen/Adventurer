// -----------------------------------------------------------------------
// <copyright file="Tile.cs" company="Kalasen Games">
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
    /// Represents a 1x2x1 meter space, roughly
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class.
        /// </summary>
        public Tile()
        {
            this.image = ImageName.GRASS;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class.
        /// </summary>
        /// <param name="image">
        /// The image this tile should be drawn with.
        /// </param>
        public Tile(ImageName image)
        {
            this.image = image;
        }

        /// <summary>
        /// Gets or sets. A list of creatures standing on this tile.
        /// </summary>
        public List<Creature> creatures { get; set; }

        /// <summary>
        /// Gets or sets what image this tile should be drawn with.
        /// </summary>
        public ImageName image { get; set; }
    }
}

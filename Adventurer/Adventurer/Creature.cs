// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Creature.cs" company="Kalasen Games">
//   GNU GPL
// </copyright>
// <summary>
//   Represents a creature in the world
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adventurer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents a creature in the world
    /// </summary>
    public class Creature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Creature"/> class.
        /// </summary>
        /// <param name="image">
        /// The image to represent this creature.
        /// </param>
        public Creature(ImageName image)
        {
            this.image = image;
        }

        /// <summary>
        /// Gets or sets the image that should represent this creature.
        /// </summary>
        public ImageName image { get; set; }
    }
}
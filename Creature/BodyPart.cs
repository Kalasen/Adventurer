using Newtonsoft.Json;

namespace Adventurer
{
    /// <summary>
    /// Represents anything from limbs to organs
    /// </summary>
    public class BodyPart
    {
        public string Name { get; set; }
        public int MaxHealth { get; set; }

        private int _currentHealth;
        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                if (_currentHealth > MaxHealth)
                    _currentHealth = MaxHealth;

                if (_currentHealth < 0)
                    _currentHealth = 0;

                _currentHealth = value;
            }
        }

        public BodyPartFlags Flags { get; set; }
        public InjuryLevel Injury
        {
            get
            {
                if (CurrentHealth <= 0)
                    return InjuryLevel.Destroyed;
                if (CurrentHealth <= MaxHealth * 0.25)
                    return InjuryLevel.Mangled;
                if (CurrentHealth <= MaxHealth * 0.50)
                    return InjuryLevel.Broken;
                if (CurrentHealth <= MaxHealth * 0.75)
                    return InjuryLevel.Minor;

                return InjuryLevel.Healthy;
            }
        }

        public BodyPart Parent;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">What to call this body part.</param>
        /// <param name="health">How much damage this part can take. TODO: Percent based?</param>
        /// <param name="flags">The optional bit properties of this body part.</param>
        /// <param name="parent">Fingers connected to the hand, hand connected to the forearm, etc.</param>
        [JsonConstructor]
        public BodyPart(string name, int health, BodyPartFlags flags, BodyPart parent = null)
        {
            Name = name;
            CurrentHealth = MaxHealth = health;
            Flags = flags;
            Parent = parent;
        }


        /// <summary>
        /// Deep copy contructor.
        /// </summary>
        /// <param name="b">The body part to clone.</param>
        public BodyPart(BodyPart b)
        {
            Name = b.Name;
            CurrentHealth = b.CurrentHealth;
            MaxHealth = b.MaxHealth;
            Flags = b.Flags;
            Parent = b.Parent != null ? new BodyPart(b.Parent) : null;
        }

        /// <summary>
        /// Represent this body part in text as its name.
        /// </summary>
        /// <returns>The name of this body part.</returns>
        public override string ToString() => Name;
    }
}
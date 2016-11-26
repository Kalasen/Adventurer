namespace Adventurer
{
    public class Atom //The fundamental material structure
    {
        public byte protonCount {get;set;}
        public string name {get;set;}
        public Atom(string name, byte protonCount)
        {
            this.name = name;
            this.protonCount = protonCount;
        }

        //String representation of an atom should be its name
        public override string ToString()
        {
            return name;
        }
    }
}

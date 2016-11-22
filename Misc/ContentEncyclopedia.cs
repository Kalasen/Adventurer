using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Adventurer
{
    public class ContentEncyclopedia
    {
        public List<Atom> atoms = new List<Atom>();
        public List<Molecule> molecules = new List<Molecule>();
        public List<Material> materials = new List<Material>();
        public List<Item> items = new List<Item>();
        public List<Species> bestiary = new List<Species>();

        public ContentEncyclopedia()
        {
                     
        }

        public void LoadAll()
        {
            LoadAtoms();
            LoadMolecules();
            LoadMaterials();
            LoadItems();
            LoadCreatures();            
        }

        void LoadAtoms()
        {
            var xmlRoot = XDocument.Load("Content/Encyclopaedia Adventura/Atoms.xml").Root;

            //Load in any atoms in the file
            atoms = atoms.Concat(
                from atomNode in xmlRoot.Elements("Atom")
                select new Atom(atomNode.Attribute("name").Value,
                byte.Parse(atomNode.Attribute("protonCount").Value))
            ).ToList();
        }

        void LoadMolecules()
        {
            var xmlRoot = XDocument.Load("Content/Encyclopaedia Adventura/Molecules.xml").Root;

            //Load in any molecules in the file
            molecules = molecules.Concat(
                from moleculeNode in xmlRoot.Elements("Molecule")
                select new Molecule(
                    moleculeNode.Attribute("name").Value,
                    float.Parse(moleculeNode.Attribute("meltingPoint").Value),
                    float.Parse(moleculeNode.Attribute("boilingPoint").Value),
                    GetAtomsInMolecule(moleculeNode)
                )
            ).ToList();
        }

        void LoadMaterials()
        {
            var xmlRoot = XDocument.Load("Content/Encyclopaedia Adventura/Materials.xml").Root;

            var newMaterials = from materialNode in xmlRoot.Elements("Material")
                               select new Material(
                                   materialNode.Attribute("name").Value,
                                   GetMoleculesInMaterial(materialNode),
                                   float.Parse(materialNode.Attribute("density").Value),
                                   100, //Shouldn't boil/melt be calculated from the molecules?
                                   0
                               );
            materials = materials.Concat(newMaterials).ToList();
        }

        void LoadItems()
        {
            var xmlRoot = XDocument.Load("Content/Encyclopaedia Adventura/Items.xml").Root;
            foreach (var itemNode in xmlRoot.Elements("Item"))
            {
                var type = itemNode.Attribute("type").Value;
                switch (type)
                {
                    case "component":
                        LoadComponent(itemNode);                        
                        break;

                    case "amulet":
                        LoadAmulet(itemNode);
                        break;

                    case "armor":
                        LoadArmor(itemNode);
                        break;

                    case "misc":
                    case "tool":
                        LoadMiscItem(itemNode);
                        break;

                    case "potion":
                        LoadPotion(itemNode);
                        break;

                    case "weapon":
                        LoadWeapon(itemNode);
                        break;

                    default:
                        throw new Exception($"Unhandled item type '{type}'in LoadItems");
                }
            }
        }        

        void LoadCreatures()
        {
            //Load in the BodyTypes that species will template from
            var bodyTypes = new Dictionary<string, List<BodyPart>>();

            //TODO: Rest of creature loading
        }

        void LoadComponent(XElement itemNode)
        {
            var mass = (float)itemNode.Attribute("mass");
            var name = (string)itemNode.Attribute("name");
            var newItem = new Item(mass, mass, name);

            var material = (string)itemNode.Attribute("material");
            newItem.material = materials.FirstOrDefault((m) => m.name == material);
            if (newItem.material == null)
                throw new Exception($"The item component '{name}' is made of {material}, which is not a known material. Check the materials.xml file and make sure {material} is set up there, or items.xml to change the {name} definition.");

            items.Add(newItem);
        }

        void LoadAmulet(XElement itemNode)
        {
            var name = (string)itemNode.Attribute("name");
            var color = Color.FromArgb(Convert.ToInt32(int.Parse(itemNode.Attribute("color").Value, System.Globalization.NumberStyles.HexNumber)));
            var newAmulet = new Amulet(name, color);
            newAmulet.componentList = GetComponentsInItem(itemNode);

            items.Add(newAmulet);
        }

        void LoadArmor(XElement itemNode)
        {
            var aC = int.Parse(itemNode.Attribute("defence").Value);
            var name = (string)itemNode.Attribute("name");
            var color = Color.FromArgb(Convert.ToInt32(int.Parse(itemNode.Attribute("color").Value, System.Globalization.NumberStyles.HexNumber)));
            var fitting = itemNode.Attribute("fitting").Value;
            var newItem = new Armor(aC, fitting, name, color);
            newItem.componentList = GetComponentsInItem(itemNode);

            items.Add(newItem);
        }

        void LoadMiscItem(XElement itemNode)
        {
            var name = (string)itemNode.Attribute("name");
            var color = Color.FromArgb(Convert.ToInt32(int.Parse(itemNode.Attribute("color").Value, System.Globalization.NumberStyles.HexNumber)));
            var newItem = new Item(name, color);
            newItem.componentList = GetComponentsInItem(itemNode);

            items.Add(newItem);
        }

        void LoadPotion(XElement itemNode)
        {
            var name = (string)itemNode.Attribute("name");
            var color = Color.FromArgb(Convert.ToInt32(int.Parse(itemNode.Attribute("color").Value, System.Globalization.NumberStyles.HexNumber)));
            var newItem = new Item(name, color);
            newItem.componentList = GetComponentsInItem(itemNode);

            items.Add(newItem);
        }

        void LoadWeapon(XElement itemNode)
        {
            var name = (string)itemNode.Attribute("name");
            var color = Color.FromArgb(Convert.ToInt32(int.Parse(itemNode.Attribute("color").Value, System.Globalization.NumberStyles.HexNumber)));
            //TODO: Load damage
            var newItem = new Weapon(name);
            newItem.componentList = GetComponentsInItem(itemNode);

            items.Add(newItem);
        }

        //TODO: Can probably make these functions into smarter LINQ queries
        List<Atom> GetAtomsInMolecule(XElement moleculeNode)
        {
            var moleculeAtoms = new List<Atom>();
            foreach(var atomNode in moleculeNode.Elements("Atom"))
                moleculeAtoms.AddRange(Enumerable.Repeat(atoms.First(a => a.name == atomNode.Attribute("name").Value),
                                                         int.Parse(atomNode.Attribute("count").Value)));
            return moleculeAtoms;
        }

        List<Molecule> GetMoleculesInMaterial(XElement materialNode)
        {
            var materialMolecules = new List<Molecule>();
            foreach (var moleculeNode in materialNode.Elements("Molecule"))
                materialMolecules.Add(molecules.ToList().Find(molecule => molecule.name == moleculeNode.Attribute("name").Value));
            return materialMolecules;
        }

        List<Item> GetComponentsInItem(XElement itemNode)
        {
            var components = new List<Item>();
            foreach (var componentNode in itemNode.Elements("Component"))
            {
                components.Add(items.Find(component => component.name == componentNode.Attribute("name").Value));
            }
            return components;
        }
    }
}

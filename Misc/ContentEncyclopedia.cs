using KalaGame;
using Newtonsoft.Json;
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

        Dictionary<string, List<BodyPart>> bodyTemplates = new Dictionary<string, List<BodyPart>>();

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

            File.WriteAllText("Content/Encyclopaedia.json", JsonConvert.SerializeObject(this, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto }));
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
            var xmlRoot = XDocument.Load("Content/Encyclopaedia Adventura/Creatures.xml").Root;

            //Load in the BodyTypes that species will template from
            foreach (var bodyTypeNode in xmlRoot.Elements("BodyType"))
            {
                var bodyTemplateName = bodyTypeNode.Attribute("name").Value;

                //Load in the parts in this body type
                var bodyParts = new List<BodyPart>();                
                foreach (var bodyPartNode in bodyTypeNode.Elements("BodyPart"))
                    bodyParts.Add(LoadBodyPart(bodyPartNode));

                bodyTemplates.Add(bodyTemplateName, bodyParts); 
            }

            var newSpecies = new List<Species>();

            //TODO: Rest of creature loading
            foreach (var speciesNode in xmlRoot.Elements("Species"))
                newSpecies.Add(LoadSpecies(speciesNode));

            bestiary.AddRange(newSpecies);            
        }

        BodyPart LoadBodyPart(XElement partNode)
        {
            //Set any flags attached to this body part
            var flags = new BodyPartFlags();
            if (partNode.Attribute("lifeCritical") != null)
                flags |= BodyPartFlags.LifeCritical;
            if (partNode.Attribute("canPickUp") != null)
                flags |= BodyPartFlags.CanPickUpItem;
            if (partNode.Attribute("missingno") != null)
                flags |= BodyPartFlags.LifeCritical;

            return new BodyPart(
                partNode.Attribute("name").Value,
                int.Parse(partNode.Attribute("health").Value),
                flags,
                null
            );
        }

        Species LoadSpecies(XElement speciesNode)
        {
            var anatomy = new List<BodyPart>();
            anatomy.AddRange(bodyTemplates[speciesNode.Element("BodyType").Attribute("name").Value]);

            return new Species(
                byte.Parse(speciesNode.Attribute("speed").Value),
                new DNotation(2, 4, 2),
                500,
                byte.Parse(speciesNode.Attribute("image").Value),
                ColorTranslator.FromHtml(speciesNode.Attribute("color").Value),
                speciesNode.Attribute("name").Value,
                speciesNode.Element("Habitat").Attribute("name").Value,
                anatomy,
                new Random().Next()
            );
        }

        void LoadComponent(XElement itemNode)
        {
            var mass = (float)itemNode.Attribute("mass");
            var name = (string)itemNode.Attribute("name");
            var newItem = new Item(mass, mass, name, Color.White, new List<Item>(), new List<string>());

            var material = (string)itemNode.Attribute("material");
            newItem.material = materials.FirstOrDefault((m) => m.name == material);
            if (newItem.material == null)
                throw new Exception($"The item component '{name}' is made of {material}, which is not a known material. Check the materials.xml file and make sure {material} is set up there, or items.xml to change the {name} definition.");

            items.Add(newItem);
        }

        void LoadAmulet(XElement itemNode)
        {
            var name = (string)itemNode.Attribute("name");
            var color = ColorTranslator.FromHtml(itemNode.Attribute("color").Value);
            var newAmulet = new Amulet(50f, 50f, name, color, GetComponentsInItem(itemNode), new List<string>());

            items.Add(newAmulet);
        }

        void LoadArmor(XElement itemNode)
        {
            var aC = int.Parse(itemNode.Attribute("defence").Value);
            var name = (string)itemNode.Attribute("name");
            var color = ColorTranslator.FromHtml(itemNode.Attribute("color").Value);
            var fitting = itemNode.Attribute("fitting").Value;
            var newItem = new Armor(500f, 500f, aC, fitting, new List<Item>(), name, new List<string>(), color, new List<string>());
            newItem.componentList = GetComponentsInItem(itemNode);

            items.Add(newItem);
        }

        void LoadMiscItem(XElement itemNode)
        {
            var name = (string)itemNode.Attribute("name");
            var color = ColorTranslator.FromHtml(itemNode.Attribute("color").Value);
            var newItem = new Item(100f, 100f, name, color, new List<Item>(), new List<string>());
            newItem.componentList = GetComponentsInItem(itemNode);

            items.Add(newItem);
        }

        void LoadPotion(XElement itemNode)
        {
            var name = (string)itemNode.Attribute("name");
            var color = ColorTranslator.FromHtml(itemNode.Attribute("color").Value);
            var newItem = new Potion(name, 100f, 100f, color, new List<Item>(), new List<string>());
            newItem.componentList = GetComponentsInItem(itemNode);

            items.Add(newItem);
        }

        void LoadWeapon(XElement itemNode)
        {
            var name = (string)itemNode.Attribute("name");
            var color = ColorTranslator.FromHtml(itemNode.Attribute("color").Value);
            //TODO: Load damage
            var newItem = new Weapon(100f, 100f, name, color, new DNotation(2, 4, 2), new List<Item>(), new List<string>());
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
                components.Add(items.First(component => component.name == componentNode.Attribute("name").Value));
            }
            return components;
        }
    }
}

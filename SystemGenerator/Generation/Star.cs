using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static SystemGenerator.Generation.Gen;
using static SystemGenerator.FormMain;

namespace SystemGenerator.Generation
{
    public class Star
    {
        //General characteristics
        public char   type;
        public double m;     //Mass            (Solar masses)
        public double r;     //Radius          (Solar radii)
        public double g;     //Gravity         (Gs)
        public double escV;  //Escape velocity (km/s)
        public double metal; //Metallicity     (dimensionless)

        //Radiance
        public double lumin;  //Luminosity             (Solar lumens)
        public double temp;   //Black-body temperature (K)
        public double bv;     //B-V color index        (dimensionless)
        public double magAbs; //Absolute magnitude     (dimensionless)
        public double magRel; //Relative magnitude from the perspective of
                              //the habitable planet   (dimensionless)

        //Time
        public double life; //Expected lifetime    (by🜨)
        public double age;  //Current age          (by🜨)
        public double y;    //Galactic year length (my🜨)

        //Zones (AU)
        public double zoneFormMin;
        public double zoneFormMax;
        public double zoneHabMin;
        public double zoneHabMax;
        public double zoneFrost;
        public int indexFrost;
        public int indexHab;

        public List<double> orbits;
        public string flavortext;

        public Star()
        {
            Utils.writeLog(Environment.NewLine + "    Generating star");
            this.genProperties();
            this.genOrbits();
            this.flavortext = "";
        }

        /**
         * This function randomly populates the properties of a Star object.
         */
        public void genProperties()
        {
            Utils.writeLog("        Generating physical characteristics");
            this.m     = Utils.randDouble(Gen.Star.STAR_MASS_MIN, Gen.Star.STAR_MASS_MAX);
            this.r     = Math.Pow(this.m, 0.74);
            this.g     = (this.m / Math.Pow(this.r, 2.0)) * 27.9;
            this.escV  = Math.Sqrt(this.m / this.r) * 1118.6 * 55.0;
            this.metal = Utils.fudge(0.01);
            
            Utils.writeLog("        Generating luminosity characteristics");
            this.temp    = Math.Pow(this.m, 0.505) * Const.SUN_TEMP;
            this.bv      = (8464.0 + (-2.1344 * this.temp) + Math.Sqrt(71639296 + (0.98724906 * this.temp * this.temp))) / (1.6928 * this.temp);
            this.lumin   = Math.Pow(this.m, 3.5);
            this.magAbs = 4.8 - (2.5 * Math.Log10(this.lumin));
            this.magRel = 0;

            if (this.temp < 5300.0)
                this.type = 'K';
            else if (this.temp < 6000.0)
                this.type = 'G';
            else
                this.type = 'F';

            this.life = Math.Pow(this.m, -2.5)*11.72;
            this.age  = Utils.randDouble(0.33*0.98*this.life, 0.33*1.02*this.life);
            this.y    = Utils.randDouble(200.0, 300.0);
            
            Utils.writeLog("        Generating planetary zones");
            this.zoneFormMin = this.m *  0.1;
            this.zoneFormMax = this.m * 40.0;
            this.zoneHabMin  = Math.Sqrt(this.lumin) * 0.95;
            this.zoneHabMax  = Math.Sqrt(this.lumin) * 1.35;
            this.zoneFrost   = Math.Sqrt(this.lumin) * 4.85;

            Utils.writeLog("    Star generation complete");
            Utils.updateProgress();
        }
    

        public void genFlavorText()
        {
            string flavor = "";

            flavor += "This star is a main sequence star, with a spectral type " + this.type + ". It is a";

            if (this.bv <= 0.6)
                flavor += " cool white color to human eyes.";
            else if (this.bv <= 0.8)
                flavor += " warm white color to human eyes, like Sol.";
            else if (this.bv <= 1.4)
                flavor += " cream color to human eyes.";
            else
                flavor += "n orange color to human eyes.";

            flavor += String.Format(" From the habitable planet, it would have an apparent magnitude of {0:N3}, appearing ", this.magRel);
            
            flavor += Utils.getMagDesc(this.magRel) + ". ";

            flavor += String.Format("It is currently {0:N3} billion Earth years old, about a third of the way through its life.", this.age);

            this.flavortext = flavor;
        }

        /**
         * This function returns a list of orbital distances in AU based on this star.
         */
        public void genOrbits()
        {
            List<double> orbits;
            bool habitable;
            do
            {
                Utils.writeLog(Environment.NewLine + "    Generating orbital distances");
                orbits = new List<double>();
                double current, frost;

                //Start with the frost line orbit
                orbits.Add(this.zoneFrost + (Utils.randDouble(1.0, 1.2) * Utils.randSign()));
                current = orbits[0];
                frost = orbits[0];
                Utils.writeLog("        Frost orbit: " + frost);

                //Gen outside orbits
                Utils.writeLog("        Generating outer orbits");
                while (true)
                {
                    current *= Utils.randDouble(Gen.System.ORBIT_SPACING_MIN, Gen.System.ORBIT_SPACING_MAX);

                    if (current > this.zoneFormMax)
                        break;

                    if (current > (orbits[orbits.Count-1]+0.15))
                        orbits.Add(current);
                }

                //Reset current
                current = orbits[0];

                //Gen inside orbits
                Utils.writeLog("        Generating inner orbits");
                while (true)
                {
                    current /= Utils.randDouble(Gen.System.ORBIT_SPACING_MIN, Gen.System.ORBIT_SPACING_MAX);

                    if (current < this.zoneFormMin)
                        break;

                    if (current < (orbits[orbits.Count - 1] - 0.15))
                        orbits.Add(current);
                }

                Utils.writeLog("        Total orbits: " + orbits.Count);

                //Bubble sort
                for (int i = 0; i < orbits.Count - 1; i++)
                {
                    bool swapped = false;

                    for (int j = 0; j < orbits.Count - i - 1; j++)
                    {
                        if (orbits[j] > orbits[j+1])
                        {
                            double temp = orbits[j];
                            orbits[j] = orbits[j+1];
                            orbits[j+1] = temp;
                            swapped = true;
                        }
                    }

                    if (!swapped)
                        break;
                }

                //Find the frost index
                this.indexFrost = orbits.IndexOf(frost);

                //Make sure a habitable planet exists
                habitable = false;
                for (int i = 0; i < orbits.Count; i++)
                {
                    if ((this.zoneHabMin < orbits[i]) && (orbits[i] < this.zoneHabMax))
                    {
                        habitable = true;
                        this.indexHab = i;
                        break;
                    }
                }

                Utils.writeLog("        Habitable orbit exists: " + habitable);

                //If not, retry; if yes, return orbit list
                if (!habitable)
                    Utils.writeLog("            Rerandomizing orbits");
                else
                    Utils.writeLog("    Distance generation complete");
            }
            while (!habitable);
            
            this.orbits = orbits;
            Utils.updateProgress();
        }
    
        /**
         * This function picks the type of planet for each orbital distance.
         */
        public List<char> genTypes()
        {
            List<char> types;
            bool migrated, belt_dwarf;
            int num, indexMig, indexF;

            do
            {
                Utils.writeLog(Environment.NewLine + "    Picking types of planets");
                types = new List<char>();
                migrated = false;
                belt_dwarf = false;
                indexMig = -1;
                indexF = this.indexFrost;

                //First do the inner orbits

                //Decide migration
                if (Utils.flip() <= Gen.System.MIGRATE_HOT_CHANCE)
                {
                    types.Add(ID.Planet.GAS_HOT);
                    migrated = true;
                    indexMig = 0;

                    Utils.writeLog("        Frost giant migrated to inner system: orbit 0");
                }

                for (int i = indexMig+1; i < this.indexHab; i++)
                {
                    Decay denseDecay = new Decay(Gen.System.DENSE_DECAY, Gen.System.DENSE_CHANCE, 0, this.indexHab, Decay.DecayDir.DECREASING);
                    Utils.writeLog("            Dense chance: " + denseDecay.getDecayedChance(i));

                    if (Utils.flip() < denseDecay.getDecayedChance(i))
                        types.Add(ID.Planet.ROCK_DENSE);
                    else
                        types.Add(ID.Planet.ROCK_DESERT);
                
                    Utils.writeLog("        Orbit " + i + " (" + this.orbits[i] + " AU): " + Utils.getDescription(types[i]).ToLower() + " (inner orbit)");
                }

                //Second do the habitable orbit
                Decay oceanDecay = new Decay(Gen.System.OCEAN_DECAY, Gen.System.OCEAN_CHANCE, 0, this.indexFrost, Decay.DecayDir.INCREASING);

                if (Utils.flip() < oceanDecay.getDecayedChance(this.indexHab))
                    types.Add(ID.Planet.WATER_GREEN);
                else
                    types.Add(ID.Planet.ROCK_GREEN);

                Utils.writeLog("        Orbit " + this.indexHab + " (" + this.orbits[this.indexHab] + " AU): " + Utils.getDescription(types[this.indexHab]).ToLower() + " (hab orbit)");

                //Third do the mid orbits

                //Decide migration
                if (Utils.flip() <= Gen.System.MIGRATE_MID_CHANCE && !migrated)
                {
                    do
                        indexMig = (int)Math.Round(Utils.randDouble(this.indexHab, indexF));
                    while (indexMig == this.indexHab);
                    
                    migrated = true;
                
                    Utils.writeLog("        Frost giant migrated to mid-system: orbit " + indexMig);
                }

                for (int i = this.indexHab+1; i < indexF; i++)
                {
                    if (i == indexMig && migrated) //If the frost giant has migrated to mid system
                    {
                       types.Add(ID.Planet.GAS_PUFFY);
                    }

                    //If i is immediately before the frost giant, and it has not migrated, determine whether to add an asteroid belt
                    else if (Utils.flip() < Gen.Belt.INNER_BELT_CHANCE && !migrated && i == indexF-1)
                    {
                        types.Add(ID.Belt.BELT_INNER);

                        //Also decide whether to add a dwarf planet to this belt
                        if (Utils.flip() < Gen.Belt.INNER_DWARF_CHANCE)
                        {
                            Utils.writeLog("        Orbit " + i + " (" + this.orbits[i] + " AU): " + Utils.getDescription(types[i]).ToLower() + " (mid orbit)");
                            types.Add(ID.Belt.DWARF);
                            i++;
                            indexF++;
                            belt_dwarf = true;
                        }
                    }

                    //Otherwise create a planet
                    else if (Utils.flip() < oceanDecay.getDecayedChance(i))
                        types.Add(ID.Planet.WATER_OCEAN);
                    else
                        types.Add(ID.Planet.ROCK_DESERT);
                
                    Utils.writeLog("        Orbit " + i + " (" + this.orbits[i] + " AU): " + Utils.getDescription(types[i]).ToLower() + " (mid orbit)");
                }

                //Fourth do the frost orbit (if not migrated)
                if (!migrated)
                {
                    types.Add(ID.Planet.GAS_GIANT);
            
                    Utils.writeLog("        Orbit " + indexF + " (" + this.orbits[indexF] + " AU): " + Utils.getDescription(types[indexF]).ToLower() + " (frost orbit)");
                }
                else
                {
                    types.Add(ID.Planet.EMPTY);

                    Utils.writeLog("        Orbit " + indexF + " (" + this.orbits[indexF] + " AU): empty (frost orbit)");
                }

                //Fifth do the outer orbits
                num = (
                    belt_dwarf
                    ? this.orbits.Count+1
                    : this.orbits.Count
                );

                bool giantsFinished = false; //Once the first non-gas-giant planet generates, gas giants will stop generating entirely

                for (int i = indexF+1; i < num; i++)
                {
                    Decay iceDecay   = new Decay(Gen.System.ICE_GIANT_DECAY, Gen.System.ICE_GIANT_CHANCE, indexF, num, Decay.DecayDir.INCREASING);
                    Decay dwarfDecay = new Decay(Gen.System.ICE_DWARF_DECAY, Gen.System.ICE_DWARF_CHANCE, indexF, num, Decay.DecayDir.INCREASING);

                    double ice_chance   = iceDecay.getDecayedChance(i);
                    double dwarf_chance = dwarfDecay.getDecayedChance(i);
                    double roll;

                    while (true)
                    {
                        roll = Utils.flip();

                        if (roll < ice_chance)
                        {
                            types.Add(ID.Planet.ICE_GIANT);
                            giantsFinished = true;
                            break;
                        }
                        else if (roll > dwarf_chance)
                        {
                            types.Add(ID.Planet.ICE_DWARF);
                            giantsFinished = true;
                            break;
                        }
                        else if (!giantsFinished || Gen.System.DISABLE_GAS_GIANT_CUTOFF)
                        {
                            types.Add(ID.Planet.GAS_GIANT);
                            break;
                        }
                    }
                
                    Utils.writeLog("        Orbit " + i + " (" + this.orbits[i-(num-orbits.Count)] + " AU): " + Utils.getDescription(types[i-(num-orbits.Count)]).ToLower() + " (outer orbit)");
                }

                //If there are more types than orbits, redo
            }
            while (false); //!(( belt_dwarf && types.Count > this.orbits.Count+1) || (!belt_dwarf && types.Count > this.orbits.Count ) )

            //Finally decide whether to generate a kuiper belt
            if (Utils.flip() < Gen.Belt.KUIPER_BELT_CHANCE)
            {
                types.Add(ID.Belt.BELT_KUIPER);
                
                Utils.writeLog("        Orbit " + num + " ( X AU): " + Utils.getDescription(types[num]).ToLower() + " (kuiper orbit)");

                //Decide number of dwarf planets
                int plutinos  = Utils.randInt(Gen.Belt.MIN_PLUTINO  , Gen.Belt.MAX_PLUTINO  );
                int cubewanos = Utils.randInt(Gen.Belt.MIN_CUBEWANO , Gen.Belt.MAX_CUBEWANO );
                int twotinos  = Utils.randInt(Gen.Belt.MIN_TWOTINO  , Gen.Belt.MAX_TWOTINO  );
                int scattered = Utils.randInt(Gen.Belt.MIN_SCATTERED, Gen.Belt.MAX_SCATTERED);
                int sednoids  = Utils.randInt(Gen.Belt.MIN_SEDNOID  , Gen.Belt.MAX_SEDNOID  );

                Utils.writeLog("            Generating " + plutinos + " plutinos, " + cubewanos + " cubewanos, " + twotinos + " twotinos, " + scattered + " SDOs, and " + sednoids + " sednoids");

                for (int i = 0; i < plutinos+cubewanos+twotinos+scattered+sednoids; i++)
                {
                    if (i < plutinos)
                        types.Add(ID.Belt.PLUTINO);
                    else if (i < plutinos+cubewanos)
                        types.Add(ID.Belt.CUBEWANO);
                    else if (i < plutinos+cubewanos+twotinos)
                        types.Add(ID.Belt.TWOTINO);
                    else if (i < plutinos+cubewanos+twotinos+scattered)
                        types.Add(ID.Belt.SCATTERED);
                    else
                        types.Add(ID.Belt.SEDNOID);

                    Utils.writeLog("        Orbit " + (int)(num+1+i) + ": " + Utils.getDescription(types[num+1+i]).ToLower() + " (kuiper orbit)");
                }
            }

            Utils.writeLog("    Type generation complete");
            FormMain.genProgressBar.Maximum = 8 + (types.Count*6);
            Utils.updateProgress();

            return types;
        }

        /**
         * This is the top level function that walks through the whole generation, start to finish.
         */
        public List<Planet> genSystem()
        {
            int kuipIndex = 0, mig = 0;
            bool giantMax = false;
            double dist, maxMass = 0;

            //First create the star
            List<char> types      = this.genTypes();
            List<Planet> planets  = new List<Planet>();
            List<bool> needsSigma = new List<bool>(); //The std devs of belt orbits can't be calculated until the whole system is generated, so planets marked true here will have orbits regenerated

            //Generate
            Planet? controller;

            for (int i = 0; i < types.Count; i++)
            {
                //Skip empty orbits
                if (types[i] == ID.Planet.EMPTY)
                {
                    mig = 1;
                    continue;
                }

                //Types is null terminated
                if (types[i] == '\0')
                    break;

                Planet p = new Planet();
                p.type = types[i];

                Utils.writeLog(Environment.NewLine + "    Generating a " + Utils.getDescription(types[i]).ToLower() + " for orbit " + i);

                //Specify some limits
                //The largest giant occurs at the frost line, which will always be the first one encountered.
                //Dwarf planets that occur in belts must be smaller than the total mass of the belt.
                switch (types[i])
                {
                    case ID.Belt.BELT_KUIPER:
                        controller = null;
                        dist       = this.orbits[i-1-mig];
                        maxMass    = 0.0;
                        kuipIndex  = i-mig;
                        planets[i-1-mig].hasTrojans = true;
                        break;
                    case ID.Planet.GAS_PUFFY:
                    case ID.Planet.GAS_HOT:
                        controller = null;
                        dist       = this.orbits[i];
                        goto case ID.Planet.GAS_SUPER;
                    case ID.Planet.GAS_GIANT:
                    case ID.Planet.GAS_SUPER:
                        controller = null;
                        dist       = this.orbits[i-mig];
                        break;
                    case ID.Belt.DWARF:
                        controller   = planets[i-1];
                        dist         = this.orbits[i-1-mig];
                        maxMass      = planets[i-1].m;
                        this.orbits.Insert(i, this.orbits[i-1-mig]);
                        planets[i-1].subtype = "1";
                        break;
                    case ID.Belt.PLUTINO:
                        controller = planets[kuipIndex];
                        dist       = Utils.resonance(planets[kuipIndex-1].orbit.a, 3.0/2.0);
                        maxMass    = planets[kuipIndex].m / (double)(this.orbits.Count-kuipIndex);
                        break;
                    case ID.Belt.CUBEWANO:
                        controller = planets[kuipIndex];
                        dist       = Utils.randDouble(
                                         Utils.resonance(planets[kuipIndex-1].orbit.a, 3.0/2.0),
                                         Utils.resonance(planets[kuipIndex-1].orbit.a, 2.0/1.0)
                                     );
                        maxMass    = planets[kuipIndex].m / (double)(this.orbits.Count-kuipIndex);
                        break;
                    case ID.Belt.TWOTINO:
                        controller = planets[kuipIndex];
                        dist       = Utils.resonance(planets[kuipIndex-1].orbit.a, 2.0/1.0);
                        maxMass    = planets[kuipIndex].m / (double)(this.orbits.Count-kuipIndex);
                        break;
                    case ID.Belt.SCATTERED:
                        controller = planets[kuipIndex];
                        dist       = planets[kuipIndex-1].orbit.a*2.0;
                        maxMass    = planets[kuipIndex].m / (double)(this.orbits.Count-kuipIndex);
                        break;
                    case ID.Belt.SEDNOID:
                        controller = planets[kuipIndex];
                        dist       = Utils.randDouble(planets[kuipIndex-1].orbit.a*2.0, planets[kuipIndex-1].orbit.a*5.0);
                        maxMass    = planets[kuipIndex].m / (double)(this.orbits.Count-kuipIndex);
                        break;
                    default:
                        try
                        {
                            controller = null;
                            dist       = this.orbits[i-mig];
                            maxMass    = 0.0;
                        }
                        catch
                        {
                            controller = null;
                            dist       = this.orbits[this.orbits.Count-1];
                            maxMass    = 0.0;
                        }
                        break;
                }
                Utils.updateProgress();

                //Generate composition
                if (maxMass > 0.0)
                    p.genPlanetProperties();
                else
                    p.genPlanetProperties(maxMass);

                if (p.type == ID.Belt.DWARF)
                    maxMass = Gen.Planet.Giant.MAX_GIANT_MASS;

                if (p.isGiant && !giantMax)
                {
                    maxMass = p.m;
                    giantMax = true;
                }
                Utils.updateProgress();

                //Creating orbit
                if (controller != null)
                {
                    needsSigma.Add(true);
                    p.orbit.genOrbit(this, p, controller);
                }
                else
                {
                    needsSigma.Add(false);
                    p.orbit.genOrbit(this, p, dist);
                }
                Utils.updateProgress();

                //Creating atmosphere
                p.atmo.genAtmo(this, ref p);
                Utils.updateProgress();
                
                //Creating satellites
                p.moons = Moon.genSatList(this, ref p);
                Utils.updateProgress();

                //Picking feature
                p.genFeature();

                planets.Add(p);
                Utils.updateProgress();
            }
            
            Utils.writeLog(Environment.NewLine + "    Calculating final miscellanous values");
            
            //Generate Kirkwood gaps and distance std dev for inner belts
            if (planets[this.indexFrost-1].type == ID.Belt.BELT_INNER && planets[this.indexFrost].type != ID.Planet.EMPTY)
            {
                Utils.writeLog("      Generating Kirkwood gaps for the inner belt");

                planets[this.indexFrost-1].rings = new List<double>(new double[]{ 
                    Utils.resonance(planets[indexFrost].orbit.a, 1.0/5.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 1.0/4.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 1.0/3.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 2.0/5.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 3.0/7.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 1.0/2.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 2.0/3.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 3.0/4.0)    
                });
                planets[this.indexFrost-1].r = planets[this.indexFrost-1].rings[7] - planets[this.indexFrost-1].rings[0];

                planets[this.indexFrost].hasTrojans = true;

                planets[this.indexFrost-1].orbit.aSigma = planets[this.indexFrost-1].r / 4.0;
            }
            else if (planets[this.indexFrost-2].type == ID.Belt.BELT_INNER && planets[this.indexFrost].type != ID.Planet.EMPTY)
            {
                Utils.writeLog("      Generating Kirkwood gaps for the inner belt");

                planets[this.indexFrost-2].rings = new List<double>(new double[]{ 
                    Utils.resonance(planets[indexFrost].orbit.a, 1.0/5.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 1.0/4.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 1.0/3.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 2.0/5.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 3.0/7.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 1.0/2.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 2.0/3.0),
                    Utils.resonance(planets[indexFrost].orbit.a, 3.0/4.0)    
                });
                planets[this.indexFrost-2].r =
                    planets[this.indexFrost-2].rings[planets[this.indexFrost-2].rings.Count-1] -
                    planets[this.indexFrost-2].rings[0];

                planets[this.indexFrost].hasTrojans = true;

                planets[this.indexFrost-2].orbit.aSigma = planets[this.indexFrost-2].r / 4.0;
            }
            Utils.updateProgress();

            //Calculate dist std dev for kuiper belts
            if (planets[kuipIndex].type == ID.Belt.BELT_KUIPER)
            {
                Utils.writeLog("      Calculating standard deviation of a for Kuiper belt");

                planets[kuipIndex].r = Utils.resonance(planets[kuipIndex-1].orbit.a, 3.0/2.0) - Utils.resonance(planets[kuipIndex-1].orbit.a, 2.0/1.0);
                planets[kuipIndex].orbit.aSigma = planets[kuipIndex].r / 2.0;
            }
            Utils.updateProgress();

            //Now that we have the std devs, regenerate the belt object orbits
            Utils.writeLog("      Regenerating normally-distributed orbits");
            for (int i = 0; i < planets.Count; i++)
            {
                if (needsSigma[i])
                {
                    if (i < kuipIndex)
                        planets[i].orbit.genOrbit(this, planets[i], planets[i-1]);
                    else
                        planets[i].orbit.genOrbit(this, planets[i], planets[kuipIndex]);
                }
            }
            Utils.updateProgress();

            //Calculate apparent magnitude of the star
            do
            {
                this.magRel = this.magAbs + (5.0 * (Math.Log10(planets[this.indexHab].orbit.a) - 6.31442));
                Utils.writeLog("      Calculating apparent magnitude of star (" + this.magRel + ")");
            }
            while ((-40.0 > this.magRel) || (this.magRel > 10.0));
            Utils.updateProgress();
            
            //Generate the flavor text for everything
            this.genFlavorText();
            foreach (Planet p in planets)
            {
                p.genFlavorText(planets);

                if (p.moons != null)
                {
                    foreach (Moon m in p.moons)
                    {
                        m.genFlavorText(planets, p);
                    }
                }
            }
            Utils.updateProgress();

            return planets;
        }
    }
}

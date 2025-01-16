using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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
        public double lumin;   //Luminosity             (Solar lumens)
        public double temp;    //Black-body temperature (K)
        public double bv;      //B-V color index        (dimensionless)
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

        public Star()
        {
            Utils.writeLog(Environment.NewLine + "    Generating star");
            this.genProperties();
            this.genOrbits();
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
            
            Utils.writeLog("        Generating planetary zones");
            this.zoneFormMin = this.m *  0.1;
            this.zoneFormMax = this.m * 40.0;
            this.zoneHabMin  = Math.Sqrt(this.lumin) * 0.95;
            this.zoneHabMax  = Math.Sqrt(this.lumin) * 1.35;
            this.zoneFrost   = Math.Sqrt(this.lumin) * 4.85;

            Utils.writeLog("    Star generation complete");
        }
    
        /**
         * This function returns a list of orbital distances in AU based on this star.
         */
        public void genOrbits()
        {
            Utils.writeLog(Environment.NewLine + "    Generating orbital distances");
            List<double> orbits = new List<double>();
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
            for (int i = 0; i < orbits.Count(); i++)
            {
                if (orbits[i] == frost)
                    this.indexFrost = i;
            }

            //Make sure a habitable planet exists
            bool habitable = false;
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
            {
                Utils.writeLog("            Rerandomizing orbits");
                this.genOrbits();
            }
            else
            {
                Utils.writeLog("    Distance generation complete");
                this.orbits = orbits;
            }

        }
    
        /**
         * This function picks the type of planet for each orbital distance.
         */
        public List<char> genTypes()
        {
            Utils.writeLog(Environment.NewLine + "    Picking types of planets");
            List<char> types = new List<char>();
            bool migrated = false, belt_dwarf = false;

            //Zero out the types list
            types.EnsureCapacity(this.orbits.Count*3);
            for (int i = 0; i < types.Capacity; i++)
            {
                types.Add('\0');
            }

            types[this.indexFrost] = ID.Planet.GAS_GIANT;
            
            Utils.writeLog("        Orbit " + this.indexFrost + " (" + this.orbits[this.indexFrost] + " AU): " + Utils.getDescription(types[this.indexFrost]).ToLower() + " (frost orbit)");

            //Decide migration
            if (Utils.flip() <= Gen.System.MIGRATE_MID_CHANCE)
            {
                int mig_index;
                do
                    mig_index = (int)Math.Round(Utils.randDouble(this.indexHab, this.indexFrost));
                while (mig_index == this.indexHab);

                types[mig_index] = ID.Planet.GAS_PUFFY;
                types[this.indexFrost] = ID.Planet.EMPTY;
                migrated = true;
                
                Utils.writeLog("        Frost giant migrated to mid-system: orbit " + mig_index);
            }
            else if (Utils.flip() <= Gen.System.MIGRATE_HOT_CHANCE)
            {
                types[0] = ID.Planet.GAS_HOT;
                types[this.indexFrost] = ID.Planet.EMPTY;
                migrated = true;

                Utils.writeLog("        Frost giant migrated to inner system: orbit 0");
            }

            //Second do the habitable orbit
            Decay oceanDecay = new Decay(Gen.System.OCEAN_DECAY, Gen.System.OCEAN_CHANCE, 0, this.indexFrost, Decay.DecayDir.INCREASING);

            if (Utils.flip() < oceanDecay.getDecayedChance(this.indexHab))
                types[this.indexHab] = ID.Planet.WATER_GREEN;
            else
                types[this.indexHab] = ID.Planet.ROCK_GREEN;

            
            Utils.writeLog("        Orbit " + this.indexHab + " (" + this.orbits[this.indexHab] + " AU): " + Utils.getDescription(types[this.indexHab]).ToLower() + " (hab orbit)");

            //Third do the inner orbits
            for (int i = 0; this.orbits[i] <= this.zoneHabMin; i++)
            {
                if (types[i] == '\0') //Make sure not to overwrite
                {
                    Decay denseDecay = new Decay(Gen.System.DENSE_DECAY, Gen.System.DENSE_CHANCE, 0, this.indexHab, Decay.DecayDir.DECREASING);

                    if (Utils.flip() < denseDecay.getDecayedChance(i))
                        types[i] = ID.Planet.ROCK_DENSE;
                    else
                        types[i] = ID.Planet.ROCK_DESERT;
                }
                
                Utils.writeLog("        Orbit " + i + " (" + this.orbits[i] + " AU): " + Utils.getDescription(types[i]).ToLower() + " (inner orbit)");
            }

            //Fourth do the mid orbits
            for (int i = this.indexHab+1; i < this.indexFrost; i++)
            {
                if (types[i] == '\0') //Make sure not to overwrite
                {
                    //If i is immediately before the frost giant, and it has not migrated, determine whether to add an asteroid belt
                    if (Utils.flip() < Gen.Belt.INNER_BELT_CHANCE && !migrated)
                    {
                        types[i] = ID.Belt.BELT_INNER;

                        //Also decide whether to add a dwarf planet to this belt
                        if (Utils.flip() < Gen.Belt.INNER_DWARF_CHANCE)
                        {
                            types.Insert(++i, ID.Belt.DWARF);
                            this.indexFrost++;
                            belt_dwarf = true;
                        }
                    }

                    //Otherwise create a planet
                    if (Utils.flip() < oceanDecay.getDecayedChance(i))
                        types[i] = ID.Planet.WATER_OCEAN;
                    else
                        types[i] = ID.Planet.ROCK_DESERT;
                
                    Utils.writeLog("        Orbit " + i + " (" + this.orbits[i] + " AU): " + Utils.getDescription(types[i]).ToLower() + " (mid orbit)");
                }
                else
                {
                    Utils.writeLog("        Orbit " + i + " (" + this.orbits[i] + " AU) is already a " + Utils.getDescription(types[i]).ToLower() + " (mid orbit)");
                }
            }

            //Fifth do the outer orbits
            int num = (
                belt_dwarf
                ? this.orbits.Count+1
                : this.orbits.Count
            );

            bool giantsFinished = false; //Once the first non-gas-giant planet generates, gas giants will stop generating entirely

            for (int i = this.indexFrost; i < num; i++)
            {
                if (types[i] == '\0')
                {
                    Decay iceDecay   = new Decay(Gen.System.ICE_GIANT_DECAY, Gen.System.ICE_GIANT_CHANCE, this.indexFrost, num, Decay.DecayDir.INCREASING);
                    Decay dwarfDecay = new Decay(Gen.System.ICE_DWARF_DECAY, Gen.System.ICE_DWARF_CHANCE, this.indexFrost, num, Decay.DecayDir.INCREASING);

                    double ice_chance   = iceDecay.getDecayedChance(i);
                    double dwarf_chance = dwarfDecay.getDecayedChance(i);
                    double roll;

                    while (true)
                    {
                        roll = Utils.flip();

                        if (roll < ice_chance)
                        {
                            types[i] = ID.Planet.ICE_GIANT;
                            giantsFinished = true;
                            break;
                        }
                        else if (roll > dwarf_chance)
                        {
                            types[i] = ID.Planet.ICE_DWARF;
                            giantsFinished = true;
                            break;
                        }
                        else if (!giantsFinished || Gen.System.DISABLE_GAS_GIANT_CUTOFF)
                        {
                            types[i] = ID.Planet.GAS_GIANT;
                            break;
                        }
                    }
                
                    Utils.writeLog("        Orbit " + i + " (" + this.orbits[i-(num-orbits.Count)] + " AU): " + Utils.getDescription(types[i-(num-orbits.Count)]).ToLower() + " (outer orbit)");
                }
            }

            //Finally decide whether to generate a kuiper belt
            if (Utils.flip() < Gen.Belt.KUIPER_BELT_CHANCE)
            {
                types[num] = ID.Belt.BELT_KUIPER;
                
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
                        types[num+1+i] = ID.Belt.PLUTINO;
                    else if (i < plutinos+cubewanos)
                        types[num+1+i] = ID.Belt.CUBEWANO;
                    else if (i < plutinos+cubewanos+twotinos)
                        types[num+1+i] = ID.Belt.TWOTINO;
                    else if (i < plutinos+cubewanos+twotinos+scattered)
                        types[num+1+i] = ID.Belt.SCATTERED;
                    else
                        types[num+1+i] = ID.Belt.SEDNOID;

                    Utils.writeLog("        Orbit " + (int)(num+1+i) + ": " + Utils.getDescription(types[num+1+i]).ToLower() + " (kuiper orbit)");
                }
            }

            Utils.writeLog("    Type generation complete");

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
                    continue;

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
                        break;
                    case ID.Planet.GAS_PUFFY:
                    case ID.Planet.GAS_HOT:
                        controller = null;
                        dist       = this.orbits[i];
                        mig        = 1;
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
                        controller = null;
                        dist       = this.orbits[i-mig];
                        maxMass    = 0.0;
                        break;
                }

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

                //Creating atmosphere
                p.atmo.genAtmo(this, ref p);
                
                //Creating satellites
                p.moons = Moon.genSatList(this, ref p);

                //Picking feature
                p.GenFeature();

                planets.Add(p);
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
                planets[this.indexFrost-2].r = planets[this.indexFrost-1].rings[7] - planets[this.indexFrost-1].rings[0];

                planets[this.indexFrost].hasTrojans = true;

                planets[this.indexFrost-2].orbit.aSigma = planets[this.indexFrost-2].r / 4.0;
            }

            //Calculate dist std dev for kuiper belts
            if (planets[kuipIndex].type == ID.Belt.BELT_KUIPER)
            {
                Utils.writeLog("      Calculating standard deviation of a for Kuiper belt");

                planets[kuipIndex].r = Utils.resonance(planets[kuipIndex-1].orbit.a, 3.0/2.0) - Utils.resonance(planets[kuipIndex-1].orbit.a, 2.0/1.0);
                planets[kuipIndex].orbit.aSigma = planets[kuipIndex].r / 2.0;
            }

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

            //Calculate apparent magnitude of the star
            do
            {
                this.magRel = this.magAbs + (5.0 * (Math.Log10(planets[this.indexHab].orbit.a) - 6.31442));
                Utils.writeLog("      Calculating apparent magnitude of star (" + this.magRel + ")");
            }
            while ((-40.0 > this.magRel) || (this.magRel > 10.0));
                
            return planets;
        }
    }
}

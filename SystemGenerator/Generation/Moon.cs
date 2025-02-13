using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace SystemGenerator.Generation
{
    public class Moon
    {
        public char   type   ; 
        public char   subtype; 
        public double m      ; //Mass            (M🜨)
        public double rA     ; //Radii           (km)
        public double rB     ;
        public double rC     ;
        public double escV   ; //Escape velocity (m/s)
        public double g      ; //Gravity         (Gs)

        public int num;

        public double bulkMetal  ;
        public double bulkRock   ;
        public double bulkIces   ;
        public double bulkDensity;

        public Orbit  orbit;
        public double day  ;
        public double tilt ;

        public string flavortext;

        public bool isMajor;
        public bool isIcy;

        public Moon(Star star, Planet host, bool major, bool icy, double dist)
        {
            this.genProperties(host, major, icy);
            this.orbit = new Orbit();
            this.orbit.genOrbit(star, this, host, dist);
            this.spin(host);
            this.flavortext = "";
        }
        
        public Moon(Star star, Planet host, double massMax, bool major, bool icy, double dist)
        {
            this.genProperties(host, massMax, major, icy);
            this.orbit = new Orbit();
            this.orbit.genOrbit(star, this, host, dist);
            this.spin(host);
            this.flavortext = "";
        }
        
        public Moon(Planet host, double massMax, bool major, bool icy)
        {
            this.genProperties(host, massMax, major, icy);
            this.orbit = new Orbit();
            this.spin(host);
            this.flavortext = "";
        }
        
        public Moon(Planet host, bool major, bool icy)
        {
            this.genProperties(host, major, icy);
            this.orbit = new Orbit();
            this.spin(host);
            this.flavortext = "";
        }

        public static List<Moon> genSatList(Star star, ref Planet host)
        {
            Utils.writeLog("        Generating satellite system");
            List<Moon> moons = new List<Moon>();

            int index = star.orbits.IndexOf(host.orbit.a);

            if (host.isHab) //Habitable planets get one rocky major moon
            {
                Utils.writeLog("            Generating rocky major moon for habitable planet");
                moons.Add(new Moon(star, host, true, false, 0.0));
            }
            else if (host.isGiant) //Giants have complex satellite systems
            {
                //Hot giants have no moons
                if (host.subtype == "4" || host.subtype == "5" || host.type == ID.Planet.GAS_PUFFY || host.type == ID.Planet.GAS_PUFFY)
                    return moons;

                Utils.writeLog("            Generating moon system for giant");

                //Declare variables
                List<double> rings = new List<double>();
                List<double[]> gaps = new List<double[]>();
                double ringsMin, ringsMax, limMin, limMax, a;
                int i;
                
                Decay rockyDecay = new Decay(Gen.Sat.GIANT_ROCKY_DECAY, Gen.Sat.GIANT_ROCKY_CHANCE, star.indexFrost, star.orbits.Count, Decay.DecayDir.DECREASING);

                ringsMin = Utils.fudge(1.34)*host.r;
                ringsMax = Utils.fudge(2.44)*host.r;

                //Generate the A-type moons
                limMin = Utils.fudge((ringsMin + ringsMax)/2.0);
                limMax = ringsMax + Utils.fudge(1.0);

                a = limMin;

                for (i = 0; a < limMax; i++)
                {
                    //Create
                    Utils.writeLog("                Generating moon " + i + " as type-A minor moon (a = " + a + ")");
                    moons.Add(new Moon(star, host, false, !(Utils.flip() < rockyDecay.getDecayedChance(index)), a));
                    moons[i].type = ID.Sat.MOONA;

                    //Add a ring gap if applicable
                    if (a < ringsMax)
                    {
                        double length = (Utils.fudge(Math.Max(moons[i].rA, Math.Max(moons[i].rB, moons[i].rC)))/Const.Earth.RADIUS);
                        gaps.Add(new double[]{ a - length, a + length });
                        Utils.writeLog("                    Added ring gap from " + gaps[i][0] + " to " + gaps[i][1]);
                    }
                    
                    a += Utils.randDouble(0.25, 1.5);
                }

                //Generate the B-type moons
                limMin = Utils.fudge(3.0)*host.r;
                limMax = Utils.fudge(15.0)*host.r;

                a = limMin;

                for (; a < limMax; i++)
                {
                    //Create
                    Utils.writeLog("                Generating moon " + i + " as type-B major moon (a = " + a + ")");
                    moons.Add(new Moon(star, host, host.m*0.0001, true, !(Utils.flip() < rockyDecay.getDecayedChance(index)), a));
                    moons[i].type = ID.Sat.MOONB;

                    //Add ring gap if applicable
                    double resonance = Utils.resonance(a, 0.5);

                    if (ringsMin < resonance && resonance < ringsMax)
                    {
                        double length = Utils.fudge(0.25);
                        double[] gap = { resonance - length, resonance + length };

                        //Check for overlap
                        bool overlap = false;
                        int j;
                        Utils.writeLog("                    Checking for resonance gap overlap (" + gaps.Count + " existing gaps)");
                        for (j = 0; j < gaps.Count; j++)
                        {
                            if (
                                (gaps[j][0] >= gap[0] && gaps[j][1] <= gap[1]) || //Existing gap is entirely within this gap
                                (gap[0] >= gaps[j][0] && gap[1] <= gaps[j][1]) || //This gap is entirely within existing gap
                                (gaps[j][0] <  gap[0] && gaps[j][1] >= gap[0] && gaps[j][1] < gap[1]) || //Existing gap overlaps on inner edge
                                (gaps[j][0] >  gap[0] && gaps[j][0] <  gap[1] && gaps[j][1] > gap[1])    //Existing gap overlaps on outer edge
                            )
                            {
                                Utils.writeLog("                        Gap from moon " + j + " overlaps");
                                overlap = true;
                                break;
                            }

                            Utils.writeLog("                        Gap from moon " + j + " does not overlap");
                        }

                        if (!overlap)
                        {
                            //If no overlap then simply add the gap
                            gaps.Add(gap);
                            Utils.writeLog("                    Added ring gap from " + gap[0] + " to " + gap[1]);
                        }
                        else
                        {
                            //Otherwise overwrite overlapped gap and move minor moon into resonance
                            gaps[j] = gap;

                            moons[j].orbit.a = resonance;
                            Utils.writeLog("                    Overwrote ring gap from moon " + j + " with " + gap[0] + " to " + gap[1]);
                        }
                    }

                    //Generate lagrangian companions
                    if (Utils.flip() < Gen.Sat.LAGRANGIAN_CHANCE)
                    {
                        //Forward companion
                        i++;
                        Utils.writeLog("                Generating moon " + i + " as forward lagrangian companion to moon " + (i-1));
                        moons.Add(new Moon(star, host, moons[i-1].m, false, !(Utils.flip() < rockyDecay.getDecayedChance(index)), a));
                        moons[i].type  = ID.Sat.FOR_B;
                        moons[i].orbit = moons[i-1].orbit;
                    }

                    if (Utils.flip() < Gen.Sat.LAGRANGIAN_CHANCE)
                    {
                        //Reverse companion
                        i++;
                        Utils.writeLog("                Generating moon " + i + " as reverse lagrangian companion to moon " + (i-1));
                        moons.Add(new Moon(star, host, moons[i-1].m, false, !(Utils.flip() < rockyDecay.getDecayedChance(index)), a));
                        moons[i].type  = ID.Sat.REV_B;
                        moons[i].orbit = moons[i-1].orbit;
                    }

                    a += Utils.randDouble(host.r, host.r*Gen.Sat.MAX_MOON_SPACING);
                }

                //Generate the C-type moon clusters
                limMin = limMax;
                limMax = host.orbit.a * Math.Pow(host.m/star.m, (1.0/3.0)) * 235.0;

                Decay ctypeDecay = new Decay(Gen.Sat.CTYPE_DECAY, Gen.Sat.CTYPE_CHANCE, Gen.Sat.MIN_CTYPE_CLUSTERS, Gen.Sat.MAX_CTYPE_CLUSTERS, Gen.Sat.CTYPE_DIR);

                //Make sure the outer limit doesn't extend into other planet orbits
                if (Math.Abs(star.orbits[index]-star.orbits[index-1]) < limMax)
                    limMax = Math.Abs(star.orbits[index]-star.orbits[index-1]);
                if (star.orbits.Count > index+1)
                    if (Math.Abs(star.orbits[index]-star.orbits[index+1]) < limMax)
                        limMax = Math.Abs(star.orbits[index]-star.orbits[index+1]);

                for (int j = 0; Utils.flip() < ctypeDecay.getDecayedChance(j); j++)
                {
                    //Create
                    moons.Add(new Moon(host, false, !(Utils.flip() < rockyDecay.getDecayedChance(index))));
                    moons[i].type = ID.Sat.MOONC;
                    Utils.writeLog("                Generating moon " + i + " as type-C minor moon group (a = " + moons[i].orbit.a + ")");

                    List<double> aSamp = new List<double>();
                    double[] distr;

                    //Set limits
                    double eMin = Utils.randDouble(0.0, 0.5);
                    double eMax = Utils.randDouble(0.5, 1.0);
                    double iMax = Gen.SAMPLE_SIZE*10.0;

                    //Populate
                    for (int k = 0; k < Gen.SAMPLE_SIZE; k++)
                        aSamp.Add(Utils.randDouble(limMin+(Math.Abs(limMax-limMin)*(k/3.0)),limMin+(Math.Abs(limMax-limMin)*((k+1)/3.0))));

                    distr = Utils.getDistribution(aSamp.ToArray());
                    moons[i].orbit.a      = distr[0];
                    moons[i].orbit.aSigma = distr[1];

                    moons[i].orbit.y = Math.Sqrt(Math.Pow(moons[i].orbit.a, 3.0) / star.m);
                    moons[i].orbit.v = Math.Sqrt(star.m / moons[i].orbit.a) * Const.Earth.ORBV;

                    distr = Utils.getDistribution(eMin, eMax);
                    moons[i].orbit.e      = distr[0];
                    moons[i].orbit.eSigma = distr[1];

                    distr = Utils.getDistribution(iMax-10.0, iMax);
                    moons[i].orbit.i      = distr[0];
                    moons[i].orbit.iSigma = distr[1];

                    distr = Utils.getDistribution(0.0, 360.0);
                    moons[i].orbit.l      = distr[0];
                    moons[i].orbit.lSigma = distr[1];

                    distr = Utils.getDistribution(0.0, 360.0);
                    moons[i].orbit.p      = distr[0];
                    moons[i].orbit.pSigma = distr[1];

                    distr = Utils.getDistribution(Gen.Sat.MIN_DAY_LENGTH, Gen.Sat.MAX_DAY_LENGTH);
                    moons[i].day = distr[0];
                    
                    


                    i++;

                    Utils.writeLog("                    Generated distribution of orbital parameters");
                }

                //Flatten the ring gaps list
                gaps.Add(new double[]{ ringsMin, ringsMax });

                for (int j = 0; j < gaps.Count(); j++)
                {
                    rings.Add(gaps[j][0]);
                    rings.Add(gaps[j][1]);
                }

                for (int j = 0; j < rings.Count-1; j++)
                {
                    for (int k = 0; k < rings.Count-j-1; k++)
                    {
                        if (rings[k] > rings[k+1])
                        {
                            double temp = rings[k+1];
                            rings[k+1] = rings[k];
                            rings[k] = temp;
                        }
                    }
                }

                host.rings = rings;
                host.ringsMin = ringsMin;
                host.ringsMax = ringsMax;
            }
            else if (host.isDwarf) //Dwarfs have middling satellite systems
            {
                Utils.writeLog("            Generating moon system for dwarf");

                Decay rockyDecay = new Decay(Gen.Sat.DWARF_ROCKY_DECAY, Gen.Sat.DWARF_ROCKY_CHANCE, 0, star.indexFrost, Decay.DecayDir.DECREASING);

                double numMoons = Utils.randDouble(Gen.Sat.MIN_DWARF_MOONS, Gen.Sat.MAX_DWARF_MOONS);

                double hillMax = host.orbit.a * Math.Pow(host.m / star.m, (1.0/3.0)) * 235.0;
                double hillMin, dist = 0;

                int mjr = 0;

                //Decide major moon
                if (Utils.flip() < Gen.Sat.MAJOR_DWARF_MOON_CHANCE)
                {
                    Moon moon = new Moon(host, true, true);
                    hillMin = 2.44 * host.r * Math.Pow( host.bulkDensity / moon.bulkDensity, (1.0/3.0) );
                    dist = Utils.fudge(hillMin + (hillMin*Gen.FUDGE_FACTOR));
                    Utils.writeLog("                Generating moon 0 as major moon (a = " + dist + ")");
                    moon.orbit.genOrbit(star, moon, host, dist);
                    moons.Add(moon);

                    mjr++;

                    dist = Utils.randDouble(
                        moons[0].orbit.a + (host.r*10.0), 
                        hillMax*(1.0/((double)numMoons+mjr))
                    );
                }

                //Decide the minor moons
                for (int i = mjr; i < numMoons; i++)
                {
                    Moon moon = new Moon(host, true, true);

                    if (mjr == 0 && i == mjr)
                    {
                        hillMin = 2.44 * host.r * Math.Pow( host.bulkDensity / moon.bulkDensity, (1.0/3.0) );
                        dist = Utils.fudge(hillMin + (hillMin*Gen.FUDGE_FACTOR));
                    }
                    
                    Utils.writeLog("                Generating moon " + i + " as minor moon (a = " + dist + ")");
                    moons.Add(new Moon(host, false, Utils.flip() < rockyDecay.getDecayedChance(index)));
                    moons[i].orbit.genOrbit(star, moons[i], host, dist);

                    dist = Utils.randDouble(
                        moons[i].orbit.a + (host.r*10.0), 
                        hillMax*((double)i/((double)numMoons+mjr))
                    );
                }
            }
            else if (host.isBelt) //Belts have no satellites
            {
                return moons;
            }
            else //Desert planets get a few minor moons
            {
                Utils.writeLog("            Generating moons for desert/water planet");

                Decay moonsDecay = new Decay(Gen.Sat.TERRES_MOONS_DECAY, Gen.Sat.TERRES_MOONS_CHANCE, 0, star.indexFrost, Decay.DecayDir.INCREASING);

                if (Utils.flip() <= moonsDecay.getDecayedChance(index))
                {
                    int numMoons = (int)Utils.randExpo(Gen.Sat.MIN_TERRES_MOONS, Gen.Sat.MAX_TERRES_MOONS, 1.0/moonsDecay.getDecayedChance(index));
                    Decay icyDecay = new Decay(Gen.Sat.TERRES_ICY_DECAY, Gen.Sat.TERRES_ICY_CHANCE, 0, star.indexFrost, Decay.DecayDir.INCREASING);

                    for (int i = 0; i < numMoons; i++)
                    {
                        moons.Add(new Moon(star, host, false, Utils.flip() <= icyDecay.getDecayedChance(index), 0.0));
                        Utils.writeLog("                Generated moon " + i + " as minor moon (a = " + moons[i].orbit.a + ")");
                    }
                }
            }

            
            Utils.writeLog("        Satellite system generation complete");

            return moons;
        }

        private void genProperties(Planet host, bool major, bool icy)
        {
            this.genProperties(host, host.m, major, icy);
        }

        private void genProperties(Planet host, double mass_max, bool major, bool icy)
        {
            this.isMajor = major;
            this.isIcy   = icy;
            do
            {
                //Decide radius
                if (major)
                {
                    this.type = ID.Sat.MAJOR;

                    if (mass_max > 0)
                        this.rA = Utils.randDouble(Gen.Sat.MIN_RADIUS, mass_max*Const.MOON_RAD_MULT);
                    else
                        this.rA = Utils.randDouble(Gen.Sat.MIN_RADIUS, host.r*Const.Earth.RADIUS);

                    this.rB = this.rC = this.rA;
                }
                else
                {
                    this.type = ID.Sat.MINOR;
                
                    this.rA = Utils.randDouble(0.0, Gen.Sat.MIN_RADIUS);
                    this.rB = Utils.randDouble(0.0, Gen.Sat.MIN_RADIUS);
                    this.rC = Utils.randDouble(0.0, Gen.Sat.MIN_RADIUS);
                }

                //Decide composition
                if (icy)
                {
                    this.bulkIces  = Utils.randDouble(Gen.Sat.MIN_COMP_DROPOFF, Gen.Sat.MAX_COMP_DROPOFF);
                    this.bulkRock  = (1.0-this.bulkIces) * Utils.randDouble(Gen.Sat.MIN_COMP_DROPOFF, Gen.Sat.MAX_COMP_DROPOFF);
                    this.bulkMetal =  1.0 - this.bulkIces - this.bulkRock;
                }
                else
                {
                    this.bulkRock  = Utils.randDouble(Gen.Sat.MIN_COMP_DROPOFF, Gen.Sat.MAX_COMP_DROPOFF);
                    this.bulkMetal = (1.0-this.bulkRock) * Utils.randDouble(Gen.Sat.MIN_COMP_DROPOFF, Gen.Sat.MAX_COMP_DROPOFF);
                    this.bulkIces  =  1.0 - this.bulkMetal - this.bulkRock;
                }

                //Calculate properties
                double avgR = ((this.rA + this.rB + this.rC)/3.0)/Const.Earth.RADIUS;

                this.bulkDensity = this.bulkMetal*Const.METAL_DENS + this.bulkRock*Const.ROCK_DENS + this.bulkIces*Const.WATER_DENS;
                this.m           = this.rA * this.rB * this.rC * this.bulkDensity * Const.Earth.DENSITY * Math.Pow((1.0 / Const.Earth.RADIUS), 3.0);
                this.g           = this.m / Math.Pow(avgR, 2.0);
                this.escV        = Math.Sqrt( (2.0 * Const.GRAV_CONST * Const.Earth.MASS * this.m) / (Const.Earth.RADIUS * avgR)) * Const.Earth.ESCV;
            }
            while (this.m > mass_max || this.m < 0);
        }

        private void spin(Planet host)
        {
            //Decide tilt
            this.tilt = Utils.randDouble(Gen.Sat.MIN_TILT, Gen.Sat.MAX_TILT);

            //Decide day length
            if (this.orbit.a < Utils.resonance(host.orbit.a, 1.0/2.0))
            {
                //Close moons are tidally locked
                this.day = this.orbit.y;
            }
            else
            {
                //Farther moons can spin freely

                //Each number of digits should have the same probability for day length
                int digits_min = (int)Math.Floor(Math.Log10(Gen.Sat.MIN_DAY_LENGTH)) + 1;
                int digits_max = (int)Math.Floor(Math.Log10(Gen.Sat.MAX_DAY_LENGTH)) + 1;

                int roll = Utils.roll(digits_max-digits_min);

                double min = Math.Min(digits_min, Math.Pow(10.0, roll+1));
                double max = Math.Min(digits_max, Math.Pow(10.0, roll+2));

                this.day = Utils.randDouble(min, max);
            }

            //Retrograde spin is caused by impacts, so only slow spins can be retrograde
            if (this.day >= Gen.Sat.MIN_RETRO_DAY_LENGTH)
                this.tilt += (
                    Utils.flip() < Gen.Sat.RETRO_DAY_CHANCE
                    ? 180.0
                    : 0.0
                );
        }
    
        public void genFlavorText(List<Planet> planets, Planet host)
        {
            string flavor = "";

            switch (this.type)
            {
                case ID.Sat.MINOR:
                case ID.Sat.MOONA:
                case ID.Sat.FOR_B:
                case ID.Sat.REV_B:
                    if (this.isIcy)
                        flavor += "This moon is an icy minor moon. ";
                    else
                        flavor += "This moon is a rocky minor moon. ";
                    break;
                case ID.Sat.MAJOR:
                case ID.Sat.MOONB:
                    if (this.isIcy)
                        flavor += "This moon is an icy minor moon. ";
                    else
                        flavor += "This moon is a rocky minor moon. ";
                    break;
                case ID.Sat.MOONC:
                    if (this.isIcy)
                        flavor += "These moons are a group of distant captured icy asteroids. ";
                    else
                        flavor += "These moons are a group of distant captured rocky asteroids. ";
                    break;
            }

            if (this.type != ID.Sat.MOONC)
                flavor += "Its surface is visibly cratered. ";
            else
                flavor += "Their surfaces are visibly cratered. ";

            //Determine resonance
            double resonance, precision = 5.0;
            int indexResonant = -1;
            for (int i = 0; i < host.moons.Count; i++)
            {
                resonance = Utils.resonance(host.moons[i].orbit.a, 1.0/2.0);
                if ((resonance * (1.0-(Gen.FUDGE_FACTOR/precision)) <= this.orbit.a) && (resonance * (1.0+(Gen.FUDGE_FACTOR/precision)) <= this.orbit.a))
                {
                    //indexResonant is a major moon causing the gap
                    indexResonant = i;
                    break;
                }
                
                resonance = Utils.resonance(host.moons[i].orbit.a, 2.0/1.0);
                if ((resonance * (1.0-(Gen.FUDGE_FACTOR/precision)) <= this.orbit.a) && (resonance * (1.0+(Gen.FUDGE_FACTOR/precision)) <= this.orbit.a))
                {
                    //indexResonant is a minor moon caught in the gap
                    indexResonant = i;
                    break;
                }

            }

            switch (this.type)
            {
                case ID.Sat.MOONA:
                    if (this.orbit.a < host.ringsMax)
                    {
                        if (indexResonant > 0)
                            flavor += "It orbits within a resonant gap in the rings caused by the " + (indexResonant+1) + Utils.getOrdinal(indexResonant+1) + " moon.";
                        else
                            flavor += "Its orbit is within its host's ring system, clearing gaps in the rings as it ejects material from its path.";
                    }
                    else
                    {
                        if (indexResonant > 0)
                            flavor += "It orbits just outside its host's ring system, caught in a 1:2 mean motion resonance with the " + (indexResonant+1) + Utils.getOrdinal(indexResonant+1) + " moon.";
                        else
                            flavor += "It orbits just outside its host's ring system.";
                    }
                    break;
                case ID.Sat.FOR_B:
                    flavor += "It is an L₄ (forward) lagrangian companion to the previous major moon.";
                    break;
                case ID.Sat.REV_B:
                    flavor += "It is an L₅ (reverse) lagrangian companion to the previous major moon.";
                    break;
                case ID.Sat.MOONC:
                    flavor += "There is nothing particularly interesting about them.";
                    break;
                default:
                    if (host.ringsMin < Utils.resonance(this.orbit.a, 2.0/1.0) && Utils.resonance(this.orbit.a, 2.0/1.0) < host.ringsMax)
                    {
                        if (indexResonant > 0)
                            flavor += "The " + (indexResonant+1) + Utils.getOrdinal(indexResonant+1) + " has a 1:2 mean motion resonance with it, causing a substantial gap in the rings.";
                        else
                            flavor += "There is a major gap in the rings at its 1:2 mean motion resonance.";
                    }
                    else
                    {
                        flavor += "There is nothing particularly interesting about it.";
                    }
                    break;
            }

            this.flavortext = flavor;
        }
    }
}

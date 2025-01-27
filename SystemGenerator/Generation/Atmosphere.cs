using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SystemGenerator.Generation
{
    public class Atmosphere
    {
        public char typeMajor;
        public char typeMinor;
        
        public double pressure; //Equatorial sea level atmospheric pressure (atms)
        public double density ; //Average atmospheric molar weight (kg/mol)
        public double height  ; //Atmospheric scale height (m)

        public string colorName     ;
        public double color         ;
        public string colorCloudName;
        public double colorCloud    ;

        public List<Component> comps;

        public Atmosphere()
        {
            this.colorName      = "";
            this.colorCloudName = "";
            this.comps          = new List<Component>();
        }

        public void genAtmo(Star star, ref Planet planet)
        {
            Utils.writeLog("        Generating atmosphere");
            planet.hasAir = true;

            //Dwarfs and mercurians don't get atmospheres
            if (planet.isDwarf || planet.isBelt || planet.type == ID.Planet.ROCK_DENSE)
            {
                Utils.writeLog("            Planet has no atmosphere");
                planet.hasAir = false;
            }

            //Get a basic approximation of surface temp, this will be iteratively refined later
            planet.t = (Math.Pow(star.lumin, 0.25) / Math.Pow(planet.orbit.a, 0.5)) * Const.Earth.TEMP;

            Utils.writeLog("            Estimated temperature: " + planet.t);

            if (planet.hasAir)
            {
                //Pick chemical composition
                Utils.writeLog("            Picking a major class for the atmosphere");
                genMajorType(ref planet);

                //Pick color and albedo
                Utils.writeLog("            Generating minor class for the atmosphere");
                bool allowed;
                do
                {
                    genMinorType(ref planet);

                    Utils.writeLog("                Calculating albedo");
                    genAlbedo(star, ref planet);

                    //Make sure the temp is still within the range for the minor class
                    allowed = 
                          0.0 <= planet.t && planet.t <   90.0 && this.typeMinor == ID.Atmo.MNR_CRYOAZURIAN ||
                          5.0 <= planet.t && planet.t <   20.0 && this.typeMinor == ID.Atmo.MNR_FRIGIDIAN   ||
                         15.0 <= planet.t && planet.t <   35.0 && this.typeMinor == ID.Atmo.MNR_NEONEAN     ||
                         35.0 <= planet.t && planet.t <   60.0 && this.typeMinor == ID.Atmo.MNR_BOREAN      ||
                         60.0 <= planet.t && planet.t <   90.0 && this.typeMinor == ID.Atmo.MNR_METHANEAN   ||
                         90.0 <= planet.t && planet.t <  550.0 && this.typeMinor == ID.Atmo.MNR_MESOAZURIAN ||
                         60.0 <= planet.t && planet.t <  550.0 && this.typeMinor == ID.Atmo.MNR_THOLIAN     ||
                         80.0 <= planet.t && planet.t <  180.0 && this.typeMinor == ID.Atmo.MNR_SULFANIAN   ||
                         80.0 <= planet.t && planet.t <  190.0 && this.typeMinor == ID.Atmo.MNR_AMMONIAN    ||
                        170.0 <= planet.t && planet.t <  350.0 && this.typeMinor == ID.Atmo.MNR_HYDRONIAN   ||
                        250.0 <= planet.t && planet.t <  500.0 && this.typeMinor == ID.Atmo.MNR_ACIDIAN     ||
                        550.0 <= planet.t && planet.t < 1300.0 && this.typeMinor == ID.Atmo.MNR_PYROAZURIAN ||
                        400.0 <= planet.t && planet.t < 1000.0 && this.typeMinor == ID.Atmo.MNR_SULFOLIAN   ||
                        550.0 <= planet.t && planet.t < 1000.0 && this.typeMinor == ID.Atmo.MNR_AITHALIAN    ;

                    if (!allowed)
                        Utils.writeLog("                    Planet is outside temp range for minor class, rerandomizing");
                }
                while (!allowed);

                Utils.writeLog("            Minor class generation complete");

                //Calculate molar weight and remaining characteristics
                genMolarWeight(planet);
            }
            else
            {
                Utils.writeLog("                Calculating albedo");
                genAlbedo(star, ref planet);
            }
            
            Utils.writeLog("        Atmospheric generation complete");
        }

        public void genMajorType(ref Planet planet)
        {
            //Determine what types of atmospheres are allowed
            //Atmosphere types come from the Extended World Classification System from Orion's Arm
            //https://www.orionsarm.com/eg-article/5e724eb65b934

            bool[] allowed = {
                (Utils.canRetain(Comps.HYDROGEN.m  , planet) || planet.isGiant) && !planet.isHab                   , //Jotunnian
                 Utils.canRetain(Comps.HELIUM.m    , planet)                    && !planet.isHab && !planet.isGiant, //Helian
                 Utils.canRetain(Comps.AMMONIA.m   , planet)                    && !planet.isHab && !planet.isGiant, //Ydatrian
                (Utils.canRetain(Comps.NITROGEN.m  , planet) || planet.isHab  )                  && !planet.isGiant, //Rhean
                 Utils.canRetain(Comps.C_MONOXIDE.m, planet)                    && !planet.isHab && !planet.isGiant, //Minervan
                 Utils.canRetain(Comps.NEON.m      , planet)                    && !planet.isHab && !planet.isGiant  //Edelian
            };
            
            Utils.writeLog("                Jotunnian allowed: " + allowed[0]);
            Utils.writeLog("                Helian    allowed: " + allowed[1]);
            Utils.writeLog("                Ydatrian  allowed: " + allowed[2]);
            Utils.writeLog("                Rhean     allowed: " + allowed[3]);
            Utils.writeLog("                Minervan  allowed: " + allowed[4]);
            Utils.writeLog("                Edelian   allowed: " + allowed[5]);

            int type, die;
            double remain;

            //Generate
            List<Component> comps;
            
            do
            {
                do
                {
                    type = Utils.roll(allowed.Length);
                    Utils.writeLog("                    Picked class " + type + " (allowed: " + allowed[type] + ")");
                }
                while(!allowed[type]);

                comps = new List<Component>();
                remain = 1.0;

                switch (type)
                {
                    case 0: //Jotunnian
                        Utils.writeLog("                Major class: Jotunnian");
                        this.typeMajor = ID.Atmo.MJR_JOTUNNIAN;

                        comps.Add(Comps.HYDROGEN.copy());
                        comps[0].quantity = Utils.randDouble(Gen.Atmo.MIN_MAJORITY_COMP, Gen.Atmo.MAX_MAJORITY_COMP);
                        remain -= comps[0].quantity;
                        Utils.writeLogAtmo(comps[0], remain);

                        comps.Add(Comps.HELIUM.copy());
                        comps[1].quantity = remain*Utils.fudge(Gen.Atmo.ATMO_DROPOFF);
                        remain -= comps[1].quantity;
                        Utils.writeLogAtmo(comps[1], remain);

                        break;
                    case 1: //Helian
                        Utils.writeLog("                Major class: Helian");
                        this.typeMajor = ID.Atmo.MJR_HELIAN;

                        comps.Add(Comps.HELIUM.copy());
                        comps[0].quantity = Utils.randDouble(Gen.Atmo.MIN_MAJORITY_COMP, Gen.Atmo.MAX_MAJORITY_COMP);
                        remain -= comps[0].quantity;
                        Utils.writeLogAtmo(comps[0], remain);

                        die = Utils.roll(11);

                        if (die <= 3)
                            pickHydride(ref comps);
                        else if (die <= 7)
                            pickNonmetal(ref comps);
                        else if (die <= 9)
                            comps.Add(Comps.NITROGEN.copy());
                        else
                            comps.Add(Comps.NEON.copy());
                    
                        comps[1].quantity = remain*Utils.fudge(Gen.Atmo.ATMO_DROPOFF);
                        remain -= comps[1].quantity;
                        Utils.writeLogAtmo(comps[1], remain);

                        break;
                    case 2: //Ydatrian
                        Utils.writeLog("                Major class: Ydatrian");
                        this.typeMajor = ID.Atmo.MJR_YDATRIAN;

                        pickHydride(ref comps);
                        comps[0].quantity = Utils.randDouble(Gen.Atmo.MIN_MAJORITY_COMP, Gen.Atmo.MAX_MAJORITY_COMP);
                        remain -= comps[0].quantity;
                        Utils.writeLogAtmo(comps[0], remain);

                        pickHydride(ref comps);
                        comps[1].quantity = remain*Utils.fudge(Gen.Atmo.ATMO_DROPOFF);
                        remain -= comps[1].quantity;
                        Utils.writeLogAtmo(comps[1], remain);

                        break;
                    case 3: //Rhean
                        Utils.writeLog("                Major class: Rhean");
                        this.typeMajor = ID.Atmo.MJR_RHEAN;

                        if (planet.isHab)
                        {
                            comps.Add(Comps.NITROGEN.copy());
                            comps[0].quantity = Utils.fudge(0.6);
                            remain -= comps[0].quantity;
                            Utils.writeLogAtmo(comps[0], remain);

                            comps.Add(Comps.OXYGEN.copy());
                            comps[1].quantity = Utils.fudge(1.0/3.0);
                            remain -= comps[1].quantity;
                            Utils.writeLogAtmo(comps[1], remain);
                        }
                        else
                        {
                            comps.Add(Comps.NITROGEN.copy());
                            comps[0].quantity = Utils.randDouble(Gen.Atmo.MIN_MAJORITY_COMP, Gen.Atmo.MAX_MAJORITY_COMP);
                            remain -= comps[0].quantity;
                            Utils.writeLogAtmo(comps[0], remain);

                            die = Utils.roll(2);

                            if (die == 1)
                                pickHydride(ref comps);
                            else
                                pickNonmetal(ref comps);
                    
                            comps[1].quantity = remain*Utils.fudge(Gen.Atmo.ATMO_DROPOFF);
                            remain -= comps[1].quantity;
                            Utils.writeLogAtmo(comps[1], remain);
                        }

                        break;
                    case 4: //Minervan
                        Utils.writeLog("                Major class: Minervan");
                        this.typeMajor = ID.Atmo.MJR_MINERVAN;

                        pickNonmetal(ref comps);
                        comps[0].quantity = Utils.randDouble(Gen.Atmo.MIN_MAJORITY_COMP, Gen.Atmo.MAX_MAJORITY_COMP);
                        remain -= comps[0].quantity;
                        Utils.writeLogAtmo(comps[0], remain);

                        pickNonmetal(ref comps);
                        comps[1].quantity = remain*Utils.fudge(Gen.Atmo.ATMO_DROPOFF);
                        remain -= comps[1].quantity;
                        Utils.writeLogAtmo(comps[1], remain);

                        break;
                    default: //Edelian
                        Utils.writeLog("                Major class: Edelian");
                        this.typeMajor = ID.Atmo.MJR_EDELIAN;

                        comps.Add(Comps.NEON.copy());
                        comps[0].quantity = Utils.randDouble(Gen.Atmo.MIN_MAJORITY_COMP, Gen.Atmo.MAX_MAJORITY_COMP);
                        remain -= comps[0].quantity;
                        Utils.writeLogAtmo(comps[0], remain);

                        comps.Add(Comps.ARGON.copy());
                        comps[1].quantity = remain*Utils.fudge(Gen.Atmo.ATMO_DROPOFF);
                        remain -= comps[1].quantity;
                        Utils.writeLogAtmo(comps[1], remain);

                        break;
                }

                if (planet.isGiant)
                {
                    pickHydrocarbon(ref comps);
                    comps[2].quantity = Utils.randDouble(0.005, 0.02);
                    remain -= comps[2].quantity;
                    Utils.writeLogAtmo(comps[2], remain);
                }
                else if (this.typeMajor != ID.Atmo.MJR_EDELIAN)
                {
                    comps.Add(Comps.ARGON.copy());
                    comps[2].quantity = Utils.randDouble(0.005, 0.02);
                    remain -= comps[2].quantity;
                    Utils.writeLogAtmo(comps[2], remain);
                }
                else
                {
                    die = Utils.roll(10);

                    if (die <= 3)
                        pickHydride(ref comps);
                    else if (die <= 7)
                        pickNonmetal(ref comps);
                    else
                        comps.Add(Comps.NITROGEN.copy());
                    comps[2].quantity = Utils.randDouble(0.005, 0.02);
                    remain -= comps[2].quantity;
                    Utils.writeLogAtmo(comps[2], remain);
                }
            
                if (remain < 0)
                    Utils.writeLog("                Remainder is negative, rereandomizing");
            }
            while (remain < 0);

            //Generate minor components (not to be confused with minor class)
            bool fail;
            for (int i = 0; i < 3; i++)
            {
                die = Utils.roll(7);
                fail = false;

                if (planet.isGiant)
                {
                    switch (die)
                    {
                        case 0:
                            fail = pickHydrocarbon(ref comps);
                            break;
                        case 1:
                            fail = pickHydride(ref comps);
                            break;
                        case 2:
                            fail = pickNonmetal(ref comps);
                            break;
                        case 3:
                            if (!Utils.contains(comps, Comps.AMMONIA))
                                comps.Add(Comps.AMMONIA.copy());
                            else
                                fail = true;
                            break;
                        case 4:
                            if (!Utils.contains(comps, Comps.PHOSPHINE))
                                comps.Add(Comps.PHOSPHINE.copy());
                            else
                                fail = true;
                            break;
                        case 5:
                            if (!Utils.contains(comps, Comps.NITROGEN))
                                comps.Add(Comps.NITROGEN.copy());
                            else
                                fail = true;
                            break;
                        default:
                            if (!Utils.contains(comps, Comps.THOLINS))
                            {
                                comps.Add(Comps.THOLINS.copy());
                                comps[i+2].m = Utils.randDouble(0.04, 0.2);
                            }
                            else
                                fail = true;
                            break;
                    }
                }
                else
                {
                    switch (die)
                    {
                        case 0:
                            fail = pickHydrocarbon(ref comps);
                            break;
                        case 1:
                            fail = pickHydride(ref comps);
                            break;
                        case 2:
                        case 3:
                            fail = pickNonmetal(ref comps);
                            break;
                        case 4:
                            if (planet.atmo.typeMajor != ID.Atmo.MJR_EDELIAN)
                                fail = pickNoble(ref comps, planet);
                            else
                                fail = true;
                            break;
                        case 5:
                            if (!Utils.contains(comps, Comps.NITROGEN))
                                comps.Add(Comps.NITROGEN.copy());
                            else
                                fail = true;
                            break;
                        case 6:
                            if (!Utils.contains(comps, Comps.OXYGEN))
                                comps.Add(Comps.OXYGEN.copy());
                            else
                                fail = true;
                            break;
                    }
                }
                
                if (fail)
                {
                    i--;
                    continue;
                }

                double frac = Utils.fudge(Gen.Atmo.ATMO_DROPOFF);
                comps[i+3].quantity = remain*(frac > 0.99 ? 0.99 : frac);
                remain -= comps[i+3].quantity;
                Utils.writeLogAtmo(comps[i+3], remain);
            }

            //Sort comps
            for (int i = 0; i < comps.Count-1; i++)
            {
                for (int j = 0; j < comps.Count-i-1; j++)
                {
                    if (comps[j].quantity < comps[j+1].quantity)
                    {
                        Component temp = comps[j+1];
                        comps[j+1] = comps[j];
                        comps[j] = temp;
                    }
                }
            }

            planet.atmo.comps = comps;

            Utils.writeLog("            Major class generation complete");
        }

        private bool pickHydrocarbon(ref List<Component> comps)
        {
            Component[] c = { 
                Comps.METHANE,
                Comps.METHYLENE,
                Comps.ETHANE,
                Comps.ETHYLENE,
                Comps.ACETYLENE,
                Comps.DIACETYLENE,
                Comps.PROPANE,
                Comps.PROPYNE
            };

            List<Component> list = new List<Component>();

            Utils.writeLog("                    Picking hydrocarbon");

            for (int i = 0; i < c.Length; i++)
            {
                if (!Utils.contains(comps, c[i]))
                {
                    Utils.writeLog(String.Format("                        {0} has not already been picked", c[i].name));
                    list.Add(c[i]);
                }
                else
                    Utils.writeLog(String.Format("                        {0} has already been picked", c[i].name));
            }

            if (list.Count == 0)
            {
                Utils.writeLog("                    Failed to generate hydrocarbon");
                return true;
            }

            comps.Add(list[Utils.roll(list.Count)].copy());
            return false;
        }

        private bool pickHydride(ref List<Component> comps)
        {
            Component[] c = { 
                Comps.WATER,
                Comps.H_SULFIDE,
                Comps.AMMONIA,
                Comps.H_CYANIDE,
                Comps.PHOSPHINE,
                Comps.SILANE,
                Comps.H_FLUORIDE,
                Comps.H_CHLORIDE
            };

            List<Component> list = new List<Component>();

            Utils.writeLog("                    Picking hydride");

            for (int i = 0; i < c.Length; i++)
            {
                if (!Utils.contains(comps, c[i]))
                {
                    Utils.writeLog(String.Format("                        {0} has not already been picked", c[i].name));
                    list.Add(c[i]);
                }
                else
                    Utils.writeLog(String.Format("                        {0} has already been picked", c[i].name));
            }

            if (list.Count == 0)
            {
                Utils.writeLog("                    Failed to generate hydride");
                return true;
            }

            comps.Add(list[Utils.roll(list.Count)].copy());
            return false;
        }

        private bool pickNonmetal(ref List<Component> comps)
        {
            Component[] c = { 
                Comps.N_OXIDE,
                Comps.N_DIOXIDE,
                Comps.S_DIOXIDE,
                Comps.C_DISULFIDE,
                Comps.CARBONYL_S,
                Comps.C_MONOXIDE,
                Comps.C_DIOXIDE,
                Comps.CYANOGEN
            };

            List<Component> list = new List<Component>();

            Utils.writeLog("                    Picking nonmetal");

            for (int i = 0; i < c.Length; i++)
            {
                if (!Utils.contains(comps, c[i]))
                {
                    Utils.writeLog(String.Format("                        {0} has not already been picked", c[i].name));
                    list.Add(c[i]);
                }
                else
                    Utils.writeLog(String.Format("                        {0} has already been picked", c[i].name));
            }

            if (list.Count == 0)
            {
                Utils.writeLog("                    Failed to generate nonmetal");
                return true;
            }

            comps.Add(list[Utils.roll(list.Count)].copy());
            return false;
        }
        
        private bool pickNoble(ref List<Component> comps, Planet planet)
        {            
            Component[] c = { 
                Comps.NEON,
                Comps.ARGON,
                Comps.KRYPTON
            };

            List<Component> list = new List<Component>();

            Utils.writeLog("                    Picking noble gas");

            for (int i = 0; i < c.Length; i++)
            {
                if (!Utils.contains(comps, c[i]))
                {
                    Utils.writeLog(String.Format("                        {0} has not already been picked", c[i].name));
                    list.Add(c[i]);
                }
                else
                    Utils.writeLog(String.Format("                        {0} has already been picked", c[i].name));
            }

            if (list.Count == 0)
            {
                Utils.writeLog("                    Failed to generate noble gas");
                return true;
            }

            int die = Utils.roll(list.Count);

            if (list[die] == Comps.KRYPTON && planet.isGiant)
                pickNoble(ref comps, planet);
            else
                comps.Add(list[die].copy());
            return true;
        }

        private void genMinorType(ref Planet planet)
        {
            List<string> colorNames      = new List<string>();
            List<double> colors          = new List<double>();
            List<string> colorCloudNames = new List<string>();
            List<double> colorsCloud     = new List<double>();

            //Determine which classes are allowed based on temp
            bool[] allowed = { 
                  0.0 <= planet.t && planet.t <   90.0, //Cryoazurian
                  5.0 <= planet.t && planet.t <   20.0, //Frigidian
                 15.0 <= planet.t && planet.t <   35.0, //Neonean
                 35.0 <= planet.t && planet.t <   60.0, //Borean
                 60.0 <= planet.t && planet.t <   90.0, //Methanean
                 90.0 <= planet.t && planet.t <  550.0, //Mesoazurian
                 60.0 <= planet.t && planet.t <  550.0, //Tholian
                 80.0 <= planet.t && planet.t <  180.0, //Sulfanian
                 80.0 <= planet.t && planet.t <  190.0, //Ammonian
                170.0 <= planet.t && planet.t <  350.0, //Hydronian
                250.0 <= planet.t && planet.t <  500.0, //Acidian
                550.0 <= planet.t && planet.t < 1300.0, //Pyroazurian
                400.0 <= planet.t && planet.t < 1000.0, //Sulfolian
                550.0 <= planet.t && planet.t < 1000.0  //Aithalian
            };

            //Pick one
            int minorClass;
            do
            {
                minorClass = Utils.roll(14);
            }
            while (!allowed[minorClass] && minorClass != -1);

            switch (minorClass)
            {
                case 0: //Cryoazurian
                    Utils.writeLog("                Minor class: Cryoazurian");
                    this.typeMinor = ID.Atmo.MNR_CRYOAZURIAN;

                    colorNames.AddRange(new string[] { 
                        "dull blue",
                        "dull cyan",
                        "sand blue",
                        "steel blue",
                        "slate gray",
                        "grayish-blue",
                        "cornflower-blue"
                    });

                    break;

                case 1: //Frigidian
                    Utils.writeLog("                Minor class: Frigidian");
                    this.typeMinor = ID.Atmo.MNR_FRIGIDIAN;

                    colorNames.AddRange(new string[] { 
                        "gray",
                        "grayish-blue",
                        "slate gray",
                        "pewter brown"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "light gray",
                        "white",
                        "washed-out blue"
                    });

                    break;

                case 2: //Neonean
                    Utils.writeLog("                Minor class: Neonean");
                    this.typeMinor = ID.Atmo.MNR_NEONEAN;

                    colorNames.AddRange(new string[] { 
                        "pink",
                        "pale pink",
                        "primrose",
                        "light pink"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "light gray",
                        "white",
                        "washed-out pink"
                    });

                    break;

                case 3: //Borean
                    Utils.writeLog("                Minor class: Borean");
                    this.typeMinor = ID.Atmo.MNR_BOREAN;

                    colorNames.AddRange(new string[] { 
                        "pink",
                        "pale pink",
                        "primrose",
                        "pale purple",
                        "magenta",
                        "peach",
                        "burnt orange"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "gray",
                        "white",
                        "washed-out pink",
                        "very close to the rest of the atmosphere",
                        "washed-out pink",
                        "pale orange",
                        "tan"
                    });

                    break;

                case 4: //Methanean
                    Utils.writeLog("                Minor class: Methanean");
                    this.typeMinor = ID.Atmo.MNR_METHANEAN;

                    colorNames.AddRange(new string[] { 
                        "cyan",
                        "turquoise",
                        "aqua",
                        "teal",
                        "pale blue",
                        "light blue",
                        "blue-green",
                        "dull green"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "white",
                        "pale blue",
                        "pale green"
                    });

                    break;

                case 5: //Mesoazurian
                    Utils.writeLog("                Minor class: Mesoazurian");
                    this.typeMinor = ID.Atmo.MNR_MESOAZURIAN;

                    colorNames.AddRange(new string[] { 
                        "azure",
                        "steel blue",
                        "teal",
                        "smalt",
                        "blue-green",
                        "turquoise",
                        "blue"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "teal",
                        "smalt",
                        "turqoise",
                        "pale blue",
                        "pale green"
                    });

                    break;

                case 6: //Tholian
                    Utils.writeLog("                Minor class: Tholian");
                    this.typeMinor = ID.Atmo.MNR_THOLIAN;

                    colorNames.AddRange(new string[] { 
                        "pale yellow",
                        "pale orange",
                        "yellow",
                        "orange",
                        "peach",
                        "burnt orange",
                        "brown"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "gray",
                        "tan",
                        "pale yellow",
                        "pale brown"
                    });

                    break;

                case 7: //Sulfanian
                    Utils.writeLog("                Minor class: Sulfanian");
                    this.typeMinor = ID.Atmo.MNR_SULFANIAN;

                    colorNames.AddRange(new string[] { 
                        "pale yellow",
                        "pale orange",
                        "yellow",
                        "orange",
                        "dull yellow",
                        "gold",
                        "tan"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "gray",
                        "white",
                        "pale yellow",
                        "pale tan"
                    });

                    break;

                case 8: //Ammonian
                    Utils.writeLog("                Minor class: Ammonian");
                    this.typeMinor = ID.Atmo.MNR_AMMONIAN;

                    colorNames.AddRange(new string[] { 
                        "orange",
                        "pale orange",
                        "peach",
                        "burnt orange",
                        "brown",
                        "dark gray",
                        "dark tan"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "gray",
                        "white",
                        "pale yellow",
                        "pale brown"
                    });

                    break;

                case 9: //Hydronian
                    Utils.writeLog("                Minor class: Hydronian");
                    this.typeMinor = ID.Atmo.MNR_HYDRONIAN;

                    colorNames.AddRange(new string[] { 
                        "white",
                        "light gray",
                        "gray",
                        "pale yellow",
                        "pale orange",
                        "pale green"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "light gray",
                        "white",
                        "gray",
                        "pale yellow"
                    });

                    break;

                case 10: //Acidian
                    Utils.writeLog("                Minor class: Acidian");
                    this.typeMinor = ID.Atmo.MNR_ACIDIAN;

                    colorNames.AddRange(new string[] { 
                        "pale yellow",
                        "dull yellow",
                        "yellow",
                        "peach",
                        "tan",
                        "light brown",
                        "beige"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "light gray",
                        "white",
                        "pale yellow",
                        "pale brown"
                    });

                    break;

                case 11: //Pyroazurian
                    Utils.writeLog("                Minor class: Pyroazurian");
                    this.typeMinor = ID.Atmo.MNR_PYROAZURIAN;

                    colorNames.AddRange(new string[] { 
                        "azure",
                        "blue",
                        "deep blue",
                        "dark blue",
                        "cobalt blue",
                        "smalt"
                    });

                    break;

                case 12: //Sulfolian
                    Utils.writeLog("                Minor class: Sulfolian");
                    this.typeMinor = ID.Atmo.MNR_SULFOLIAN;

                    colorNames.AddRange(new string[] { 
                        "yellow",
                        "yellow-green",
                        "light green",
                        "bronze",
                        "gold",
                        "dull yellow",
                        "dull green"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "tan",
                        "dull green",
                        "pale yellow",
                        "pale green"
                    });

                    break;

                case 13: //Aithalian
                    Utils.writeLog("                Minor class: Aithalian");
                    this.typeMinor = ID.Atmo.MNR_AITHALIAN;

                    colorNames.AddRange(new string[] { 
                        "brown",
                        "dark brown",
                        "dark gray",
                        "hazel",
                        "walnut",
                        "burnt orange",
                        "olive"
                    });

                    colorCloudNames.AddRange(new string[] { 
                        "gray",
                        "light brown",
                        "pale brown"
                    });

                    break;
            }

            //Pick the colors
            this.colorName = colorNames[Utils.roll(colorNames.Count)];

            if (colorCloudNames.Count > 1)
                this.colorCloudName = colorCloudNames[Utils.roll(colorCloudNames.Count)];
            else
                this.colorCloudName = "";

            //If the two colors are the same, try to differentiate them
            if (this.colorName == this.colorCloudName)
            {
                //Check if there's a lighter version of the cloud color
                if (colorLookup(planet, "light " + this.colorCloudName) > 0)
                    this.colorCloudName = "light " + this.colorCloudName;
                else if (colorLookup(planet, "pale " + this.colorCloudName) > 0)
                    this.colorCloudName = "pale " + this.colorCloudName;
                else if (colorLookup(planet, "washed-out " + this.colorCloudName) > 0)
                    this.colorCloudName = "washed-out " + this.colorCloudName;

                //If not, check if there's a darker version of the main color
                else if (colorLookup(planet, "dark " + this.colorName) > 0)
                    this.colorName = "dark " + this.colorName;
                else if (colorLookup(planet, "dull " + this.colorName) > 0)
                    this.colorName = "dull " + this.colorName;
                else if (colorLookup(planet, "deep " + this.colorName) > 0)
                    this.colorName = "deep " + this.colorName;

                //If neither, just keep them equal
                else
                    this.colorCloudName = "very close to the rest of the atmosphere";
            }

            //Set the RGB values
            this.color      = colorLookup(planet, this.colorName     );
            this.colorCloud = colorLookup(planet, this.colorCloudName);
            
            Utils.writeLog(String.Format("                Atmosphere color: {0} (0x{1:X})", this.colorName     , (int)this.color     ));
            Utils.writeLog(String.Format("                Cloud      color: {0} (0x{1:X})", this.colorCloudName, (int)this.colorCloud));
        }

        private double albedoFromRGB(double color)
        {
            double r = (double)((int)(color / (double)0x10000));
            double g = (double)((int)((color - (r*(double)0x10000)) / (double)0x100));
            double b = color - (r*(double)0x10000) - (g*(double)0x100);

            r /= 255.0;
            g /= 255.0;
            b /= 255.0;

            return (Math.Pow(r, 2.2) + Math.Pow(g, 2.2) + Math.Pow(b, 2.2)) / 3.0;
        }

        private double colorLookup(Planet planet, string color)
        {
            if (!planet.hasAir)
                return 0;

            switch (color)
            {
                case "light green"    : return 0x90EE90;
                case "pale green"     : return 0x98FB98;
                case "dull green"     : return 0x67A37F;
                case "yellow-green"   : return 0xD6E865;

                case "yellow"         : return 0xFFFF00;
                case "pale yellow"    : return 0xFFFFAA;
                case "dull yellow"    : return 0xA3A367;
                case "gold"           : return 0xFFD700;
                
                case "orange"         : return 0xFF8C00;
                case "pale orange"    : return 0xFFD381;
                case "peach"          : return 0xFFE5B4;
                case "burnt orange"   : return 0xBF5700;
                case "bronze"         : return 0xCD7F32;

                case "tan"            : return 0xD2B48C;
                case "dark tan"       : return 0x918151;
                case "beige"          : return 0xF5F5DC;

                case "brown"          : return 0x8B4513;
                case "pale brown"     : return 0xB99C80;
                case "light brown"    : return 0xB5651D;
                case "pewter brown"   : return 0x999DA0;
                case "dark brown"     : return 0x654321;
                case "hazel"          : return 0xA7A079;
                case "walnut"         : return 0x59392B;
                case "olive"          : return 0x808000;

                case "white"          : return 0xFAFAFA;
                case "light gray"     : return 0xD3D3D3;
                case "gray"           : return 0x828282;
                case "dark gray"      : return 0x5D5D5D;

                case "pink"           : return 0xFFC1CC;
                case "pale pink"      : return 0xFFEEEE;
                case "light pink"     : return 0xFFD9DC;
                case "washed-out pink": return 0xFFC0CB;
                case "primrose"       : return 0xEED7D9;
                case "magenta"        : return 0xE11584;
                case "pale purple"    : return 0xB660CF;

                case "blue"           : return 0x0000FF;
                case "pale blue"      : return 0x86C5D8;
                case "light blue"     : return 0xAFEEEE;
                case "cornflower-blue": return 0x6495ED;
                case "washed-out blue": return 0xEAF0FF;
                case "azure"          : return 0x007FFF;

                case "cyan"           : return 0x00FFFF;
                case "turquoise"      : return 0x40E0D0;
                case "aqua"           : return 0x00E6E6;

                case "dull blue"      : return 0x677FA3;
                case "dull cyan"      : return 0x749E9F;
                case "sand blue"      : return 0x6074A1;
                case "steel blue"     : return 0x4682B4;

                case "slate gray"     : return 0x737CA1;
                case "grayish-blue"   : return 0x98AFC7;

                case "blue-green"     : return 0x0ABAB5;
                case "teal"           : return 0x008080;

                case "smalt"          : return 0x489090;
                case "deep blue"      : return 0x0F52BA;
                case "dark blue"      : return 0x0000CC;
                case "cobalt blue"    : return 0x0047AB;

                default:
                    if (color != planet.atmo.colorName)
                        colorLookup(planet, planet.atmo.colorName);
                    break;
            }

            return 0;
        }

        private void genAlbedo(Star star, ref Planet planet)
        {
            if (planet.isBelt) //Belts have neither temp nor atmo so don't need albedo
            {
                return;
            }
            else if (planet.isIcy) //Ice giants get basic approximations
            {
                planet.albedo = Utils.fudge(0.3);
                planet.t = 65.0 * Math.Pow((1.0 - planet.albedo) * 340.0 * (star.lumin / Math.Pow(planet.orbit.a, 2.0)), 0.25);
                Utils.writeLog("                Calculated temp: " + planet.t);
            }
            else if (planet.isGiant)
            {
                //Giants have albedo and temp based on Sudarsky classifications
                //https://en.wikipedia.org/wiki/Sudarsky's_gas_giant_classification

                while (true)
                {
                    switch (planet.subtype)
                    {
                        case "1":
                            planet.albedo = Utils.fudge(0.57 * (2.0 / 3.0));
                            break;
                        case "2":
                            planet.albedo = Utils.fudge(0.81);
                            break;
                        case "3":
                            planet.albedo = Utils.fudge(0.12);
                            break;
                        case "4":
                            planet.albedo = Utils.fudge(0.03);
                            break;
                        case "5":
                            planet.albedo = Utils.fudge(0.55);
                            break;
                    }

                    planet.t = 65.0 * Math.Pow((1.0 - planet.albedo) * 340.0 * (star.lumin / Math.Pow(planet.orbit.a, 2.0)), 0.25);
                    planet.t *= Utils.randDouble(1.0, 2.0); //Internal heat
                    Utils.writeLog("                Calculated temp: " + planet.t);

                    if (planet.t < 150.0)
                    {
                        if (planet.subtype.Equals("1"))
                        {
                            Utils.writeLog("                    Confirmed class-1");
                            if (planet.m > (Gen.Planet.Giant.GAS_RADIUS_LIM * (1.0 + Gen.FUDGE_FACTOR)))
                                planet.type = ID.Planet.GAS_SUPER;
                            else
                                planet.type = ID.Planet.GAS_GIANT;
                            break;
                        }
                        else
                        {
                            Utils.writeLog("                    Switched to class-1");
                            planet.subtype = "1";
                        }
                    }
                    else if (150.0 < planet.t && planet.t <= 250)
                    {
                        if (planet.subtype.Equals("2"))
                        {
                            Utils.writeLog("                    Confirmed class-2");
                            if (planet.m > (Gen.Planet.Giant.GAS_RADIUS_LIM * (1.0 + Gen.FUDGE_FACTOR)))
                                planet.type = ID.Planet.GAS_SUPER;
                            else
                                planet.type = ID.Planet.GAS_GIANT;
                            break;
                        }
                        else
                        {
                            Utils.writeLog("                    Switched to class-2");
                            planet.subtype = "2";
                        }
                    }
                    else if (250.0 < planet.t && planet.t <= 850)
                    {
                        if (planet.subtype.Equals("3"))
                        {
                            Utils.writeLog("                    Confirmed class-3");
                            planet.type = ID.Planet.GAS_PUFFY;
                            break;
                        }
                        else
                        {
                            Utils.writeLog("                    Switched to class-3");
                            planet.subtype = "3";
                        }   
                    }
                    else if (850.0 < planet.t && planet.t <= 1400)
                    {
                        if (planet.subtype.Equals("4"))
                        {
                            Utils.writeLog("                    Confirmed class-4");
                            planet.type = ID.Planet.GAS_HOT;
                            break;
                        }
                        else
                        {
                            Utils.writeLog("                    Switched to class-4");
                            planet.subtype = "4";
                        }
                    }
                    else if (1400.0 < planet.t)
                    {
                        if (planet.subtype.Equals("5"))
                        {
                            Utils.writeLog("                    Confirmed class-5");
                            planet.type = ID.Planet.GAS_HOT;
                            break;
                        }
                        else
                        {
                            Utils.writeLog("                    Switched to class-5");
                            planet.subtype = "5";
                        }
                    }

                }
            }
            else
            {
                //Terrestrial planets get in-depth calculations
                Surface surface = new Surface();

                //Generate clouds

                double cloudCover = 0, landCover;
                switch (planet.atmo.typeMinor)
                {
                    case ID.Atmo.MNR_CRYOAZURIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_CRYOAZURIAN_CLOUD_COVER, Gen.Atmo.MAX_CRYOAZURIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_CRYOAZURIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_CRYOAZURIAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_FRIGIDIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_FRIGIDIAN_CLOUD_COVER, Gen.Atmo.MAX_FRIGIDIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_FRIGIDIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_FRIGIDIAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_NEONEAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_NEONEAN_CLOUD_COVER, Gen.Atmo.MAX_NEONEAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_NEONEAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_NEONEAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_BOREAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_BOREAN_CLOUD_COVER, Gen.Atmo.MAX_BOREAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_BOREAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_BOREAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_MESOAZURIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_MESOAZURIAN_CLOUD_COVER, Gen.Atmo.MAX_MESOAZURIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_MESOAZURIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_MESOAZURIAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_THOLIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_THOLIAN_CLOUD_COVER, Gen.Atmo.MAX_THOLIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_THOLIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_THOLIAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_SULFANIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_SULFANIAN_CLOUD_COVER, Gen.Atmo.MAX_SULFANIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_SULFANIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_SULFANIAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_AMMONIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_AMMONIAN_CLOUD_COVER, Gen.Atmo.MAX_AMMONIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_AMMONIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_AMMONIAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_HYDRONIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_HYDRONIAN_CLOUD_COVER, Gen.Atmo.MAX_HYDRONIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_HYDRONIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_HYDRONIAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_ACIDIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_ACIDIAN_CLOUD_COVER, Gen.Atmo.MAX_ACIDIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_ACIDIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_ACIDIAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_PYROAZURIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_PYROAZURIAN_CLOUD_COVER, Gen.Atmo.MAX_PYROAZURIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_PYROAZURIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_PYROAZURIAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_SULFOLIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_SULFOLIAN_CLOUD_COVER, Gen.Atmo.MAX_SULFOLIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_SULFOLIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_SULFOLIAN_THICK_CLOUD_COVER);
                        break;
                    case ID.Atmo.MNR_AITHALIAN:
                        cloudCover = Utils.randDouble(Gen.Atmo.MIN_AITHALIAN_CLOUD_COVER, Gen.Atmo.MAX_AITHALIAN_CLOUD_COVER);
                        surface.coverCloudThick = Utils.randDouble(Gen.Atmo.MIN_AITHALIAN_THICK_CLOUD_COVER, Gen.Atmo.MAX_AITHALIAN_THICK_CLOUD_COVER);
                        break;
                }

                surface.coverCloudThin   = cloudCover * (1.0-surface.coverCloudThick);
                surface.coverCloudThick *= cloudCover;
                
                surface.albedoCloudThin  = (albedoFromRGB(this.colorCloud)/2.0);
                surface.albedoCloudThick =  albedoFromRGB(this.colorCloud);
                
                Utils.writeLog("                    Thin  clouds     cover  " + surface.coverCloudThin *100.0 + "% of the surface, contributing " + surface.coverCloudThin  + " albedo");
                Utils.writeLog("                    Thick clouds     cover  " + surface.coverCloudThick*100.0 + "% of the surface, contributing " + surface.coverCloudThick + " albedo");

                //Generate land

                if (planet.isWater && planet.subtype == "1") //Water planet, mostly rock
                    landCover = Utils.randDouble(Gen.Planet.Terrestrial.MIN_OCEAN1_LAND, Gen.Planet.Terrestrial.MAX_OCEAN1_LAND);
                else if (planet.isWater && planet.subtype == "2") //Water planet, equal parts water and rock
                    landCover = Utils.randDouble(Gen.Planet.Terrestrial.MIN_OCEAN2_LAND, Gen.Planet.Terrestrial.MAX_OCEAN2_LAND);
                else if (planet.isWater && planet.subtype == "3") //Water planet, mostly water
                    landCover = Utils.randDouble(Gen.Planet.Terrestrial.MIN_OCEAN3_LAND, Gen.Planet.Terrestrial.MAX_OCEAN3_LAND);
                else if (planet.isHab)
                    landCover = Utils.randDouble(Gen.Planet.Terrestrial.MIN_HAB_LAND, Gen.Planet.Terrestrial.MAX_HAB_LAND);
                else
                    landCover = 1.0;

                double rockBright = Utils.randDouble(0.0, 1.0);

                surface.coverRockBright = landCover *        rockBright ;
                surface.coverRockDull   = landCover * (1.0 - rockBright);
                
                surface.albedoRockBright = Utils.randDouble(Gen.Planet.Terrestrial.MIN_BRIGHT_ALBEDO, Gen.Planet.Terrestrial.MAX_BRIGHT_ALBEDO);
                surface.albedoRockDull   = Utils.randDouble(Gen.Planet.Terrestrial.MIN_DULL_ALBEDO  , Gen.Planet.Terrestrial.MAX_DULL_ALBEDO  );
                
                Utils.writeLog("                    Reflective land  covers " + surface.coverRockBright*100.0 + "% of the surface, contributing " + surface.albedoRockBright + " albedo");
                Utils.writeLog("                    Absorptive land  covers " + surface.coverRockDull  *100.0 + "% of the surface, contributing " + surface.albedoRockDull   + " albedo");

                //Generate water (if there is any)

                if (planet.isWater || planet.isHab)
                {
                    //If the planet has water, freeze the polar caps
                    if (planet.t > Const.KELVIN)
                        surface.coverIce = 1.0 - Math.Cos( Math.PI / 4.0 ) - ( planet.tilt * ( Math.PI/180.0 ) );
                    else
                        surface.coverIce = 1.0;
                    
                    surface.coverWater = (1.0 - landCover) * (1.0 - surface.coverIce);
                    surface.coverIce   = (1.0 - landCover) *        surface.coverIce ;

                    surface.albedoWater = Utils.randDouble(Gen.Planet.Terrestrial.MIN_WATER_ALBEDO, Gen.Planet.Terrestrial.MAX_WATER_ALBEDO);
                    surface.albedoIce   = Utils.randDouble(Gen.Planet.Terrestrial.MIN_ICE_ALBEDO  , Gen.Planet.Terrestrial.MAX_ICE_ALBEDO  );

                    Utils.writeLog("                    Reflective ice   covers " + surface.coverIce  *100.0 + "% of the surface, contributing " + surface.albedoIce   + " albedo");
                    Utils.writeLog("                    Absorptive water covers " + surface.coverWater*100.0 + "% of the surface, contributing " + surface.albedoWater + " albedo");
                }
                else
                {
                    surface.coverWater  = 0;
                    surface.coverIce    = 0;
                    surface.albedoWater = 0;
                    surface.albedoIce   = 0;

                    Utils.writeLog("                    The planet has no ice or water, contributing 0 albedo");
                }

                //Calculate temperature

                planet.albedo  = surface.getAlbedo();
                planet.surface = surface;

                Utils.writeLog("                    Total albedo: " + planet.albedo);

                planet.t = 65.0*Math.Pow((1.0-planet.albedo)*340.0*(star.lumin/Math.Pow(planet.orbit.a, 2.0)), 0.25);

                if (planet.isHab)
                {
                    planet.t *= Utils.fudge(1.1);             //Greenhouse effect
                    planet.t += Utils.randDouble(35.0, 45.0); //Geothermal heat
                }
                else if (planet.hasAir)
                    planet.t *= Utils.randDouble(1.1, 2.0); //Greenhouse effect

                Utils.writeLog("                Calculated temp: " + planet.t);

                //Sanity checks

                if (planet.isWater && (planet.t < Const.KELVIN) && (planet.surface.coverIce < 0.8)) //If a water world is freezing but has a liquid ocean, redo
                {
                    Utils.writeLog("                Planet has liquid surface and freezing temp, rerandomizing");
                    genAlbedo(star, ref planet);
                }

                if (planet.isWater && (planet.t > Const.KELVIN) && (planet.surface.coverIce >= 0.8)) //If a water world is above freezing but has a frozen ocean, redo
                {
                    Utils.writeLog("                Planet has frozen surface and above-melting temp, rerandomizing");
                    genAlbedo(star, ref planet);
                }

                if (planet.isHab && (planet.t < Const.KELVIN)) //If a habitable planet is freezing, redo
                {
                    Utils.writeLog("                Planet is habitable and has freezing temp, rerandomizing");
                    genAlbedo(star, ref planet);
                }
            }
        }

        private void genMolarWeight(Planet planet)
        {
            Utils.writeLog("            Calculating molar weight");
            this.density = 0;

            foreach (Component comp in comps)
                this.density += comp.quantity*comp.m;

            this.density *= 10.0;

            if (this.density > 0.0)
            {
                this.pressure = Utils.randDouble(Gen.Atmo.MIN_SURFACE_PRESSURE, Gen.Atmo.MAX_SURFACE_PRESSURE);
                this.height   = (planet.t * Const.GAS_CONST) / (planet.g * Const.Earth.GRAVITY * this.density);
                this.density *= (this.pressure * 101325) / (planet.t * Const.GAS_CONST);
            
                if (planet.isGiant || planet.isIcy)
                {
                    this.height = (this.height*10.0)+Utils.fudge(20000.0);
                }
            }
        }

        public class Component
        {
            public string name;
            public double quantity; //(%)
            public double m;        //(kg/mol)
            public int    color;    //(hex)
            
            public Component(string n, double m, int c)
            {
                this.name     = n;
                this.m        = m;
                this.color    = c;
            }
            public Component(string n, double q, double m, int c)
            {
                this.name     = n;
                this.quantity = q;
                this.m        = m;
                this.color    = c;
            }

            public Component copy()
            {
                return new Component(this.name, this.quantity, this.m, this.color);
            }
        }

        public class Surface
        {
            public double coverRockDull   ; //% of surface covered by dull-colored rock
            public double coverRockBright ; //% of surface covered by bright-colored rock
            public double coverIce        ; //% of surface covered by ice
            public double coverWater      ; //% of surface covered by water
            public double coverCloudThin  ; //% of surface covered by thin clouds
            public double coverCloudThick ; //% of surface covered by thick clouds
            
            public double albedoRockDull  ; //Albedo of dull rock
            public double albedoRockBright; //Albedo of bright rock
            public double albedoIce       ; //Albedo of ice
            public double albedoWater     ; //Albedo of water
            public double albedoCloudThin ; //Albedo of thin clouds
            public double albedoCloudThick; //Albedo of thick clouds

            public Surface()
            {
                this.coverRockDull    = 0;
                this.coverRockBright  = 0;
                this.coverIce         = 0;
                this.coverWater       = 0;
                this.coverCloudThin   = 0;
                this.coverCloudThick  = 0;
                this.albedoRockDull   = 0;
                this.albedoRockBright = 0;
                this.albedoIce        = 0;
                this.albedoWater      = 0;
                this.albedoCloudThin  = 0;
                this.albedoCloudThick = 0;
            }

            public double getAlbedo()
            {
                return this.coverCloudThin  * this.albedoCloudThin                                                      +
                       this.coverCloudThick * this.albedoCloudThick                                                     +
                       this.coverRockDull   * this.albedoRockDull  * (1.0 - this.coverCloudThin - this.coverCloudThick) +
                       this.coverRockBright * this.coverRockBright * (1.0 - this.coverCloudThin - this.coverCloudThick) +
                       this.coverIce        * this.albedoIce       * (1.0 - this.coverCloudThin - this.coverCloudThick) +
                       this.coverWater      * this.albedoWater     * (1.0 - this.coverCloudThin - this.coverCloudThick) ;
            }
        }
     }
}

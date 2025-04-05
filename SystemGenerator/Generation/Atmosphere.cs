using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace SystemGenerator.Generation
{
    public class Atmosphere
    {
        public MajorClass classMajor;
        public MinorClass classMinor;
        
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
            this.classMajor     = new MajorClass();
            this.classMinor     = new MinorClass();
            this.colorName      = "";
            this.colorCloudName = "";
            this.comps          = new List<Component>();
        }
        
        public void genAtmo(Star star, ref Planet planet)
        {
            Utils.writeLog("        Generating atmosphere");
            planet.hasAir = true;

            //Dwarfs and belts don't get atmospheres
            if (planet.isDwarf || planet.isBelt)
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

                if (planet.hasAir)
                {
                    //Pick color and albedo
                    Utils.writeLog("            Generating minor class for the atmosphere");
                    bool allowed;
                    do
                    {
                        genMinorType(planet.t);

                        Utils.writeLog("                Calculating albedo");
                        genAlbedo(star, ref planet);

                        //Make sure the temp is still within the range for the minor class
                        allowed = this.classMinor.minTemp <= planet.t && planet.t < this.classMinor.maxTemp;

                        if (!allowed)
                            Utils.writeLog("                    Planet is outside temp range for minor class, rerandomizing");
                    }
                    while (!allowed);

                    Utils.writeLog("            Minor class generation complete");

                    //Calculate molar weight and remaining characteristics
                    genMolarWeight(planet.t, planet.g, planet.isGiant || planet.isIcy);
                }
            }
            
            if (!planet.hasAir)
            {
                Utils.writeLog("                Calculating albedo");
                genAlbedo(star, ref planet);
            }
            
            Utils.writeLog("        Atmospheric generation complete");
        }
        public void genAtmo(Star star, Planet host, ref Moon moon, bool gen)
        {
            Utils.writeLog("        Generating atmosphere");
            moon.hasAir = gen;

            //Get a basic approximation of surface temp, this will be iteratively refined later
            moon.t = (Math.Pow(star.lumin, 0.25) / Math.Pow(moon.orbit.a, 0.5)) * Const.Earth.TEMP;

            Utils.writeLog("            Estimated temperature: " + moon.t);

            if (moon.hasAir)
            {
                //Pick chemical composition
                Utils.writeLog("            Picking a major class for the atmosphere");
                genMajorType(ref moon);

                if (moon.hasAir)
                {
                    //Pick color and albedo
                    Utils.writeLog("            Generating minor class for the atmosphere");
                    bool allowed;
                    do
                    {
                        genMinorType(moon.t);

                        Utils.writeLog("                Calculating albedo");
                        genAlbedo(star, host, ref moon);

                        //Make sure the temp is still within the range for the minor class
                        allowed = this.classMinor.minTemp <= moon.t && moon.t < this.classMinor.maxTemp;

                        if (!allowed)
                            Utils.writeLog("                    Moon is outside temp range for minor class, rerandomizing");
                    }
                    while (!allowed);

                    Utils.writeLog("            Minor class generation complete");

                    //Calculate molar weight and remaining characteristics
                    genMolarWeight(moon.t, moon.g, false);
                }
            }
            
            if (!moon.hasAir)
            {
                Utils.writeLog("                Calculating albedo");
                genAlbedo(star, host, ref moon);
            }
            
            Utils.writeLog("        Atmospheric generation complete");
        }

        public void genMajorType(ref Planet planet)
        {
            //Determine what types of atmospheres are allowed
            //Atmosphere types come from the Extended World Classification System from Orion's Arm
            //https://www.orionsarm.com/eg-article/5e724eb65b934

            bool[] allowed = new bool[Gen.Atmo.MAJOR_CLASSES.Length];

            for (int i = 0; i < allowed.Length; i++)
                allowed[i] = Gen.Atmo.MAJOR_CLASSES[i].isAllowed(planet) && !planet.isHab;

            allowed[0] = Gen.Atmo.MAJOR_CLASSES[1].isAllowed(planet) || planet.isGiant;
            allowed[3] = Gen.Atmo.MAJOR_CLASSES[3].isAllowed(planet) || planet.isHab;
            
            Utils.writeLog("                Jotunnian allowed: " + allowed[0]);
            Utils.writeLog("                Helian    allowed: " + allowed[1]);
            Utils.writeLog("                Ydatrian  allowed: " + allowed[2]);
            Utils.writeLog("                Rhean     allowed: " + allowed[3]);
            Utils.writeLog("                Minervan  allowed: " + allowed[4]);
            Utils.writeLog("                Edelian   allowed: " + allowed[5]);

            if (!(allowed[0] || allowed[1] || allowed[2] || allowed[3] || allowed[4] || allowed[5]))
            {
                Utils.writeLog("            Planet has no atmosphere");
                planet.hasAir = false;
                return;
            }

            int type;

            //Generate
            do
            {
                type = Utils.rollWeighted(allowed.Length, Gen.Atmo.MAJOR_WEIGHTS);
                Utils.writeLog("                    Picked class " + type + " (allowed: " + allowed[type] + ")");
            }
            while(!allowed[type]);

            Utils.writeLog("                Major class: " + Gen.Atmo.MAJOR_CLASSES[type].name);
            this.classMajor = Gen.Atmo.MAJOR_CLASSES[type];

            planet.atmo.comps = this.classMajor.genAtmoComp();

            Utils.writeLog("            Major class generation complete");
        }
        public void genMajorType(ref Moon moon)
        {
            //Determine what types of atmospheres are allowed
            //Atmosphere types come from the Extended World Classification System from Orion's Arm
            //https://www.orionsarm.com/eg-article/5e724eb65b934

            bool[] allowed = new bool[Gen.Atmo.MAJOR_CLASSES.Length];

            for (int i = 0; i < allowed.Length; i++)
                allowed[i] = Gen.Atmo.MAJOR_CLASSES[i].isAllowed(moon);
            
            Utils.writeLog("                Jotunnian allowed: " + allowed[0]);
            Utils.writeLog("                Helian    allowed: " + allowed[1]);
            Utils.writeLog("                Ydatrian  allowed: " + allowed[2]);
            Utils.writeLog("                Rhean     allowed: " + allowed[3]);
            Utils.writeLog("                Minervan  allowed: " + allowed[4]);
            Utils.writeLog("                Edelian   allowed: " + allowed[5]);

            if (!(allowed[0] || allowed[1] || allowed[2] || allowed[3] || allowed[4] || allowed[5]))
            {
                Utils.writeLog("            Planet has no atmosphere");
                moon.hasAir = false;
                return;
            }

            int type;

            //Generate
            do
            {
                type = Utils.rollWeighted(allowed.Length, Gen.Atmo.MAJOR_WEIGHTS);
                Utils.writeLog("                    Picked class " + type + " (allowed: " + allowed[type] + ")");
            }
            while(!allowed[type]);

            Utils.writeLog("                Major class: " + Gen.Atmo.MAJOR_CLASSES[type].name);
            this.classMajor = Gen.Atmo.MAJOR_CLASSES[type];

            moon.atmo.comps = this.classMajor.genAtmoComp();

            Utils.writeLog("            Major class generation complete");
        }

        private void genMinorType(double t)
        {
            List<double> colors          = new List<double>();
            List<double> colorsCloud     = new List<double>();

            //Determine which classes are allowed based on temp
            bool[] allowed = new bool[Gen.Atmo.MINOR_CLASSES.Length];

            for (int i = 0; i < allowed.Length; i++)
                allowed[i] = Gen.Atmo.MINOR_CLASSES[i].minTemp <= t && (t < Gen.Atmo.MINOR_CLASSES[i].maxTemp || Gen.Atmo.MINOR_CLASSES[i].maxTemp < 0);

            Utils.writeLog("            Picking minor class");
            for (int i = 0; i < allowed.Count(); i++)
                Utils.writeLog(String.Format("                    {0,-16} allowed: {1}", Gen.Atmo.MINOR_CLASSES[i].name, allowed[i]));

            //Pick one
            int minorClass;
            do
                minorClass = Utils.rollWeighted(allowed.Length, Gen.Atmo.MINOR_WEIGHTS);
            while (!allowed[minorClass] && minorClass != -1);

            this.classMinor = Gen.Atmo.MINOR_CLASSES[minorClass];

            //Pick the colors
            this.colorName = this.classMinor.colorNames[Utils.roll(this.classMinor.colorNames.Length)];

            if (this.classMinor.cloudColorNames != null)
                this.colorCloudName = this.classMinor.cloudColorNames[Utils.roll(this.classMinor.cloudColorNames.Length)];
            else
                this.colorCloudName = "";

            //If the two colors are the same, try to differentiate them
            if (this.colorName == this.colorCloudName)
            {
                //Check if there's a lighter version of the cloud color
                if (colorLookup("light " + this.colorCloudName) > 0)
                    this.colorCloudName = "light " + this.colorCloudName;
                else if (colorLookup("pale " + this.colorCloudName) > 0)
                    this.colorCloudName = "pale " + this.colorCloudName;
                else if (colorLookup("washed-out " + this.colorCloudName) > 0)
                    this.colorCloudName = "washed-out " + this.colorCloudName;

                //If not, check if there's a darker version of the main color
                else if (colorLookup("dark " + this.colorName) > 0)
                    this.colorName = "dark " + this.colorName;
                else if (colorLookup("dull " + this.colorName) > 0)
                    this.colorName = "dull " + this.colorName;
                else if (colorLookup("deep " + this.colorName) > 0)
                    this.colorName = "deep " + this.colorName;

                //If neither, just keep them equal
                else
                    this.colorCloudName = "barely-distringuishable";
            }

            //Set the RGB values
            this.color      = colorLookup(this.colorName     );
            this.colorCloud = colorLookup(this.colorCloudName);
            
            Utils.writeLog(String.Format("                Atmosphere color: {0} (0x{1:X})", this.colorName     , (int)this.color     ));
            Utils.writeLog(String.Format("                Cloud      color: {0} (0x{1:X})", this.colorCloudName, (int)this.colorCloud));
        }

        private double albedoFromRGB(double color)
        {
            Color c = Utils.UI.colorFromHex((int)color);
            return (Math.Pow(c.R/255.0, 2.2) + Math.Pow(c.G/255.0, 2.2) + Math.Pow(c.B/255.0, 2.2)) / 3.0;
        }

        public static double colorLookup(string color)
        {
            switch (color)
            {
                case "light green"    : return 0x90EE90;
                case "pale green"     : return 0x98FB98; //X
                case "dull green"     : return 0x67A37F;
                case "dark green"     : return 0x013220;
                case "yellow-green"   : return 0xD6E865;

                case "yellow"         : return 0xFFFF00; //X
                case "pale yellow"    : return 0xFFFFAA;
                case "dull yellow"    : return 0xA3A367;
                case "gold"           : return 0xFFD700;
                
                case "orange"         : return 0xFF8C00;
                case "pale orange"    : return 0xFFD381;
                case "peach"          : return 0xFFE5B4;
                case "burnt orange"   : return 0xBF5700;
                case "bronze"         : return 0xCD7F32; //X

                case "tan"            : return 0xD2B48C; //X
                case "pale tan"       : return 0xF8E8CA;
                case "dark tan"       : return 0x918151;
                case "beige"          : return 0xF5F5DC;

                case "brown"          : return 0x8B4513; //X
                case "pale brown"     : return 0xB99C80;
                case "light brown"    : return 0xB5651D;
                case "pewter brown"   : return 0x999DA0;
                case "dark brown"     : return 0x654321;
                case "hazel"          : return 0xA7A079;
                case "walnut"         : return 0x59392B;
                case "olive"          : return 0x808000;
                case "dark olive"     : return 0x556B2F;

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
                    return 0;
            }
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

                if (planet.hasAir)
                {
                    cloudCover = Utils.randDouble(this.classMinor.minThinCloudCover, this.classMinor.maxThinCloudCover);
                    surface.coverCloudThick = Utils.randDouble(this.classMinor.minThickCloudCover, this.classMinor.maxThickCloudCover);
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

                bool frozen = false;
                if (planet.isWater || planet.isHab)
                {
                    //If the planet has water, freeze the polar caps
                    if (planet.t > Const.KELVIN)
                        surface.coverIce = 1.0 - Math.Cos( Math.PI / 4.0 ) - ( planet.tilt * ( Math.PI/180.0 ) );
                    else
                    {
                        surface.coverIce = 1.0;
                        frozen = true;
                    }
                    
                    surface.coverWater = (1.0 - landCover) * (1.0 - surface.coverIce);
                    surface.coverIce   = (1.0 - landCover) *        surface.coverIce ;

                    surface.coverIce = Math.Abs( surface.coverIce );

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

                if (planet.isWater && (planet.t < Const.KELVIN) && !frozen) //If a water world is freezing but has a liquid ocean, redo
                {
                    Utils.writeLog("                Planet has liquid surface and freezing temp, rerandomizing");
                    genAlbedo(star, ref planet);
                }

                if (planet.isWater && (planet.t > Const.KELVIN) && frozen) //If a water world is above freezing but has a frozen ocean, redo
                {
                    Utils.writeLog("                Planet has frozen surface and above-melting temp, rerandomizing");
                    genAlbedo(star, ref planet);
                }

                if (planet.isHab && frozen) //If a habitable planet is freezing, redo
                {
                    Utils.writeLog("                Planet is habitable and has freezing temp, rerandomizing");
                    genAlbedo(star, ref planet);
                }
            }
        }
        private void genAlbedo(Star star, Planet host, ref Moon moon)
        {
            //Moons get the same basic approximations that ice giants do
            moon.albedo = Utils.randDouble(Gen.Sat.MIN_MOON_ALBEDO, Gen.Sat.MAX_MOON_ALBEDO);
            moon.t = 65.0 * Math.Pow((1.0 - moon.albedo) * 340.0 * (star.lumin / Math.Pow(host.orbit.a, 2.0)), 0.25);
            Utils.writeLog("                Calculated temp: " + moon.t);
        }

        private void genMolarWeight(double t, double g, bool isGiant)
        {
            Utils.writeLog("            Calculating molar weight");
            this.density = 0;

            foreach (Component comp in comps)
                this.density += comp.quantity*comp.m;

            this.density *= 10.0;

            if (this.density > 0.0)
            {
                if (isGiant)
                    this.pressure = 1.0;
                else
                {
                    //Each number of digits should have the same probability for surface pressure
                    int digits_min = (int)Math.Floor(Math.Log10(this.classMajor.minPressure)) + 1;
                    int digits_max = (int)Math.Floor(Math.Log10(this.classMajor.maxPressure)) + 1;

                    int roll = Utils.roll(digits_max-digits_min);

                    double min = Math.Min(digits_min, Math.Pow(10.0, roll+1));
                    double max = Math.Min(digits_max, Math.Pow(10.0, roll+2));

                    this.pressure = Utils.randDouble(
                        Math.Max(
                            this.classMajor.minPressure,
                            min
                        ),
                        Math.Max(
                            this.classMajor.maxPressure,
                            max
                        )
                    );
                }

                this.height   = (t * Const.GAS_CONST) / (g * Const.Earth.GRAVITY * this.density);
                this.density *= (this.pressure * 101325) / (t * Const.GAS_CONST);
            
                if (isGiant)
                {
                    this.height = (this.height*10.0)+Utils.fudge(20000.0);
                }
            }
        }

        public class MajorClass
        {
            public char   ID;
            public string name;
            public string flavor;

            public bool   allowedTerres;
            public bool   allowedGiant;
            public bool   allowedMoon;
            public double maxTemp;

            public double minPressure;
            public double maxPressure;

            public Component[] primary;
            public int[]       primaryWeight;
            public Component[] secondary;
            public int[]       secondaryWeight;
            public Component[] tertiary;
            public int[]       tertiaryWeight;
            public Component[] minor;
            public int[]       minorWeight;

            public MajorClass()
            {
                this.name = "";
                this.flavor = "";
                this.primary         = new Component[0];
                this.primaryWeight   = new int[0];
                this.secondary       = new Component[0];
                this.secondaryWeight = new int[0];
                this.tertiary        = new Component[0];
                this.tertiaryWeight  = new int[0];
                this.minor           = new Component[0];
                this.minorWeight     = new int[0];
            }

            public MajorClass(char iD, string name, string fl, bool aT, bool aG, bool aM, double mxT, double mnP, double mxP, Component[] p, int[] pW, Component[] s, int[] sW, Component[] t, int[] tW, Component[] m, int[] mW)
            {
                this.ID              = iD;
                this.name            = name;
                this.flavor          = fl;
                this.allowedTerres   = aT;
                this.allowedGiant    = aG;
                this.allowedMoon     = aM;
                this.maxTemp         = mxT;
                this.minPressure     = mnP;
                this.maxPressure     = mxP;
                this.primary         = p;
                this.primaryWeight   = pW;
                this.secondary       = s;
                this.secondaryWeight = sW;
                this.tertiary        = t;
                this.tertiaryWeight  = tW;
                this.minor           = m;
                this.minorWeight     = mW;
            }
            
            public bool isAllowed(Planet planet)
            {
                //Asteroid belts and dwarf planets have no atmospheres
                if (planet.isBelt || planet.isDwarf)
                    return false;

                //If the planet is a giant, and the atmosphere class is not allowed for giants, return false
                if (!this.allowedGiant && planet.isGiant)
                    return false;
                
                //If the planet is rocky, and the atmosphere class is not allowed for rocky planets, return false
                if (!this.allowedTerres && planet.isRocky)
                    return false;

                //If the planet's temp is greater than the class's max temp, return false
                if (this.maxTemp < planet.t && this.maxTemp > 0.0)
                    return false;

                //Check if the planet's gravity can retain this kind of atmosphere
                Component lightest = this.primary[0];

                for (int i = 0; i < this.primary.Count(); i++)
                {
                    if (this.primary[i].m < lightest.m)
                        lightest = this.primary[i];
                }

                //If the planet can't retain the lightest substance of this atmosphere, return false
                if (!Utils.canRetain(lightest.m, planet))
                    return false;

                //If all checks clear, return true
                return true;
            }
            public bool isAllowed(Moon moon)
            {
                //If the planet is a giant, and the atmosphere class is not allowed for giants, return false
                if (!this.allowedMoon)
                    return false;

                //If the planet's temp is greater than the class's max temp, return false
                if (this.maxTemp < moon.t && this.maxTemp > 0.0)
                    return false;

                //Check if the planet's gravity can retain this kind of atmosphere
                Component lightest = this.primary[0];

                for (int i = 0; i < this.primary.Count(); i++)
                {
                    if (this.primary[i].m < lightest.m)
                        lightest = this.primary[i];
                }

                //If the planet can't retain the lightest substance of this atmosphere, return false
                if (!Utils.canRetain(lightest.m, moon))
                    return false;

                //If all checks clear, return true
                return true;
            }

            public List<Atmosphere.Component> genAtmoComp()
            {
                List<Atmosphere.Component> list = new List<Atmosphere.Component>();
                Atmosphere.Component pick;
                int[] sW = this.secondaryWeight.ToArray();
                int[] tW = this.tertiaryWeight.ToArray();
                int[] mW = this.minorWeight.ToArray();

                double remain = 1.0;

                Utils.writeLog("                    Picking primary component");

                //Add primary component
                list.Add(this.primary[Utils.rollWeighted(this.primary.Length, this.primaryWeight.ToArray())].copy());
                list[0].quantity = Utils.randDouble(Gen.Atmo.MIN_MAJORITY_COMP, Gen.Atmo.MAX_MAJORITY_COMP);
                remain -= list[0].quantity;
                Utils.writeLogAtmo(list[0], remain);

                //Increment the probabilities of related substances
                for (int i = 0; i < list[0].props.Length; i++)
                {
                    if (list[0].props[i])
                    {
                        //Increment same property in secondary
                        for (int j = 0; j < this.secondary.Length; j++)
                        {
                            if (this.secondary[j].props[i])
                                sW[j]++;
                        }

                        //Increment same property in tertiary
                        for (int j = 0; j < this.tertiary.Length; j++)
                        {
                            if (this.tertiary[j].props[i])
                                tW[j]++;
                        }

                        //Increment same property in minor
                        for (int j = 0; j < this.minor.Length; j++)
                        {
                            if (this.minor[j].props[i])
                                mW[j]++;
                        }
                    }
                }

                //Add secondary component
                Utils.writeLog("                    Picking secondary component");
                do
                {
                     pick = this.secondary[Utils.rollWeighted(this.secondary.Length, sW)];
                }
                while (Utils.contains(list, pick));
                list.Add(pick.copy());
                list[1].quantity = remain*Utils.fudge(Gen.Atmo.ATMO_DROPOFF);
                remain -= list[1].quantity;
                Utils.writeLogAtmo(list[1], remain);

                //Increment the probabilities of related substances
                for (int i = 0; i < list[1].props.Length; i++)
                {
                    if (list[1].props[i])
                    {
                        //Increment same property in tertiary
                        for (int j = 0; j < this.tertiary.Length; j++)
                        {
                            if (this.tertiary[j].props[i])
                                tW[j]++;
                        }

                        //Increment same property in minor
                        for (int j = 0; j < this.minor.Length; j++)
                        {
                            if (this.minor[j].props[i])
                                mW[j]++;
                        }
                    }
                }

                //Add tertiary component
                Utils.writeLog("                    Picking tertiary component");
                do
                {
                     pick = this.tertiary[Utils.rollWeighted(this.tertiary.Length, tW)];
                }
                while (Utils.contains(list, pick));
                list.Add(pick.copy());
                list[2].quantity = Utils.fudge(0.01);
                remain -= list[2].quantity;
                Utils.writeLogAtmo(list[2], remain);

                //Increment the probabilities of related substances
                if (this.tertiary.Length > 1)
                {
                    for (int i = 0; i < list[2].props.Length; i++)
                    {
                        if (list[2].props[i])
                        {
                            //Increment same property in minor
                            for (int j = 0; j < this.minor.Length; j++)
                            {
                                if (this.minor[j].props[i])
                                    mW[j]++;
                            }
                        }
                    }
                }

                if (remain > Gen.Atmo.MINOR_FRACTION)
                {
                    list[0].quantity += remain-Gen.Atmo.MINOR_FRACTION;
                    remain -= Gen.Atmo.MINOR_FRACTION;
                }
                
                Utils.writeLog("                    Picking minor components");

                //Add minor components
                for (int m = 0; m < 4; m++)
                {              
                    do
                    {
                         pick = this.minor[Utils.rollWeighted(this.minor.Length, mW)];
                    }
                    while (Utils.contains(list, pick));
                    list.Add(pick.copy());
                    list[m+3].quantity = remain*Utils.fudge(Gen.Atmo.ATMO_DROPOFF);
                    remain -= list[m+3].quantity;
                    Utils.writeLogAtmo(list[m+3], remain);

                    //Increment the probabilities of related substances
                    for (int i = 0; i < list[m+3].props.Length; i++)
                    {
                        if (list[m+3].props[i])
                        {
                            //Increment same property in minor
                            for (int j = 0; j < this.minor.Length; j++)
                            {
                                if (this.minor[j].props[i])
                                    mW[j]++;
                            }
                        }
                    }
                }

                //Sort list
                for (int i = 0; i < list.Count-1; i++)
                {
                    for (int j = 0; j < list.Count-i-1; j++)
                    {
                        if (list[j].quantity < list[j+1].quantity)
                        {
                            Component temp = list[j+1].copy();
                            list[j+1] = list[j];
                            list[j] = temp;
                        }
                    }
                }


                if (remain < 0)
                    return this.genAtmoComp();
                else
                    return list;
            }
        }

        public class MinorClass
        {
            public char   ID;
            public string name;
            public string prefix;

            public double minTemp;
            public double maxTemp;

            public double minThinCloudCover;
            public double maxThinCloudCover;
            public double minThickCloudCover;
            public double maxThickCloudCover;

            public string[] colorNames;
            public string[] cloudColorNames;

            public string flavor;
            
            public MinorClass()
            {
                this.name            = "";
                this.prefix          = "";
                this.colorNames      = new string[0];
                this.cloudColorNames = new string[0];
                this.flavor          = "";
            }

            public MinorClass(char id, string name, string pf, string fl, double mnT, double mxT, double mnTn, double mxTn, double mnTk, double mxTk, string[] cn, string[] ccn)
            {
                this.ID                 = id;
                this.name               = name;
                this.prefix             = pf;
                this.flavor             = fl;
                this.minTemp            = mnT;
                this.maxTemp            = mxT;
                this.minThinCloudCover  = mnTn;
                this.maxThinCloudCover  = mxTn;
                this.minThickCloudCover = mnTk;
                this.maxThickCloudCover = mxTk;
                this.colorNames         = cn;
                this.cloudColorNames    = ccn;
            }
        }

        public class Component
        {
            public string name;
            public double quantity; //(%)
            public double m;        //(kg/mol)
            public int    color;    //(hex)

            public bool[] props; 
            /*
                isNoble
                isHydride
                isNitride
                isOxide
                isHCarbon
                isSulfur
                isExotic
            */
            
            public Component(string n, double m, int c, bool[] p)
            {
                this.name  = n;
                this.m     = m;
                this.color = c;
                this.props = p;
            }

            public Component(string n, double q, double m, int c, bool[] p)
            {
                this.name     = n;
                this.quantity = q;
                this.m        = m;
                this.color    = c;
                this.props    = p;
            }

            public Component copy()
            {
                return new Component(this.name, this.quantity, this.m, this.color, this.props);
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

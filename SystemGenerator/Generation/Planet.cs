using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace SystemGenerator.Generation
{
    public class Planet
    {
        //Characteristics
        public char   type   ;
        public string subtype;

        public double m      ; //Mass            (Earth masses)
        public double r      ; //Radius          (Earth radii)
        public double g      ; //Gravity         (Gs)
        public double escV   ; //Escape velocity (m/s)
        public double albedo ; //                (%)

        //Bulk composition
        public double bulkMetal  ; //Metallicity, typically the metal core
        public double bulkRock   ; //Rocky nonmetals
        public double bulkWater  ; //Water and water ice
        public double bulkIces   ; //Volatile gases like ammonia and methane
        public double bulkNoble  ; //H-²H-He (giants only)
        public double bulkCarbon ; //Carbonaceous asteroids (belts only)
        public double bulkDensity; //(g/m³)

        //Surface conditions
        public double t                  ; //Temperature (K)
        public Atmosphere.Surface surface;
        public string feature            ; //Flavor text
        public string flavortext         ;

        //Atmosphere
        public Atmosphere atmo;

        //Orbit and position
        public Orbit  orbit;
        public double day  ; //(hours)
        public double tilt ; //(degrees)
        public double turn ; //%
        public double rise ; //(degrees)

        //Satellites
        public List<Moon> moons  ;
        public double ringsMin   ;
        public double ringsMax   ;
        public List<double> rings;
        public bool ringsVisible ;
        public bool hasTrojans   ;

        //Flags
        public bool isGiant;
        public bool isRocky;
        public bool isIcy  ;
        public bool isWater;
        public bool isHab  ;
        public bool isBelt ;
        public bool isDwarf;
        public bool hasAir ;

        public Bitmap image;

        public Planet()
        {
            this.orbit      = new Orbit();
            this.atmo       = new Atmosphere();
            this.rings      = new List<double>();
            this.moons      = new List<Moon>();
            this.surface    = new Atmosphere.Surface();
            this.subtype    = "";
            this.feature    = "";
            this.flavortext = "";
        }

        public void genPlanetProperties()
        {
            genPlanetProperties(Gen.Planet.Giant.MAX_GIANT_MASS);
        }

        public void genPlanetProperties(double mass_max)
        {
            Utils.writeLog("        Generating physical characteristics");

            switch (this.type)
            {
                case ID.Planet.ROCK_DENSE:
                    //Set flags
                    this.isGiant = false;
                    this.isRocky = true ;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.m         = Utils.randDouble(Gen.Planet.Terrestrial.MIN_DENSE_MASS, Gen.Planet.Terrestrial.MAX_DENSE_MASS);
                    this.bulkMetal = Utils.randDouble(Gen.Planet.Terrestrial.MIN_CORE_MASS , Gen.Planet.Terrestrial.MAX_CORE_MASS );
                    this.bulkRock  = 1.0-this.bulkMetal;
                    this.bulkWater = 0.0;
                    propsRocky();
                    break;

                case ID.Planet.ROCK_DESERT:
                    //Set flags
                    this.isGiant = false;
                    this.isRocky = true ;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.m         = Utils.randDouble(Gen.Planet.Terrestrial.MIN_ROCKY_MASS, Gen.Planet.Terrestrial.MAX_ROCKY_MASS);
                    this.bulkRock  = Utils.randDouble(Gen.Planet.Terrestrial.MIN_CORE_MASS , Gen.Planet.Terrestrial.MAX_CORE_MASS );
                    this.bulkMetal = 1.0-this.bulkRock;
                    this.bulkWater = 0.0;
                    propsRocky();
                    break;
                    
                case ID.Planet.ROCK_GREEN:
                    //Set flags
                    this.isGiant = false;
                    this.isRocky = true ;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = true;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.m         = Utils.randDouble(Gen.Planet.Terrestrial.MIN_ROCKY_MASS      , Gen.Planet.Terrestrial.MAX_ROCKY_MASS      );
                    this.bulkRock  = Utils.randDouble(Gen.Planet.Terrestrial.MIN_CORE_MASS       , Gen.Planet.Terrestrial.MAX_CORE_MASS       );
                    this.bulkWater = Utils.randDouble(Gen.Planet.Terrestrial.MIN_EARTH_WATER_MASS, Gen.Planet.Terrestrial.MAX_EARTH_WATER_MASS);
                    this.bulkMetal = 1.0-this.bulkRock-this.bulkWater;
                    propsRocky();
                    break;

                case ID.Planet.WATER_OCEAN:
                    //Set flags
                    this.isGiant = false;
                    this.isRocky = true ;
                    this.isIcy   = false;
                    this.isWater = true ;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.m = Utils.randDouble(Gen.Planet.Terrestrial.MIN_ROCKY_MASS, Gen.Planet.Terrestrial.MAX_ROCKY_MASS);
                    
                    //Roll a d3 to determine the type of ocean planet
                    switch (Utils.roll(3))
                    {
                        case 0: //Mostly water
                            this.bulkWater = Utils.randDouble(Gen.Planet.Terrestrial.MIN_OCEAN_WATER_MASS, Gen.Planet.Terrestrial.MAX_OCEAN_WATER_MASS);
                            this.bulkRock  = (1.0-this.bulkWater)*Utils.fudge(Gen.Planet.Terrestrial.MAX_CORE_MASS);
                            this.bulkMetal = 1.0 - this.bulkRock - this.bulkWater;
                            this.subtype = "3";
                            break;

                        case 1: //Equal parts water and rock
                            this.bulkMetal = Utils.randDouble(Gen.Planet.Terrestrial.MIN_OCEAN_CORE_MASS, Gen.Planet.Terrestrial.MAX_OCEAN_CORE_MASS);
                            this.bulkWater = (1.0-this.bulkMetal)* Utils.fudge(0.5);
                            this.bulkRock  = 1.0-this.bulkWater-this.bulkMetal;
                            this.subtype = "2";
                            break;

                        default: //Mostly rock
                            this.bulkRock  = Utils.randDouble(Gen.Planet.Terrestrial.MIN_OCEAN_WATER_MASS, Gen.Planet.Terrestrial.MAX_OCEAN_WATER_MASS);
                            this.bulkWater = (1.0-this.bulkRock)*Utils.fudge(Gen.Planet.Terrestrial.MAX_CORE_MASS);
                            this.bulkMetal = 1.0 - this.bulkRock - this.bulkWater;
                            this.subtype = "1";
                            break;
                    }

                    propsRocky();
                    break;

                case ID.Planet.WATER_GREEN:
                    //Set flags
                    this.isGiant = false;
                    this.isRocky = true ;
                    this.isIcy   = false;
                    this.isWater = true ;
                    this.isHab   = true ;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.m = Utils.randDouble(Gen.Planet.Terrestrial.MIN_ROCKY_MASS, Gen.Planet.Terrestrial.MAX_ROCKY_MASS);

                    //Set composition (habitable water worlds are always mostly rock)
                    this.bulkRock  = Utils.randDouble(Gen.Planet.Terrestrial.MIN_OCEAN_WATER_MASS, Gen.Planet.Terrestrial.MAX_OCEAN_WATER_MASS);
                    this.bulkWater = (1.0-this.bulkRock)*Utils.fudge(Gen.Planet.Terrestrial.MAX_CORE_MASS);
                    this.bulkMetal = 1.0 - this.bulkRock - this.bulkWater;
                    this.subtype = "1";
                    propsRocky();
                    break;

                case ID.Planet.GAS_GIANT:
                    //Set flags
                    this.isGiant = true;
                    this.isRocky = false;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.subtype = "1";
                    this.m = Utils.randDouble(Math.Min(mass_max, Gen.Planet.Giant.MIN_GIANT_MASS), Math.Min(mass_max, Gen.Planet.Giant.MAX_GIANT_MASS));
                    this.r = Gen.Planet.Giant.GAS_RADIUS_NORM * Utils.fudge(1.0);

                    if (this.m >= (Gen.Planet.Giant.GAS_RADIUS_LIM*(1.0+Gen.FUDGE_FACTOR)))
                        this.type = ID.Planet.GAS_SUPER;
                    else
                        this.type = ID.Planet.GAS_GIANT;

                    propsGiant();

                    break;

                case ID.Planet.GAS_PUFFY:
                    //Set flags
                    this.isGiant = true;
                    this.isRocky = false;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.subtype = "3";

                    do
                    {
                        this.m = Utils.randDouble(Math.Min(mass_max, Gen.Planet.Giant.MIN_GIANT_MASS), Math.Min(mass_max, Gen.Planet.Giant.GAS_RADIUS_LIM));
                        this.r = Utils.randDouble(Gen.Planet.Giant.GAS_RADIUS_NORM, Gen.Planet.Giant.GAS_RADIUS_MAX);
                        propsGiant();
                    }
                    while (this.bulkDensity > Gen.Planet.Giant.MAX_PUFFY_DENSITY);

                    break;

                case ID.Planet.GAS_HOT:
                    //Set flags
                    this.isGiant = true;
                    this.isRocky = false;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.subtype = "4";

                    do
                    {
                        this.m = Utils.randDouble(Math.Min(mass_max, Gen.Planet.Giant.MIN_GIANT_MASS), Math.Min(mass_max, Gen.Planet.Giant.GAS_RADIUS_LIM));
                        this.r = Utils.randDouble(Gen.Planet.Giant.GAS_RADIUS_NORM, Gen.Planet.Giant.GAS_RADIUS_MAX);
                        propsGiant();
                    }
                    while (this.bulkDensity > Gen.Planet.Giant.MAX_PUFFY_DENSITY);

                    break;

                case ID.Planet.ICE_DWARF:
                    //Set flags
                    this.isGiant = true;
                    this.isRocky = false;
                    this.isIcy   = true;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.m = Utils.randDouble(Gen.Planet.Giant.MIN_ICE_GIANT_MASS  , Gen.Planet.Giant.MAX_ICE_GIANT_MASS  );
                    this.r = Utils.randDouble(Gen.Planet.Giant.MIN_ICE_GIANT_RADIUS, Gen.Planet.Giant.MAX_ICE_GIANT_RADIUS);
                    propsIcy();
                    break;

                case ID.Planet.ICE_GIANT:
                    //Set flags
                    this.isGiant = true;
                    this.isRocky = false;
                    this.isIcy   = true;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    do
                    {
                        this.m = Utils.randDouble(Gen.Planet.Giant.MIN_ICE_GIANT_MASS  , Gen.Planet.Giant.MAX_ICE_GIANT_MASS  );
                        this.r = Utils.randDouble(Gen.Planet.Giant.MIN_ICE_GIANT_RADIUS, Gen.Planet.Giant.MAX_ICE_GIANT_RADIUS);
                        propsIcy();
                    }
                    while (this.bulkDensity < Gen.Planet.Giant.MIN_ICE_GIANT_DENS || this.bulkDensity > Gen.Planet.Giant.MAX_ICE_GIANT_DENS);
                    break;

                case ID.Belt.BELT_INNER:
                    //Set flags
                    this.isGiant = false;
                    this.isRocky = false;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = true ;
                    this.isDwarf = false;

                    //Set composition
                    this.m = Utils.randDouble(Gen.Belt.MIN_BELT_MASS, Gen.Belt.MAX_BELT_MASS);
                   
                    this.bulkRock   = Utils.randDouble(Gen.Belt.MIN_INNER_ROCK_PERCENT  , Gen.Belt.MAX_INNER_ROCK_PERCENT  );
                    this.bulkCarbon = Utils.randDouble(Gen.Belt.MIN_INNER_CARBON_PERCENT, Gen.Belt.MAX_INNER_CARBON_PERCENT);
                    this.bulkIces   = Utils.randDouble(Gen.Belt.MIN_INNER_ICES_PERCENT  , Gen.Belt.MAX_INNER_ICES_PERCENT  );
                    this.bulkMetal  = 1.0 - this.bulkRock - this.bulkCarbon - this.bulkIces;

                    propsBelt();

                    break;

                case ID.Belt.BELT_KUIPER:
                    //Set flags
                    this.isGiant = false;
                    this.isRocky = false;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = true ;
                    this.isDwarf = false;

                    //Set composition
                    this.m = Utils.randDouble(Gen.Belt.MIN_BELT_MASS, Gen.Belt.MAX_BELT_MASS);
                   
                    this.bulkMetal  = Utils.randDouble(Gen.Belt.MIN_KUIPER_METAL_PERCENT , Gen.Belt.MAX_KUIPER_METAL_PERCENT );
                    this.bulkCarbon = Utils.randDouble(Gen.Belt.MIN_KUIPER_CARBON_PERCENT, Gen.Belt.MAX_KUIPER_CARBON_PERCENT);
                    this.bulkIces   = Utils.randDouble(Gen.Belt.MIN_KUIPER_ICES_PERCENT  , Gen.Belt.MAX_KUIPER_ICES_PERCENT  );
                    this.bulkRock   = 1.0 - this.bulkMetal - this.bulkCarbon - this.bulkIces;

                    propsBelt();

                    break;

                case ID.Belt.DWARF:
                    //Set flags
                    this.isGiant = false;
                    this.isRocky = false;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = true ;

                    //Set composition
                    this.r = Utils.randDouble(Gen.Belt.MIN_DWARF_RADIUS, Math.Min(mass_max*2660.16, Gen.Belt.MAX_DWARF_RADIUS));

                    this.bulkRock  = Utils.randDouble(Gen.Belt.MIN_INNER_DWARF_ROCK_PERCENT, Gen.Belt.MAX_INNER_DWARF_ROCK_PERCENT);
                    this.bulkMetal = (1.0-this.bulkRock)*Utils.randDouble(Gen.Belt.MIN_INNER_DWARF_METAL_PERCENT, Gen.Belt.MAX_INNER_DWARF_METAL_PERCENT);
                    this.bulkIces  = 1.0 - this.bulkMetal - this.bulkRock;

                    propsDwarf();

                    break;

                case ID.Belt.PLUTINO:
                case ID.Belt.CUBEWANO:
                case ID.Belt.TWOTINO:
                case ID.Belt.SCATTERED:
                case ID.Belt.SEDNOID:
                    //Set flags
                    this.isGiant = false;
                    this.isRocky = false;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = true ;

                    //Set composition
                    this.r = Utils.randDouble(Gen.Belt.MIN_DWARF_RADIUS, Math.Min(mass_max*2660.16, Gen.Belt.MAX_DWARF_RADIUS));

                    this.bulkIces  = Utils.randDouble(Gen.Belt.MIN_KUIPER_DWARF_ICES_PERCENT, Gen.Belt.MAX_KUIPER_DWARF_ICES_PERCENT);
                    this.bulkRock  = (1.0-this.bulkIces)*Utils.randDouble(Gen.Belt.MIN_KUIPER_DWARF_ROCK_PERCENT, Gen.Belt.MAX_KUIPER_DWARF_ROCK_PERCENT);
                    this.bulkMetal = 1.0 - this.bulkIces - this.bulkRock;

                    propsDwarf();

                    break;

                default:
                    break;
            }
            Utils.writeLog("            Mass: " + this.m + " (Max: " + mass_max + ")");

            if (this.m < 0)
                genPlanetProperties(mass_max);

            Utils.writeLog("        Physical property generation complete");
        }

        //Calculates the properties of a terrestrial planet given its mass and composition.
        private void propsRocky()
        {
            double massRock    , massMetal    , massWater    , components = 3,
                   radRock  = 0, radMetal  = 0, radWater  = 0;

            //Terrestrial planets have neither volatile ices nor period-1s
            this.bulkIces  = 0;
            this.bulkNoble = 0;

            //Calculate the mass of each bulk material to gage its contribution to the overall radius
            massRock  = (this.m*this.bulkRock )/10.55;
            massMetal = (this.m*this.bulkMetal)/ 5.80;
            massWater = (this.m*this.bulkWater)/ 5.52;

            //Formulae in this section come from https://arxiv.org/pdf/0707.2895
            //Eq. 23 on page 9, table 4 on page 17

            //Metal
            if (this.bulkMetal > 0.0)
                radMetal = 2.52 * Math.Pow(10, -0.209450 + (1.0/3.0)*Math.Log10(massMetal) - 0.0804*Math.Pow(massMetal, 0.394));
            else
                components--;

            //Rock
            if (this.bulkRock > 0.0)
                if (massRock > 4.0)
                    radRock = 3.9 * Math.Pow(10, -0.209594 + (1.0/3.0)*Math.Log10(massRock) - 0.0799*Math.Pow(massRock, 0.413));
                else 
                    radRock = 3.9 * Math.Pow(10, -0.209490 + (1.0/3.0)*Math.Log10(massRock) - 0.0804*Math.Pow(massRock, 0.394));
            else
                components--;

            //Water
            if (this.bulkWater > 0.0)
                if (massWater > 4.0)
                    radWater = 4.43 * Math.Pow(10, -0.209396 + (1.0/3.0)*Math.Log10(massRock) - 0.0807*Math.Pow(massRock, 0.375));
                else 
                    radWater = 3.9 * Math.Pow(10, -0.209490 + (1.0/3.0)*Math.Log10(massRock) - 0.0804*Math.Pow(massRock, 0.394));
            else
                components--;

            //Calculate properties
            this.r = Utils.fudge((
                (1+radMetal)*radMetal +
                (1+radRock )*radRock  +
                (1+radWater)*radWater
            ) / components);

            this.bulkDensity = (
                this.bulkMetal*Const.METAL_DENS +
                this.bulkRock *Const.ROCK_DENS  +
                this.bulkWater*Const.WATER_DENS
            );

            this.g    = this.m / Math.Pow(this.r, 2.0);
            this.escV = Math.Sqrt( this.m/this.r ) * Const.Earth.ESCV;

            spin();
        }
    
        //Calculates the properties and composition of a gaseous planet given its mass.
        private void propsGiant()
        {
            //Calculate composition
            double massSolid = Utils.randDouble(Gen.Planet.Giant.MIN_GIANT_SOLID_MASS, Gen.Planet.Giant.MAX_GIANT_SOLID_MASS);
            double massRocky = Utils.randDouble(Gen.Planet.Giant.MIN_GIANT_ROCKY_MASS, Gen.Planet.Giant.MAX_GIANT_ROCKY_MASS);

            this.bulkRock  = Utils.fudge(Gen.Planet.Giant.GIANT_SILICATE_PERCENT);
            this.bulkMetal = 1.0-this.bulkRock;

            //Roll a d3
            switch (Utils.roll(3))
            {
                case 0: //Remaining mass is mostly volatiles
                    this.bulkIces  = (1.0-massRocky)*Utils.randDouble(Gen.Planet.Giant.MIN_GIANT_ICES_MASS, Gen.Planet.Giant.MAX_GIANT_ICES_MASS);
                    this.bulkWater = (1.0-massRocky)-this.bulkIces;
                    break;

                case 1: //Remaining mass is mostly water
                    this.bulkWater = (1.0-massRocky)*Utils.randDouble(Gen.Planet.Giant.MIN_GIANT_ICES_MASS, Gen.Planet.Giant.MAX_GIANT_ICES_MASS);
                    this.bulkIces  = (1.0-massRocky)-this.bulkWater;
                    break;

                default: //Remaining mass is equal parts water and volatiles
                    this.bulkWater = (1.0-massRocky)*Utils.fudge(0.5);
                    this.bulkIces  = (1.0-massRocky)-this.bulkWater;
                    break;
            }

            //Scale up to whole planet
            this.bulkMetal *= massSolid*massRocky;
            this.bulkRock  *= massSolid*massRocky;
            this.bulkWater *= massSolid;
            this.bulkIces  *= massSolid;
            this.bulkNoble  = 1.0 - this.bulkMetal - this.bulkRock - this.bulkWater - this.bulkIces;

            //Calculate properties
            this.bulkDensity = this.m/Math.Pow(this.r, 3.0)*Const.Earth.DENSITY;
            this.g           = this.m/Math.Pow(this.r, 2.0)      ;
            this.escV        = Math.Sqrt( this.m/this.r ) * Const.Earth.ESCV;

            //Assign albedo
            switch (this.subtype)
            {
                case "1":
                    this.albedo = Utils.fudge(0.57*(2.0/3.0));
                    break;
                    
                case "2":
                    this.albedo = Utils.fudge(0.81);
                    break;

                case "3":
                    this.albedo = Utils.fudge(0.12);
                    break;
                    
                case "4":
                    this.albedo = Utils.fudge(0.03);
                    break;
                    
                case "5":
                    this.albedo = Utils.fudge(0.55);
                    break;
            }

            spin();
        }

        //Calculates the properties and composition of an icy planet given its mass.
        private void propsIcy()
        {
            //Calculate composition
            double massCore = Utils.randDouble(Gen.Planet.Giant.MIN_ICY_CORE_MASS, Gen.Planet.Giant.MAX_ICY_CORE_MASS);

            this.bulkNoble = Utils.randDouble(Gen.Planet.Giant.MIN_ICY_ATMO_MASS, Gen.Planet.Giant.MAX_ICY_ATMO_MASS);

            //The majority of the rocky mass is silicates
            this.bulkRock  = Utils.fudge(Gen.Planet.Giant.GIANT_SILICATE_PERCENT);
            this.bulkMetal = 1.0-this.bulkRock;

            //Rescale
            this.bulkRock  *= massCore;
            this.bulkMetal *= massCore;

            if (this.type == ID.Planet.ICE_DWARF)
                this.bulkRock *= 2.0;

            double remain = 1.0 - this.bulkRock - this.bulkMetal - this.bulkNoble;

            //Roll a d3
            switch (Utils.roll(3))
            {
                case 0: //Remaining mass is mostly volatiles
                    this.bulkIces  = remain*Utils.randDouble(Gen.Planet.Giant.MIN_ICY_ICES_MASS, Gen.Planet.Giant.MAX_ICY_ICES_MASS);
                    this.bulkWater = remain-this.bulkIces;
                    this.subtype = "1";
                    break;

                case 1: //Remaining mass is mostly water
                    this.bulkWater = remain*Utils.randDouble(Gen.Planet.Giant.MIN_ICY_ICES_MASS, Gen.Planet.Giant.MAX_ICY_ICES_MASS);
                    this.bulkIces  = remain-this.bulkWater;
                    this.subtype = "3";
                    break;

                default: //Remaining mass is equal parts water and volatiles
                    this.bulkWater = remain*Utils.fudge(0.5);
                    this.bulkIces  = remain-this.bulkWater;
                    this.subtype = "2";
                    break;
            }

            //Calculate the radius
            double massRock    , massMetal    , massWater    , components = 5,
                   radRock  = 0, radMetal  = 0, radWater  = 0;

            //Calculate the mass of each bulk material to gage its contribution to the overall radius
            massRock  = (this.m*this.bulkRock )/10.55;
            massMetal = (this.m*this.bulkMetal)/ 5.80;
            massWater = (this.m*this.bulkWater)/ 5.52;

            //Formulae in this section come from https://arxiv.org/pdf/0707.2895
            //Eq. 23 on page 9, table 4 on page 17

            //Metal
            if (this.bulkMetal > 0.0)
                radMetal = 2.52 * Math.Pow(10, -0.209450 + (1.0/3.0)*Math.Log10(massMetal) - 0.0804*Math.Pow(massMetal, 0.394));
            else
                components--;

            //Rock
            if (this.bulkRock > 0.0)
                if (massRock > 4.0)
                    radRock = 3.9 * Math.Pow(10, -0.209594 + (1.0/3.0)*Math.Log10(massRock) - 0.0799*Math.Pow(massRock, 0.413));
                else 
                    radRock = 3.9 * Math.Pow(10, -0.209490 + (1.0/3.0)*Math.Log10(massRock) - 0.0804*Math.Pow(massRock, 0.394));
            else
                components--;

            //Water
            if (this.bulkRock > 0.0)
                if (massRock > 4.0)
                    radRock = 4.43 * Math.Pow(10, -0.209396 + (1.0/3.0)*Math.Log10(massRock) - 0.0807*Math.Pow(massRock, 0.375));
                else 
                    radRock = 3.9 * Math.Pow(10, -0.209490 + (1.0/3.0)*Math.Log10(massRock) - 0.0804*Math.Pow(massRock, 0.394));
            else
                components--;

            if (this.bulkNoble == 0)
                components--;
            if (this.bulkIces == 0)
                components--;

            //Calculate properties
            this.r = Utils.fudge((
                (1.0+this.bulkMetal)*radMetal +
                (1.0+this.bulkRock )*radRock  +
                (1.0+this.bulkWater)*radWater +
                (1.0+this.bulkIces )*this.r   +
                (1.0+this.bulkNoble)*this.r
            ) / components);

            this.bulkDensity = (this.m / Math.Pow(this.r, 3.0))*Const.Earth.DENSITY;
            this.g           = this.m / Math.Pow(this.r, 2.0);
            this.escV        = Math.Sqrt( this.m/this.r ) * Const.Earth.ESCV;

            spin();
        }
   
        //Calculates the properties of asteroid belt.
        private void propsBelt()
        {
            this.albedo = (
                this.bulkCarbon * Utils.randDouble(Gen.Belt.MIN_CARBON_ALBEDO, Gen.Belt.MAX_CARBON_ALBEDO) +
                this.bulkRock   * Utils.randDouble(Gen.Belt.MIN_ROCK_ALBEDO  , Gen.Belt.MAX_ROCK_ALBEDO  ) +
                this.bulkIces   * Utils.randDouble(Gen.Belt.MIN_ICES_ALBEDO  , Gen.Belt.MAX_ICES_ALBEDO  ) +
                this.bulkMetal  * Utils.randDouble(Gen.Belt.MIN_METAL_ALBEDO , Gen.Belt.MAX_METAL_ALBEDO )
            );

            this.bulkDensity = (
                this.bulkCarbon * Utils.fudge(1.70) +
                this.bulkRock   * Utils.fudge(3.00) +
                this.bulkIces   * Utils.fudge(0.93) +
                this.bulkMetal  * Utils.randDouble(3.0, 8.0)
            );
        }

        //Calculates the properties a dwarf planet given its mass.
        private void propsDwarf()
        {
            
            this.bulkDensity = (
                this.bulkMetal*Const.METAL_DENS +
                this.bulkRock *Const.ROCK_DENS  +
                this.bulkWater*Const.WATER_DENS
            );

            this.m    = Math.Pow(this.r, 3.0) * (this.bulkDensity*Const.Earth.DENSITY) * Math.Pow((1.0/Const.Earth.RADIUS), 3.0);
            this.g    = (this.m / Math.Pow(this.r/Const.Earth.DENSITY, 2.0));
            this.escV = Math.Sqrt( this.m/this.r ) * Const.Earth.ESCV;

            spin();
        }
    
        //Assigns tilt and day length of terrestrial planets.
        private void spin()
        {
            Utils.writeLog("            Assigning tilt, rise and spin");
            if (this.isGiant)
            {
                //Assign tilt
                this.tilt = Utils.randExpo(Gen.Planet.Giant.MIN_TILT, Gen.Planet.Giant.MAX_TILT, 0.1);
                this.rise = Utils.randExpo(Gen.Planet.Giant.MIN_RISE, Gen.Planet.Giant.MAX_RISE, 0.1);

                //Giant planets have fast spins since they're not rigid
                this.day = Utils.randDouble(Gen.Planet.Giant.MIN_DAY_LENGTH, Gen.Planet.Giant.MAX_DAY_LENGTH);

                //Giants never have retrograde spins
            }
            else
            {
                //Assign tilt
                this.tilt = Utils.randExpo(Gen.Planet.Terrestrial.MIN_TILT, Gen.Planet.Terrestrial.MAX_TILT, 0.01);
                this.rise = Utils.randExpo(Gen.Planet.Terrestrial.MIN_RISE, Gen.Planet.Terrestrial.MAX_RISE, 0.01);

                //Each number of digits should have the same probability for day length
                int digits_min = (int)Math.Floor(Math.Log10(Gen.Planet.Terrestrial.MIN_DAY_LENGTH)) + 1;
                int digits_max = (int)Math.Floor(Math.Log10(Gen.Planet.Terrestrial.MAX_DAY_LENGTH)) + 1;

                int roll = Utils.roll(digits_max-digits_min);

                double min = Math.Min(digits_min, Math.Pow(10.0, roll+1));
                double max = Math.Min(digits_max, Math.Pow(10.0, roll+2));

                this.day = Utils.randDouble(min, max);

                //Retrograde spin is caused by impacts, so only slow spins can be retrograde
                if (this.day >= Gen.Planet.Terrestrial.MIN_RETRO_DAY_LENGTH)
                    this.tilt += (
                        Utils.flip() < Gen.Planet.Terrestrial.RETRO_DAY_CHANCE
                        ? 180.0
                        : 0.0
                    );
            }
        }
    
        public void genFeature()
        {
            List<string> features = new List<string>();

            if ((this.type == ID.Planet.ROCK_DESERT && this.t <= Const.KELVIN+200.0) ||
                 this.type == ID.Planet.ROCK_DENSE                                    )
                features.Add("Its surface is visibly cratered.");

            if ((this.type == ID.Planet.ROCK_DESERT && this.t >= Const.KELVIN+100.0) ||
                (this.type == ID.Planet.ROCK_GREEN  && this.t <= Const.KELVIN+100.0)  )
                features.Add("Tectonic and volcanic activity is evident on its surface.");

            if ((this.type == ID.Planet.ROCK_DESERT && this.t >= Const.KELVIN+100.0) ||
                (this.type == ID.Planet.ROCK_GREEN  && this.t <= Const.KELVIN+100.0)  )
                if (this.albedo < 0.2)
                    features.Add("Its surface is covered in dark igneous rock.");

            if (this.isHab && this.subtype != "3" && this.t > Const.KELVIN)
            {
                int continents = Utils.randInt(Gen.Planet.Terrestrial.MAX_CONTINENTS, Gen.Planet.Terrestrial.MAX_CONTINENTS);
                features.Add(
                    String.Format(
                        "It has {0} continent{1}, covering about {2:N0}% of its surface.",
                        continents,
                        (
                            (continents == 1)
                            ? ""
                            : "s"
                        ),
                        (this.surface.coverRockBright+this.surface.coverRockDull)*100.0
                    )
                );
            }

            if (this.isWater && this.subtype == "3" && this.t > Const.KELVIN)
                features.Add("Its surface is dominated by ocean with occasional islands.");

            if ((this.isWater || this.isHab) && this.t <= Const.KELVIN)
                features.Add("Its surface is covered with ice.");

            if (this.isWater && this.t <= Const.KELVIN)
                features.Add("There are cryovolcanoes on its surface.");

            if (this.type == ID.Planet.GAS_GIANT || this.type == ID.Planet.GAS_SUPER)
            {
                string pole  = (Utils.randSign() > 0) ? "north" : "south";
                string sides = "";

                switch (Utils.randInt(4,9))
                {
                    case 4:
                        sides = "square";
                        break;
                    case 5:
                        sides = "pentagonal";
                        break;
                    case 6:
                        sides = "hexagonal";
                        break;
                    case 7:
                        sides = "septgonal";
                        break;
                    case 8:
                        sides = "octagonal";
                        break;
                    case 9:
                        sides = "nonagonal";
                        break;
                }

                features.Add(
                    String.Format(
                        "There is a persistent {0} storm over its {1} pole.",
                        sides,
                        pole
                    )
                );
            }

            if (this.type == ID.Planet.GAS_GIANT || this.type == ID.Planet.GAS_SUPER || this.type == ID.Planet.ICE_GIANT)
            {
                string pole = (Utils.randSign() > 0) ? "northern" : "southern";
                features.Add(
                    String.Format(
                        "A large, discolored storm rages in its {0} hemisphere.",
                        pole
                    )
                );
            }

            if (features.Count > 0)
                this.feature = features[Utils.randInt(0, features.Count-1)];
        }
    
        public void genFlavorText(Star star, List<Planet> planets)
        {
            string flavor = "";

            if (!this.isBelt)
            {
                flavor += "This planet is " + Utils.getLongDesc(this) + ". ";

                if (planets.IndexOf(this) == star.indexFrost)
                    flavor += "This planet formed at the frost line, making it the largest planet in this system. ";

                if (this.hasAir)
                {
                    flavor += "It has " + this.atmo.classMinor.prefix + "-" + this.atmo.classMajor.name.ToLower() + " atmosphere, largely composed of " + this.atmo.classMajor.flavor + ". ";
                    
                    if (this.atmo.classMinor.cloudColorNames != null)
                        flavor += String.Format(this.atmo.classMinor.flavor, this.atmo.colorName, this.atmo.colorCloudName);
                    else
                        flavor += String.Format(this.atmo.classMinor.flavor, this.atmo.colorName);

                    if (this.atmo.classMinor.ID != ID.Atmo.MNR_CRYOAZURIAN && this.atmo.classMinor.ID != ID.Atmo.MNR_PYROAZURIAN && this.surface.coverCloudThick + this.surface.coverCloudThin > 0.0)
                        flavor += String.Format(" They cover about {0:N0}% of the planet.", (this.surface.coverCloudThick + this.surface.coverCloudThin) * 100.0);
                }
                else
                    flavor += " It has no atmosphere, as it lacks a strong enough magnetosphere.";

                flavor += " " + this.feature;

                if (this.isGiant && this.ringsVisible)
                    flavor += " The planet's rings are icy and clearly visible.";
                else if (this.isGiant && !this.ringsVisible)
                    flavor += " The planet's rings are dusty and faint.";

                if (this.moons.Count > 0)
                {
                    int rockMinor = 0, rockMajor = 0, icyMinor = 0, icyMajor = 0;
                    for (int i = 0; i < this.moons.Count; i++)
                    {
                        if (this.moons[i].isMajor && this.moons[i].isIcy)
                            icyMajor++;
                        else if (this.moons[i].isMajor && !this.moons[i].isIcy)
                            rockMajor++;
                        else if (!this.moons[i].isMajor && this.moons[i].isIcy)
                            icyMinor++;
                        else if (!this.moons[i].isMajor && !this.moons[i].isIcy)
                            rockMinor++;
                    }

                    if (this.moons.Count > 1)
                        flavor += " " + this.moons.Count + " satellites orbit this planet:";
                    else
                        flavor += " " + this.moons.Count + " satellite orbits this planet:";

                    if (rockMajor > 0)
                    {
                        flavor += " " + rockMajor + " rocky major moon" + (rockMajor > 1 ? "s" : "");
                        if (rockMinor != 0 || icyMajor != 0 || icyMinor != 0)
                            flavor += ",";
                    }
                    if (rockMinor > 0)
                    {
                        if (rockMajor != 0 && icyMajor == 0 && icyMinor == 0)
                            flavor += " and";
                        flavor += " " + rockMinor + " rocky minor moon" + (rockMinor > 1 ? "s" : "");
                        if (icyMajor != 0 || icyMinor != 0)
                            flavor += ",";
                    }
                    if (icyMajor > 0)
                    {
                        if ((rockMajor != 0 || rockMinor != 0) && icyMinor == 0)
                            flavor += " and";
                        flavor += " " + icyMajor + " icy major moon" + (icyMajor > 1 ? "s" : "");
                        if (icyMinor != 0)
                            flavor += ",";
                    }
                    if (icyMinor > 0)
                    {
                        if (rockMajor != 0 || rockMinor != 0 || icyMajor != 0)
                            flavor += " and";
                        flavor += " " + icyMinor + " icy minor moon" + (icyMinor > 1 ? "s" : "");
                    }

                    flavor += ".";
                }
                else
                    flavor += " No satellites orbit this planet.";

                if (this.hasTrojans)
                    flavor += " It has also captured swarms of trojans from the asteroid belt.";

                this.flavortext = flavor;
            }
            else
            {
                if (this.type == ID.Belt.BELT_INNER)
                {
                    flavor += "The asteroid belt formed when mean motion resonances with the " + (planets.IndexOf(this)+1) + Utils.getOrdinal(planets.IndexOf(this)+1) + " planet disrupted planet formation in this region.";

                    if (this.subtype == "1")
                        flavor += " It contains one dwarf planet.";
                    else
                        flavor += " It does not contain a dwarf planet.";
                }
                else
                {
                    flavor += "The Kuiper belt formed when gravitational interactions with the giant planets in the early system ejected material out onto wide orbits.";

                    int plutinos = 0, cubewanos = 0, twotinos = 0, scattered = 0, sednoids = 0;
                    for (int i = planets.IndexOf(this)+1; i < planets.Count; i++)
                    {
                        if (planets[i].type == ID.Belt.PLUTINO)
                            plutinos++;
                        else if (planets[i].type == ID.Belt.CUBEWANO)
                            cubewanos++;
                        else if (planets[i].type == ID.Belt.TWOTINO)
                            twotinos++;
                        else if (planets[i].type == ID.Belt.SCATTERED)
                            scattered++;
                        else if (planets[i].type == ID.Belt.SEDNOID)
                            sednoids++;
                    }

                    if (planets.Count - (planets.IndexOf(this)+1) > 1)
                        flavor += " It contains " + (planets.Count - (planets.IndexOf(this)+1)) + " objects of interest:";
                    else
                        flavor += " It contains 1 object of interest:";

                    if (plutinos > 0)
                    {
                        flavor += " " + plutinos + " plutino" + (plutinos > 1 ? "s" : "");
                        if (cubewanos != 0 || twotinos != 0 || scattered != 0 || sednoids != 0)
                            flavor += ",";
                    }
                    if (cubewanos > 0)
                    {
                        if (plutinos != 0 && twotinos == 0 && scattered == 0 && sednoids == 0)
                            flavor += " and";
                        flavor += " " + cubewanos + " cubewano" + (cubewanos > 1 ? "s" : "");
                        if (twotinos != 0 || scattered != 0 || sednoids != 0)
                            flavor += ",";
                    }
                    if (twotinos > 0)
                    {
                        if ((plutinos != 0 || cubewanos != 0) && scattered == 0 && sednoids == 0)
                            flavor += " and";
                        flavor += " " + twotinos + " twotino" + (twotinos > 1 ? "s" : "");
                        if (scattered != 0 || sednoids != 0)
                            flavor += ",";
                    }
                    if (scattered > 0)
                    {
                        if ((plutinos != 0 || cubewanos != 0 || twotinos != 0) && sednoids == 0)
                            flavor += " and";
                        flavor += " " + scattered + " scattered disk object" + (scattered > 1 ? "s" : "");
                        if (sednoids != 0)
                            flavor += ",";
                    }
                    if (sednoids > 0)
                    {
                        if (plutinos != 0 || cubewanos != 0 || twotinos != 0 || sednoids != 0)
                            flavor += " and";
                        flavor += " " + sednoids + " scattered disk object" + (sednoids > 1 ? "s" : "");
                    }

                    flavor += ".";
                }
            }
            
            this.flavortext = flavor;
        }
    
        public void genImage(int x, int y, bool blur, double scale)
        {
            if (this.isBelt)
                return;
            else if (this.isGiant || this.isIcy)
                drawGiant(x, y, blur, scale);
            else
                drawRocky(x, y, blur, scale);
        }

        private void drawGiant(int x, int y, bool blur, double s)
        {            
            Bitmap image   = new Bitmap(x,y);
            Bitmap surface = new Bitmap(x,y);
            Bitmap mask    = new Bitmap(x,y);
            Bitmap smask;
            
            Graphics g  = Graphics.FromImage(image);
            Graphics gs = Graphics.FromImage(surface);
            Graphics gm = Graphics.FromImage(mask);
            Pen p = new Pen(Color.Black);
            PathGradientBrush pgb = null;

            g.Clear(Color.Black);

            GraphicsPath path;
            Point center = new Point(x/2, y/2);
            Rectangle rect;
            Color surf, band;

            int radius = (int)Math.Round(this.r * Const.Earth.RADIUS * s) + UI.BLUR_RADIUS;

            p.Width = 1;

            //For giants the atmo is the planet
            Color h = Utils.UI.colorFromHex((int)this.atmo.color);
            p.Color = Color.FromArgb(h.R * (Color.White.R/255), h.G * (Color.White.R/255), h.B * (Color.White.R/255));
            surf = p.Color;
            
            int lightness;
            if (Atmosphere.colorLookup(this.atmo.colorCloudName) > 0)
            { 
                band = Utils.UI.colorFromHex((int)this.atmo.colorCloud);
                //band = Color.FromArgb(h.R * (Color.White.R/255), h.G * (Color.White.R/255), h.B * (Color.White.R/255));
            }
            else
            {
                band = Utils.UI.colorFromHex((int)this.atmo.color);
                lightness = Utils.randInt(Color.White.R/7, Color.White.R/6) * (int)Utils.randSign();
                band = Color.FromArgb(Math.Max(0, Math.Min(band.R+lightness, 255)), Math.Max(0, Math.Min(band.G+lightness, 255)), Math.Max(0, Math.Min(band.B+lightness, 255)));
            }
            
            rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
            gs.FillEllipse(p.Brush, rect);

            radius -= UI.BLUR_RADIUS;
            rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
            gm.FillEllipse(p.Brush, rect);
            radius += UI.BLUR_RADIUS;
            
            //Draw the bands and jets
            double riseFactor, latFactor, offset;

            p.StartCap = LineCap.Round;
            p.EndCap   = LineCap.Round;

            this.rise       = Utils.randDouble(10.0, 20.0);
            double latitude = Utils.fudge(50.0);
            double minLat   = Utils.fudge(15.0);
            double damping  = 0.7;
            double latN, latS, width = 0, widthNom;
            bool first = true;
            
            for (; latitude > 0; latitude -= width+Utils.randDouble(width, (1.0 / 2.0)*widthNom))
            {

                widthNom = Math.Abs(2.0 * Math.Pow(Math.Cos(latitude * (Math.PI/180)) * (180/Math.PI), damping)) -
                           Math.Abs(1.0 * Math.Pow(Math.Cos(latitude * (Math.PI/180)) * (180/Math.PI), damping));

                if (first)
                {
                    width = Utils.randDouble((1.0/4.0)*widthNom, (1.0/2.0)*widthNom);
                    first = false;
                }
                else
                {
                    width = Utils.randDouble(width, (1.0/2.0)*widthNom);
                }

                //Once we get close to the equator, create the special center band and quit
                if (latitude <= minLat)
                {
                    latitude = Utils.fudge(7.0);
                    width    = latitude;
                }

                //Draw the northern hemisphere band
                path = new GraphicsPath();
                latN = latitude;

                //Add the top arc
                riseFactor = Math.Sin(this.rise * (Math.PI/180.0));
                latFactor  = Math.Cos(latN      * (Math.PI/180.0));
                offset     = Math.Sin(latN      * (Math.PI/180.0));
                rect = new Rectangle(
                    center.X - (int)Math.Round(radius*latFactor),
                    center.Y - (int)Math.Round(radius*latFactor*riseFactor) - (int)Math.Round(radius*offset),
                    (int)Math.Round(radius*2.0*latFactor),
                    (int)Math.Round(radius*2.0*latFactor*riseFactor)
                );
                if (rect.Width  == 0) rect.Width  = 1;
                if (rect.Height == 0) rect.Height = 1;
                if (rect.Width  <  0) rect.Width  = Math.Abs(rect.Width );
                if (rect.Height <  0) rect.Height = Math.Abs(rect.Height);

                path.AddArc(rect, 0, 180);

                //Add the left arc
                rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
                path.AddArc(rect, 180+(int)Math.Round(latN), -(int)Math.Round(width));

                //Add the bottom arc
                latN -= width;
                riseFactor = Math.Sin(this.rise * (Math.PI/180.0));
                latFactor  = Math.Cos(latN      * (Math.PI/180.0));
                offset     = Math.Sin(latN      * (Math.PI/180.0));
                rect = new Rectangle(
                    center.X - (int)Math.Round(radius*latFactor),
                    center.Y - (int)Math.Round(radius*latFactor*riseFactor) - (int)Math.Round(radius*offset),
                    (int)Math.Round(radius*2*latFactor),
                    (int)Math.Round(radius*2*latFactor*riseFactor)
                );
                if (rect.Width  == 0) rect.Width  = 1;
                if (rect.Height == 0) rect.Height = 1;
                if (rect.Width  <  0) rect.Width  = Math.Abs(rect.Width );
                if (rect.Height <  0) rect.Height = Math.Abs(rect.Height);
            
                path.AddArc(rect, 180, -180);

                //Add the right arc
                rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
                path.AddArc(rect, -(int)Math.Round(latN), -(int)Math.Round(width));

                path.CloseFigure();

                //Fill in the band
                pgb = new PathGradientBrush(path);
                pgb.CenterColor = band;
                pgb.SurroundColors = new Color[]{surf};
                pgb.FocusScales = new PointF(1.0f, 1.0f);

                gs.FillPath(pgb, path);

                path.Dispose();
                pgb.Dispose();

                //Draw the southern hemisphere band
                path = new GraphicsPath();
                latS = -latitude;

                //Add the top arc
                riseFactor = Math.Sin(this.rise * (Math.PI/180.0));
                latFactor  = Math.Cos(latS      * (Math.PI/180.0));
                offset     = Math.Sin(latS      * (Math.PI/180.0));
                rect = new Rectangle(
                    center.X - (int)Math.Round(radius*latFactor),
                    center.Y - (int)Math.Round(radius*latFactor*riseFactor) - (int)Math.Round(radius*offset),
                    (int)Math.Round(radius*2.0*latFactor),
                    (int)Math.Round(radius*2.0*latFactor*riseFactor)
                );
                if (rect.Width  == 0) rect.Width  = 1;
                if (rect.Height == 0) rect.Height = 1;
                if (rect.Width  <  0) rect.Width  = Math.Abs(rect.Width );
                if (rect.Height <  0) rect.Height = Math.Abs(rect.Height);

                path.AddArc(rect, 0, 180);

                //Add the left arc
                rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
                path.AddArc(rect, 180+(int)Math.Round(latS), (int)Math.Round(width));

                //Add the bottom arc
                latS += width;
                riseFactor = Math.Sin(this.rise * (Math.PI/180.0));
                latFactor  = Math.Cos(latS      * (Math.PI/180.0));
                offset     = Math.Sin(latS      * (Math.PI/180.0));
                rect = new Rectangle(
                    center.X - (int)Math.Round(radius*latFactor),
                    center.Y - (int)Math.Round(radius*latFactor*riseFactor) - (int)Math.Round(radius*offset),
                    (int)Math.Round(radius*2*latFactor),
                    (int)Math.Round(radius*2*latFactor*riseFactor)
                );
                if (rect.Width  == 0) rect.Width  = 1;
                if (rect.Height == 0) rect.Height = 1;
                if (rect.Width  <  0) rect.Width  = Math.Abs(rect.Width );
                if (rect.Height <  0) rect.Height = Math.Abs(rect.Height);
            
                path.AddArc(rect, 180, -180);

                //Add the right arc
                rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
                path.AddArc(rect, -(int)Math.Round(latS), (int)Math.Round(width));

                path.CloseFigure();

                //Fill in the band
                pgb = new PathGradientBrush(path);
                pgb.CenterColor = band;
                pgb.SurroundColors = new Color[]{surf};
                pgb.FocusScales = new PointF(1.0f, 1.0f);

                gs.FillPath(pgb, path);

                path.Dispose();
                pgb.Dispose();
            }
            
            /*
            for (int i = 0; l < 100; i++)
            {
                mn = (int)Math.Abs(Math.Round(1.0 * Math.Sin(i * (Math.PI/180)) * (180/Math.PI)));
                mx = (int)Math.Abs(Math.Round(2.0 * Math.Sin(i * (Math.PI/180)) * (180/Math.PI)));

                sweep = Utils.randInt(mn, mx);

                if (i%2!=0)
                {
                    h = Color.Red;
                    lightness = Utils.randInt(Color.White.R/7, Color.White.R/6) * (int)Utils.randSign();
                    h = Color.FromArgb(Math.Max(0, Math.Min(h.R+lightness, 255)), Math.Max(0, Math.Min(h.G+lightness, 255)), Math.Max(0, Math.Min(h.B+lightness, 255)));

                    //Draw the upper hemisphere band
                    path = new GraphicsPath();

                    //Draw side arcs then add straight lines by closing figure
                    path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 270+l      , sweep);
                    path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 270-l-sweep, sweep);
                    path.CloseFigure();

                    //Fill in the band
                    pgb = new PathGradientBrush(path);
                    
                    R = Math.Min(Color.White.R, (int)Math.Round((double)h.R * (Color.White.R/255)));
                    G = Math.Min(Color.White.R, (int)Math.Round((double)h.G * (Color.White.R/255)));
                    B = Math.Min(Color.White.R, (int)Math.Round((double)h.B * (Color.White.R/255)));

                    A = Utils.randInt(Color.White.R/7, Color.White.R/3);
                    
                    band = Color.FromArgb(A,R,G,B);
                    pgb.CenterColor = band;
                    pgb.SurroundColors = new Color[]{surf};
                    pgb.FocusScales = new PointF(1.0f, 1.0f);

                    gs.FillPath(pgb, path);

                    path.Dispose();
                    pgb.Dispose();

                    //Draw the lower hemisphere band

                    path = new GraphicsPath();

                    path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 90+l      , sweep);
                    path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 90-l-sweep, sweep);
                    path.CloseFigure();

                    pgb = new PathGradientBrush(path);
                    
                    R = Math.Min(Color.White.R, (int)Math.Round((double)h.R * (Color.White.R/255)));
                    G = Math.Min(Color.White.R, (int)Math.Round((double)h.G * (Color.White.R/255)));
                    B = Math.Min(Color.White.R, (int)Math.Round((double)h.B * (Color.White.R/255)));

                    A = Utils.randInt(Color.White.R/7, Color.White.R/5);
                    
                    band = Color.FromArgb(A,R,G,B);
                    pgb.CenterColor = band;
                    pgb.SurroundColors = new Color[]{surf};
                    pgb.FocusScales = new PointF(1.0f, 1.0f);

                    gs.FillPath(pgb, path);

                    path.Dispose();
                    pgb.Dispose();
                }

                l += sweep;
            }
            */

            //Greeble

            int A, R, G, B, lat, height, lfLim, rtLim, greebles, factor;
            double avgLight = (Atmosphere.albedoFromRGB(Utils.UI.hexFromColor(surf)) + Atmosphere.albedoFromRGB(Utils.UI.hexFromColor(band))) / 2.0;
            smask = surface;

            if (isIcy)
                greebles = 50;
            else
                greebles = 150;

            for (int line = 0; line < greebles; line++)
            {
                p.Width = Utils.randInt(5, 20);

                lat = (int)Utils.randSign()*(int)Math.Round(Utils.randDouble(0.0, 70.0));
                
                //Calculate the bounding box of the arc
                riseFactor = Math.Sin(this.rise * (Math.PI/180.0));
                latFactor  = Math.Cos(lat       * (Math.PI/180.0));
                offset     = Math.Sin(lat       * (Math.PI/180.0));
                rect = new Rectangle(
                    center.X - (int)Math.Round(radius*latFactor),
                    center.Y - (int)Math.Round(radius*latFactor*riseFactor) - (int)Math.Round(radius*offset),
                    (int)Math.Round(radius*2.0*latFactor),
                    (int)Math.Round(radius*2.0*latFactor*riseFactor)
                );
                if (rect.Width  == 0) rect.Width  = 1;
                if (rect.Height == 0) rect.Height = 1;
                if (rect.Width  <  0) rect.Width  = Math.Abs(rect.Width );
                if (rect.Height <  0) rect.Height = Math.Abs(rect.Height);

                height = rect.Y + rect.Height;

                lfLim = (int)Math.Round(Utils.randDouble(-90.0, 270.0));
                rtLim = (int)Math.Round(Utils.randDouble(-90.0, 270.0));

                factor = (int)Math.Round(Math.Sqrt(Math.Abs(lat)));

                if (rect.Y + 0.5*rect.Height < center.Y)
                {
                    if (lfLim > 180)
                        lfLim = 180;
                    else if (lfLim < 0)
                        lfLim = 0;

                    if (rtLim > 180)
                        rtLim = 180;
                    else if (rtLim < 0)
                        rtLim = 0;
                }
                else
                {
                    if (lfLim > 180-factor)
                        lfLim = 180-factor;
                    else if (lfLim < factor)
                        lfLim =  factor;

                    if (rtLim > 180-factor)
                        rtLim = 180-factor;
                    else if (rtLim < factor)
                        rtLim =  factor;
                }

                if (lfLim > rtLim)
                {
                    int temp = lfLim;
                    lfLim = rtLim;
                    rtLim = temp;
                }

                if (lfLim == rtLim && ( rtLim == 0 || rtLim == 180))
                {
                    line--;
                    continue;
                }
            
                /*
                mid    =    (int)Math.Round(0.5*(lfLim + rtLim));
                wid    =    (int)Math.Abs(lfLim-rtLim);

                left   =    wid;
                right  =    (int)Math.Round(mid - 0.5*wid);

                factor = (int)Math.Round(Math.Sqrt(Math.Abs(lat)));

                if (rect.Y + 0.5*rect.Height < center.Y)
                {
                    if (mid+(0.5*wid) > 180)
                        left = (int)Math.Round((180-mid)+(0.5*wid));
                    else if (mid-(0.5*wid) < 0)
                    {
                        left  = (int)Math.Round((mid)+(0.5*wid));
                        right = 0;
                    }
                }
                else
                {
                    if (mid+(0.5*wid) > 180-factor)
                        left = (int)Math.Round((180-factor-mid)+(0.5*wid));
                    else if (mid-(0.5*wid) < factor)
                    {
                        left  = (int)Math.Round((mid+factor)+(0.5*wid));
                        right = factor;
                    }
                }
                */

                lightness = Utils.randInt(Color.White.R/11, Color.White.R/7) * (int)Utils.randSign();
                A = Utils.randInt(Color.White.R/7, Color.White.R/3);
                
                if (Atmosphere.albedoFromRGB(Utils.UI.hexFromColor(smask.GetPixel(radius, Math.Min(y-1, height)))) <= avgLight)
                {
                    R = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)band.R * (Color.White.R/255)))));
                    G = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)band.G * (Color.White.R/255)))));
                    B = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)band.B * (Color.White.R/255)))));
                }
                else
                {
                    R = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)surf.R * (Color.White.R/255)))));
                    G = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)surf.G * (Color.White.R/255)))));
                    B = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)surf.B * (Color.White.R/255)))));
                }
                
                p.Color = Color.FromArgb(A,R,G,B);
                
                //gs.DrawLine(p, new Point(center.X + left, center.Y + height), new Point(center.X + right, center.Y + height));

                gs.DrawArc(p, rect, lfLim, rtLim-lfLim);
            }

            
            //Rotate the image to match the planet's tilt
            surface = Utils.UI.rotate(surface, this.tilt*Utils.randSign());

            //Blur the surface of the planet
            surface = Utils.UI.blur(surface, UI.BLUR_RADIUS);


            //Copy surface onto this.image using mask to cut off the parts where the planet blurs into space
            for (int sx = 0; sx < image.Width; sx++)
                for (int sy = 0; sy < image.Height; sy++)
                    if (mask.GetPixel(sx, sy).R > 0 || mask.GetPixel(sx, sy).G > 0 || mask.GetPixel(sx, sy).B > 0)
                        image.SetPixel(sx, sy, surface.GetPixel(sx, sy));
            
            //Add lighting
            this.turn = Utils.randDouble(UI.MIN_TURN, UI.MAX_TURN);
            image     = Utils.UI.shade(image, turn, radius, 0, center);

            g.Dispose();
            gs.Dispose();
            gm.Dispose();
            p.Dispose();
            if (pgb != null)
                pgb.Dispose();

            this.image = image;
        }

        private void drawGiantOld(int x, int y, bool blur, double s)
        {
            this.image     = new Bitmap(x,y);
            Bitmap surface = new Bitmap(x,y);
            Bitmap mask    = new Bitmap(x,y);
            
            Graphics g  = Graphics.FromImage(this.image);
            Graphics gs = Graphics.FromImage(surface);
            Graphics gm = Graphics.FromImage(mask);
            Pen p = new Pen(Color.Black);
            PathGradientBrush pgb = null;

            g.Clear(Color.Black);

            GraphicsPath path;
            Point center = new Point(x/2, y/2);
            Rectangle rect;
            Color surf, band;

            int radius = (int)Math.Round(this.r * Const.Earth.RADIUS * s) + UI.BLUR_RADIUS;

            p.Width = 1;

            //For giants the atmo is the planet
            Color h = Utils.UI.colorFromHex((int)this.atmo.color);
            p.Color = Color.FromArgb(h.R * (Color.White.R/255), h.G * (Color.White.R/255), h.B * (Color.White.R/255));
            surf = p.Color;
            
            rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
            gs.FillEllipse(p.Brush, rect);

            radius -= UI.BLUR_RADIUS;
            rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
            gm.FillEllipse(p.Brush, rect);
            radius += UI.BLUR_RADIUS;
            
            //Draw the bands and jets
            int l = (int)Math.Round(Utils.fudge(30));
            int sweep, A, R, G, B, mn, mx, upLim, dnLim, lfLim, rtLim, lightness, height, left, right, greebles;

            p.StartCap = LineCap.Round;
            p.EndCap   = LineCap.Round;

            for (int i = 0; l < 100; i++)
            {
                mn = (int)Math.Abs(Math.Round(1.0 * Math.Sin(i * (Math.PI/180)) * (180/Math.PI)));
                mx = (int)Math.Abs(Math.Round(2.0 * Math.Sin(i * (Math.PI/180)) * (180/Math.PI)));

                sweep = Utils.randInt(mn, mx);

                if (i%2!=0)
                {
                    if (this.atmo.colorCloud != 0)
                        h = Utils.UI.colorFromHex((int)this.atmo.colorCloud);
                    else
                    {
                        h = Utils.UI.colorFromHex((int)this.atmo.color);
                        lightness = Utils.randInt(Color.White.R/7, Color.White.R/6) * (int)Utils.randSign();
                        h = Color.FromArgb(Math.Max(0, Math.Min(h.R+lightness, 255)), Math.Max(0, Math.Min(h.G+lightness, 255)), Math.Max(0, Math.Min(h.B+lightness, 255)));
                    }

                    //Draw the upper hemisphere band
                    path = new GraphicsPath();

                    //Draw side arcs then add straight lines by closing figure
                    path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 270+l      , sweep);
                    path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 270-l-sweep, sweep);
                    path.CloseFigure();

                    //Fill in the band
                    pgb = new PathGradientBrush(path);
                    
                    R = Math.Min(Color.White.R, (int)Math.Round((double)h.R * (Color.White.R/255)));
                    G = Math.Min(Color.White.R, (int)Math.Round((double)h.G * (Color.White.R/255)));
                    B = Math.Min(Color.White.R, (int)Math.Round((double)h.B * (Color.White.R/255)));

                    if (this.atmo.classMinor.cloudColorNames == null)
                        A = Utils.randInt(Color.White.R/7, Color.White.R/3);
                    else
                        A = Utils.randInt((Color.White.R*3)/5, Color.White.R);
                    
                    band = Color.FromArgb(A,R,G,B);
                    pgb.CenterColor = band;
                    pgb.SurroundColors = new Color[]{surf};
                    pgb.FocusScales = new PointF(1.0f, 1.0f);

                    gs.FillPath(pgb, path);

                    path.Dispose();
                    pgb.Dispose();

                    //Draw the lower hemisphere band

                    path = new GraphicsPath();

                    path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 90+l      , sweep);
                    path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 90-l-sweep, sweep);
                    path.CloseFigure();

                    pgb = new PathGradientBrush(path);
                    
                    R = Math.Min(Color.White.R, (int)Math.Round((double)h.R * (Color.White.R/255)));
                    G = Math.Min(Color.White.R, (int)Math.Round((double)h.G * (Color.White.R/255)));
                    B = Math.Min(Color.White.R, (int)Math.Round((double)h.B * (Color.White.R/255)));

                    if (this.atmo.classMinor.cloudColorNames == null)
                        A = Utils.randInt(Color.White.R/7, Color.White.R/5);
                    else
                        A = Utils.randInt((Color.White.R*3)/5, Color.White.R);
                    
                    band = Color.FromArgb(A,R,G,B);
                    pgb.CenterColor = band;
                    pgb.SurroundColors = new Color[]{surf};
                    pgb.FocusScales = new PointF(1.0f, 1.0f);

                    gs.FillPath(pgb, path);

                    path.Dispose();
                    pgb.Dispose();
                }

                l += sweep;
            }

            //Greeble
            
            double avgLight = (Atmosphere.albedoFromRGB(Utils.UI.hexFromColor(surf)) + Atmosphere.albedoFromRGB(Utils.UI.hexFromColor(h))) / 2.0;

            if (this.isIcy)
                greebles = 50;
            else
                greebles = 150;

            for (int line = 0; line < greebles; line++)
            {
                p.Width = Utils.randInt(5, 20);
            
                upLim = -radius + (int)Math.Round(p.Width);
                dnLim =  radius - (int)Math.Round(p.Width);

                height = Utils.randInt(upLim, dnLim);
            
                lfLim = -(int)Math.Round(radius + Math.Sqrt(radius));
                rtLim =  (int)Math.Round(radius + Math.Sqrt(radius));
            
                left   = Utils.randInt(lfLim, rtLim);
                right  = Utils.randInt(lfLim, rtLim);

                lightness = Utils.randInt(Color.White.R/7, Color.White.R/6) * (int)Utils.randSign();
                A = Utils.randInt(Color.White.R/7, Color.White.R/3);
                
                if (Atmosphere.albedoFromRGB(Utils.UI.hexFromColor(this.image.GetPixel(radius, center.Y + height))) >= avgLight)
                {
                    R = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)h.R * (Color.White.R/255)))));
                    G = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)h.G * (Color.White.R/255)))));
                    B = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)h.B * (Color.White.R/255)))));
                }
                else
                {
                    R = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)surf.R * (Color.White.R/255)))));
                    G = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)surf.G * (Color.White.R/255)))));
                    B = Math.Max(0, Math.Min(Color.White.R, lightness + ((int)Math.Round((double)surf.B * (Color.White.R/255)))));
                }
                
                p.Color = Color.FromArgb(A,R,G,B);
                
                gs.DrawLine(p, new Point(center.X + left, center.Y + height), new Point(center.X + right, center.Y + height));
            }


            //Rotate the image to match the planet's tilt
            surface = Utils.UI.rotate(surface, this.tilt*Utils.randSign());

            //Blur the surface of the planet
            if (blur)
                surface = Utils.UI.blur(surface, UI.BLUR_RADIUS);

            //Copy surface onto this.image using mask to cut off the parts where the planet blurs into space
            for (int sx = 0; sx < this.image.Width; sx++)
                for (int sy = 0; sy < this.image.Height; sy++)
                    if (mask.GetPixel(sx, sy).R > 0 || mask.GetPixel(sx, sy).G > 0 || mask.GetPixel(sx, sy).B > 0)
                        this.image.SetPixel(sx, sy, surface.GetPixel(sx, sy));
            
            //Add lighting
            this.turn  = Utils.randDouble(UI.MIN_TURN, UI.MAX_TURN);
            this.image = Utils.UI.shade(this.image, this.turn, radius, 0, center);

            g.Dispose();
            gs.Dispose();
            gm.Dispose();
            p.Dispose();
            if (pgb != null)
                pgb.Dispose();
        }

        private void drawRocky(int x, int y, bool blur, double s)
        {
            this.image     = new Bitmap(x,y);
            Bitmap mask    = new Bitmap(x,y);
            Bitmap surface = new Bitmap(x,y);
            Bitmap clouds  = new Bitmap(x,y);
            
            Graphics g  = Graphics.FromImage(this.image);
            Graphics gm = Graphics.FromImage(mask);
            Graphics gs = Graphics.FromImage(surface);
            Graphics gc = Graphics.FromImage(clouds);
            Pen p = new Pen(Color.Black);
            PathGradientBrush pgb = null;

            g.Clear(Color.Black);

            GraphicsPath path = new GraphicsPath();
            Point center = new Point(x/2, y/2);
            Rectangle rect;
            Color surf;
            Color h;

            int radius, atmoHeight = 0; //Radius of the planet

            radius = (int)Math.Round(this.r * Const.Earth.RADIUS * s) + UI.BLUR_RADIUS;

            p.Width = 1;

            //Draw the planet and atmosphere separately
            if (this.hasAir)
            {
                int decayConstants = UI.ATMO_SCALE_HEIGHTS;
                int samplingRate   = UI.ATMO_SAMPLING_RATE;

                radius -= 2*UI.BLUR_RADIUS;

                //Extra radius of the atmosphere
                atmoHeight = (int)Math.Round((this.atmo.height * s) / 1000.0)*decayConstants;
                float surfacePercent, transparency, position;

                //Create a bounding box around the atmosphere
                rect = new Rectangle(center.X - (radius + atmoHeight), center.Y - (radius + atmoHeight), (radius + radius + atmoHeight + atmoHeight), (radius + radius + atmoHeight + atmoHeight));
                path.AddEllipse(rect);

                pgb = new PathGradientBrush(path);

                //Determine how far along the atmosphere circle the surface is, and set the transparency at that point to the specified transparency
                surfacePercent = (float)radius/(float)(radius+atmoHeight);
                //Utils.writeLog("Calculated that the surface is at " + ((double)((int)(surfacePercent*10000.0))/100.0) + "%");

                h = Utils.UI.colorFromHex((int)this.atmo.color);
                h = Color.FromArgb(Color.White.R, h.R * (Color.White.R/255), h.G * (Color.White.G/255), h.B * (Color.White.B/255));

                //Sample points between the boundary and surfacePercent so that transparency decreases exponentially
                int samples = decayConstants*samplingRate;
                Color[] sampleColors    = new Color[samples+2];
                float[] samplePositions = new float[samples+2];
                sampleColors[0]            = Color.Black;
                samplePositions[0]         = 0.0f;
                sampleColors[samples+1]    = h;
                samplePositions[samples+1] = 1.0f;
                for (int i = 1; i <= samples; i++)
                {
                    //Calculate transparency and position
                    transparency         = (float)Math.Exp(((double)(i-1)/samplingRate)-decayConstants);
                    position             = (float)(i*((1.0-surfacePercent)/(float)samples));

                    //Add to the list
                    sampleColors[i]    = Color.FromArgb((int)Math.Round(transparency*Color.White.R), h);
                    samplePositions[i] = position;
                    //Utils.writeLog("Calculated transparency of " + ((double)((int)(transparency*10000.0))/100.0) + "% at position " + ((double)((int)(position*10000.0))/100.0) + "%");
                }

                //Utils.writeLog("Final blend object:");
                //for (int i = 0; i < sampleColors.Length; i++)
                    //Utils.writeLog("   Position: " + ((double)((int)(samplePositions[i]*10000.0))/100.0) + "%   Transparency: " + ((double)((int)(sampleColors[i].A*(1.0/Color.White.R)*10000.0))/100.0) + "%");

                //Create a Blend object and assign it to pgb
                ColorBlend blend        = new ColorBlend();
                blend.Colors            = sampleColors;
                blend.Positions         = samplePositions;

                pgb.InterpolationColors = blend;
                //pgb.FocusScales = new PointF((float)surfacePercent, (float)surfacePercent);

                g.FillEllipse(pgb, rect);

                pgb.Dispose();
                path.Dispose();

                radius += 2*UI.BLUR_RADIUS;
            }
            
            double hue;
            double saturation = Utils.randDouble(0, 0.5);
            double lightness  = this.albedo;

            if (this.isWater)
                hue = Utils.randDouble(180.0, 240.0);
            else
                hue = Utils.randDouble(0.0, 60.0);
            
            if (this.type == ID.Planet.ROCK_DENSE)
                hue = Utils.randDouble(0.0, 1.0/5.0);

            p.Color = Utils.UI.HslToRgb(hue, saturation, lightness);
            surf = p.Color;
            
            rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
            gs.FillEllipse(p.Brush, rect);

            radius -= UI.BLUR_RADIUS;
            rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
            gm.FillEllipse(p.Brush, rect);
            radius += UI.BLUR_RADIUS;
            
            //Now draw the planet's surfaces
            
            //Draw random geographic features
            double theta, r;
            int rad;
            
            if (this.isWater)
            {
                hue = Utils.randDouble(0.0, 50.0);
                saturation = Utils.randDouble(0, 0.5);
                lightness  = this.albedo;
            
                p.Color = Utils.UI.HslToRgb(hue, saturation, lightness);
            }
            else
            {
                int light = Utils.randInt(Color.White.R/10, Color.White.R/5);
            
                if (this.albedo < 0.15)
                    p.Color = Color.FromArgb(p.Color.R+light, p.Color.G+light, p.Color.B+light);
                else
                    p.Color = Color.FromArgb(Math.Abs(p.Color.R-light), Math.Abs(p.Color.G-light), Math.Abs(p.Color.B-light));
            }
                
            Point[] points;
            
            double numFeatures = Utils.randDouble(100.0, 150.0);
            
            //For each feature
            for (int f = 0; f < numFeatures; f++)
            {
                points = new Point[2];
            
                theta = Utils.randDouble(2.0 * Math.PI * (((double)f)/3.0), 2.0 * Math.PI * (((double)f+1.0)/3.0));
                r = Utils.randDouble(radius/4.0, radius);
                points[0] = new Point(center.X + (int)Math.Round(r * Math.Cos(theta)), center.Y - (int)Math.Round(r * Math.Sin(theta)));
            
                rad = (int)Math.Round(Utils.randDouble((radius-r)/10.0, radius-r));
                points[1] = new Point(rad, rad);
            
                gs.FillEllipse(p.Brush, new Rectangle(points[0].X, points[0].Y, points[1].X, points[1].Y));
                //gs.FillEllipse(p.Brush, new Rectangle(points[0].X - points[1].X/2, points[0].Y - points[1].Y/2, points[1].X, points[1].Y));
            }
            
            //Switch back to surface color
            p.Color = surf;
            
            double numPoints = Utils.randDouble(6.0, 12.0);
            
            //Pepper on some extra circles so the interior isn't boring
            for (int f = 0; f < numPoints; f++)
            {
                points = new Point[2];
            
                theta = Utils.randDouble(0.0, 2.0 * Math.PI);
                r = Utils.randDouble(radius/4.0, radius);
                points[0] = new Point(center.X + (int)Math.Round(r * Math.Cos(theta)), center.Y - (int)Math.Round(r * Math.Sin(theta)));
            
                rad = (int)Math.Round(Utils.randDouble((radius-r)/10.0, radius-r));
                points[1] = new Point(rad, rad);
            
                gs.FillEllipse(p.Brush, new Rectangle(points[0].X - points[1].X/2, points[0].Y - points[1].Y/2, points[1].X, points[1].Y));
            }
            
            //Add clouds
            if (this.hasAir && false)
            {
                //Switch to cloud color
                p.Color = Color.FromArgb(Utils.randInt(Color.White.R/7, Color.White.R/3), Utils.UI.colorFromHex((int)this.atmo.colorCloud));

                numFeatures = (int)Math.Round(1000.0 * (this.surface.coverCloudThick + this.surface.coverCloudThin));
            
                //For each feature
                for (int f = 0; f < numFeatures; f++)
                {
                    points = new Point[2];
            
                    theta = Utils.randDouble(2.0 * Math.PI * (((double)f)/3.0), 2.0 * Math.PI * (((double)f+1.0)/3.0));
                    r = Utils.randDouble(radius/4.0, radius);
                    points[0] = new Point(center.X + (int)Math.Round(r * Math.Cos(theta)), center.Y - (int)Math.Round(r * Math.Sin(theta)));
            
                    rad = (int)Math.Round(Utils.randDouble((radius-r)/10.0, (radius-r)/5.0));
                    points[1] = new Point(rad, rad);
            
                    gs.FillEllipse(p.Brush, new Rectangle(points[0].X, points[0].Y, points[1].X, points[1].Y));
                    //gs.FillEllipse(p.Brush, new Rectangle(points[0].X - points[1].X/2, points[0].Y - points[1].Y/2, points[1].X, points[1].Y));
                }
            
                p.Color = Color.Black;
            
                numPoints = Utils.randDouble(6.0, 12.0);
            
                /*
                //Pepper on some extra circles so the interior isn't boring
                for (int f = 0; f < numPoints; f++)
                {
                    points = new Point[2];
            
                    theta = Utils.randDouble(0.0, 2.0 * Math.PI);
                    r = Utils.randDouble(radius/4.0, radius);
                    points[0] = new Point(center.X + (int)Math.Round(r * Math.Cos(theta)), center.Y - (int)Math.Round(r * Math.Sin(theta)));
            
                    rad = (int)Math.Round(Utils.randDouble((radius-r)/10.0, radius-r));
                    points[1] = new Point(rad, rad);
            
                    gc.FillEllipse(p.Brush, new Rectangle(points[0].X - points[1].X/2, points[0].Y - points[1].Y/2, points[1].X, points[1].Y));
                }

                //Copy clouds onto surface
                for (int sx = 0; sx < this.image.Width; sx++)
                    for (int sy = 0; sy < this.image.Height; sy++)
                        if (clouds.GetPixel(sx, sy).R > 0 || clouds.GetPixel(sx, sy).G > 0 || clouds.GetPixel(sx, sy).B > 0)
                            surface.SetPixel(sx, sy, Utils.UI.alphaBlend(surface.GetPixel(sx, sy), clouds.GetPixel(sx, sy)));
                */

                surface = Utils.UI.rotate(surface, this.tilt);
            }
                        
            //Blur
            if (blur)
                surface = Utils.UI.blur(surface, UI.BLUR_RADIUS/2);

            //Copy surface onto this.image using mask to preserve the sharp edge between surface and atmo
            for (int sx = 0; sx < this.image.Width; sx++)
                for (int sy = 0; sy < this.image.Height; sy++)
                    if (mask.GetPixel(sx, sy).R > 0 || mask.GetPixel(sx, sy).G > 0 || mask.GetPixel(sx, sy).B > 0)
                        this.image.SetPixel(sx, sy, surface.GetPixel(sx, sy));

            
            //Give a light sheen of atmo color
            h = Utils.UI.colorFromHex((int)this.atmo.color);
            p.Color = Color.FromArgb((int)Math.Round((Color.White.R/Gen.Atmo.MAX_SURFACE_PRESSURE)*this.atmo.pressure)/(int)Gen.Atmo.RETENTION_FACTOR, h.R * (Color.White.R/255), h.G * (Color.White.R/255), h.B * (Color.White.R/255));
                
            rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);
            
            g.FillEllipse(p.Brush, rect);

            this.image = Utils.UI.rotate(this.image, this.tilt);

            //Add lighting
            this.turn  = Utils.randDouble(UI.MIN_TURN, UI.MAX_TURN);
            this.image = Utils.UI.shade(this.image, this.turn, radius, atmoHeight, center);

            g.Dispose();
            gs.Dispose();
            gm.Dispose();
            gc.Dispose();
            p.Dispose();

            if (pgb != null)
                pgb.Dispose();
        }
    }
}

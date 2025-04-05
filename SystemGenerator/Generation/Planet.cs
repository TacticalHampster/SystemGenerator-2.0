using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Utils.writeLog("            Assigning tilt and spin");
            if (this.isGiant)
            {
                //Assign tilt
                this.tilt = Utils.randExpo(Gen.Planet.Giant.MIN_TILT, Gen.Planet.Giant.MAX_TILT, 0.1);

                //Giant planets have fast spins since they're not rigid
                this.day = Utils.randDouble(Gen.Planet.Giant.MIN_DAY_LENGTH, Gen.Planet.Giant.MAX_DAY_LENGTH);

                //Giants never have retrograde spins
            }
            else
            {
                //Assign tilt
                this.tilt = Utils.randExpo(Gen.Planet.Terrestrial.MIN_TILT, Gen.Planet.Terrestrial.MAX_TILT, 0.01);

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
    
        public void genImage(int x, int y)
        {
            this.image = new Bitmap(x,y);

            Graphics g = Graphics.FromImage(this.image);
            Pen p = new Pen(Color.Black);
            PathGradientBrush pgb = null;

            g.Clear(Color.Black);

            if (this.isBelt)
            {
                g.Dispose();
                p.Dispose();
                return;
            }

            GraphicsPath path = new GraphicsPath();
            Point center = new Point(x/2, y/2);
            Rectangle rect;
            Color surf;

            int radius; //Radius of the planet

            if (this.isDwarf)
                radius = (int)Math.Round(this.r * UI.SCALE_SMALL);
            else if (this.isGiant && (this.r/Gen.Planet.Giant.GAS_RADIUS_NORM) > 1.1)
                radius = (int)Math.Round(this.r * Const.Earth.RADIUS * UI.SCALE_BIG);
            else
                radius = (int)Math.Round(this.r * Const.Earth.RADIUS * UI.SCALE_MID);

            p.Width = 1;

            //For giants the atmo is the planet
            if (this.isGiant)
            {
                Color h = Utils.UI.colorFromHex((int)this.atmo.color);
                p.Color = Color.FromArgb(h.R * (Color.White.R/255), h.G * (Color.White.R/255), h.B * (Color.White.R/255));
                surf = p.Color;
                
                rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);

                g.FillEllipse(p.Brush, rect);
            }
            else
            {
                //Otherwise draw the planet and atmosphere separately
                if (this.hasAir)
                {
                    int atmoHeight = (int)Math.Round(this.atmo.height * UI.SCALE_MID); //Extra radius of the atmosphere
                    double transparency = 0.9;

                    rect = new Rectangle(center.X - (radius + atmoHeight), center.Y - (radius + atmoHeight), (radius + radius + atmoHeight + atmoHeight), (radius + radius + atmoHeight + atmoHeight));
                    path.AddEllipse(rect);

                    pgb = new PathGradientBrush(path);

                    Color h = Utils.UI.colorFromHex((int)this.atmo.color);
                    pgb.CenterColor = Color.FromArgb((int)Math.Round(transparency * Color.White.R), h.R * (Color.White.R/255), h.G * (Color.White.R/255), h.B * (Color.White.R/255));
                    pgb.SurroundColors = new Color[]{Color.FromArgb((int)Math.Round(transparency * Color.White.R), 0, 0, 0)};

                    PointF scale = new PointF((float)radius/(float)(radius+atmoHeight), (float)radius/(float)(radius+atmoHeight));

                    pgb.FocusScales = scale;

                    g.FillEllipse(pgb, rect);

                    pgb.Dispose();
                    path.Dispose();
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

                g.FillEllipse(p.Brush, rect);
            }

            //Now draw the planet's surfaces
            if (this.isGiant || this.isIcy)
            {
                //Draw the bands and jets
                int l = (int)Math.Round(Utils.fudge(30));
                int sweep, A, R, G, B, mn, mx;
                Color h;

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
                            int lightness = Utils.randInt(Color.White.R/7, Color.White.R/6) * (int)Utils.randSign();
                            h = Color.FromArgb(Math.Max(0, Math.Min(h.R+lightness, 255)), Math.Max(0, Math.Min(h.G+lightness, 255)), Math.Max(0, Math.Min(h.B+lightness, 255)));
                        }

                        //Draw the upper hemisphere band
                        path = new GraphicsPath();

                        path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 270+l      -(float)this.tilt, sweep);
                        path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 270-l-sweep-(float)this.tilt, sweep);
                        path.CloseFigure();

                        pgb = new PathGradientBrush(path);
                        
                        R = Math.Min(Color.White.R, (int)Math.Round((double)h.R * (Color.White.R/255)));
                        G = Math.Min(Color.White.R, (int)Math.Round((double)h.G * (Color.White.R/255)));
                        B = Math.Min(Color.White.R, (int)Math.Round((double)h.B * (Color.White.R/255)));

                        if (this.atmo.classMinor.cloudColorNames == null)
                            A = Utils.randInt(Color.White.R/7, Color.White.R/3);
                        else
                            A = Utils.randInt((Color.White.R*3)/5, Color.White.R);
                        
                        pgb.CenterColor = Color.FromArgb(A,R,G,B);
                        pgb.SurroundColors = new Color[]{surf};
                        pgb.FocusScales = new PointF(1.0f, 1.0f);

                        g.FillPath(pgb, path);

                        path.Dispose();
                        pgb.Dispose();

                        //Draw the lower hemisphere band

                        path = new GraphicsPath();

                        path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 90+l      -(float)this.tilt, sweep);
                        path.AddArc(center.X - radius, center.Y - radius, radius*2, radius*2, 90-l-sweep-(float)this.tilt, sweep);
                        path.CloseFigure();

                        pgb = new PathGradientBrush(path);
                        
                        R = Math.Min(Color.White.R, (int)Math.Round((double)h.R * (Color.White.R/255)));
                        G = Math.Min(Color.White.R, (int)Math.Round((double)h.G * (Color.White.R/255)));
                        B = Math.Min(Color.White.R, (int)Math.Round((double)h.B * (Color.White.R/255)));

                        if (this.atmo.classMinor.cloudColorNames == null)
                            A = Utils.randInt(Color.White.R/7, Color.White.R/5);
                        else
                            A = Utils.randInt((Color.White.R*3)/5, Color.White.R);
                        
                        pgb.CenterColor = Color.FromArgb(A,R,G,B);
                        pgb.SurroundColors = new Color[]{surf};
                        pgb.FocusScales = new PointF(1.0f, 1.0f);

                        g.FillPath(pgb, path);

                        path.Dispose();
                        pgb.Dispose();
                    }
                    
                    l += sweep;
                }
            }
            else
            {
                //Draw random geographic features
                double theta, r;
                int rad;

                if (this.isWater)
                {
                    double hue = Utils.randDouble(0.0, 50.0);
                    double saturation = Utils.randDouble(0, 0.5);
                    double lightness  = this.albedo;
                
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

                    g.FillEllipse(p.Brush, new Rectangle(points[0].X - points[1].X/2, points[0].Y - points[1].Y/2, points[1].X, points[1].Y));
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

                    g.FillEllipse(p.Brush, new Rectangle(points[0].X - points[1].X/2, points[0].Y - points[1].Y/2, points[1].X, points[1].Y));
                }

                //Add clouds
                /*
                int q = Utils.randInt(5, 20);
                for (int f = -radius+q; f < radius-q; f++)
                {
                    if (Utils.flip() > this.surface.coverCloudThin)
                        continue;

                    path = new GraphicsPath();
                    r = (int)Math.Floor(Math.Sqrt((radius*radius) - (f*f)));
                    int height = (int)Math.Round(r * Math.Tan(10.0 * (Math.PI/180.0)));

                    Rectangle arcrect = new Rectangle(
                        center.X - (int)r,
                        center.Y + f - height/2,
                        (int)r * 2,
                        height
                    );

                    if (arcrect.Height < 1 || arcrect.Width < 1)
                    {
                        path.Dispose();
                        continue;
                    }

                    int theta1, theta2 = (int)Utils.randInt( 0, 90);
                    int roll = Utils.roll(3);
                    switch (roll)
                    {
                        case 0:
                            theta1 = (int)Utils.randInt(0, 90) + 90;
                            break;
                        case 1:
                            theta1 = (int)Utils.fudge(45);
                            break;
                        default:
                            theta1 = (int)Utils.randInt(0, 90);
                            break;
                    }

                    theta1 = 90;
                    theta2 = 270;
                    
                    path.AddArc(arcrect, theta1 - theta2/2, theta2);

                    try
                    {
                        pgb = new PathGradientBrush(path);
                    }
                    catch (Exception ex)
                    {
                        path.Dispose();
                        pgb.Dispose();
                        continue;
                    }

                    if (this.atmo.colorCloud < 1)
                        surf = Utils.UI.colorFromHex((int)this.atmo.color);
                    else
                        surf = Utils.UI.colorFromHex((int)this.atmo.colorCloud);

                    pgb.CenterColor = surf;
                    pgb.SurroundColors = new Color[]{Color.FromArgb((int)Math.Round(Color.White.R*0.1), surf)};
                    
                    g.FillPath(pgb, path);

                    pgb.Dispose();
                    path.Dispose();
                }
                */

                //Give a light sheen of atmo color
                Color h = Utils.UI.colorFromHex((int)this.atmo.color);
                p.Color = Color.FromArgb((int)Math.Round((Color.White.R/Gen.Atmo.MAX_SURFACE_PRESSURE)*this.atmo.pressure)/(int)Gen.Atmo.RETENTION_FACTOR, h.R * (Color.White.R/255), h.G * (Color.White.R/255), h.B * (Color.White.R/255));
                    
                rect = new Rectangle(center.X - radius, center.Y - radius, radius*2, radius*2);

                g.FillEllipse(p.Brush, rect);
            }

            g.Dispose();
            p.Dispose();

            if (pgb != null)
                pgb.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        public bool isIcy  ;
        public bool isWater;
        public bool isHab  ;
        public bool isBelt ;
        public bool isDwarf;
        public bool hasAir ;

        public Planet()
        {
            this.orbit   = new Orbit();
            this.atmo    = new Atmosphere();
            this.rings   = new List<double>();
            this.moons   = new List<Moon>();
            this.surface = new Atmosphere.Surface();
            this.subtype = "";
            this.feature = "";
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
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = true;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.m         = Utils.randDouble(Gen.Planet.Terrestrial.MIN_ROCKY_MASS      , Gen.Planet.Terrestrial.MAX_ROCKY_MASS      );
                    this.bulkRock  = Utils.randDouble(Gen.Planet.Terrestrial.MIN_CORE_MASS       , Gen.Planet.Terrestrial.MAX_CORE_MASS       );
                    this.bulkWater = Utils.randDouble(Gen.Planet.Terrestrial.MIN_EARTH_WATER_MASS, Gen.Planet.Terrestrial.MAX_EARTH_WATER_MASS);
                    this.bulkMetal = 1.0-this.bulkMetal-this.bulkWater;
                    propsRocky();
                    break;

                case ID.Planet.WATER_OCEAN:
                    //Set flags
                    this.isGiant = false;
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
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.subtype = "1";
                    this.m = Utils.randDouble(Gen.Planet.Giant.MIN_GIANT_MASS, Math.Min(mass_max, Gen.Planet.Giant.MAX_GIANT_MASS));
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
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.subtype = "3";

                    do
                    {
                        this.m = Utils.randDouble(Gen.Planet.Giant.MIN_GIANT_MASS , Gen.Planet.Giant.GAS_RADIUS_LIM);
                        this.r = Utils.randDouble(Gen.Planet.Giant.GAS_RADIUS_NORM, Gen.Planet.Giant.GAS_RADIUS_MAX);
                        propsGiant();
                    }
                    while (this.bulkDensity > Gen.Planet.Giant.MAX_PUFFY_DENSITY);

                    break;

                case ID.Planet.GAS_HOT:
                    //Set flags
                    this.isGiant = true;
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = false;

                    //Set composition
                    this.subtype = "4";

                    do
                    {
                        this.m = Utils.randDouble(Gen.Planet.Giant.MIN_GIANT_MASS , Gen.Planet.Giant.GAS_RADIUS_LIM);
                        this.r = Utils.randDouble(Gen.Planet.Giant.GAS_RADIUS_NORM, Gen.Planet.Giant.GAS_RADIUS_MAX);
                        propsGiant();
                    }
                    while (this.bulkDensity > Gen.Planet.Giant.MAX_PUFFY_DENSITY);

                    break;

                case ID.Planet.ICE_DWARF:
                    //Set flags
                    this.isGiant = true;
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
                    this.isIcy   = false;
                    this.isWater = false;
                    this.isHab   = false;
                    this.isBelt  = false;
                    this.isDwarf = true ;

                    //Set composition
                    this.r = Utils.randDouble(Gen.Belt.MIN_DWARF_RADIUS, Math.Min(mass_max*2660.16, Gen.Belt.MAX_DWARF_RADIUS));

                    this.bulkIces  = Utils.randDouble(Gen.Belt.MIN_KUIPER_DWARF_ICES_PERCENT, Gen.Belt.MAX_KUIPER_DWARF_ICES_PERCENT);
                    this.bulkMetal = (this.bulkIces)*Utils.randDouble(Gen.Belt.MIN_KUIPER_DWARF_ROCK_PERCENT, Gen.Belt.MAX_KUIPER_DWARF_ROCK_PERCENT);
                    this.bulkIces  = 1.0 - this.bulkMetal - this.bulkRock;

                    propsDwarf();

                    break;

                default:
                    break;
            }
            Utils.writeLog("            Mass: " + this.m);
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
                this.tilt = Utils.randDouble(Gen.Planet.Giant.MIN_TILT, Gen.Planet.Giant.MAX_TILT);

                //Giant planets have fast spins since they're not rigid
                this.day = Utils.randDouble(Gen.Planet.Giant.MIN_DAY_LENGTH, Gen.Planet.Giant.MAX_DAY_LENGTH);

                //Giants never have retrograde spins
            }
            else
            {
                //Assign tilt
                this.tilt = Utils.randDouble(Gen.Planet.Terrestrial.MIN_TILT, Gen.Planet.Terrestrial.MAX_TILT);

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
    
        public void GenFeature()
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
                        "It has %i continent%s, covering about %.0f%% of its surface.",
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
    }
}

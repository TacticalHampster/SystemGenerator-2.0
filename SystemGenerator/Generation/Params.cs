using System;
using System.Collections.Generic;
using System.Drawing.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemGenerator.Generation
{
    public class Const
    {
        public const double SUN_TEMP   = 5772.0;    //(K)
        public const double SUN_LUMIN  = 3.828e26;

        public const double GAS_CONST  = 8.3144598;
        public const double GRAV_CONST = 6.6743015e-11;
        public const double KELVIN     = 273.15;

        public const double MOON_RAD_MULT = 2660.16; //No idea where this comes from

        public const double METAL_DENS = 7.87;
        public const double ROCK_DENS  = 3.25;
        public const double WATER_DENS = 0.93;

        public const string FORMAT = "{0,7:N4}";

        public const double ZETTAGRAMS_PER_EARTHMASS = 5972200.0;
        public const double AU_PER_EARTHRADIUS       = 6378000.0/149597870700.0;

        public class Earth
        {
            public const double MASS    = 5.9722e24; //(kg)
            public const double RADIUS  = 6378.0;    //(m)
            public const double YEAR    = 365.2515;  //(d🜨)
            public const double GRAVITY = 9.80665;   //(m/s²)
            public const double DENSITY = 5.513;     //(g/cm³)
            public const double ESCV    = 11186.0;
            public const double ORBV    = 29.7827;   //(km/s)
            public const double TEMP    = 288.0;     //(K)
        }
    }

    public class Gen
    {
        public const double FUDGE_FACTOR  = 0.05;
        public const int    SAMPLE_SIZE   = 20;

        public class System
        {
            public const double ORBIT_SPACING_MIN        = 1.4;
            public const double ORBIT_SPACING_MAX        = 2.0;

            public const double MIGRATE_MID_CHANCE       = 0.05;
            public const double MIGRATE_HOT_CHANCE       = 0.05;

            public const bool DISABLE_GAS_GIANT_CUTOFF   = false;

            //Probability distributions of planet types
            public const double DENSE_CHANCE             = 0.8;
            public const double OCEAN_CHANCE             = 0.6;
            public const double ICE_GIANT_CHANCE         = 0.8;
            public const double ICE_DWARF_CHANCE         = 0.15;

            public const Decay.DecayType OCEAN_DECAY     = Decay.DecayType.LINEAR;
            public const Decay.DecayType DENSE_DECAY     = Decay.DecayType.EXP;
            public const Decay.DecayType ICE_GIANT_DECAY = Decay.DecayType.LINEAR;
            public const Decay.DecayType ICE_DWARF_DECAY = Decay.DecayType.EXP;
        }

        public class Star
        {
            public const double STAR_MASS_MIN = 0.6;
            public const double STAR_MASS_MAX = 1.4;
        }

        public class Orbit
        {
            public class Giant
            {
                public const double MIN_ECCENTRICITY = 0.001;
                public const double MAX_ECCENTRICITY = 0.100;

                public const double MIN_INCLINATION  = 0.000;
                public const double MAX_INCLINATION  = 2.000;
            }

            public class Terrestrial
            {
                public const double MIN_ECCENTRICITY = 0.000;
                public const double MAX_ECCENTRICITY = 0.200;

                public const double MIN_INCLINATION  = 0.000;
                public const double MAX_INCLINATION  = 2.000;
            }

            public class Belt
            {
                public const double MIN_BELT_ECCENTRICITY    =  0.001;
                public const double MAX_BELT_ECCENTRICITY    =  0.100;

                public const double MIN_DWARF_ECCENTRICITY   =  0.001;
                public const double MAX_DWARF_ECCENTRICITY   =  0.010;

                public const double MIN_SEDNOID_ECCENTRICITY =  0.750;
                public const double MAX_SEDNOID_ECCENTRICITY =  1.000;

                public const double MIN_INCLINATION          =  0.000;
                public const double MAX_INCLINATION          = 10.000;

                public const double MIN_INNER_HEIGHT         =  0.000;
                public const double MAX_INNER_HEIGHT         = 17.000;

                public const double MIN_KUIPER_HEIGHT        =  0.000;
                public const double MAX_KUIPER_HEIGHT        = 40.000;
            }
            
            public class Sat
            {
                public const double MIN_GIANT_ECCENTRICITY  = 0.001;
                public const double MAX_GIANT_ECCENTRICITY  = 0.010;

                public const double MIN_GIANT_INCLINATION   = 0.000;
                public const double MAX_GIANT_INCLINATION   = 2.000;

                public const double MIN_TERRES_ECCENTRICITY = 0.001;
                public const double MAX_TERRES_ECCENTRICITY = 0.010;

                public const double MIN_TERRES_INCLINATION  = 0.000;
                public const double MAX_TERRES_INCLINATION  = 2.000;

                public const double MIN_DWARF_ECCENTRICITY  = 0.001;
                public const double MAX_DWARF_ECCENTRICITY  = 0.010;

                public const double MIN_DWARF_INCLINATION   = 0.000;
                public const double MAX_DWARF_INCLINATION   = 2.000;
            }
        }

        public class Planet
        {
            public class Giant
            {
                public const double MIN_GIANT_MASS         =   10.000;
                public const double MAX_GIANT_MASS         = 4131.769;
            
                public const double GAS_RADIUS_NORM        =   10.973;
                public const double GAS_RADIUS_LIM         =  635.657;
                public const double GAS_RADIUS_MAX         =   21.297;

                public const double MAX_PUFFY_DENSITY      =    1.000;
                
                public const double MIN_ICE_GIANT_MASS     =    2.000;
                public const double MAX_ICE_GIANT_MASS     =   20.000;
                public const double MIN_ICE_GIANT_RADIUS   =    3.000;
                public const double MAX_ICE_GIANT_RADIUS   =         GAS_RADIUS_NORM;
                public const double MIN_ICE_GIANT_DENS     =    1.000;
                public const double MAX_ICE_GIANT_DENS     =    3.000;

                public const double MIN_GIANT_SOLID_MASS   =    0.030;
                public const double MAX_GIANT_SOLID_MASS   =    0.130;

                public const double MIN_GIANT_ROCKY_MASS   =    0.450;
                public const double MAX_GIANT_ROCKY_MASS   =    0.750;

                public const double GIANT_SILICATE_PERCENT =    0.750;

                public const double MIN_GIANT_ICES_MASS    =    0.450;
                public const double MAX_GIANT_ICES_MASS    =    0.750;

                public const double MIN_ICY_CORE_MASS      =    0.030;
                public const double MAX_ICY_CORE_MASS      =    0.080;

                public const double MIN_ICY_ATMO_MASS      =    0.150;
                public const double MAX_ICY_ATMO_MASS      =    0.500;

                public const double MIN_ICY_ICES_MASS      =    0.600;
                public const double MAX_ICY_ICES_MASS      =    0.800;

                public const double MIN_TILT               =    0.000;
                public const double MAX_TILT               =   90.000;

                public const double MIN_DAY_LENGTH         =    8.000;
                public const double MAX_DAY_LENGTH         =   24.000;
            }
            
            public class Terrestrial
            {
                public const double MIN_DENSE_MASS       =    0.0001;
                public const double MAX_DENSE_MASS       =    1.6000;

                public const double MIN_ROCKY_MASS       =    0.1000;
                public const double MAX_ROCKY_MASS       =   10.0000;

                public const double MIN_CORE_MASS        =    0.6000;
                public const double MAX_CORE_MASS        =    0.8000;

                public const double MIN_EARTH_WATER_MASS =    0.0000;
                public const double MAX_EARTH_WATER_MASS =    0.0015;

                public const double MIN_OCEAN_WATER_MASS =    0.6000;
                public const double MAX_OCEAN_WATER_MASS =    0.8000;

                public const double MIN_OCEAN_CORE_MASS  =    0.0100;
                public const double MAX_OCEAN_CORE_MASS  =    0.0800;

                public const double MIN_TILT             =    0.0000;
                public const double MAX_TILT             =   90.0000;

                public const double MIN_DAY_LENGTH       =    8.0000;
                public const double MAX_DAY_LENGTH       = 3000.0000;

                public const double MIN_RETRO_DAY_LENGTH = 1500.0000;
                public const double RETRO_DAY_CHANCE     =    0.1000;

                public const double MIN_OCEAN1_LAND      =    0.1000;
                public const double MAX_OCEAN1_LAND      =    0.1500;
                public const double MIN_OCEAN2_LAND      =    0.0500;
                public const double MAX_OCEAN2_LAND      =    0.1000;
                public const double MIN_OCEAN3_LAND      =    0.0000;
                public const double MAX_OCEAN3_LAND      =    0.0700;
                public const double MIN_HAB_LAND         =    0.3333;
                public const double MAX_HAB_LAND         =    0.5000;

                public const double MIN_DULL_ALBEDO      =    0.0500;
                public const double MAX_DULL_ALBEDO      =    0.2500;
                public const double MIN_BRIGHT_ALBEDO    =    0.2000;
                public const double MAX_BRIGHT_ALBEDO    =    0.4000;

                public const double MIN_WATER_ALBEDO     =    0.0600;
                public const double MAX_WATER_ALBEDO     =    0.0800;
                public const double MIN_ICE_ALBEDO       =    0.6500;
                public const double MAX_ICE_ALBEDO       =    0.7500;

                public const int    MIN_CONTINENTS       =    1     ;
                public const int    MAX_CONTINENTS       =    6     ;
            }
        }

        public class Belt
        {
            public const double INNER_BELT_CHANCE  = 0.25;
            public const double INNER_DWARF_CHANCE = 0.5;
            
            public const double KUIPER_BELT_CHANCE = 0.25;

            public const int MIN_PLUTINO   = 0;
            public const int MAX_PLUTINO   = 2;
            public const int MIN_CUBEWANO  = 0;
            public const int MAX_CUBEWANO  = 4;
            public const int MIN_TWOTINO   = 0;
            public const int MAX_TWOTINO   = 2;
            public const int MIN_SCATTERED = 0;
            public const int MAX_SCATTERED = 6;
            public const int MIN_SEDNOID   = 0;
            public const int MAX_SEDNOID   = 3;
                
            public const double MIN_CARBON_ALBEDO = 0.03;
            public const double MAX_CARBON_ALBEDO = 0.10;
            public const double MIN_ROCK_ALBEDO   = 0.05;
            public const double MAX_ROCK_ALBEDO   = 0.25;
            public const double MIN_ICES_ALBEDO   = 0.30;
            public const double MAX_ICES_ALBEDO   = 0.30;
            public const double MIN_METAL_ALBEDO  = 0.10;
            public const double MAX_METAL_ALBEDO  = 0.25;

            public const double MIN_BELT_MASS          =  0.3636;
            public const double MAX_BELT_MASS          =  3.636;

            public const double MIN_INNER_BELT_HEIGHT  =  0.0;
            public const double MAX_INNER_BELT_HEIGHT  = 17.0;
            public const double MIN_KUIPER_BELT_HEIGHT =  0.0;
            public const double MAX_KUIPER_BELT_HEIGHT = 40.0;

            public const double MIN_INNER_CARBON_PERCENT = 0.75 ;
            public const double MAX_INNER_CARBON_PERCENT = 0.75 ;
            public const double MIN_INNER_ROCK_PERCENT   = 0.17 ;
            public const double MAX_INNER_ROCK_PERCENT   = 0.17 ;
            public const double MIN_INNER_ICES_PERCENT   = 0.00 ;
            public const double MAX_INNER_ICES_PERCENT   = 0.005;
                        
            public const double MIN_KUIPER_CARBON_PERCENT = 0.17;
            public const double MAX_KUIPER_CARBON_PERCENT = 0.17;
            public const double MIN_KUIPER_ICES_PERCENT   = 0.75;
            public const double MAX_KUIPER_ICES_PERCENT   = 0.75;
            public const double MIN_KUIPER_METAL_PERCENT  = 0.00;
            public const double MAX_KUIPER_METAL_PERCENT  = 0.005;

            public const double MIN_DWARF_RADIUS = 300.0;
            public const double MAX_DWARF_RADIUS = Const.Earth.RADIUS*0.001;

            public const double MIN_INNER_DWARF_ROCK_PERCENT = 0.6;
            public const double MAX_INNER_DWARF_ROCK_PERCENT = 0.9;
            public const double MIN_INNER_DWARF_METAL_PERCENT = 0.6;
            public const double MAX_INNER_DWARF_METAL_PERCENT = 0.9;

            public const double MIN_KUIPER_DWARF_ICES_PERCENT = 0.7;
            public const double MAX_KUIPER_DWARF_ICES_PERCENT = 0.9;
            public const double MIN_KUIPER_DWARF_ROCK_PERCENT = 0.75;
            public const double MAX_KUIPER_DWARF_ROCK_PERCENT = 0.99;
        }

        public class Sat
        {
            public const double MIN_RADIUS = 300.0;

            public const double MIN_COMP_DROPOFF = 0.6;
            public const double MAX_COMP_DROPOFF = 0.9;

            public const double TERRES_MOONS_CHANCE = 0.95;
            public const double TERRES_ICY_CHANCE   = 0.03;
            public const double DWARF_ROCKY_CHANCE  = 0.03;
            public const double GIANT_ROCKY_CHANCE  = 0.03;

            public const Decay.DecayType TERRES_MOONS_DECAY = Decay.DecayType.LINEAR;
            public const Decay.DecayType TERRES_ICY_DECAY   = Decay.DecayType.LINEAR;
            public const Decay.DecayType DWARF_ROCKY_DECAY  = Decay.DecayType.LINEAR;
            public const Decay.DecayType GIANT_ROCKY_DECAY  = Decay.DecayType.LINEAR;

            public const int MIN_TERRES_MOONS = 0;
            public const int MAX_TERRES_MOONS = 4;

            public const int MIN_DWARF_MOONS = 0;
            public const int MAX_DWARF_MOONS = 5;

            public const int MIN_CTYPE_CLUSTERS  = 0;
            public const int MAX_CTYPE_CLUSTERS  = 3;
            public const double CTYPE_MOON_DECAY = 0.9;
            public const int MIN_CTYPE_MOONS     = 0;
            public const int MAX_CTYPE_MOONS     = 4;

            public const double MAJOR_DWARF_MOON_CHANCE = 0.33;

            public const double LAGRANGIAN_CHANCE = 0.15;

            public const double MAX_MOON_SPACING = 4.0;


            public const double MIN_TILT =  0.0;
            public const double MAX_TILT = 90.0;

            public const double MIN_DAY_LENGTH =    8.0;
            public const double MAX_DAY_LENGTH = 3000.0;

            public const double MIN_RETRO_DAY_LENGTH = 1500.0;
            public const double RETRO_DAY_CHANCE     =    0.1;
        }
    
        public class Atmo
        {
            public const double RETENTION_FACTOR = 6.0;
            public const double ATMO_DROPOFF     = 0.9;

            public const double MIN_MAJORITY_COMP = 0.85;
            public const double MAX_MAJORITY_COMP = 0.91;
            
            public const double MIN_CLOUD_COVER = 0.1;
            public const double MAX_CLOUD_COVER = 0.7;

            public const double MIN_SURFACE_PRESSURE =   0.0;
            public const double MAX_SURFACE_PRESSURE = 100.0;
            
            public const double MIN_CRYOAZURIAN_CLOUD_COVER       = 0.0;
            public const double MAX_CRYOAZURIAN_CLOUD_COVER       = 0.2;
            public const double MIN_CRYOAZURIAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_CRYOAZURIAN_THICK_CLOUD_COVER = 0.0;
            
            public const double MIN_FRIGIDIAN_CLOUD_COVER       = 0.1;
            public const double MAX_FRIGIDIAN_CLOUD_COVER       = 0.7;
            public const double MIN_FRIGIDIAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_FRIGIDIAN_THICK_CLOUD_COVER = 0.3;
            
            public const double MIN_NEONEAN_CLOUD_COVER       = 0.1;
            public const double MAX_NEONEAN_CLOUD_COVER       = 0.7;
            public const double MIN_NEONEAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_NEONEAN_THICK_CLOUD_COVER = 0.3;
            
            public const double MIN_BOREAN_CLOUD_COVER       = 0.1;
            public const double MAX_BOREAN_CLOUD_COVER       = 0.7;
            public const double MIN_BOREAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_BOREAN_THICK_CLOUD_COVER = 0.3;
            
            public const double MIN_METHANEAN_CLOUD_COVER       = 0.25;
            public const double MAX_METHANEAN_CLOUD_COVER       = 0.85;
            public const double MIN_METHANEAN_THICK_CLOUD_COVER = 0.1;
            public const double MAX_METHANEAN_THICK_CLOUD_COVER = 0.4;
            
            public const double MIN_MESOAZURIAN_CLOUD_COVER       = 0.15;
            public const double MAX_MESOAZURIAN_CLOUD_COVER       = 0.7;
            public const double MIN_MESOAZURIAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_MESOAZURIAN_THICK_CLOUD_COVER = 0.1;
            
            public const double MIN_THOLIAN_CLOUD_COVER       = 0.5;
            public const double MAX_THOLIAN_CLOUD_COVER       = 1.0;
            public const double MIN_THOLIAN_THICK_CLOUD_COVER = 0.25;
            public const double MAX_THOLIAN_THICK_CLOUD_COVER = 0.85;
            
            public const double MIN_SULFANIAN_CLOUD_COVER       = 0.1;
            public const double MAX_SULFANIAN_CLOUD_COVER       = 0.7;
            public const double MIN_SULFANIAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_SULFANIAN_THICK_CLOUD_COVER = 0.3;
            
            public const double MIN_AMMONIAN_CLOUD_COVER       = 0.1;
            public const double MAX_AMMONIAN_CLOUD_COVER       = 0.7;
            public const double MIN_AMMONIAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_AMMONIAN_THICK_CLOUD_COVER = 0.3;
            
            public const double MIN_HYDRONIAN_CLOUD_COVER       = 0.1;
            public const double MAX_HYDRONIAN_CLOUD_COVER       = 0.7;
            public const double MIN_HYDRONIAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_HYDRONIAN_THICK_CLOUD_COVER = 0.3;
            
            public const double MIN_ACIDIAN_CLOUD_COVER       = 0.1;
            public const double MAX_ACIDIAN_CLOUD_COVER       = 0.7;
            public const double MIN_ACIDIAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_ACIDIAN_THICK_CLOUD_COVER = 0.3;
            
            public const double MIN_PYROAZURIAN_CLOUD_COVER       = 0.0;
            public const double MAX_PYROAZURIAN_CLOUD_COVER       = 0.2;
            public const double MIN_PYROAZURIAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_PYROAZURIAN_THICK_CLOUD_COVER = 0.0;
            
            public const double MIN_SULFOLIAN_CLOUD_COVER       = 0.1;
            public const double MAX_SULFOLIAN_CLOUD_COVER       = 0.7;
            public const double MIN_SULFOLIAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_SULFOLIAN_THICK_CLOUD_COVER = 0.3;
            
            public const double MIN_AITHALIAN_CLOUD_COVER       = 0.1;
            public const double MAX_AITHALIAN_CLOUD_COVER       = 0.7;
            public const double MIN_AITHALIAN_THICK_CLOUD_COVER = 0.0;
            public const double MAX_AITHALIAN_THICK_CLOUD_COVER = 0.3;
        }
    }

    public class ID
    {
        public class Planet
        {
            public const char GAS_GIANT   = 'J';
            public const char GAS_SUPER   = 'S'; //Super Jupiter
            public const char GAS_PUFFY   = 'P'; //Low-density gas giant
            public const char GAS_HOT     = 'H'; 
            public const char ICE_GIANT   = 'I';
            public const char ICE_DWARF   = 'N'; //Gas dwarf
            public const char ROCK_DENSE  = 'M'; //Core-dominated mercurial
            public const char ROCK_DESERT = 'V'; //Non-habitable rocky
            public const char ROCK_GREEN  = 'E'; //Habitable rocky
            public const char WATER_OCEAN = 'O'; //Non-habitable ocean
            public const char WATER_GREEN = 'W'; //Habitable ocean
            public const char EMPTY       = '0';
        }

        public class Sat
        {
            public const char MINOR = 'm'; //Captured asteroid of non-giant
            public const char MAJOR = 't'; //Gravitationally rounded moon
            public const char MOONA = 'a'; //A-type ring shepherd minor moon of giant
            public const char MOONB = 'b'; //B-type major moon of giant
            public const char MOONC = 'c'; //C-type distant irregular minor moon of giant
            public const char FOR_B = 'f'; //Forward lagrangian companion to a B-type
            public const char REV_B = 'r'; //Reverse lagrangian companion to a B-type
        }

        public class Belt
        {
            public const char BELT_INNER  = 'B';
            public const char BELT_KUIPER = 'k';

            public const char DWARF     = 'd';
            public const char PLUTINO   = '1'; //2:3 resonant kuiper object
            public const char CUBEWANO  = 'C'; //Classical non-resonant kuiper object
            public const char TWOTINO   = '2'; //1:2 resonant kuiper object
            public const char SCATTERED = 'Q'; //Extra-kuiper detacted objects
            public const char SEDNOID   = 'D'; //Extremely distant extra-kuiper objects
        }

        public class Atmo
        {
            public const char MJR_JOTUNNIAN = 'J'; //Primarily hydrogen
            public const char MJR_HELIAN    = 'H'; //Primarily helium
            public const char MJR_YDATRIAN  = 'Y'; //Primarily hydrides
            public const char MJR_RHEAN     = 'R'; //Primarily nitrogen
            public const char MJR_MINERVAN  = 'M'; //Primarily covalent compounts
            public const char MJR_EDELIAN   = 'E'; //Primarily neon

            public const char MNR_CRYOAZURIAN = 'Y'; //Cold, dull blue
            public const char MNR_FRIGIDIAN   = 'F'; //Cold, grey-blue
            public const char MNR_NEONEAN     = 'N'; //Cold, pinkish
            public const char MNR_BOREAN      = 'B'; //Cold, pink-purple
            public const char MNR_METHANEAN   = 'E'; //Cold, blue-green

            public const char MNR_MESOAZURIAN = 'M'; //Warm, blue-green
            public const char MNR_THOLIAN     = 'T'; //Warm, yellow-orange
            public const char MNR_SULFANIAN   = 'S'; //Warm, yellow-gold
            public const char MNR_AMMONIAN    = 'A'; //Warm, cream
            public const char MNR_HYDRONIAN   = 'H'; //Warm, off-white
            public const char MNR_ACIDIAN     = 'C'; //Warm, tan

            public const char MNR_PYROAZURIAN = 'P'; //Hot, blue
            public const char MNR_SULFOLIAN   = 'U'; //Hot, gold-green
            public const char MNR_AITHALIAN   = 'L'; //Hot, grey-brown
        }
    }

    public class Comps
    {
        public static readonly Atmosphere.Component HYDROGEN    = new Atmosphere.Component("Hydrogen"         , 0.00201588, Color.HYDROGEN);
        public static readonly Atmosphere.Component HELIUM      = new Atmosphere.Component("Helium"           , 0.0040026 , Color.HYDROGEN);
        
        public static readonly Atmosphere.Component NITROGEN    = new Atmosphere.Component("Nitrogen"         , 0.028014  , Color.NITROGEN);
        public static readonly Atmosphere.Component AMMONIA     = new Atmosphere.Component("Ammonia"          , 0.017031  , Color.NITROGEN);
        public static readonly Atmosphere.Component H_CYANIDE   = new Atmosphere.Component("Hydrogen Cyanide" , 0.027026  , Color.NITROGEN);
        public static readonly Atmosphere.Component N_OXIDE     = new Atmosphere.Component("Nitric Oxide"     , 0.030006  , Color.NITROGEN);
        public static readonly Atmosphere.Component N_DIOXIDE   = new Atmosphere.Component("Nitrogen Dioxide" , 0.046005  , Color.NITROGEN);
        public static readonly Atmosphere.Component CYANOGEN    = new Atmosphere.Component("Cyanogen"         , 0.052036  , Color.NITROGEN);
         
        public static readonly Atmosphere.Component C_MONOXIDE  = new Atmosphere.Component("Carbon Monoxide"  , 0.02801   , Color.CARBON  );
        public static readonly Atmosphere.Component C_DIOXIDE   = new Atmosphere.Component("Carbon Dioxide"   , 0.044009  , Color.CARBON  );
        public static readonly Atmosphere.Component C_DISULFIDE = new Atmosphere.Component("Carbon Disulfide" , 0.07613   , Color.CARBON  );
        public static readonly Atmosphere.Component METHANE     = new Atmosphere.Component("Methane"          , 0.01604   , Color.CARBON  );
        public static readonly Atmosphere.Component METHYLENE   = new Atmosphere.Component("Methylene"        , 0.0140266 , Color.CARBON  );
        public static readonly Atmosphere.Component ETHANE      = new Atmosphere.Component("Ethane"           , 0.03007   , Color.CARBON  );
        public static readonly Atmosphere.Component ETHYLENE    = new Atmosphere.Component("Ethylene"         , 0.028054  , Color.CARBON  );
        public static readonly Atmosphere.Component ACETYLENE   = new Atmosphere.Component("Acetylene"        , 0.026038  , Color.CARBON  );
        public static readonly Atmosphere.Component DIACETYLENE = new Atmosphere.Component("Diacetylene"      , 0.05006   , Color.CARBON  );
        public static readonly Atmosphere.Component PROPANE     = new Atmosphere.Component("Propane"          , 0.0441    , Color.CARBON  );
        public static readonly Atmosphere.Component PROPYNE     = new Atmosphere.Component("Propyne"          , 0.040065  , Color.CARBON  );
        public static readonly Atmosphere.Component THOLINS     = new Atmosphere.Component("Tholins"          , 0.0       , Color.CARBON  );
        
        public static readonly Atmosphere.Component H_SULFIDE   = new Atmosphere.Component("Hydrogen Sulfide" , 0.03408   , Color.SULFUR  );
        public static readonly Atmosphere.Component S_DIOXIDE   = new Atmosphere.Component("Sulfur Dioxide"   , 0.06406   , Color.SULFUR  );
        public static readonly Atmosphere.Component CARBONYL_S  = new Atmosphere.Component("Carbonyl Sulfide" , 0.06007   , Color.SULFUR  );

        public static readonly Atmosphere.Component OXYGEN      = new Atmosphere.Component("Oxygen"           , 0.031998  , Color.OXYGEN  );
        public static readonly Atmosphere.Component WATER       = new Atmosphere.Component("Water"            , 0.018015  , Color.WATER   );
        public static readonly Atmosphere.Component PHOSPHINE   = new Atmosphere.Component("Phosphine"        , 0.033998  , Color.PHOSPHOR);
        public static readonly Atmosphere.Component SILANE      = new Atmosphere.Component("Silane"           , 0.032117  , Color.SILICON );
        public static readonly Atmosphere.Component H_FLUORIDE  = new Atmosphere.Component("Hydrogen Fluoride", 0.020006  , Color.FLUORINE);
        public static readonly Atmosphere.Component H_CHLORIDE  = new Atmosphere.Component("Hydrogen Chloride", 0.03646   , Color.CHLORINE);

        public static readonly Atmosphere.Component NEON        = new Atmosphere.Component("Neon"             , 0.0201797 , Color.NEON    );
        public static readonly Atmosphere.Component ARGON       = new Atmosphere.Component("Argon"            , 0.039948  , Color.ARGON   );
        public static readonly Atmosphere.Component KRYPTON     = new Atmosphere.Component("Krypton"          , 0.083798  , Color.KRYPTON );


        public class Color
        {
            public        const    int HYDROGEN = 0xFFFFFF;
            public        const    int CARBON   = 0x838383;
            public static readonly int NITROGEN = System.Drawing.Color.RoyalBlue.ToArgb();
            public        const    int OXYGEN   = 0x00FFFF;
            public static readonly int WATER    = System.Drawing.Color.DodgerBlue.ToArgb();
            public        const    int FLUORINE = 0xFFFF00;
            public        const    int NEON     = 0xFF4500;
            public        const    int SILICON  = 0xB8860B;
            public        const    int PHOSPHOR = 0x8B0000;
            public        const    int SULFUR   = 0xFFEA00;
            public        const    int CHLORINE = 0x006400;
            public        const    int ARGON    = 0xEE82EE;
            public        const    int KRYPTON  = 0xA9A9A9;
            public        const    int XENON    = 0x99CFE0;
        }
    }
}

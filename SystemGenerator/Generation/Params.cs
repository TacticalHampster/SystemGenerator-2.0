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
        public const double SUN_RADIUS = 109.0 * Const.Earth.RADIUS;

        public const double GAS_CONST  = 8.3144598;
        public const double GRAV_CONST = 6.6743015e-11;
        public const double KELVIN     = 273.15;

        public const double MOON_RAD_MULT = 2660.16; //No idea where this comes from

        public const double METAL_DENS = 7.87;
        public const double ROCK_DENS  = 3.25;
        public const double WATER_DENS = 0.93;

        public const double EARTHMASS_PER_JOVEMASS   = 317.8;
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

    public class UI
    {
        public const string FORMAT      = "{0,7:N4}";
        public const double SCALE_SMALL = 1.0/2.0;
        public const double SCALE_MAJOR = 1.0/5.0;
        public const double SCALE_MID   = 1.0/200.0;
        public const double SCALE_BIG   = 1.0/400.0;
        public const double SCALE_STAR  = 1.0/1600.0;
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
            public const double DENSE_CHANCE             = 0.4;
            public const double OCEAN_CHANCE             = 0.6;
            public const double ICE_GIANT_CHANCE         = 0.6;
            public const double ICE_DWARF_CHANCE         = 0.4;

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
            public const double MIN_KUIPER_ICES_PERCENT   = 0.65;
            public const double MAX_KUIPER_ICES_PERCENT   = 0.85;
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

            public const double          CTYPE_CHANCE = 0.75;
            public const Decay.DecayType CTYPE_DECAY  = Decay.DecayType.EXP;
            public const Decay.DecayDir  CTYPE_DIR    = Decay.DecayDir.DECREASING;

            public const int MIN_CTYPE_CLUSTERS  = 0;
            public const int MAX_CTYPE_CLUSTERS  = 5;
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

            public const double MIN_MOON_ALBEDO = 0.1;
            public const double MAX_MOON_ALBEDO = 0.8;

            public const double MOON_ATMO_CHANCE = 0.07;
        }
    
        public class Atmo
        {
            public const double RETENTION_FACTOR = 6.0;
            public const double ATMO_DROPOFF     = 0.9;

            public const double MIN_MAJORITY_COMP = 0.85;
            public const double MAX_MAJORITY_COMP = 0.91;

            public const double MINOR_FRACTION = 0.02;
            
            public const double MIN_CLOUD_COVER = 0.1;
            public const double MAX_CLOUD_COVER = 0.7;

            public const double MIN_SURFACE_PRESSURE =   0.001;
            public const double MAX_SURFACE_PRESSURE = 100.0;

            public static readonly Atmosphere.MajorClass JOTUNNIAN = new Atmosphere.MajorClass(
                ID.Atmo.MJR_JOTUNNIAN,
                "Jotunnian", "hydrogen and helium",
                false, true, false,
                -1.0,
                1.0,
                1.0,
                new Atmosphere.Component[]{ Comps.HYDROGEN },
                new int                 []{ 1              },
            
                new Atmosphere.Component[]{ Comps.HELIUM   },
                new int                 []{ 1              },
            
                new Atmosphere.Component[]{ Comps.METHANE, Comps.METHYLENE, Comps.ETHANE   , Comps.ETHYLENE   , Comps.ACETYLENE , Comps.DIACETYLENE, Comps.PROPANE   , Comps.PROPYNE     },
                new int                 []{ 2            , 1              , 2              , 1                , 1               , 2                , 1               , 2                 },
            
                new Atmosphere.Component[]{ Comps.METHANE, Comps.METHYLENE, Comps.ETHANE   , Comps.ETHYLENE   , Comps.ACETYLENE , Comps.DIACETYLENE, Comps.PROPANE   , Comps.PROPYNE   ,
                                            Comps.WATER  , Comps.H_SULFIDE, Comps.AMMONIA  , Comps.H_CYANIDE  , Comps.PHOSPHINE , Comps.SILANE     , Comps.H_FLUORIDE, Comps.H_CHLORIDE, 
                                            Comps.N_OXIDE, Comps.N_DIOXIDE, Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.CARBONYL_S, Comps.C_MONOXIDE , Comps.C_DIOXIDE , Comps.CYANOGEN  ,
                                            Comps.AMMONIA, Comps.PHOSPHINE, Comps.NITROGEN , Comps.THOLINS                                                                               },
                new int                 []{ 1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            56           , 56             , 56             , 56                                                                                          }
            );

            public static readonly Atmosphere.MajorClass HELIAN = new Atmosphere.MajorClass(
                ID.Atmo.MJR_HELIAN,
                "Helian", "helium",
                true, false, false,
                300.0,
                0.001,
                1.000,
                new Atmosphere.Component[]{ Comps.HELIUM },
                new int                 []{ 1            },
            
                new Atmosphere.Component[]{ Comps.METHANE , Comps.METHYLENE, Comps.ETHANE   , Comps.ETHYLENE   , Comps.ACETYLENE , Comps.DIACETYLENE, Comps.PROPANE   , Comps.PROPYNE   ,
                                            Comps.N_OXIDE , Comps.N_DIOXIDE, Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.CARBONYL_S, Comps.C_MONOXIDE , Comps.C_DIOXIDE , Comps.CYANOGEN  ,
                                            Comps.NITROGEN, Comps.NEON                                                                                                                    },
                new int                 []{ 1             , 1              , 1              , 1                , 1               , 1                , 1               , 1               ,
                                            1             , 1              , 1              , 1                , 1               , 1                , 1               , 1               ,
                                            32            , 32                                                                                                                            },
            
                new Atmosphere.Component[]{ Comps.ARGON },
                new int                 []{ 1           },
            
                new Atmosphere.Component[]{ Comps.METHANE, Comps.METHYLENE, Comps.ETHANE   , Comps.ETHYLENE   , Comps.ACETYLENE , Comps.DIACETYLENE, Comps.PROPANE   , Comps.PROPYNE   ,
                                            Comps.WATER  , Comps.H_SULFIDE, Comps.AMMONIA  , Comps.H_CYANIDE  , Comps.PHOSPHINE , Comps.SILANE     , Comps.H_FLUORIDE, Comps.H_CHLORIDE, 
                                            Comps.N_OXIDE, Comps.N_DIOXIDE, Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.CARBONYL_S, Comps.C_MONOXIDE , Comps.C_DIOXIDE , Comps.CYANOGEN  ,
                                            Comps.NEON   , Comps.ARGON    , Comps.KRYPTON  , Comps.NITROGEN   , Comps.OXYGEN                                                           },
                new int                 []{ 1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            2            , 2              , 2              , 2                , 2               , 2                , 2               , 2               , 
                                            11           , 11             , 11             , 56               , 56                                                                        }
            );

            public static readonly Atmosphere.MajorClass YDATRIAN = new Atmosphere.MajorClass(
                ID.Atmo.MJR_YDATRIAN,
                "Ydatrian", "simple hydride compounds",
                true, false, false,
                300.0,
                0.001,
                10.0,
                new Atmosphere.Component[]{ Comps.WATER   , Comps.H_SULFIDE, Comps.AMMONIA  , Comps.H_CYANIDE  , Comps.METHANE, Comps.METHYLENE },
                new int                 []{ 3             , 2              , 3              , 2                , 3            , 1               },
            
                new Atmosphere.Component[]{ Comps.WATER   , Comps.H_SULFIDE, Comps.AMMONIA  , Comps.H_CYANIDE  , Comps.METHANE, Comps.METHYLENE },
                new int                 []{ 3             , 2              , 3              , 2                , 3            , 1               },
            
                new Atmosphere.Component[]{ Comps.ARGON },
                new int                 []{ 1           },
            
                new Atmosphere.Component[]{ Comps.METHANE, Comps.METHYLENE, Comps.ETHANE   , Comps.ETHYLENE   , Comps.ACETYLENE , Comps.DIACETYLENE, Comps.PROPANE   , Comps.PROPYNE   ,
                                            Comps.WATER  , Comps.H_SULFIDE, Comps.AMMONIA  , Comps.H_CYANIDE  , Comps.PHOSPHINE , Comps.SILANE     , Comps.H_FLUORIDE, Comps.H_CHLORIDE, 
                                            Comps.N_OXIDE, Comps.N_DIOXIDE, Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.CARBONYL_S, Comps.C_MONOXIDE , Comps.C_DIOXIDE , Comps.CYANOGEN  ,
                                            Comps.NEON   , Comps.ARGON    , Comps.KRYPTON  , Comps.NITROGEN   , Comps.OXYGEN                                                             },
                new int                 []{ 1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            2            , 2              , 2              , 2                , 2               , 2                , 2               , 2               , 
                                            11           , 11             , 11             , 56               , 56                                                                       }
            );

            public static readonly Atmosphere.MajorClass RHEAN = new Atmosphere.MajorClass(
                ID.Atmo.MJR_RHEAN,
                "Rhean", "nitrogen",
                true, false, true,
                550.0,
                0.001,
                100.0,
                new Atmosphere.Component[]{ Comps.NITROGEN },
                new int                 []{ 1              },

                new Atmosphere.Component[]{ Comps.METHANE  , Comps.METHYLENE  , Comps.ETHANE    , Comps.ETHYLENE   , Comps.ACETYLENE , Comps.DIACETYLENE, Comps.N_OXIDE   , Comps.N_DIOXIDE,
                                            Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.CARBONYL_S, Comps.C_MONOXIDE , Comps.C_DIOXIDE , Comps.CYANOGEN                                        },
                new int                 []{ 1              , 1                , 1               , 1                , 1               , 1                , 1               , 1              ,
                                            1              , 1                , 1               , 1                , 1               , 1                                                     },

                new Atmosphere.Component[]{ Comps.ARGON },
                new int                 []{ 1           },

                new Atmosphere.Component[]{ Comps.METHANE, Comps.METHYLENE, Comps.ETHANE   , Comps.ETHYLENE   , Comps.ACETYLENE , Comps.DIACETYLENE, Comps.PROPANE   , Comps.PROPYNE   ,
                                            Comps.WATER  , Comps.H_SULFIDE, Comps.AMMONIA  , Comps.H_CYANIDE  , Comps.PHOSPHINE , Comps.SILANE     , Comps.H_FLUORIDE, Comps.H_CHLORIDE, 
                                            Comps.N_OXIDE, Comps.N_DIOXIDE, Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.CARBONYL_S, Comps.C_MONOXIDE , Comps.C_DIOXIDE , Comps.CYANOGEN  ,
                                            Comps.NEON   , Comps.ARGON    , Comps.KRYPTON  , Comps.NITROGEN   , Comps.OXYGEN                                                             },
                new int                 []{ 1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            2            , 2              , 2              , 2                , 2               , 2                , 2               , 2               , 
                                            11           , 11             , 11             , 56               , 56                                                                       }
            );

            public static readonly Atmosphere.MajorClass MINERVAN = new Atmosphere.MajorClass(
                ID.Atmo.MJR_MINERVAN,
                "Minervan", "compounds of nonmetals",
                true, false, false,
                -1.0,
                0.001,
                100.0,
                new Atmosphere.Component[]{ Comps.N_OXIDE, Comps.N_DIOXIDE, Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.C_MONOXIDE , Comps.C_DIOXIDE },
                new int                 []{ 1            , 2              , 2              , 1                , 1                , 2               },
            
                new Atmosphere.Component[]{ Comps.N_OXIDE, Comps.N_DIOXIDE, Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.CARBONYL_S, Comps.C_MONOXIDE , Comps.C_DIOXIDE , Comps.CYANOGEN },
                new int                 []{ 1            , 2              , 2              , 1                , 1               , 1                , 2               , 1              },
            
                new Atmosphere.Component[]{ Comps.ARGON },
                new int                 []{ 1           },
            
                new Atmosphere.Component[]{ Comps.METHANE, Comps.METHYLENE, Comps.ETHANE   , Comps.ETHYLENE   , Comps.ACETYLENE , Comps.DIACETYLENE, Comps.PROPANE   , Comps.PROPYNE   ,
                                            Comps.WATER  , Comps.H_SULFIDE, Comps.AMMONIA  , Comps.H_CYANIDE  , Comps.PHOSPHINE , Comps.SILANE     , Comps.H_FLUORIDE, Comps.H_CHLORIDE, 
                                            Comps.N_OXIDE, Comps.N_DIOXIDE, Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.CARBONYL_S, Comps.C_MONOXIDE , Comps.C_DIOXIDE , Comps.CYANOGEN  ,
                                            Comps.NEON   , Comps.ARGON    , Comps.KRYPTON  , Comps.NITROGEN   , Comps.OXYGEN                                                             },
                new int                 []{ 1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            2            , 2              , 2              , 2                , 2               , 2                , 2               , 2               , 
                                            11           , 11             , 11             , 56               , 56                                                                       }
            );

            public static readonly Atmosphere.MajorClass EDELIAN = new Atmosphere.MajorClass(
                ID.Atmo.MJR_EDELIAN,
                "Edelian", "neon and argon",
                true, false, false,
                300.0,
                0.001,
                1.000,
                new Atmosphere.Component[]{ Comps.NEON, Comps.ARGON },
                new int                 []{ 1         , 1           },
            
                new Atmosphere.Component[]{ Comps.NEON, Comps.ARGON },
                new int                 []{ 1         , 1           },
            
                new Atmosphere.Component[]{ Comps.WATER   , Comps.H_SULFIDE, Comps.AMMONIA  , Comps.H_CYANIDE  , Comps.PHOSPHINE , Comps.SILANE     , Comps.H_FLUORIDE, Comps.H_CHLORIDE,
                                            Comps.N_OXIDE , Comps.N_DIOXIDE, Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.CARBONYL_S, Comps.C_MONOXIDE , Comps.C_DIOXIDE , Comps.CYANOGEN  ,
                                            Comps.NITROGEN                                                                                                                                },
                new int                 []{ 1             , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            1             , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            24                                                                                                                                            },
            
                new Atmosphere.Component[]{ Comps.METHANE, Comps.METHYLENE, Comps.ETHANE   , Comps.ETHYLENE   , Comps.ACETYLENE , Comps.DIACETYLENE, Comps.PROPANE   , Comps.PROPYNE   ,
                                            Comps.WATER  , Comps.H_SULFIDE, Comps.AMMONIA  , Comps.H_CYANIDE  , Comps.PHOSPHINE , Comps.SILANE     , Comps.H_FLUORIDE, Comps.H_CHLORIDE, 
                                            Comps.N_OXIDE, Comps.N_DIOXIDE, Comps.S_DIOXIDE, Comps.C_DISULFIDE, Comps.CARBONYL_S, Comps.C_MONOXIDE , Comps.C_DIOXIDE , Comps.CYANOGEN  ,
                                            Comps.NEON   , Comps.ARGON    , Comps.KRYPTON  , Comps.NITROGEN   , Comps.OXYGEN                                                             },
                new int                 []{ 1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            1            , 1              , 1              , 1                , 1               , 1                , 1               , 1               , 
                                            2            , 2              , 2              , 2                , 2               , 2                , 2               , 2               , 
                                            21           , 21             , 21             , 56               , 56                                                                       }
            );
        
            public static readonly Atmosphere.MajorClass[] MAJOR_CLASSES = new Atmosphere.MajorClass[]{ JOTUNNIAN, HELIAN, YDATRIAN, RHEAN, MINERVAN, EDELIAN };
            public static readonly int[]                   MAJOR_WEIGHTS = new int                  []{         1,      1,        1,     1,        1,       1 };        

            public static readonly Atmosphere.MinorClass CRYOAZURIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_CRYOAZURIAN, "Cryoazurian", "a cryoazuri",
                "The sky is {0}, with thin, hydrocarbon-based hazes.",
                0.0, 90.0,
                0.0,  0.2,
                0.0,  0.0,
                new string[] { "dull blue"       ,
                               "dull cyan"       ,
                               "sand blue"       ,
                               "steel blue"      ,
                               "slate gray"      ,
                               "grayish-blue"    ,
                               "cornflower-blue" },
                null
            );

            public static readonly Atmosphere.MinorClass FRIGIDIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_FRIGIDIAN, "Frigidian", "a frigi",
                "The sky is {0}, with {1} clouds of condensed hydrogen.",
                5.0, 20.0,
                0.1,  0.7,
                0.0,  0.3,
                new string[] { "gray"            ,
                               "grayish-blue"    ,
                               "slate gray"      ,
                               "pewter brown"    },
                new string[] { "light gray"      ,
                               "white"           ,
                               "washed-out blue" }
            );

            public static readonly Atmosphere.MinorClass NEONEAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_NEONEAN, "Neonean", "a neono",
                "The sky is {0}, with {1} clouds of neon.",
                15.0, 35.0,
                 0.1,  0.7,
                 0.0,  0.3,
                new string[] { "pink"            ,
                               "pale pink"       ,
                               "primrose"        ,
                               "light pink"      },
                new string[] { "light gray"      ,
                               "white"           ,
                               "washed-out pink" }
            );

            public static readonly Atmosphere.MinorClass BOREAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_BOREAN, "Borean", "a boreo",
                "Hazes have made the sky {0}, with {1} clouds of nitrogen and carbon monoxide.",
                35.0, 60.0,
                 0.1,  0.7,
                 0.0,  0.3,
                new string[] { "pink"                   ,
                               "pale pink"              ,
                               "primrose"               ,
                               "pale purple"            ,
                               "magenta"                ,
                               "peach"                  ,
                               "burnt orange"           },
                new string[] { "gray"                   ,
                               "white"                  ,
                               "washed-out pink"        ,
                               "barely-distringuishable",
                               "washed-out pink"        ,
                               "pale orange"            ,
                               "tan"                    }
            );

            public static readonly Atmosphere.MinorClass METHANEAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_METHANEAN, "Methanean", "a metho",
                "The sky is {0}, with {1} hazes of organic chemicals.",
                60.00, 90.00,
                 0.25,  0.85,
                 0.10,  0.40,
                new string[] { "cyan"       ,
                               "turquoise"  ,
                               "aqua"       ,
                               "teal"       ,
                               "pale blue"  ,
                               "light blue" ,
                               "blue-green" ,
                               "dull green" },
                new string[] { "white"      ,
                               "pale blue"  ,
                               "pale green" }
            );

            public static readonly Atmosphere.MinorClass MESOAZURIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_MESOAZURIAN, "Mesoazurian", "a mesoazuri",
                "The sky is a clear {0}, with slight {1} hazes.",
                90.00, 550.00,
                 0.15,   0.70,
                 0.00,   0.10,
                new string[] { "azure"      ,
                               "steel blue" ,
                               "teal"       ,
                               "smalt"      ,
                               "blue-green" ,
                               "turquoise"  ,
                               "blue"       },
                new string[] { "teal"       ,
                               "smalt"      ,
                               "turqoise"   ,
                               "pale blue"  ,
                               "pale green" }
            );

            public static readonly Atmosphere.MinorClass THOLIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_THOLIAN, "Tholian", "a tholi",
                "The sky is {0}, with {1} hazes of hydrocarbons and organosulfurs.",
                60.00, 550.00,
                 0.50,   1.00,
                 0.25,   0.85,
                new string[] { "pale yellow" ,
                               "pale orange" ,
                               "yellow"      ,
                               "orange"      ,
                               "peach"       ,
                               "burnt orange",
                               "brown"       },
                new string[] { "gray"        ,
                               "tan"         ,
                               "pale yellow" ,
                               "pale brown"  }
            );

            public static readonly Atmosphere.MinorClass SULFANIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_SULFANIAN, "Sulfanian", "a sulfa",
                "The sky is {0}, with {1} clouds of hydrogen and ammonium sulfide.",
                80.00, 180.00,
                 0.10,   0.70,
                 0.00,   0.30,
                new string[] { "pale yellow" ,
                               "pale orange" ,
                               "yellow"      ,
                               "orange"      ,
                               "dull yellow" ,
                               "gold"        ,
                               "tan"         },
                new string[] { "gray"        ,
                               "white"       ,
                               "pale yellow" ,
                               "pale tan"    }
            );

            public static readonly Atmosphere.MinorClass AMMONIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_AMMONIAN, "Ammonian", "an ammo",
                "The sky is {0}, with {1} clouds of ammonia and hydrogen sulfde.",
                80.00, 190.00,
                 0.10,   0.70,
                 0.00,   0.30,
                new string[] { "orange"      ,
                               "pale orange" ,
                               "peach"       ,
                               "burnt orange",
                               "brown"       ,
                               "dark gray"   ,
                               "dark tan"    },
                new string[] { "gray"        ,
                               "white"       ,
                               "pale yellow" ,
                               "pale brown"  }
            );
        
            public static readonly Atmosphere.MinorClass HYDRONIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_HYDRONIAN, "Hydronian", "a hydro",
                "The sky is {0}, with {1} clouds of water vapor.",
                170.00, 350.00,
                  0.10,   0.70,
                  0.00,   0.30,
                new string[] { "white"       ,
                               "light gray"  ,
                               "gray"        ,
                               "pale yellow" ,
                               "pale orange" ,
                               "pale green"  },
                new string[] { "light gray"  ,
                               "white"       ,
                               "gray"        ,
                               "pale yellow" }
            );
        
            public static readonly Atmosphere.MinorClass ACIDIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_ACIDIAN, "Acidian", "an acidi",
                "The sky is {0}, with {1} clouds of sulfuric acid.",
                250.00, 500.00,
                  0.10,   0.70,
                  0.00,   0.30,
                new string[] { "pale yellow" ,
                               "dull yellow" ,
                               "yellow"      ,
                               "peach"       ,
                               "tan"         ,
                               "light brown" ,
                               "beige"       },
                new string[] { "light gray"  ,
                               "white"       ,
                               "pale yellow" ,
                               "pale brown"  }
            );
        
            public static readonly Atmosphere.MinorClass PYROAZURIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_PYROAZURIAN, "Pyroazurian", "a pyroazuri",
                "The clouds on this planet are very thin, making the atmosphere {0}.",
                550.00, 1300.00,
                  0.00,    0.20,
                  0.00,    0.00,
                new string[] { "azure"       ,
                               "blue"        ,
                               "deep blue"   ,
                               "dark blue"   ,
                               "cobalt blue" ,
                               "smalt"       },
                null
            );
        
            public static readonly Atmosphere.MinorClass SULFOLIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_SULFOLIAN, "Sulfolian", "a sulfoli",
                "The sky is {0}, with sulfurous {1} clouds.",
                400.00, 1000.00,
                  0.10,    0.70,
                  0.00,    0.30,
                new string[] { "yellow"       ,
                               "yellow-green" ,
                               "light green"  ,
                               "bronze"       ,
                               "gold"         ,
                               "dull yellow"  ,
                               "dull green"   },
                new string[] { "tan"          ,
                               "dull green"   ,
                               "pale yellow"  ,
                               "pale green"   }
            );
        
            public static readonly Atmosphere.MinorClass AITHALIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_AITHALIAN, "Aithalian", "an aithali",
                "The sky is {0}, with carbonaceous {1} clouds.",
                550.00, 1000.00,
                  0.10,    0.70,
                  0.00,    0.30,
                new string[] { "brown"        ,
                               "dark brown"   ,
                               "dark gray"    ,
                               "hazel"        ,
                               "walnut"       ,
                               "burnt orange" ,
                               "olive"        },
                new string[] { "gray"         ,
                               "light brown"  ,
                               "pale brown"   }
            );

            public static readonly Atmosphere.MinorClass ALKALINEAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_ALKALINEAN, "Alkalinean", "an alkali",
                "The sky is {0}, with {1} clouds of alkaline chlorides.",
                700.00, 850.00,
                  0.50,   0.70,
                  0.30,   0.60,
                new string[] { "brown"        ,
                               "dark brown"   ,
                               "dark green"   ,
                               "hazel"        ,
                               "walnut"       ,
                               "olive"        ,
                               "dark olive"   },
                new string[] { "brown"        ,
                               "burnt orange" ,
                               "olive"        }
            );

            public static readonly Atmosphere.MinorClass HYPERPYROAZURIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_HYPERPYRO, "Hyperpyroazurian", "a hyperpyro",
                "On the day-side, the sky is {0}, but the night-side is visibly glowing from the extreme temperature.",
                1300.00,   -1.00,
                   0.00,    0.20,
                   0.00,    0.00,
                new string[] { "azure"       ,
                               "blue"        ,
                               "deep blue"   ,
                               "dark blue"   ,
                               "cobalt blue" ,
                               "smalt"       },
                null
            );

            public static readonly Atmosphere.MinorClass ENSTATIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_ENSTATIAN, "Enstatian", "an enstato",
                "On the day-side, the sky is {0}; but the night-side is visibly glowing from the extreme temperature, and common solids like silicon, magnesium and iron compounds have vaporized to form {1} cloud decks.",
                1300.00, 1900.00,
                   0.10,    0.70,
                   0.00,    0.30,
                new string[] { "gray"        ,
                               "dark gray"   ,
                               "dull blue"   ,
                               "dull green"  ,
                               "pale brown"  ,
                               "pewter brown"},
                new string[] { "light gray"  ,
                               "pale blue"   ,
                               "slate gray"  ,
                               "light brown" }
            );

            public static readonly Atmosphere.MinorClass REFRACTIAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_REFRACTIAN, "Refractian", "a refra",
                "On the day-side, the sky is {0}; but the night-side is visibly glowing from the extreme temperature, and oxides of aluminum, titanium, and calcium have vaporized to form {1} cloud decks.",
                1800.00, 2300.00,
                   0.10,    0.70,
                   0.00,    0.30,
                new string[] { "orange"      ,
                               "burnt orange",
                               "bronze"      ,
                               "dark tan"    ,
                               "beige"       },
                new string[] { "pale orange" ,
                               "peach"       ,
                               "tan"         ,
                               "beige"       ,
                               "light brown" }
            );

            public static readonly Atmosphere.MinorClass CARBEAN = new Atmosphere.MinorClass(
                ID.Atmo.MNR_CARBEAN, "Carbean", "a carbo",
                "On the day-side, the sky is {0}; but the night-side is visibly glowing from the extreme temperature, and silicon, titanium, and vanadium carbides have vaporized to form {1} cloud decks.",
                2000.00, 2900.00,
                   0.50,    0.70,
                   0.30,    0.60,
                new string[] { "brown"        ,
                               "dark brown"   ,
                               "dark green"   ,
                               "hazel"        ,
                               "walnut"       ,
                               "olive"        ,
                               "dark olive"   },
                new string[] { "brown"        ,
                               "burnt orange" ,
                               "olive"        }
            );

            public static readonly Atmosphere.MinorClass[] MINOR_CLASSES = new Atmosphere.MinorClass[]{ CRYOAZURIAN, FRIGIDIAN, NEONEAN   , BOREAN          , METHANEAN, MESOAZURIAN,
                                                                                                        THOLIAN    , SULFANIAN, AMMONIAN  , HYDRONIAN       , ACIDIAN  , PYROAZURIAN,
                                                                                                        SULFOLIAN  , AITHALIAN, ALKALINEAN, HYPERPYROAZURIAN, ENSTATIAN, REFRACTIAN ,
                                                                                                        CARBEAN                                                                     };
            public static readonly int[]                   MINOR_WEIGHTS = new int                  []{           1,         1,          1,                1,         1,           1,
                                                                                                                  1,         1,          1,                1,         1,           1,
                                                                                                                  1,         1,          1,                1,         1,           1,
                                                                                                                  1                                                                 };

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

            public const char MNR_CRYOAZURIAN = 'R'; //Cold, dull blue
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
            public const char MNR_ALKALINEAN  = 'K'; //Hot, green-brown

            public const char MNR_HYPERPYRO   = 'p'; //Glowing, blue
            public const char MNR_ENSTATIAN   = 'e'; //Glowing, grey
            public const char MNR_REFRACTIAN  = 'r'; //Glowing, orange
            public const char MNR_CARBEAN     = 'c'; //Glowing, brown
        }
    }

    public class Comps
    {
        public static readonly Atmosphere.Component HYDROGEN    = new Atmosphere.Component("Hydrogen"         , 0.00201588, Color.HYDROGEN, new bool[]{ true , false, false, false, false, false });
        public static readonly Atmosphere.Component HELIUM      = new Atmosphere.Component("Helium"           , 0.0040026 , Color.HYDROGEN, new bool[]{ false, false, false, false, false, false });
        
        public static readonly Atmosphere.Component NITROGEN    = new Atmosphere.Component("Nitrogen"         , 0.028014  , Color.NITROGEN, new bool[]{ false, true , false, false, false, false });
        public static readonly Atmosphere.Component AMMONIA     = new Atmosphere.Component("Ammonia"          , 0.017031  , Color.NITROGEN, new bool[]{ true , true , false, false, false, false });
        public static readonly Atmosphere.Component H_CYANIDE   = new Atmosphere.Component("Hydrogen Cyanide" , 0.027026  , Color.NITROGEN, new bool[]{ true , true , false, false, false, false });
        public static readonly Atmosphere.Component N_OXIDE     = new Atmosphere.Component("Nitric Oxide"     , 0.030006  , Color.NITROGEN, new bool[]{ false, true , true , false, false, false });
        public static readonly Atmosphere.Component N_DIOXIDE   = new Atmosphere.Component("Nitrogen Dioxide" , 0.046005  , Color.NITROGEN, new bool[]{ false, true , true , false, false, false });
        public static readonly Atmosphere.Component CYANOGEN    = new Atmosphere.Component("Cyanogen"         , 0.052036  , Color.NITROGEN, new bool[]{ false, true , false, true , false, false });
         
        public static readonly Atmosphere.Component C_MONOXIDE  = new Atmosphere.Component("Carbon Monoxide"  , 0.02801   , Color.CARBON  , new bool[]{ false, false, true , true , false, false });
        public static readonly Atmosphere.Component C_DIOXIDE   = new Atmosphere.Component("Carbon Dioxide"   , 0.044009  , Color.CARBON  , new bool[]{ false, false, true , true , false, false });
        public static readonly Atmosphere.Component C_DISULFIDE = new Atmosphere.Component("Carbon Disulfide" , 0.07613   , Color.CARBON  , new bool[]{ false, false, false, true , true , false });
        public static readonly Atmosphere.Component METHANE     = new Atmosphere.Component("Methane"          , 0.01604   , Color.CARBON  , new bool[]{ true , false, false, true , false, false });
        public static readonly Atmosphere.Component METHYLENE   = new Atmosphere.Component("Methylene"        , 0.0140266 , Color.CARBON  , new bool[]{ true , false, false, true , false, false });
        public static readonly Atmosphere.Component ETHANE      = new Atmosphere.Component("Ethane"           , 0.03007   , Color.CARBON  , new bool[]{ true , false, false, true , false, false });
        public static readonly Atmosphere.Component ETHYLENE    = new Atmosphere.Component("Ethylene"         , 0.028054  , Color.CARBON  , new bool[]{ true , false, false, true , false, false });
        public static readonly Atmosphere.Component ACETYLENE   = new Atmosphere.Component("Acetylene"        , 0.026038  , Color.CARBON  , new bool[]{ false, false, false, true , false, false });
        public static readonly Atmosphere.Component DIACETYLENE = new Atmosphere.Component("Diacetylene"      , 0.05006   , Color.CARBON  , new bool[]{ false, false, false, true , false, false });
        public static readonly Atmosphere.Component PROPANE     = new Atmosphere.Component("Propane"          , 0.0441    , Color.CARBON  , new bool[]{ false, false, false, true , false, false });
        public static readonly Atmosphere.Component PROPYNE     = new Atmosphere.Component("Propyne"          , 0.040065  , Color.CARBON  , new bool[]{ false, false, false, true , false, false });
        public static readonly Atmosphere.Component THOLINS     = new Atmosphere.Component("Tholins"          , 0.0       , Color.CARBON  , new bool[]{ false, false, false, true , false, false });
        
        public static readonly Atmosphere.Component H_SULFIDE   = new Atmosphere.Component("Hydrogen Sulfide" , 0.03408   , Color.SULFUR  , new bool[]{ true , false, false, false, true , false });
        public static readonly Atmosphere.Component S_DIOXIDE   = new Atmosphere.Component("Sulfur Dioxide"   , 0.06406   , Color.SULFUR  , new bool[]{ false, false, true , false, true , false });
        public static readonly Atmosphere.Component CARBONYL_S  = new Atmosphere.Component("Carbonyl Sulfide" , 0.06007   , Color.SULFUR  , new bool[]{ false, false, true , false, true , false });

        public static readonly Atmosphere.Component OXYGEN      = new Atmosphere.Component("Oxygen"           , 0.031998  , Color.OXYGEN  , new bool[]{ false, false, true , false, false, false });
        public static readonly Atmosphere.Component WATER       = new Atmosphere.Component("Water"            , 0.018015  , Color.WATER   , new bool[]{ true , false, true , false, false, false });
        public static readonly Atmosphere.Component PHOSPHINE   = new Atmosphere.Component("Phosphine"        , 0.033998  , Color.PHOSPHOR, new bool[]{ true , false, false, false, false, true  });
        public static readonly Atmosphere.Component SILANE      = new Atmosphere.Component("Silane"           , 0.032117  , Color.SILICON , new bool[]{ true , false, false, false, false, true  });
        public static readonly Atmosphere.Component H_FLUORIDE  = new Atmosphere.Component("Hydrogen Fluoride", 0.020006  , Color.FLUORINE, new bool[]{ true , false, false, false, false, true  });
        public static readonly Atmosphere.Component H_CHLORIDE  = new Atmosphere.Component("Hydrogen Chloride", 0.03646   , Color.CHLORINE, new bool[]{ true , false, false, false, false, true  });

        public static readonly Atmosphere.Component NEON        = new Atmosphere.Component("Neon"             , 0.0201797 , Color.NEON    , new bool[]{ false, false, false, false, false, false });
        public static readonly Atmosphere.Component ARGON       = new Atmosphere.Component("Argon"            , 0.039948  , Color.ARGON   , new bool[]{ false, false, false, false, false, false });
        public static readonly Atmosphere.Component KRYPTON     = new Atmosphere.Component("Krypton"          , 0.083798  , Color.KRYPTON , new bool[]{ false, false, false, false, false, false });

        
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

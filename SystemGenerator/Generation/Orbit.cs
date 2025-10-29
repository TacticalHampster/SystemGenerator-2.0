using Accessibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SystemGenerator.Generation
{
    public class Orbit
    {
        //Properties
        public double a      ; //Semimajor axis              (AU)
        public double e      ; //Eccentricity                (dimensionless)
        public double i      ; //Inclination                 (degrees)
        public double h      ; //Height (for asteroid belts) (degrees)
        public double l      ; //Longitude of Ascending node (degrees)
        public double p      ; //Argument of Periapsis       (degrees)
        public double y      ; //Year length                 (Earth years)
        public double v      ; //Orbital velocity            (km/s)
        
        //Standard deviations for the above values (asteroid belts and c-type moon groups only)
        public double aSigma;
        public double eSigma; 
        public double iSigma; 
        public double lSigma; 
        public double pSigma; 

        //Sets the orbital parameters based on the planet type
        public void genOrbit(Star star, Planet planet, double dist)
        {
            Utils.writeLog("        Generating orbital parameters");

            //Calculate lambda
            double lambda = 0.584 * Math.Pow(2, -1.2);
            for (int i = 0; i < star.orbits.Count; i++)
            {
                if (star.orbits[i] == dist)
                {
                    lambda = 0.584 * Math.Pow(i, -1.2);
                    break;
                }
            }

            //Generate the params
            this.a = dist;
                        
            switch (planet.type)
            {
                case ID.Planet.ROCK_DENSE:
                case ID.Planet.ROCK_DESERT:
                case ID.Planet.WATER_OCEAN:
                    this.e = Utils.randDouble(Gen.Orbit.Terrestrial.MIN_ECCENTRICITY, Gen.Orbit.Terrestrial.MAX_ECCENTRICITY);
                    this.y = Math.Sqrt(Math.Pow(this.a, 3.0) / star.m);
                    this.v = Math.Sqrt(star.m / this.a) * Const.Earth.ORBV;
                    this.i = Utils.randDouble(Gen.Orbit.Terrestrial.MIN_INCLINATION, Gen.Orbit.Terrestrial.MAX_INCLINATION) * Utils.randSign();
                    this.l = Utils.randDouble(0.0, 360.0);
                    this.p = Utils.randDouble(0.0, 360.0);
                    break;

                case ID.Planet.ROCK_GREEN:
                case ID.Planet.WATER_GREEN:
                    this.e = Utils.randDouble(Gen.Orbit.Terrestrial.MIN_ECCENTRICITY, Gen.Orbit.Terrestrial.MAX_ECCENTRICITY);
                    this.y = Math.Sqrt(Math.Pow(this.a, 3.0) / star.m);
                    this.v = Math.Sqrt(star.m / this.a) * Const.Earth.ORBV;
                    this.i =  0.0;
                    this.l = 90.0;
                    this.p = -1.0;
                    break;

                case ID.Planet.GAS_GIANT:
                case ID.Planet.GAS_SUPER:
                case ID.Planet.GAS_PUFFY:
                case ID.Planet.GAS_HOT:
                case ID.Planet.ICE_DWARF:
                case ID.Planet.ICE_GIANT:
                    this.e = Utils.randDouble(Gen.Orbit.Giant.MIN_ECCENTRICITY, Gen.Orbit.Giant.MAX_ECCENTRICITY);
                    this.y = Math.Sqrt(Math.Pow(this.a, 3.0) / star.m);
                    this.v = Math.Sqrt(star.m / this.a) * Const.Earth.ORBV;
                    this.i = Utils.randDouble(Gen.Orbit.Giant.MIN_INCLINATION, Gen.Orbit.Giant.MAX_INCLINATION) * Utils.randSign();
                    this.l = Utils.randDouble(0.0, 360.0);
                    this.p = Utils.randDouble(0.0, 360.0);
                    break;

                case ID.Belt.BELT_KUIPER:
                    this.a = (Utils.resonance(dist, 3.0/2.0) + (dist*1.3)) / 2.0;
                    this.e = Utils.randDouble(Gen.Orbit.Belt.MIN_BELT_ECCENTRICITY, Gen.Orbit.Belt.MAX_BELT_ECCENTRICITY);
                    this.y = Math.Sqrt(Math.Pow(this.a, 3.0) / star.m);
                    this.v = Math.Sqrt(star.m / this.a) * Const.Earth.ORBV;
                    this.i = Utils.randDouble(Gen.Orbit.Belt.MIN_INCLINATION, Gen.Orbit.Belt.MAX_INCLINATION) * Utils.randSign();
                    this.h = Utils.randDouble(Gen.Orbit.Belt.MIN_KUIPER_HEIGHT, Gen.Orbit.Belt.MAX_KUIPER_HEIGHT);
                    this.l = Utils.randDouble(0.0, 360.0);
                    this.p = Utils.randDouble(0.0, 360.0);

                    this.eSigma = Utils.getDistribution( Gen.Orbit.Belt.MIN_BELT_ECCENTRICITY, Gen.Orbit.Belt.MAX_BELT_ECCENTRICITY)[1];
                    this.iSigma = Utils.getDistribution(-Gen.Orbit.Belt.MAX_KUIPER_HEIGHT     , Gen.Orbit.Belt.MAX_KUIPER_HEIGHT     )[1];
                    this.lSigma = Utils.getDistribution(0.0, 360.0)[1];
                    this.pSigma = Utils.getDistribution(0.0, 360.0)[1];
                    break;

                case ID.Belt.BELT_INNER:
                    this.e = Utils.randDouble(Gen.Orbit.Belt.MIN_BELT_ECCENTRICITY, Gen.Orbit.Belt.MAX_BELT_ECCENTRICITY);
                    this.y = Math.Sqrt(Math.Pow(this.a, 3.0) / star.m);
                    this.v = Math.Sqrt(star.m / this.a) * Const.Earth.ORBV;
                    this.i = Utils.randDouble(Gen.Orbit.Belt.MIN_INCLINATION, Gen.Orbit.Belt.MAX_INCLINATION) * Utils.randSign();
                    this.h = Utils.randDouble(Gen.Orbit.Belt.MIN_INNER_HEIGHT, Gen.Orbit.Belt.MAX_INNER_HEIGHT);
                    this.l = Utils.randDouble(0.0, 360.0);
                    this.p = Utils.randDouble(0.0, 360.0);

                    this.eSigma = Utils.getDistribution( Gen.Orbit.Belt.MIN_BELT_ECCENTRICITY, Gen.Orbit.Belt.MAX_BELT_ECCENTRICITY)[1];
                    this.iSigma = Utils.getDistribution(-Gen.Orbit.Belt.MAX_INNER_HEIGHT     , Gen.Orbit.Belt.MAX_INNER_HEIGHT     )[1];
                    this.lSigma = Utils.getDistribution(0.0, 360.0)[1];
                    this.pSigma = Utils.getDistribution(0.0, 360.0)[1];
                    break;

                default:
                    break;
            }

            Utils.writeLog("        Orbital parameter generation complete");
        }

        //Overload of above for belt objects
        public void genOrbit(Star star, Planet planet, Planet belt)
        {
            Utils.writeLog("        Generating orbital parameters");

            switch (planet.type)
            {
                case ID.Belt.DWARF:
                    this.a = Utils.randNormal(belt.orbit.a, belt.orbit.aSigma);
                    this.e = Utils.randNormal(belt.orbit.e, belt.orbit.eSigma);
                    this.y = Math.Sqrt(Math.Pow(this.a, 3.0) / star.m);
                    this.v = Math.Sqrt(star.m / this.a) * Const.Earth.ORBV;
                    this.i = Utils.randNormal(belt.orbit.i, belt.orbit.iSigma);
                    this.l = Utils.randNormal(belt.orbit.l, belt.orbit.lSigma);
                    this.p = Utils.randNormal(belt.orbit.p, belt.orbit.pSigma);
                    break;

                case ID.Belt.SEDNOID:
                    this.a = Utils.randNormal((belt.orbit.a*50.0)+(belt.orbit.aSigma*3.0), belt.orbit.aSigma);
                    this.e = Utils.randNormal(Gen.Orbit.Belt.MIN_SEDNOID_ECCENTRICITY, Gen.Orbit.Belt.MAX_SEDNOID_ECCENTRICITY);
                    this.y = Math.Sqrt(Math.Pow(this.a, 3.0) / star.m);
                    this.v = Math.Sqrt(star.m / this.a) * Const.Earth.ORBV;
                    this.i = Utils.randNormal(belt.orbit.i, belt.orbit.iSigma);
                    this.l = Utils.randNormal(belt.orbit.l, belt.orbit.lSigma);
                    this.p = Utils.randNormal(belt.orbit.p, belt.orbit.pSigma);
                    break;

                case ID.Belt.PLUTINO:
                case ID.Belt.CUBEWANO:
                case ID.Belt.TWOTINO:
                case ID.Belt.SCATTERED:
                    this.a = Utils.randNormal(belt.orbit.a, belt.orbit.aSigma);
                    this.e = Utils.randNormal(belt.orbit.e, belt.orbit.eSigma);
                    this.y = Math.Sqrt(Math.Pow(this.a, 3.0) / star.m);
                    this.v = Math.Sqrt(star.m / this.a) * Const.Earth.ORBV;
                    this.i = Utils.randNormal(belt.orbit.i, belt.orbit.iSigma);
                    this.l = Utils.randNormal(belt.orbit.l, belt.orbit.lSigma);
                    this.p = Utils.randNormal(belt.orbit.p, belt.orbit.pSigma);
                    break;
                    
                default:
                    break;
            }

            Utils.writeLog("        Orbital parameter generation complete");
        }

        //Overload of the above for satellites
        public void genOrbit(Star star, Moon moon, Planet host, double dist)
        {
            Utils.writeLog("                    Generating orbital parameters");

            if (host.isGiant)
            {
                this.a = dist;
                this.e = Utils.randDouble(Gen.Orbit.Sat.MIN_GIANT_ECCENTRICITY, Gen.Orbit.Sat.MAX_GIANT_ECCENTRICITY);
                this.i = Utils.randDouble(Gen.Orbit.Sat.MIN_GIANT_INCLINATION, Gen.Orbit.Sat.MAX_GIANT_INCLINATION) * Utils.randSign();
            }
            else if (host.isDwarf)
            {
                if (moon.isMajor)
                {
                    double hill_max = host.orbit.a * Math.Pow(host.m / star.m, (1.0/3.0)) * 235.0;
                    double hill_min = 2.44 * host.r * Math.Pow( host.bulkDensity / moon.bulkDensity, (1.0/3.0) );

                    if (moon.isMajor)
                        this.a = Utils.randDouble(hill_min, hill_max/2.0);
                    else
                        this.a = Utils.randDouble(hill_min, hill_max);

                    this.e = Utils.randDouble(Gen.Orbit.Sat.MIN_DWARF_ECCENTRICITY, Gen.Orbit.Sat.MAX_DWARF_ECCENTRICITY);
                    this.i = Utils.randDouble(Gen.Orbit.Sat.MIN_DWARF_INCLINATION, Gen.Orbit.Sat.MAX_DWARF_ECCENTRICITY);
                }
                else
                {
                    this.a = dist;
                    this.e = Utils.randDouble(Gen.Orbit.Sat.MIN_DWARF_ECCENTRICITY, Gen.Orbit.Sat.MAX_DWARF_ECCENTRICITY);
                    this.i = Utils.randDouble(Gen.Orbit.Sat.MIN_DWARF_INCLINATION, Gen.Orbit.Sat.MAX_DWARF_ECCENTRICITY);
                }
            }
            else
            {
                double hill_max = host.orbit.a * Math.Pow(host.m / star.m, (1.0/3.0)) * 235.0;
                double hill_min = 2.44 * host.r * Math.Pow( host.bulkDensity / moon.bulkDensity, (1.0/3.0) );

                if (moon.isMajor)
                    this.a = Utils.randDouble(hill_min, hill_max/2.0);
                else
                    this.a = Utils.randDouble(hill_min, hill_max);

                this.e = Utils.randDouble(Gen.Orbit.Sat.MIN_TERRES_ECCENTRICITY, Gen.Orbit.Sat.MAX_TERRES_ECCENTRICITY);
                this.i = Utils.randDouble(Gen.Orbit.Sat.MIN_TERRES_INCLINATION, Gen.Orbit.Sat.MAX_TERRES_ECCENTRICITY);
            }

            this.y = 0.0588 * Math.Sqrt( Math.Pow(this.a, 3.0) / (moon.m + host.m) );
            this.v = Math.Sqrt( host.m / moon.m ) * Const.Earth.ORBV;
            this.l = Utils.randDouble(0.0, 360.0);
            this.p = Utils.randDouble(0.0, 360.0);

            moon.turn = host.turn;

            Utils.writeLog("                    Orbital parameter generation complete");
        }
    }
}

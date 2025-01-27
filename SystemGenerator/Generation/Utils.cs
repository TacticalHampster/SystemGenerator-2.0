using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using SystemGenerator.Generation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static SystemGenerator.Generation.Gen;

namespace SystemGenerator.Generation
{
    public class Utils
    {
        public static Random rng = new Random();

        /**
         * Returns an evenly distributed random double from mn to mx.
         */
        public static double randDouble(double mn, double mx)
        {
            if (mx > mn)
                return (rng.NextDouble() * (mx - mn)) + mn;
            else if (mx < mn)
                return (rng.NextDouble() * (mn - mx)) + mx;
            else
                return mn;
        }

        /**
         * Returns a normally distributed random double given mean and standard deviation.
         */
        public static double randNormal(double mean, double std)
        {
            //Algorithm from https://stackoverflow.com/questions/218060/random-gaussian-variables
            double u1 = 1.0-rng.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0-rng.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return mean + std * randStdNormal; //random normal(mean,stdDev^2)
        }
        
        /**
         * Returns an evenly distributed random double from mn to mx with lambda steep.
         */
        public static double randExpo(double mn, double mx, double steep)
        {
            double u = Math.Log(1.0 - flip()) / (-steep);

            if (mx > mn)
                return (u * (mx - mn)) + mn;
            else if (mx < mn)
                return (u * (mn - mx)) + mx;
            else
                return fudge(u);
        }
        
        /**
         * Returns an evenly distributed random int from mn to mx.
         */
        public static int randInt(int mn, int mx)
        {
            if (mx > mn)
                return rng.Next(mn, mx);
            else if (mx < mn)
                return rng.Next(mx, mn);
            else
                return mn;
        }

        /**
         * Flips a coin. If heads, returns +1; if tails, returns -1.
         */
        public static double randSign()
        {
            if (randDouble(0.0, 1.0) >= 0.5)
                return 1.0;
            else return -1.0;
        }

        /**
         * Returns an evenly distributed double between 0.0 and 1.0.
         */
        public static double flip()
        {
            return randDouble(0.0, 1.0);
        }

        /**
         * Rolls a die with the specified number of sides.
         */
        public static int roll(int sides)
        {
            return randInt(0, sides);
        }

        /**
         * Randomly varies arg by FUDGE_FACTOR %.
         */
        public static double fudge(double arg)
        {
            return randDouble(arg * (1.0 - Gen.FUDGE_FACTOR), arg * (1.0 + Gen.FUDGE_FACTOR));
        }
    
        /**
         * Returns the orbital distance which is in the given resonant ratio with a.
         */
        public static double resonance(double a, double ratio)
        {
            return a * Math.Pow(ratio, (2.0/3.0));
        }
    
        /**
         * Returns the mean and standard deviation of a random sample of N items within the limits.
         */
        public static double[] getDistribution(double min, double max)
        {
            List<double> dist = new List<double>();
            double[] result = new double[]{0.0,0.0};

            for (int i = 0; i < Gen.SAMPLE_SIZE; i++)
            {
                dist.Add(randDouble(min, max));
                result[0] += dist[i];
            }

            result[0] /= Gen.SAMPLE_SIZE;

            for (int i = 0; i < Gen.SAMPLE_SIZE; i++)
                result[1] += Math.Pow(result[0]-dist[i], 2.0);

            result[1] = Math.Sqrt(result[1] / (double)dist.Count);

            return result;
        }

        /**
         * Returns the mean and standard deviation of a given dataset.
         */
        public static double[] getDistribution(double[] data)
        {
            double[] result = new double[]{0.0,0.0};

            for (int i = 0; i < data.Length; i++)
                result[0] += data[i];

            result[0] /= data.Length;

            for (int i = 0; i < data.Length; i++)
                result[1] += Math.Pow(result[0]-data[i], 2.0);

            result[1] = Math.Sqrt(result[1] / (double)data.Length);

            return result;
        }

        /**
         * Determines whether or not the given planet can retain a substance with the given molar weight in its atmosphere.
         */
        public static bool canRetain(double mass, Planet planet)
        {
            //Formula comes from Artifexian's earth-like atmospheres spreadsheet
            double rms = Math.Sqrt((3.0 * Const.GAS_CONST * ( planet.t / Const.Earth.TEMP ) * 1500.0) / mass);
            double lim = planet.escV / Gen.Atmo.RETENTION_FACTOR;

            if (rms / lim < 1.0)
                Utils.writeLog("                    Planet can retain substance (rms=" + rms + ", lim=" + lim + ", div=" + (rms / lim) + ")");
            else
                Utils.writeLog("                    Planet cannot retain substance (rms=" + rms + ", lim=" + lim + ", div=" + (rms / lim) + ")");

            return rms / lim < 1.0;
        }
    
        /**
         * Determines whether or not the given list contains the given component.
         */
        public static bool contains(List<Atmosphere.Component> comps, Atmosphere.Component comp)
        {
            foreach (Atmosphere.Component c in comps)
                if (c.name == comp.name)
                    return true;

            return false;
        }

        /**
         * Returns the ordinal suffix of num.
         */
        public static string getOrdinal(int num)
        {
            bool tens = 10 <= num && num <= 13;
            if (num%10 == 1 && !tens)
                return "st";
            else if (num%10 == 2 && !tens)
                return "nd";
            else if (num%10 == 3 && !tens)
                return "rd";
            else
                return "th";
        }
    
        /**
         * Returns a text description of the given ID for the listbox.
         */
        public static string getDescription(char ch)
        {
            switch (ch) 
            {
                case 'K':
                case 'G':
                case 'F':
                    return ch + "-class star";
                case ID.Planet.ROCK_DENSE : return "Dense rocky planet"    ;
                case ID.Planet.ROCK_DESERT: return "Rocky planet"          ;
                case ID.Planet.ROCK_GREEN : return "Habitable rocky planet";
                case ID.Planet.WATER_OCEAN: return "Ocean planet"          ;
                case ID.Planet.WATER_GREEN: return "Habitable ocean planet";
                case ID.Planet.GAS_GIANT  : return "Gas giant"             ;
                case ID.Planet.GAS_SUPER  : return "Gas supergiant"        ;
                case ID.Planet.GAS_PUFFY  : return "Puffy gas giant"       ;
                case ID.Planet.GAS_HOT    : return "Hot gas giant"         ;
                case ID.Planet.ICE_GIANT  : return "Ice giant"             ;
                case ID.Planet.ICE_DWARF  : return "Gas dwarf"             ;
                case ID.Belt.BELT_INNER   : return "Inner asteroid belt"   ;
                case ID.Belt.BELT_KUIPER  : return "Kuiper asteroid belt"  ;
                case ID.Belt.DWARF        : return "Inner belt dwarf"      ;
                case ID.Belt.PLUTINO      : return "Plutino dwarf"         ;
                case ID.Belt.CUBEWANO     : return "Cubewano dwarf"        ;
                case ID.Belt.TWOTINO      : return "Twotino dwarf"         ;
                case ID.Belt.SCATTERED    : return "Scattered disk dwarf"  ;
                case ID.Belt.SEDNOID      : return "Sednoid dwarf"         ;
                case ID.Sat.MINOR         : return "Captured asteroid"     ;
                case ID.Sat.MAJOR         : return "Rounded major moon"    ;
                case ID.Sat.MOONA         : return "Minor ring shepherd"   ;
                case ID.Sat.MOONB         : return "Rounded major moon"    ;
                case ID.Sat.MOONC         : return "Distant asteroid group";
                case ID.Sat.FOR_B         : return "L₄ Lagrangian companion";
                case ID.Sat.REV_B         : return "L₅ Lagrangian companion";
                default                   : return String.Format("GENERATION ERROR: type {0} ({1})", ch, Convert.ToInt32(ch));
            }
        }
            
        /**
         * Returns a longer text description of the given ID for the flavor text.
         */
        public static string getLongDesc(Planet p)
        {
            switch (p.type) 
            {
                case ID.Planet.ROCK_DENSE : return "a hot, dense Mercurial world"     ;
                case ID.Planet.ROCK_DESERT: return "a rocky terrestrial planet"       ;
                case ID.Planet.ROCK_GREEN : return "a life-bearing terrestrial planet";
                case ID.Planet.WATER_OCEAN:
                    if (p.subtype == "1")
                        return "a water-ocean planet, comprised primarily of rock";
                    else if (p.subtype == "3")
                        return "a water-ocean planet, comprised primarily of water";
                    else
                        return "a water-ocean planet, comprised of roughly equal parts water and rock";
                case ID.Planet.WATER_GREEN: return "a life-bearing ocean planet, comprised primarily of rock";
                case ID.Planet.GAS_GIANT  :
                    if (p.subtype == "1")
                        return "a class-1 gas giant";
                    else
                        return "a class-2 gas giant";
                case ID.Planet.GAS_SUPER  :
                    if (p.subtype == "1")
                        return "a class-1 super-Jupiter";
                    else
                        return "a class-2 super-Jupiter";
                case ID.Planet.GAS_PUFFY  : return "a class-3 puffy gas giant";
                case ID.Planet.GAS_HOT    :
                    if (p.subtype == "4")
                        return "a class-4 hot Jupiter";
                    else
                        return "a class-5 hot Jupiter";
                case ID.Planet.ICE_GIANT  :
                    if (p.subtype == "1")
                        return "an ice giant, comprised primarily of volatile ices";
                    else if (p.subtype == "3")
                        return "an ice giant, comprised primarily of water";
                    else
                        return "an ice giant, comprised of roughly equal parts water and volatiles";
                case ID.Planet.ICE_DWARF  :
                    if (p.subtype == "1")
                        return "a gas dwarf, comprised primarily of volatile ices";
                    else if (p.subtype == "3")
                        return "a gas dwarf, comprised primarily of water";
                    else
                        return "a gas dwarf, comprised of roughly equal parts water and volatiles";
                default                   : return String.Format("GENERATION ERROR: type {0} ({1})", p.type, Convert.ToInt32(p.type));
            }
        }
    
        /**
         * Writes string s to the log file.
         */
        public static void writeLog(string s)
        {
            //Debug.WriteLine(s);

            using (StreamWriter output = File.AppendText("C:\\Users\\green\\source\\repos\\SystemGenerator\\SystemGenerator\\log.txt"))
                output.WriteLine(s);
        }
     
        /**
         * Writes a line to the log file about the given atmospheric component.
         */
        public static void writeLogAtmo(Atmosphere.Component comp, double remain)
        {
            Utils.writeLog(String.Format("                    Generated {0}% {1,-15} ({2,7:5}% remaining)", comp.quantity*100.0, comp.name, remain*100.0));
        }
    }

    public class Decay
    {
        public enum DecayType
        {
            CONST,
            LINEAR,
            EXP,
            ATMO
        }

        public enum DecayDir
        {
            INCREASING,
            DECREASING
        }

        private double initChance;
        private double lim_inner;
        private double lim_outer;
        private DecayDir  dir;
        private DecayType type;
        private double t;
        private double b;

        public Decay(DecayType type, double initChance, int inner, int outer, DecayDir dir)
        {
            this.dir = dir;
            this.initChance = initChance;
            this.lim_inner = inner;
            this.lim_outer = outer;
            this.type = type;
        }

        public Decay(double T, double B)
        {
            this.type = DecayType.ATMO;
            this.dir  = DecayDir.DECREASING;
            this.t    = T;
            this.b    = B;
        }

        public double getDecayedChance(int x)
        {
            double chance = 0;

            if (this.type == DecayType.ATMO)
                chance = Utils.fudge(t*Math.Exp(Math.E - (b*x)))/100.0;
            else if (this.type == DecayType.LINEAR && this.dir == DecayDir.INCREASING)
                chance = (this.initChance / (this.lim_outer - this.lim_inner)) * (x - this.lim_inner);
            else if (this.type == DecayType.LINEAR && this.dir == DecayDir.DECREASING)
                chance = ((-this.initChance / (this.lim_outer - this.lim_inner)) * (x - this.lim_inner)) + (2.0 * this.initChance);
            else if (this.type == DecayType.EXP && this.dir == DecayDir.INCREASING)
                chance = Math.Exp(x - this.lim_outer - Math.Log(1.0/this.initChance));
            else if (this.type == DecayType.EXP && this.dir == DecayDir.DECREASING)
                chance = Math.Exp(-x + this.lim_outer - Math.Log(1.0/this.initChance));
            else
                chance = this.initChance;

            return chance;
        }
    }
}

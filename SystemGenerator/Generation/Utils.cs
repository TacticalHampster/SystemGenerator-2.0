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

        //Evenly distributed double
        public static double randDouble(double mn, double mx)
        {
            if (mx > mn)
                return (rng.NextDouble() * (mx - mn)) + mn;
            else if (mx < mn)
                return (rng.NextDouble() * (mn - mx)) + mx;
            else
                return mn;
        }

        //Normally distributed double
        public static double randNormal(double mean, double std)
        {
            //Algorithm from https://stackoverflow.com/questions/218060/random-gaussian-variables
            double u1 = 1.0-rng.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0-rng.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return mean + std * randStdNormal; //random normal(mean,stdDev^2)
        }

        //Exponentially distributed double
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
        
        //Evenly distributed int
        public static int randInt(int mn, int mx)
        {
            if (mx > mn)
                return rng.Next(mn, mx);
            else if (mx < mn)
                return rng.Next(mx, mn);
            else
                return mn;
        }

        //Flips a coin. If heads, returns +1; if tails, returns -1.
        public static double randSign()
        {
            if (randDouble(0.0, 1.0) >= 0.5)
                return 1.0;
            else return -1.0;
        }

        //Evenly distributed double between 0.0 and 1.0
        public static double flip()
        {
            return randDouble(0.0, 1.0);
        }

        //Rolls a die with the specified number of sides.
        public static int roll(int sides)
        {
            return randInt(0, sides);
        }

        //Randomly varies arg by FUDGE_FACTOR %.
        public static double fudge(double arg)
        {
            return randDouble(arg * (1.0 - Gen.FUDGE_FACTOR), arg * (1.0 + Gen.FUDGE_FACTOR));
        }
    
        //Calculates the orbital distance which is in the given resonant ratio with the given distance
        public static double resonance(double a, double ratio)
        {
            return a * Math.Pow(ratio, (2.0/3.0));
        }
    
        //Calculates the mean and std dev of a random sample of N items within the limits.
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

        //Overload of above for existing data
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

        //Determines if a planet can retain a gas with the given mass
        public static bool canRetain(double mass, Planet planet)
        {
            //Formula comes from Artifexian's earth-like atmospheres spreadsheet
            double rms = Math.Sqrt(3.0 * Const.GAS_CONST * ( planet.t / Const.Earth.TEMP ) * 1500.0) / mass;
            double lim = planet.escV / Gen.Atmo.RETENTION_FACTOR;

            if (rms / lim < 1.0)
                Utils.writeLog("                    Planet can retain substance (rms=" + rms + ", lim=" + lim + ", div=" + (rms / lim) + ")");
            else
                Utils.writeLog("                    Planet cannot retain substance (rms=" + rms + ", lim=" + lim + ", div=" + (rms / lim) + ")");

            return rms / lim < 1.0;
        }
    
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
                default                   : return String.Format("GENERATION ERROR: type {0} ({1})", ch, Convert.ToInt32(ch));
            }
        }
    
        public static void writeLog(string s)
        {
            //Debug.WriteLine(s);

            using (StreamWriter output = File.AppendText("C:\\Users\\green\\source\\repos\\SystemGenerator\\SystemGenerator\\log.txt"))
                output.WriteLine(s);
        }
     
        public static void writeLogAtmo(Atmosphere.Component comp, double remain)
        {
            Utils.writeLog(String.Format("                      Generated {0}% {1,-15} ({2}% remaining)", comp.quantity*100.0, comp.name, remain*100.0));
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

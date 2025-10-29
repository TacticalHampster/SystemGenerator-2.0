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
using static SystemGenerator.Generation.UI;
using System.Diagnostics.Eventing.Reader;
using System.Numerics;
using System.Windows.Forms;

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
            double uMin = Math.Exp(-mn*steep);
            double uMax = Math.Exp(-mx*steep);
            double random;

            do
            {
                if (uMax > uMin)
                    random = -Math.Log(randDouble(uMin, uMax))/steep;
                else if (uMax < uMin)
                    random = -Math.Log(randDouble(uMax, uMin))/steep;
                else
                    return fudge(uMin);
            }
            while ((random > mx) || (random < mn));

            return random;
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
         * Rolls a die with the specified weights.
         */
        public static int rollWeighted(int sides, int[] weights)
        {

            int num = 0, j, k = 0;

            for (int i = 0; i < sides; i++)
                num += weights[i];

            int[] options = new int[num];

            for (int i = 0; k < num; i++)
            {
                for (j = 0; j < weights[i]; j++)
                {
                    options[k+j] = i;
                }
                k += j;
            }

            return options[roll(num)];
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
         * Determines whether or not the given moon can retain a substance with the given molar weight in its atmosphere.
         */
        public static bool canRetain(double mass, Moon moon)
        {
            //Formula comes from Artifexian's earth-like atmospheres spreadsheet
            double rms = Math.Sqrt((3.0 * Const.GAS_CONST * ( moon.t / Const.Earth.TEMP ) * 1500.0) / mass);
            double lim = moon.escV / Gen.Atmo.RETENTION_FACTOR;

            if (rms / lim < 1.0)
                Utils.writeLog("                    Moon can retain substance (rms=" + rms + ", lim=" + lim + ", div=" + (rms / lim) + ")");
            else
                Utils.writeLog("                    Moon cannot retain substance (rms=" + rms + ", lim=" + lim + ", div=" + (rms / lim) + ")");

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
                case ID.Planet.EMPTY      : return "empty orbit"           ;
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

                case ID.Belt.DWARF:
                    if (p.isIcy)
                        return "an icy dwarf planet orbiting in the inner asteroid belt";
                    else
                        return "a rocky dwarf planet orbiting in the inner asteroid belt";

                case ID.Belt.PLUTINO:
                    if (p.isIcy)
                        return "an icy plutino, orbiting in 2:3 mean motion resonance with the outermost giant";
                    else
                        return "a rocky plutino, orbiting in 2:3 mean motion resonance with the outermost giant";

                case ID.Belt.CUBEWANO:
                    if (p.isIcy)
                        return "an icy cubewano, orbiting between the 2:3 and 1:2 mean motion resonances of the outermost giant";
                    else
                        return "a rocky cubewano, orbiting between the 2:3 and 1:2 mean motion resonances of the outermost giant";

                case ID.Belt.TWOTINO:
                    if (p.isIcy)
                        return "an icy twotino, orbiting in 1:2 mean motion resonance with the outermost giant";
                    else
                        return "a rocky twotino, orbiting in 1:2 mean motion resonance with the outermost giant";

                case ID.Belt.SCATTERED:
                    if (p.isIcy)
                        return "an icy scattered disk object";
                    else
                        return "a rocky scattered disk object";

                case ID.Belt.SEDNOID:
                    if (p.isIcy)
                        return "an icy sednoid, orbiting with extreme distance and eccentricity";
                    else
                        return "a rocky sednoid, orbiting with extreme distance and eccentricity";

                default                   : return String.Format("GENERATION ERROR: type {0} ({1})", p.type, Convert.ToInt32(p.type));
            }
        }
    
        /**
         * Returns a text description of the given relative magnitude value.
         */
        public static string getMagDesc(double mag)
        {
            if (mag <= -26.8 - Gen.FUDGE_FACTOR)
                return "brighter than Sol (-26.74)";
            else if (mag <= -26.8 + Gen.FUDGE_FACTOR)
                return "about as bright as Sol (-26.74)";
            else if (mag <= -24.0)
                return "slightly dimmer than Sol (-26.74)";
            else if (mag <= -14.74)
                return "brighter than our full moon (-12.74), but dimmer than Sol (-26.74)";
            else if (mag <= -12.74 - Gen.FUDGE_FACTOR)
                return "slightly dimmer than our full moon (-12.74)";
            else if (mag <= -12.74 + Gen.FUDGE_FACTOR)
                return "about as bright as our full moon (-12.74)";
            else if (mag <= -4.92 - Gen.FUDGE_FACTOR)
                return "brighter than Venus (-4.92), but dimmer than our full moon (-12.74)";
            else if (mag <= -4.92 + Gen.FUDGE_FACTOR)
                return "about as bright as Venus (-4.92)";
            else if (mag <= -2.94 - FUDGE_FACTOR)
                return "brighter than Jupiter or Mars (-2.94), but dimmer than Venus (-4.92)";
            else if (mag <= -2.9 + FUDGE_FACTOR)
                return "about as bright as Jupiter or Mars (-2.94)";
            else if (mag <= -1.47 - FUDGE_FACTOR)
                return "brighter than Sirius (-1.47), but dimmer than Jupiter or Mars (-2.94)";
            else if (mag <= -1.47 + FUDGE_FACTOR)
                return "about as bright as Sirius (-1.47)";
            else if (mag <= 1.25 - FUDGE_FACTOR)
                return "brighter than Deneb (1.25), but dimmer than Sirius (-1.47)";
            else if (mag <= 1.25 + FUDGE_FACTOR)
                return "about as bright as Deneb (1.25)";
            else if (mag <= 1.97 - FUDGE_FACTOR)
                return "brighter than Polaris (1.97), but dimmer than Deneb (1.25)";
            else if (mag <= 1.97 + FUDGE_FACTOR)
                return "about as bright as Polaris (1.97)";
            else if (mag <= 3.5 - FUDGE_FACTOR)
                return "brighter than Tau Ceti (3.5), but dimmer than Polaris (1.97)";
            else if (mag <= 3.5 + FUDGE_FACTOR)
                return "about as bright as Tau Ceti (3.5)";
            else if (mag <= 5.32 - FUDGE_FACTOR)
                return "brighter than Uranus (5.32), but dimmer than Tau Ceti (3.5)";
            else if (mag <= 5.32 + FUDGE_FACTOR)
                return "about as bright as Uranus (5.32)";
            else
                return "dimmer than Uranus (5.32)";
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
            Utils.writeLog(String.Format("                    Generated {0,8:N5}% {1,-20} ({2,8:N5}% remaining)", comp.quantity*100.0, comp.name, remain*100.0));
        }

        public static void updateProgress()
        {
            FormMain.genProgressBar.Value++;
        }
        
        public class UI
        {
            public static Color colorFromHex(int color)
            {
                double r = (double)((int)(color / (double)0x10000));
                double g = (double)((int)((color - (r*(double)0x10000)) / (double)0x100));
                double b = color - (r*(double)0x10000) - (g*(double)0x100);

                return Color.FromArgb((int)Math.Round(r),(int)Math.Round(g),(int)Math.Round(b));
            }

            public static int hexFromColor(Color color)
            {
                return (color.R * 0x10000) + (color.G * 0x100) + (color.B);
            }

            //Formulae for the next two are from https://stackoverflow.com/questions/2353211/hsl-to-rgb-color-conversion
            public static Color HslToRgb(double h, double s, double l)
            {
                double chroma, hprime, X;
                double[] rgb = new double[3];

                chroma = (1.0 - Math.Abs((2.0 * l) - 1.0)) * s;

                hprime = h / 60.0;

                X = chroma * ( 1.0 - Math.Abs(( hprime % 2.0 ) - 1.0 ));

                if (0.0 <= hprime && hprime <= 1.0)
                    rgb = new double[]{ chroma, X, 0.0 };
                else if (1.0 <= hprime && hprime <= 2.0)
                    rgb = new double[]{ X, chroma, 0.0 };
                else if (2.0 <= hprime && hprime <= 3.0)
                    rgb = new double[]{ 0.0, chroma, X };
                else if (3.0 <= hprime && hprime <= 4.0)
                    rgb = new double[]{ 0.0, X, chroma };
                else if (4.0 <= hprime && hprime <= 5.0)
                    rgb = new double[]{ X, 0.0, chroma };
                else if (5.0 <= hprime && hprime <= 6.0)
                    rgb = new double[]{ chroma, 0.0, X };
                
                Utils.writeLog(String.Format("Chroma = {0}, H' = {1}, X = {2}", chroma, hprime, X));
                Utils.writeLog(String.Format("R = {0}, G = {1}, B = {2}", rgb[0], rgb[1], rgb[2]));
                
                rgb[0] = Math.Round((rgb[0] + l - (chroma/2.0)) * Color.White.R);
                rgb[1] = Math.Round((rgb[1] + l - (chroma/2.0)) * Color.White.R);
                rgb[2] = Math.Round((rgb[2] + l - (chroma/2.0)) * Color.White.R);

                Color c = Color.FromArgb((int)rgb[0], (int)rgb[1], (int)rgb[2]);

                Utils.writeLog(String.Format("Returned color is ({0:D}, {1:D}, {2:D})", c.R, c.G, c.B));

                return c;
            }

            public static double RgbToHue(int color)
            {
                return RgbToHue(colorFromHex(color));
            }

            public static double RgbToHue(Color color)
            {
                //Code is from https://stackoverflow.com/questions/23090019/fastest-formula-to-get-hue-from-rgb

                float min = Math.Min(Math.Min(color.R, color.G), color.B);
                float max = Math.Max(Math.Min(color.R, color.G), color.B);

                if (min == max) {
                    return 0;
                }

                float hue = 0f;
                if (max == color.R) {
                    hue =      (color.G - color.B) / (max - min);
                } else if (max == color.G) {
                    hue = 2f + (color.B - color.R) / (max - min);
                } else {
                    hue = 4f + (color.R - color.G) / (max - min);
                }

                hue *= 60f;

                if (hue < 0) hue += 360f;

                return Math.Round(hue);
            }

            public static Bitmap expandBitmap(Bitmap bmp)
            { 
                //Expand dimensions of b
                Bitmap bitmap = new Bitmap(bmp.Width+2, bmp.Height+2);

                //Copy b into bitmap
                for (int y = 0; y < bmp.Height; y++)
                    for (int x = 0; x < bmp.Width; x++)
                        bitmap.SetPixel(x+1, y+1, bmp.GetPixel(x, y));

                //Duplicate horizontal edges
                for (int x = 1; x < bitmap.Width-1; x++)
                {
                    bitmap.SetPixel(x, 0, bitmap.GetPixel(x, 1));
                    bitmap.SetPixel(x, bitmap.Height-1, bitmap.GetPixel(x, bitmap.Height-2));    
                }

                //Duplicate vertical edges
                for (int y = 1; y < bitmap.Height-1; y++)
                {
                    bitmap.SetPixel(0, y, bitmap.GetPixel(1, y));
                    bitmap.SetPixel(bitmap.Width-1, y, bitmap.GetPixel(bitmap.Width-2, y));    
                }

                //Duplicate corners
                bitmap.SetPixel(0             , 0              , bitmap.GetPixel(1             , 1              ));
                bitmap.SetPixel(0             , bitmap.Height-1, bitmap.GetPixel(1             , bitmap.Height-2));
                bitmap.SetPixel(bitmap.Width-1, 0              , bitmap.GetPixel(bitmap.Width-2, 0              ));
                bitmap.SetPixel(bitmap.Width-1, bitmap.Height-1, bitmap.GetPixel(bitmap.Width-2, bitmap.Height-2));

                return bitmap;
            }

            public static Bitmap blur(Bitmap bmp, int stddev)
            {
                Bitmap bitmap = bmp;
                Bitmap result = new Bitmap(bmp.Width, bmp.Height);

                //Create the kernel
                int radius = 2*stddev;
                int kwidth = (2*radius) +1;
                double[][] kernel = new double[kwidth][];
                double total = 0, d, q = 2.0*stddev*stddev;

                //Expand dimensions of bitmap
                for (int i = 0; i < radius; i++)
                    bitmap = expandBitmap(bitmap);

                //Zero the kernel
                for (int x = 0; x < kwidth; x++)
                {
                    kernel[x] = new double[kwidth];
                    for (int y = 0; y < kwidth; y++)
                        kernel[x][y] = 0;
                }

                //Calculate the kernel values
                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        if (kernel[x+radius][y+radius] != 0)
                            continue;

                        d = (x * x) + (y * y);
                        kernel[ x+radius][ y+radius] = (1.0 / (Math.PI*q)) * Math.Exp(-d / q);
                        kernel[-x+radius][ y+radius] = kernel[x + radius][y + radius];
                        kernel[ x+radius][-y+radius] = kernel[x + radius][y + radius];
                        kernel[-x+radius][-y+radius] = kernel[x + radius][y + radius];
                        
                        total += kernel[x + radius][y + radius];
                    }
                }

                Utils.writeLog("Calculated Convolution kernel:");
                string s;
                for (int i = 0; i < kwidth; i++)
                {
                    s = "";

                    for (int j = 0; j < kwidth; j++)
                        s += string.Format("{0,7:N4}  ", kernel[i][j]);

                    Utils.writeLog(s);
                }

                Utils.writeLog("Total: " + total);

                double r, g, b;
                bool start = false;
                int overlaps, backtrack;

                //Convolve
                for (int y = 0; y < result.Height; y++)
                {
                    for (int x = 0; x < result.Width; x++)
                    { 
                        overlaps   = 0;
                        backtrack  = 0;

                        //If color enters the kernel, stop skipping forward and start convolving
                        if (!start)
                        {
                            if (bitmap.GetPixel(Math.Min(x+radius, result.Width), Math.Min(y+radius, result.Height)).R > 0 || bitmap.GetPixel(Math.Min(x+radius, result.Width), Math.Min(y+radius, result.Height)).G > 0 || bitmap.GetPixel(Math.Min(x+radius, result.Width), Math.Min(y+radius, result.Height)).B > 0)
                            {
                                start     = true;
                                backtrack = Math.Min(x+radius, result.Width) + 10*radius;
                                //Utils.writeLog("Detected color at (" + Math.Min(x+radius, result.Width) + ", " + y + "), starting convolution (Setting backtrack marker at " + backtrack + ")");
                                x -= radius;
                            }
                        }

                        //If the entire kernel is black, we've passed the image and we can skip forward to the next row
                        if (start && x > backtrack)
                        {
                            overlaps = 0;
                            for (int kx = 0; kx < kwidth; kx++)
                                for (int ky = 0; ky < kwidth; ky++)
                                    if (bitmap.GetPixel(x+kx, y+ky).R == 0 && bitmap.GetPixel(x+kx, y+ky).G == 0 && bitmap.GetPixel(x+kx, y+ky).B == 0)
                                        overlaps++;
                            
                            if (overlaps == kwidth*kwidth)
                            {
                                //Utils.writeLog("Kernel is entirely over black, skipping forward");
                                start = false;
                            }
                        }
                        
                        if (!start)
                        {
                            continue;
                        }

                        r = 0;
                        g = 0;
                        b = 0;

                        total = 0;
                        for (int ky = 0; ky < kwidth; ky++)
                        {
                            for (int kx = 0; kx < kwidth; kx++)
                            {
                                //If any part of the kernel overlaps the black background, don't include it
                                if (bitmap.GetPixel(x+kx, y+ky).R == 0 && bitmap.GetPixel(x+kx, y+ky).G == 0 && bitmap.GetPixel(x+kx, y+ky).B == 0)
                                    continue;

                                //Otherwise keep tallying the kernel
                                r += bitmap.GetPixel(x+kx, y+ky).R*kernel[kx][ky];
                                g += bitmap.GetPixel(x+kx, y+ky).G*kernel[kx][ky];
                                b += bitmap.GetPixel(x+kx, y+ky).B*kernel[kx][ky];
                                total += kernel[kx][ky];
                            }
                        }

                        //Round and normalize
                        r = Math.Round(r/total);
                        g = Math.Round(g/total);
                        b = Math.Round(b/total);
                        
                        if (r > 255)
                        { 
                            Utils.writeLog("Pixel red   value overflowed at (" + x + ", " + y + "): was " + r);
                            r = 255;
                        }
                        else if (r < 0)
                        {
                            Utils.writeLog("Pixel red   value underflowed at (" + x + ", " + y + "): was " + r);
                            r = 0;
                        }

                        if (g > 255)
                        { 
                            Utils.writeLog("Pixel green value overflowed at (" + x + ", " + y + "): was " + g);
                            g = 255;
                        }
                        else if (g < 0)
                        {
                            Utils.writeLog("Pixel green value underflowed at (" + x + ", " + y + "): was " + g);
                            g = 0;
                        }

                        if (b > 255)
                        { 
                            Utils.writeLog("Pixel blue  value overflowed at (" + x + ", " + y + "): was " + b);
                            b = 255;
                        }
                        else if (b < 0)
                        {
                            Utils.writeLog("Pixel blue  value underflowed at (" + x + ", " + y + "): was " + b);
                            b = 0;
                        }

                        result.SetPixel(
                            x,
                            y,
                            Color.FromArgb(
                                (int)Math.Round(r),
                                (int)Math.Round(g),
                                (int)Math.Round(b)
                            )
                        );
                        
                        xloop:
                        for (;false;){ }
                    }
                
                    updateProgress();    
                }

                return result;
            }

            public static Bitmap rotate(Bitmap b, double angle)
            {
                //Code is from https://foxlearn.com/csharp/image-rotation-8368.html
                
                // Create a new empty bitmap to hold the rotated image
                Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
                // Create a graphics object from the empty bitmap
                Graphics g = Graphics.FromImage(returnBitmap);
     
                // Move the rotation point to the center of the image
                g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
     
                // Rotate the image by the specified angle
                g.RotateTransform((float)angle);
     
                // Move the image back to its original position
                g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
     
                // Draw the original image onto the graphics object
                g.DrawImage(b, new Point(0, 0));
     
                // Return the rotated image
                return returnBitmap;
            }
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
        public Decay(DecayType type, double initChance, double inner, double outer, DecayDir dir)
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
            {
                if (x < this.lim_inner)
                    chance = 0;
                else if (x > this.lim_outer)
                    chance = this.initChance;
                else
                    chance = (this.initChance / (this.lim_outer - this.lim_inner)) * (x - this.lim_inner);
            }

            else if (this.type == DecayType.LINEAR && this.dir == DecayDir.DECREASING)
            {
                if (x < this.lim_inner)
                    chance = this.initChance;
                else if (x > this.lim_outer)
                    chance = 0;
                else
                    chance = ((-this.initChance / (this.lim_outer - this.lim_inner)) * (x - this.lim_inner)) + this.initChance;
            }
                
            else if (this.type == DecayType.EXP && this.dir == DecayDir.INCREASING)
            {
                if (x < this.lim_inner)
                    chance = this.initChance * Math.Exp(this.lim_inner-this.lim_outer);
                else if (x > this.lim_outer)
                    chance = this.initChance;
                else
                    chance = this.initChance * (Math.Exp(x - this.lim_inner)/Math.Exp(this.lim_outer - this.lim_inner));
            }
                
            else if (this.type == DecayType.EXP && this.dir == DecayDir.DECREASING)
            {
                
                if (x < this.lim_inner)
                    chance = this.initChance;
                else if (x > this.lim_outer)
                    chance = this.initChance * Math.Exp(this.lim_inner-this.lim_outer);
                else
                    chance = this.initChance * Math.Exp(-x + this.lim_inner);
            }

            else
                chance = this.initChance;

            return chance;
        }
    }
}

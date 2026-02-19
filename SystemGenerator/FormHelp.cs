using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using SystemGenerator.Generation;

namespace SystemGenerator
{
    public partial class FormHelp:Form
    {
        public FormHelp()
        {
            InitializeComponent();
        }

        private void FormHelp_Load(object sender,EventArgs e)
        {
            majorClassesTable1.Hide();
            majorClassesTable2.Hide();
            minorClassesTable.Hide();
            textLabel.Text = "";

            listPropsToLabel(ref jotunnianLabelFirstComp,ref jotunnianLabelFirstChance,Gen.Atmo.JOTUNNIAN.primary,Gen.Atmo.JOTUNNIAN.primaryWeight);
            listPropsToLabel(ref jotunnianLabelSecondComp,ref jotunnianLabelSecondChance,Gen.Atmo.JOTUNNIAN.secondary,Gen.Atmo.JOTUNNIAN.secondaryWeight);
            listPropsToLabel(ref jotunnianLabelThirdComp,ref jotunnianLabelThirdChance,Gen.Atmo.JOTUNNIAN.tertiary,Gen.Atmo.JOTUNNIAN.tertiaryWeight);
            listPropsToLabel(ref jotunnianLabelMinorComp,ref jotunnianLabelMinorChance,Gen.Atmo.JOTUNNIAN.minor,Gen.Atmo.JOTUNNIAN.minorWeight);

            listPropsToLabel(ref helianLabelFirstComp,ref helianLabelFirstChance,Gen.Atmo.HELIAN.primary,Gen.Atmo.HELIAN.primaryWeight);
            listPropsToLabel(ref helianLabelSecondComp,ref helianLabelSecondChance,Gen.Atmo.HELIAN.secondary,Gen.Atmo.HELIAN.secondaryWeight);
            listPropsToLabel(ref helianLabelThirdComp,ref helianLabelThirdChance,Gen.Atmo.HELIAN.tertiary,Gen.Atmo.HELIAN.tertiaryWeight);
            listPropsToLabel(ref helianLabelMinorComp,ref helianLabelMinorChance,Gen.Atmo.HELIAN.minor,Gen.Atmo.HELIAN.minorWeight);

            listPropsToLabel(ref ydatrianLabelFirstComp,ref ydatrianLabelFirstChance,Gen.Atmo.YDATRIAN.primary,Gen.Atmo.YDATRIAN.primaryWeight);
            listPropsToLabel(ref ydatrianLabelSecondComp,ref ydatrianLabelSecondChance,Gen.Atmo.YDATRIAN.secondary,Gen.Atmo.YDATRIAN.secondaryWeight);
            listPropsToLabel(ref ydatrianLabelThirdComp,ref ydatrianLabelThirdChance,Gen.Atmo.YDATRIAN.tertiary,Gen.Atmo.YDATRIAN.tertiaryWeight);
            listPropsToLabel(ref ydatrianLabelMinorComp,ref ydatrianLabelMinorChance,Gen.Atmo.YDATRIAN.minor,Gen.Atmo.YDATRIAN.minorWeight);

            listPropsToLabel(ref rheanLabelFirstComp,ref rheanLabelFirstChance,Gen.Atmo.RHEAN.primary,Gen.Atmo.RHEAN.primaryWeight);
            listPropsToLabel(ref rheanLabelSecondComp,ref rheanLabelSecondChance,Gen.Atmo.RHEAN.secondary,Gen.Atmo.RHEAN.secondaryWeight);
            listPropsToLabel(ref rheanLabelThirdComp,ref rheanLabelThirdChance,Gen.Atmo.RHEAN.tertiary,Gen.Atmo.RHEAN.tertiaryWeight);
            listPropsToLabel(ref rheanLabelMinorComp,ref rheanLabelMinorChance,Gen.Atmo.RHEAN.minor,Gen.Atmo.RHEAN.minorWeight);

            listPropsToLabel(ref minervanLabelFirstComp,ref minervanLabelFirstChance,Gen.Atmo.MINERVAN.primary,Gen.Atmo.MINERVAN.primaryWeight);
            listPropsToLabel(ref minervanLabelSecondComp,ref minervanLabelSecondChance,Gen.Atmo.MINERVAN.secondary,Gen.Atmo.MINERVAN.secondaryWeight);
            listPropsToLabel(ref minervanLabelThirdComp,ref minervanLabelThirdChance,Gen.Atmo.MINERVAN.tertiary,Gen.Atmo.MINERVAN.tertiaryWeight);
            listPropsToLabel(ref minervanLabelMinorComp,ref minervanLabelMinorChance,Gen.Atmo.MINERVAN.minor,Gen.Atmo.MINERVAN.minorWeight);

            listPropsToLabel(ref edelianLabelFirstComp,ref edelianLabelFirstChance,Gen.Atmo.EDELIAN.primary,Gen.Atmo.EDELIAN.primaryWeight);
            listPropsToLabel(ref edelianLabelSecondComp,ref edelianLabelSecondChance,Gen.Atmo.EDELIAN.secondary,Gen.Atmo.EDELIAN.secondaryWeight);
            listPropsToLabel(ref edelianLabelThirdComp,ref edelianLabelThirdChance,Gen.Atmo.EDELIAN.tertiary,Gen.Atmo.EDELIAN.tertiaryWeight);
            listPropsToLabel(ref edelianLabelMinorComp,ref edelianLabelMinorChance,Gen.Atmo.EDELIAN.minor,Gen.Atmo.EDELIAN.minorWeight);

            Label[] cryoazurianColorLabels = { cryoazurianColor1, cryoazurianColor2, cryoazurianColor3 , cryoazurianColor4 , cryoazurianColor5 , cryoazurianColor6 , cryoazurianColor7 ,
                                               cryoazurianColor8, cryoazurianColor9, cryoazurianColor10, cryoazurianColor11, cryoazurianColor12, cryoazurianColor13, cryoazurianColor14 };
            Label[] frigidianColorLabels   = { frigidianColor1  , frigidianColor2  , frigidianColor3   , frigidianColor4   , frigidianColor5   , frigidianColor6   , frigidianColor7   ,
                                               frigidianColor8  , frigidianColor9  , frigidianColor10  , frigidianColor11  , frigidianColor12  , frigidianColor13  , frigidianColor14   };
            Label[] neoneanColorLabels     = { neoneanColor1    , neoneanColor2    , neoneanColor3     , neoneanColor4     , neoneanColor5     , neoneanColor6     , neoneanColor7     ,
                                               neoneanColor8    , neoneanColor9    , neoneanColor10    , neoneanColor11    , neoneanColor12    , neoneanColor13    , neoneanColor14     };
            Label[] boreanColorLabels      = { boreanColor1     , boreanColor2     , boreanColor3      , boreanColor4      , boreanColor5      , boreanColor6      , boreanColor7      ,
                                               boreanColor8     , boreanColor9     , boreanColor10     , boreanColor11     , boreanColor12     , boreanColor13     , boreanColor14      };
            Label[] methaneanColorLabels   = { methaneanColor1  , methaneanColor2  , methaneanColor3   , methaneanColor4   , methaneanColor5   , methaneanColor6   , methaneanColor7   ,
                                               methaneanColor8  , methaneanColor9  , methaneanColor10  , methaneanColor11  , methaneanColor12  , methaneanColor13  , methaneanColor14   };
            Label[] mesoazurianColorLabels = { mesoazurianColor1, mesoazurianColor2, mesoazurianColor3 , mesoazurianColor4 , mesoazurianColor5 , mesoazurianColor6 , mesoazurianColor7 ,
                                               mesoazurianColor8, mesoazurianColor9, mesoazurianColor10, mesoazurianColor11, mesoazurianColor12, mesoazurianColor13, mesoazurianColor14 };
            Label[] tholianColorLabels     = { tholianColor1    , tholianColor2    , tholianColor3     , tholianColor4     , tholianColor5     , tholianColor6     , tholianColor7     ,
                                               tholianColor8    , tholianColor9    , tholianColor10    , tholianColor11    , tholianColor12    , tholianColor13    , tholianColor14     };
            Label[] sulfanianColorLabels   = { sulfanianColor1  , sulfanianColor2  , sulfanianColor3   , sulfanianColor4   , sulfanianColor5   , sulfanianColor6   , sulfanianColor7   ,
                                               sulfanianColor8  , sulfanianColor9  , sulfanianColor10  , sulfanianColor11  , sulfanianColor12  , sulfanianColor13  , sulfanianColor14   };
            Label[] ammonianColorLabels    = { ammonianColor1   , ammonianColor2   , ammonianColor3    , ammonianColor4    , ammonianColor5    , ammonianColor6    , ammonianColor7    ,
                                               ammonianColor8   , ammonianColor9   , ammonianColor10   , ammonianColor11   , ammonianColor12   , ammonianColor13   , ammonianColor14    };
            Label[] hydronianColorLabels   = { hydronianColor1  , hydronianColor2  , hydronianColor3   , hydronianColor4   , hydronianColor5   , hydronianColor6   , hydronianColor7   ,
                                               hydronianColor8  , hydronianColor9  , hydronianColor10  , hydronianColor11  , hydronianColor12  , hydronianColor13  , hydronianColor14   };
            Label[] acidianColorLabels     = { acidianColor1    , acidianColor2    , acidianColor3     , acidianColor4     , acidianColor5     , acidianColor6     , acidianColor7     ,
                                               acidianColor8    , acidianColor9    , acidianColor10    , acidianColor11    , acidianColor12    , acidianColor13    , acidianColor14     };
            Label[] pyroazurianColorLabels = { pyroazurianColor1, pyroazurianColor2, pyroazurianColor3 , pyroazurianColor4 , pyroazurianColor5 , pyroazurianColor6 , pyroazurianColor7 ,
                                               pyroazurianColor8, pyroazurianColor9, pyroazurianColor10, pyroazurianColor11, pyroazurianColor12, pyroazurianColor13, pyroazurianColor14 };
            Label[] sulfolianColorLabels   = { sulfolianColor1  , sulfolianColor2  , sulfolianColor3   , sulfolianColor4   , sulfolianColor5   , sulfolianColor6   , sulfolianColor7   ,
                                               sulfolianColor8  , sulfolianColor9  , sulfolianColor10  , sulfolianColor11  , sulfolianColor12  , sulfolianColor13  , sulfolianColor14   };
            Label[] aithalianColorLabels   = { aithalianColor1  , aithalianColor2  , aithalianColor3   , aithalianColor4   , aithalianColor5   , aithalianColor6   , aithalianColor7   ,
                                               aithalianColor8  , aithalianColor9  , aithalianColor10  , aithalianColor11  , aithalianColor12  , aithalianColor13  , aithalianColor14   };
            Label[] alkalineanColorLabels  = { alkalineanColor1 , alkalineanColor2 , alkalineanColor3  , alkalineanColor4  , alkalineanColor5  , alkalineanColor6  , alkalineanColor7  ,
                                               alkalineanColor8 , alkalineanColor9 , alkalineanColor10 , alkalineanColor11 , alkalineanColor12 , alkalineanColor13 , alkalineanColor14  };


            listColorsToLabel(ref cryoazurianColorLabels,Gen.Atmo.CRYOAZURIAN.colorNames,Gen.Atmo.CRYOAZURIAN.cloudColorNames);
            listColorsToLabel(ref frigidianColorLabels,Gen.Atmo.FRIGIDIAN.colorNames,Gen.Atmo.FRIGIDIAN.cloudColorNames);
            listColorsToLabel(ref neoneanColorLabels,Gen.Atmo.NEONEAN.colorNames,Gen.Atmo.NEONEAN.cloudColorNames);
            listColorsToLabel(ref boreanColorLabels,Gen.Atmo.BOREAN.colorNames,Gen.Atmo.BOREAN.cloudColorNames);
            listColorsToLabel(ref methaneanColorLabels,Gen.Atmo.METHANEAN.colorNames,Gen.Atmo.METHANEAN.cloudColorNames);
            listColorsToLabel(ref mesoazurianColorLabels,Gen.Atmo.MESOAZURIAN.colorNames,Gen.Atmo.MESOAZURIAN.cloudColorNames);
            listColorsToLabel(ref tholianColorLabels,Gen.Atmo.THOLIAN.colorNames,Gen.Atmo.THOLIAN.cloudColorNames);
            listColorsToLabel(ref sulfanianColorLabels,Gen.Atmo.SULFANIAN.colorNames,Gen.Atmo.SULFANIAN.cloudColorNames);
            listColorsToLabel(ref ammonianColorLabels,Gen.Atmo.AMMONIAN.colorNames,Gen.Atmo.AMMONIAN.cloudColorNames);
            listColorsToLabel(ref hydronianColorLabels,Gen.Atmo.HYDRONIAN.colorNames,Gen.Atmo.HYDRONIAN.cloudColorNames);
            listColorsToLabel(ref acidianColorLabels,Gen.Atmo.ACIDIAN.colorNames,Gen.Atmo.ACIDIAN.cloudColorNames);
            listColorsToLabel(ref pyroazurianColorLabels,Gen.Atmo.PYROAZURIAN.colorNames,Gen.Atmo.PYROAZURIAN.cloudColorNames);
            listColorsToLabel(ref sulfolianColorLabels,Gen.Atmo.SULFOLIAN.colorNames,Gen.Atmo.SULFOLIAN.cloudColorNames);
            listColorsToLabel(ref aithalianColorLabels,Gen.Atmo.AITHALIAN.colorNames,Gen.Atmo.AITHALIAN.cloudColorNames);
            listColorsToLabel(ref alkalineanColorLabels,Gen.Atmo.ALKALINEAN.colorNames,Gen.Atmo.ALKALINEAN.cloudColorNames);
        }

        private void listPropsToLabel(ref Label cLabel,ref Label wLabel,Atmosphere.Component[] comps,int[] weights)
        {
            Atmosphere.Component cTemp;
            int sum = 0, iTemp;
            //int reducedWeight, reducedSum, count = 0;

            //Calculate the total sides of the die
            for(int i = 0;i < weights.Length;i++)
                sum += weights[i];

            //Sort the components descending by weight
            for(int i = 0;i < comps.Length - 1;i++)
            {
                for(int j = 0;j < comps.Length - i - 1;j++)
                {
                    if(weights[j] < weights[j + 1])
                    {
                        iTemp = weights[j + 1];
                        weights[j + 1] = weights[j];
                        weights[j] = iTemp;

                        cTemp = comps[j + 1].copy();
                        comps[j + 1] = comps[j];
                        comps[j] = cTemp;
                    }
                }
            }

            //Alphabetize within each weight
            for(int i = 0;i < comps.Length - 1;i++)
            {
                for(int j = 0;j < comps.Length - i - 1;j++)
                {
                    if((weights[j] == weights[j + 1]) && String.Compare(comps[j].name,comps[j + 1].name) > 0)
                    {
                        iTemp = weights[j + 1];
                        weights[j + 1] = weights[j];
                        weights[j] = iTemp;

                        cTemp = comps[j + 1].copy();
                        comps[j + 1] = comps[j];
                        comps[j] = cTemp;
                    }
                }
            }

            //Write the lists to the labels
            cLabel.Text = "";
            wLabel.Text = "";
            for(int i = 0;i < comps.Length;i++)
            {
                if(i != 0)
                {
                    cLabel.Text += "\n";
                    wLabel.Text += "\n";
                }

                if(weights[i] == 0)
                    continue;

                cLabel.Text += comps[i].name;

                if(weights[i] == sum)
                    wLabel.Text += "Always";
                else
                {
                    //Reduce the chance fraction
                    /*
                    reducedWeight = weights[i];
                    reducedSum = sum;

                    Utils.writeLog(Environment.NewLine + "Reducing fraction " + reducedWeight + "/" + reducedSum);

                    for(int j = 1;j < reducedWeight;j++)
                    {
                        do
                        {
                            Utils.writeLog("   " + reducedWeight + "%" + (j + 1) + "=" + (reducedWeight % (j + 1)) + ", " + reducedSum + "%" + (j + 1) + "=" + (reducedSum % (j + 1)));

                            //If j evenly divides both weight and sum
                            if((reducedWeight % (j + 1) == 0) && (reducedSum % (j + 1) == 0))
                            {
                                //Cancel it
                                count++;
                                Utils.writeLog("   " + (j + 1) + " evenly divides both " + reducedWeight + " and " + reducedSum);
                                reducedWeight /= (j + 1);
                                reducedSum /= (j + 1);
                                Utils.writeLog("   New fraction: " + reducedWeight + "/" + reducedSum);
                            }
                        }
                        while((reducedWeight % (j + 1) == 0) && (reducedSum % (j + 1) == 0)); //Repeat until it no longer works
                    }

                    if(count == 0)
                        Utils.writeLog("   Fraction is irreducible");
                    else
                        Utils.writeLog("   Final fraction: " + reducedWeight + "/" + reducedSum);
                    */

                    if(i == 0 || weights[i-1] != weights[i])
                        wLabel.Text += String.Format("{0,4:N1}%",((double)weights[i]/(double)sum)*100.0);
                    else
                        wLabel.Text += "";
                }
            }
        }

        private void listColorsToLabel(ref Label[] labels,string[] baseColors,string[] cloudColors)
        {
            //Concatenate the lists of colors
            Color[] colors;
            string[] cs;
            if(baseColors == null && cloudColors == null)
                return;
            else if(baseColors == null)
            {
                cs = new string[cloudColors.Length];
                cloudColors.CopyTo(cs,0);
            }
            else if(cloudColors == null)
            {
                cs = new string[baseColors.Length];
                baseColors.CopyTo(cs,0);
            }
            else
            {
                cs = new string[baseColors.Length + cloudColors.Length];
                baseColors.CopyTo(cs,0);
                cloudColors.CopyTo(cs,baseColors.Length);
            }

            colors = new Color[cs.Length];
            for(int j = 0;j < cs.Length;j++)
                colors[j] = Utils.UI.colorFromHex((int)Atmosphere.colorLookup(cs[j]));

            //Sort the colors ascending by hue
            Color cTemp;
            for(int k = 0;k < colors.Length - 1;k++)
            {
                for(int j = 0;j < colors.Length - k - 1;j++)
                {
                    if(Utils.UI.RgbToHue(colors[j]) < Utils.UI.RgbToHue(colors[j + 1]))
                    {
                        cTemp = colors[j + 1];
                        colors[j + 1] = colors[j];
                        colors[j] = cTemp;
                    }
                }
            }

            //Sort by albedo within each hue
            for(int k = 0;k < colors.Length - 1;k++)
            {
                for(int j = 0;j < colors.Length - k - 1;j++)
                {
                    if((Utils.UI.RgbToHue(colors[j]) == Utils.UI.RgbToHue(colors[j + 1])) && Atmosphere.albedoFromRGB(colors[j]) < Atmosphere.albedoFromRGB(colors[j + 1]))
                    {
                        cTemp = colors[j + 1];
                        colors[j + 1] = colors[j];
                        colors[j] = cTemp;
                    }
                }
            }

            int offset = 0;
            int i;
            bool repeat;
            Color color;
            for(i = 0;i < colors.Length;i++)
            {
                color = colors[i];

                repeat = false;

                for(int j = 0;j < labels.Length;j++)
                    if(labels[j].BackColor == colors[i])
                        repeat = true;

                if(Utils.UI.hexFromColor(colors[i]) == 0 || repeat)
                {
                    offset++;
                    continue;

                }

                labels[i - offset].ForeColor = colors[i];
                labels[i - offset].BackColor = colors[i];
            }

            do
            {
                labels[i - offset].ForeColor = Color.Black;
                labels[i - offset].BackColor = Color.Black;
                i++;
            }
            while(i - offset < labels.Length);
        }

        private void helpListBox_SelectedIndexChanged(object sender,EventArgs e)
        {
            switch(helpListBox.SelectedIndex)
            {
                case 2:
                    textLabel.Text = "This program generates atmospheric compositions from one of six preset classes, based on factors like temperature, gravity, and planet type. These pages list these preset classes, along with the appoximate chances of each chemical appearing and in what amount. The classes themselves are based on the Extended World Classification System from Orion's Arm, so look them up to learn more.";
                    majorClassesTable1.Show();
                    majorClassesTable2.Hide();
                    minorClassesTable.Hide();
                    pictureBoxTest.Hide();
                    break;

                case 3:
                    textLabel.Text = "This program generates atmospheric compositions from one of six preset classes, based on factors like temperature, gravity, and planet type. These pages list these preset classes, along with the appoximate chances of each chemical appearing and in what amount. The classes themselves are based on the Extended World Classification System from Orion's Arm, so look them up to learn more.";
                    majorClassesTable1.Hide();
                    majorClassesTable2.Show();
                    minorClassesTable.Hide();
                    pictureBoxTest.Hide();
                    break;

                case 4:
                    textLabel.Text = "This program chooses the color of the atmosphere based on one of 15 preset classes, based on factors like temperature and planet type. This page lists these preset classes with color swatches typical of each class. The classes themselves are based on the Extended World Classification System from Orion's Arm, so look them up to learn more.";
                    majorClassesTable1.Hide();
                    majorClassesTable2.Hide();
                    minorClassesTable.Show();
                    pictureBoxTest.Hide();
                    break;

                default:
                    majorClassesTable1.Hide();
                    majorClassesTable2.Hide();
                    minorClassesTable.Hide();
                    pictureBoxTest.Show();
                    break;
            }
        }
    
        /*
        private void testGiantRise()
        {
            int x = pictureBoxTest.Width;
            int y = pictureBoxTest.Height;

            bool isIcy = false;
            
            Bitmap image   = new Bitmap(x,y);
            Bitmap surface = new Bitmap(x,y);
            Bitmap smask   = new Bitmap(x,y);
            Bitmap mask    = new Bitmap(x,y);
            
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

            int radius = (int)Math.Round(11 * Const.Earth.RADIUS * (1.0/400.0)) + UI.BLUR_RADIUS;

            p.Width = 1;

            //For giants the atmo is the planet
            Color h = Color.DarkGoldenrod;
            p.Color = Color.FromArgb(h.R * (Color.White.R/255), h.G * (Color.White.R/255), h.B * (Color.White.R/255));
            surf = p.Color;
            
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
            band = Color.Tan;

            double rise     = Utils.randDouble(10.0, 20.0);
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

                //if (latitude < 0)
                //    goto southernBand;

                //Add the top arc
                riseFactor = Math.Sin(rise * (Math.PI/180.0));
                latFactor  = Math.Cos(latN * (Math.PI/180.0));
                offset     = Math.Sin(latN * (Math.PI/180.0));
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
                riseFactor = Math.Sin(rise * (Math.PI/180.0));
                latFactor  = Math.Cos(latN * (Math.PI/180.0));
                offset     = Math.Sin(latN * (Math.PI/180.0));
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
                riseFactor = Math.Sin(rise * (Math.PI/180.0));
                latFactor  = Math.Cos(latS * (Math.PI/180.0));
                offset     = Math.Sin(latS * (Math.PI/180.0));
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
                riseFactor = Math.Sin(rise * (Math.PI/180.0));
                latFactor  = Math.Cos(latS * (Math.PI/180.0));
                offset     = Math.Sin(latS * (Math.PI/180.0));
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

            //Greeble

            int A, R, G, B, lightness, lat, height, mid, wid, lfLim, rtLim, left, right, greebles, factor;
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
                riseFactor = Math.Sin(rise * (Math.PI/180.0));
                latFactor  = Math.Cos(lat  * (Math.PI/180.0));
                offset     = Math.Sin(lat  * (Math.PI/180.0));
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

                lightness = Utils.randInt(Color.White.R/11, Color.White.R/7) * (int)Utils.randSign();
                A = Utils.randInt(Color.White.R/7, Color.White.R/3);
                
                if (Atmosphere.albedoFromRGB(Utils.UI.hexFromColor(smask.GetPixel(radius, height))) <= avgLight)
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
            double tilt = Utils.randDouble(Gen.Planet.Giant.MIN_TILT, Gen.Planet.Giant.MAX_TILT);
            surface = Utils.UI.rotate(surface, tilt*Utils.randSign());

            //Blur the surface of the planet
            surface = Utils.UI.blur(surface, UI.BLUR_RADIUS);


            //Copy surface onto this.image using mask to cut off the parts where the planet blurs into space
            for (int sx = 0; sx < image.Width; sx++)
                for (int sy = 0; sy < image.Height; sy++)
                    if (mask.GetPixel(sx, sy).R > 0 || mask.GetPixel(sx, sy).G > 0 || mask.GetPixel(sx, sy).B > 0)
                        image.SetPixel(sx, sy, surface.GetPixel(sx, sy));
            
            //Add lighting
            double turn = Utils.randDouble(UI.MIN_TURN, UI.MAX_TURN);
            image       = Utils.UI.shade(image, turn, radius, 0, center);

            g.Dispose();
            gs.Dispose();
            gm.Dispose();
            p.Dispose();
            if (pgb != null)
                pgb.Dispose();

            pictureBoxTest.Image = image;
        }
        */
    }
}

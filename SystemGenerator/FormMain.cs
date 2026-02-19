using System.Diagnostics.Eventing.Reader;
using System.Drawing.Drawing2D;
using SystemGenerator.Generation;

namespace SystemGenerator
{
    public partial class FormMain:Form
    {
        public FormMain()
        {
            InitializeComponent();
            star = new Star();
            planets = new List<Planet>();
        }

        private Star star;
        private List<Planet> planets;

        private void FormMain_Load(object sender,EventArgs e)
        {
            //Clear log file
            using(StreamWriter output = new StreamWriter("C:\\Users\\green\\source\\repos\\SystemGenerator\\SystemGenerator\\log.txt"))
                output.Write("");

            FormHelp fh = new FormHelp();
            fh.Show();

            propsPhysicalStarGroup.Hide();
            propsLightGroup.Hide();
            propsOrbitStarGroup.Hide();

            propsPhysicalBeltGroup.Hide();
            propsBulkBeltGroup.Hide();
            propsOrbitBeltGroup.Hide();

            propsPhysicalPlanetGroup.Hide();
            propsBulkGroup.Hide();
            propsOrbitPlanetGroup.Hide();
            propsAtmoGroup.Hide();

            propsPhysicalMoonGroup.Hide();

            blankGroupBox1.Show();
            blankGroupBox2.Show();
            blankGroupBox3.Show();
            blankGroupBox4.Show();

            /*
            propsPhysicalBeltGroup.Location = propsPhysicalPlanetGroup.Location;
            propsPhysicalStarGroup.Location = propsPhysicalPlanetGroup.Location;
            propsPhysicalMoonGroup.Location = propsPhysicalPlanetGroup.Location;

            propsBulkBeltGroup.Location = propsBulkGroup.Location;
            propsLightGroup.Location = propsBulkGroup.Location;

            propsOrbitBeltGroup.Location = propsOrbitPlanetGroup.Location;
            propsOrbitStarGroup.Location = propsOrbitPlanetGroup.Location;
            */

            flavorTextLabel.Text = "";

            genProgressBar.Hide();
            genButton.Show();
            scaleLabel.Hide();

            this.Show();
            int height = Screen.PrimaryScreen.Bounds.Height;
            int width = Screen.PrimaryScreen.Bounds.Width;

            //Rescale widgets
            textGroupBox.Location = new Point(optionListBox.Location.X,optionListBox.Location.Y + optionListBox.Height);

            //Generate a system once the form loads
            //genButton_Click(sender, e);
        }

        private void genButton_Click(object sender,EventArgs e)
        {
            genButton.Hide();
            genProgressBar.Value = 0;
            genProgressBar.Maximum = 100;
            genProgressBar.ForeColor = Color.Blue;
            genProgressBar.Show();

            //Switch to correct groups
            pictureBox.Hide();

            blankGroupBox1.Show();
            blankGroupBox2.Show();
            blankGroupBox3.Show();
            blankGroupBox4.Show();

            propsPhysicalStarGroup.Hide();
            propsLightGroup.Hide();
            propsOrbitStarGroup.Hide();

            propsPhysicalBeltGroup.Hide();
            propsBulkBeltGroup.Hide();
            propsOrbitBeltGroup.Hide();

            propsPhysicalMoonGroup.Hide();

            propsPhysicalPlanetGroup.Hide();
            propsBulkGroup.Hide();
            propsOrbitPlanetGroup.Hide();
            propsAtmoGroup.Hide();

            scaleLabel.Text = "";
            flavorTextLabel.Text = "";

            systemListBox.Items.Clear();
            optionListBox.Items.Clear();

            //Clear log file
            using(StreamWriter output = new StreamWriter("C:\\Users\\green\\source\\repos\\SystemGenerator\\SystemGenerator\\log.txt"))
                output.Write("");

            Utils.writeLog("Beginning system generation");

            star = new Star();
            planets = star.genSystem();

            genImages();

            string option;

            Utils.writeLog(Environment.NewLine + "System generation complete");

            systemListBox.Items.Clear();
            optionListBox.Items.Clear();

            systemListBox.Items.Add(Utils.getDescription(star.type));
            int sub = 0;

            for(int i = 0;i < planets.Count;i++)
            {
                if(planets[i].isBelt)
                {
                    option = Utils.getDescription(planets[i].type);
                    sub++;
                }
                else if(planets[i].type == ID.Belt.DWARF)
                {
                    option = String.Format("   1st: {0}",Utils.getDescription(planets[i].type));
                    sub++;
                }
                else
                    option = String.Format("{0,2}{1}: {2}",(i + 1 - sub),Utils.getOrdinal(i + 1 - sub),Utils.getDescription(planets[i].type));

                systemListBox.Items.Add(option);
            }

            systemListBox.SelectedIndex = 0;

            genProgressBar.Hide();
            genButton.Show();
            scaleLabel.Show();
        }

        private void systemListBox_SelectedIndexChanged(object sender,EventArgs e)
        {
            optionListBox.Items.Clear();
            if(systemListBox.SelectedIndex <= 0)
                showStarProps(star);
            else if(planets[systemListBox.SelectedIndex - 1].isBelt)
                showBeltProps(planets,planets[systemListBox.SelectedIndex - 1]);
            else
            {
                showPlanetProps(planets[systemListBox.SelectedIndex - 1]);
                if(planets[systemListBox.SelectedIndex - 1].moons != null)
                {
                    if(planets[systemListBox.SelectedIndex - 1].moons.Count > 0)
                    {
                        optionListBox.Items.Add("");
                        for(int i = 0;i < planets[systemListBox.SelectedIndex - 1].moons.Count;i++)
                        {
                            optionListBox.Items.Add(String.Format("{0,2}{1}: {2}",(i + 1),Utils.getOrdinal(i + 1),Utils.getDescription(planets[systemListBox.SelectedIndex - 1].moons[i].type)));
                        }
                        optionListBox.SelectedIndex = 0;
                    }
                }
            }

            if(optionListBox.Items.Count != 0)
                optionListBox.SelectedIndex = 0;
        }

        private void optionListBox_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(optionListBox.SelectedIndex <= 0)
                showPlanetProps(planets[systemListBox.SelectedIndex - 1]);
            else
                showMoonProps(planets[systemListBox.SelectedIndex - 1].moons[optionListBox.SelectedIndex - 1]);
        }

        private void systemListBox_KeyDown(object sender,System.Windows.Forms.KeyEventArgs e)
        {
            if(genProgressBar.Visible)
                return;

            int iSystemSelectedIndex = systemListBox.SelectedIndex;
            int iOptionSelectedIndex = optionListBox.SelectedIndex;

            //Control system listbox index
            if(e.KeyCode == Keys.W)
            {
                iSystemSelectedIndex -= 1;
                iOptionSelectedIndex = 0;
            }
            else if(e.KeyCode == Keys.S)
            {
                iSystemSelectedIndex += 1;
                iOptionSelectedIndex = 0;
            }

            //Control option listbox index
            if(e.KeyCode == Keys.A)
                iOptionSelectedIndex -= 1;
            else if(e.KeyCode == Keys.D)
                iOptionSelectedIndex += 1;

            //Handle rollover
            if(iSystemSelectedIndex >= systemListBox.Items.Count)
                iSystemSelectedIndex = 0;
            if(iSystemSelectedIndex < 0)
                iSystemSelectedIndex = systemListBox.Items.Count - 1;

            if(iOptionSelectedIndex >= optionListBox.Items.Count)
                iOptionSelectedIndex = 0;
            if(iOptionSelectedIndex < 0)
                iOptionSelectedIndex = optionListBox.Items.Count - 1;

            //Update indices
            if(systemListBox.Items.Count != 0)
                systemListBox.SelectedIndex = iSystemSelectedIndex;

            if(optionListBox.Items.Count != 0)
                optionListBox.SelectedIndex = iOptionSelectedIndex;
        }

        private void optionListBox_KeyDown(object sender,System.Windows.Forms.KeyEventArgs e)
        {
            systemListBox_KeyDown(sender,e);
        }

        private void showPlanetProps(Planet planet)
        {
            //Switch to the right groups
            pictureBox.Hide();

            blankGroupBox1.Hide();
            blankGroupBox2.Hide();
            blankGroupBox3.Hide();
            blankGroupBox4.Hide();

            propsPhysicalStarGroup.Hide();
            propsLightGroup.Hide();
            propsOrbitStarGroup.Hide();

            propsPhysicalBeltGroup.Hide();
            propsBulkBeltGroup.Hide();
            propsOrbitBeltGroup.Hide();

            propsPhysicalMoonGroup.Hide();

            propsPhysicalPlanetGroup.Show();
            propsBulkGroup.Show();
            propsOrbitPlanetGroup.Show();
            propsAtmoGroup.Show();

            //Populate tables

            //Physical
            if(!planet.isDwarf && planet.m > 317.8 * (1.0 - Gen.FUDGE_FACTOR))
            {
                propsPhysicalPlanetValueMass.Text = String.Format(UI.FORMAT,planet.m / Const.EARTHMASS_PER_JOVEMASS);
                propsPhysicalPlanetUnitMass.Text = "M♃";
            }
            else if(!planet.isDwarf && planet.m > 0.001)
            {
                propsPhysicalPlanetValueMass.Text = String.Format(UI.FORMAT,planet.m);
                propsPhysicalPlanetUnitMass.Text = "M🜨";
            }
            else
            {
                propsPhysicalPlanetValueMass.Text = String.Format(UI.FORMAT,planet.m * Const.ZETTAGRAMS_PER_EARTHMASS);
                propsPhysicalPlanetUnitMass.Text = "Zg";
            }

            if(!planet.isDwarf && planet.r > Gen.Planet.Giant.GAS_RADIUS_NORM * (1.0 - Gen.FUDGE_FACTOR))
            {
                propsPhysicalPlanetValueRadius.Text = String.Format(UI.FORMAT,planet.r / Gen.Planet.Giant.GAS_RADIUS_NORM);
                propsPhysicalPlanetUnitRadius.Text = "R♃";
            }
            else if(!planet.isDwarf)
            {
                propsPhysicalPlanetValueRadius.Text = String.Format(UI.FORMAT,planet.r);
                propsPhysicalPlanetUnitRadius.Text = "R🜨";
            }
            else
            {
                propsPhysicalPlanetValueRadius.Text = String.Format(UI.FORMAT,planet.r);
                propsPhysicalPlanetUnitRadius.Text = "km";
            }

            if(planet.g < 0.0001)
            {
                propsPhysicalPlanetValueGravity.Text = String.Format(UI.FORMAT,planet.g * Const.Earth.GRAVITY * 1000.0);
                propsPhysicalPlanetUnitGravity.Text = "mm/s²";
            }
            else
            {
                propsPhysicalPlanetValueGravity.Text = String.Format(UI.FORMAT,planet.g);
                propsPhysicalPlanetUnitGravity.Text = "G";
            }

            propsPhysicalPlanetValueEscV.Text = String.Format(UI.FORMAT,planet.escV / 1000.0);
            propsPhysicalPlanetValueTemp.Text = String.Format(UI.FORMAT,planet.t - Const.KELVIN);
            propsPhysicalPlanetValueAlbedo.Text = String.Format(UI.FORMAT,planet.albedo * 100.0);

            //Bulk
            propsBulkValueRock.Text = String.Format(UI.FORMAT,planet.bulkRock * 100.0);
            propsBulkValueMetal.Text = String.Format(UI.FORMAT,planet.bulkMetal * 100.0);
            propsBulkValueIce.Text = String.Format(UI.FORMAT,planet.bulkIces * 100.0);
            propsBulkLabelWater.Text = "Water";
            propsBulkValueWater.Text = String.Format(UI.FORMAT,planet.bulkWater * 100.0);
            propsBulkUnitWater.Text = "%";
            propsBulkLabelHydrogen.Text = "Hydrogen and Helium";
            propsBulkValueHydrogen.Text = String.Format(UI.FORMAT,planet.bulkNoble * 100.0);
            propsBulkUnitHydrogen.Text = "%";
            propsBulkValueDensity.Text = String.Format(UI.FORMAT,planet.bulkDensity);

            //Atmo
            if(planet.hasAir)
            {
                Label[][] labels = new Label[][]{
                    new Label[]{ propsAtmoLabelComp1, propsAtmoLabelComp2, propsAtmoLabelComp3, propsAtmoLabelComp4, propsAtmoLabelComp5, propsAtmoLabelComp6, propsAtmoLabelComp7},
                    new Label[]{ propsAtmoValueComp1, propsAtmoValueComp2, propsAtmoValueComp3, propsAtmoValueComp4, propsAtmoValueComp5, propsAtmoValueComp6, propsAtmoValueComp7},
                    new Label[]{ propsAtmoUnitComp1 , propsAtmoUnitComp2 , propsAtmoUnitComp3 , propsAtmoUnitComp4 , propsAtmoUnitComp5 , propsAtmoUnitComp6 , propsAtmoUnitComp7 }
                };

                for(int i = 0;i < labels[0].Length;i++)
                {
                    labels[0][i].ForeColor = Color.FromArgb(planet.atmo.comps[i].color);
                    labels[1][i].ForeColor = Color.FromArgb(planet.atmo.comps[i].color);
                    labels[2][i].ForeColor = Color.FromArgb(planet.atmo.comps[i].color);

                    labels[0][i].Text = planet.atmo.comps[i].name;

                    if(planet.atmo.comps[i].quantity * 100.0 >= 0.1)
                    {
                        labels[1][i].Text = String.Format(UI.FORMAT,planet.atmo.comps[i].quantity * 100.0);
                        labels[2][i].Text = "%";
                    }
                    else if(planet.atmo.comps[i].quantity * 100.0 < 0.1)
                    {
                        labels[1][i].Text = String.Format(UI.FORMAT,planet.atmo.comps[i].quantity * 10000);
                        labels[2][i].Text = "ppm";
                    }
                    else if(planet.atmo.comps[3].quantity * 100000.0 < 0.1)
                    {
                        labels[1][i].Text = String.Format(UI.FORMAT,planet.atmo.comps[i].quantity * 10000000);
                        labels[2][i].Text = "ppb";
                    }
                    else
                    {
                        labels[1][i].Text = String.Format(UI.FORMAT,planet.atmo.comps[i].quantity * 10000000000);
                        labels[2][i].Text = "ppt";
                    }
                }

                propsAtmoValueHeight.Text = String.Format(UI.FORMAT,planet.atmo.height / 1000.0);
                propsAtmoValueWeight.Text = String.Format(UI.FORMAT,planet.atmo.density);

                if(planet.isGiant)
                {
                    propsAtmoValuePressure.Text = "N/A   ";
                    propsAtmoUnitPressure.Text = "";
                }
                else
                {
                    propsAtmoValuePressure.Text = String.Format(UI.FORMAT,planet.atmo.pressure);
                    propsAtmoUnitPressure.Text = "atm";
                }

            }
            else
            {
                blankGroupBox3.Show();
                propsAtmoGroup.Hide();
            }

            //Orbit
            propsOrbitValueD.Text = String.Format(UI.FORMAT,planet.day);
            propsOrbitValueY.Text = String.Format(UI.FORMAT,planet.orbit.y);
            propsOrbitUnitA.Text = "y🜨";
            propsOrbitValueV.Text = String.Format(UI.FORMAT,planet.orbit.v);
            propsOrbitValueT.Text = String.Format(UI.FORMAT,planet.tilt);
            propsOrbitValueA.Text = String.Format(UI.FORMAT,planet.orbit.a);
            propsOrbitUnitA.Text = "AU";
            propsOrbitValueE.Text = String.Format(UI.FORMAT,planet.orbit.e * 100.0);
            propsOrbitValueI.Text = String.Format(UI.FORMAT,planet.orbit.i);
            propsOrbitValueL.Text = String.Format(UI.FORMAT,planet.orbit.l);
            if(planet.orbit.p < 0)
                propsOrbitValueP.Text = "undefined";
            else
                propsOrbitValueP.Text = String.Format(UI.FORMAT,planet.orbit.p);

            //Display flavor text

            flavorTextLabel.Text = planet.flavortext;

            //Draw planet

            drawPlanet(planet);
        }

        private void showBeltProps(List<Planet> planets,Planet belt)
        {
            pictureBox.Hide();

            blankGroupBox1.Hide();
            blankGroupBox2.Hide();
            blankGroupBox4.Hide();

            propsPhysicalStarGroup.Hide();
            propsLightGroup.Hide();
            propsOrbitStarGroup.Hide();

            propsPhysicalPlanetGroup.Hide();
            propsBulkGroup.Hide();
            propsOrbitPlanetGroup.Hide();
            propsAtmoGroup.Hide();

            propsPhysicalMoonGroup.Hide();

            propsPhysicalBeltGroup.Show();
            propsBulkBeltGroup.Show();
            blankGroupBox3.Show();
            propsOrbitBeltGroup.Show();

            //Physical
            propsPhysicalBeltValueMass.Text = String.Format(UI.FORMAT,belt.m);
            propsPhysicalBeltValueWidth.Text = String.Format(UI.FORMAT,belt.r);
            propsPhysicalBeltValueHeight.Text = String.Format(UI.FORMAT,belt.orbit.h);
            propsPhysicalBeltValueAlbedo.Text = String.Format(UI.FORMAT,belt.albedo * 100.0);

            //Bulk
            propsBulkBeltValueRock.Text = String.Format(UI.FORMAT,belt.bulkRock * 100.0);
            propsBulkBeltValueMetal.Text = String.Format(UI.FORMAT,belt.bulkMetal * 100.0);
            propsBulkBeltValueCarbon.Text = String.Format(UI.FORMAT,belt.bulkCarbon * 100.0);
            propsBulkBeltValueIce.Text = String.Format(UI.FORMAT,belt.bulkIces * 100.0);
            propsBulkBeltValueDensity.Text = String.Format(UI.FORMAT,belt.bulkDensity);

            //Orbit
            propsOrbitBeltValueD.Text = String.Format(UI.FORMAT,belt.day);
            propsOrbitBeltValueY.Text = String.Format(UI.FORMAT,belt.orbit.y);
            propsOrbitBeltValueV.Text = String.Format(UI.FORMAT,belt.orbit.v);

            propsOrbitBeltMuA.Text = String.Format(UI.FORMAT,belt.orbit.a);
            propsOrbitBeltMuE.Text = String.Format(UI.FORMAT,belt.orbit.e * 100.0);
            propsOrbitBeltMuI.Text = String.Format(UI.FORMAT,belt.orbit.i);
            propsOrbitBeltMuL.Text = String.Format(UI.FORMAT,belt.orbit.l);
            propsOrbitBeltMuP.Text = String.Format(UI.FORMAT,belt.orbit.p);

            propsOrbitBeltSigmaA.Text = String.Format(UI.FORMAT,belt.orbit.aSigma);
            propsOrbitBeltSigmaE.Text = String.Format(UI.FORMAT,belt.orbit.eSigma * 100.0);
            propsOrbitBeltSigmaI.Text = String.Format(UI.FORMAT,belt.orbit.iSigma);
            propsOrbitBeltSigmaL.Text = String.Format(UI.FORMAT,belt.orbit.lSigma);
            propsOrbitBeltSigmaP.Text = String.Format(UI.FORMAT,belt.orbit.pSigma);

            //Flavor text

            flavorTextLabel.Text = belt.flavortext;

            //Draw planet

            drawPlanet(belt);
        }

        private void showStarProps(Star star)
        {
            pictureBox.Hide();

            blankGroupBox1.Hide();
            blankGroupBox2.Hide();
            blankGroupBox4.Hide();

            propsPhysicalBeltGroup.Hide();
            propsBulkBeltGroup.Hide();
            propsOrbitBeltGroup.Hide();

            propsPhysicalPlanetGroup.Hide();
            propsBulkGroup.Hide();
            propsOrbitPlanetGroup.Hide();
            propsAtmoGroup.Hide();

            propsPhysicalMoonGroup.Hide();

            propsPhysicalStarGroup.Show();
            propsLightGroup.Show();
            blankGroupBox3.Show();
            propsOrbitStarGroup.Show();

            //Physical
            propsPhysicalStarValueMass.Text = String.Format(UI.FORMAT,star.m);
            propsPhysicalStarValueRadius.Text = String.Format(UI.FORMAT,star.r);
            propsPhysicalStarValueGravity.Text = String.Format(UI.FORMAT,star.g);
            propsPhysicalStarValueEscV.Text = String.Format(UI.FORMAT,star.escV / 60.0);
            propsPhysicalStarValueMetal.Text = String.Format(UI.FORMAT,star.metal * 100.0);
            propsPhysicalStarValueLife.Text = String.Format(UI.FORMAT,star.life);

            //Radiance
            propsLightValueLumin.Text = String.Format(UI.FORMAT,star.lumin);
            propsLightValueTemp.Text = String.Format(UI.FORMAT,star.temp);
            propsLightValueBV.Text = String.Format(UI.FORMAT,star.bv);
            propsLightValueAbs.Text = String.Format(UI.FORMAT,star.magAbs);
            propsLightValueRel.Text = String.Format(UI.FORMAT,star.magRel);

            //Orbit
            propsOrbitValueGY.Text = String.Format(UI.FORMAT,star.y);
            propsOrbitValueFZMin.Text = String.Format(UI.FORMAT,star.zoneFormMin);
            propsOrbitValueFZMax.Text = String.Format(UI.FORMAT,star.zoneFormMax);
            propsOrbitValueGZMin.Text = String.Format(UI.FORMAT,star.zoneHabMin);
            propsOrbitValueGZMax.Text = String.Format(UI.FORMAT,star.zoneHabMax);
            propsOrbitValueFL.Text = String.Format(UI.FORMAT,star.zoneFrost);

            //Flavor text
            flavorTextLabel.Text = star.flavortext;

            //Draw planet

            drawStar(star);
        }

        private void showMoonProps(Moon moon)
        {
            //Switch to the right groups
            pictureBox.Hide();

            blankGroupBox1.Hide();
            blankGroupBox2.Hide();
            blankGroupBox3.Hide();
            blankGroupBox4.Hide();

            propsPhysicalStarGroup.Hide();
            propsLightGroup.Hide();
            propsOrbitStarGroup.Hide();

            propsPhysicalBeltGroup.Hide();
            propsBulkBeltGroup.Hide();

            propsPhysicalPlanetGroup.Hide();
            propsAtmoGroup.Hide();

            propsPhysicalMoonGroup.Show();
            propsBulkGroup.Show();
            propsAtmoGroup.Show();
            propsOrbitPlanetGroup.Show();
            propsOrbitBeltGroup.Show();

            //Populate tables

            //Physical
            if(moon.m < 0.001)
            {
                propsPhysicalMoonValueMass.Text = String.Format(UI.FORMAT,moon.m * Const.ZETTAGRAMS_PER_EARTHMASS);
                propsPhysicalMoonUnitMass.Text = "Zg";
            }
            else
            {
                propsPhysicalMoonValueMass.Text = String.Format(UI.FORMAT,moon.m);
                propsPhysicalMoonUnitMass.Text = "M🜨";
            }
            propsPhysicalMoonValueRadA.Text = String.Format(UI.FORMAT,(moon.rA + moon.rB + moon.rC) / 3.0);
            propsPhysicalMoonValueGravity.Text = String.Format(UI.FORMAT,moon.g * Const.Earth.GRAVITY);
            propsPhysicalMoonValueEscV.Text = String.Format(UI.FORMAT,moon.escV / 1000.0);
            propsPhysicalMoonValueTemp.Text = String.Format(UI.FORMAT,moon.t - Const.KELVIN);
            propsPhysicalMoonValueAlbedo.Text = String.Format(UI.FORMAT,moon.albedo * 100.0);

            //Bulk
            propsBulkValueRock.Text = String.Format(UI.FORMAT,moon.bulkRock * 100.0);
            propsBulkValueMetal.Text = String.Format(UI.FORMAT,moon.bulkMetal * 100.0);
            propsBulkValueIce.Text = String.Format(UI.FORMAT,moon.bulkIces * 100.0);
            propsBulkLabelWater.Text = propsBulkValueWater.Text = propsBulkUnitWater.Text = "";
            propsBulkLabelHydrogen.Text = propsBulkValueHydrogen.Text = propsBulkUnitHydrogen.Text = "";
            propsBulkValueDensity.Text = String.Format(UI.FORMAT,moon.bulkDensity);

            //Atmo
            if(moon.hasAir)
            {
                Label[][] labels = new Label[][]{
                    new Label[]{ propsAtmoLabelComp1, propsAtmoLabelComp2, propsAtmoLabelComp3, propsAtmoLabelComp4, propsAtmoLabelComp5 },
                    new Label[]{ propsAtmoValueComp1, propsAtmoValueComp2, propsAtmoValueComp3, propsAtmoValueComp4, propsAtmoValueComp5 },
                    new Label[]{ propsAtmoUnitComp1 , propsAtmoUnitComp2 , propsAtmoUnitComp3 , propsAtmoUnitComp4 , propsAtmoUnitComp5  }
                };

                for(int i = 0;i < labels[0].Length;i++)
                {
                    labels[0][i].ForeColor = Color.FromArgb(moon.atmo.comps[i].color);
                    labels[1][i].ForeColor = Color.FromArgb(moon.atmo.comps[i].color);
                    labels[2][i].ForeColor = Color.FromArgb(moon.atmo.comps[i].color);

                    labels[0][i].Text = moon.atmo.comps[i].name;

                    if(moon.atmo.comps[i].quantity * 100.0 >= 0.1)
                    {
                        labels[1][i].Text = String.Format(UI.FORMAT,moon.atmo.comps[i].quantity * 100.0);
                    }
                    else if(moon.atmo.comps[i].quantity * 100.0 < 0.1)
                    {
                        labels[1][i].Text = String.Format(UI.FORMAT,moon.atmo.comps[i].quantity * 10000);
                        labels[2][i].Text = "ppm";
                    }
                    else if(moon.atmo.comps[3].quantity * 100000.0 < 0.1)
                    {
                        labels[1][i].Text = String.Format(UI.FORMAT,moon.atmo.comps[i].quantity * 10000000);
                        labels[2][i].Text = "ppb";
                    }
                    else
                    {
                        labels[1][i].Text = String.Format(UI.FORMAT,moon.atmo.comps[i].quantity * 10000000000);
                        labels[2][i].Text = "ppt";
                    }
                }

                propsAtmoValueHeight.Text = String.Format(UI.FORMAT,moon.atmo.height / 1000.0);
                propsAtmoValueWeight.Text = String.Format(UI.FORMAT,moon.atmo.density);

                propsAtmoValuePressure.Text = String.Format(UI.FORMAT,moon.atmo.pressure);
                propsAtmoUnitPressure.Text = "atm";

            }
            else
            {
                blankGroupBox3.Show();
                propsAtmoGroup.Hide();
            }


            //Orbit
            if(moon.type != ID.Sat.MOONC)
            {
                propsOrbitBeltGroup.Hide();
                propsOrbitValueD.Text = String.Format(UI.FORMAT,moon.day);
                if(moon.orbit.y >= 1000.0)
                {
                    propsOrbitValueY.Text = String.Format(UI.FORMAT,moon.orbit.y / Const.Earth.YEAR);
                    propsOrbitUnitY.Text = "y🜨";
                }
                else
                {
                    propsOrbitValueY.Text = String.Format(UI.FORMAT,moon.orbit.y);
                    propsOrbitUnitY.Text = "d🜨";
                }
                propsOrbitValueV.Text = String.Format(UI.FORMAT,moon.orbit.v / 1000.0);
                propsOrbitValueT.Text = String.Format(UI.FORMAT,moon.tilt);
                if(moon.orbit.a > 9999.0)
                {
                    propsOrbitValueA.Text = String.Format(UI.FORMAT,moon.orbit.a * Const.AU_PER_EARTHRADIUS);
                    propsOrbitUnitA.Text = "AU";
                }
                else
                {
                    propsOrbitValueA.Text = String.Format(UI.FORMAT,moon.orbit.a);
                    propsOrbitUnitA.Text = "R🜨";
                }
                propsOrbitValueE.Text = String.Format(UI.FORMAT,moon.orbit.e * 100.0);
                propsOrbitValueI.Text = String.Format(UI.FORMAT,moon.orbit.i);
                propsOrbitValueL.Text = String.Format(UI.FORMAT,moon.orbit.l);
                propsOrbitValueP.Text = String.Format(UI.FORMAT,moon.orbit.p);
            }
            else
            {
                propsOrbitPlanetGroup.Hide();
                propsOrbitBeltValueD.Text = String.Format(UI.FORMAT,moon.day);
                if(moon.orbit.y >= 1000.0)
                {
                    propsOrbitBeltValueY.Text = String.Format(UI.FORMAT,moon.orbit.y / Const.Earth.YEAR);
                    propsOrbitBeltUnitY.Text = "y🜨";
                }
                else
                {
                    propsOrbitBeltValueY.Text = String.Format(UI.FORMAT,moon.orbit.y);
                    propsOrbitBeltUnitY.Text = "d🜨";
                }
                propsOrbitBeltValueV.Text = String.Format(UI.FORMAT,moon.orbit.v / 1000.0);
                if(moon.orbit.a > 9999.0)
                {
                    propsOrbitBeltMuA.Text = String.Format(UI.FORMAT,moon.orbit.a * Const.AU_PER_EARTHRADIUS);
                    propsOrbitBeltUnitA.Text = "AU";
                }
                else
                {
                    propsOrbitBeltMuA.Text = String.Format(UI.FORMAT,moon.orbit.a);
                    propsOrbitBeltUnitA.Text = "R🜨";
                }
                propsOrbitBeltMuE.Text = String.Format(UI.FORMAT,moon.orbit.e * 100.0);
                propsOrbitBeltMuI.Text = String.Format(UI.FORMAT,moon.orbit.i);
                propsOrbitBeltMuL.Text = String.Format(UI.FORMAT,moon.orbit.l);
                propsOrbitBeltMuP.Text = String.Format(UI.FORMAT,moon.orbit.p);

                propsOrbitBeltSigmaA.Text = String.Format(UI.FORMAT,moon.orbit.aSigma);
                propsOrbitBeltSigmaE.Text = String.Format(UI.FORMAT,moon.orbit.eSigma * 100.0);
                propsOrbitBeltSigmaI.Text = String.Format(UI.FORMAT,moon.orbit.iSigma);
                propsOrbitBeltSigmaL.Text = String.Format(UI.FORMAT,moon.orbit.lSigma);
                propsOrbitBeltSigmaP.Text = String.Format(UI.FORMAT,moon.orbit.pSigma);
            }

            //Flavor text
            flavorTextLabel.Text = moon.flavortext;

            //Draw moon
            drawMoon(moon);
        }

        private void drawPlanet(Planet planet)
        {
            pictureBox.Hide();

            pictureBox.Image = planet.image;

            if(planet.isGiant && (planet.r / Gen.Planet.Giant.GAS_RADIUS_NORM) > 1.1)
                scaleLabel.Text = String.Format("1/{0:D} Scale",(int)Math.Round(1.0 / UI.SCALE_BIG));
            else if(planet.isDwarf)
                scaleLabel.Text = String.Format("1/{0:D} Scale",(int)Math.Round(1.0 / UI.SCALE_SMALL));
            else
                scaleLabel.Text = String.Format("1/{0:D} Scale",(int)Math.Round(1.0 / UI.SCALE_MID));


            pictureBox.Show();
        }

        private void drawMoon(Moon moon)
        {
            pictureBox.Hide();

            pictureBox.Image = moon.image;

            if(moon.isMajor)
                scaleLabel.Text = String.Format("1/{0:D} Scale",(int)Math.Round(1.0 / UI.SCALE_MAJOR));
            else
                scaleLabel.Text = String.Format("1/{0:D} Scale",(int)Math.Round(1.0 / UI.SCALE_SMALL));

            pictureBox.Show();
        }

        private void drawStar(Star star)
        {
            pictureBox.Hide();

            pictureBox.Image = star.image;

            scaleLabel.Text = String.Format("1/{0:D} Scale",(int)Math.Round(1.0 / UI.SCALE_STAR));

            pictureBox.Show();
        }

        private void genImages()
        {
            genButton.Hide();

            int count = 1;
            double scale = 0;

            foreach(Planet p in planets)
            {
                if(!p.isBelt)
                    count++;

                if(p.moons != null)
                    foreach(Moon m in p.moons)
                        if(m.isMajor)
                            count++;
            }

            genProgressBar.Value = 0;
            genProgressBar.Maximum = count;
            genProgressBar.ForeColor = Color.Lime;
            genProgressBar.Show();

            star.genImage(pictureBox.Width,pictureBox.Height);

            foreach(Planet p in planets)
            {
                if(p.isBelt)
                    continue;

                if(p.moons != null)
                {
                    foreach(Moon m in p.moons)
                    {
                        if(m.isMajor)
                        {
                            if(m.isMajor && !p.isDwarf)
                                scale = UI.SCALE_MAJOR;
                            else
                                scale = UI.SCALE_SMALL;

                            m.genImage(pictureBox.Width,pictureBox.Height,false,scale);
                            Utils.updateProgress();
                        }
                    }
                }

                if(p.isGiant && (p.r / Gen.Planet.Giant.GAS_RADIUS_NORM) > 1.1)
                    scale = UI.SCALE_BIG;
                else if(p.isDwarf)
                    scale = UI.SCALE_SMALL / Const.Earth.RADIUS;
                else
                    scale = UI.SCALE_MID;

                p.genImage(pictureBox.Width,pictureBox.Height,true,scale);
                Utils.updateProgress();
            }
        }
    }
}
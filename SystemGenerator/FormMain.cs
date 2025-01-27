using System.Diagnostics.Eventing.Reader;
using SystemGenerator.Generation;

namespace SystemGenerator
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private Star star;
        private List<Planet> planets;

        private void FormMain_Load(object sender, EventArgs e)
        {
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

            propsPhysicalBeltGroup.Location = propsPhysicalPlanetGroup.Location;
            propsPhysicalStarGroup.Location = propsPhysicalPlanetGroup.Location;
            propsPhysicalMoonGroup.Location = propsPhysicalPlanetGroup.Location;

            propsBulkBeltGroup.Location = propsBulkGroup.Location;
            propsLightGroup.Location = propsBulkGroup.Location;

            propsOrbitBeltGroup.Location = propsOrbitPlanetGroup.Location;
            propsOrbitStarGroup.Location = propsOrbitPlanetGroup.Location;

            this.Show();
            int height = Screen.PrimaryScreen.Bounds.Height;
            int width = Screen.PrimaryScreen.Bounds.Width;

            //Rescale widgets
            textGroupBox.Location = new Point(optionListBox.Location.X, optionListBox.Location.Y + optionListBox.Height);

        }

        private void systemListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            optionListBox.Items.Clear();
            if (systemListBox.SelectedIndex == 0)
                showStarProps(star);
            else if (planets[systemListBox.SelectedIndex - 1].isBelt)
                showBeltProps(planets[systemListBox.SelectedIndex - 1]);
            else
            {
                showPlanetProps(planets[systemListBox.SelectedIndex - 1]);
                if (planets[systemListBox.SelectedIndex - 1].moons != null)
                {
                    if (planets[systemListBox.SelectedIndex - 1].moons.Count > 0)
                    {
                        optionListBox.Items.Add("");
                        for (int i = 0; i < planets[systemListBox.SelectedIndex - 1].moons.Count; i++)
                        {
                            optionListBox.Items.Add(String.Format("{0,2}{1}: {2}", (i + 1), Utils.getOrdinal(i + 1), Utils.getDescription(planets[systemListBox.SelectedIndex - 1].moons[i].type)));
                        }
                        optionListBox.SelectedIndex = 0;
                    }
                }
            }
        }

        private void optionListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optionListBox.SelectedIndex == 0)
                showPlanetProps(planets[systemListBox.SelectedIndex - 1]);
            else
                showMoonProps(planets[systemListBox.SelectedIndex - 1].moons[optionListBox.SelectedIndex - 1]);
        }

        private void showPlanetProps(Planet planet)
        {
            //Switch to the right groups
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
            if (!planet.isDwarf && planet.m > 317.8 * (1.0 - Gen.FUDGE_FACTOR))
            {
                propsPhysicalPlanetValueMass.Text = String.Format(Const.FORMAT, planet.m / 317.8);
                propsPhysicalPlanetUnitMass.Text = "M♃";
            }
            else if (!planet.isDwarf && planet.m > 0.001)
            {
                propsPhysicalPlanetValueMass.Text = String.Format(Const.FORMAT, planet.m);
                propsPhysicalPlanetUnitMass.Text = "M🜨";
            }
            else
            {
                propsPhysicalPlanetValueMass.Text = String.Format(Const.FORMAT, planet.m * 5972200.0);
                propsPhysicalPlanetUnitMass.Text = "Zg";
            }

            if (!planet.isDwarf && planet.r > Gen.Planet.Giant.GAS_RADIUS_NORM * (1.0 - Gen.FUDGE_FACTOR))
            {
                propsPhysicalPlanetValueRadius.Text = String.Format(Const.FORMAT, planet.r / Gen.Planet.Giant.GAS_RADIUS_NORM);
                propsPhysicalPlanetUnitRadius.Text = "R♃";
            }
            else if (!planet.isDwarf)
            {
                propsPhysicalPlanetValueRadius.Text = String.Format(Const.FORMAT, planet.r);
                propsPhysicalPlanetUnitRadius.Text = "R🜨";
            }
            else
            {
                propsPhysicalPlanetValueRadius.Text = String.Format(Const.FORMAT, planet.r);
                propsPhysicalPlanetUnitRadius.Text = "km";
            }

            propsPhysicalPlanetValueGravity.Text = String.Format(Const.FORMAT, planet.g);
            propsPhysicalPlanetValueEscV.Text = String.Format(Const.FORMAT, planet.escV / 1000.0);
            propsPhysicalPlanetValueTemp.Text = String.Format(Const.FORMAT, planet.t);
            propsPhysicalPlanetValueAlbedo.Text = String.Format(Const.FORMAT, planet.albedo * 100.0);

            //Bulk
            propsBulkValueRock.Text = String.Format(Const.FORMAT, planet.bulkRock * 100.0);
            propsBulkValueMetal.Text = String.Format(Const.FORMAT, planet.bulkMetal * 100.0);
            propsBulkValueIce.Text = String.Format(Const.FORMAT, planet.bulkIces * 100.0);
            propsBulkLabelWater.Text = "Water";
            propsBulkValueWater.Text = String.Format(Const.FORMAT, planet.bulkWater * 100.0);
            propsBulkUnitWater.Text = "%";
            propsBulkLabelHydrogen.Text = "Hydrogen and Helium";
            propsBulkValueHydrogen.Text = String.Format(Const.FORMAT, planet.bulkNoble * 100.0);
            propsBulkUnitHydrogen.Text = "%";
            propsBulkValueDensity.Text = String.Format(Const.FORMAT, planet.bulkDensity);

            //Atmo
            if (planet.hasAir)
            {
                Label[][] labels = new Label[][]{
                    new Label[]{ propsAtmoLabelComp1, propsAtmoLabelComp2, propsAtmoLabelComp3, propsAtmoLabelComp4, propsAtmoLabelComp5 },
                    new Label[]{ propsAtmoValueComp1, propsAtmoValueComp2, propsAtmoValueComp3, propsAtmoValueComp4, propsAtmoValueComp5 },
                    new Label[]{ propsAtmoUnitComp1 , propsAtmoUnitComp2 , propsAtmoUnitComp3 , propsAtmoUnitComp4 , propsAtmoUnitComp5  }
                };

                for (int i = 0; i < labels[0].Length; i++)
                {
                    labels[0][i].ForeColor = Color.FromArgb(planet.atmo.comps[i].color);
                    labels[1][i].ForeColor = Color.FromArgb(planet.atmo.comps[i].color);
                    labels[2][i].ForeColor = Color.FromArgb(planet.atmo.comps[i].color);

                    labels[0][i].Text = planet.atmo.comps[i].name;

                    if (planet.atmo.comps[i].quantity * 100.0 >= 0.1)
                    {
                        labels[1][i].Text = String.Format(Const.FORMAT, planet.atmo.comps[i].quantity * 100.0);
                    }
                    else if (planet.atmo.comps[i].quantity * 100.0 < 0.1)
                    {
                        labels[1][i].Text = String.Format(Const.FORMAT, planet.atmo.comps[i].quantity * 10000);
                        labels[2][i].Text = "ppm";
                    }
                    else if (planet.atmo.comps[3].quantity * 100000.0 < 0.1)
                    {
                        labels[1][i].Text = String.Format(Const.FORMAT, planet.atmo.comps[i].quantity * 10000000);
                        labels[2][i].Text = "ppb";
                    }
                    else
                    {
                        labels[1][i].Text = String.Format(Const.FORMAT, planet.atmo.comps[i].quantity * 10000000000);
                        labels[2][i].Text = "ppt";
                    }
                }

                propsAtmoValueHeight.Text = String.Format(Const.FORMAT, planet.atmo.height / 1000.0);
                propsAtmoValueWeight.Text = String.Format(Const.FORMAT, planet.atmo.density);
                propsAtmoValuePressure.Text = String.Format(Const.FORMAT, planet.atmo.pressure);
            }
            else
                propsAtmoGroup.Hide();

            //Orbit
            propsOrbitValueD.Text = String.Format(Const.FORMAT, planet.day);
            propsOrbitValueY.Text = String.Format(Const.FORMAT, planet.orbit.y);
            propsOrbitUnitA.Text = "y🜨";
            propsOrbitValueV.Text = String.Format(Const.FORMAT, planet.orbit.v);
            propsOrbitValueT.Text = String.Format(Const.FORMAT, planet.tilt);
            propsOrbitValueA.Text = String.Format(Const.FORMAT, planet.orbit.a);
            propsOrbitUnitA.Text = "AU";
            propsOrbitValueE.Text = String.Format(Const.FORMAT, planet.orbit.e * 100.0);
            propsOrbitValueI.Text = String.Format(Const.FORMAT, planet.orbit.i);
            propsOrbitValueL.Text = String.Format(Const.FORMAT, planet.orbit.l);
            propsOrbitValueP.Text = String.Format(Const.FORMAT, planet.orbit.p);

            //Build flavor text
            string flavor = "";

            flavor += "This planet is " + Utils.getLongDesc(planet) + ". ";

            if (planet.hasAir)
            {
                flavor += "It has a";
                switch (planet.atmo.typeMinor)
                {
                    case ID.Atmo.MNR_CRYOAZURIAN:
                        flavor += " cryoazuri";
                        break;
                    case ID.Atmo.MNR_FRIGIDIAN:
                        flavor += " frigi";
                        break;
                    case ID.Atmo.MNR_NEONEAN:
                        flavor += " neono";
                        break;
                    case ID.Atmo.MNR_BOREAN:
                        flavor += " boreo";
                        break;
                    case ID.Atmo.MNR_METHANEAN:
                        flavor += " metho";
                        break;
                    case ID.Atmo.MNR_MESOAZURIAN:
                        flavor += " mesoazuri";
                        break;
                    case ID.Atmo.MNR_THOLIAN:
                        flavor += " tholi";
                        break;
                    case ID.Atmo.MNR_SULFANIAN:
                        flavor += " sulfa";
                        break;
                    case ID.Atmo.MNR_AMMONIAN:
                        flavor += "n ammo";
                        break;
                    case ID.Atmo.MNR_HYDRONIAN:
                        flavor += " hydro";
                        break;
                    case ID.Atmo.MNR_ACIDIAN:
                        flavor += "n acidi";
                        break;
                    case ID.Atmo.MNR_PYROAZURIAN:
                        flavor += " pyroazuri";
                        break;
                    case ID.Atmo.MNR_SULFOLIAN:
                        flavor += " sulfoli";
                        break;
                    case ID.Atmo.MNR_AITHALIAN:
                        flavor += "n aithali";
                        break;
                }

                flavor += "-";

                switch (planet.atmo.typeMajor)
                {
                    case ID.Atmo.MJR_JOTUNNIAN:
                        flavor += "jotunnian atmosphere, largely composed of hydrogen and helium. ";
                        break;
                    case ID.Atmo.MJR_HELIAN:
                        flavor += "helian atmosphere, largely composed of helium. ";
                        break;
                    case ID.Atmo.MJR_YDATRIAN:
                        flavor += "ydatrian atmosphere, largely composed of simple hydride compounds. ";
                        break;
                    case ID.Atmo.MJR_RHEAN:
                        flavor += "rhean atmosphere, largely composed of nitrogen. ";
                        break;
                    case ID.Atmo.MJR_MINERVAN:
                        flavor += "minervan atmosphere, largely composed of compounds of nonmetals. ";
                        break;
                    case ID.Atmo.MJR_EDELIAN:
                        flavor += "edelian atmosphere, largely composed of neon and argon. ";
                        break;
                }

                switch (planet.atmo.typeMinor)
                {
                    case ID.Atmo.MNR_CRYOAZURIAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with thin, hydrocarbon-based hazes.";
                        break;
                    case ID.Atmo.MNR_FRIGIDIAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with " + planet.atmo.colorCloudName + " clouds of condensed hydrogen.";
                        break;
                    case ID.Atmo.MNR_NEONEAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with " + planet.atmo.colorCloudName + " clouds of neon.";
                        break;
                    case ID.Atmo.MNR_BOREAN:
                        flavor += "Hazes have made the sky " + planet.atmo.colorName + ", with " + planet.atmo.colorCloudName + " clouds of nitrogen and carbon monoxide.";
                        break;
                    case ID.Atmo.MNR_METHANEAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with " + planet.atmo.colorCloudName + " hazes of organic chemicals.";
                        break;
                    case ID.Atmo.MNR_MESOAZURIAN:
                        flavor += "The sky is a clear " + planet.atmo.colorName + ", with slight " + planet.atmo.colorCloudName + " hazes.";
                        break;
                    case ID.Atmo.MNR_THOLIAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with " + planet.atmo.colorCloudName + " hazes of hydrocarbons and organosulfurs.";
                        break;
                    case ID.Atmo.MNR_SULFANIAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with " + planet.atmo.colorCloudName + " clouds of hydrogen and ammonium sulfide.";
                        break;
                    case ID.Atmo.MNR_AMMONIAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with " + planet.atmo.colorCloudName + " clouds of ammonia and hydrogen sulfde.";
                        break;
                    case ID.Atmo.MNR_HYDRONIAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with " + planet.atmo.colorCloudName + " clouds of water vapor.";
                        break;
                    case ID.Atmo.MNR_ACIDIAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with " + planet.atmo.colorCloudName + " clouds of sulfuric acid.";
                        break;
                    case ID.Atmo.MNR_PYROAZURIAN:
                        flavor += "The clouds on this planet are very thin, making the atmosphere " + planet.atmo.colorName + ".";
                        break;
                    case ID.Atmo.MNR_SULFOLIAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with sulfurous " + planet.atmo.colorCloudName + " clouds.";
                        break;
                    case ID.Atmo.MNR_AITHALIAN:
                        flavor += "The sky is " + planet.atmo.colorName + ", with carbonaceous " + planet.atmo.colorCloudName + " clouds.";
                        break;
                }

                if (planet.atmo.typeMinor != ID.Atmo.MNR_CRYOAZURIAN && planet.atmo.typeMinor != ID.Atmo.MNR_PYROAZURIAN && planet.surface.coverCloudThick + planet.surface.coverCloudThin > 0.0)
                    flavor += String.Format(" They cover about {0:N0}% of the planet.", (planet.surface.coverCloudThick + planet.surface.coverCloudThin) * 100.0);
            }
            else
                flavor += " It has no atmosphere, as it lacks a strong enough magnetosphere.";

            flavor += " " + planet.feature;

            if (planet.isGiant && planet.ringsVisible)
                flavor += " The planet's rings are icy and clearly visible.";
            else if (planet.isGiant && !planet.ringsVisible)
                flavor += " The planet's rings are dusty and faint.";

            if (planet.moons.Count > 0)
            {
                int rockMinor = 0, rockMajor = 0, icyMinor = 0, icyMajor = 0;
                for (int i = 0; i < planet.moons.Count; i++)
                {
                    if (planet.moons[i].isMajor && planet.moons[i].isIcy)
                        icyMajor++;
                    else if (planet.moons[i].isMajor && !planet.moons[i].isIcy)
                        rockMajor++;
                    else if (!planet.moons[i].isMajor && planet.moons[i].isIcy)
                        icyMinor++;
                    else if (!planet.moons[i].isMajor && !planet.moons[i].isIcy)
                        rockMinor++;
                }

                if (planet.moons.Count > 1)
                    flavor += " " + planet.moons.Count + " satellites orbit this planet:";
                else
                    flavor += " " + planet.moons.Count + " satellite orbits this planet:";

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

            if (planet.hasTrojans)
                flavor += " It has also captured swarms of trojans from the asteroid belt.";

            flavorTextLabel.Text = flavor;
        }

        private void showBeltProps(Planet belt)
        {
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
            propsOrbitBeltGroup.Show();

            //Physical
            propsPhysicalBeltValueMass.Text = String.Format(Const.FORMAT, belt.m);
            propsPhysicalBeltValueWidth.Text = String.Format(Const.FORMAT, belt.r);
            propsPhysicalBeltValueHeight.Text = String.Format(Const.FORMAT, belt.orbit.h);
            propsPhysicalBeltValueAlbedo.Text = String.Format(Const.FORMAT, belt.albedo * 100.0);

            //Bulk
            propsBulkBeltValueRock.Text = String.Format(Const.FORMAT, belt.bulkRock * 100.0);
            propsBulkBeltValueMetal.Text = String.Format(Const.FORMAT, belt.bulkMetal * 100.0);
            propsBulkBeltValueCarbon.Text = String.Format(Const.FORMAT, belt.bulkCarbon * 100.0);
            propsBulkBeltValueIce.Text = String.Format(Const.FORMAT, belt.bulkIces * 100.0);
            propsBulkBeltValueDensity.Text = String.Format(Const.FORMAT, belt.bulkDensity);

            //Orbit
            propsOrbitBeltValueD.Text = String.Format(Const.FORMAT, belt.day);
            propsOrbitBeltValueY.Text = String.Format(Const.FORMAT, belt.orbit.y);
            propsOrbitBeltValueV.Text = String.Format(Const.FORMAT, belt.orbit.v);

            propsOrbitBeltMuA.Text = String.Format(Const.FORMAT, belt.orbit.a);
            propsOrbitBeltMuE.Text = String.Format(Const.FORMAT, belt.orbit.e * 100.0);
            propsOrbitBeltMuI.Text = String.Format(Const.FORMAT, belt.orbit.i);
            propsOrbitBeltMuL.Text = String.Format(Const.FORMAT, belt.orbit.l);
            propsOrbitBeltMuP.Text = String.Format(Const.FORMAT, belt.orbit.p);

            propsOrbitBeltSigmaA.Text = String.Format(Const.FORMAT, belt.orbit.aSigma);
            propsOrbitBeltSigmaE.Text = String.Format(Const.FORMAT, belt.orbit.eSigma * 100.0);
            propsOrbitBeltSigmaI.Text = String.Format(Const.FORMAT, belt.orbit.iSigma);
            propsOrbitBeltSigmaL.Text = String.Format(Const.FORMAT, belt.orbit.lSigma);
            propsOrbitBeltSigmaP.Text = String.Format(Const.FORMAT, belt.orbit.pSigma);

            //Flavor text
            flavorTextLabel.Text = "";
        }

        private void showStarProps(Star star)
        {
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
            propsOrbitStarGroup.Show();

            //Physical
            propsPhysicalStarValueMass.Text = String.Format(Const.FORMAT, star.m);
            propsPhysicalStarValueRadius.Text = String.Format(Const.FORMAT, star.r);
            propsPhysicalStarValueGravity.Text = String.Format(Const.FORMAT, star.g);
            propsPhysicalStarValueEscV.Text = String.Format(Const.FORMAT, star.escV / 60.0);
            propsPhysicalStarValueMetal.Text = String.Format(Const.FORMAT, star.metal * 100.0);
            propsPhysicalStarValueLife.Text = String.Format(Const.FORMAT, star.life);

            //Radiance
            propsLightValueLumin.Text = String.Format(Const.FORMAT, star.lumin);
            propsLightValueTemp.Text = String.Format(Const.FORMAT, star.temp);
            propsLightValueBV.Text = String.Format(Const.FORMAT, star.bv);
            propsLightValueAbs.Text = String.Format(Const.FORMAT, star.magAbs);
            propsLightValueRel.Text = String.Format(Const.FORMAT, star.magRel);

            //Orbit
            propsOrbitValueGY.Text = String.Format(Const.FORMAT, star.y);
            propsOrbitValueFZMin.Text = String.Format(Const.FORMAT, star.zoneFormMin);
            propsOrbitValueFZMax.Text = String.Format(Const.FORMAT, star.zoneFormMax);
            propsOrbitValueGZMin.Text = String.Format(Const.FORMAT, star.zoneHabMin);
            propsOrbitValueGZMax.Text = String.Format(Const.FORMAT, star.zoneHabMax);
            propsOrbitValueFL.Text = String.Format(Const.FORMAT, star.zoneFrost);

            //Flavor text
            flavorTextLabel.Text = "";
        }

        private void showMoonProps(Moon moon)
        {

            //Switch to the right groups
            propsPhysicalStarGroup.Hide();
            propsLightGroup.Hide();
            propsOrbitStarGroup.Hide();

            propsPhysicalBeltGroup.Hide();
            propsBulkBeltGroup.Hide();

            propsPhysicalPlanetGroup.Hide();
            propsAtmoGroup.Hide();

            propsPhysicalMoonGroup.Show();
            propsBulkGroup.Show();
            propsOrbitPlanetGroup.Show();
            propsOrbitBeltGroup.Show();

            //Populate tables

            //Physical
            if (moon.m < 0.001)
            {
                propsPhysicalMoonValueMass.Text = String.Format(Const.FORMAT, moon.m * Const.ZETTAGRAMS_PER_EARTHMASS);
                propsPhysicalMoonUnitMass.Text = "Zg";
            }
            else
            {
                propsPhysicalMoonValueMass.Text = String.Format(Const.FORMAT, moon.m);
                propsPhysicalMoonUnitMass.Text = "M🜨";
            }
            propsPhysicalMoonValueRadA.Text = String.Format(Const.FORMAT, moon.rA);
            propsPhysicalMoonValueRadB.Text = String.Format(Const.FORMAT, moon.rB);
            propsPhysicalMoonValueRadC.Text = String.Format(Const.FORMAT, moon.rC);
            propsPhysicalMoonValueGravity.Text = String.Format(Const.FORMAT, moon.g * Const.Earth.GRAVITY);
            propsPhysicalMoonValueEscV.Text = String.Format(Const.FORMAT, moon.escV / 1000.0);

            //Bulk
            propsBulkValueRock.Text = String.Format(Const.FORMAT, moon.bulkRock * 100.0);
            propsBulkValueMetal.Text = String.Format(Const.FORMAT, moon.bulkMetal * 100.0);
            propsBulkValueIce.Text = String.Format(Const.FORMAT, moon.bulkIces * 100.0);
            propsBulkLabelWater.Text = propsBulkValueWater.Text = propsBulkUnitWater.Text = "";
            propsBulkLabelHydrogen.Text = propsBulkValueHydrogen.Text = propsBulkUnitHydrogen.Text = "";
            propsBulkValueDensity.Text = String.Format(Const.FORMAT, moon.bulkDensity);


            //Orbit
            if (moon.type != ID.Sat.MOONC)
            {
                propsOrbitBeltGroup.Hide();
                propsOrbitValueD.Text = String.Format(Const.FORMAT, moon.day);
                if (moon.orbit.y >= 1000.0)
                {
                    propsOrbitValueY.Text = String.Format(Const.FORMAT, moon.orbit.y / Const.Earth.YEAR);
                    propsOrbitUnitY.Text = "y🜨";
                }
                else
                {
                    propsOrbitValueY.Text = String.Format(Const.FORMAT, moon.orbit.y);
                    propsOrbitUnitY.Text = "d🜨";
                }
                propsOrbitValueV.Text = String.Format(Const.FORMAT, moon.orbit.v / 1000.0);
                propsOrbitValueT.Text = String.Format(Const.FORMAT, moon.tilt);
                if (moon.orbit.a > 9999.0)
                {
                    propsOrbitValueA.Text = String.Format(Const.FORMAT, moon.orbit.a * Const.AU_PER_EARTHRADIUS);
                    propsOrbitUnitA.Text = "AU";
                }
                else
                {
                    propsOrbitValueA.Text = String.Format(Const.FORMAT, moon.orbit.a);
                    propsOrbitUnitA.Text = "R🜨";
                }
                propsOrbitValueE.Text = String.Format(Const.FORMAT, moon.orbit.e * 100.0);
                propsOrbitValueI.Text = String.Format(Const.FORMAT, moon.orbit.i);
                propsOrbitValueL.Text = String.Format(Const.FORMAT, moon.orbit.l);
                propsOrbitValueP.Text = String.Format(Const.FORMAT, moon.orbit.p);
            }
            else
            {
                propsOrbitPlanetGroup.Hide();
                propsOrbitBeltValueD.Text = String.Format(Const.FORMAT, moon.day);
                if (moon.orbit.y >= 1000.0)
                {
                    propsOrbitBeltValueY.Text = String.Format(Const.FORMAT, moon.orbit.y / Const.Earth.YEAR);
                    propsOrbitBeltUnitY.Text = "y🜨";
                }
                else
                {
                    propsOrbitBeltValueY.Text = String.Format(Const.FORMAT, moon.orbit.y);
                    propsOrbitBeltUnitY.Text = "d🜨";
                }
                propsOrbitBeltValueV.Text = String.Format(Const.FORMAT, moon.orbit.v / 1000.0);
                if (moon.orbit.a > 9999.0)
                {
                    propsOrbitBeltMuA.Text = String.Format(Const.FORMAT, moon.orbit.a * Const.AU_PER_EARTHRADIUS);
                    propsOrbitBeltUnitA.Text = "AU";
                }
                else
                {
                    propsOrbitBeltMuA.Text = String.Format(Const.FORMAT, moon.orbit.a);
                    propsOrbitBeltUnitA.Text = "R🜨";
                }
                propsOrbitBeltMuE.Text = String.Format(Const.FORMAT, moon.orbit.e * 100.0);
                propsOrbitBeltMuI.Text = String.Format(Const.FORMAT, moon.orbit.i);
                propsOrbitBeltMuL.Text = String.Format(Const.FORMAT, moon.orbit.l);
                propsOrbitBeltMuP.Text = String.Format(Const.FORMAT, moon.orbit.p);

                propsOrbitBeltSigmaA.Text = String.Format(Const.FORMAT, moon.orbit.aSigma);
                propsOrbitBeltSigmaE.Text = String.Format(Const.FORMAT, moon.orbit.eSigma * 100.0);
                propsOrbitBeltSigmaI.Text = String.Format(Const.FORMAT, moon.orbit.iSigma);
                propsOrbitBeltSigmaL.Text = String.Format(Const.FORMAT, moon.orbit.lSigma);
                propsOrbitBeltSigmaP.Text = String.Format(Const.FORMAT, moon.orbit.pSigma);
            }

            //Flavor text
            flavorTextLabel.Text = "";
        }

        private void genButton_Click(object sender, EventArgs e)
        {
            //Clear log file
            using (StreamWriter output = new StreamWriter("C:\\Users\\green\\source\\repos\\SystemGenerator\\SystemGenerator\\log.txt"))
                output.Write("");

            Utils.writeLog("Beginning system generation");

            star = new Star();
            planets = star.genSystem();
            string option;

            Utils.writeLog(Environment.NewLine + "System generation complete");

            systemListBox.Items.Clear();

            systemListBox.Items.Add(Utils.getDescription(star.type));
            int sub = 0;

            for (int i = 0; i < planets.Count; i++)
            {
                if (planets[i].isBelt)
                {
                    option = Utils.getDescription(planets[i].type);
                    sub++;
                }
                else if (planets[i].type == ID.Belt.DWARF)
                {
                    option = String.Format("   1st: {0}", Utils.getDescription(planets[i].type));
                    sub++;
                }
                else
                    option = String.Format("{0,2}{1}: {2}", (i + 1 - sub), Utils.getOrdinal(i + 1 - sub), Utils.getDescription(planets[i].type));

                systemListBox.Items.Add(option);
            }

            systemListBox.SelectedIndex = 0;
        }
    }
}
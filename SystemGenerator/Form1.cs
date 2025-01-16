using SystemGenerator.Generation;

namespace SystemGenerator
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            propsLightGroup.Hide();
            propsOrbitStarGroup.Hide();
            this.Show();
            int height = Screen.PrimaryScreen.Bounds.Height;
            int width = Screen.PrimaryScreen.Bounds.Width;

            bool star = true;

            //Rescale widgets
            textGroupBox.Location = new Point(optionListBox.Location.X, optionListBox.Location.Y + optionListBox.Height);

            //Show and hide
            if (star)
            {
                propsBulkGroup.Hide();
                propsOrbitPlanetGroup.Hide();
                propsLightGroup.Show();
                propsOrbitStarGroup.Show();
            }
            else
            {
                propsLightGroup.Hide();
                propsOrbitStarGroup.Hide();
                propsBulkGroup.Show();
                propsOrbitPlanetGroup.Show();
            }

            //Clear log file
            /*
            using (StreamWriter output = new StreamWriter("C:\\Users\\green\\source\\repos\\SystemGenerator\\SystemGenerator\\log.txt"))
                output.Write("");

            Utils.writeLog("Beginning system generation");

            Star star = new Star();
            List<Planet> planets = star.genSystem();
            string option;

            Utils.writeLog(Environment.NewLine + "System generation complete");

            systemListBox.Items.Add(Utils.getDescription(star.type));

            for (int i = 0; i < planets.Count; i++)
            {
                option = (i + 1) + Utils.getOrdinal(i + 1) + ": " + Utils.getDescription(planets[i].type);
                systemListBox.Items.Add(option);
            }
            */
        }

        private void systemListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
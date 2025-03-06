using System;
using System.Drawing;
using System.Windows.Forms;

namespace MentalStates
{
    public partial class SettingsForm : Form
    {
        private CheckedListBox statesCheckedListBox;
        private NumericUpDown sphereSizeNumericUpDown;
        private ComboBox rippleColorComboBox;
        private NumericUpDown rippleSizeNumericUpDown;
        private ComboBox shockwaveColorComboBox;
        private NumericUpDown speedMultiplierNumericUpDown;
        private NumericUpDown backgroundIntensityNumericUpDown;
        private ComboBox sphereColorComboBox;
        private Button saveButton;

        public SettingsForm()
        {
            //
            this.Text = "How are you feeling today?";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            //table layout panel for the... panel
            var table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 2; //two columns of items
            table.RowCount = 10; //10 items we can edit
            table.Padding = new Padding(10); //pad the items
            table.AutoSize = true; //auto-size based on the size of the screen
            table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            this.Controls.Add(table);

            //select the mental states using a selection box; not dropdown

            var statesLabel = new Label() { Text = "Select Mental States:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.White };
            table.Controls.Add(statesLabel, 0, 0);
            table.SetColumnSpan(statesLabel, 2);
            statesCheckedListBox = new CheckedListBox() { Dock = DockStyle.Fill, Height = 80, BackColor = Color.FromArgb(50, 50, 50), ForeColor = Color.White };
            string[] mentalStates = { "Synesthesia", "Mania", "Panic", "Insomnia", "Depression", "Euphoria", "Dissociation" };
            statesCheckedListBox.Items.AddRange(mentalStates);
            //check all of them as default
            for (int i = 0; i < statesCheckedListBox.Items.Count; i++)
                statesCheckedListBox.SetItemChecked(i, true);
            table.Controls.Add(statesCheckedListBox, 0, 1);
            table.SetColumnSpan(statesCheckedListBox, 2);

            //sphere size selection
            table.Controls.Add(new Label() { Text = "Sphere Size:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.White }, 0, 2);
            sphereSizeNumericUpDown = new NumericUpDown() { Minimum = 1, Maximum = 100, Value = 10, Dock = DockStyle.Fill };
            table.Controls.Add(sphereSizeNumericUpDown, 1, 2);

            //select ripple color; either from the drop-down selection or a hex selection
            table.Controls.Add(new Label() { Text = "Ripple Color:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.White }, 0, 3);
            rippleColorComboBox = new ComboBox()
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            rippleColorComboBox.Items.AddRange(new string[] { "Red", "Green", "Blue", "White", "Magenta", "Custom..." });
            rippleColorComboBox.SelectedIndex = 0;
            table.Controls.Add(rippleColorComboBox, 1, 3);

            //the ripple size
            table.Controls.Add(new Label() { Text = "Ripple Size:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.White }, 0, 4);
            rippleSizeNumericUpDown = new NumericUpDown() { Minimum = 1, Maximum = 100, Value = 10, Dock = DockStyle.Fill };
            table.Controls.Add(rippleSizeNumericUpDown, 1, 4);

            //shockwave color using a drop-down box or hex selection
            table.Controls.Add(new Label() { Text = "Shockwave Color:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.White }, 0, 5);
            shockwaveColorComboBox = new ComboBox()
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            shockwaveColorComboBox.Items.AddRange(new string[] { "Red", "Yellow", "Cyan", "White", "Custom..." });
            shockwaveColorComboBox.SelectedIndex = 0;
            table.Controls.Add(shockwaveColorComboBox, 1, 5);

            //multiplier that has a floor and a ceiling
            table.Controls.Add(new Label() { Text = "Speed Multiplier", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.White }, 0, 6);
            speedMultiplierNumericUpDown = new NumericUpDown()
            {
                Minimum = 0.1M,
                Maximum = 5.0M,
                DecimalPlaces = 2,
                Increment = 0.1M,
                Value = 1.0M,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(speedMultiplierNumericUpDown, 1, 6);

            //static intensity
            table.Controls.Add(new Label() { Text = "Static Intensity (0-255):", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.White }, 0, 7);
            backgroundIntensityNumericUpDown = new NumericUpDown()
            {
                Minimum = 0,
                Maximum = 255,
                Value = 0,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(backgroundIntensityNumericUpDown, 1, 7);

            //sphere color using color section or hex selection
            table.Controls.Add(new Label() { Text = "Sphere Color:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.White }, 0, 8);
            sphereColorComboBox = new ComboBox()
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };
            sphereColorComboBox.Items.AddRange(new string[] { "Random", "Red", "Green", "Blue", "White", "Custom..." });
            sphereColorComboBox.SelectedIndex = 0;
            table.Controls.Add(sphereColorComboBox, 1, 8);

            //button that saves the input;
            saveButton = new Button() { Text = "Save", Dock = DockStyle.Fill, BackColor = Color.FromArgb(70, 70, 70), ForeColor = Color.White };
            saveButton.Click += SaveButton_Click;
            table.Controls.Add(saveButton, 0, 9);
            table.SetColumnSpan(saveButton, 2);

            //allow for custom selection; colorDialog selection as the Hex
            rippleColorComboBox.SelectedIndexChanged += (s, e) =>
            {
                if (rippleColorComboBox.SelectedItem.ToString() == "Custom...")
                {
                    using (ColorDialog dlg = new ColorDialog())
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            rippleColorComboBox.Items[rippleColorComboBox.SelectedIndex] = ColorTranslator.ToHtml(dlg.Color);
                        }
                    }
                }
            };

            shockwaveColorComboBox.SelectedIndexChanged += (s, e) =>
            {
                if (shockwaveColorComboBox.SelectedItem.ToString() == "Custom...")
                {
                    using (ColorDialog dlg = new ColorDialog())
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            shockwaveColorComboBox.Items[shockwaveColorComboBox.SelectedIndex] = ColorTranslator.ToHtml(dlg.Color);
                        }
                    }
                }
            };

            sphereColorComboBox.SelectedIndexChanged += (s, e) =>
            {
                if (sphereColorComboBox.SelectedItem.ToString() == "Custom...")
                {
                    using (ColorDialog dlg = new ColorDialog())
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            sphereColorComboBox.Items[sphereColorComboBox.SelectedIndex] = ColorTranslator.ToHtml(dlg.Color);
                        }
                    }
                }
            };
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            //build the mental states based on the user selection
            string selectedStates = string.Join(",", statesCheckedListBox.CheckedItems);
            int sphereSize = (int)sphereSizeNumericUpDown.Value;

            string rippleColor = rippleColorComboBox.SelectedItem.ToString();
            if (rippleColor.Equals("Custom...", StringComparison.OrdinalIgnoreCase))
            {
                //fallback just in case the user doesn't select a color
                rippleColor = "#FFFFFF";
            }

            int rippleSize = (int)rippleSizeNumericUpDown.Value;

            string shockwaveColor = shockwaveColorComboBox.SelectedItem.ToString();
            if (shockwaveColor.Equals("Custom...", StringComparison.OrdinalIgnoreCase))
            {
                shockwaveColor = "#FF0000";
            }

            string speedMultiplier = speedMultiplierNumericUpDown.Value.ToString();
            string backgroundIntensity = backgroundIntensityNumericUpDown.Value.ToString();

            string sphereColor = sphereColorComboBox.SelectedItem.ToString();
            if (sphereColor.Equals("Custom...", StringComparison.OrdinalIgnoreCase))
            {
                sphereColor = "#FFFFFF";
            }

            SettingsManager.SaveSetting("PsychologicalStates", selectedStates);
            SettingsManager.SaveSetting("SphereSize", sphereSize.ToString());
            SettingsManager.SaveSetting("RippleColor", rippleColor);
            SettingsManager.SaveSetting("RippleSize", rippleSize.ToString());
            SettingsManager.SaveSetting("ShockwaveColor", shockwaveColor);
            SettingsManager.SaveSetting("SpeedMultiplier", speedMultiplier);
            SettingsManager.SaveSetting("BackgroundIntensity", backgroundIntensity);
            SettingsManager.SaveSetting("SphereColor", sphereColor);

            MessageBox.Show("Settings saved!");
            Close();
        }

    }
}

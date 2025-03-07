using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using System.IO;
using System.Collections.Generic;

namespace MentalStates

{
    public static class RandomExtensions
    {
        /*
            NextFloat

            Generates a random float.
        */
        public static float NextFloat(this Random random, float minValue, float maxValue)
        {
            return (float)(random.NextDouble() * (maxValue - minValue) + minValue);
        }
    }

    public partial class ScreensaverForm : Form
    {
        //various variables
        private System.Windows.Forms.Timer animationTimer;
        private DateTime startTime;

        private List<Sphere> spheres = new();
        private List<Ripple> ripples = new();
        private List<Shockwave> shockwaves = new();

        private Random random = new();
        private float cloudOffset = 0.0f;
        private float paranoiaIntensity = 0.0f;
        private int redAnger = 0;
        private float distortionStrength = 5.0f;

        float globalSpeedMultiplier = GetFloatSetting("SpeedMultiplier", "1.0");

        private PointF lastMousePosition;
        private List<PointF> echoTrail = new();

        /*
         * ScreensaverForm
         * 
         * This initalizes the screensaver! 
         * 
        */
        public ScreensaverForm()
        {
            AppSettings.LoadSettings();

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;

            startTime = DateTime.Now;

            string sphereColorSetting = SettingsManager.GetSetting("SphereColor", "Random");

            for (int i = 0; i < 15; i++)
            {
                Color colorToUse = sphereColorSetting.Equals("Random", StringComparison.OrdinalIgnoreCase)
                    ? ColorFromHSV(random.Next(0, 360), 1.0, 1.0)
                    : ColorTranslator.FromHtml(sphereColorSetting);

                spheres.Add(new Sphere(
                    random.Next(-300, 300),
                    random.Next(-200, 200),
                    AppSettings.SphereSize,
                    random.NextFloat(-6f, 6f),
                    random.NextFloat(-6f, 6f),
                    random.NextFloat(-2f, 2f),
                    colorToUse,
                    AppSettings.PsychologicalStates[random.Next(AppSettings.PsychologicalStates.Length)]
                ));
            }

            animationTimer = new System.Windows.Forms.Timer { Interval = 1 };

            animationTimer.Tick += (s, e) =>
            {
                PointF currentMousePosition = this.PointToClient(Cursor.Position);
                paranoiaIntensity = (float)(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 300.0) * 20);

                if (echoTrail.Count == 0 || Distance(echoTrail[^1], currentMousePosition) > 5)
                {
                    echoTrail.Add(currentMousePosition);
                }

                if (echoTrail.Count > 20)
                {
                    echoTrail.RemoveAt(0);
                }

                cloudOffset = (cloudOffset + 0.05f) % ClientSize.Width;


                foreach (var sphere in spheres)
                {
                    // Optionally combine the state multiplier and global speed multiplier:
                    sphere.Update(ClientSize, ripples, globalSpeedMultiplier);
                }

                ripples.RemoveAll(r => r.Opacity <= 0);
                shockwaves.RemoveAll(r => r.Opacity <= 0);

                lastMousePosition = currentMousePosition;

                if (redAnger > 0) redAnger -= 5;

                this.Invalidate();
            };
            animationTimer.Start();

            this.Paint += CombinedPaint;
            this.KeyDown += HandleExit;
            this.MouseClick += ReleaseShockwave;
        }

        /*
        * GetFloatSetting
        *
        * Changes the number according to the cultural settings of the laptop/PC.
        */
        public static float GetFloatSetting(string key, string defaultValue = "0")
        {
            string value = SettingsManager.GetSetting(key, defaultValue);
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                result = float.Parse(defaultValue, CultureInfo.InvariantCulture);
            }
            return result;
        }

        /*
         * CombinedPaint
         * 
         * Combines all the effects. Remember, they are in order, make sure the ones on the top are painted last!!
        */
        private void CombinedPaint(object sender, PaintEventArgs e)
        {
            DrawDistortion(e);
            DrawGlitchEffect(e);
            DrawSpheres(sender, e);
            DrawEchoes(e);
            DrawParanoiaEffect(e);
        }

        /*
         * DrawGlitchEffect
         * 
         * A subtle effect. You can remove it but it might take away from the experience slightly. Just know where to remove it!
        */
        private void DrawGlitchEffect(PaintEventArgs e)
        {
            if (random.NextDouble() < 0.01)
            {
                int glitchX = random.Next(1, 10);
                int glitchY = random.Next(0, this.ClientSize.Height - glitchX);

                using (Brush glitchBrush = new SolidBrush(Color.FromArgb(30, random.Next(256), random.Next(256), random.Next(256))))
                {
                    e.Graphics.FillRectangle(glitchBrush, 0, glitchY, this.ClientSize.Width, glitchX);
                }
            }
        }

        /*
         * DrawParanoiaEffect
         * 
         *  The small red circle around your mouse that follows you...
        */
        private void DrawParanoiaEffect(PaintEventArgs e)
        {
            float jitterX = (float)(random.NextDouble() * paranoiaIntensity - paranoiaIntensity / 2);
            float jitterY = (float)(random.NextDouble() * paranoiaIntensity - paranoiaIntensity / 2);

            float targetX = lastMousePosition.X + jitterX;
            float targetY = lastMousePosition.Y + jitterY;

            using (Pen redPen = new Pen(Color.FromArgb(50, 255, 0, 0), 2))
            {
                e.Graphics.DrawEllipse(redPen, targetX - 10, targetY - 10, 20, 20);
            }
        }

        /*
         * DrawDistortion
         * 
         * Draws the distortion effect you see; the lines that make it look like a crt screen almost.
        */
        private void DrawDistortion(PaintEventArgs e)
        {
            int waveStrength = (int)(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 100.0) * distortionStrength);

            for (int y = 0; y < this.ClientSize.Height; y += 5)
            {
                int offset = (int)(Math.Sin(y / 10.0 + DateTime.Now.TimeOfDay.TotalMilliseconds / 500.0) * waveStrength);
                using (Pen distortionPen = new Pen(Color.FromArgb(5, Color.DarkGray), 1))
                {
                    e.Graphics.DrawLine(distortionPen, 0, y + offset, this.ClientSize.Width, y + offset);
                }
            }
        }

        /*
         * DrawEchoes
         * 
         * Draws those echoes you see near your cursor.
        */
        private void DrawEchoes(PaintEventArgs e)
        {
            for (int i = 0; i < echoTrail.Count; i++)
            {
                float alpha = (int)(255 * ((float)i / echoTrail.Count));
                float decay = (float)(1.0 - (float)i / echoTrail.Count);
                using (Pen echoPen = new Pen(Color.FromArgb((int)(alpha * decay), ColorFromHSV(i * 15 % 360, 1, 1)), 1))
                {
                    float offset = (echoTrail.Count - i) * 0.3f;
                    e.Graphics.DrawEllipse(echoPen, echoTrail[i].X - offset, echoTrail[i].Y - offset, offset * 2, offset * 2);
                }
            }
        }

        /*
         * DrawSpheres
         * 
         * Draws the spheres and other related objects.
        */
        private void DrawSpheres(object sender, PaintEventArgs e)
        {
            string shockwaveColorSetting = SettingsManager.GetSetting("ShockwaveColor", "#FF0000");
            Color shockwaveColor = ColorTranslator.FromHtml(shockwaveColorSetting);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;

            DrawClouds(g);

            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;

            foreach (var ripple in ripples)
            {
                using (Pen pen = new Pen(Color.FromArgb(ripple.Opacity, ripple.Color), 2))
                {
                    int size = (int)(ripple.Radius * 2);
                    g.DrawEllipse(pen, ripple.X - size / 2, ripple.Y - size / 2, size, size);
                }
                ripple.Update();
            }

            foreach (var shockwave in shockwaves)
            {
                using (Pen pen = new Pen(Color.FromArgb(shockwave.Opacity, shockwaveColor), 4))
                {
                    int size = (int)(shockwave.Radius * 2);
                    g.DrawEllipse(pen, shockwave.X - size / 2, shockwave.Y - size / 2, size, size);
                }
                shockwave.Update();
            }

            foreach (var sphere in spheres)
            {
                float depth = 1.0f / (1.0f + sphere.Z / 100.0f);
                depth = Math.Clamp(depth, 0.01f, 1.0f);

                int size = (int)(sphere.Radius * depth * 5);
                size = Math.Clamp(size, 1, 200);

                int x = (int)(centerX + sphere.X * depth);
                int y = (int)(centerY + sphere.Y * depth);

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Rectangle(x - size / 2, y - size / 2, size, size),
                    sphere.Color,
                    ControlPaint.Dark(sphere.Color),
                    LinearGradientMode.ForwardDiagonal))
                {
                    g.FillEllipse(brush, x - size / 2, y - size / 2, size, size);
                }
            }

            if (redAnger > 0)
            {
                using (SolidBrush redTint = new SolidBrush(Color.FromArgb(redAnger, shockwaveColor)))
                {
                    g.FillRectangle(redTint, this.ClientRectangle);
                }
            }
        }

        /*
         * DrawClouds
         * 
         * Draws the static in the background; bigger the value put in, the larger the "clouds"
         * It looks more cloudy with a larger value and more like static with a smaller one
         *
        */
        private void DrawClouds(Graphics g)
        {
            float backgroundSize = GetFloatSetting("BackgroundIntensity", "0"); //initial user intensity
            int staticDensity = (int)(200 * (1 + backgroundSize / 10)); //scale the density based on the intensity
            int maxStaticSize = Math.Max(2, (int)(1 + backgroundSize / 5));

            for (int i = 0; i < staticDensity; i++)
            {
                int x = random.Next(ClientSize.Width);
                int y = random.Next(ClientSize.Height);

                int alpha = Math.Clamp(random.Next(10, (int)(backgroundSize * 25)), 10, 255); //ensure it stays within the proper range for the transparency
                int staticSize = random.Next(1, Math.Max(2, maxStaticSize)); //control static size based on the user input

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 200, 200, 200)))
                {
                    g.FillRectangle(brush, x, y, staticSize, staticSize); //draw larger or smaller static/cloud blobs
                }
            }
        }

        /*
         * ColorFromHSV
         * 
         * Random color generator. Feel free to edit this.
         *
        */
        private static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => Color.FromArgb(v, t, p),
                1 => Color.FromArgb(q, v, p),
                2 => Color.FromArgb(p, v, t),
                3 => Color.FromArgb(p, q, v),
                4 => Color.FromArgb(t, p, v),
                _ => Color.FromArgb(v, p, q),
            };
        }

        /*
         * HandleExit
         * 
         * Exits the screensaver
        */
        private void HandleExit(object sender, KeyEventArgs e)
        {
            if ((DateTime.Now - startTime).TotalSeconds > 3)
            {
                FadeOutAndClose();
            }
        }

        /*
         * FadeOutAndClose
         * 
         * Fades the screensaver slowly before it closes.
        */
        private async void FadeOutAndClose()
        {
            for (int opacity = 100; opacity >= 0; opacity -= 5)
            {
                this.Opacity = opacity / 100.0;
                await System.Threading.Tasks.Task.Delay(50);
            }
            this.Close();
        }

        /*
         * ReleaseShockwave
         * 
         * Releases the shockwave when the user clicks.
        */
        private void ReleaseShockwave(object sender, MouseEventArgs e)
        {
            shockwaves.Add(new Shockwave(e.X, e.Y));
            redAnger = 100;
        }

        /*
         * Distance
         * 
         * Generates the distance between two objects when necessary; mainly for the user pointer.
        */
        private float Distance(PointF p1, PointF p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public class Sphere
    {
        public float X, Y, Z;
        public float Radius;
        public float VelocityX, VelocityY, VelocityZ;
        public Color Color;
        public string PsychologicalState;

        /*
         * Sphere
         * 
         * Generates a sphere based on the position, size, color, and the made-up arbitary states.
         * 
        */
        public Sphere(float x, float y, float radius, float vx, float vy, float vz, Color color, string state)
        {
            X = x;
            Y = y;
            Z = 0;
            Radius = radius;
            VelocityX += vx;
            VelocityY += vy;
            VelocityZ += vz;
            Color = color;
            PsychologicalState = state;
        }

        /*
         * GetSpeedMultiplier 
         * 
         * Generate the speed based on the state; some states are slower than others.
         */
        private float GetSpeedMultiplier(string state)
        {
            return state switch
            {
                "Mania" => 1.5f,
                "Panic" => 1.8f,
                "Insomnia" => 1.3f,
                "Depression" => 0.8f,
                "Euphoria" => 1.4f,
                "Dissociation" => 1.1f,
                _ => 1.0f, 
            };
        }

        /*
         * Update
         * 
         * Update the sphere and the ripples that go along w/ it.
        */
        public void Update(Size clientSize, List<Ripple> ripples, float globalMultiplier)
        {
            float stateMultiplier = GetSpeedMultiplier(PsychologicalState);
            float multiplier = stateMultiplier * globalMultiplier;

            X += VelocityX * multiplier;
            Y += VelocityY * multiplier;
            Z += VelocityZ * multiplier;

            //gives it that wall-bouncing effect
            if (X - Radius < -clientSize.Width / 2 || X + Radius > clientSize.Width / 2)
            {
                VelocityX = -VelocityX;
                ripples.Add(new Ripple(X, Y, ColorTranslator.FromHtml(AppSettings.RippleColorHex)));
            }
            if (Y - Radius < -clientSize.Height / 2 || Y + Radius > clientSize.Height / 2)
            {
                VelocityY = -VelocityY;
                ripples.Add(new Ripple(X, Y, ColorTranslator.FromHtml(AppSettings.RippleColorHex)));
            }
            if (Z < -100 || Z > 100)
            {
                VelocityZ = -VelocityZ;
            }
        }
    }


    public class Ripple
    {
        public float X, Y;
        public float Radius = 0;
        public int Opacity = 255;
        public Color Color;

        public int rippleSize = GetIntSetting("RippleSize", "5");

        /*
        * GetIntSettings 
        *
        * Changes integer values to match the cultural settings of the PC/laptop.
        */
        public static int GetIntSetting(string key, string defaultValue = "2")
        {
            string value = SettingsManager.GetSetting(key, defaultValue);
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                result = int.Parse(defaultValue, CultureInfo.InvariantCulture);
            }
            return result;
        }
        /*
         * Ripple
         * 
         * Generates a ripple based on the position and color
        */
        public Ripple(float x, float y, Color color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        /*
         * Update
         * 
         * Update the ripple radius and opacity
        */
        public void Update()
        {
            Radius += AppSettings.RippleSize;
            Opacity -= 5;
        }
    }

    public class Shockwave
    {
        public int X { get; }
        public int Y { get; }
        public float Radius { get; private set; }
        public int Opacity { get; private set; } = 255;
        public float GrowthRate { get; } = 20f;
        public float MaxRadius { get; }

        /*
         * ShockWave
         * 
         * Generates the shockwave's size and radius
        */
        public Shockwave(int x, int y)
        {
            X = x;
            Y = y;
            MaxRadius = 2000;
        }

        /*
         * Update
         * 
         * Update the size and opacity of the shockwave based on growth rate.
        */
        public void Update()
        {
            Radius += GrowthRate;
            Opacity = (int)(255 * (1 - Radius / MaxRadius));
            if (Opacity < 0) Opacity = 0;
        }
    }

    public class Echo
    {
        public PointF Position { get; }
        public Color Color { get; }
        private int alpha = 255;

        /*
         * Echo
         * 
         * Generates the echoes that follow your mouse.
        */
        public Echo(PointF position, Color color)
        {
            Position = position;
            Color = color;
        }

        /*
         * Update
         * 
         * Updates their transparency.
        */
        public void Update()
        {
            alpha -= 5;
        }

        /*
         * Draw
         * 
         * Generate them as they follow your mouse.
        */
        public void Draw(Graphics g)
        {
            if (alpha > 0)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(alpha, Color)))
                {
                    g.FillEllipse(brush, Position.X - 10, Position.Y - 10, 20, 20);
                }
            }
        }

        
        public bool IsFaded => alpha <= 0; //check if the current echo has faded; if the transparency is equal to 0
    }


}

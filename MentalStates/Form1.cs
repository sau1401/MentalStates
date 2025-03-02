using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MentalStates

{
    public static class RandomExtensions
    {
        public static float NextFloat(this Random random, float minValue, float maxValue)
        {
            return (float)(random.NextDouble() * (maxValue - minValue) + minValue);
        }
    }

    public partial class ScreensaverForm : Form
    {
        private System.Windows.Forms.Timer animationTimer;
        private DateTime startTime;
        private List<Sphere> spheres = new();
        private List<Ripple> ripples = new();
        private Random random = new();
        private float cloudOffset = 0;
        // private int lightningCooldown = 0;
        // private List<Lightning> lightnings = new();

        public ScreensaverForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;

            startTime = DateTime.Now;

            for (int i = 0; i < 20; i++)
            {
                spheres.Add(new Sphere(
                    random.Next(-300, 300),
                    random.Next(-200, 200),
                    random.Next(5, 15),
                    random.NextFloat(-6f, 6f),
                    random.NextFloat(-6f, 6f),
                    random.NextFloat(-2f, 2f),
                    ColorFromHSV(random.Next(0, 360), 1.0, 1.0),
                    GetPsychologicalState()
                ));
            }

            animationTimer = new System.Windows.Forms.Timer { Interval = 30 };
            animationTimer.Tick += (s, e) =>
            {
                cloudOffset = (cloudOffset + 0.05f) % ClientSize.Width;
                foreach (var sphere in spheres)
                {
                    sphere.Update(ClientSize, ripples);
                }
                ripples.RemoveAll(r => r.Opacity <= 0);
                this.Invalidate();
            };
            animationTimer.Start();

            this.Paint += DrawSpheres;
            this.KeyDown += HandleExit;
            this.MouseMove += HandleExit;
        }

        private void DrawSpheres(object sender, PaintEventArgs e)
        {
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
        }

        private void DrawClouds(Graphics g)
        {
            int cloudDensity = 200;
            for (int i = 0; i < cloudDensity; i++)
            {
                int x = random.Next(ClientSize.Width);
                int y = random.Next(ClientSize.Height);
                int alpha = random.Next(10, 50);

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 200, 200, 200)))
                {
                    g.FillRectangle(brush, x, y, 2, 2);
                }
            }
        }

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

        private string GetPsychologicalState()
        {
            string[] states = { "Synesthesia", "Mania", "Panic", "Insomnia", "Depression", "Euphoria", "Dissociation" };
            return states[random.Next(states.Length)];
        }

        private void HandleExit(object sender, EventArgs e)
        {
            if ((DateTime.Now - startTime).TotalSeconds > 3)
            {
                this.Close();
            }
        }
    }

    public class Sphere
    {
        public float X, Y, Z;
        public float Radius;
        public float VelocityX, VelocityY, VelocityZ;
        public Color Color;
        public string PsychologicalState;

        public Sphere(float x, float y, float radius, float vx, float vy, float vz, Color color, string state)
        {
            X = x;
            Y = y;
            Z = 0;
            Radius = radius;
            VelocityX = vx;
            VelocityY = vy;
            VelocityZ = vz;
            Color = color;
            PsychologicalState = state;
        }

        public void Update(Size clientSize, List<Ripple> ripples)
        {
            X += VelocityX;
            Y += VelocityY;
            Z += VelocityZ;

            if (X - Radius < -clientSize.Width / 2 || X + Radius > clientSize.Width / 2)
            {
                VelocityX = -VelocityX;
                ripples.Add(new Ripple(X, Y, Color));
            }
            if (Y - Radius < -clientSize.Height / 2 || Y + Radius > clientSize.Height / 2)
            {
                VelocityY = -VelocityY;
                ripples.Add(new Ripple(X, Y, Color));
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

        public Ripple(float x, float y, Color color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        public void Update()
        {
            Radius += 2;
            Opacity -= 5;
        }
    }
}

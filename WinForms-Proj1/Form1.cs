using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace WinForms_Proj1
{
    public partial class Form1 : Form
    {
        bool isDrawing = false;
        List<(List<Point>, Color, int)> strokes;
        List<Point> pixels;
        List<Color> colors;
        Color currentColor = Color.Black;
        Mode mode = Mode.None;
        Point shapeOrigin = new Point(-1, -1);
        List<(Point, Point, Color, int)> rectangles;
        List<(Point, Point, Color, int)> ellipses;
        (Point, Point) currentShape;
        int currentThickness = 2;

        public Form1()
        {
            InitializeComponent();
            strokes = new List<(List<Point>, Color, int)>();
            pixels = new List<Point>();
            colors = new List<Color>();
            rectangles = new List<(Point, Point, Color, int)>();
            ellipses = new List<(Point, Point, Color, int)>();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            toolStripComboBox1.SelectedItem = toolStripComboBox1.Items[1];
            toolStripButton7.Image = null;
            toolStripButton7.BackColor = currentColor;
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = (int)(0.9 * Width);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // start drawing
            if (e.Button == MouseButtons.Left && mode == Mode.Brush)
            {
                isDrawing = true;
                pixels = new List<Point>();
            }
            else if (e.Button == MouseButtons.Left && mode == Mode.Rectangle)
            {
                shapeOrigin = new Point(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Left && mode == Mode.Elipse)
            {
                shapeOrigin = new Point(e.X, e.Y);
            }
            if (e.Button == MouseButtons.Right)
            {
                strokes.Add((pixels, currentColor, currentThickness));
                isDrawing = false;
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // add shapes to lists
            if (mode == Mode.Brush)
            {
                strokes.Add((pixels, currentColor, currentThickness));
                isDrawing = false;
            }
            else if (mode == Mode.Rectangle)
            {
                rectangles.Add((shapeOrigin, new Point(e.X, e.Y), currentColor, currentThickness));
                shapeOrigin = new Point(-1, -1);
            }
            else if (mode == Mode.Elipse)
            {
                ellipses.Add((shapeOrigin, new Point(e.X, e.Y), currentColor, currentThickness));
                shapeOrigin = new Point(-1, -1);
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // get info for preview of strokes or shapes
            if (mode == Mode.Brush && isDrawing)
            {
                Point p = new Point(e.X, e.Y);
                pixels.Add(p);
                pictureBox1.Invalidate();
            }
            else if (mode == Mode.Rectangle || mode == Mode.Elipse)
            {
                currentShape = (shapeOrigin, new Point(e.X, e.Y));
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // handling brushes
            foreach (var s in strokes)
            {
                for (int i = 0; i < s.Item1.Count - 1; i++)
                {
                    e.Graphics.DrawLine(new Pen(new SolidBrush(s.Item2), s.Item3), s.Item1[i], s.Item1[i + 1]);
                }
            }
            if (pixels.Count != 0)
            {
                for (int i = 0; i < pixels.Count - 1; i++)
                {
                    e.Graphics.DrawLine(new Pen(new SolidBrush(currentColor), currentThickness), pixels[i], pixels[i + 1]);
                }
            }
            // handling rectangles
            foreach (var r in rectangles)
            {
                Rectangle rr = new Rectangle(
                    Math.Min(r.Item1.X, r.Item2.X),
                    Math.Min(r.Item1.Y, r.Item2.Y),
                    Math.Abs(r.Item2.X - r.Item1.X),
                    Math.Abs(r.Item2.Y - r.Item1.Y));
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(r.Item3), r.Item4), rr);
            }
            if (mode == Mode.Rectangle && shapeOrigin.X != -1)
            {
                var r = currentShape;
                Rectangle rr = new Rectangle(
                   Math.Min(r.Item1.X, r.Item2.X),
                   Math.Min(r.Item1.Y, r.Item2.Y),
                   Math.Abs(r.Item2.X - r.Item1.X),
                   Math.Abs(r.Item2.Y - r.Item1.Y));
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(currentColor), currentThickness), rr);
            }
            // handling elipses
            foreach (var r in ellipses)
            {
                Rectangle rr = new Rectangle(
                    Math.Min(r.Item1.X, r.Item2.X),
                    Math.Min(r.Item1.Y, r.Item2.Y),
                    Math.Abs(r.Item2.X - r.Item1.X),
                    Math.Abs(r.Item2.Y - r.Item1.Y));
                e.Graphics.DrawEllipse(new Pen(new SolidBrush(r.Item3), r.Item4), rr);
            }
            if (mode == Mode.Elipse && shapeOrigin.X != -1)
            {
                var r = currentShape;
                Rectangle rr = new Rectangle(
                   Math.Min(r.Item1.X, r.Item2.X),
                   Math.Min(r.Item1.Y, r.Item2.Y),
                   Math.Abs(r.Item2.X - r.Item1.X),
                   Math.Abs(r.Item2.Y - r.Item1.Y));
                e.Graphics.DrawEllipse(new Pen(new SolidBrush(currentColor), currentThickness), rr);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // load known colors
            KnownColor[] known = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            foreach (KnownColor knowColor in known)
            {
                colors.Add(Color.FromKnownColor(knowColor));
            }
            flowLayoutPanel1.Invalidate();
            toolStripButton7.Invalidate();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            // drawin color squares
            int size = 25;
            int columns = flowLayoutPanel1.Width / size;
            for (int i = 0; i < colors.Count; i++)
            {
                e.Graphics.FillRectangle(new SolidBrush(colors[i]), (i % columns) * size, (i / columns) * size, size-1, size-1);
                if (colors[i] == currentColor)
                {
                    Pen pen = new Pen(Color.FromArgb(255 - currentColor.R, 255 - currentColor.G, 255 - currentColor.B), 2);
                    pen.DashPattern = new float[] { 2.0f, 2.0f };
                    e.Graphics.DrawRectangle(pen, (i % columns) * size+1, (i / columns) * size+1, size-2, size-2);
                }
            }
        }

        private void flowLayoutPanel1_Click(object sender, EventArgs e)
        {
            // detecting what color I clicked
            int size = 25;
            Point p = flowLayoutPanel1.PointToClient(MousePosition);
            int i = (p.Y / size) * (flowLayoutPanel1.Width / size);
            i += (p.X / size);
            currentColor = colors[i];
            toolStripButton7.BackColor = currentColor;
            flowLayoutPanel1.Invalidate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // brush button click
            mode = Mode.Brush;
            toolStripButton4.Checked = false;
            toolStripButton5.Checked = false;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            // rectangle button click
            mode = Mode.Rectangle;
            toolStripButton1.Checked = false;
            toolStripButton5.Checked = false;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            // ellipse button click
            mode = Mode.Elipse;
            toolStripButton1.Checked = false;
            toolStripButton4.Checked = false;
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedIndex == 0)
            {
                currentThickness = 1;
            }
            else if (toolStripComboBox1.SelectedIndex == 1)
            {
                currentThickness = 2;
            }
            else if (toolStripComboBox1.SelectedIndex == 2)
            {
                currentThickness = 3;
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            // reset everything
            strokes = new List<(List<Point>, Color, int)>();
            pixels = new List<Point>();
            colors = new List<Color>();
            rectangles = new List<(Point, Point, Color, int)>();
            ellipses = new List<(Point, Point, Color, int)>();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.InitialImage = null;
            pictureBox1.Invalidate();
        }

        private void toolStripButton7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl-PL");
            this.Controls.Clear();
            this.InitializeComponent();
            flowLayoutPanel1.Invalidate();
            toolStripComboBox1.SelectedItem = toolStripComboBox1.Items[1];
            currentThickness = 2;
        }

        enum Mode
        {
            Brush,
            Rectangle,
            Elipse,
            None
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            this.Controls.Clear();
            this.InitializeComponent();
            flowLayoutPanel1.Invalidate();
            toolStripComboBox1.SelectedItem = toolStripComboBox1.Items[1];
            currentThickness = 2;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = @"PNG|*.png|JPEG|*.jpg|BMP|*.bmp" })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    pictureBox1.DrawToBitmap(bmp, pictureBox1.ClientRectangle);
                    bmp.Save(saveFileDialog.FileName);
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog loadFileDialog = new OpenFileDialog() { Filter = @"PNG|*.png|JPEG|*.jpg|BMP|*.bmp" })
            {
                if (loadFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(loadFileDialog.FileName);
                    pictureBox1.Image = bmp;
                    pictureBox1.Width = bmp.Width;
                    pictureBox1.Height = bmp.Height;
                    pictureBox1.ClientSize = new System.Drawing.Size(bmp.Width, bmp.Height);
                    Width = (int)(bmp.Width * 10f / 9f);
                    Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);
                    int titleHeight = screenRectangle.Top - Top;
                    Height = bmp.Height + toolStrip1.Height + titleHeight;
                }
            }
        }
    }
}
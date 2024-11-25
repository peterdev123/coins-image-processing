using ImageProcess2;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace Coins_Activity
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed;
        public Form1()
        {
            InitializeComponent();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();   
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            processed = (Bitmap)loaded.Clone();

            bool success;

            // Apply Gaussian Blur
            success = BitmapFilter.GaussianBlur(processed, 4);

            // Apply Thresholding
            processed = BitmapFilter.ApplyThreshold(processed, 170);

            // Edge Detection
            success = BitmapFilter.EdgeDetectHomogenity(processed, 60);

            pictureBox2.Image = processed;

            // Count white circles using connected component labeling
            int circleCount = CountWhiteCircles(processed);

            // Display the result
            MessageBox.Show($"Number of white circles: {circleCount}");
        }

        public static int CountWhiteCircles(Bitmap binaryImage)
        {
            BitmapData data = binaryImage.LockBits(
                new Rectangle(0, 0, binaryImage.Width, binaryImage.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb
            );

            bool[,] visited = new bool[binaryImage.Width, binaryImage.Height];
            int circleCount = 0;

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;

                for (int y = 0; y < binaryImage.Height; y++)
                {
                    for (int x = 0; x < binaryImage.Width; x++)
                    {
                        int index = y * data.Stride + x * 3;

                        if (ptr[index] == 255 && !visited[x, y])
                        {
                            // Found a new white component
                            circleCount++;
                            FloodFill(ptr, visited, x, y, binaryImage.Width, binaryImage.Height, data.Stride);
                        }
                    }
                }
            }

            binaryImage.UnlockBits(data);
            return circleCount;
        }

        private unsafe static void FloodFill(byte* ptr, bool[,] visited, int x, int y, int width, int height, int stride)
        {
            Stack<Point> stack = new Stack<Point>();
            stack.Push(new Point(x, y));

            while (stack.Count > 0)
            {
                Point p = stack.Pop();
                int index = p.Y * stride + p.X * 3;

                if (p.X < 0 || p.Y < 0 || p.X >= width || p.Y >= height) continue;
                if (visited[p.X, p.Y]) continue;
                if (ptr[index] != 255) continue;

                visited[p.X, p.Y] = true;

                stack.Push(new Point(p.X + 1, p.Y));
                stack.Push(new Point(p.X - 1, p.Y));
                stack.Push(new Point(p.X, p.Y + 1));
                stack.Push(new Point(p.X, p.Y - 1));
            }
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
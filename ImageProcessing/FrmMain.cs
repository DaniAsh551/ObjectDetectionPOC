/// Copyright 2019 Dani / Shum (d4n1.551 / Dani551)
/// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
/// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
/// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
/// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
/// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
/// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
/// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Title = "Open Image",
                //Filter = "Images files (*.png|*.jpg|*.bmp)",
            };

            dialog.ShowDialog();

            txtFile.Text = dialog.FileName;
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            var text = txtFile.Text;
            if (string.IsNullOrWhiteSpace(text) || !File.Exists(text))
            {
                return;
            }

            lblDetails.Text = "Loading Image...";

            var original = new Bitmap(Image.FromFile(text));
            var imageObjDetection = new Bitmap(Image.FromFile(text));
            var imageEdgeDetection = new Bitmap(Image.FromFile(text));
            var perfomanceCounter = new long[2];
            var noOfDetections = 0;

            var edgeDetectionThread = new Thread(() =>
            {
                var stopWatch = new Stopwatch();

                pbOriginal.Image = original;

                lblDetails.Text = "Detecting Edges...";
                stopWatch.Start();

                var edges = imageEdgeDetection.DetectEdges();
                stopWatch.Stop();
                perfomanceCounter[0] = stopWatch.ElapsedMilliseconds;
                pbEdges.Image = edges;
                lblDetails.Text = "Edge Detection Complete...";
            });

            var objectDetectionThread = new Thread(() =>
            {
                var stopWatch = new Stopwatch();
                lblDetails.Text = "Detecting objects...";
                stopWatch.Restart();
                var detections = imageObjDetection.DetectObjects();
                stopWatch.Stop();
                perfomanceCounter[1] = stopWatch.ElapsedMilliseconds;
                noOfDetections = detections.Length;

                using (var graphics = Graphics.FromImage(imageObjDetection))
                using (var pen = new Pen(Color.Aqua))
                {
                    foreach (var detection in detections)
                    {
                        detection.Draw(graphics, pen);
                    }
                }

                pbDetection.Image = imageObjDetection;
            });

            var finalStatusSetThread = new Thread(() => 
            {
                while (objectDetectionThread.IsAlive || edgeDetectionThread.IsAlive)
                    Thread.Sleep(10);
                
                lblDetails.Text = $"{noOfDetections} detections in {perfomanceCounter[1]}ms  |   Edge Detection Time: {perfomanceCounter[0]}ms   | Total: {perfomanceCounter.Sum()}ms";
            });

            edgeDetectionThread.Start();
            objectDetectionThread.Start();
            finalStatusSetThread.Start();
        }
    }
}
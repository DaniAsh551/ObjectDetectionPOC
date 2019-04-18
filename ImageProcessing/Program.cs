using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //var image = Image.FromFile("image.bmp");
            //image.DetectEdges().Save("edges.bmp");
            //image.DetectObjects().Save("objects.bmp");
            Application.Run(new FrmMain());

        }
    }
}

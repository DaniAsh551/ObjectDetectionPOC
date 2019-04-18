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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    public static class ImageProcessor
    {
        /// <summary>
        /// Detect objects in the image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Detection[] DetectObjects(this Image image)
        {
            //Detect edges first so that it becomes easier to distinguish one object from another
            var ubmp = UBitmap.FromImage(image.DetectEdges());
            //We'll use this list to keep our 'Points' of interest (no pun intended ;-) )
            var detections = new List<Point>();
            
            //Iterate through each pixel in the image row by row
            //y is row
            for (var y = 0; y < ubmp.Height - 1; y++)
                //x is column
                for (var x = 0; x < ubmp.Width - 1; x++)
                {
                    //Get the pixel represented at the (x,y) coordinates
                    var pixelColor = ubmp.GetPixel(x, y);
                    //get the argb color code of the pixel
                    var pixel = pixelColor.ToArgb();

                    //check if pixel is white
                    if(pixel != -1)
                    {
                        //if not white, add the coordinates to the list
                        detections.Add(new Point(x, y));
                    }
                }

            //If there are no points of interest, we can assume that we have no objects in the image, so no need to continue ¯\_(ツ)_/¯
            if (!detections?.Any() ?? true)
                return null;

            //if multiple points are 'clumped' together we can assume that its from the same object
            var clumps = new List<List<Point>>();
            //add the first point as the first point of the first clump
            clumps.Add(new List<Point> { detections.First() });

            //Always keeping a record of what was the last point we processed
            var lastPoint = clumps.Last().Last();
            //iterate over each point except the first one since we added it already
            foreach (var point in detections.Skip(1))
            {
                //The x coordinates of the last point and this point
                var xs = new int[] { lastPoint.X, point.X };
                //The y coordinates of the last point and this point
                var ys = new int[] { lastPoint.Y, point.Y };
                
                //the difference in the lastpoint's x coordinates
                var xDiff = xs.Max() - xs.Min();
                //the difference in the lastpoint's y coordinates
                var yDiff = ys.Max() - ys.Min();

                //Check to see if the two points are 'clumped together' (there is no blank-white-space between them)
                //This tells us that the points are of the same object
                if (xDiff < 2 || yDiff < 2)
                {
                    //see if we have a previous record of the lastPoint (if lastpoint was interesting and of the same object)
                    var clump = clumps.FirstOrDefault(c => c.Any(p => p.Equals(lastPoint)));

                    //if no record exists, add a new record because why not?
                    if (clump == null)
                    {
                        clump = new List<Point>();
                        //add the new record to the list
                        clumps.Add(clump);
                    }

                    //add the point to the record
                    clump.Add(point);
                    //update the lastpoint
                    lastPoint = point;
                }
                //this block would be processed if there is atleast one blank-white-space between this point and the one before
                else
                {
                    //create a new record, since this is the first time we are dealing with this object
                    var clump = new List<Point>();
                    //add the record to the list
                    clumps.Add(clump);
                    //add the point to the record
                    clump.Add(point);
                    //update the lastpoint
                    lastPoint = point;
                }
            }

            //Make Detection objects out of our clumps (convert them into something meaninful and interpretable)
            var objects = clumps.Select((clump, i) => 
            {
                //Create a new Detection Object
                var d = new Detection
                {
                    //Use index as Id
                    Id = i,
                    //find lowest X coordinate in the clump
                    LX = clump.Min(x => x.X),
                    //find lowest y coordinate in the clump
                    LY = clump.Min(y => y.Y),
                    //find highest X coordinate in the clump
                    HX = clump.Max(x => x.X),
                    //find highest X coordinate in the clump
                    HY = clump.Max(y => y.Y)
                };

                return d;
            }).ToArray();

            //return the objects;
            return objects;

            //var target = new Bitmap(image);
            //using (var pen = new Pen(Color.Aqua))
            //using(var g = Graphics.FromImage(target))
            //foreach (var obj in objects)
            //{
            //        g.DrawRectangle(pen, obj.Rectangle);
            //}

            //return target;
        }

        /// <summary>
        /// Detect the edges in the image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Image DetectEdges(this Image image)
        {
            //Downscale or Upscale
            //image = image.Resize(500);
            //image = image.MakeGrayscale3();

            var bmp = UBitmap.FromImage(image);
            var target = new UBitmap(bmp.Width, bmp.Height);
            //var pixels = new List<Point>();

            //Iterate through each pixel in the image row by row
            //y is row
            for (var y = 0; y < bmp.Height - 1; y++)
                //x is column
                for(var x = 0; x < bmp.Width - 1; x++)
                {
                    //Get the pixel represented at the (x,y) coordinates
                    var pixelColor = bmp.GetPixel(x, y);
                    //get the argb color code of the pixel
                    var pixel = pixelColor.ToArgb();

                    //make sure the pixel is not the first one on either of X and Y planes, because we need to process neighbouring pixels
                    if(x != 0 && y != 0)
                    {
                        //Pixel directly above this one, the names should be self explanatory, so not going into details of those.
                        var top = bmp.GetPixel(x, y - 1).ToArgb();
                        var left = bmp.GetPixel(x - 1, y).ToArgb();
                        var bottom = bmp.GetPixel(x, y + 1).ToArgb();
                        var right = bmp.GetPixel(x + 1, y).ToArgb();
                        var topLeft = bmp.GetPixel(x - 1, y - 1).ToArgb();
                        var topRight = bmp.GetPixel(x + 1, y - 1).ToArgb();
                        var bottomLeft = bmp.GetPixel(x - 1, y + 1).ToArgb();
                        var bottomRight = bmp.GetPixel(x + 1, y + 1).ToArgb();

                        //Take all of the neighbouring pixels into an array for easier processing
                        var colors = new int[] { top,left,bottom,right,topLeft,topRight,bottomLeft,bottomRight };
                        //check whether this pixel is worthy of being added to the 'edge' detection
                        var shouldAdd = colors.Select(color =>
                        {
                            //the pixels to compare
                            //pixel is this pixel, color is the neighbouring one
                            var compare = new int[] { pixel, color };
                            //Find the difference between the two
                            var difference = compare.Max() - compare.Min();
                            //Find the percentage difference between the two
                            //Also we need a positive number, whether they turn negative or positive,
                            //so we convert any negatives into positives and leave positives untouched
                            //If that didn't make any sense, you should read about 'Math.Abs' (Absolute) function
                            //There should be tonnes of articles,answers and explanations on the internet
                            var percentageDifference = Math.Abs(((double)difference / -1.0d) * 100d);
                            //Check if the difference is atleast 50%, if so, we assume it as an edge
                            return percentageDifference >= 50.0d;
                        }).Any(b => b);

                        if (shouldAdd)
                            //Add the pixel as an edge
                            target.SetPixel(x, y, pixelColor);
                    }
                }

            //We cannot have any messy values in the image's pixels so we convert all of them into white color (-1 is white)
            for (var i = 0; i < target.Bits.Length; i++)
            {
                if (target.Bits[i] == 0)
                    target.Bits[i] = -1;
            }

            //return the edge detected image
            return target.Bitmap;
        }

        public static Image Resize(this Image image, int targetWidth, int targetHeight = 0)
        {
            float scale = targetHeight > 0 ? Math.Min(targetWidth / image.Width, targetHeight / image.Height) : targetWidth/image.Width;

            if (targetHeight == 0)
                targetHeight = (int)(image.Height * scale);

            var bmp = new Bitmap(targetWidth, targetHeight);
            var graph = Graphics.FromImage(bmp);

            // uncomment for higher quality output
            //graph.InterpolationMode = InterpolationMode.High;
            //graph.CompositingQuality = CompositingQuality.HighQuality;
            //graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int)(image.Width * scale);
            var scaleHeight = (int)(image.Height * scale);

            using (var brush = new SolidBrush(Color.Black))
            {
                graph.FillRectangle(brush, new RectangleF(0, 0, targetWidth, targetHeight));
                graph.DrawImage(image, (image.Width - scaleWidth) / 2, (image.Height - scaleHeight) / 2, scaleWidth, scaleHeight);
                return bmp;
            }
        }

        public static Image MakeGrayscale3(this Image original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
               });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }
    }
}

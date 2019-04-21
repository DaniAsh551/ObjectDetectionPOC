/// Copyright 2019 Dani / Shum (d4n1.551 / Dani551)
/// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
/// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
/// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
/// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
/// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
/// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
/// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Drawing;

namespace ImageProcessing
{
    /// <summary>
    /// Represents an object detected in the image.
    /// </summary>
    public class Detection
    {
        /// <summary>
        /// The Starting point of the object relative to the image.
        /// </summary>
        public Point Start => new Point(LX, LY);
        /// <summary>
        /// The Ending point of the object relative to the image.
        /// </summary>
        public Point End => new Point(HX, HY);
        /// <summary>
        /// The size of the object on the image in pixels.
        /// </summary>
        public Size Size => new Size(HX - LX, HY - LY);
        /// <summary>
        /// A rectangle representing the bounds of the object.
        /// </summary>
        public Rectangle Rectangle => new Rectangle(Start, Size);
        /// <summary>
        /// A unique number to identify each object, this is the index number of detections.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Lowest X Value occupied by the object on the image.
        /// </summary>
        public int LX { get; set; }
        /// <summary>
        /// Lowest Y Value occupied by the object on the image.
        /// </summary>
        public int LY { get; set; }
        /// <summary>
        /// Hightst X Value occupied by the object on the image.
        /// </summary>
        public int HX { get; set; }
        /// <summary>
        /// Highest Y Value occupied by the object on the image.
        /// </summary>
        public int HY { get; set; }

        /// <summary>
        /// Draws the bounding box of an object on the image.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="pen"></param>
        public void Draw(Graphics graphics, Pen pen)
        {
            graphics.DrawRectangle(pen, Rectangle);
        }
    }
}
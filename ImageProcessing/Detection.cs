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
// This file is part of the NanoleafControlPlugin project.
// 
// Copyright (c) 2022 Dominic Ris
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Loupedeck.NanoleafControlPlugin.Helper
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;

    public static class DrawingHelper
    {
        private static String RESOURCE_PATH = "Loupedeck.NanoleafControlPlugin.Resources";

        public static GraphicsPath RoundedRect(Rectangle bounds, Int32 radius)
        {
            var diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc
            path.AddArc(arc, 180, 90);

            // top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static BitmapImage ReadImage(String imageName, String ext = "png", String addPath = "Images")
            => EmbeddedResources.ReadImage($"{RESOURCE_PATH}.{addPath}.{imageName}.{ext}");

        public static BitmapBuilder LoadBitmapBuilder
        (String imageName = "clear", String text = null, BitmapColor? textColor = null, String ext = "png",
            String addPath = "Images")
            => LoadBitmapBuilder(ReadImage(imageName, ext, addPath), text, textColor);

        public static BitmapBuilder LoadBitmapBuilder
            (BitmapImage image, String text = null, BitmapColor? textColor = null)
        {
            var builder = new BitmapBuilder(90, 90);

            builder.Clear(BitmapColor.Black);
            builder.DrawImage(image);
            // builder.FillRectangle(0, 0, 90, 90, new BitmapColor(0, 0, 0, 100));

            return text is null ? builder : builder.AddTextOutlined(text, textColor: textColor);
        }

        public static BitmapImage LoadBitmapImage
        (String imageName = "clear", String text = null, BitmapColor? textColor = null, String ext = "png",
            String addPath = "Images")
            => LoadBitmapBuilder(imageName, text, textColor, ext, addPath).ToImage();

        public static BitmapImage LoadBitmapImage(BitmapImage image, String text = null, BitmapColor? textColor = null)
            => LoadBitmapBuilder(image, text, textColor).ToImage();

        public static BitmapBuilder AddTextOutlined(this BitmapBuilder builder, String text,
            BitmapColor? outlineColor = null,
            BitmapColor? textColor = null, Int32 fontSize = 12)
        {
            // TODO: Make it outline
            builder.DrawText(text, 0, -25, 90, 90, textColor, fontSize, 0, 0);
            return builder;
        }

        public static BitmapImage DrawDefaultImage(String innerText, String outerText, Color brushColor)
        {
            var imageDimension = 90;
            var dimension = 70;
            using var bitmap = new Bitmap(imageDimension, imageDimension);
            using var g = Graphics.FromImage(bitmap);
            var font = new Font("Arial", 20, FontStyle.Bold);
            var brush = new SolidBrush(brushColor);
            var rect = new Rectangle(0, 0, dimension, dimension / 2);
            rect.X = bitmap.Width / 2 - rect.Width / 2;
            rect.Y = bitmap.Height / 2 - rect.Height / 2;
            var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            g.DrawPath(new Pen(brush.Color, 2), RoundedRect(rect, 15));
            g.DrawAutoAdjustedFont(innerText, font, brush, rect, sf, 20);

            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);

            return LoadBitmapImage(new BitmapImage(ms.ToArray()), outerText);
        }

        public static void DrawAutoAdjustedFont(this Graphics g, String s, Font font, Brush brush, Single x,
            Single y, Int32 maxFontSize = 90, Int32 minFontSize = 5, Boolean smallestOnFail = true, Int32 width = 0)
            => g.DrawAutoAdjustedFont(s, font, brush, new RectangleF(x, y, 0.0f, 0.0f), (StringFormat)null,
                maxFontSize, minFontSize, smallestOnFail);

        public static void DrawAutoAdjustedFont(this Graphics g, String s, Font font, Brush brush, PointF point,
            Int32 maxFontSize = 90, Int32 minFontSize = 5, Boolean smallestOnFail = true, Int32 width = 0)
            => g.DrawAutoAdjustedFont(s, font, brush, new RectangleF(point.X, point.Y, 0.0f, 0.0f),
                (StringFormat)null, maxFontSize, minFontSize, smallestOnFail);

        public static void DrawAutoAdjustedFont(this Graphics g, String s, Font font, Brush brush, Single x, Single y,
            StringFormat format, Int32 maxFontSize = 90, Int32 minFontSize = 5, Boolean smallestOnFail = true,
            Int32 width = 0)
            => g.DrawAutoAdjustedFont(s, font, brush, new RectangleF(x, y, 0.0f, 0.0f), format,
                maxFontSize, minFontSize, smallestOnFail);

        public static void DrawAutoAdjustedFont(this Graphics g, String s, Font font, Brush brush, PointF point,
            StringFormat format, Int32 maxFontSize = 90, Int32 minFontSize = 5, Boolean smallestOnFail = true,
            Int32 width = 0)
            => g.DrawAutoAdjustedFont(s, font, brush, new RectangleF(point.X, point.Y, 0.0f, 0.0f), format,
                maxFontSize, minFontSize, smallestOnFail);

        public static void
            DrawAutoAdjustedFont(this Graphics g, String s, Font font, Brush brush, RectangleF layoutRectangle,
                Int32 maxFontSize = 90, Int32 minFontSize = 5, Boolean smallestOnFail = true, Int32 width = 0)
            => g.DrawAutoAdjustedFont(s, font, brush, layoutRectangle, (StringFormat)null,
                maxFontSize, minFontSize, smallestOnFail);

        public static void DrawAutoAdjustedFont(this Graphics g, String s, Font font, Brush brush,
            RectangleF layoutRectangle, StringFormat format,
            Int32 maxFontSize = 90, Int32 minFontSize = 5, Boolean smallestOnFail = true, Int32 width = 0)
        {
            g.DrawString(
                s,
                g.GetAdjustedFont(s, font, width == 0 ? layoutRectangle.Width : width, maxFontSize, minFontSize,
                    smallestOnFail),
                brush,
                layoutRectangle,
                format
            );
        }

        private static Font GetAdjustedFont(this Graphics g, String graphicString, Font originalFont,
            Single containerWidth, Int32 maxFontSize = 90, Int32 minFontSize = 5, Boolean smallestOnFail = true)
        {
            Font testFont = null;
            for (var adjustedSize = maxFontSize; adjustedSize >= minFontSize; adjustedSize--)
            {
                testFont = new Font(originalFont.Name, adjustedSize, originalFont.Style);

                var adjustedSizeNew = g.MeasureString(graphicString, testFont);

                if (containerWidth > Convert.ToInt32(adjustedSizeNew.Width))
                {
                    return testFont;
                }
            }

            return smallestOnFail ? testFont : originalFont;
        }
    }
}
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
    using System.Globalization;
    using System.IO;

    using SkiaSharp;

    public static class DrawingHelper
    {
        private static String RESOURCE_PATH = "Loupedeck.NanoleafControlPlugin.Resources";

        public static SKPath RoundedRect(SKRect bounds, Int32 radius)
        {
            var diameter = radius * 2;
            SKRect arc = new SKRect(bounds.Left, bounds.Top, bounds.Left + diameter, bounds.Top + diameter);
            SKPath path = new SKPath();

            if (radius == 0)
            {
                path.AddRect(bounds);
                return path;
            }

            // top left arc
            path.ArcTo(arc, 180, 90, false);

            // top right arc
            arc = new SKRect(bounds.Right - diameter, bounds.Top, bounds.Right, bounds.Top + diameter);
            path.ArcTo(arc, 270, 90, false);

            // bottom right arc
            arc = new SKRect(bounds.Right - diameter, bounds.Bottom - diameter, bounds.Right, bounds.Bottom);
            path.ArcTo(arc, 0, 90, false);

            // bottom left arc
            arc = new SKRect(bounds.Left, bounds.Bottom - diameter, bounds.Left + diameter, bounds.Bottom);
            path.ArcTo(arc, 90, 90, false);

            path.Close();
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

        public static BitmapImage DrawDefaultImage(String innerText, String outerText, SKColor brushColor)
        {
            // Set the dimensions and font
            int width = 80;
            int height = 80;
            SKTypeface font = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            int fontSize = 20;

            // Create the canvas and paint
            var info = new SKImageInfo(width, height);
            var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            var paint = new SKPaint { Color = brushColor, IsAntialias = true, Typeface = font };

            // Calculate the dimensions of the rounded rectangle outline
            var rect = new SKRect(5, 20, width - 5, height - 20);

            // Adjust the font size if necessary to fit the inner text within the dimensions of the rounded rectangle outline
            while (true)
            {
                paint.TextSize = fontSize;
                SKRect tb = new SKRect();
                paint.MeasureText(innerText, ref tb);
                if (tb.Width < rect.Width - 5 && tb.Height < rect.Height)
                {
                    break;
                }

                fontSize--;
            }

            // Draw the rounded rectangle outline
            var cornerRadius = Math.Min(width, height) / 2;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;
            canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
            paint.Style = SKPaintStyle.Fill;

            // Draw the inner text centered within the rounded rectangle outline
            paint.TextAlign = SKTextAlign.Center;
            paint.TextSize = fontSize;
            canvas.DrawText(innerText, rect.MidX, rect.MidY - ((paint.FontMetrics.Descent + paint.FontMetrics.Ascent) / 2), paint);

            // Save the image to memory and return the memory streams
            var image = surface.Snapshot();
            var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return LoadBitmapImage(BitmapImage.FromArray(data.ToArray()), outerText);
        }

        public static BitmapImage DrawVolumeBar(PluginImageSize imageSize, BitmapColor backgroundColor, BitmapColor foregroundColor, Single currentValue, Int32 minValue, Int32 maxValue, String cmd, String name = "")
        {
            // Prepare variables
            var dim = imageSize.GetDimension();
            var percentage = (currentValue - minValue) / (maxValue - minValue) * 100;
            var height = (Int32)(dim * 1.0);
            var width = (Int32)(dim * 0.7);
            var calculatedHeight = (Int32)(height * percentage / 100);
            var xCenter = dim / 2 - width / 2;
            var yCenter = dim / 2 + height / 2;
            var builder = new BitmapBuilder(dim, dim);

            // Reset to black
            builder.Clear(BitmapColor.Black);

            // Draw volume bar and border
            builder.DrawRectangle(xCenter, yCenter, width, -height, backgroundColor);
            builder.FillRectangle(xCenter, yCenter, width, -calculatedHeight, backgroundColor);

            // Draw value text at the center
            builder.DrawText(currentValue.ToString(CultureInfo.CurrentCulture), foregroundColor);

            const Int32 fontSize = 16;

            var cmdSize = GetFontSize(fontSize, cmd, dim);

            // Draw cmd text at the bottom
            builder.DrawText(cmd, 0, dim / 2 - cmdSize / 2, dim, dim, foregroundColor, cmdSize, 0, 0);

            // if name is available, draw it over the volume bar
            if (String.IsNullOrEmpty(name))
            {
                return builder.ToImage();
            }

            var nameSize = GetFontSize(fontSize, name, dim);

            // draw the text using the calculated font size
            builder.DrawText(name, 0, dim / 2 * -1 + nameSize / 2, dim, dim, foregroundColor, nameSize, 0, 0);

            return builder.ToImage();
        }

        private static Int32 GetFontSize(Int32 fontSize, String text, Int32 dim)
        {
            // create a SKPaint object for measuring the text
            var paint = new SKPaint { TextSize = fontSize, IsAntialias = true };

            // measure the size of the text
            var textBounds = new SKRect();
            paint.MeasureText(text, ref textBounds);

            // adjust the font size until the text fits within the bounds of the image
            while (textBounds.Width > dim || textBounds.Height > dim)
            {
                fontSize -= 1;
                paint.TextSize = fontSize;
                paint.MeasureText(text, ref textBounds);
            }

            return fontSize;
        }

        public static Int32 GetDimension(this PluginImageSize size) => size == PluginImageSize.Width60 ? 50 : 80;
    }
}
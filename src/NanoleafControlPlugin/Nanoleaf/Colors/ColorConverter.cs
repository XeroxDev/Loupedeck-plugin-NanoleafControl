namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Colors
{
    using System;

    internal class ColorConverter
    {
        public static Hsv RgbToHsv(Double r, Double g, Double b)
        {
            var rr = r / 255;
            var gg = g / 255;
            var bb = b / 255;

            var min = Math.Min(rr, Math.Min(gg, bb));
            var max = Math.Max(rr, Math.Max(gg, bb));
            var chrome = max - min;

            Double H = 0;
            Double S;
            Double V;

            V = max;

            if (chrome == 0) // for gray
            {
                H = 0;
                S = 0;
            }
            else
            {
                S = chrome / max;

                if (rr == max)
                {
                    H = (gg - bb) / chrome;
                    if (gg < bb)
                    {
                        H += 6;
                    }
                }
                else if (gg == max)
                {
                    H = 2 + (bb - rr) / chrome;
                }
                else if (bb == max)
                {
                    H = 4 + (rr - gg) / chrome;
                }

                H *= 60;
            }

            return new Hsv(H, S * 100, V * 100);
        }
    }
}
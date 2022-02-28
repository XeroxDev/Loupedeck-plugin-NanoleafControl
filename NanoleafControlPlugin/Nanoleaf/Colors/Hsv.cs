namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Colors
{
    using System;

    internal class Hsv
    {
        public Hsv(Double h, Double s, Double v)
        {
            this.H = (Int32)Math.Round(h);
            this.S = (Int32)Math.Round(s);
            this.V = (Int32)Math.Round(v);
        }

        public Int32 H { get; }

        public Int32 S { get; }

        public Int32 V { get; }
    }
}
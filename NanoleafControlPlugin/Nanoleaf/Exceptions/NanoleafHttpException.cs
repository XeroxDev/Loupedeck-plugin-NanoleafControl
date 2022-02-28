namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Exceptions
{
    using System;

    public class NanoleafHttpException : Exception
    {
        public NanoleafHttpException(String message) : base(message)
        {
        }
    }
}
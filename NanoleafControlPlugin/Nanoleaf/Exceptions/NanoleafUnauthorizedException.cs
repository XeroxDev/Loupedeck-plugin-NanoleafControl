namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Exceptions
{
    using System;

    public class NanoleafUnauthorizedException : NanoleafHttpException
    {
        public NanoleafUnauthorizedException(String message) : base(message)
        {
        }
    }
}
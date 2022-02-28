namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Exceptions
{
    using System;

    public class NanoleafResourceNotFoundException : NanoleafHttpException
    {
        public NanoleafResourceNotFoundException(String message) : base(message)
        {
        }
    }
}
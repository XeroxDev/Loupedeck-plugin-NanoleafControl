namespace Loupedeck.NanoleafControlPlugin.Nanoleaf
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    /// <summary>
    ///     Streaming client used for sending UDP color data to Nanoleaf Devices
    /// </summary>
    public class NanoleafStreamingClient : IDisposable
    {
        private readonly Boolean _disposeSender;
        private readonly IPEndPoint _ipEndPoint;
        private readonly UdpClient _sender;
        private readonly Int32 _streamMode;

        /// <summary>
        ///     Create a new nanoleaf streaming client
        /// </summary>
        /// <param name="target">The IP Address or hostname of the nanoleaf device.</param>
        /// <param name="streamMode">
        ///     (Optional) Streaming mode supported by the device. See the nanoleaf documentation for more
        ///     info.
        /// </param>
        /// <param name="sender">
        ///     (Optional) If specified, use a shared UdpClient. Be sure to disable blocking and
        ///     set the socket options to ReuseAddress, or you will encounter issues.
        /// </param>
        public NanoleafStreamingClient(String target, Int32 streamMode = 2, UdpClient sender = null)
        {
            this._ipEndPoint = Parse(target, 60222);

            if (sender != null)
            {
                this._sender = sender;
            }
            else
            {
                this._disposeSender = true;
                this._sender = new UdpClient();
                this._sender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this._sender.Client.Blocking = false;
                this._sender.DontFragment = true;
            }

            this._streamMode = streamMode;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this._disposeSender)
            {
                this._sender?.Dispose();
            }
        }

        #endregion


        /// <summary>
        ///     Send a UDP packet with updated color data
        /// </summary>
        /// <param name="colors">
        ///     A dictionary of int, color; where int is the Panel ID,
        ///     and color is a System.Drawing.Color to set. Use <see cref="M:NanoleafClient.GetLayoutAsync" /> to get layout info.
        /// </param>
        /// <param name="fadeTime"></param>
        public async Task SetColorAsync(Dictionary<Int32, Color> colors, Int32 fadeTime = 0)
        {
            var byteString = new List<Byte>();
            if (this._streamMode == 2)
            {
                byteString.AddRange(PadInt(colors.Count));
            }
            else
            {
                byteString.Add(IntByte(colors.Count));
            }

            foreach (var pd in colors)
            {
                var id = pd.Key;
                if (this._streamMode == 2)
                {
                    byteString.AddRange(PadInt(id));
                }
                else
                {
                    byteString.Add(IntByte(id));
                }

                var color = pd.Value;

                // Add rgb values
                byteString.Add(IntByte(color.R));
                byteString.Add(IntByte(color.G));
                byteString.Add(IntByte(color.B));
                // White value
                byteString.AddRange(PadInt(0, 1));
                // Pad duration time
                byteString.AddRange(this._streamMode == 2 ? PadInt(fadeTime) : PadInt(fadeTime, 1));
            }

            await this.SendUdpUnicastAsync(byteString.ToArray());
        }

        private static Byte[] PadInt(Int32 toPad, Int32 take = 2)
        {
            var intBytes = BitConverter.GetBytes(toPad);
            Array.Reverse(intBytes);
            intBytes = intBytes.Reverse().Take(take).Reverse().ToArray();
            return intBytes;
        }

        private static Byte IntByte(Int32 toByte, String format = "X2")
        {
            var b = Convert.ToByte(toByte.ToString(format, CultureInfo.InvariantCulture), 16);
            return b;
        }

        private async Task SendUdpUnicastAsync(Byte[] data)
        {
            if (this._ipEndPoint != null)
            {
                await this._sender.SendAsync(data, data.Length, this._ipEndPoint);
            }
            else
            {
                throw new Exception("Error, no endpoint");
            }
        }

        private static IPEndPoint Parse(String endpoint, Int32 portIn)
        {
            if (String.IsNullOrEmpty(endpoint)
                || endpoint.Trim().Length == 0)
            {
                throw new ArgumentException("Endpoint descriptor may not be empty.");
            }

            if (portIn != -1 &&
                (portIn < IPEndPoint.MinPort
                 || portIn > IPEndPoint.MaxPort))
            {
                throw new ArgumentException(String.Format("Invalid default port '{0}'", portIn));
            }

            var values = endpoint.Split(':');
            IPAddress ipaddy;
            Int32 port;

            //check if we have an IPv6 or ports
            if (values.Length <= 2) // ipv4 or hostname
            {
                if (values.Length == 1)
                    //no port is specified, default
                {
                    port = portIn;
                }
                else
                {
                    port = GetPort(values[1]);
                }

                //try to use the address as IPv4, otherwise get hostname
                if (!IPAddress.TryParse(values[0], out ipaddy))
                {
                    ipaddy = GetIpFromHost(values[0]);
                }

                if (ipaddy == null)
                {
                    return null;
                }
            }
            else if (values.Length > 2) //ipv6
            {
                //could [a:b:c]:d
                if (values[0].StartsWith("[") && values[values.Length - 2].EndsWith("]"))
                {
                    var ipaddressstring = String.Join(":", values.Take(values.Length - 1).ToArray());
                    ipaddy = IPAddress.Parse(ipaddressstring);
                    port = GetPort(values[values.Length - 1]);
                }
                else //[a:b:c] or a:b:c
                {
                    ipaddy = IPAddress.Parse(endpoint);
                    port = portIn;
                }
            }
            else
            {
                throw new FormatException(String.Format("Invalid endpoint ipaddress '{0}'", endpoint));
            }

            if (port == -1)
            {
                throw new ArgumentException(String.Format("No port specified: '{0}'", endpoint));
            }

            return new IPEndPoint(ipaddy, port);
        }

        private static Int32 GetPort(String p)
        {
            Int32 port;

            if (!Int32.TryParse(p, out port)
                || port < IPEndPoint.MinPort
                || port > IPEndPoint.MaxPort)
            {
                throw new FormatException($@"Invalid end point port '{p}'");
            }

            return port;
        }

        private static IPAddress GetIpFromHost(String p)
        {
            if (String.IsNullOrEmpty(p))
            {
                return null;
            }

            var hosts = Dns.GetHostAddresses(p);

            if (hosts == null || hosts.Length == 0)
            {
                throw new ArgumentException(String.Format("Host not found: {0}", p));
            }

            return hosts[0];
        }
    }
}
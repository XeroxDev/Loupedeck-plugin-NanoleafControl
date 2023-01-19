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

namespace Loupedeck.NanoleafControlPlugin.Nanoleaf.Discovery
{
    using System;
    using System.Collections.Generic;

    using Types;

    using Zeroconf;

    public class NanoleafDiscovery
    {
        public List<Device> Devices { get; } = new List<Device>();
        private static String DiscoveryKey => "_nanoleafapi._tcp.local.";

        public void Discover()
        {
            var listener = ZeroconfResolver.CreateListener(DiscoveryKey);
            listener.Error += this.Error;
            listener.ServiceFound += this.HandleNewDevice;
            listener.ServiceLost += this.HandleDeviceLost;
        }

        private void HandleNewDevice(Object sender, IZeroconfHost host)
        {
            foreach (var service in host.Services)
            {
                if (!service.Key.EndsWith(DiscoveryKey))
                {
                    continue;
                }

                var deviceData = service.Value;
                var idFound = deviceData.Properties[0].TryGetValue("id", out var id);
                var ip = host.IPAddress;
                var port = deviceData.Port;
                if (!idFound)
                {
                    continue;
                }

                var deviceFound = this.Devices.Find(nanoleafDevice => nanoleafDevice.Id.Equals(id));
                if (deviceFound != null)
                {
                    deviceFound.StopDisconnect();
                    continue;
                }

                var device = new Device(id, new NanoleafClient($"{ip}:{port}"), host.DisplayName);
                this.Devices.Add(device);
                this.DeviceFound.Invoke(this, device);
            }
        }

        private void HandleDeviceLost(Object sender, IZeroconfHost host)
        {
            foreach (var service in host.Services)
            {
                if (!service.Key.EndsWith(DiscoveryKey))
                {
                    continue;
                }
                

                var idFound = service.Value.Properties[0].TryGetValue("id", out var id);
                if (!idFound)
                {
                    continue;
                }

                var device = this.Devices.Find(nanoleafDevice => nanoleafDevice.Id.Equals(id));
                if (device is null)
                {
                    continue;
                }

                device.Disconnected += this.DeviceDisconnected;
                device.Reconnected += this.DeviceReconnected;
                _ = device.StartDisconnect();
            }
        }

        private void DeviceDisconnected(Object _, Device device)
        {
            device.Disconnected -= this.DeviceDisconnected;
            device.Reconnected -= this.DeviceReconnected;
            this.Devices.Remove(device);
        }

        private void DeviceReconnected(Object _, Device device)
        {
            device.Disconnected -= this.DeviceDisconnected;
            device.Reconnected -= this.DeviceReconnected;
        }

        #region Events

        public event EventHandler<Device> DeviceFound = delegate { };
        public event EventHandler<Exception> Error = delegate(Object _, Exception exception) { Console.WriteLine(exception.Message); };

        #endregion
    }
}
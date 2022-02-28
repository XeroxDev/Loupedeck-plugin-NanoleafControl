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

namespace Loupedeck.NanoleafControlPlugin
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Helper;

    using Nanoleaf.Discovery;
    using Nanoleaf.Types;

    public class NanoleafControlPlugin : Plugin
    {
        public static NanoleafDiscovery Discovery;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public NanoleafControlPlugin()
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            Discovery = new NanoleafDiscovery();
            Discovery.Discover();

            Task.Run(async () =>
            {
                while (!this._cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        this.CheckAuthentication();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    await Task.Delay(5000);
                }
            }, this._cancellationTokenSource.Token);
        }

        public override Boolean HasNoApplication => true;
        public override Boolean UsesApplicationApiOnly => true;

        public Boolean TryGetDeviceSetting(String deviceId, String settingName, out String settingValue) =>
            this.TryGetPluginSetting($"{deviceId}-{settingName}", out settingValue);

        public void SetDeviceSetting(String deviceId, String settingName, String settingValue, Boolean backupOnline = false) =>
            this.SetPluginSetting($"{deviceId}-{settingName}", settingValue, backupOnline);

        public void DeleteDeviceSetting(String deviceId, String settingName) =>
            this.DeletePluginSetting($"{deviceId}-{settingName}");

        private void CheckAuthentication()
        {
            foreach (var device in Discovery.Devices)
            {
                if (device.Authorized || !this.TryGetDeviceSetting(device.Id, "token", out var authToken))
                {
                    continue;
                }

                this.AuthenticateDevice(device, authToken);
            }
        }

        private void AuthenticateDevice(Device device, String authToken) => device.Authorize(authToken);

        #region Loupedeck

        public override void Load()
        {
            this.Info.Icon16x16 = DrawingHelper.ReadImage("icon-16", addPath: "Icon");
            this.Info.Icon32x32 = DrawingHelper.ReadImage("icon-32", addPath: "Icon");
            this.Info.Icon48x48 = DrawingHelper.ReadImage("icon-48", addPath: "Icon");
            this.Info.Icon256x256 = DrawingHelper.ReadImage("icon-256", addPath: "Icon");
        }

        public override void Unload() => this._cancellationTokenSource.Cancel();

        public override void RunCommand(String commandName, String parameter)
        {
        }

        public override void ApplyAdjustment(String adjustmentName, String parameter, Int32 diff)
        {
        }

        #endregion
    }
}
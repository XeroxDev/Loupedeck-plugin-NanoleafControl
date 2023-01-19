namespace Loupedeck.NanoleafControlPlugin
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Nanoleaf.Discovery;
    using Nanoleaf.Types;


    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class NanoleafControlPlugin : Plugin
    {
        public static NanoleafDiscovery Discovery;
        private readonly CancellationTokenSource _cancellationTokenSource;
        
        // Gets a value indicating whether this is an Universal plugin or an Application plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean HasNoApplication => true;
        
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

        // This method is called when the plugin is loaded during the Loupedeck service start-up.
        public override void Load()
        {
        }

        // This method is called when the plugin is unloaded during the Loupedeck service shutdown.
        public override void Unload() => this._cancellationTokenSource.Cancel();

        #endregion
    }
}

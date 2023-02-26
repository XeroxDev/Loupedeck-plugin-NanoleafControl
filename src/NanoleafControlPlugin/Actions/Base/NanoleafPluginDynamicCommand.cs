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

namespace Loupedeck.NanoleafControlPlugin.Actions.Base
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Nanoleaf.Discovery;
    using Nanoleaf.Types;

    #endregion

    /// <summary>
    ///     Base class for simple commands.
    /// </summary>
    public class NanoleafPluginDynamicCommand : PluginDynamicCommand
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly List<Device> _devices = new List<Device>();

        private Boolean _listening;

        public NanoleafPluginDynamicCommand() { }

        public NanoleafPluginDynamicCommand(DeviceType supportedDevice) : base(supportedDevice) { }

        public NanoleafPluginDynamicCommand(String displayName, String description, String groupName, DeviceType supportedDevice = DeviceType.All) : base(displayName, description, groupName,
            supportedDevice)
        {
        }

        private NanoleafDiscovery _discovery
        {
            get
            {
                if (NanoleafControlPlugin.Discovery != null)
                {
                    return NanoleafControlPlugin.Discovery;
                }

                while (NanoleafControlPlugin.Discovery is null)
                {
                    Task.Delay(100).Wait();
                }

                return NanoleafControlPlugin.Discovery;
            }
        }

        protected CancellationToken Token => this._cancellationTokenSource.Token;

        /// <summary>
        ///     Gets the base / initial class for the plugin.
        /// </summary>
        protected NanoleafControlPlugin NanoleafPlugin { get; private set; }

        /// <inheritdoc />
        protected override Boolean OnUnload()
        {
            this._discovery.DeviceFound -= this.RegisterFoundDevice;
            this._discovery.Error -= this.ErroredDevice;
            this._listening = false;
            this._cancellationTokenSource.Cancel();
            return base.OnUnload();
        }

        /// <inheritdoc />
        protected override Boolean OnLoad()
        {
            this.NanoleafPlugin = this.Plugin as NanoleafControlPlugin;
            new Thread(this.Init).Start();
            return base.OnLoad();
        }

        /// <summary>
        ///     Method gets called after a device was found and the base class set everything up.
        /// </summary>
        /// <param name="sender">Class this method got called from</param>
        /// <param name="device">The device which got found</param>
        /// <returns>A task</returns>
        protected virtual Task DeviceFound(Object sender, Device device) => Task.CompletedTask;

        /// <summary>
        ///     Method gets called after a device got lost and the base class set everything up.
        /// </summary>
        /// <param name="sender">Class this method got called from</param>
        /// <param name="device">The device which got lost</param>
        /// <returns>A task</returns>
        protected virtual Task DeviceLost(Object sender, Device device) => Task.CompletedTask;

        /// <summary>
        ///     Method gets called after an error occured.
        /// </summary>
        /// <param name="sender">Class this method got called from</param>
        /// <param name="e">The error that occured</param>
        /// <returns>A task</returns>
        protected virtual Task Error(Object sender, Exception e) => Task.CompletedTask;

        /// <summary>
        ///     Finds and returns device by its id.
        /// </summary>
        /// <param name="id">The id to search for</param>
        /// <returns>A task</returns>
        protected Device GetDevice(String id) => this._devices.Find(d => d.Id == id);

        /// <summary>
        ///     Initializes the action.
        /// </summary>
        private void Init()
        {
            if (this._listening)
            {
                return;
            }

            this._discovery.DeviceFound += this.RegisterFoundDevice;
            this._discovery.Error += this.ErroredDevice;

            this._discovery.Devices.ForEach(d => this.RegisterFoundDevice(this, d));

            _ = this.Timer();
            this._listening = true;
        }

        /// <summary>
        ///     Timer which calles the <see cref="TimerTick" /> method every second.
        /// </summary>
        private async Task Timer()
        {
            if (this.Token.IsCancellationRequested)
            {
                return;
            }

            await this.TimerTick();
            await Task.Delay(1000, this.Token);
            _ = this.Timer();
        }

        /// <summary>
        ///     Wrapper which handels the <see cref="Device.AuthenticationChanged" /> event and calls the
        ///     <see cref="HandleAuthenticationChanged" /> method.
        /// </summary>
        /// <param name="sender">Class this method got called from</param>
        /// <param name="device">The device that was changed</param>
        private void HandleAuthenticationChanged(Object sender, Device device) => this.AuthenticationChanged(sender, device);

        /// <summary>
        ///     Registers a found device and calles the <see cref="DeviceFound" /> method.
        /// </summary>
        /// <param name="sender">Class this method got called from</param>
        /// <param name="device">The device which got found</param>
        private void RegisterFoundDevice(Object sender, Device device)
        {
            this._devices.Add(device);

            device.Disconnected += this.UnregisterDevice;
            device.AuthenticationChanged += this.HandleAuthenticationChanged;

            this.HandleParameter(device, false);
            this.DeviceFound(sender, device);
        }

        /// <summary>
        ///     Unregisters found device after it got lost, calles the <see cref="DeviceLost" /> and <see cref="HandleParameter" />
        ///     methods.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void UnregisterDevice(Object sender, Device device)
        {
            this._devices.Remove(device);

            device.Disconnected -= this.UnregisterDevice;
            device.AuthenticationChanged -= this.HandleAuthenticationChanged;

            this.HandleParameter(device, true);
            this.DeviceLost(sender, device);
        }

        /// <summary>
        ///     Adds / removes a action per device.
        /// </summary>
        /// <param name="device">The device to add / remove action from</param>
        /// <param name="remove">True if the action should be removed, false if it should be added</param>
        protected virtual void HandleParameter(Device device, Boolean remove)
        {
            if (this.DisplayName is null)
            {
                return;
            }

            var available = this.HasParameter && this.TryGetParameter(device.Id, out _);

            switch (remove)
            {
                case true when available:
                    this.RemoveParameter(device.Id);
                    break;
                case false when !available:
                    this.AddParameter(device.Id, this.DisplayName, device.DisplayName, this.SuperGroupName);
                    break;
            }
        }

        /// <summary>
        ///     Is getting called if <see cref="NanoleafDiscovery.Error" /> is raised.
        /// </summary>
        /// <param name="sender">Class this method got called from</param>
        /// <param name="e">The error that occured</param>
        private void ErroredDevice(Object sender, Exception e) => this.Error(sender, e);

        /// <summary>
        ///     Tries to get a device from id.
        /// </summary>
        /// <param name="actionParameter">The id of the device</param>
        /// <param name="device">The device which got found</param>
        /// <param name="checkForAuthentication">
        ///     True if the device should only be returned, if it's authenticated, false if it
        ///     should be returned even if it's not authenticated
        /// </param>
        /// <returns>True if the device was found, false if not</returns>
        protected Boolean TryGetDevice(String actionParameter, out Device device, Boolean checkForAuthentication = false)
        {
            device = null;
            if (actionParameter is null)
            {
                return false;
            }

            device = this.GetDevice(actionParameter);

            if (device is null)
            {
                return false;
            }

            return !checkForAuthentication || device.Authorized;
        }

        /// <summary>
        ///     Gets called if <see cref="Device.Authorized" /> was changed
        /// </summary>
        /// <param name="sender">Class this method got called from</param>
        /// <param name="device">The device which got changed</param>
        /// <returns>A task</returns>
        protected virtual Task AuthenticationChanged(Object sender, Device device) => Task.CompletedTask;

        /// <summary>
        ///     Gets called every second from <see cref="Timer" />
        /// </summary>
        /// <returns></returns>
        protected virtual Task TimerTick() => Task.CompletedTask;
    }
}
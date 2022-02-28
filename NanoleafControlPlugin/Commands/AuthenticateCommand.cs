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

namespace Loupedeck.NanoleafControlPlugin.Commands
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Base;

    using Helper;

    public class AuthenticateCommand : NanoleafPluginDynamicCommand
    {
        public AuthenticateCommand()
        {
            this.DisplayName = "Authenticate";
            this.Description = "Authenticate with the Nanoleaf device";
            this.SuperGroupName = "Authentication";
        }

        /// <summary>
        ///     Starts authentication process with the Nanoleaf device.
        /// </summary>
        /// <param name="actionParameter">The id of the device</param>
        protected override async void RunCommand(String actionParameter)
        {
            if (!this.TryGetDevice(actionParameter, out var device))
            {
                return;
            }

            if (device.Authorized)
            {
                MessageHelper.Notify("Info", "Already authorized", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (this.NanoleafPlugin.TryGetDeviceSetting(device.Id, "token", out var authToken))
            {
                device.Authorize(authToken);
                MessageHelper.Notify("Authentication Successful", "You can now use this device from your Loupedeck!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var msgBox = MessageHelper.Show(
                "Authenticate",
                "Please hold the power button on your Nanoleaf for 5-7 seconds to start the authentication process and press OK.",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (msgBox != DialogResult.OK)
            {
                return;
            }

            String token;

            try
            {
                token = (await device.Client.CreateTokenAsync()).Token;
            }
            catch (Exception ex)
            {
                MessageHelper.Notify("Authentication Error", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (token is null)
            {
                MessageHelper.Notify("Authentication Error", "Couldn't authenticate, please try again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            device.Authorize(token);
            if (!device.Authorized)
            {
                MessageHelper.Notify("Authentication Error", "Couldn't authenticate, please try again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                await device.Client.GetInfoAsync();
            }
            catch (Exception ex)
            {
                MessageHelper.Notify("Authentication Error", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.NanoleafPlugin.SetDeviceSetting(device.Id, "token", token);

            MessageHelper.Notify("Authentication Successful", "You can now use this device from your Loupedeck!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows if device is authenticated
        /// </summary>
        /// <param name="actionParameter">The id of the device</param>
        /// <param name="imageSize"></param>
        /// <returns></returns>
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (!this.TryGetDevice(actionParameter, out var device))
            {
                return null;
            }

            return device.Authorized ? "Authenticated" : "Authenticate";
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
            !this.TryGetDevice(actionParameter, out var device)
                ? null
                : DrawingHelper.DrawDefaultImage(device.Authorized ? "Authenticated" : "Authenticate", $"{device.DisplayName}", device.Authorized ? Color.Green : Color.Red);
    }
}
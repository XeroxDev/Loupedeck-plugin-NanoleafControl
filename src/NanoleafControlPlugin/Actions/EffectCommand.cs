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

namespace Loupedeck.NanoleafControlPlugin.Actions
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Helper;

    using Nanoleaf.Discovery;

    using SkiaSharp;

    public class EffectCommand : PluginDynamicCommand
    {
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

        public EffectCommand() : base("Effects", "Device effects", "Effects") => this.MakeProfileAction("tree");

        protected override PluginProfileActionData GetProfileActionData()
        {
            var tree = new PluginProfileActionTree("Selection", "Select device and effect");

            tree.AddLevel("Device", "Select device");
            tree.AddLevel("Effect", "Select effect");

            var devices = this._discovery.Devices.FindAll(device => device.Authorized);
            foreach (var device in devices)
            {
                var node = tree.Root.AddNode(device.DisplayName);
                var items = device.Client.GetEffectsAsync().Result;

                foreach (var item in items)
                {
                    node.AddItem($"{device.Id}---{item}", item);
                }
            }

            return tree;
        }

        protected override void RunCommand(String actionParameter)
        {
            if (actionParameter is null)
            {
                return;
            }

            var regex = new Regex(@"(?<deviceId>.+)---(?<effect>.+)");
            var match = regex.Match(actionParameter);

            if (!match.Success)
            {
                return;
            }

            var deviceId = match.Groups["deviceId"].Value;
            var effect = match.Groups["effect"].Value;

            this._discovery.Devices.Find(d => d.Id == deviceId)?.Client.SetEffectAsync(effect).Wait();
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (actionParameter is null)
            {
                return null;
            }

            var regex = new Regex(@"(?<deviceId>.+)---(?<effect>.+)");
            var match = regex.Match(actionParameter);

            if (!match.Success)
            {
                return null;
            }

            var deviceId = match.Groups["deviceId"].Value;
            var effect = match.Groups["effect"].Value;
            var device = this._discovery.Devices.Find(d => d.Id == deviceId);

            if (device is null || !device.Authorized)
            {
                return DrawingHelper.DrawDefaultImage("Not available", device?.DisplayName ?? "Unknown", SKColors.Red);
            }

            var currentEffect = device.Client.GetCurrentEffectAsync().Result;
            var isCurrentEffect = currentEffect == effect;
            return DrawingHelper.DrawDefaultImage(isCurrentEffect ? "On" : "Off", $"{device.DisplayName}\n{effect}", isCurrentEffect ? SKColors.Green : SKColors.Red);
        }
    }
}
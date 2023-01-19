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

namespace Loupedeck.NanoleafControlPlugin.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class Debouncer : IDisposable
    {
        readonly TimeSpan _ts;
        readonly Action _action;
        readonly HashSet<ManualResetEvent> _resets = new HashSet<ManualResetEvent>();
        readonly Object _mutex = new Object();

        public Debouncer(TimeSpan timespan, Action action)
        {
            this._ts = timespan;
            this._action = action;
        }

        public void Invoke()
        {
            var thisReset = new ManualResetEvent(false);

            lock (this._mutex)
            {
                while (this._resets.Count > 0)
                {
                    var otherReset = this._resets.First();
                    this._resets.Remove(otherReset);
                    otherReset.Set();
                }

                this._resets.Add(thisReset);
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    if (!thisReset.WaitOne(this._ts))
                    {
                        this._action();
                    }
                }
                finally
                {
                    lock (this._mutex)
                    {
                        using (thisReset)
                        {
                            this._resets.Remove(thisReset);
                        }
                    }
                }
            });
        }

        public void Dispose()
        {
            lock (this._mutex)
            {
                while (this._resets.Count > 0)
                {
                    var reset = this._resets.First();
                    this._resets.Remove(reset);
                    reset.Set();
                }
            }
        }
    }
}
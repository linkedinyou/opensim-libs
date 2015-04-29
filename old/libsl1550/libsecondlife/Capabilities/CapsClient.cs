/*
 * Copyright (c) 2007, Second Life Reverse Engineering Team
 * All rights reserved.
 *
 * - Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions are met:
 *
 * - Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 * - Neither the name of the Second Life Reverse Engineering Team nor the names
 *   of its contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Net;
using libsecondlife.StructuredData;

namespace libsecondlife.Capabilities
{
    public class CapsClient
    {
        public delegate void ProgressCallback(CapsClient client, long bytesReceived, long bytesSent, 
            long totalBytesToReceive, long totalBytesToSend);
        public delegate void CompleteCallback(CapsClient client, LLSD result, Exception error);

        public ProgressCallback OnProgress;
        public CompleteCallback OnComplete;

        public IWebProxy Proxy;
        public object UserData;

        protected CapsBase _Client;
        protected byte[] _PostData;
        protected string _ContentType;

        public CapsClient(Uri capability)
        {
            _Client = new CapsBase(capability);
            _Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
            _Client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(Client_DownloadDataCompleted);
            _Client.UploadProgressChanged += new UploadProgressChangedEventHandler(Client_UploadProgressChanged);
            _Client.UploadDataCompleted += new UploadDataCompletedEventHandler(Client_UploadDataCompleted);
        }

        public void StartRequest()
        {
            StartRequest(null, null);
        }

        public void StartRequest(LLSD llsd)
        {
            byte[] postData = LLSDParser.SerializeXmlBytes(llsd);
            StartRequest(postData, null);
        }

        public void StartRequest(byte[] postData)
        {
            StartRequest(postData, null);
        }

        public void StartRequest(byte[] postData, string contentType)
        {
            _PostData = postData;
            _ContentType = contentType;

            if (_Client.IsBusy)
            {
                SecondLife.LogStatic("New CAPS request to " + _Client.Location +
                    " initiated, closing previous request", Helpers.LogLevel.Warning);
                _Client.CancelAsync();
            }
            else
            {
                SecondLife.DebugLogStatic("New CAPS request to " + _Client.Location + " initiated");
            }
            
            // Proxy
            if (Proxy != null)
                _Client.Proxy = Proxy;

            // Content-Type
            _Client.Headers.Clear();
            if (!String.IsNullOrEmpty(contentType))
                _Client.Headers.Add(HttpRequestHeader.ContentType, contentType);
            else
                _Client.Headers.Add(HttpRequestHeader.ContentType, "application/xml");

            if (postData == null)
                _Client.DownloadStringAsync(_Client.Location);
            else
                _Client.UploadDataAsync(_Client.Location, postData);
        }

        public void Cancel()
        {
            if (_Client.IsBusy)
                _Client.CancelAsync();
        }

        #region Callback Handlers

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (OnProgress != null)
            {
                try { OnProgress(this, e.BytesReceived, 0, e.TotalBytesToReceive, 0); }
                catch (Exception ex) { SecondLife.LogStatic(ex.ToString(), Helpers.LogLevel.Error); }
            }
        }

        private void Client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (OnComplete != null && !e.Cancelled)
            {
                if (e.Error == null)
                {
                    LLSD result = LLSDParser.DeserializeXml(e.Result);

                    try { OnComplete(this, result, e.Error); }
                    catch (Exception ex) { SecondLife.LogStatic(ex.ToString(), Helpers.LogLevel.Error); }
                }
                else
                {
                    if (Helpers.StringContains(e.Error.Message, "502"))
                    {
                        // These are normal, retry the request automatically
                        StartRequest(_PostData, _ContentType);
                    }
                    else
                    {
                        try { OnComplete(this, null, e.Error); }
                        catch (Exception ex) { SecondLife.LogStatic(ex.ToString(), Helpers.LogLevel.Error); }
                    }
                }
            }
        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            if (OnProgress != null)
            {
                try { OnProgress(this, e.BytesReceived, e.BytesSent, e.TotalBytesToReceive, e.TotalBytesToSend); }
                catch (Exception ex) { SecondLife.LogStatic(ex.ToString(), Helpers.LogLevel.Error); }
            }
        }

        private void Client_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            if (OnComplete != null && !e.Cancelled)
            {
                if (e.Error == null)
                {
                    LLSD result = LLSDParser.DeserializeXml(e.Result);

                    try { OnComplete(this, result, e.Error); }
                    catch (Exception ex) { SecondLife.LogStatic(ex.ToString(), Helpers.LogLevel.Error); }
                }
                else
                {
                    if (Helpers.StringContains(e.Error.Message, "502"))
                    {
                        // These are normal, retry the request automatically
                        SecondLife.DebugLogStatic("502 error from capability " + _Client.Location);
                        StartRequest(_PostData, _ContentType);
                    }
                    else
                    {
                        try { OnComplete(this, null, e.Error); }
                        catch (Exception ex) { SecondLife.LogStatic(ex.ToString(), Helpers.LogLevel.Error); }
                    }
                }
            }
            else if (e.Cancelled)
            {
                SecondLife.DebugLogStatic("Capability action at " + _Client.Location + " cancelled");
            }
        }

        #endregion Callback Handlers
    }
}

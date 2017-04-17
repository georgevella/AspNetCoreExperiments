// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Context = Microsoft.AspNetCore.Hosting.Internal.HostingApplication.Context;

namespace Microsoft.AspNetCore.TestHost
{
    public class XTestServer : IServer
    {
        private const string DefaultEnvironmentName = "Development";
        private const string ServerName = nameof(TestServer);
        private IWebHost _hostInstance;
        private bool _disposed = false;
        private IHttpApplication<Context> _application;

        public XTestServer(IWebHostBuilder builder)
        {
            var host = builder.UseServer(this).Build();
            host.Start();
            _hostInstance = host;
        }

        public Uri BaseAddress { get; set; } = new Uri("http://localhost/");

        public IWebHost Host
        {
            get
            {
                return _hostInstance;
            }
        }

        IFeatureCollection IServer.Features { get; }

        public HttpMessageHandler CreateHandler()
        {
            var pathBase = BaseAddress == null ? PathString.Empty : PathString.FromUriComponent(BaseAddress);
            return new XClientHandler(pathBase, _application);
        }

        public HttpClient CreateClient()
        {
            return new HttpClient(CreateHandler()) { BaseAddress = BaseAddress };
        }

        //public WebSocketClient CreateWebSocketClient()
        //{
        //    //var pathBase = BaseAddress == null ? PathString.Empty : PathString.FromUriComponent(BaseAddress);
        //    //return new WebSocketClient(pathBase, _application);
        //}

        /// <summary>
        /// Begins constructing a request message for submission.
        /// </summary>
        /// <param name="path"></param>
        /// <returns><see cref="RequestBuilder"/> to use in constructing additional request details.</returns>
        public RequestBuilder CreateRequest(string path)
        {
            return new RequestBuilder(this, path);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _hostInstance.Dispose();
            }
        }

        void IServer.Start<TContext>(IHttpApplication<TContext> application)
        {
            _application = new ApplicationWrapper<Context>((IHttpApplication<Context>)application, () =>
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
            });
        }

        private class ApplicationWrapper<TContext> : IHttpApplication<TContext>
        {
            private readonly IHttpApplication<TContext> _application;
            private readonly Action _preProcessRequestAsync;

            public ApplicationWrapper(IHttpApplication<TContext> application, Action preProcessRequestAsync)
            {
                _application = application;
                _preProcessRequestAsync = preProcessRequestAsync;
            }

            public TContext CreateContext(IFeatureCollection contextFeatures)
            {
                return _application.CreateContext(contextFeatures);
            }

            public void DisposeContext(TContext context, Exception exception)
            {
                _application.DisposeContext(context, exception);
            }

            public Task ProcessRequestAsync(TContext context)
            {
                _preProcessRequestAsync();
                return _application.ProcessRequestAsync(context);
            }
        }
    }

    /// <summary>
    /// Used to construct a HttpRequestMessage object.
    /// </summary>
    public class RequestBuilder
    {
        private readonly XTestServer _server;
        private readonly HttpRequestMessage _req;

        /// <summary>
        /// Construct a new HttpRequestMessage with the given path.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="path"></param>
        public RequestBuilder(XTestServer server, string path)
        {
            if (server == null)
            {
                throw new ArgumentNullException(nameof(server));
            }

            _server = server;
            _req = new HttpRequestMessage(HttpMethod.Get, path);
        }

        /// <summary>
        /// Configure any HttpRequestMessage properties.
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public RequestBuilder And(Action<HttpRequestMessage> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            configure(_req);
            return this;
        }

        /// <summary>
        /// Add the given header and value to the request or request content.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public RequestBuilder AddHeader(string name, string value)
        {
            if (!_req.Headers.TryAddWithoutValidation(name, value))
            {
                if (_req.Content == null)
                {
                    _req.Content = new StreamContent(Stream.Null);
                }
                if (!_req.Content.Headers.TryAddWithoutValidation(name, value))
                {
                    // TODO: throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidHeaderName, name), "name");
                    throw new ArgumentException("Invalid header name: " + name, "name");
                }
            }
            return this;
        }

        /// <summary>
        /// Set the request method and start processing the request.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> SendAsync(string method)
        {
            _req.Method = new HttpMethod(method);
            return _server.CreateClient().SendAsync(_req);
        }

        /// <summary>
        /// Set the request method to GET and start processing the request.
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetAsync()
        {
            _req.Method = HttpMethod.Get;
            return _server.CreateClient().SendAsync(_req);
        }

        /// <summary>
        /// Set the request method to POST and start processing the request.
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> PostAsync()
        {
            _req.Method = HttpMethod.Post;
            return _server.CreateClient().SendAsync(_req);
        }
    }


    /// <summary>
    /// This adapts HttpRequestMessages to ASP.NET Core requests, dispatches them through the pipeline, and returns the
    /// associated HttpResponseMessage.
    /// </summary>
    public class XClientHandler : HttpMessageHandler
    {
        private readonly IHttpApplication<Context> _application;
        private readonly PathString _pathBase;

        /// <summary>
        /// Create a new handler.
        /// </summary>
        /// <param name="pathBase">The base path.</param>
        /// <param name="application">The <see cref="IHttpApplication{TContext}"/>.</param>
        public XClientHandler(PathString pathBase, IHttpApplication<Context> application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            _application = application;

            // PathString.StartsWithSegments that we use below requires the base path to not end in a slash.
            if (pathBase.HasValue && pathBase.Value.EndsWith("/"))
            {
                pathBase = new PathString(pathBase.Value.Substring(0, pathBase.Value.Length - 1));
            }
            _pathBase = pathBase;
        }

        /// <summary>
        /// This adapts HttpRequestMessages to ASP.NET Core requests, dispatches them through the pipeline, and returns the
        /// associated HttpResponseMessage.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var state = new RequestState(request, _pathBase, _application);
            var requestContent = request.Content ?? new StreamContent(Stream.Null);
            var body = await requestContent.ReadAsStreamAsync();
            if (body.CanSeek)
            {
                // This body may have been consumed before, rewind it.
                body.Seek(0, SeekOrigin.Begin);
            }
            state.Context.HttpContext.Request.Body = body;
            var registration = cancellationToken.Register(state.AbortRequest);

            // Async offload, don't let the test code block the caller.
            var offload = Task.Factory.StartNew(async () =>
            {
                try
                {
                    await _application.ProcessRequestAsync(state.Context);
                    await state.CompleteResponseAsync();
                    state.ServerCleanup(exception: null);
                }
                catch (Exception ex)
                {
                    state.Abort(ex);
                    state.ServerCleanup(ex);
                }
                finally
                {
                    registration.Dispose();
                }
            });

            return await state.ResponseTask.ConfigureAwait(false);
        }

        internal class RequestFeature : IHttpRequestFeature
        {
            public RequestFeature()
            {
                Body = Stream.Null;
                Headers = new HeaderDictionary();
                Method = "GET";
                Path = "";
                PathBase = "";
                Protocol = "HTTP/1.1";
                QueryString = "";
                Scheme = "http";
            }

            public Stream Body { get; set; }

            public IHeaderDictionary Headers { get; set; }

            public string Method { get; set; }

            public string Path { get; set; }

            public string PathBase { get; set; }

            public string Protocol { get; set; }

            public string QueryString { get; set; }

            public string Scheme { get; set; }

            public string RawTarget { get; set; }
        }


        private class RequestState
        {
            private readonly HttpRequestMessage _request;
            private readonly IHttpApplication<Context> _application;
            private TaskCompletionSource<HttpResponseMessage> _responseTcs;
            private ResponseStream _responseStream;
            private ResponseFeature _responseFeature;
            private CancellationTokenSource _requestAbortedSource;
            private bool _pipelineFinished;

            internal RequestState(HttpRequestMessage request, PathString pathBase, IHttpApplication<Context> application)
            {
                _request = request;
                _application = application;
                _responseTcs = new TaskCompletionSource<HttpResponseMessage>();
                _requestAbortedSource = new CancellationTokenSource();
                _pipelineFinished = false;

                if (request.RequestUri.IsDefaultPort)
                {
                    request.Headers.Host = request.RequestUri.Host;
                }
                else
                {
                    request.Headers.Host = request.RequestUri.GetComponents(UriComponents.HostAndPort, UriFormat.UriEscaped);
                }

                var contextFeatures = new FeatureCollection();
                contextFeatures.Set<IHttpRequestFeature>(new RequestFeature());
                _responseFeature = new ResponseFeature();
                contextFeatures.Set<IHttpResponseFeature>(_responseFeature);
                Context = application.CreateContext(contextFeatures);
                var httpContext = Context.HttpContext;

                var serverRequest = httpContext.Request;
                serverRequest.Protocol = "HTTP/" + request.Version.ToString(2);
                serverRequest.Scheme = request.RequestUri.Scheme;
                serverRequest.Method = request.Method.ToString();

                var fullPath = PathString.FromUriComponent(request.RequestUri);
                PathString remainder;
                //if (fullPath.StartsWithSegments(pathBase, out remainder))
                //{
                //    serverRequest.PathBase = pathBase;
                //    serverRequest.Path = remainder;
                //}
                //else
                {
                    serverRequest.PathBase = PathString.Empty;
                    serverRequest.Path = fullPath;
                }

                serverRequest.QueryString = QueryString.FromUriComponent(request.RequestUri);

                foreach (var header in request.Headers)
                {
                    serverRequest.Headers.Append(header.Key, header.Value.ToArray());
                }
                var requestContent = request.Content;
                if (requestContent != null)
                {
                    foreach (var header in request.Content.Headers)
                    {
                        serverRequest.Headers.Append(header.Key, header.Value.ToArray());
                    }
                }

                _responseStream = new ResponseStream(ReturnResponseMessageAsync, AbortRequest);
                httpContext.Response.Body = _responseStream;
                httpContext.Response.StatusCode = 200;
                httpContext.RequestAborted = _requestAbortedSource.Token;
            }

            public Context Context { get; private set; }

            public Task<HttpResponseMessage> ResponseTask
            {
                get { return _responseTcs.Task; }
            }

            internal void AbortRequest()
            {
                if (!_pipelineFinished)
                {
                    _requestAbortedSource.Cancel();
                }
                _responseStream.Complete();
            }

            internal async Task CompleteResponseAsync()
            {
                _pipelineFinished = true;
                await ReturnResponseMessageAsync();
                _responseStream.Complete();
                await _responseFeature.FireOnResponseCompletedAsync();
            }

            internal async Task ReturnResponseMessageAsync()
            {
                // Check if the response has already started because the TrySetResult below could happen a bit late
                // (as it happens on a different thread) by which point the CompleteResponseAsync could run and calls this
                // method again.
                if (!Context.HttpContext.Response.HasStarted)
                {
                    var response = await GenerateResponseAsync();
                    // Dispatch, as TrySetResult will synchronously execute the waiters callback and block our Write.
                    var setResult = Task.Factory.StartNew(() => _responseTcs.TrySetResult(response));
                }
            }

            private async Task<HttpResponseMessage> GenerateResponseAsync()
            {
                await _responseFeature.FireOnSendingHeadersAsync();
                var httpContext = Context.HttpContext;

                var response = new HttpResponseMessage();
                response.StatusCode = (HttpStatusCode)httpContext.Response.StatusCode;
                response.ReasonPhrase = httpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase;
                response.RequestMessage = _request;
                // response.Version = owinResponse.Protocol;

                response.Content = new StreamContent(_responseStream);

                foreach (var header in httpContext.Response.Headers)
                {
                    if (!response.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value))
                    {
                        bool success = response.Content.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value);
                        Contract.Assert(success, "Bad header");
                    }
                }
                return response;
            }

            internal void Abort(Exception exception)
            {
                _pipelineFinished = true;
                _responseStream.Abort(exception);
                _responseTcs.TrySetException(exception);
            }

            internal void ServerCleanup(Exception exception)
            {
                _application.DisposeContext(Context, exception);
            }
        }

        internal class ResponseStream : Stream
        {
            private bool _complete;
            private bool _aborted;
            private Exception _abortException;
            private ConcurrentQueue<byte[]> _bufferedData;
            private ArraySegment<byte> _topBuffer;
            private SemaphoreSlim _readLock;
            private SemaphoreSlim _writeLock;
            private TaskCompletionSource<object> _readWaitingForData;
            private object _signalReadLock;

            private Func<Task> _onFirstWriteAsync;
            private bool _firstWrite;
            private Action _abortRequest;

            internal ResponseStream(Func<Task> onFirstWriteAsync, Action abortRequest)
            {
                if (onFirstWriteAsync == null)
                {
                    throw new ArgumentNullException(nameof(onFirstWriteAsync));
                }

                if (abortRequest == null)
                {
                    throw new ArgumentNullException(nameof(abortRequest));
                }

                _onFirstWriteAsync = onFirstWriteAsync;
                _firstWrite = true;
                _abortRequest = abortRequest;

                _readLock = new SemaphoreSlim(1, 1);
                _writeLock = new SemaphoreSlim(1, 1);
                _bufferedData = new ConcurrentQueue<byte[]>();
                _readWaitingForData = new TaskCompletionSource<object>();
                _signalReadLock = new object();
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            #region NotSupported

            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            #endregion NotSupported

            public override void Flush()
            {
                CheckNotComplete();

                _writeLock.Wait();
                try
                {
                    FirstWriteAsync().GetAwaiter().GetResult();
                }
                finally
                {
                    _writeLock.Release();
                }

                // TODO: Wait for data to drain?
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                    tcs.TrySetCanceled();
                    return tcs.Task;
                }

                Flush();

                // TODO: Wait for data to drain?

                return Task.FromResult<object>(null);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                VerifyBuffer(buffer, offset, count, allowEmpty: false);
                _readLock.Wait();
                try
                {
                    int totalRead = 0;
                    do
                    {
                        // Don't drain buffered data when signaling an abort.
                        CheckAborted();
                        if (_topBuffer.Count <= 0)
                        {
                            byte[] topBuffer = null;
                            while (!_bufferedData.TryDequeue(out topBuffer))
                            {
                                if (_complete)
                                {
                                    CheckAborted();
                                    // Graceful close
                                    return totalRead;
                                }
                                WaitForDataAsync().Wait();
                            }
                            _topBuffer = new ArraySegment<byte>(topBuffer);
                        }
                        int actualCount = Math.Min(count, _topBuffer.Count);
                        Buffer.BlockCopy(_topBuffer.Array, _topBuffer.Offset, buffer, offset, actualCount);
                        _topBuffer = new ArraySegment<byte>(_topBuffer.Array,
                            _topBuffer.Offset + actualCount,
                            _topBuffer.Count - actualCount);
                        totalRead += actualCount;
                        offset += actualCount;
                        count -= actualCount;
                    }
                    while (count > 0 && (_topBuffer.Count > 0 || _bufferedData.Count > 0));
                    // Keep reading while there is more data available and we have more space to put it in.
                    return totalRead;
                }
                finally
                {
                    _readLock.Release();
                }
            }
#if NET451
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            // TODO: This option doesn't preserve the state object.
            // return ReadAsync(buffer, offset, count);
            return base.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            // return ((Task<int>)asyncResult).Result;
            return base.EndRead(asyncResult);
        }
#endif
            public async override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                VerifyBuffer(buffer, offset, count, allowEmpty: false);
                CancellationTokenRegistration registration = cancellationToken.Register(Abort);
                await _readLock.WaitAsync(cancellationToken);
                try
                {
                    int totalRead = 0;
                    do
                    {
                        // Don't drained buffered data on abort.
                        CheckAborted();
                        if (_topBuffer.Count <= 0)
                        {
                            byte[] topBuffer = null;
                            while (!_bufferedData.TryDequeue(out topBuffer))
                            {
                                if (_complete)
                                {
                                    CheckAborted();
                                    // Graceful close
                                    return totalRead;
                                }
                                await WaitForDataAsync();
                            }
                            _topBuffer = new ArraySegment<byte>(topBuffer);
                        }
                        int actualCount = Math.Min(count, _topBuffer.Count);
                        Buffer.BlockCopy(_topBuffer.Array, _topBuffer.Offset, buffer, offset, actualCount);
                        _topBuffer = new ArraySegment<byte>(_topBuffer.Array,
                            _topBuffer.Offset + actualCount,
                            _topBuffer.Count - actualCount);
                        totalRead += actualCount;
                        offset += actualCount;
                        count -= actualCount;
                    }
                    while (count > 0 && (_topBuffer.Count > 0 || _bufferedData.Count > 0));
                    // Keep reading while there is more data available and we have more space to put it in.
                    return totalRead;
                }
                finally
                {
                    registration.Dispose();
                    _readLock.Release();
                }
            }

            // Called under write-lock.
            private Task FirstWriteAsync()
            {
                if (_firstWrite)
                {
                    _firstWrite = false;
                    return _onFirstWriteAsync();
                }
                return Task.FromResult(true);
            }

            // Write with count 0 will still trigger OnFirstWrite
            public override void Write(byte[] buffer, int offset, int count)
            {
                VerifyBuffer(buffer, offset, count, allowEmpty: true);
                CheckNotComplete();

                _writeLock.Wait();
                try
                {
                    FirstWriteAsync().GetAwaiter().GetResult();
                    if (count == 0)
                    {
                        return;
                    }
                    // Copies are necessary because we don't know what the caller is going to do with the buffer afterwards.
                    byte[] internalBuffer = new byte[count];
                    Buffer.BlockCopy(buffer, offset, internalBuffer, 0, count);
                    _bufferedData.Enqueue(internalBuffer);

                    SignalDataAvailable();
                }
                finally
                {
                    _writeLock.Release();
                }
            }
#if NET451
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            Write(buffer, offset, count);
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(state);
            tcs.TrySetResult(null);
            IAsyncResult result = tcs.Task;
            if (callback != null)
            {
                callback(result);
            }
            return result;
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
        }
#endif
            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                VerifyBuffer(buffer, offset, count, allowEmpty: true);
                if (cancellationToken.IsCancellationRequested)
                {
                    TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                    tcs.TrySetCanceled();
                    return tcs.Task;
                }

                Write(buffer, offset, count);
                return Task.FromResult<object>(null);
            }

            private static void VerifyBuffer(byte[] buffer, int offset, int count, bool allowEmpty)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                if (offset < 0 || offset > buffer.Length)
                {
                    throw new ArgumentOutOfRangeException("offset", offset, string.Empty);
                }
                if (count < 0 || count > buffer.Length - offset
                    || (!allowEmpty && count == 0))
                {
                    throw new ArgumentOutOfRangeException("count", count, string.Empty);
                }
            }

            private void SignalDataAvailable()
            {
                // Dispatch, as TrySetResult will synchronously execute the waiters callback and block our Write.
                Task.Factory.StartNew(() => _readWaitingForData.TrySetResult(null));
            }

            private Task WaitForDataAsync()
            {
                // Prevent race with Dispose
                lock (_signalReadLock)
                {
                    _readWaitingForData = new TaskCompletionSource<object>();

                    if (!_bufferedData.IsEmpty || _complete)
                    {
                        // Race, data could have arrived before we created the TCS.
                        _readWaitingForData.TrySetResult(null);
                    }

                    return _readWaitingForData.Task;
                }
            }

            internal void Abort()
            {
                Abort(new OperationCanceledException());
            }

            internal void Abort(Exception innerException)
            {
                Contract.Requires(innerException != null);
                _aborted = true;
                _abortException = innerException;
                Complete();
            }

            internal void Complete()
            {
                // If HttpClient.Dispose gets called while HttpClient.SetTask...() is called
                // there is a chance that this method will be called twice and hang on the lock
                // to prevent this we can check if there is already a thread inside the lock
                if (_complete)
                {
                    return;
                }

                // Prevent race with WaitForDataAsync
                lock (_signalReadLock)
                {
                    // Throw for further writes, but not reads.  Allow reads to drain the buffered data and then return 0 for further reads.
                    _complete = true;
                    _readWaitingForData.TrySetResult(null);
                }
            }

            private void CheckAborted()
            {
                if (_aborted)
                {
                    throw new IOException(string.Empty, _abortException);
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _abortRequest();
                }
                base.Dispose(disposing);
            }

            private void CheckNotComplete()
            {
                if (_complete)
                {
                    throw new IOException("The request was aborted or the pipeline has finished");
                }
            }
        }

        internal class ResponseFeature : IHttpResponseFeature
        {
            private Func<Task> _responseStartingAsync = () => Task.FromResult(true);
            private Func<Task> _responseCompletedAsync = () => Task.FromResult(true);

            public ResponseFeature()
            {
                Headers = new HeaderDictionary();
                Body = new MemoryStream();

                // 200 is the default status code all the way down to the host, so we set it
                // here to be consistent with the rest of the hosts when writing tests.
                StatusCode = 200;
            }

            public int StatusCode { get; set; }

            public string ReasonPhrase { get; set; }

            public IHeaderDictionary Headers { get; set; }

            public Stream Body { get; set; }

            public bool HasStarted { get; set; }

            public void OnStarting(Func<object, Task> callback, object state)
            {
                var prior = _responseStartingAsync;
                _responseStartingAsync = async () =>
                {
                    await callback(state);
                    await prior();
                };
            }

            public void OnCompleted(Func<object, Task> callback, object state)
            {
                var prior = _responseCompletedAsync;
                _responseCompletedAsync = async () =>
                {
                    try
                    {
                        await callback(state);
                    }
                    finally
                    {
                        await prior();
                    }
                };
            }

            public async Task FireOnSendingHeadersAsync()
            {
                await _responseStartingAsync();
                HasStarted = true;
            }

            public Task FireOnResponseCompletedAsync()
            {
                return _responseCompletedAsync();
            }
        }

    }
}

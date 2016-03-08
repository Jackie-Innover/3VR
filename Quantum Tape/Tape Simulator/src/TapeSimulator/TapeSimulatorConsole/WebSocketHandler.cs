using System;
using System.Threading;
using BackupLib;
using ThreeVR.Common;

namespace TapeSimulatorConsole
{
    public class WebSocketHandler
    {
        public static object SendRequestReturnResult(WebSocketClient handler, WebSocketRequest request)
        {
            var syncRequest = new WebSocketSyncRequest(request);
            WebSocketNotificationHandler.InitWaitEvent(syncRequest);
            object result = null;
            if (handler.SendRequest(request))
            {
                result = WebSocketNotificationHandler.WaitResponse(syncRequest);
            }
            return result;
        }

        /// <summary>
        /// Client code don't use it if AsyncWebSocketRequests.Instance.Start() is not called which will use a bunch of threads to
        /// send websocket requests asynchronously
        /// </summary>
        /// <param name="webSocketRequest"></param>
        /// <returns></returns>
        public object AsyncSendRequest(WebSocketRequest webSocketRequest)
        {
            var syncRequest = new WebSocketSyncRequest(webSocketRequest);
            WebSocketNotificationHandler.InitWaitEvent(syncRequest);
            AsyncWebSocketRequests.Instance.Add(syncRequest.Request);
            return WebSocketNotificationHandler.WaitResponse(syncRequest);
        }

        public bool PutFile(string fileName)
        {
            // Now we tranfer file to ESS Server by block. To reduce the time to write the file in ESS Server,
            // We keep file open if current block is not a file tail. It can reduce around half of the time during my UT.
            // When there are multiple child ESS'es, we should make sure that multiple file block requests should be sent to
            // same child ESS, so we have to first send the put file request to one websocket connection, and in that connection,
            // we send multiple block file bytes to same child ESS server.
            // PutFileProxyRequest is the request used to send put file request to one websocket connection,
            // and PutFileRequest is the request used to send multiple block file bytest to same child ESS Server
            var putFileProxyRequest = new PutFileProxyRequest
            {
                RequestId = RequestHelper.NextRequestId(),
                FilePath = fileName
            };
            if (!(bool)AsyncSendRequest(putFileProxyRequest))
            {
                return false;
            }
            return true;
        }
    }

    public class WebSocketSyncRequest
    {
        public WebSocketSyncRequest(WebSocketRequest request)
        {
            Request = request;
            WaitEvent = new ManualResetEvent(false);
        }

        public WebSocketRequest Request { get; set; }
        public ManualResetEvent WaitEvent { get; set; }
    }

    public static class WebSocketNotificationHandler
    {
        // Store the request and corresponding ManualResetEvent into dictionary
        private static readonly ExpiringHashtable RequestsOnNotification = new ExpiringHashtable(600, "RequestsOnNotification");

        private static readonly ExpiringHashtable Responses = new ExpiringHashtable(600, "Responses");
        private static readonly TimeSpan NotifyTimeout = TimeSpan.FromSeconds(20);

        /// <summary>
        /// Add syncRequest into the notification dictionary
        /// </summary>
        /// <param name="syncRequest"></param>
        public static void InitWaitEvent(WebSocketSyncRequest syncRequest)
        {
            RequestsOnNotification.Set(syncRequest.Request.RequestId, syncRequest.WaitEvent);
        }

        /// <summary>
        /// Wait the response for syncRequest
        /// </summary>
        /// <param name="syncRequest"></param>
        /// <returns></returns>
        public static object WaitResponse(WebSocketSyncRequest syncRequest)
        {
            TimeSpan timeoutSpan = NotifyTimeout;
            //For PutFileProxyRequest, because it's an internal request which is only processed in ESS client
            // to make sure the block of file bytes will be sent to same child ESS. If one video file is a little
            // huge, it may spend more than 1min(default), so we set the timeout time to be 30min which should be
            // enough to send a video file.
            if (syncRequest.Request.RequestType == WebSocketRequestType.PutFileProxyRequest)
            {
                timeoutSpan = TimeSpan.FromMinutes(30);
            }
            if (!syncRequest.WaitEvent.WaitOne(timeoutSpan))
            {
                RequestsOnNotification.Remove(syncRequest.Request.RequestId);
                Console.WriteLine("Timeout for this WebSocket request " + syncRequest.Request);
                return false;
            }
            object status;
            if ((status = Responses.Get(syncRequest.Request.RequestId)) != null)
            {
                Responses.Remove(syncRequest.Request.RequestId);
            }
            return status;
        }

        /// <summary>
        /// Set the ManualResetEvent to unblock WaitResponse()
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="result"></param>
        public static void ReceiveResponse(long requestId, object result)
        {
            ManualResetEvent waitEvent;
            if ((waitEvent = (ManualResetEvent)RequestsOnNotification.Get(requestId)) != null)
            {
                RequestsOnNotification.Remove(requestId);
                Responses.Set(requestId, result);
                waitEvent.Set();
            }
        }
    }
}
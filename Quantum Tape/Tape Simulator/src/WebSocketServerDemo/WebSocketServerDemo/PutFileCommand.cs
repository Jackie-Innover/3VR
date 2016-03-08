using System;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace WebSocketServerDemo
{
    class PutFileCommand : CommandBase<AppSession, StringRequestInfo>
    {
        public override void ExecuteCommand(AppSession session, StringRequestInfo requestInfo)
        {
            Console.WriteLine(requestInfo.Key);
        }
    }
}

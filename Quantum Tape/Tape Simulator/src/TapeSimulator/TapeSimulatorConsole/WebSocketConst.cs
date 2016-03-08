namespace TapeSimulatorConsole
{
    public static class WebSocketConst
    {
        /// <summary>
        /// The user name key
        /// </summary>
        public const string UserNameKey = "userName";

        /// <summary>
        /// The password key
        /// </summary>
        public const string PasswordKey = "userPassword";

        /// <summary>
        /// The appliance unique identifier key
        /// </summary>
        public const string ClientGuidKey = "applianceGuid";

        /// <summary>
        /// The host name
        /// </summary>
        public const string ClientHostNameKey = "hostName";

        /// <summary>
        /// The test client display name
        /// </summary>
        public const string TestClientDisplayName = "TestClientForTestBackupDestinationSettings";

        /// <summary>
        /// The version of websocket Extended Storage client
        /// Following is the feature for each version:
        /// - 1: The file block sent to server or received from server are encoded by base64 via Json. It cause high CPU.
        ///      Different file blocks for same file maybe sent to different websocket connections.
        /// - 2: The file block sent to server or received from server are directly bytes.
        ///      Different file blocks for same file maybe sent to same websocket connection.
        /// </summary>
        public const string WebSocketClientVersion = "WebSocketClientVersion";
    }
}
namespace Cabby.Client.Shared.Contracts.Network
{
    public static class WebSocketMessageTypes
    {
        public static class Inbound
        {
            public const string StartSession = "start_session";
            public const string StopSession = "stop_session";
            public const string StartProcess = "start_process";
            public const string StopProcess = "stop_process";
        }

        public static class Outbound
        {
            public const string UnknownMessageType = "unknown_message_type";
            public const string SessionStatusUpdate = "session_status_update";
            public const string SessionFailed = "session_failed";
            public const string ReadyForSession = "ready_for_session";
            public const string CannotStartMultipleSessions = "cannot_start_multiple_sessions";
            public const string ProcessStatusUpdate = "process_status_update";
            public const string NoSessionToStop = "no_session_to_stop";
            public const string StopSessionIdMismatch = "stop_session_id_mismatch";
        }
    }
}
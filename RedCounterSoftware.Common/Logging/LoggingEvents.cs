namespace RedCounterSoftware.Common.Logging
{
    using Microsoft.Extensions.Logging;

    public static class LoggingEvents
    {
        public static EventId HttpRequest => new EventId(1, "HttpRequest");

        public static EventId Trace => new EventId(100, "Trace");

        public static EventId Authentication => new EventId(200, "Authentication");

        public static EventId AuthenticationOk => new EventId(201, "Authentication-Ok");

        public static EventId AuthenticationFail => new EventId(202, "Authentication-Fail");

        public static EventId ChangePassword => new EventId(203, "ChangePassword");

        public static EventId ChangePasswordOk => new EventId(204, "ChangePassword-Ok");

        public static EventId ChangePasswordFail => new EventId(205, "ChangePassword-Fail");

        public static EventId Impersonation => new EventId(206, "Impersonation");

        public static EventId ImpersonationStart => new EventId(207, "Impersonation-Start");

        public static EventId ImpersonationEnd => new EventId(208, "Impersonation-End");

        public static EventId ImpersonationFail => new EventId(209, "Impersonation-Fail");

        public static EventId ResetPassword => new EventId(210, "ResetPassword");

        public static EventId UserConnected => new EventId(211, "UserConnected");

        public static EventId UserDisconnected => new EventId(212, "UserDisconnected");

        public static EventId Activation => new EventId(300, "Activation");

        public static EventId ActivationOk => new EventId(301, "Activation-Ok");

        public static EventId ActivationFail => new EventId(302, "Activation-Fail");

        public static EventId Crud => new EventId(400, "CRUD");

        public static EventId CrudSearch => new EventId(401, "CRUD-Search");

        public static EventId CrudGet => new EventId(402, "CRUD-Get");

        public static EventId CrudAdd => new EventId(403, "CRUD-Add");

        public static EventId CrudPatch => new EventId(404, "CRUD-Patch");

        public static EventId CrudDelete => new EventId(405, "CRUD-Delete");

        public static EventId CrudNotFound => new EventId(406, "CRUD-NotFound");

        public static EventId CrudAlreadyExists => new EventId(407, "CRUD-AlreadyExists");

        public static EventId Validation => new EventId(500, "Validation");

        public static EventId ValidationOk => new EventId(501, "Validation-Ok");

        public static EventId ValidationFail => new EventId(502, "Validation-Fail");

        public static EventId ExternalService => new EventId(600, "ExternalService");

        public static EventId ExternalServiceOk => new EventId(601, "ExternalService-Ok");

        public static EventId ExternalServiceError => new EventId(602, "ExternalService-Error");

        public static EventId Exception => new EventId(700, "Exception");
    }
}

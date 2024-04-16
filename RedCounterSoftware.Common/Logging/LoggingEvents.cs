namespace RedCounterSoftware.Common.Logging
{
    using Microsoft.Extensions.Logging;

    public static class LoggingEvents
    {
        public static EventId HttpRequest => new(1, "HttpRequest");

        public static EventId Trace => new(100, "Trace");

        public static EventId Authentication => new(200, "Authentication");

        public static EventId AuthenticationOk => new(201, "Authentication-Ok");

        public static EventId AuthenticationFail => new(202, "Authentication-Fail");

        public static EventId ChangePassword => new(203, "ChangePassword");

        public static EventId ChangePasswordOk => new(204, "ChangePassword-Ok");

        public static EventId ChangePasswordFail => new(205, "ChangePassword-Fail");

        public static EventId Impersonation => new(206, "Impersonation");

        public static EventId ImpersonationStart => new(207, "Impersonation-Start");

        public static EventId ImpersonationEnd => new(208, "Impersonation-End");

        public static EventId ImpersonationFail => new(209, "Impersonation-Fail");

        public static EventId ResetPassword => new(210, "ResetPassword");

        public static EventId UserConnected => new(211, "UserConnected");

        public static EventId UserDisconnected => new(212, "UserDisconnected");

        public static EventId Activation => new(300, "Activation");

        public static EventId ActivationOk => new(301, "Activation-Ok");

        public static EventId ActivationFail => new(302, "Activation-Fail");

        public static EventId Crud => new(400, "CRUD");

        public static EventId CrudSearch => new(401, "CRUD-Search");

        public static EventId CrudGet => new(402, "CRUD-Get");

        public static EventId CrudAdd => new(403, "CRUD-Add");

        public static EventId CrudPatch => new(404, "CRUD-Patch");

        public static EventId CrudDelete => new(405, "CRUD-Delete");

        public static EventId CrudNotFound => new(406, "CRUD-NotFound");

        public static EventId CrudAlreadyExists => new(407, "CRUD-AlreadyExists");

        public static EventId Validation => new(500, "Validation");

        public static EventId ValidationOk => new(501, "Validation-Ok");

        public static EventId ValidationFail => new(502, "Validation-Fail");

        public static EventId ExternalService => new(600, "ExternalService");

        public static EventId ExternalServiceOk => new(601, "ExternalService-Ok");

        public static EventId ExternalServiceError => new(602, "ExternalService-Error");

        public static EventId Exception => new(700, "Exception");
    }
}

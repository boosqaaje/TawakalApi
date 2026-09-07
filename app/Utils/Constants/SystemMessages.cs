namespace TawakalApi.app.Utils.Constants;


public static class SystemMessages
{

    public const string BodyNull = "Request body cannot be null.";
    public const string UserID = "user_id";
    public const string UserRole = "user_role";

    public static class Partner
    {
        public const string PartnerUsernameMissing = "Missing partner username";
        public const string PartnerCreated = "Partner successfully created. Save this secret now; it will not be shown again.";
        public const string PartnerIdRequired = "Partner ID is required";
        public const string PartnerNotFound = "Partner not found.";
    }
    public static class Auth
    {
        public const string InvalidCredentials = "Invalid credentials.";
        public const string EmailExist = "User with this email is already exist.";
        public const string FirstNameRequired = "First name is required.";
        public const string LastNameRequired = "Last name is required.";
        public const string EmailRequired = "Email is required.";
        public const string UserNameRequired = "Username is required.";
        public const string PasswordRequired = "Pasword is required.";
        public const string RoleRequired = "Role is required.";
        public const string UserRegistered = "User registered successfully.";
        public const string UserNameExist = "Username already exists.";
        public const string LoginSuccess = "Login successful.";
        public const string AccessDeniedMsg = "Access denied.";
        public const string PasswordChanged = "Password successfully changed";
        public const string InvalidGrantType = "The grant_type must be 'client_credentials'.";
        public const string InvalidClientSecretMsg = "Client authentication failed. Check your client_id and client_secret.";
    }

    public static class Tran
    {
        public const string TranRefNumberRequired = "Transaction reference is required.";
        public const string CurrencyRequired = "Currency is required.";
        public const string ServiceRequired = "Service code is required.";
        public const string AmountRequired = "Amount should be greater than zero.";
        public const string TranCancelled = "Transaction cancelled successfully.";
        public const string TranNotFound = "Transaction not found or access denied.";
        public const string TranListNotFound = "No transactions were found.";
        public const string NonCancellable = "Cannot cancel a transaction that is already completed.";
        public const string TranAlreadyCancelled = "Transaction is already cancelled.";
        public const string TranFailed = "Transaction failed.";
        public const string TranStatusRetreived = "Transaction status retrieved successfully.";
        public const string TranRetreived = "Transactions retrieved successfully.";

        public static class Status
        {
            public const string Cancelled = "CANCELLED";
        }
    }
}
namespace TawakalApi.app.Utils.Constants;

public static class SystemCodes
{
    // General Errors
    public const int Success = 100;
    public const int GeneralError = 101;
    public const int RequiredField = 102;
    public const int BodyNull = 103;
    public const int Unauthorized = 401;
    public const int NotFound = 404;

    public static class Partner
    {
        public const int PartnerUsernameMissing = 200;
    }

    // Authentication & Portal User Errors
    public static class Auth
    {
        public const int EmailExist = 300;
        public const int InvalidCredentials = 301;
        public const int AccessDenied = 302;
        public const int UserNameExist = 303;
    }

    // Transaction Errors
    public static class Tran
    {
        public const int DuplicateTransaction = 901;
        public const int TrnNotFound = 902;
        public const int NonCancellable = 903;
        public const int TransactionFailed = 904;
    }
}
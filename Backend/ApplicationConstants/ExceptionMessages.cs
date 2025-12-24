namespace AtmApplication.Backend.ApplicationConstants
{
    public static class ExceptionMessages
    {
        public const string AccountFrozen = "Account is frozen due to multiple failed attempts.";
        public const string InsufficientFunds = "Insufficient funds in account.";
        public const string InvalidCredentials = "Invalid username or PIN.";
        public const string UsernameTaken = "Username is already taken.";
        public const string ExceededPinAttempts = "Maximum PIN attempts exceeded. Account has been frozen.";
        public const string AccountNotFound = "Account not found.";
        public const string UserNotFound = "User not found.";
        public const string AtmNotFound = "ATM details not found.";
        public const string TransactionFailed = "Transaction failed to complete.";
        public const string UnauthorizedAccess = "Only administrators can perform this operation.";
        public const string InvalidOperation = "The requested operation is invalid.";
        public const string AdminFreezeAttempt = "Cannot freeze admin account. Admin accounts cannot be frozen.";
        public const string AccountNotFrozen = "Account is not frozen.";
    }
}

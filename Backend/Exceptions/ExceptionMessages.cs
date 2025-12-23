namespace Backend.Exceptions
{
    /// <summary>
    /// Contains all exception messages used throughout the Backend layer.
    /// Centralizes exception messages for consistency and maintainability.
    /// </summary>
    public static class ExceptionMessages
    {
        // Custom Business Exceptions
        public const string AccountFrozen = "Account is frozen due to multiple failed attempts.";
        public const string InsufficientFunds = "Insufficient funds in account or ATM.";
        public const string InvalidCredentials = "Invalid username or PIN.";
        public const string UsernameTaken = "Username is already taken.";
        public const string ExceededPinAttempts = "Maximum PIN attempts exceeded. Account has been frozen.";

        // System/Infrastructure Exceptions
        public const string AccountNotFound = "Account not found.";
        public const string UserNotFound = "User not found.";
        public const string AtmNotFound = "ATM details not found.";
        public const string TransactionFailed = "Transaction failed to complete.";
        public const string UnauthorizedAccess = "Only administrators can perform this operation.";
        public const string InvalidOperation = "The requested operation is invalid.";
    }
}

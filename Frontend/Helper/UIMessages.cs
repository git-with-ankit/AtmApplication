namespace Frontend.Helper
{
    public static class UIMessages
    {
        public const string WelcomeMessage = "   Welcome to ATM Console Application\n";
        public const string WelcomeUser = "\n--- User Portal ---";
        public const string WelcomeAdmin = "\n--- Admin Portal ---";
        public const string WelcomeSignup = "\n--- User Registration ---";
        public const string WelcomeLogin = "\n--- User Login ---";

        public const string SelectRole = "\nSelect your role:\n1. User\n2. Admin";
        public const string UserMenu = "\n1. Sign Up\n2. Login\n3. Go Back";
        public const string AdminMenu = "\n1. Login\n2. Go Back";
        public const string UserActionMenu = "\n1. Deposit\n2. Withdraw\n3. View Balance\n4. Change PIN\n5. View Transaction History\n6. Sign Out";
        public const string AdminActionMenu = "\n1. View Frozen Accounts\n2. Unfreeze Account\n3. View ATM Balance\n4. Deposit to ATM\n5. View ATM Transactions\n6. Change Admin\n7. Exit";

        public const string EnterUsername = "Enter username:";
        public const string EnterPin = "Enter PIN (4 digits):";
        public const string EnterCurrentPin = "Enter current PIN:";
        public const string EnterNewPin = "Enter new PIN (4 digits):";
        public const string EnterAmount = "Enter amount:";
        public const string EnterChoice = "Enter your choice:";

        public const string SignupSuccess = "Account created successfully!";
        public const string LoginSuccess = "Login successful! Welcome, {0}";
        public const string DepositSuccess = "Amount ${0} deposited successfully!";
        public const string WithdrawSuccess = "Amount ${0} withdrawn successfully!";
        public const string PinChangeSuccess = "PIN changed successfully!";
        public const string UnfreezeSuccess = "Account unfrozen successfully!";
        public const string AtmDepositSuccess = "Amount ${0} deposited to ATM successfully!";
        public const string ChangeAdminSuccess = "Admin privileges transferred successfully!";

        public const string InvalidChoice = "Invalid choice. Please try again.";
        public const string InvalidInput = "Invalid input. Please enter a valid number.";
        public const string InvalidUsername = "Username must be 3-20 characters and contain only letters, numbers, and underscores.";
        public const string InvalidPin = "PIN must be exactly 4 digits.";
        public const string InvalidAmount = "Amount must be a positive number.";
        public const string UsernameNotFound = "Username not found.";
        public const string AccountFrozen = "Your account is frozen.";
        public const string ContactAdmin = "Please contact administrator for assistance.";
        public const string PinAttemptsExceeded = "Maximum PIN attempts exceeded. Account frozen.";
        public const string PinMismatch = "Incorrect PIN. {0} attempt(s) remaining.";
        public const string NoTransactions = "No transactions found.";
        public const string NoFrozenAccounts = "No frozen accounts found.";

        public const string SigningOut = "Signing out...";
        public const string Exiting = "Exiting...";
        public const string PressEnterToContinue = "\nPress Enter to continue...";
    }
}

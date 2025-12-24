namespace Frontend.Model
{
    public enum RoleOption
    {
        User = 1,
        Admin = 2
    }

    public enum UserMenuOption
    {
        SignUp = 1,
        Login = 2,
        GoBack = 3
    }

    public enum UserActionOption
    {
        Deposit = 1,
        Withdraw = 2,
        ViewBalance = 3,
        ChangePin = 4,
        ViewTransactions = 5,
        SignOut = 6
    }

    public enum AdminActionOption
    {
        ViewFrozenAccounts = 1,
        UnfreezeAccount = 2,
        ViewAtmBalance = 3,
        DepositToAtm = 4,
        ViewAtmTransactions = 5,
        ChangeAdmin = 6,
        Exit = 7
    }
}

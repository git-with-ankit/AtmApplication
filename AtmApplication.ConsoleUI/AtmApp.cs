using AtmApplication.Backend.Services;
using AtmApplication.DataAccess.FileRepository;
using AtmApplication.DataAccess.Interfaces;
using AtmApplication.ConsoleUI.Menus;
namespace AtmApplication.ConsoleUI.Dependencies
{
    internal sealed class AtmApp
    {
        public IUserRepository UserRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IAtmRepository AtmRepository { get; }
        public ITransactionRepository TransactionRepository { get; }
        public IIdentityService IdentityService { get; }
        public ITransactionService TransactionService { get; }
        public IValidationService ValidationService { get; }
        public UserMenu UserMenu { get; }
        public AdminMenu AdminMenu { get; }
        public MainMenu MainMenu { get; }

        public AtmApp()
        {
            UserRepository = new FileUserRepository();
            AccountRepository = new FileAccountsRepository();
            AtmRepository = new FileAtmRepository();
            TransactionRepository = new FileTransactionRepository();

            ValidationService = new ValidationService(UserRepository, AccountRepository);
            IdentityService = new IdentityService(ValidationService, UserRepository, AccountRepository, AtmRepository);
            TransactionService = new TransactionService(ValidationService, AccountRepository, AtmRepository, TransactionRepository);

            UserMenu = new UserMenu(IdentityService, TransactionService, ValidationService);
            AdminMenu = new AdminMenu(IdentityService, TransactionService, ValidationService);
            MainMenu = new MainMenu(UserMenu, AdminMenu);
        }   
    }
}

using Backend.Services;
using DataAccess;
using DataAccess.Entities;
using DataAccess.FileRepository;
using Frontend.UserInterface;

namespace Frontend.Dependencies
{
    public class AtmApp
    {
        public IRepository<UserDetails> UserRepository { get; }
        public IRepository<AccountDetails> AccountRepository { get; }
        public IRepository<AtmDetails> AtmRepository { get; }
        public IRepository<TransactionDetails> TransactionRepository { get; }
        public IIdentityService IdentityService { get; }
        public ITransactionService TransactionService { get; }
        public IValidationService ValidationService { get; }
        public ConsoleUI ConsoleUI { get; }
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

            ConsoleUI = new ConsoleUI();
            UserMenu = new UserMenu(ConsoleUI, IdentityService, TransactionService);
            AdminMenu = new AdminMenu(ConsoleUI, IdentityService, TransactionService);
            MainMenu = new MainMenu(ConsoleUI, UserMenu, AdminMenu);
        }   
    }
}

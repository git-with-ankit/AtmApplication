using Backend.Services;
using DataAccess;
using DataAccess.Entities;
using DataAccess.FileRepository;
using Frontend.UserInterface;

namespace Frontend.Dependencies
{
    public class SharedFlow
    {
        public IRepository<UserDetails> UserRepository { get; }
        public IRepository<AccountDetails> AccountRepository { get; }
        public IRepository<AtmDetails> AtmRepository { get; }
        public IRepository<TransactionDetails> TransactionRepository { get; }
        public IIdentityService IdentityService { get; }
        public ITransactionService TransactionService { get; }
        public ConsoleUI ConsoleUI { get; }
        public UserMenu UserMenu { get; }
        public AdminMenu AdminMenu { get; }
        public MainMenu MainMenu { get; }

        public SharedFlow()
        {
            // Initialize repositories
            UserRepository = new FileUserRepository();
            AccountRepository = new FileAccountsRepository();
            AtmRepository = new FileAtmRepository();
            TransactionRepository = new FileTransactionRepository();

            // Initialize ValidationService
            var validationService = new ValidationService(UserRepository, AccountRepository);

            // Initialize services with ValidationService
            IdentityService = new IdentityService(validationService, UserRepository, AccountRepository);
            TransactionService = new TransactionService(validationService, AccountRepository, AtmRepository, TransactionRepository);

            // Initialize UI components
            ConsoleUI = new ConsoleUI();
            UserMenu = new UserMenu(ConsoleUI, IdentityService, TransactionService);
            AdminMenu = new AdminMenu(ConsoleUI, IdentityService, TransactionService);
            MainMenu = new MainMenu(ConsoleUI, UserMenu, AdminMenu);
        }
    }
}

using System;
using System.Threading.Tasks;
using AtmApplication.Backend.ApplicationConstants;
using AtmApplication.Backend.DTOs;
using AtmApplication.Backend.Exceptions;
using AtmApplication.Backend.Services;
using AtmApplication.DataAccess.Entities;
using AtmApplication.ConsoleUI.Model;
using AtmApplication.ConsoleUI.Menus;
using AtmApplication.ConsoleUI.Helper;
using AtmApplication.ConsoleUI.ApplicationConstants;

namespace AtmApplication.ConsoleUI.Menus
{
    internal class UserMenu
    {
        private readonly IIdentityService _identityService;
        private readonly ITransactionService _transactionService;
        private readonly IValidationService _validationService;

        public UserMenu(
            IIdentityService identityService, ITransactionService transactionService, IValidationService validationService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        public async Task HandleUserMenuAsync()
        {
            DisplayHelper.DisplayMessage(UIMessages.WelcomeUser);
            DisplayHelper.DisplayMessage(UIMessages.UserMenu);
            DisplayHelper.DisplayPrompt(UIMessages.EnterChoice + " ");

            var option = InputHelper.GetEnumInput<UserMenuOption>();

            switch (option)
            {
                case UserMenuOption.SignUp:
                    await HandleSignupAsync();
                    break;
                case UserMenuOption.Login:
                    await HandleLoginAsync();
                    break;
                case UserMenuOption.GoBack:
                    break;
            }
        }

        private async Task HandleSignupAsync()
        {
            try
            {
                DisplayHelper.DisplayMessage(UIMessages.WelcomeSignup);
                
                string username = InputHelper.GetUsernameInput(_validationService);
                int pin = InputHelper.GetPinInput();

                var signupDto = new SignupDto
                {
                    Username = username,
                    Pin = pin,
                    IsAdmin = false
                };

                var result = await _identityService.SignupAsync(signupDto);
                DisplayHelper.DisplaySuccess(UIMessages.SignupSuccess);
                
                await HandleUserActionsAsync(username, pin);
            }
            catch (UsernameTakenException ex)
            {
                DisplayHelper.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Signup failed: {ex.Message}");
            }
        }

        private async Task HandleLoginAsync()
        {
            try
            {
                DisplayHelper.DisplayMessage(UIMessages.WelcomeLogin);
                
                string username = InputHelper.GetUsernameInput(_validationService);
                int pin = InputHelper.GetPinInput();

                var loginDto = new LoginDto
                {
                    Username = username,
                    Pin = pin
                };

                var result = await _identityService.LoginAsync(loginDto);

                if (!result.IsLoginSuccessful)
                {
                    if (result.IsFrozen)
                    {
                        DisplayHelper.DisplayError(UIMessages.AccountFrozen);
                        DisplayHelper.DisplayMessage(UIMessages.ContactAdmin);
                    }
                    else
                    {
                        DisplayHelper.DisplayError(ExceptionMessages.InvalidCredentials);
                    }
                    return;
                }

                DisplayHelper.DisplaySuccess(string.Format(UIMessages.LoginSuccess, username));
                await HandleUserActionsAsync(username, pin);
            }
            catch (AccountFrozenException ex)
            {
                DisplayHelper.DisplayError(ex.Message);
                DisplayHelper.DisplayMessage(UIMessages.ContactAdmin);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Login failed: {ex.Message}");
            }
        }

        private async Task HandleUserActionsAsync(string username, int userPin)
        {
            while (true)
            {
                DisplayHelper.DisplayMessage(UIMessages.UserActionMenu);
                DisplayHelper.DisplayPrompt(UIMessages.EnterChoice + " ");

                var action = InputHelper.GetEnumInput<UserActionOption>();

                bool shouldExit = false;

                switch (action)
                {
                    case UserActionOption.Deposit:
                        shouldExit = !await HandleDepositAsync(username, userPin);
                        break;
                    case UserActionOption.Withdraw:
                        shouldExit = !await HandleWithdrawAsync(username, userPin);
                        break;
                    case UserActionOption.ViewBalance:
                        shouldExit = !await HandleViewBalanceAsync(username, userPin);
                        break;
                    case UserActionOption.ChangePin:
                        shouldExit = !await HandleChangePinAsync(username, userPin);
                        break;
                    case UserActionOption.ViewTransactions:
                        await HandleViewTransactionsAsync(username);
                        break;
                    case UserActionOption.SignOut:
                        DisplayHelper.DisplayMessage(UIMessages.SigningOut);
                        return;
                }

                if (shouldExit) return;
            }
        }

        private async Task<bool> HandleDepositAsync(string username, int userPin)
        {
            try
            {
                double amount = InputHelper.GetAmountInput();
                DisplayHelper.DisplayMessage("\nPlease enter your PIN to confirm:");
                int enteredPin = InputHelper.GetPinInput();

                var transactionDto = new TransactionDto
                {
                    Username = username,
                    Type = TransactionType.Credit,
                    Amount = amount
                };

                var result = await _transactionService.DepositAsync(transactionDto, enteredPin);
                DisplayHelper.DisplaySuccess(string.Format(UIMessages.DepositSuccess, amount.ToString("F2")));
                DisplayHelper.DisplayInfo($"New Balance: ${result.NewBalance:F2}");
                
                return true;
            }
            catch (AccountFrozenException)
            {
                DisplayHelper.DisplayError(UIMessages.AccountFrozen);
                DisplayHelper.DisplayMessage(UIMessages.ContactAdmin);
                return false; 
            }
            catch (ExceededPinAttemptsException)
            {
                DisplayHelper.DisplayError(UIMessages.PinAttemptsExceeded);
                DisplayHelper.DisplayError(UIMessages.AccountFrozen);
                DisplayHelper.DisplayMessage(UIMessages.ContactAdmin);
                return false; 
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Deposit failed: {ex.Message}");
                return true;
            }
        }

        private async Task<bool> HandleWithdrawAsync(string username, int userPin)
        {
            try
            {
                double amount = InputHelper.GetAmountInput();
                DisplayHelper.DisplayMessage("\nPlease enter your PIN to confirm:");
                int enteredPin = InputHelper.GetPinInput();

                var transactionDto = new TransactionDto
                {
                    Username = username,
                    Type = TransactionType.Debit,
                    Amount = amount
                };

                var result = await _transactionService.WithdrawAsync(transactionDto, enteredPin);
                DisplayHelper.DisplaySuccess(string.Format(UIMessages.WithdrawSuccess, amount.ToString("F2")));
                DisplayHelper.DisplayInfo($"New Balance: ${result.NewBalance:F2}");
                
                return true;
            }
            catch (AccountFrozenException)
            {
                DisplayHelper.DisplayError(UIMessages.AccountFrozen);
                DisplayHelper.DisplayMessage(UIMessages.ContactAdmin);
                return false; 
            }
            catch (ExceededPinAttemptsException)
            {
                DisplayHelper.DisplayError(UIMessages.PinAttemptsExceeded);
                DisplayHelper.DisplayError(UIMessages.AccountFrozen);
                DisplayHelper.DisplayMessage(UIMessages.ContactAdmin);
                return false; 
            }
            catch (InsufficientFundsException ex)
            {
                DisplayHelper.DisplayError(ex.Message);
                return true;
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Withdrawal failed: {ex.Message}");
                return true;
            }
        }

        private async Task<bool> HandleViewBalanceAsync(string username, int userPin)
        {
            try
            {
                DisplayHelper.DisplayMessage("\nPlease enter your PIN to view balance:");
                int enteredPin = InputHelper.GetPinInput();

                var balanceDto = await _transactionService.GetBalanceAsync(username, enteredPin);
                DisplayHelper.DisplayBalance(balanceDto.Balance);
                
                return true;
            }
            catch (AccountFrozenException)
            {
                DisplayHelper.DisplayError(UIMessages.AccountFrozen);
                DisplayHelper.DisplayMessage(UIMessages.ContactAdmin);
                return false; 
            }
            catch (ExceededPinAttemptsException)
            {
                DisplayHelper.DisplayError(UIMessages.PinAttemptsExceeded);
                DisplayHelper.DisplayError(UIMessages.AccountFrozen);
                DisplayHelper.DisplayMessage(UIMessages.ContactAdmin);
                return false; 
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Failed to retrieve balance: {ex.Message}");
                return true;
            }
        }

        private async Task<bool> HandleChangePinAsync(string username, int currentPin)
        {
            try
            {
                DisplayHelper.DisplayMessage("\n--- Change PIN ---");
                DisplayHelper.DisplayMessage($"Enter new PIN ({Constants.PinLength} digits):");
                int newPin = InputHelper.GetPinInput();
                
                DisplayHelper.DisplayMessage($"Confirm new PIN ({Constants.PinLength} digits):");
                int confirmPin = InputHelper.GetPinInput();

                if (newPin == currentPin)
                {
                    DisplayHelper.DisplayError("New PIN cannot be the same as current PIN.");
                    return true;
                }

                var pinChangeDto = new PinChangeDto
                {
                    Username = username,
                    CurrentPin = currentPin,
                    NewPin = newPin
                };

                await _identityService.ChangePinAsync(pinChangeDto);
                
                DisplayHelper.DisplaySuccess(UIMessages.PinChangeSuccess);
                
                return true;
            }
            catch (UserNotFoundException)
            {
                DisplayHelper.DisplayError("User not found. Please contact administrator.");
                return true;
            }
            catch (InvalidCredentialsException)
            {
                DisplayHelper.DisplayError("Current PIN is incorrect. Please try again.");
                return true;
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"PIN change failed: {ex.Message}");
                return true;
            }
        }

        private async Task HandleViewTransactionsAsync(string username)
        {
            try
            {
                var history = await _transactionService.GetTransactionHistoryAsync(username, 5);
                DisplayHelper.DisplayMessage("\n--- Last 5 Transactions ---");
                DisplayHelper.DisplayTransactionHistory(history);
            }
            catch (Exception ex)
            {
                DisplayHelper.DisplayError($"Failed to retrieve transactions: {ex.Message}");
            }
        }
    }
}

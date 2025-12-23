using System;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Exceptions;
using Backend.Services;
using DataAccess.Entities;
using Frontend.Helper;
using Frontend.Model;
using Frontend.UserInterface;

namespace Frontend.UserInterface
{
    public class UserMenu
    {
        private readonly ConsoleUI _consoleUI;
        private readonly IIdentityService _identityService;
        private readonly ITransactionService _transactionService;
        private const int MaxPinAttempts = 3;

        public UserMenu(ConsoleUI consoleUI, IIdentityService identityService, ITransactionService transactionService)
        {
            _consoleUI = consoleUI;
            _identityService = identityService;
            _transactionService = transactionService;
        }

        public async Task HandleUserFlowAsync()
        {
            _consoleUI.DisplayMessage(UIMessages.WelcomeUser);
            _consoleUI.DisplayMessage(UIMessages.UserMenu);
            Console.Write(UIMessages.EnterChoice + " ");

            int choice = InputHelper.GetIntegerInput((int)UserMenuOption.SignUp, (int)UserMenuOption.GoBack);
            var option = (UserMenuOption)choice;

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
                _consoleUI.DisplayMessage(UIMessages.WelcomeSignup);
                
                string username = InputHelper.GetUsernameInput();
                int pin = InputHelper.GetPinInput();

                var signupDto = new SignupDto
                {
                    Username = username,
                    Pin = pin,
                    Role = UserRole.User
                };

                var result = await _identityService.SignupAsync(signupDto);
                _consoleUI.DisplaySuccess(UIMessages.SignupSuccess);
                
                // Auto-login after signup
                await HandleUserActionsAsync(username, pin);
            }
            catch (UsernameTakenException ex)
            {
                _consoleUI.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Signup failed: {ex.Message}");
            }
        }

        private async Task HandleLoginAsync()
        {
            try
            {
                _consoleUI.DisplayMessage(UIMessages.WelcomeLogin);
                
                string username = InputHelper.GetUsernameInput();
                int pin = InputHelper.GetPinInput();

                var loginDto = new LoginDTO
                {
                    Username = username,
                    Pin = pin
                };

                var result = await _identityService.LoginAsync(loginDto);

                if (!result.IsLoginSuccessful)
                {
                    if (result.IsFrozen)
                    {
                        _consoleUI.DisplayError(UIMessages.AccountFrozen);
                        _consoleUI.DisplayMessage(UIMessages.ContactAdmin);
                    }
                    else
                    {
                        _consoleUI.DisplayError(ExceptionMessages.InvalidCredentials);
                    }
                    return;
                }

                _consoleUI.DisplaySuccess(string.Format(UIMessages.LoginSuccess, username));
                await HandleUserActionsAsync(username, pin);
            }
            catch (AccountFrozenException ex)
            {
                _consoleUI.DisplayError(ex.Message);
                _consoleUI.DisplayMessage(UIMessages.ContactAdmin);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Login failed: {ex.Message}");
            }
        }

        private async Task HandleUserActionsAsync(string username, int userPin)
        {
            while (true)
            {
                _consoleUI.DisplayMessage(UIMessages.UserActionMenu);
                Console.Write(UIMessages.EnterChoice + " ");

                int choice = InputHelper.GetIntegerInput((int)UserActionOption.Deposit, (int)UserActionOption.SignOut);
                var action = (UserActionOption)choice;

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
                        _consoleUI.DisplayMessage(UIMessages.SigningOut);
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

                // Verify PIN before transaction
                if (!VerifyPinWithAttempts(userPin))
                {
                    await FreezeAccountAsync(username);
                    return false;
                }

                var transactionDto = new TransactionDto
                {
                    Username = username,
                    Type = TransactionType.Credit,
                    Amount = amount
                };

                var result = await _transactionService.DepositAsync(transactionDto);
                _consoleUI.DisplaySuccess(string.Format(UIMessages.DepositSuccess, amount.ToString("F2")));
                _consoleUI.DisplayInfo($"New Balance: ${result.NewBalance:F2}");
                
                return true;
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Deposit failed: {ex.Message}");
                return true;
            }
        }

        private async Task<bool> HandleWithdrawAsync(string username, int userPin)
        {
            try
            {
                double amount = InputHelper.GetAmountInput();

                // Verify PIN before transaction
                if (!VerifyPinWithAttempts(userPin))
                {
                    await FreezeAccountAsync(username);
                    return false;
                }

                var transactionDto = new TransactionDto
                {
                    Username = username,
                    Type = TransactionType.Debit,
                    Amount = amount
                };

                var result = await _transactionService.WithdrawAsync(transactionDto);
                _consoleUI.DisplaySuccess(string.Format(UIMessages.WithdrawSuccess, amount.ToString("F2")));
                _consoleUI.DisplayInfo($"New Balance: ${result.NewBalance:F2}");
                
                return true;
            }
            catch (InsufficientFundsException ex)
            {
                _consoleUI.DisplayError(ex.Message);
                return true;
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Withdrawal failed: {ex.Message}");
                return true;
            }
        }

        private async Task<bool> HandleViewBalanceAsync(string username, int userPin)
        {
            try
            {
                // Verify PIN before showing balance
                if (!VerifyPinWithAttempts(userPin))
                {
                    await FreezeAccountAsync(username);
                    return false;
                }


                var balanceDto = await _transactionService.GetBalanceAsync(username);
                _consoleUI.DisplayBalance(balanceDto.Balance);
                
                return true;
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Failed to retrieve balance: {ex.Message}");
                return true;
            }
        }

        private async Task<bool> HandleChangePinAsync(string username, int currentPin)
        {
            try
            {
                // Verify current PIN
                if (!VerifyPinWithAttempts(currentPin))
                {
                    await FreezeAccountAsync(username);
                    return false;
                }

                Console.Write(UIMessages.EnterNewPin + " ");
                int newPin = InputHelper.GetPinInput();

                if (newPin == currentPin)
                {
                    _consoleUI.DisplayError("New PIN cannot be the same as current PIN.");
                    return true;
                }

                var pinChangeDto = new PinChangeDto
                {
                    Username = username,
                    CurrentPin = currentPin,
                    NewPin = newPin
                };

                bool success = await _identityService.ChangePinAsync(pinChangeDto);
                
                if (success)
                {
                    _consoleUI.DisplaySuccess(UIMessages.PinChangeSuccess);
                }
                else
                {
                    _consoleUI.DisplayError("PIN change failed.");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"PIN change failed: {ex.Message}");
                return true;
            }
        }

        private async Task HandleViewTransactionsAsync(string username)
        {
            try
            {
                var history = await _transactionService.GetTransactionHistoryAsync(username, 5);
                _consoleUI.DisplayMessage("\n--- Last 5 Transactions ---");
                _consoleUI.DisplayTransactionHistory(history);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Failed to retrieve transactions: {ex.Message}");
            }
        }

        private bool VerifyPinWithAttempts(int correctPin)
        {
            int attempts = MaxPinAttempts;

            while (attempts > 0)
            {
                Console.Write(UIMessages.EnterPin + " ");
                int enteredPin = InputHelper.GetPinInput();

                if (enteredPin == correctPin)
                {
                    return true;
                }

                attempts--;
                if (attempts > 0)
                {
                    _consoleUI.DisplayError(string.Format(UIMessages.PinMismatch, attempts));
                }
            }

            _consoleUI.DisplayError(UIMessages.PinAttemptsExceeded);
            return false;
        }

        private async Task FreezeAccountAsync(string username)
        {
            try
            {
                // Note: In real implementation, this would need admin username
                // For now, we'll just display the message
                _consoleUI.DisplayError(UIMessages.AccountFrozen);
                _consoleUI.DisplayMessage(UIMessages.ContactAdmin);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Error: {ex.Message}");
            }
        }
    }
}

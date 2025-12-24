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
    public class AdminMenu
    {
        private readonly ConsoleUI _consoleUI;
        private readonly IIdentityService _identityService;
        private readonly ITransactionService _transactionService;

        public AdminMenu(ConsoleUI consoleUI, IIdentityService identityService, ITransactionService transactionService)
        {
            _consoleUI = consoleUI;
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        public async Task HandleAdminMenuAsync()
        {
            _consoleUI.DisplayMessage(UIMessages.WelcomeAdmin);
            _consoleUI.DisplayMessage(UIMessages.AdminMenu);
            Console.Write(UIMessages.EnterChoice + " ");

            int choice = InputHelper.GetIntegerInput(1, 2);
            
            if (choice == 1)
            {
                await HandleAdminLoginAsync();
            }
        }

        private async Task HandleAdminLoginAsync()
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

                if (!result.IsLoginSuccessful || !result.IsAdmin)
                {
                    _consoleUI.DisplayError("Invalid admin credentials.");
                    return;
                }

                _consoleUI.DisplaySuccess(string.Format(UIMessages.LoginSuccess, username));
                await HandleAdminActionsAsync(username);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Admin login failed: {ex.Message}");
            }
        }

        private async Task HandleAdminActionsAsync(string adminUsername)
        {
            while (true)
            {
                _consoleUI.DisplayMessage(UIMessages.AdminActionMenu);
                Console.Write(UIMessages.EnterChoice + " ");

                int choice = InputHelper.GetIntegerInput((int)AdminActionOption.ViewFrozenAccounts, (int)AdminActionOption.Exit);
                var action = (AdminActionOption)choice;

                switch (action)
                {
                    case AdminActionOption.ViewFrozenAccounts:
                        await HandleViewFrozenAccountsAsync(adminUsername);
                        break;
                    case AdminActionOption.UnfreezeAccount:
                        await HandleUnfreezeAccountAsync(adminUsername);
                        break;
                    case AdminActionOption.ViewAtmBalance:
                        await HandleViewAtmBalanceAsync(adminUsername);
                        break;
                    case AdminActionOption.DepositToAtm:
                        await HandleDepositToAtmAsync(adminUsername);
                        break;
                    case AdminActionOption.ViewAtmTransactions:
                        await HandleViewAtmTransactionsAsync(adminUsername);
                        break;
                    case AdminActionOption.ChangeAdmin:
                        await HandleChangeAdminAsync(adminUsername);
                        return; // Exit after changing admin
                    case AdminActionOption.Exit:
                        _consoleUI.DisplayMessage(UIMessages.Exiting);
                        return;
                }
            }
        }

        private async Task HandleViewFrozenAccountsAsync(string adminUsername)
        {
            try
            {
                var frozenAccounts = await _identityService.GetFrozenAccountsAsync(adminUsername);
                _consoleUI.DisplayFrozenAccounts(frozenAccounts);
            }
            catch (UnauthorizedAccessException ex)
            {
                _consoleUI.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Failed to retrieve frozen accounts: {ex.Message}");
            }
        }

        private async Task HandleUnfreezeAccountAsync(string adminUsername)
        {
            try
            {
                string username = InputHelper.GetUsernameInput();

                var unfreezeDto = new UnfreezeUserDto
                {
                    AdminUsername = adminUsername,
                    Username = username
                };

                bool success = await _identityService.UnfreezeAccountAsync(unfreezeDto);
                
                if (success)
                {
                    _consoleUI.DisplaySuccess(UIMessages.UnfreezeSuccess);
                }
                else
                {
                    _consoleUI.DisplayError("Failed to unfreeze account.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _consoleUI.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Unfreeze operation failed: {ex.Message}");
            }
        }

        private async Task HandleViewAtmBalanceAsync(string adminUsername)
        {
            try
            {
                var atmBalance = await _transactionService.GetAtmBalanceAsync(adminUsername);
                _consoleUI.DisplayAtmBalance(atmBalance.TotalBalance);
            }
            catch (UnauthorizedAccessException ex)
            {
                _consoleUI.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Failed to retrieve ATM balance: {ex.Message}");
            }
        }

        private async Task HandleDepositToAtmAsync(string adminUsername)
        {
            try
            {
                double amount = InputHelper.GetAmountInput();

                var depositDto = new DepositCashDto
                {
                    AdminUsername = adminUsername,
                    Amount = amount
                };

                bool success = await _transactionService.DepositToAtmAsync(depositDto);
                
                if (success)
                {
                    _consoleUI.DisplaySuccess(string.Format(UIMessages.AtmDepositSuccess, amount.ToString("F2")));
                }
                else
                {
                    _consoleUI.DisplayError("ATM deposit failed.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _consoleUI.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"ATM deposit failed: {ex.Message}");
            }
        }

        private async Task HandleViewAtmTransactionsAsync(string adminUsername)
        {
            try
            {
                var history = await _transactionService.GetTransactionHistoryAsync(adminUsername, Backend.ApplicationConstants.Constants.DefaultTransactionHistoryCount);
                _consoleUI.DisplayMessage("\n--- Last 5 ATM Transactions ---");
                _consoleUI.DisplayTransactionHistory(history);
            }
            catch (UnauthorizedAccessException ex)
            {
                _consoleUI.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Failed to retrieve ATM transactions: {ex.Message}");
            }
        }

        private async Task HandleChangeAdminAsync(string adminUsername)
        {
            try
            {
                _consoleUI.DisplayMessage("\n--- Change Admin ---");
                _consoleUI.DisplayMessage("WARNING: You will lose admin privileges after this operation!");
                
                string newAdminUsername = InputHelper.GetUsernameInput();
                int newAdminPin = InputHelper.GetPinInput();

                var changeAdminDto = new ChangeAdminDto
                {
                    CurrentAdminUsername = adminUsername,
                    NewAdminUsername = newAdminUsername,
                    NewAdminPin = newAdminPin
                };

                bool success = await _identityService.ChangeAdminAsync(changeAdminDto);
                
                if (success)
                {
                    _consoleUI.DisplaySuccess(UIMessages.ChangeAdminSuccess);
                    _consoleUI.DisplayMessage($"New admin: {newAdminUsername}");
                    _consoleUI.DisplayMessage("You have been logged out.");
                }
                else
                {
                    _consoleUI.DisplayError("Failed to change admin. User not found or invalid operation.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _consoleUI.DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                _consoleUI.DisplayError($"Change admin operation failed: {ex.Message}");
            }
        }
    }
}

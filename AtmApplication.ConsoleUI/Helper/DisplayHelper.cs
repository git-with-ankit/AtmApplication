using System;
using AtmApplication.Backend.DTOs;
using AtmApplication.DataAccess.Entities;

namespace AtmApplication.ConsoleUI.Helper
{
    internal static class DisplayHelper
    {
        public static void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static void DisplayError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }

        public static void DisplaySuccess(string success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(success);
            Console.ResetColor();
        }

        public static void DisplayInfo(string info)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(info);
            Console.ResetColor();
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static void DisplayPrompt(string prompt)
        {
            Console.Write(prompt);
        }

        public static void DisplayTransactionHistory(TransactionsHistoryDto history)
        {
            if (history.Transactions.Count == 0)
            {
                DisplayInfo("No transactions found.");
                return;
            }

            Console.WriteLine("\n--- Transaction History ---");
            foreach (var transaction in history.Transactions)
            {
                string typeSymbol = transaction.Type == TransactionType.Credit ? "+" : "-";
                var localTime = transaction.Timestamp.ToLocalTime();
                string adminIndicator = transaction.IsAdminTransaction ? " [ADMIN]" : "";
                Console.WriteLine($"{localTime:yyyy-MM-dd HH:mm:ss} | {transaction.Username}{adminIndicator} | {typeSymbol}${transaction.Amount:F2} | Balance: ${transaction.NewBalance:F2}");
            }
        }

        public static void DisplayBalance(double balance)
        {
            Console.WriteLine($"\nYour current balance: ${balance:F2}");
        }

        public static void DisplayAtmBalance(double balance)
        {
            Console.WriteLine($"\nATM Total Balance: ${balance:F2}");
        }

        public static void DisplayFrozenAccounts(FrozenAccountsListDto frozenAccountsList)
        {
            Console.WriteLine("\n--- Frozen Accounts ---");
            
            if (frozenAccountsList.FrozenAccounts.Count == 0)
            {
                Console.WriteLine("No frozen accounts found.");
                return;
            }

            Console.WriteLine($"Total Frozen Accounts: {frozenAccountsList.FrozenAccounts.Count}\n");
            
            foreach (var account in frozenAccountsList.FrozenAccounts)
            {
                Console.WriteLine($"Username: {account.Username}");
                Console.WriteLine($"Balance: ${account.Balance:F2}");
                Console.WriteLine(new string('-', 40));
            }
        }
    }
}

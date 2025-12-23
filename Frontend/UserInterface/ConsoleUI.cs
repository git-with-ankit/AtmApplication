using System;
using Backend.DTOs;
using DataAccess.Entities;

namespace Frontend.UserInterface
{
    public class ConsoleUI
    {
        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void DisplayError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {error}");
            Console.ResetColor();
        }

        public void DisplaySuccess(string success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(success);
            Console.ResetColor();
        }

        public void DisplayInfo(string info)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(info);
            Console.ResetColor();
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void DisplayTransactionHistory(TransactionsHistoryDto history)
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
                Console.WriteLine($"{transaction.Timestamp:yyyy-MM-dd HH:mm:ss} | {typeSymbol}${transaction.Amount:F2} | Balance: ${transaction.NewBalance:F2}");
            }
        }

        public void DisplayBalance(double balance)
        {
            Console.WriteLine($"\nYour current balance: ${balance:F2}");
        }

        public void DisplayAtmBalance(double balance)
        {
            Console.WriteLine($"\nATM Total Balance: ${balance:F2}");
        }

        public void DisplayFrozenAccounts(UserListDto user)
        {
            if (string.IsNullOrEmpty(user.Username))
            {
                DisplayInfo("No frozen accounts found.");
                return;
            }

            Console.WriteLine("\n--- Frozen Accounts ---");
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"Balance: ${user.Balance:F2}");
            Console.WriteLine($"Frozen: {user.IsFrozen}");
        }
    }
}

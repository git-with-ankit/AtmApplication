using DataAccess.ApplicationConstants;
using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.FileRepository
{
    internal sealed class FileAccountsRepository : FileRepositoryBase, IAccountRepository<AccountDetails>
    {
        public FileAccountsRepository() : base(FilePaths.AccountsFilePath) { }

        public async Task<List<AccountDetails>> GetAllDataAsync()
        {
            var lines = await ReadAllLinesAsync();
            return lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(ParseAccountDetails)
                .ToList();
        }

        public async Task<AccountDetails?> GetDataByUsernameAsync(string username)
        {
            var accountRecords = await GetAllDataAsync();
            return accountRecords.FirstOrDefault(record => record.Username == username);
        }

        public async Task AddDataAsync(AccountDetails account)
        {
            var existingAccount = await GetDataByUsernameAsync(account.Username);
            if (existingAccount != null)
            {
                throw new InvalidOperationException($"Account for '{account.Username}' already exists");
            }

            var accountRecords = await GetAllDataAsync();
            accountRecords.Add(account);
            await SaveAllDataAsync(accountRecords);
        }

        public async Task DeleteDataByUsernameAsync(string username)
        {
            var accountRecords = await GetAllDataAsync();
            var remainingAccounts = accountRecords
                .Where(record => record.Username != username)
                .ToList();
            await SaveAllDataAsync(remainingAccounts);
        }

        public async Task UpdateDataAsync(AccountDetails account)
        {
            var accountRecords = await GetAllDataAsync();
            var updatedAccounts = accountRecords
                .Select(record =>
                {
                    if (record.Username == account.Username)
                    {
                        return account; 
                    }
                    return record;
                })
                .ToList();

            await SaveAllDataAsync(updatedAccounts);
        }

        public async Task<double> GetBalanceAsync(string username)
        {
            var account = await GetDataByUsernameAsync(username);
            return account?.Balance ?? 0;
        }

        public async Task UpdateBalanceAsync(string username, double newBalance)
        {
            var account = await GetDataByUsernameAsync(username);
            if (account == null)
            {
                throw new InvalidOperationException($"No account found for '{username}'");
            }

            account.Balance = newBalance;
            await UpdateDataAsync(account);
        }

        private static AccountDetails ParseAccountDetails(string record)
        {
            var values = record.Split(',');
            if (values.Length != 2)
            {
                throw new FormatException("Invalid account details format. Expected 2 fields.");
            }

            return new AccountDetails
            {
                Username = values[0],
                Balance = double.Parse(values[1]) 
            };
        }

        private async Task SaveAllDataAsync(List<AccountDetails> accountRecords)
        {
            var lines = accountRecords
                .Select(record => $"{record.Username};{record.Balance:0.00}")
                .ToArray();
            await WriteAllLinesAsync(lines);
        }
    }
}

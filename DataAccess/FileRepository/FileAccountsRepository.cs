using DataAccess.ApplicationConstants;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.FileRepository
{
    public sealed class FileAccountsRepository : FileRepositoryBase, IRepository<AccountDetails>
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
            return accountRecords.FirstOrDefault(record => record.Username.Equals(username));
        }

        public async Task AddDataAsync(AccountDetails account)
        {
            var existingAccount = await GetDataByUsernameAsync(account.Username);
            if (existingAccount is not null)
            {
                throw new InvalidOperationException(string.Format(ExceptionConstants.AccountAlreadyExists, account.Username));
            }

            var accountRecords = await GetAllDataAsync();
            accountRecords.Add(account);
            await SaveAllDataAsync(accountRecords);
        }

        public async Task DeleteDataByUsernameAsync(string username)
        {
            var accountRecords = await GetAllDataAsync();
            var remainingAccounts = accountRecords
                .Where(record => !record.Username.Equals(username))
                .ToList();
            await SaveAllDataAsync(remainingAccounts);
        }

        public async Task UpdateDataAsync(AccountDetails account)
        {
            var accountRecords = await GetAllDataAsync();
            var updatedAccounts = accountRecords
                .Select(record =>
                {
                    if (record.Username.Equals(account.Username))
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
            if (account is null)
            {
                throw new InvalidOperationException(string.Format(ExceptionConstants.AccountNotFound, username));
            }

            account.Balance = newBalance;
            await UpdateDataAsync(account);
        }

        private static AccountDetails ParseAccountDetails(string record)
        {
            var values = record.Split(',');
            if (values.Length != 2)
            {
                throw new FormatException(ExceptionConstants.InvalidAccountDetailsFormat);
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

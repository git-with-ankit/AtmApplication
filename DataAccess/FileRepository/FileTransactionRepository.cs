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
    internal class FileTransactionRepository : FileRepositoryBase, ITransactionRepository<TransactionDetails>
    {
        public FileTransactionRepository() : base(FilePaths.TransactionsFilePath) { }
        public async Task AddDataAsync(TransactionDetails transaction)
        {
            var transactionRecords = await GetAllDataAsync();
            transactionRecords.Add(transaction);
            await SaveAllDataAsync(transactionRecords);
        }
        public async Task<List<TransactionDetails>> GetAllDataAsync()
        {
            var lines = await ReadAllLinesAsync();
            return lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(ParseTransactionDetails)
                .ToList();
        }
        public async Task<List<TransactionDetails>> GetTransactionsByUsernameAsync(string username, int count)
        {
            var transactionRecords = await GetAllDataAsync();
            return transactionRecords
                .Where(t => t.UserName == username)
                .OrderByDescending(t => t.TimeStamp)  
                .Take(count)                         
                .ToList();
        }
        public async Task<List<TransactionDetails>> GetLastTransactionsAsync(int count)
        {
            var transactionRecords = await GetAllDataAsync();
            return transactionRecords
                .OrderByDescending(t => t.TimeStamp)
                .Take(count)
                .ToList();
        }
        private TransactionDetails ParseTransactionDetails(string record)
        {
            var values = record.Split(',');
            if (values.Length != 6)
            {
                throw new FormatException("Invalid transaction details format.");
            }
            return new TransactionDetails()
            {
                UserName = values[0],
                Type = Enum.Parse<TransactionType>(values[1]),
                Amount = double.Parse(values[2]),
                TimeStamp = DateTime.Parse(values[3]).ToLocalTime(),
                NewBalance = double.Parse(values[4]),
                IsAdminTransaction = bool.Parse(values[5])
            };
        }
        private async Task SaveAllDataAsync(List<TransactionDetails> transactionRecords)
        {
            var lines = transactionRecords
                .Select(record => $"{record.UserName},{record.Type}," +
                                 $"{record.Amount:0.00},{record.TimeStamp}," +
                                 $"{record.NewBalance:0.00},{record.IsAdminTransaction}")
                .ToArray();
            await WriteAllLinesAsync(lines);
        }
    }
}

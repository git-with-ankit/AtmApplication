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
    public sealed class FileAtmRepository : FileRepositoryBase, IRepository<AtmDetails>
    {
        public FileAtmRepository() : base(FilePaths.AtmFilePath) { }

        public async Task<List<AtmDetails>> GetAllDataAsync()
        {
            var lines = await ReadAllLinesAsync();
            return lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(ParseAtmDetails)
                .ToList();
        }

        public async Task<AtmDetails?> GetDataByUsernameAsync(string username)
        {
            var atmRecords = await GetAllDataAsync();
            return atmRecords.FirstOrDefault(record => record.AdminUsername.Equals(username));
        }

        public async Task AddDataAsync(AtmDetails atmDetails)
        {
            var existingAtm = await GetDataByUsernameAsync(atmDetails.AdminUsername);
            if (existingAtm is not null)
            {
                throw new InvalidOperationException(string.Format(ExceptionConstants.AtmRecordAlreadyExists, atmDetails.AdminUsername));
            }

            var atmRecords = await GetAllDataAsync();
            atmRecords.Add(atmDetails);
            await SaveAllDataAsync(atmRecords);
        }

        public async Task UpdateDataAsync(AtmDetails atmDetails)
        {
            var atmRecords = await GetAllDataAsync();
            var updatedRecords = atmRecords
                .Select(record =>
                {
                    if (record.AdminUsername.Equals(atmDetails.AdminUsername))
                    {
                        return atmDetails;
                    }
                    return record;
                })
                .ToList();

            await SaveAllDataAsync(updatedRecords);
        }

        public async Task DeleteDataByUsernameAsync(string username)
        {
            var atmRecords = await GetAllDataAsync();
            var remainingRecords = atmRecords
                .Where(record => !record.AdminUsername.Equals(username))
                .ToList();
            await SaveAllDataAsync(remainingRecords);
        }


        private static AtmDetails ParseAtmDetails(string record)
        {
            var values = record.Split(',');
            if (values.Length != 2)
            {
                throw new FormatException(ExceptionConstants.InvalidAtmDetailsFormat);
            }

            return new AtmDetails
            {
                AdminUsername = values[0],
                TotalBalance = double.Parse(values[1])
            };
        }

        private async Task SaveAllDataAsync(List<AtmDetails> atmRecords)
        {
            var lines = atmRecords
                .Select(record => $"{record.AdminUsername},{record.TotalBalance:0.00}")
                .ToArray();
            await WriteAllLinesAsync(lines);
        }
    }
}

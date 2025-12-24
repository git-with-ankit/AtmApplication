using AtmApplication.DataAccess.ApplicationConstants;
using AtmApplication.DataAccess.Entities;
using AtmApplication.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmApplication.DataAccess.FileRepository
{
    public sealed class FileAtmRepository : FileRepositoryBase, IAtmRepository
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

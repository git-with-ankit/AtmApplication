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
    internal sealed class FileAtmRepository : FileRepositoryBase, IAtmRepository<AtmDetails>
    {
        public FileAtmRepository() : base(FilePaths.AtmFilePath) { }

        public async Task<double> GetBalanceAsync()
        {
            var lines = await ReadAllLinesAsync();
            var firstLine = lines.FirstOrDefault(line => !string.IsNullOrWhiteSpace(line));
            return firstLine != null ? double.Parse(firstLine) : 0.0;
        }

        public async Task UpdateDataAsync(AtmDetails atmDetails)
        {
            await WriteAllLinesAsync(new[] { $"{atmDetails.TotalBalance:0.00}" }); 
        }
    }
}

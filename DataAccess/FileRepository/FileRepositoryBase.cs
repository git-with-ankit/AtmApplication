using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.ApplicationConstants;
using DataAccess.Models;

namespace DataAccess.FileRepository
{
    internal abstract class FileRepositoryBase
    {
        private string _filePath;

        protected FileRepositoryBase(string filePath)
        {
            _filePath = filePath;
            EnsureFilePathExists(filePath);
        }

        protected void EnsureFilePathExists(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (!File.Exists(filePath))
            {
                if(filePath == FilePaths.UsersFilePath)
                {
                    string adminLine = $"{DefaultConstants.AdminUsername},{DefaultConstants.AdminPin},{UserRole.Admin},False";
                    File.WriteAllText(filePath, adminLine);
                }
                else
                {
                    File.WriteAllText(filePath, String.Empty);
                }
            }
        }

        protected async Task<string[]> ReadAllLinesAsync()
        {
            return await File.ReadAllLinesAsync(_filePath);
        }

        protected async Task WriteAllLinesAsync(string[] lines)
        {
            await File.WriteAllLinesAsync(_filePath, lines);  
        }

        protected async Task AppendLineAsync(string line)
        {
            await File.AppendAllLinesAsync(_filePath, new[] {line});
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtmApplication.DataAccess.ApplicationConstants;
using AtmApplication.DataAccess.Entities;
using AtmApplication.DataAccess.Helper;

namespace AtmApplication.DataAccess.FileRepository
{
    public abstract class FileRepositoryBase
    {
        private readonly string _filePath;

        protected FileRepositoryBase(string filePath)
        {
            _filePath = filePath;
            EnsureFilePathExists(filePath);
        }

        private void EnsureFilePathExists(string filePath)
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
                    string adminLine = $"{DefaultConstants.AdminUsername},{DefaultConstants.AdminPin},True,False,0";
                    File.WriteAllText(filePath, adminLine);
                }
                else if(filePath == FilePaths.AtmFilePath)
                {
                    string atmLine = $"{DefaultConstants.AdminUsername},0.00";
                    File.WriteAllText(filePath, atmLine);
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

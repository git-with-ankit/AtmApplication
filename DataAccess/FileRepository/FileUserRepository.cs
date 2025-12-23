using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.ApplicationConstants;

namespace DataAccess.FileRepository
{
    public sealed class FileUserRepository : FileRepositoryBase, IRepository<UserDetails>
    {
        public FileUserRepository() : base(FilePaths.UsersFilePath) { }

        public async Task<List<UserDetails>> GetAllDataAsync()
        {
            var lines = await ReadAllLinesAsync();
            return lines
                    .Where(line => !String.IsNullOrWhiteSpace(line))
                    .Select(ParseUserDetails)
                    .ToList();
        }

        public async Task<UserDetails?> GetDataByUsernameAsync(string username)
        {
            var userRecords = await GetAllDataAsync();
            return userRecords.FirstOrDefault(record => record.Username.Equals(username));
        }

        public async Task AddDataAsync(UserDetails user)
        {
            var existingUser = await GetDataByUsernameAsync(user.Username);
            if (existingUser is not null) 
            {
                throw new InvalidOperationException(string.Format(ExceptionConstants.UserAlreadyExists, user.Username));
            }
            var userRecords = await GetAllDataAsync();
            userRecords.Add(user);
            await SaveAllDataAsync(userRecords);
        }

        public async Task DeleteDataByUsernameAsync(string username)
        {
            var userRecords = await GetAllDataAsync();
            var remainingUsers = userRecords
                                    .Where(record => !record.Username.Equals(username))
                                    .ToList();
            await SaveAllDataAsync(remainingUsers);
        }
        public async Task UpdateDataAsync(UserDetails user)
        {
            var userRecords = await GetAllDataAsync();
            var updatedUsers = userRecords
                                    .Select(record =>
                                    {
                                        if (record.Username.Equals(user.Username))
                                        {
                                            return user; 
                                        }
                                        return record;
                                    })
                                    .ToList();

            await SaveAllDataAsync(updatedUsers);
        }

        private UserDetails ParseUserDetails(string record)
        {
            var values = record.Split(',');
            if(values.Length != 4)
            {
                throw new FormatException(ExceptionConstants.InvalidUserDetailsFormat);
            }
            return new UserDetails()
            {
                Username = values[0],
                Pin = int.Parse(values[1]),
                Role = Enum.Parse<UserRole>(values[2]),
                IsFreezed = bool.Parse(values[3])
            };
        }

        private async Task SaveAllDataAsync(List<UserDetails> userRecords)
        {
            var lines = userRecords
                            .Select(record => $"{record.Username},{record.Pin},{record.Role},{record.IsFreezed}")
                            .ToArray();
            await WriteAllLinesAsync(lines);
        }
    }
}

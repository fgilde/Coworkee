using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Coworkee.Application.Configurations;
using Coworkee.Application.Contracts.Attributes;
using Coworkee.Application.Contracts.Services;
using Coworkee.Infrastructure.Contexts;

namespace Coworkee.Infrastructure.Services
{
    [RegisterAsIfConfigValueIsEmpty(typeof(IDbBackupService), new[] { nameof(BackupOptions), nameof(BackupOptions.BucketName) })]
    public class LocalDbBackupService : IDbBackupService
    {
        public string BackupDirectory => "DBBackups";
        
        private IFileAccess _fileAccess;
        private readonly ApplicationDbContext _dbContext;

        public LocalDbBackupService(IFileAccess fileAccess, ApplicationDbContext dbContext)
        {
            _fileAccess = fileAccess;
            _dbContext = dbContext;
        }

        public string BackupDatabase(DbContext context, string fileName = null)
        {
            var backupPath = _fileAccess.EnsureFileNotExists(BackupDirectory, fileName ?? $"DB_Backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.bak");
            // Get the connection string for the database
            //var connectionString = context.Database.GetDbConnection().ConnectionString;

            // Create a backup command using the connection string
            string backupCommand = $"BACKUP DATABASE {context.Database.GetDbConnection().Database} TO DISK='{backupPath}'";
            //string backupCommand = $"BACKUP DATABASE {context.Database.GetDbConnection().Database} TO DISK = '{backupPath}' WITH FORMAT";

            // Open the connection to the database
            using var connection = context.Database.GetDbConnection();
            connection.Open();

            // Create a command using the connection and the backup command
            using var command = connection.CreateCommand();
            command.CommandText = backupCommand;

            // Execute the backup command
            command.ExecuteNonQuery();
            return backupPath;
        }

        public Task<string> BackupDatabaseAsync(string fileName, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => BackupDatabase(_dbContext, fileName), cancellationToken);
        }

        public Task RestoreDatabaseAsync(string fullFileName, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(fullFileName))
                fullFileName = _fileAccess.GetPath(BackupDirectory, fullFileName);
            if (!File.Exists(fullFileName))
                throw new FileNotFoundException("File not found", fullFileName);

            return Task.Run(() => ImportDatabase(_dbContext, fullFileName), cancellationToken);
        }


        public void ImportDatabase(DbContext context, string importPath)
        {
            var database = context.Database.GetDbConnection().Database;
            context.Database.CloseConnection();
            context.Database.EnsureDeleted();
            
            // Get the connection string for the database
            string connectionString = context.Database.GetDbConnection().ConnectionString;

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = "master";
            string masterConnectionString = builder.ConnectionString;

            // Create a restore command using the connection string and the import path
            
            string restoreCommand = $"RESTORE DATABASE {database} FROM DISK='{importPath}' WITH REPLACE";

            // Open the connection to the database
            using var connection = context.Database.GetDbConnection();
            connection.Close();

            using var masterConnection = new SqlConnection(masterConnectionString);
            masterConnection.Open();

            // Create a command using the connection and the restore command
            using var command = masterConnection.CreateCommand();
            command.CommandText = restoreCommand;

            // Execute the restore command
            command.ExecuteNonQuery();


            masterConnection.Close();
            // Re-open the connection
            connection.Open();

        }

        //public async Task<string> BackupDatabaseToS3(DbContext context, string fileName = null, string bucketName = "your-bucket-name")
        //{
        //    // Generate a unique file name if not specified
        //    fileName = fileName ?? $"DB_Backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.bak";
        //    var backupPath = Path.Combine(BackupDirectory, fileName);

        //    // Backup the database to the local file system
        //    BackupDatabase(context, fileName);

        //    // Upload the backup file to S3
        //    using (var fileStream = new FileStream(backupPath, FileMode.Open))
        //    {
        //        var client = new AmazonS3Client();
        //        var request = new PutObjectRequest
        //        {
        //            BucketName = bucketName,
        //            Key = fileName,
        //            InputStream = fileStream
        //        };
        //        await client.PutObjectAsync(request);
        //    }

        //    // Return the S3 file URL
        //    return $"https://s3.amazonaws.com/{bucketName}/{fileName}";
        //}

    }
}

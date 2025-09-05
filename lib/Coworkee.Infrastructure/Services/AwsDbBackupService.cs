using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using lib.Coworkee.Application.Configurations;
using lib.Coworkee.Application.Contracts.Attributes;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Infrastructure.Contexts;

namespace lib.Coworkee.Infrastructure.Services;

[RegisterAsIfConfigValueIsNotEmpty(typeof(IDbBackupService), new[] { nameof(BackupOptions), nameof(BackupOptions.BucketName) })]
public class AwsDbBackupService : IDbBackupService
{
	public string BackupDirectory => "DBBackups";

	private readonly ApplicationDbContext _dbContext;
	private readonly IOptions<BackupOptions> _options;

	public AwsDbBackupService(ApplicationDbContext dbContext, IOptions<BackupOptions> options)
	{
		_dbContext = dbContext;
		_options = options;
	}

	public async Task<string> BackupDatabaseAsync(string fileName, CancellationToken cancellationToken = default)
	{
		fileName ??= $"DB_Backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.bak";
		var database = _dbContext.Database.GetDbConnection().Database;
		var cmd = $"""
				   exec msdb.dbo.rds_backup_database
				   @source_db_name='{database}', @s3_arn_to_backup_to='arn:aws:s3:::{_options.Value.BucketName}/{fileName}'
				   """;

		// Open the connection to the database
		await using var connection = _dbContext.Database.GetDbConnection();
		await connection.OpenAsync(cancellationToken);

		// Create a command using the connection and the backup command
		await using var command = connection.CreateCommand();
		command.CommandText = cmd;

		// Execute the backup command
		await command.ExecuteNonQueryAsync(cancellationToken);
		return fileName;
	}

	public async Task RestoreDatabaseAsync(string fullFileName, CancellationToken cancellationToken = default)
	{
		var database = _dbContext.Database.GetDbConnection().Database;
		await _dbContext.Database.CloseConnectionAsync();
		await _dbContext.Database.EnsureDeletedAsync(cancellationToken);

		// Get the connection string for the database
		var connectionString = _dbContext.Database.GetDbConnection().ConnectionString;

		var builder = new SqlConnectionStringBuilder(connectionString)
		{
			InitialCatalog = "master"
		};
		var masterConnectionString = builder.ConnectionString;

		// Create a restore command using the connection string and the import path

		var restoreCommand = $"""
							  exec msdb.dbo.rds_restore_database
							  @restore_db_name='{database}',
							  @s3_arn_to_restore_from='arn:aws:s3:::{_options.Value.BucketName}/{fullFileName}';
							  """;

		// Open the connection to the database
		await using var connection = _dbContext.Database.GetDbConnection();
		await connection.CloseAsync();

		await using var masterConnection = new SqlConnection(masterConnectionString);
		await masterConnection.OpenAsync(cancellationToken);

		// Create a command using the connection and the restore command
		await using var command = masterConnection.CreateCommand();
		command.CommandText = restoreCommand;

		// Execute the restore command
		await command.ExecuteNonQueryAsync(cancellationToken);


		masterConnection.Close();
		// Re-open the connection
		await connection.OpenAsync(cancellationToken);
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
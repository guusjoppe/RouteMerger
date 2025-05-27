using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using RouteMerger.Infrastructure.Configuration;
using RouteMerger.Infrastructure.Enums;
using RouteMerger.Infrastructure.Interfaces;

namespace RouteMerger.Infrastructure.Services;

public class AwsS3Service : IFileStorageService
{
    private readonly ILogger<IFileStorageService> _logger;
    private readonly FileStorageConfiguration _fileStorageConfiguration;
    private readonly AmazonS3Client _s3Client;

    public AwsS3Service(
        ILogger<IFileStorageService> logger,
        FileStorageConfiguration fileStorageConfiguration)
    {
        _logger = logger;
        _fileStorageConfiguration = fileStorageConfiguration;
        
        var s3Credentials = new BasicAWSCredentials(
            fileStorageConfiguration.AwsS3!.AccessKey,
            fileStorageConfiguration.AwsS3.SecretAccessKey);
        var s3Config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(fileStorageConfiguration.AwsS3.Region),
        };
        _s3Client = new AmazonS3Client(
            s3Credentials,
            s3Config);
    }
    
    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        Action<long> onBytesRead,
        FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        var trustedFileName = Guid.NewGuid() + Path.GetExtension(fileName);
        var fileSize = fileStream.Length;
        try
        {
            var transferUtility = new TransferUtility(_s3Client);
            var transferRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = GetFileKey(trustedFileName, fileDirectory),
                BucketName = _fileStorageConfiguration.AwsS3!.BucketName,
                CannedACL = S3CannedACL.NoACL,
            };

            transferRequest.UploadProgressEvent += (_, e) =>
            {
                onBytesRead(e.TransferredBytes);
            };
            
            await transferUtility.UploadAsync(transferRequest);
        }
        catch(AmazonS3Exception s3Ex)
        {
            _logger.LogError(s3Ex, "Error uploading file to S3: {FileName}", fileName);
            throw new InvalidOperationException(s3Ex.Message, s3Ex);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error uploading file to S3: {FileName}", fileName);
            throw;
        }

        _logger.LogInformation("Successfully saved: {FileName} of size {FileSize} kB from S3 bucket {Bucket} in {Directory}",
            trustedFileName,
            fileSize / 1024,
            _fileStorageConfiguration.AwsS3.BucketName,
            fileDirectory);
        return trustedFileName;
    }

    public async Task<Stream> DownloadFileStreamAsync(
        string fileName,
        FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        try
        {
            var response = await _s3Client.GetObjectAsync(
                _fileStorageConfiguration.AwsS3!.BucketName,
                GetFileKey(fileName, fileDirectory));
            
            _logger.LogInformation("Downloading file: {FileName} from S3 bucket {Bucket} in {Directory}",
                fileName,
                _fileStorageConfiguration.AwsS3.BucketName,
                fileDirectory);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error downloading file from S3: {FileName}", fileName);
            throw new FileNotFoundException("File not found in S3", fileName);
        }
    }

    public async Task DeleteFileAsync(string fileName, FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        try
        {
            var deleteRequest = new Amazon.S3.Model.DeleteObjectRequest
            {
                BucketName = _fileStorageConfiguration.AwsS3!.BucketName,
                Key = GetFileKey(fileName, fileDirectory),
            };
            await _s3Client.DeleteObjectAsync(deleteRequest);
            _logger.LogInformation("Deleted file: {FileName} from S3 bucket {Bucket} in {Directory}",
                fileName,
                _fileStorageConfiguration.AwsS3.BucketName,
                fileDirectory);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from S3: {FileName}", fileName);
            throw new FileNotFoundException("File not found in S3", fileName);
        }
    }

    public void DeleteFile(string fileName, FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        DeleteFileAsync(fileName, fileDirectory).Wait();
    }
    
    public async Task DeleteFilesAsync(IEnumerable<string> fileNames, FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        var files = fileNames.ToList();
        var tasks = files.Select(fileName => DeleteFileAsync(fileName, fileDirectory));
        await Task.WhenAll(tasks);
        _logger.LogInformation("Deleted {Count} files from S3 bucket {Bucket} in {Directory}",
            files.Count,
            _fileStorageConfiguration.AwsS3!.BucketName,
            fileDirectory);
    }

    public void DeleteFiles(IEnumerable<string> fileNames, FileDirectory fileDirectory = FileDirectory.Uploaded)
    {
        DeleteFilesAsync(fileNames, fileDirectory).Wait();
    }
    
    private string GetFileKey(string fileName, FileDirectory fileDirectory)
    {
        return fileDirectory switch
        {
            FileDirectory.Uploaded => $"{_fileStorageConfiguration.UploadDirectory}/{fileName}",
            FileDirectory.Merged => $"{_fileStorageConfiguration.MergedRoutesDirectory}/{fileName}",
            _ => throw new ArgumentOutOfRangeException(nameof(fileDirectory), fileDirectory, null),
        };
    }
}
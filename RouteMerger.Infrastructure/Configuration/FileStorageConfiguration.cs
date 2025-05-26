namespace RouteMerger.Infrastructure.Configuration;

public class FileStorageConfiguration
{
    public const string SectionKey = "FileStorage";
    
    public string UploadDirectory { get; set; } = string.Empty;
    public string MergedRoutesDirectory { get; set; } = string.Empty;
    
    public bool UseLocalFileStorage { get; set; } = false;
    public AwsS3Configuration? AwsS3 { get; set; }
    
    public void Validate()
    {
        if (string.IsNullOrEmpty(UploadDirectory))
        {
            throw new ArgumentException("Upload directory must be specified.", nameof(UploadDirectory));
        }

        if (string.IsNullOrEmpty(MergedRoutesDirectory))
        {
            throw new ArgumentException("Merged routes directory must be specified.", nameof(MergedRoutesDirectory));
        }

        if (UseLocalFileStorage == false && AwsS3 == null)
        {
            throw new ArgumentException("AWS S3 configuration must be provided when not using local file storage.", nameof(AwsS3));
        }
        
        AwsS3?.Validate();
    }
}

public class AwsS3Configuration
{
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    
    public void Validate()
    {
        if (string.IsNullOrEmpty(BucketName))
        {
            throw new ArgumentException("AWS S3 bucket name must be specified.", nameof(BucketName));
        }

        if (string.IsNullOrEmpty(Region))
        {
            throw new ArgumentException("AWS S3 region must be specified.", nameof(Region));
        }

        if (string.IsNullOrEmpty(AccessKey))
        {
            throw new ArgumentException("AWS S3 access key ID must be specified.", nameof(AccessKey));
        }

        if (string.IsNullOrEmpty(SecretAccessKey))
        {
            throw new ArgumentException("AWS S3 secret access key must be specified.", nameof(SecretAccessKey));
        }
    }
}
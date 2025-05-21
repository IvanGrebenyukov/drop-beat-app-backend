using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropBeatAPI.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DropBeatAPI.Infrastructure.Services
{
    public class YandexStorageService : IYandexStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public YandexStorageService(IOptions<YandexStorageSettings> settings)
        {
            _bucketName = settings.Value.BucketName;

            _s3Client = new AmazonS3Client(settings.Value.AccessKey, settings.Value.SecretKey, new AmazonS3Config
            {
                ServiceURL = "https://storage.yandexcloud.net",
                ForcePathStyle = true
            });
        }
        
        public async Task<Stream?> GetFileAsync(string fileKey)
        {
            try
            {
                var response = await _s3Client.GetObjectAsync(_bucketName, fileKey);
                var memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (AmazonS3Exception)
            {
                return null;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName, string contentType, string folder)
        {
            using var stream = file.OpenReadStream();
            return await UploadFileAsync(stream, fileName, contentType, folder);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder)
        {
            var objectKey = $"{folder}/{Guid.NewGuid()}_{fileName}";

            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = objectKey,
                    InputStream = fileStream,
                    ContentType = contentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                await _s3Client.PutObjectAsync(putRequest);

                return $"https://{_bucketName}.storage.yandexcloud.net/{objectKey}";
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"Ошибка загрузки файла: {ex.Message}");
            }
        }

        public async Task<bool> DeleteFileAsync(string fileKey)
        {
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileKey
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"Ошибка удаления файла: {ex.Message}");
            }
        }
        
        public async Task<IEnumerable<string>> ListFilesAsync(string prefix)
        {
            var keys = new List<string>();
            string continuationToken = null;

            do
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = prefix,
                    ContinuationToken = continuationToken
                };

                var response = await _s3Client.ListObjectsV2Async(request);

                keys.AddRange(response.S3Objects.Select(o => o.Key));

                continuationToken = response.IsTruncated ? response.NextContinuationToken : null;

            } while (continuationToken != null);

            return keys;
        }
    }
}

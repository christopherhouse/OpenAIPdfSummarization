using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using OpenAIPdfSummarization.Models;

namespace OpenAIPdfSummarization.Functions.Activities
{
    public class StorePdfBlobActivity
    {
        private static readonly string _blobContainerName = Environment.GetEnvironmentVariable("blobContainer");
        private static readonly string _storageAccountName = Environment.GetEnvironmentVariable("storageAccontName");

        private static readonly Uri _blobContainerUri =
            new Uri($"https://{_storageAccountName}.blob.core.windows.net/{_blobContainerName}");

        private static readonly StorageSharedKeyCredential _storageCredential = new StorageSharedKeyCredential(
            Environment.GetEnvironmentVariable("storageAccountName"),
            Environment.GetEnvironmentVariable(("storageAccountKey")));

        private static readonly BlobContainerClient _blobContainerClient = new BlobContainerClient(_blobContainerUri, _storageCredential);

        public static async Task<Uri> StorePdfBlob([ActivityTrigger] FileData fileData, IBinder binder)
        {
            var fileName =
                $"{Path.GetFileNameWithoutExtension(fileData.FileName)}_{DateTime.UtcNow.ToString("yyyy-MM-dd-HHmmss")}.{Path.GetExtension(fileData.FileName)}";
            var outputBlob = new BlobAttribute($"{_blobContainerName}/{fileName}", FileAccess.Write);
            outputBlob.Connection = "storageConnectionString";
            await using var writer = binder.Bind<Stream>(outputBlob);
            await writer.WriteAsync(fileData.Data);

            var blobClient = new BlobClient(new Uri($"{_blobContainerUri}/{outputBlob.BlobPath}"));

            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _blobContainerName,
                BlobName = blobClient.Name,
                ExpiresOn = DateTime.UtcNow.AddHours(1),
                StartsOn = DateTime.UtcNow.AddMinutes(-5)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.List);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri;
        }
    }
}

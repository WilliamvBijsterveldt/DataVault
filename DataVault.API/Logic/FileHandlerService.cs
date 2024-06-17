using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using DataVault.API.Models;

namespace DataVault.API.Logic;

public class FileHandlerService
{
    private readonly string _storageAccount = "477074blob";
    private readonly string _key = "f9iwtPAaAwTRAjhosqCFeF6Pxf3B6kqThl6uMR4qYE9DN8Cu5ReRrbxh2OqfkgPQ57QJwDF1e8vS+AStu3zT0w==";
    private readonly BlobContainerClient _filesContainer;

    public FileHandlerService()
    {
        var credential = new StorageSharedKeyCredential(_storageAccount, _key);
        var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        _filesContainer = blobServiceClient.GetBlobContainerClient("477074files");
    }

    public async Task<IEnumerable<Blob>> ListAsync()
    {
        List<Blob> files = new List<Blob>();

        await foreach (var file in _filesContainer.GetBlobsAsync())
        {
            string uri = _filesContainer.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}/{name}";
            
            files.Add(new Blob
            {
                Uri = fullUri,
                Name = name,
                ContentType = file.Properties.ContentType
            });
        }

        return files;
    }

    public async Task<BlobResponse> UploadAsync(IFormFile blob)
    {
        BlobResponse response = new();
        BlobClient client = _filesContainer.GetBlobClient(blob.FileName);

        await using (Stream? data = blob.OpenReadStream())
        {
            await client.UploadAsync(data);
        }

        response.Status = $"File {blob.FileName} uploaded successfully";
        response.Error = false;
        response.Blob.Uri = client.Uri.AbsoluteUri;
        response.Blob.Name = client.Name;

        return response;
    }

    public async Task<Blob?> DownloadAsync(string blobFilename)
    {
        BlobClient file = _filesContainer.GetBlobClient(blobFilename);

        if (await file.ExistsAsync())
        {
            var data = await file.OpenReadAsync();
            Stream blobContent = data;

            var content = await file.DownloadContentAsync();

            string name = blobFilename;
            string contentType = content.Value.Details.ContentType;

            return new Blob
            {
                Content = blobContent,
                Name = name,
                ContentType = contentType
            };
        }

        return null;
    }

    public async Task<BlobResponse> DeleteAsync(string blobFilename)
    {
        BlobClient file = _filesContainer.GetBlobClient(blobFilename);

        await file.DeleteAsync();

        return new BlobResponse
        {
            Error = false,
            Status = $"File {blobFilename} has been successfully deleted"
        };
    }
}
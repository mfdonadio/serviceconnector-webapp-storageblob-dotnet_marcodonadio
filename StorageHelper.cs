using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Azure.Identity;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WebStorageSample
{
    public class StorageHelper
    {
        // Preserve binary data and reject existing blob names atomically.
        static public async Task UploadBlob(string containerEndpoint, string containerName, string blobName,
            Stream contents, TokenCredential credential, CancellationToken cancellationToken = default)
        {
            var blobContainerUri = new Uri(new Uri(containerEndpoint), containerName);
            var containerClient = new BlobContainerClient(blobContainerUri, credential);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(contents, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "application/octet-stream" },
                Conditions = new BlobRequestConditions { IfNoneMatch = Azure.ETag.All }
            }, cancellationToken);
        }

        static public async Task UploadBlob(string containerEndpoint, string containerName, string blobName, string blobContents, TokenCredential credential)
        {
            var blobContainerUri = new Uri(new Uri(containerEndpoint), containerName);
            BlobContainerClient containerClient = new BlobContainerClient(blobContainerUri, credential);

            try
            {
                // Create the container if it does not exist.
                await containerClient.CreateIfNotExistsAsync();

                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // Upload text to a new block blob.
                byte[] byteArray = Encoding.ASCII.GetBytes(blobContents);

                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        static public async Task<string> GetBlob(string containerEndpoint, string containerName, string blobName, TokenCredential credential)
        {
            var blobContainerUri = new Uri(new Uri(containerEndpoint), containerName);
            BlobContainerClient containerClient = new BlobContainerClient(blobContainerUri, credential);

            try
            {
                // Create the container if it does not exist.
                await containerClient.CreateIfNotExistsAsync();

                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                if (await blobClient.ExistsAsync())
                {
                    var response = await blobClient.DownloadAsync();
                    using (var streamReader = new StreamReader(response.Value.Content))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            var line = await streamReader.ReadLineAsync();
                            Console.WriteLine(line);
                            return line;
                        }
                    }
                }
                return "";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

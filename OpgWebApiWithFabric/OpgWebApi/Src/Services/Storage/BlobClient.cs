using Microsoft.WindowsAzure.Storage.Blob;
using OpgWebApi.Src.Statics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.Storage
{
    public class BlobClient
    {
        private AzureStorageClient _storageClient;

        private CloudBlobContainer container;

        public BlobClient(AzureStorageClient client)
        {
            _storageClient = client;
            container = client.BlobClient.GetContainerReference(Configurations.AzureStorageContainerName);
            container.CreateIfNotExistsAsync();
        }

        public async Task<CloudBlockBlob> UploadStreamAsImage(Stream  stream, string name)
        {
            var blockBlob = container.GetBlockBlobReference(name + ".jpeg");
            await blockBlob.UploadFromStreamAsync(stream);
            return blockBlob;
        }

    }
}

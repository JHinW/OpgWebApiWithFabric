using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.Storage
{
    public class AzureStorageClient
    {
        private CloudStorageAccount storageAccount;

        public CloudBlobClient BlobClient
        {
            get
            {
                return storageAccount.CreateCloudBlobClient();
            }
            private set { }
        }

        public CloudTableClient TableClient
        {
            get
            {
                return storageAccount.CreateCloudTableClient();
            }
            private set { }
        }

        public AzureStorageClient(string connectionStr)
        {
            storageAccount = CloudStorageAccount.Parse(connectionStr);
        }

    }
}

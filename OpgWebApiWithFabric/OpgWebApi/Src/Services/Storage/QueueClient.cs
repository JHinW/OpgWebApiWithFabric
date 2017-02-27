using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;

namespace OpgWebApi.Src.Services.Storage
{
    public class QueueClient
    {
        public CloudStorageAccount StorageAccount { get; set; }

        public CloudQueueClient CloudQueueClient { get; set; }

        public CloudQueue CloudQueue { get; set; }

        public QueueClient(string StorageConnectionString, string queueName)
        {
            StorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            CloudQueueClient = StorageAccount.CreateCloudQueueClient();
            CloudQueue = CloudQueueClient.GetQueueReference(queueName);
            CloudQueue.CreateIfNotExistsAsync();
        }

        public async void AddStrMessageAsync(string message)
        {
            await CloudQueue.AddMessageAsync(new CloudQueueMessage(message));
        }
        public async void AddObjectMessageAsync(Object o)
        {
            var sz = JsonConvert.SerializeObject(o);
            await CloudQueue.AddMessageAsync(new CloudQueueMessage(sz));
        }


    }
}

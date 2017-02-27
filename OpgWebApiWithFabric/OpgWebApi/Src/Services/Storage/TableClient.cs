using Microsoft.WindowsAzure.Storage.Table;
using OpgWebApi.Src.Entitys.Storage;
using OpgWebApi.Src.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Services.Storage
{
    public class TableClient
    {
        private AzureStorageClient _storageClient;

        private CloudTable _table;

        public TableClient(AzureStorageClient client)
        {
            _storageClient = client;
            _table = _storageClient.TableClient.GetTableReference(Configurations.AzureStorageTableName);
            _table.CreateIfNotExistsAsync();
        }

        public async Task InsertTableInfo(AppServerErrEntity entity)
        {
            var insertOperation = TableOperation.Insert(entity);
            await _table.ExecuteAsync(insertOperation);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using System;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace TREmberDeployable.Controllers
{
    public class IndexController : Controller
    {
        private string ApplicationKey;
        private CloudTable StorageTable;
        
        public IndexController()
        {
            var storageConnectionString = Environment.GetEnvironmentVariable("TABLESTORAGE_CONNECTIONSTRING");
            var storageTableName = Environment.GetEnvironmentVariable("TABLESTORAGE_TABLENAME");

            ApplicationKey = Environment.GetEnvironmentVariable("EMBER_APP_KEY");

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            StorageTable = tableClient.GetTableReference(storageTableName);
        }

        [HttpGet("{*url}")]
        public async Task<IActionResult> Get(string version = null)
        {
            await StorageTable.CreateIfNotExistsAsync();

            if(string.IsNullOrEmpty(version))
            {
                version = await GetCurrentVersion();
            }

            var content = await Find(version);

            return Content(content, "text/html");
        }

        private async Task<string> GetCurrentVersion()
        {
            return await Find($"{ApplicationKey}:current");
        }

        private async Task<string> Find(string key)
        {
            var result = (DynamicTableEntity)(await StorageTable.ExecuteAsync(TableOperation.Retrieve("manifest", key))).Result;

            return result.Properties["content"].StringValue;
        }
    }
}

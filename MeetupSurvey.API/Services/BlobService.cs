using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetupSurvey.API.Services
{
    public class BlobService : IFileService
    {
        readonly static CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=meetupsurveystorage;AccountKey=U0e99V7naX+SZbNaq0Jp5INmDY+NwO82YATq0A9tC0+CGRznMfyxDdZ9jZ7jB5pxbnRoTfXReJYa7CgOlRf9ww==;EndpointSuffix=core.windows.net");
        readonly static CloudBlobClient _blobClient = storageAccount.CreateCloudBlobClient();

        public static async Task CreateContainer(string name)
        {
            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(name);
            await cloudBlobContainer.CreateIfNotExistsAsync();

            // Set the permissions so the blobs are public. 
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await cloudBlobContainer.SetPermissionsAsync(permissions);
        }

        public static async Task<CloudBlockBlob> UploadBlob(string containerName, byte[] blob, string blobTitle)
        {
            var blobContainer = _blobClient.GetContainerReference(containerName);

            var blockBlob = blobContainer.GetBlockBlobReference(blobTitle);
            await blockBlob.UploadFromByteArrayAsync(blob, 0, blob.Length);

            return blockBlob;
        }

        public static async Task DeleteBlobs(string containerName, List<string> blobTitles)
        {
            var blobContainer = _blobClient.GetContainerReference(containerName);
            foreach(var blobTitle in blobTitles)
            {
                var blockBlob = blobContainer.GetBlockBlobReference(blobTitle);
                await blockBlob.DeleteIfExistsAsync();
            }
        }

        public static async Task DeleteBlob(string containerName, string blobTitle)
        {
            var blobContainer = _blobClient.GetContainerReference(containerName);
            var blockBlob = blobContainer.GetBlockBlobReference(blobTitle);
            await blockBlob.DeleteIfExistsAsync();
        }


        public static async Task<List<T>> DownloadBlob<T>(string containerName, string prefix = "", int? maxresultsPerQuery = null, BlobListingDetails blobListingDetails = BlobListingDetails.None) where T : ICloudBlob
        {
            var blobContainer = _blobClient.GetContainerReference(containerName);

            var blobList = new List<T>();
            BlobContinuationToken continuationToken = null;

            try
            {
                do
                {
                    var response = await blobContainer.ListBlobsSegmentedAsync(prefix, true, blobListingDetails, maxresultsPerQuery, continuationToken, null, null);

                    continuationToken = response?.ContinuationToken;

                    foreach (var blob in response?.Results?.OfType<T>())
                    {
                        blobList.Add(blob);
                    }

                } while (continuationToken != null);
            }
            catch (Exception e)
            {
                //Handle Exception
            }

            return blobList;
        }


    }
}

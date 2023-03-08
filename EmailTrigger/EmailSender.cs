using System;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace EmailTrigger
{
    public class EmailSender
    {
        [FunctionName("EmailSender")]
        public void Run([BlobTrigger("files/{name}", Connection = "DefaultEndpointsProtocol=https;AccountName=internshipstorage2002000;AccountKey=T1iuBTdtjnOk1CnWTHQS578P+AJmI8M5Ya2jg9X/V+Nme3XSq/zZBfBh3tFJsLUC+o+mHKRUiuO4+AStaeKsbg==;EndpointSuffix=core.windows.net")]Stream myBlob, string name, ILogger log)
        {
            /*BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=internshipstorage2002000;AccountKey=T1iuBTdtjnOk1CnWTHQS578P+AJmI8M5Ya2jg9X/V+Nme3XSq/zZBfBh3tFJsLUC+o+mHKRUiuO4+AStaeKsbg==;EndpointSuffix=core.windows.net");
            var blobContainerClient = blobServiceClient.GetBlobContainerClient("emails");
            var blobClient = blobContainerClient.GetBlobClient(name + ".txt");
            if(blobClient.Exists())
            {
                var response = blobClient.DownloadContent();
                using(var streamReader = new StreamReader(response.Value.Content.ToStream()))
                {
                    var email = streamReader.ReadToEnd().Trim();
                    log.LogInformation($"Email of client is: {email}");
                }
            }*/
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}

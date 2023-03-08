using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EmailSendTrigger
{
    [StorageAccount("BlobConnectionString")]
    public class EmailSendTriger
    {
        [FunctionName("EmailSendTriger")]
        public async Task Run([BlobTrigger("files/{name}")]Stream myBlob, string name, ILogger log)
        {
            var sendGridApiKey = "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";
            var client = new SendGridClient(sendGridApiKey);
            string emailStorageConnection = "DefaultEndpointsProtocol=https;AccountName=internshipstorage2002000;AccountKey=T1iuBTdtjnOk1CnWTHQS578P+AJmI8M5Ya2jg9X/V+Nme3XSq/zZBfBh3tFJsLUC+o+mHKRUiuO4+AStaeKsbg==;EndpointSuffix=core.windows.net";
            string emailContainerName = "emails";
            string email = "";
            var blobServiceClient = new BlobServiceClient(emailStorageConnection);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(emailContainerName);
            var blobClient = blobContainerClient.GetBlobClient(name + ".txt");
            using(var memoryStream = new MemoryStream())
            {
                blobClient.DownloadTo(memoryStream);
                memoryStream.Position = 0;
                using(var sr = new StreamReader(memoryStream))
                {
                    email = sr.ReadToEnd().Trim();
                }
            }
            var uri = Uri.EscapeUriString($"https://internshipstorage2002000.blob.core.windows.net/files/{name}");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("speshko@uwaterloo.ca", "Serhii Peshko"),
                Subject = "File uploading",
                PlainTextContent = $"Congratulations! Your file ${name} was uploaded successfully",
                HtmlContent = $"<h2>Congratulations!</h2><p>Your file <a href=\"{uri}\">{name}</a> was uploaded successfully</p>"
            };

            msg.AddTo(new EmailAddress(email));
            var response = await client.SendEmailAsync(msg);

            log.LogInformation(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong");
        }
    }
}

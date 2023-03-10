using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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
            
            string emailStorageConnection = Environment.GetEnvironmentVariable("EmailStorageConnection");
            string emailContainerName = "emails";
            string email = "";
            string apikey = Environment.GetEnvironmentVariable("SendGridApiKey");
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
            var sendGridClient = new SendGridClient(apikey);
            var message = new SendGridMessage()
            {
                From = new EmailAddress("speshko@uwaterloo.ca", "Serhii Peshko"),
                Subject = "File uploading",
                PlainTextContent = $"Congratulations! Your file ${name} was uploaded successfully",
                HtmlContent = $"<h2>Congratulations!</h2><p>Your file <a href=\"{uri}\">{name}</a> was uploaded successfully</p>"
            };

            message.AddTo(new EmailAddress(email));
            var response = await sendGridClient.SendEmailAsync(message);
            log.LogInformation(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong");
        }
    }
}

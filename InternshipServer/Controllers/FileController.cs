using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace InternshipServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileToBlob()
        {
            IFormFile file = Request.Form.Files[0];
            string email = Request.Form["email"];

            if(file.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                return new StatusCodeResult(StatusCodes.Status406NotAcceptable);
            }
            string connectionString = _configuration["BlobStorageConnectionString"];
            string containerName = "files";
            var blobContainerClient = new BlobContainerClient(connectionString, containerName);
            blobContainerClient.DeleteBlobIfExists(file.FileName);
            var blobClientFile = blobContainerClient.GetBlobClient(file.FileName);
            using(var stream = file.OpenReadStream())
            {
                await blobClientFile.UploadAsync(stream);
            }
            var blobContainerEmails = new BlobContainerClient(connectionString, "emails");
            blobContainerEmails.DeleteBlobIfExists(file.Name + ".txt");
            var blobClientEmail = blobContainerEmails.GetBlobClient(file.Name + ".txt");
            using(var writer = new StreamWriter(await blobClientEmail.OpenWriteAsync(true)))
            {
                await writer.WriteAsync(email);
            }
            return Ok();
        }
    }
}

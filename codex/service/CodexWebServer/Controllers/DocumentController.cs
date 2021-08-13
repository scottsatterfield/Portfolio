using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CodexWebServer.Controllers
{
    /// <summary>
    /// Web API controller for the book's text.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private const string DocumentPathKey = "DocumentPath";
        private const string DefaultDocumentFileNameKey = "DefaultDocumentFileName";
        private string Path { get; }
        private string Filename { get; }
        private string FullPath { get; }
        private readonly ILogger<DocumentController> _logger;

        /// <summary>
        /// Initialize the document controller.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="configuration">App Settings Configuration</param>
        public DocumentController(ILogger<DocumentController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Path = configuration[DocumentPathKey];
            Filename = configuration[DefaultDocumentFileNameKey];
            FullPath = System.IO.Path.Combine(Path, Filename);
            
            _logger.LogDebug($"Document Path: {Path}");
            _logger.LogDebug($"Document Filename: {Filename}");
            _logger.LogDebug($"Full Path: {FullPath}");
            
            try
            {
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Return the text of the book and associated metadata.
        /// </summary>
        /// <returns>Text and Metadata</returns>
        [HttpGet]
        public async Task<Document> Get()
        {
            // return an empty document
            if (!System.IO.File.Exists(FullPath))
            {
                _logger.LogInformation($"{FullPath} does not exist, returning empty document text");
                
                var now = DateTime.Now;
                return new Document
                {
                    Text = "",
                    CreationDate = now,
                    LastModifiedDate = now
                };
            }
            
            var text = await System.IO.File.ReadAllTextAsync(FullPath);
            var info = new FileInfo(FullPath);

            _logger.LogInformation($"{FullPath} exist, returning {text.Length} characters from document");
            
            return new Document()
            {
                Text = text,
                CreationDate = info.CreationTime,
                LastModifiedDate = info.LastWriteTime
            };
        }

        /// <summary>
        /// Create or overwrite the document.
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>HTTP Result of Request</returns>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(Path) || string.IsNullOrWhiteSpace(Filename) || !Directory.Exists(Path))
            {
                if (string.IsNullOrWhiteSpace(Path))
                    _logger.LogDebug($"file path {Path} is empty");
                
                if (string.IsNullOrWhiteSpace(Filename))
                    _logger.LogDebug($"file name is empty");

                if (!Directory.Exists(Path))
                    _logger.LogDebug($"path {Path} does not exist");
                
                return StatusCode(500); // 500 - internal server error
            }
                
            
            var isCreated = !System.IO.File.Exists(FullPath);
            _logger.LogInformation($"{FullPath} {(isCreated ? "will be created" : "will be overwritten")}");
            
            text = string.IsNullOrWhiteSpace(text) ? "" : text.Trim();
            await System.IO.File.WriteAllTextAsync(FullPath, text);
            _logger.LogInformation($"{text.Length} characters written to {FullPath}");

            if (isCreated)
                return Created("", Filename); // 201 - Created
            
            return Ok(); // 200 - OK
        }
    }
}
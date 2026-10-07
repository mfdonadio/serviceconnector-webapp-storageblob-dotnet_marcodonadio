using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Azure.Core;

namespace WebStorageSample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly TokenCredential _credential;

        public string DisplayWords { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, TokenCredential credential)
        {
            _logger = logger;
            _credential = credential;
        }

        public async Task OnGetAsync()
        {
            string content = string.Format("Hello Service Connector! UTC Now: {0}.", DateTimeOffset.UtcNow.ToString());

            await StorageHelper.UploadBlob(Environment.GetEnvironmentVariable(Const.ENDPOINT_ENV_KEY), Const.CONTAINER_NAME, Const.BLOB_NAME, content, _credential);
            DisplayWords = await StorageHelper.GetBlob(Environment.GetEnvironmentVariable(Const.ENDPOINT_ENV_KEY), Const.CONTAINER_NAME, Const.BLOB_NAME, _credential);
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using XmlExtract.Extentions;

namespace XmlExtract.Controllers
{
    [Route("api/extract")]
    [ApiController]
    public class ExtractController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> ReadPlainText()
        {
            string text;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {  
                text = await reader.ReadToEndAsync();
            }

            var textExtract = new TextExtract(text);
            var result = textExtract.ExtractXmlFromText();

            if (result.IsValid != true)
            {
              return BadRequest(result.Error);
            }

            return Ok(result.Content);
        }
    }
}

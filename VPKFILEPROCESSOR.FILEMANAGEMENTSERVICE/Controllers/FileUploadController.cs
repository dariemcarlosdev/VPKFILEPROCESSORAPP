using Microsoft.AspNetCore.Mvc;

namespace VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileUploadController : ControllerBase
    {
        public IActionResult Index()
        {
            //Leak implementation details
            return Ok("File Upload Controller");
        }
    }
}

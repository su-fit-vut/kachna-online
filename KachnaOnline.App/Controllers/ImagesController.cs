using System;
using System.Threading.Tasks;
using KachnaOnline.App.Extensions;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Facades;
using KachnaOnline.Dto.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KachnaOnline.App.Controllers
{
    [Route("images")]
    public class ImagesController : ControllerBase
    {
        private readonly ImagesFacade _facade;

        public ImagesController(ImagesFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// Returns an uploaded image with the given hash.
        /// </summary>
        /// <param name="md5Hash">An MD5 hash of the image.</param>
        /// <response code="200">The image.</response>
        /// <response code="404">The image does not exist.</response>
        [HttpGet("{md5Hash}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = ImageConstants.CacheMaxAge, Location = ResponseCacheLocation.Any)]
        [AllowAnonymous]
        public IActionResult GetImage(string md5Hash)
        {
            md5Hash = md5Hash.ToLowerInvariant();
            var (imagePath, mime) = _facade.GetImageActualPath(md5Hash);

            if (imagePath != null)
            {
                return this.PhysicalFile(imagePath, mime);
            }

            return this.NotFoundProblem();
        }

        /// <summary>
        /// Uploads an image.
        /// </summary>
        /// <remarks>
        /// If `md5Hash` is specified, it is first checked if the same image exists already. If it does,
        /// the image is not saved and 304 Not Modified is returned.
        /// </remarks>
        /// <param name="file">A JPEG image to upload.</param>
        /// <param name="md5Hash">An MD5 hash of the uploaded image.</param>
        /// <response code="201">The image was saved. A relative URL and its MD5 hash are returned.</response>
        /// <response code="409">An image with the same hash already exists or the value of `md5Hash` does not correspond with the uploaded image.</response>
        /// <response code="415">The provided file is not a JPEG image or its content type is not set to image/jpeg.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ImageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ImageDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Authorize(AuthConstants.AnyManagerPolicy)]
        public async Task<ActionResult<ImageDto>> UploadImage(IFormFile file, string md5Hash)
        {
            if (file is null)
                return this.BadRequest();

            if (file.ContentType is not ("image/jpeg" or "image/jpg" or "image/png"))
            {
                return this.Problem(statusCode: StatusCodes.Status415UnsupportedMediaType,
                    title: "Invalid content type", detail: "Only JPEG and PNG images are accepted.");
            }

            if (!string.IsNullOrEmpty(md5Hash))
            {
                var (actualPath, _) = _facade.GetImageActualPath(md5Hash);
                if (actualPath != null)
                {
                    return this.Conflict(new ImageDto()
                    {
                        Url = this.Url.Content($"~{ImageConstants.ImageUrlPath}/{md5Hash}"),
                        Hash = md5Hash,
                        Exists = true
                    });
                }
            }

            var result = await _facade.UploadImage(file);
            result.Url = this.Url.Content("~" + result.Url);

            if (result.Exists || (!string.IsNullOrEmpty(md5Hash) &&
                                  !md5Hash.Equals(result.Hash, StringComparison.InvariantCultureIgnoreCase)))
            {
                return this.Conflict(result);
            }
            else
            {
                return this.Created(this.Url.Action("GetImage", "Images",
                    new { md5Hash = result.Hash }) ?? throw new InvalidOperationException(), result);
            }
        }
    }
}

// ImagesController.cs
// Author: Ondřej Ondryáš

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
    [Authorize(AuthConstants.AnyManagerPolicy)]
    [Route("images")]
    public class ImagesController : ControllerBase
    {
        private readonly ImagesFacade _facade;

        public ImagesController(ImagesFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// Returns the URL of an uploaded image with the given hash.
        /// </summary>
        /// <param name="md5Hash">An MD5 hash of the image.</param>
        /// <response code="200">A relative URL of the image.</response>
        /// <response code="404">The image does not exist.</response>
        [HttpGet("{md5Hash}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = ImageConstants.CacheMaxAge, Location = ResponseCacheLocation.Any)]
        public ActionResult<string> GetImageUrl(string md5Hash)
        {
            md5Hash = md5Hash.ToLowerInvariant();
            var imagePath = _facade.GetImagePath(md5Hash);

            if (System.IO.File.Exists(imagePath))
            {
                return this.Url.Content($"~{ImageConstants.ImageUrlPath}/{md5Hash}.jpg");
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
        public async Task<ActionResult<ImageDto>> UploadImage(IFormFile file, string md5Hash)
        {
            if (file is null)
                return this.BadRequest();

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/jpg")
            {
                return this.Problem(statusCode: StatusCodes.Status415UnsupportedMediaType,
                    title: "Invalid content type", detail: "Only JPEG images (image/jpeg) are accepted.");
            }

            if (!string.IsNullOrEmpty(md5Hash))
            {
                md5Hash = md5Hash.ToLowerInvariant();
                var imagePath = _facade.GetImagePath(md5Hash);

                if (System.IO.File.Exists(imagePath))
                {
                    return this.Conflict(new ImageDto()
                    {
                        Url = this.Url.Content($"~{ImageConstants.ImageUrlPath}/{md5Hash}.jpg"),
                        Hash = md5Hash
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
                return this.Created(this.Url.Action("GetImageUrl", "Images",
                    new { md5Hash = result.Hash }) ?? throw new InvalidOperationException(), result);
            }
        }
    }
}

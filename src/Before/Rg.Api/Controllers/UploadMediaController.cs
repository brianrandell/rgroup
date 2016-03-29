using Rg.ApiTypes;
using Rg.ServiceCore.DbModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.IO;
using System.Net;
using Rg.ServiceCore.Operations;

namespace Rg.Api.Controllers
{
    /// <summary>
    /// Handles image uploads.
    /// </summary>
    //[Authorize]
    public class UploadMediaController : ApiControllerBase
    {
        private const string UserImagePath = "/userimages";

        /// <summary>
        /// Accepts images in the usual form multi file upload format.
        /// </summary>
        /// <returns>
        /// Provides the ids of the user images created.
        /// </returns>
        public async Task<MediaUploadResults> PostAsync()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string folderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                bool createdFolder = false;
                try
                {
                    Directory.CreateDirectory(folderPath);
                    createdFolder = true;

                    UserInfo userEntity = await GetUserInfoAsync();

                    var r = new MultipartFormDataStreamProvider(folderPath);
                    await Request.Content.ReadAsMultipartAsync(r);
                    IEnumerable<UserMediaOperations.UploadedMedia> items =
                        from file in r.FileData
                        select new UserMediaOperations.UploadedMedia
                        {
                            OriginalName = file.Headers.ContentDisposition.FileName,
                            LocalFileName = file.LocalFileName
                        };

                    return await UserMediaOperations.StoreMediaAsync(
                        DbContext, userEntity, items);

                }
                finally
                {
                    if (createdFolder)
                    {
                        Directory.Delete(folderPath, true);
                    }
                }
            }
            else
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                throw new HttpResponseException(response);
            }
        }
    }
}
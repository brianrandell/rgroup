using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rg.ApiTypes
{
    public class MediaUploadInProgress
    {
        /// <summary>
        /// The URL to which to POST the image being uploaded.
        /// </summary>
        public string UploadUrl { get; set; }

        /// <summary>
        /// The URL to which to POST a <see cref="CompleteMediaUpload"/>
        /// once the image file has been uploaded
        /// </summary>
        public string CompletionUrl { get; set; }
    }
}

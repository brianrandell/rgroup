namespace Rg.ServiceCore.DbModel
{
    public enum UserMediaState
    {
        /// <summary>
        /// The file has been uploaded and stored, but is not yet in use.
        /// </summary>
        UploadedButUnused,

        /// <summary>
        /// The file has been uploaded and stored, and is now in use in an album or
        /// as an avatar or banner.
        /// </summary>
        InUse
    }
}

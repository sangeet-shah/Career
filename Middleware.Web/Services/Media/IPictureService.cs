using Career.Data.Domains.Media;
using Career.Data.Repository;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Media;

/// <summary>
/// Picture service interface
/// </summary>
public interface IPictureService
{
    /// <summary>
    /// Gets a picture
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <returns>Picture</returns>
    Task<Picture> GetPictureByIdAsync(int pictureId);

    /// <summary>
    /// Get a picture URL
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <returns>Picture URL</returns>
    Task<string> GetPictureUrlAsync(int pictureId,
        int targetSize = 0,
        bool showDefaultPicture = true,
        string storeLocation = null,
        PictureType defaultPictureType = PictureType.Entity,
        bool ignoreImageKit = false);

    /// <summary>
    /// Get a picture URL caching
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <returns>Picture URL</returns>
    Task<string> GetPictureUrlCachingAsync(int pictureId,
        int targetSize = 0,
        bool showDefaultPicture = true);

    /// <summary>
    /// Gets the loaded picture binary depending on picture storage settings
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>Picture binary</returns>
    Task<byte[]> LoadPictureBinaryAsync(Picture picture);

    /// <summary>
    /// Gets or sets a value indicating whether the images should be stored in data base.
    /// </summary>
    Task<bool> IsStoreInDbAsync();

    /// <summary>
    /// Gets a collection of pictures
    /// </summary>
    /// <param name="pageIndex">Current page</param>
    /// <param name="pageSize">Items on each page</param>
    /// <returns>Paged list of pictures</returns>
    Task<IPagedList<Picture>> GetPicturesAsync(int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Get a picture URL
    /// </summary>
    /// <param name="picture">Picture instance</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <returns>Picture URL</returns>
    Task<string> GetPictureUrlAsync(Picture picture,
        int targetSize = 0,
        bool showDefaultPicture = true,
        string storeLocation = null,
        PictureType defaultPictureType = PictureType.Entity,
        bool ignoreImageKit = false);

    Task<Banner> GetActiveBannerAsync(int locationId = 0);

    /// <summary>
    /// Inserts a picture
    /// </summary>
    /// <param name="formFile">Form file</param>
    /// <param name="defaultFileName">File name which will be use if IFormFile.FileName not present</param>
    /// <param name="virtualPath">Virtual path</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    Task<Picture> InsertPictureAsync(IFormFile formFile, string defaultFileName = "", string virtualPath = "");

    /// <summary>
    /// Get product picture binary by picture identifier
    /// </summary>
    /// <param name="pictureId">The picture identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    Task<PictureBinary> GetPictureBinaryByPictureIdAsync(int pictureId);
}

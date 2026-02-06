using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains;
using Career.Data.Domains.CDN;
using Career.Data.Domains.Common;
using Career.Data.Domains.Locations;
using Career.Data.Domains.Media;
using Career.Data.Repository;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Logs;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Middleware.Web.Data;
using Middleware.Web.Helpers;

namespace Middleware.Web.Services.Media;

/// <summary>
/// Picture service
/// </summary>
public class PictureService : IPictureService
{
    #region Fields

    private const string PictureTable = "Picture";
    private const string PictureBinaryTable = "PictureBinary";
    private const string GenericAttributeTable = "GenericAttribute";
    private const string BannerTableForMedia = "Banner";

    private readonly DbConnectionFactory _db;
    private readonly ISettingService _settingService;
    private readonly ICommonService _commonService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly INopFileProvider _nopFileProvider;
    private readonly ILogService _logService;
    private readonly IStoreService _storeService;

    private readonly static string bannerPhysicalStoreMap = "BannerPhysicalStoreMap";

    #endregion

    #region Ctor

    public PictureService(DbConnectionFactory db,
        ISettingService settingService,
        ICommonService commonService,
        IStaticCacheManager staticCacheManager,
        INopFileProvider nopFileProvider,
        ILogService logService,
        IStoreService storeService)
    {
        _db = db;
        _settingService = settingService;
        _commonService = commonService;
        _staticCacheManager = staticCacheManager;
        _nopFileProvider = nopFileProvider;
        _logService = logService;
        _storeService = storeService;
    }

    #endregion

    #region methods

    /// <summary>
    /// Returns the file extension from mime type.
    /// </summary>
    /// <param name="mimeType">Mime type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the file extension
    /// </returns>
    public Task<string> GetFileExtensionFromMimeTypeAsync(string mimeType)
    {
        if (mimeType == null)
            return Task.FromResult<string>(null);

        var parts = mimeType.Split('/');
        var lastPart = parts[^1];
        lastPart = lastPart switch
        {
            "pjpeg" => "jpg",
            "jpeg" => "jpeg",
            "bmp" => "bmp",
            "gif" => "gif",
            "x-png" or "png" => "png",
            "tiff" => "tiff",
            "x-icon" => "ico",
            "webp" => "webp",
            "svg+xml" => "svg",
            _ => lastPart,
        };
        return Task.FromResult(lastPart);
    }

    /// <summary>
    /// Gets a picture
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <returns>Picture</returns>
    public async Task<Picture> GetPictureByIdAsync(int pictureId)
    {
        if (pictureId == 0)
            return null;

        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{PictureTable}] WHERE Id = @Id";
        return await conn.QueryFirstOrDefaultAsync<Picture>(sql, new { Id = pictureId });
    }

    /// <summary>
    /// Get a picture URL
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <returns>Picture URL</returns>
    public async Task<string> GetPictureUrlAsync(int pictureId,
    int targetSize = 0,
    bool showDefaultPicture = true,
    string storeLocation = null,
    PictureType defaultPictureType = PictureType.Entity, bool ignoreImageKit = false)
    {
        var picture = await GetPictureByIdAsync(pictureId);
        return await GetPictureUrlAsync(picture, targetSize, showDefaultPicture);
    }

    /// <summary>
    /// Get a picture URL caching
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <returns>Picture URL</returns>
    public async Task<string> GetPictureUrlCachingAsync(int pictureId,
    int targetSize = 0,
    bool showDefaultPicture = true)
    {
        //cacheable copy
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.PictureCacheKey, pictureId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var picture = await GetPictureByIdAsync(pictureId);
            return await GetPictureUrlAsync(picture, targetSize, showDefaultPicture);
        });

    }

    public async Task<string> GetPictureUrlAsync(Picture picture, int targetSize = 0, bool showDefaultPicture = true)
    {
        if (picture != null)
        {
            var imagekitUrl = await GetImageKitUrlAsync();
            if (!string.IsNullOrEmpty(imagekitUrl))
            {
                var multiple_Thumb_Directories_Length = NopMediaDefaults.MultipleThumbDirectoriesLength;

                // prepare thumb file name
                var seoFileName = picture.SeoFilename; // = GetPictureSeName(picture.SeoFilename); //just for sure
                var lastPart = await GetFileExtensionFromMimeTypeAsync(picture.MimeType);
                var targetParam = string.Empty;

                var thumbFileName = !string.IsNullOrEmpty(seoFileName)
                    ? $"{picture.Id:0000000}_{seoFileName}.{lastPart}"
                    : $"{picture.Id:0000000}_0.{lastPart}";

                if (targetSize != 0)
                    targetParam = "?tr=w-" + targetSize;

                var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
                var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>(storeId);
                if (mediaSettings.MultipleThumbDirectories)
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(thumbFileName);
                    if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > multiple_Thumb_Directories_Length)
                    {
                        var subDirectoryName = fileNameWithoutExtension[0..multiple_Thumb_Directories_Length];
                        return (imagekitUrl.TrimEnd('/') + "/images/" + (string.IsNullOrEmpty(seoFileName) ? "" : "thumbs/") + $"{subDirectoryName}/{thumbFileName}{targetParam}");
                    }
                }
                else
                    return (imagekitUrl.TrimEnd('/') + "/images/" + (string.IsNullOrEmpty(seoFileName) ? "" : "thumbs/") + $"{thumbFileName}{targetParam}");
            }

        }

        if (!showDefaultPicture)
            return string.Empty;

        return "/images/default-image.png";
    }

    private async Task<string> GetImageKitUrlAsync()
    {
        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var cndSettings = await _settingService.LoadSettingAsync<NopAdvanceCDNSettings>(storeId);
        if (cndSettings == null || string.IsNullOrEmpty(cndSettings.CDNImageUrl))
            return string.Empty;

        return cndSettings.CDNImageUrl;
    }

    /// <summary>
    /// Gets the loaded picture binary depending on picture storage settings
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <returns>Picture binary</returns>
    public async Task<byte[]> LoadPictureBinaryAsync(Picture picture)
    {
        return await LoadPictureBinaryAsync(picture, await IsStoreInDbAsync());
    }

    /// <summary>
    /// Gets the loaded picture binary depending on picture storage settings
    /// </summary>
    /// <param name="picture">Picture</param>
    /// <param name="fromDb">Load from database; otherwise, from file system</param>
    /// <returns>Picture binary</returns>
    protected async Task<byte[]> LoadPictureBinaryAsync(Picture picture, bool fromDb)
    {
        if (picture == null)
            throw new ArgumentNullException(nameof(picture));

        var result = fromDb
            ? picture.PictureBinary?.BinaryData ?? new byte[0]
            : await LoadPictureFromFileAsync(picture.Id, picture.MimeType);

        return result;
    }

    /// <summary>
    /// Loads a picture from file
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="mimeType">MIME type</param>
    /// <returns>Picture binary</returns>
    protected async Task<byte[]> LoadPictureFromFileAsync(int pictureId, string mimeType)
    {
        var lastPart = GetFileExtensionFromMimeType(mimeType);
        var fileName = $"{pictureId:0000000}_0.{lastPart}";
        var filePath = GetPictureLocalPath(fileName);

        return await _nopFileProvider.ReadAllBytesAsync(filePath);
    }

    /// <summary>
    /// Returns the file extension from mime type.
    /// </summary>
    /// <param name="mimeType">Mime type</param>
    /// <returns>File extension</returns>
    protected string GetFileExtensionFromMimeType(string mimeType)
    {
        if (mimeType == null)
            return null;

        var parts = mimeType.Split('/');
        var lastPart = parts[^1];
        switch (lastPart)
        {
            case "pjpeg":
                lastPart = "jpg";
                break;
            case "x-png":
                lastPart = "png";
                break;
            case "x-icon":
                lastPart = "ico";
                break;
        }

        return lastPart;
    }

    /// <summary>
    /// Get picture local path. Used when images stored on file system (not in the database)
    /// </summary>
    /// <param name="fileName">Filename</param>
    /// <returns>Local picture path</returns>
    protected string GetPictureLocalPath(string fileName)
    {
        return _nopFileProvider.GetAbsolutePath("images", fileName);
    }

    /// <summary>
    /// Delete a picture on file system
    /// </summary>
    /// <param name="picture">Picture</param>
    protected void DeletePictureOnFileSystem(Picture picture)
    {
        if (picture == null)
            throw new ArgumentNullException(nameof(picture));

        var lastPart = GetFileExtensionFromMimeType(picture.MimeType);
        var fileName = $"{picture.Id:0000000}_0.{lastPart}";
        var filePath = GetPictureLocalPath(fileName);
        _nopFileProvider.DeleteFile(filePath);
    }

    /// <summary>
    /// Save picture on file system
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="pictureBinary">Picture binary</param>
    /// <param name="mimeType">MIME type</param>
    protected async Task SavePictureInFileAsync(int pictureId, byte[] pictureBinary, string mimeType)
    {
        var lastPart = GetFileExtensionFromMimeType(mimeType);
        var fileName = $"{pictureId:0000000}_0.{lastPart}";
        await _nopFileProvider.WriteAllBytesAsync(GetPictureLocalPath(fileName), pictureBinary);
    }

    /// <summary>
    /// Gets the MIME type from the file name
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    protected string GetMimeTypeFromFileName(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }

    /// <summary>
    /// Gets a value indicating whether the images should be stored in data base.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<bool> IsStoreInDbAsync()
    {
        return await _settingService.GetSettingByKeyAsync<bool>("Media.Images.StoreInDB", true);
    }

    /// <summary>
    /// Gets a collection of pictures
    /// </summary>
    /// <param name="pageIndex">Current page</param>
    /// <param name="pageSize">Items on each page</param>
    /// <returns>Paged list of pictures</returns>
    public async Task<IPagedList<Picture>> GetPicturesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
        using var conn = _db.CreateNop();
        pageSize = Math.Max(pageSize, 1);
        var totalCount = await conn.ExecuteScalarAsync<int>($"SELECT COUNT(1) FROM [{PictureTable}]");
        var offset = pageIndex * pageSize;
        var sql = $"SELECT * FROM [{PictureTable}] ORDER BY Id DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        var list = (await conn.QueryAsync<Picture>(sql, new { Offset = offset, PageSize = pageSize })).AsList();
        return new PagedList<Picture>(list, pageIndex, pageSize, totalCount);
    }

    /// <summary>
    /// Gets the default picture URL
    /// </summary>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <returns>Picture URL</returns>
    public async Task<string> GetDefaultPictureUrlAsync(int targetSize = 0,
        PictureType defaultPictureType = PictureType.Entity,
        string storeLocation = null)
    {
        string defaultImageFileName;
        switch (defaultPictureType)
        {
            case PictureType.Avatar:
                defaultImageFileName = await _settingService.GetSettingByKeyAsync("Media.Customer.DefaultAvatarImageName", NopMediaDefaults.DefaultAvatarFileName);
                break;
            case PictureType.Entity:
            default:
                defaultImageFileName = await _settingService.GetSettingByKeyAsync("Media.DefaultImageName", NopMediaDefaults.DefaultImageFileName);
                break;
        }


        var filePath = GetPictureLocalPath(defaultImageFileName);
        if (!_nopFileProvider.FileExists(filePath))
        {
            return string.Empty;
        }

        if (targetSize == 0)
        {
            var url = (!string.IsNullOrEmpty(storeLocation)
                             ? storeLocation
                             : "pending") //_webHelper.GetStoreLocation())
                             + "images/" + defaultImageFileName;
            return url;
        }
        else
        {
            var fileExtension = _nopFileProvider.GetFileExtension(filePath);
            var thumbFileName = $"{_nopFileProvider.GetFileNameWithoutExtension(filePath)}_{targetSize}{fileExtension}";
            var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);
            if (!GeneratedThumbExists(thumbFilePath, thumbFileName))
            {                
                using var mutex = new Mutex(false, thumbFileName);
                mutex.WaitOne();
                try
                {
                    using var image = SKBitmap.Decode(filePath);
                    var codec = SKCodec.Create(filePath);
                    var format = codec.EncodedFormat;
                    var pictureBinary = ImageResize(image, format, targetSize);
                    var mimeType = GetMimeTypeFromFileName(thumbFileName);
                    await SaveThumbAsync(thumbFilePath, thumbFileName, mimeType, pictureBinary);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }

            var url = await GetThumbUrlAsync(thumbFileName, storeLocation);
            return url;
        }

    }

    /// <summary>
    /// Get a picture URL
    /// </summary>
    /// <param name="picture">Picture instance</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <param name="defaultPictureType">Default picture type</param>
    /// <returns>Picture URL</returns>
    public async Task<string> GetPictureUrlAsync(Picture picture,
        int targetSize = 0,
        bool showDefaultPicture = true,
        string storeLocation = null,
        PictureType defaultPictureType = PictureType.Entity, bool ignoreImageKit = false)
    {
        var url = string.Empty;
        byte[] pictureBinary = null;
        if (picture != null)
            pictureBinary = await LoadPictureBinaryAsync(picture);
        if (picture == null || pictureBinary == null || pictureBinary.Length <= 0)
        {
            if (showDefaultPicture)
            {
                url = await GetDefaultPictureUrlAsync(targetSize, defaultPictureType, storeLocation);
            }

            return url;
        }

        // load images from imagekit

        var seoFileName = picture.SeoFilename; // = GetPictureSeName(picture.SeoFilename); //just for sure

        var lastPart = GetFileExtensionFromMimeType(picture.MimeType);
        string thumbFileName;
        if (targetSize == 0)
        {
            thumbFileName = !string.IsNullOrEmpty(seoFileName)
                ? $"{picture.Id:0000000}_{seoFileName}.{lastPart}"
                : $"{picture.Id:0000000}.{lastPart}";
        }
        else
        {
            thumbFileName = !string.IsNullOrEmpty(seoFileName)
                ? $"{picture.Id:0000000}_{seoFileName}_{targetSize}.{lastPart}"
                : $"{picture.Id:0000000}_{targetSize}.{lastPart}";
        }

        //var thumbFilePath = await GetThumbLocalPathAsync(thumbFileName);

        ////the named mutex helps to avoid creating the same files in different threads,
        ////and does not decrease performance significantly, because the code is blocked only for the specific file.
        //using (var mutex = new Mutex(false, thumbFileName))
        //{
        //    if (!GeneratedThumbExists(thumbFilePath, thumbFileName))
        //    {
        //        mutex.WaitOne();

        //        //check, if the file was created, while we were waiting for the release of the mutex.
        //        if (!GeneratedThumbExists(thumbFilePath, thumbFileName))
        //        {
        //            byte[] pictureBinaryResized;
        //            if (targetSize != 0 && pictureBinary != null && pictureBinary.Count() > 0)
        //            {
        //                //resizing required
        //                using (var image = Image.Load(pictureBinary, out var imageFormat))
        //                {
        //                    image.Mutate(imageProcess => imageProcess.Resize(new ResizeOptions
        //                    {
        //                        Mode = ResizeMode.Max,
        //                        Size = CalculateDimensions(image.Size(), targetSize)
        //                    }));

        //                    pictureBinaryResized = await EncodeImageAsync(image, imageFormat);
        //                }
        //            }
        //            else
        //            {
        //                //create a copy of pictureBinary
        //                pictureBinaryResized = pictureBinary.ToArray();
        //            }

        //            await SaveThumbAsync(thumbFilePath, thumbFileName, picture.MimeType, pictureBinaryResized);
        //        }

        //        mutex.ReleaseMutex();
        //    }
        //}

        url = await GetThumbUrlAsync(thumbFileName, storeLocation);

        return url;
    }

    /// <summary>
    /// Get a picture local path
    /// </summary>
    /// <param name="picture">Picture instance</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
    /// <returns></returns>
    public async Task<string> GetThumbLocalPathAsync(Picture picture, int targetSize = 0, bool showDefaultPicture = true, bool ignoreImageKit = false)
    {
        var url = await GetPictureUrlAsync(picture, targetSize, showDefaultPicture, ignoreImageKit: ignoreImageKit);
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        return await GetThumbLocalPathAsync(_nopFileProvider.GetFileName(url));
    }

    /// <summary>
    /// Get picture (thumb) local path
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <returns>Local picture thumb path</returns>
    protected async Task<string> GetThumbLocalPathAsync(string thumbFileName)
    {
        var thumbsDirectoryPath = _nopFileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath);
        var _mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>();

        if (_mediaSettings.MultipleThumbDirectories)
        {
            //get the first two letters of the file name
            var fileNameWithoutExtension = _nopFileProvider.GetFileNameWithoutExtension(thumbFileName);
            if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > NopMediaDefaults.MultipleThumbDirectoriesLength)
            {
                var subDirectoryName = fileNameWithoutExtension.Substring(0, NopMediaDefaults.MultipleThumbDirectoriesLength);
                thumbsDirectoryPath = _nopFileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath, subDirectoryName);
                _nopFileProvider.CreateDirectory(thumbsDirectoryPath);
            }
        }

        var thumbFilePath = _nopFileProvider.Combine(thumbsDirectoryPath, thumbFileName);
        return thumbFilePath;
    }

    /// <summary>
    /// Get a value indicating whether some file (thumb) already exists
    /// </summary>
    /// <param name="thumbFilePath">Thumb file path</param>
    /// <param name="thumbFileName">Thumb file name</param>
    /// <returns>Result</returns>
    protected bool GeneratedThumbExists(string thumbFilePath, string thumbFileName)
    {
        return _nopFileProvider.FileExists(thumbFilePath);
    }

    /// <summary>
    /// Calculates picture dimensions whilst maintaining aspect
    /// </summary>
    /// <param name="originalSize">The original picture size</param>
    /// <param name="targetSize">The target picture size (longest side)</param>
    /// <param name="resizeType">Resize type</param>
    /// <param name="ensureSizePositive">A value indicating whether we should ensure that size values are positive</param>
    /// <returns></returns>
    //protected Size CalculateDimensions(Size originalSize, int targetSize,
    //    ResizeType resizeType = ResizeType.LongestSide, bool ensureSizePositive = true)
    //{
    //    float width, height;

    //    switch (resizeType)
    //    {
    //        case ResizeType.LongestSide:
    //            if (originalSize.Height > originalSize.Width)
    //            {
    //                // portrait
    //                width = originalSize.Width * (targetSize / (float)originalSize.Height);
    //                height = targetSize;
    //            }
    //            else
    //            {
    //                // landscape or square
    //                width = targetSize;
    //                height = originalSize.Height * (targetSize / (float)originalSize.Width);
    //            }

    //            break;
    //        case ResizeType.Width:
    //            width = targetSize;
    //            height = originalSize.Height * (targetSize / (float)originalSize.Width);
    //            break;
    //        case ResizeType.Height:
    //            width = originalSize.Width * (targetSize / (float)originalSize.Height);
    //            height = targetSize;
    //            break;
    //        default:
    //            throw new Exception("Not supported ResizeType");
    //    }

    //    if (!ensureSizePositive)
    //        return new Size((int)Math.Round(width), (int)Math.Round(height));

    //    if (width < 1)
    //        width = 1;
    //    if (height < 1)
    //        height = 1;

    //    //we invoke Math.Round to ensure that no white background is rendered - https://www.nopcommerce.com/boards/t/40616/image-resizing-bug.aspx
    //    return new Size((int)Math.Round(width), (int)Math.Round(height));
    //}

    /// <summary>
    /// Encode the image into a byte array in accordance with the specified image format
    /// </summary>
    /// <typeparam name="T">Pixel data type</typeparam>
    /// <param name="image">Image data</param>
    /// <param name="imageFormat">Image format</param>
    /// <param name="quality">Quality index that will be used to encode the image</param>
    /// <returns>Image binary data</returns>
    //protected async Task<byte[]> EncodeImageAsync<T>(Image<T> image, IImageFormat imageFormat, int? quality = null) where T : struct, IPixel<T>
    //{
    //    using (var stream = new MemoryStream())
    //    {
    //        var imageEncoder = SixLabors.ImageSharp.Configuration.Default.ImageFormatsManager.FindEncoder(imageFormat);
    //        switch (imageEncoder)
    //        {
    //            case JpegEncoder jpegEncoder:
    //                var _mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>();
    //                jpegEncoder.IgnoreMetadata = true;
    //                jpegEncoder.Quality = quality ?? _mediaSettings.DefaultImageQuality;
    //                jpegEncoder.Encode(image, stream);
    //                break;

    //            case PngEncoder pngEncoder:
    //                pngEncoder.ColorType = PngColorType.RgbWithAlpha;
    //                pngEncoder.Encode(image, stream);
    //                break;

    //            case BmpEncoder bmpEncoder:
    //                bmpEncoder.BitsPerPixel = BmpBitsPerPixel.Pixel32;
    //                bmpEncoder.Encode(image, stream);
    //                break;

    //            case GifEncoder gifEncoder:
    //                gifEncoder.IgnoreMetadata = true;
    //                gifEncoder.Encode(image, stream);
    //                break;

    //            default:
    //                imageEncoder.Encode(image, stream);
    //                break;
    //        }

    //        return stream.ToArray();
    //    }
    //}

    /// <summary>
    /// Save a value indicating whether some file (thumb) already exists
    /// </summary>
    /// <param name="thumbFilePath">Thumb file path</param>
    /// <param name="thumbFileName">Thumb file name</param>
    /// <param name="mimeType">MIME type</param>
    /// <param name="binary">Picture binary</param>
    protected async Task SaveThumbAsync(string thumbFilePath, string thumbFileName, string mimeType, byte[] binary)
    {
        //ensure \thumb directory exists
        var thumbsDirectoryPath = _nopFileProvider.GetAbsolutePath(NopMediaDefaults.ImageThumbsPath);
        _nopFileProvider.CreateDirectory(thumbsDirectoryPath);

        //save
        await _nopFileProvider.WriteAllBytesAsync(thumbFilePath, binary);
    }

    /// <summary>
    /// Get picture (thumb) URL 
    /// </summary>
    /// <param name="thumbFileName">Filename</param>
    /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
    /// <returns>Local picture thumb path</returns>
    protected async Task<string> GetThumbUrlAsync(string thumbFileName, string storeLocation = null)
    {
        storeLocation = !string.IsNullOrEmpty(storeLocation)
                                ? storeLocation
                                : "pending"; //_webHelper.GetStoreLocation();
        var url = storeLocation + "images/thumbs/";
        var _mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>();
        if (_mediaSettings.MultipleThumbDirectories)
        {
            //get the first two letters of the file name
            var fileNameWithoutExtension = _nopFileProvider.GetFileNameWithoutExtension(thumbFileName);
            if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > NopMediaDefaults.MultipleThumbDirectoriesLength)
            {
                var subDirectoryName = fileNameWithoutExtension.Substring(0, NopMediaDefaults.MultipleThumbDirectoriesLength);
                url = url + subDirectoryName + "/";
            }
        }

        url = url + thumbFileName;
        return url;
    }

    public async Task<Banner> GetActiveBannerAsync(int locationId = 0)
    {
        IList<GenericAttribute> bannerPhysicalStoreMapping;
        using (var conn = _db.CreateNop())
        {
            var sql = $"SELECT * FROM [{GenericAttributeTable}] WHERE KeyGroup = @KeyGroup AND [Key] = @Key";
            bannerPhysicalStoreMapping = (await conn.QueryAsync<GenericAttribute>(sql, new { KeyGroup = nameof(Banner), Key = bannerPhysicalStoreMap })).AsList();
        }
        var bannerPhysicalStoreMappings = new List<BannerPhysicalStoreMap>();
        if (bannerPhysicalStoreMapping != null && bannerPhysicalStoreMapping.Count != 0)
        {
            for (int i = 0; i < bannerPhysicalStoreMapping.Count; i++)
            {
                if (bannerPhysicalStoreMapping[i].Value == null || string.IsNullOrEmpty(bannerPhysicalStoreMapping[i].Value))
                    continue;
                int[] physicalStoreIds = bannerPhysicalStoreMapping[i].Value.Split(',').Select(int.Parse).ToArray();
                for (int j = 0; j < physicalStoreIds.Length; j++)
                    bannerPhysicalStoreMappings.Add(new BannerPhysicalStoreMap { BannerId = bannerPhysicalStoreMapping[i].EntityId, PhysicalStoreLocationId = physicalStoreIds[j] });
            }
        }

        var dateTimeNow = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
        IList<Banner> banners;
        using (var conn = _db.CreateNop())
        {
            var sql = $@"SELECT * FROM [{BannerTableForMedia}]
WHERE DisplaySection = 40 AND Enabled = 1
  AND (StartDate <= @Now OR StartDate IS NULL) AND (@Now <= EndDate OR EndDate IS NULL)";
            banners = (await conn.QueryAsync<Banner>(sql, new { Now = dateTimeNow })).AsList();
        }

        var result = (from b in banners
                      join pb in bannerPhysicalStoreMappings on b.Id equals pb.BannerId
                      where pb.PhysicalStoreLocationId == locationId
                      select b).FirstOrDefault();

        return result;
    }

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
    public async Task<Picture> InsertPictureAsync(IFormFile formFile, string defaultFileName = "", string virtualPath = "")
    {
        var imgExt = new List<string>
        {
            ".bmp",
            ".gif",
            ".webp",
            ".jpeg",
            ".jpg",
            ".jpe",
            ".jfif",
            ".pjpeg",
            ".pjp",
            ".png",
            ".tiff",
            ".tif",
            ".svg"
        } as IReadOnlyCollection<string>;

        var fileName = formFile.FileName;
        if (string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(defaultFileName))
            fileName = defaultFileName;

        //remove path (passed in IE)
        fileName = Path.GetFileName(fileName);

        var contentType = formFile.ContentType;

        var fileExtension = Path.GetExtension(fileName);
        if (!string.IsNullOrEmpty(fileExtension))
            fileExtension = fileExtension.ToLowerInvariant();

        if (imgExt.All(ext => !ext.Equals(fileExtension, StringComparison.CurrentCultureIgnoreCase)))
            return null;

        //contentType is not always available 
        //that's why we manually update it here
        //https://mimetype.io/all-types/
        if (string.IsNullOrEmpty(contentType))
            contentType = GetPictureContentTypeByFileExtension(fileExtension);

        using var fileStream = formFile.OpenReadStream();
        using var ms = new MemoryStream();
        fileStream.CopyTo(ms);
        var fileBytes = ms.ToArray();

        var picture = await InsertPictureAsync(fileBytes, contentType, Path.GetFileNameWithoutExtension(fileName));

        if (string.IsNullOrEmpty(virtualPath))
            return picture;

        await UpdatePictureAsync(picture);

        return picture;
    }

    /// <summary>
    /// Inserts a picture
    /// </summary>
    /// <param name="pictureBinary">The picture binary</param>
    /// <param name="mimeType">The picture MIME type</param>
    /// <param name="seoFilename">The SEO filename</param>
    /// <param name="altAttribute">"alt" attribute for "img" HTML element</param>
    /// <param name="titleAttribute">"title" attribute for "img" HTML element</param>
    /// <param name="isNew">A value indicating whether the picture is new</param>
    /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public async Task<Picture> InsertPictureAsync(byte[] pictureBinary, string mimeType, string seoFilename,
        string altAttribute = null, string titleAttribute = null,
        bool isNew = true, bool validateBinary = true)
    {
        mimeType = CommonHelper.EnsureNotNull(mimeType);
        mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

        seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

        if (validateBinary)
            pictureBinary = await ValidatePictureAsync(pictureBinary, mimeType, seoFilename);

        var picture = new Picture
        {
            MimeType = mimeType,
            SeoFilename = seoFilename,
            AltAttribute = altAttribute,
            TitleAttribute = titleAttribute,
            IsNew = isNew
        };

        try
        {
            using var conn = _db.CreateNop();
            var sql = $@"INSERT INTO [{PictureTable}] (MimeType, SeoFilename, AltAttribute, TitleAttribute, IsNew)
VALUES (@MimeType, @SeoFilename, @AltAttribute, @TitleAttribute, @IsNew);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
            picture.Id = await conn.ExecuteScalarAsync<int>(sql, picture);
        }
        catch (Exception)
        {

        }

        await UpdatePictureBinaryAsync(picture, pictureBinary);

        return picture;
    }

    /// <summary>
    /// Validates input picture dimensions
    /// </summary>
    /// <param name="pictureBinary">Picture binary</param>
    /// <param name="mimeType">MIME type</param>
    /// <param name="fileName">Name of file</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary or throws an exception
    /// </returns>
    public async Task<byte[]> ValidatePictureAsync(byte[] pictureBinary, string mimeType, string fileName)
    {
        try
        {
            using var image = SKBitmap.Decode(pictureBinary);

            //resize the image in accordance with the maximum size
            if (Math.Max(image.Height, image.Width) > 1000)
            {
                var format = GetImageFormatByMimeType(mimeType);
                pictureBinary = ImageResize(image, format, 1000);
            }
            return pictureBinary;
        }
        catch (Exception exc)
        {
            _logService.InsertLog( "Method: ValidatePictureAsync " + exc.Message, string.Empty);
            return pictureBinary;
        }
    }

    /// <summary>
    /// Get image format by mime type
    /// </summary>
    /// <param name="mimeType">Mime type</param>
    /// <returns>SKEncodedImageFormat</returns>
    protected SKEncodedImageFormat GetImageFormatByMimeType(string mimeType)
    {
        var format = SKEncodedImageFormat.Jpeg;
        if (string.IsNullOrEmpty(mimeType))
            return format;

        var parts = mimeType.ToLowerInvariant().Split('/');
        var lastPart = parts[^1];

        switch (lastPart)
        {
            case "webp":
                format = SKEncodedImageFormat.Webp;
                break;
            case "png":
            case "gif":
            case "bmp":
            case "x-icon":
                format = SKEncodedImageFormat.Png;
                break;
            default:
                break;
        }

        return format;
    }

    /// <summary>
    /// Resize image by targetSize
    /// </summary>
    /// <param name="image">Source image</param>
    /// <param name="format">Destination format</param>
    /// <param name="targetSize">Target size</param>
    /// <returns>Image as array of byte[]</returns>
    protected byte[] ImageResize(SKBitmap image, SKEncodedImageFormat format, int targetSize)
    {
        if (image == null)
            throw new ArgumentNullException("Image is null");

        float width, height;
        if (image.Height > image.Width)
        {
            // portrait
            width = image.Width * (targetSize / (float)image.Height);
            height = targetSize;
        }
        else
        {
            // landscape or square
            width = targetSize;
            height = image.Height * (targetSize / (float)image.Width);
        }

        if ((int)width == 0 || (int)height == 0)
        {
            width = image.Width;
            height = image.Height;
        }
        try
        {
            using var resizedBitmap = image.Resize(new SKImageInfo((int)width, (int)height), SKFilterQuality.Medium);
            using var cropImage = SKImage.FromBitmap(resizedBitmap);

            //In order to exclude saving pictures in low quality at the time of installation, we will set the value of this parameter to 80 (as by default)
            //return cropImage.Encode(format, _mediaSettings.DefaultImageQuality > 0 ? _mediaSettings.DefaultImageQuality : 80).ToArray();
            return cropImage.Encode(format, 80).ToArray();
        }
        catch
        {
            return image.Bytes;
        }

    }

    /// <summary>
    /// Updates the picture
    /// </summary>
    /// <param name="picture">The picture to update</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture
    /// </returns>
    public async Task<Picture> UpdatePictureAsync(Picture picture)
    {
        if (picture == null)
            return null;

        var seoFilename = CommonHelper.EnsureMaximumLength(picture.SeoFilename, 100);
        picture.SeoFilename = seoFilename;

        using (var conn = _db.CreateNop())
        {
            var sql = $"UPDATE [{PictureTable}] SET MimeType = @MimeType, SeoFilename = @SeoFilename, AltAttribute = @AltAttribute, TitleAttribute = @TitleAttribute, IsNew = @IsNew WHERE Id = @Id";
            await conn.ExecuteAsync(sql, picture);
        }
        await UpdatePictureBinaryAsync(picture, (await GetPictureBinaryByPictureIdAsync(picture.Id)).BinaryData);

        return picture;
    }

    /// <summary>
    /// Updates the picture binary data
    /// </summary>
    /// <param name="picture">The picture object</param>
    /// <param name="binaryData">The picture binary data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    protected async Task<PictureBinary> UpdatePictureBinaryAsync(Picture picture, byte[] binaryData)
    {
        if (picture == null)
            throw new ArgumentNullException(nameof(picture));

        var pictureBinary = await GetPictureBinaryByPictureIdAsync(picture.Id);

        var isNew = pictureBinary == null;

        if (isNew)
            pictureBinary = new PictureBinary
            {
                PictureId = picture.Id
            };

        pictureBinary.BinaryData = binaryData;

        if (isNew)
        {
            using var conn = _db.CreateNop();
            var sql = $@"INSERT INTO [{PictureBinaryTable}] (PictureId, BinaryData) VALUES (@PictureId, @BinaryData);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
            pictureBinary.Id = await conn.ExecuteScalarAsync<int>(sql, pictureBinary);
        }
        else
        {
            using var conn = _db.CreateNop();
            var sql = $"UPDATE [{PictureBinaryTable}] SET PictureId = @PictureId, BinaryData = @BinaryData WHERE Id = @Id";
            await conn.ExecuteAsync(sql, pictureBinary);
        }

        return pictureBinary;
    }

    /// <summary>
    /// Get product picture binary by picture identifier
    /// </summary>
    /// <param name="pictureId">The picture identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture binary
    /// </returns>
    public async Task<PictureBinary> GetPictureBinaryByPictureIdAsync(int pictureId)
    {
        using var conn = _db.CreateNop();
        var sql = $"SELECT TOP 1 * FROM [{PictureBinaryTable}] WHERE PictureId = @PictureId";
        return await conn.QueryFirstOrDefaultAsync<PictureBinary>(sql, new { PictureId = pictureId });
    }

    #endregion

    #region Common methods

    /// <summary>
    /// Get content type for picture by file extension
    /// </summary>
    /// <param name="fileExtension">The file extension</param>
    /// <returns>Picture's content type</returns>
    public string GetPictureContentTypeByFileExtension(string fileExtension)
    {
        string contentType = null;

        switch (fileExtension.ToLower())
        {
            case ".bmp":
                contentType = "image/bmp";
                break;
            case ".gif":
                contentType = "image/gif";
                break;
            case ".jpeg":
            case ".jpg":
            case ".jpe":
            case ".jfif":
            case ".pjpeg":
            case ".pjp":
                contentType = "image/jpeg";
                break;
            case ".webp":
                contentType = "image/webp";
                break;
            case ".png":
                contentType = "image/png";
                break;
            case ".svg":
                contentType = "image/svg+xml";
                break;
            case ".tiff":
            case ".tif":
                contentType = "image/tiff";
                break;
            default:
                break;
        }

        return contentType;
    }

    #endregion
}

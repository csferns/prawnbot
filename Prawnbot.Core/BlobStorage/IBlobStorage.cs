using System;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.BlobStorage
{
    public interface IBlobStorage
    {
        Task<MemoryStream> DownloadAsync(BlobContainerEnum container, string key);
        MemoryStream Download(BlobContainerEnum container, string key);

        Task<Uri> UploadAsync(BlobContainerEnum container, byte[] file, string key);
        Uri Upload(BlobContainerEnum container, byte[] file, string key);
        Task<Uri> UploadAsync(BlobContainerEnum container, MemoryStream file, string key);
        Uri Upload(BlobContainerEnum container, MemoryStream file, string key);

        Task<bool> ExistsAsync(BlobContainerEnum container, string key);
        bool Exists(BlobContainerEnum container, string key);
    }
}

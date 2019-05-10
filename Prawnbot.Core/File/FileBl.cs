using CsvHelper;
using Discord;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Prawnbot.Core.Base;
using Prawnbot.Core.Bot;
using Prawnbot.Core.Framework;
using Prawnbot.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.File
{
    public interface IFileBl
    {
        Task<CloudBlobContainer> GetBlobContainer(string containerName);
        Task<string> GetUrlFromBlobStore(string fileName, string containerName);
        Task<Stream> DownloadFileFromBlobStore(string fileName, string containerName);
        Task<bool> UploadFileToBlobStore(string fileName, string containerName);
        Task<string> WriteToCSV(List<CSVColumns> columns, string folderPath, DateTime startTime, ulong? id, string guildName);
        Task<List<CSVColumns>> CreateCSVList(ulong id);
    }

    public class FileBl : BaseBl, IFileBl
    {
        protected IBotBl _botBl;
        public FileBl()
        {
            _botBl = new BotBl();
        }

        public async Task<CloudBlobContainer> GetBlobContainer(string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=cameronfernsdiag;AccountKey=bSciFhSvcH0PlAGkd4tjRRi9VpeD5SFRcqNSsxFIR2jZNy3e6dTjwkNca+ihlWWJu9Ba3TFb1L0aN/INz90Ydg==;EndpointSuffix=core.windows.net");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            return container;
        }

        public async Task<string> GetUrlFromBlobStore(string fileName, string containerName)
        {
            var container = await GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            return blockBlob.StorageUri.PrimaryUri.AbsoluteUri;
        }

        public async Task<Stream> DownloadFileFromBlobStore(string fileName, string containerName)
        {
            var container = await GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            MemoryStream stream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(stream);

            return stream;
        }

        public async Task<bool> UploadFileToBlobStore(string fileName, string containerName)
        {
            var container = await GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromFileAsync(fileName);

            return true;
        }

        public async Task<string> WriteToCSV(List<CSVColumns> columns, string folderPath, DateTime startTime, ulong? id, string guildName)
        {
            string fileName;
            if (id.GetValueOrDefault() != default(ulong)) fileName = $"{_botBl.GetChannelById(id.GetValueOrDefault(ulong.MinValue)).Name}-backup.csv";
            else fileName = $"{guildName}-backup.csv";

            string filePath = $"{Environment.CurrentDirectory}\\{folderPath}";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            filePath += $"\\{fileName}";

            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fileStream))
            using (CsvWriter csv = new CsvWriter(writer))
            {
                fileStream.Position = fileStream.Length;
                csv.WriteRecords(columns);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"Backed up {columns.Count()} messages to {fileName}.");

            TimeSpan completionTime = DateTime.Now - startTime;

            sb.Append($"The operation took {completionTime.Hours}h:{completionTime.Minutes}m:{completionTime.Seconds}s:{completionTime.Milliseconds}ms");

            return sb.ToString();
        }

        public async Task<List<CSVColumns>> CreateCSVList(ulong id)
        {
            List<CSVColumns> records = new List<CSVColumns>();

            IEnumerable<IMessage> messagesToAdd = await _botBl.GetAllMessages(id);

            for (int i = 0; i < messagesToAdd.Count(); i++)
            {
                if (messagesToAdd.ElementAt(i).Type == MessageType.ChannelPinnedMessage)
                {
                    continue;
                }

                CSVColumns recordToAdd = new CSVColumns
                {
                    MessageID = messagesToAdd.ElementAt(i).Id,
                    Author = messagesToAdd.ElementAt(i).Author.Username,
                    AuthorIsBot = messagesToAdd.ElementAt(i).Author.IsBot,
                    MessageContent = messagesToAdd.ElementAt(i).Content,
                    Timestamp = messagesToAdd.ElementAt(i).Timestamp
                };

                if (messagesToAdd.ElementAt(i).Attachments.Count() > 0)
                {
                    List<string> attachmentUrls = new List<string>();

                    foreach (var attachment in messagesToAdd.ElementAt(i).Attachments)
                    {
                        attachmentUrls.Add(attachment.Url);
                    }

                    string.Join(", " , attachmentUrls);
                }

                records.Add(recordToAdd);
            }

            return records;
        }
    }
}

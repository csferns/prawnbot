using CsvHelper;
using Discord;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Prawnbot.Core.Models;
using Prawnbot.Data.Models.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public interface IFileBl
    {
        Task<CloudBlobContainer> GetBlobContainer(string containerName);
        Task<Uri> GetUriFromBlobStore(string fileName, string containerName);
        Task<Stream> GetStreamFromBlobStore(string fileName, string containerName);
        Task<Stream> DownloadFileFromBlobStore(string fileName, string containerName);
        Task<bool> UploadFileToBlobStore(string fileName, string containerName);
        FileStream CreateLocalFileIfNotExists(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
        Task<string[]> ReadFromFileAsync(string fileName);
        string WriteToCSV(List<CSVColumns> columns, ulong? id, string guildName);
        Task<bool> WriteToFile(string valueToWrite, string fileName);
        Task<List<CSVColumns>> CreateCSVList(ulong id);
        bool CheckIfTranslationExists();
        TranslateData GetTranslationFromFile(string toLanguage, string fromLanguage, string textToTranslate);
        Dictionary<string, string> GetAllConfigurationValues();
        void SetConfigurationValue(string configurationName, string newConfigurationValue);
        void SetEventListeners(bool newValue);
    }

    public class FileBl : BaseBl, IFileBl
    {
        public async Task<CloudBlobContainer> GetBlobContainer(string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigUtility.BlobStoreConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            return container;
        }

        public async Task<Uri> GetUriFromBlobStore(string fileName, string containerName)
        {
            var container = await GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            return blockBlob.Uri;
        }

        public async Task<Stream> GetStreamFromBlobStore(string fileName, string containerName)
        {
            using (var stream = new MemoryStream())
            {
                var container = await GetBlobContainer(containerName);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                await blockBlob.DownloadToStreamAsync(stream);

                return stream;
            }
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

        public FileStream CreateLocalFileIfNotExists(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            if (!Directory.Exists(ConfigUtility.TextFileDirectory))
            {
                Directory.CreateDirectory(ConfigUtility.TextFileDirectory);
            }

            string filePath = ConfigUtility.TextFileDirectory + $"\\{fileName}";

            return new FileStream(filePath, fileMode, fileAccess, fileShare);
        }

        public async Task<string[]> ReadFromFileAsync(string fileName)
        {
            using (CreateLocalFileIfNotExists(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {  }

            string filePath = ConfigUtility.TextFileDirectory + $"\\{fileName}";

            List<string> fileLines = new List<string>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    fileLines.Add(await reader.ReadLineAsync());
                }
            }

            return fileLines.ToArray();
        }

        public string WriteToCSV(List<CSVColumns> columns, ulong? id, string guildName)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            string fileName;
            if (id.GetValueOrDefault() != default(ulong)) fileName = $"{_botBl.GetChannelById(id.GetValueOrDefault(ulong.MinValue)).Name}-backup.csv";
            else fileName = $"{guildName}-backup.csv";

            using (FileStream fileStream = CreateLocalFileIfNotExists(fileName, FileMode.Truncate, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            using (CsvWriter csv = new CsvWriter(writer))
            {
                fileStream.Position = fileStream.Length;
                csv.WriteRecords(columns);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"Backed up {columns.Count()} messages to {fileName}. \n");
            stopwatch.Stop();

            sb.Append($"The operation took {stopwatch.Elapsed.Hours}h:{stopwatch.Elapsed.Minutes}m:{stopwatch.Elapsed.Seconds}s:{stopwatch.Elapsed.Milliseconds}ms");

            return sb.ToString();
        }

        public async Task<bool> WriteToFile(string valueToWrite, string fileName)
        {
            using (var fileStream = CreateLocalFileIfNotExists(fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                await writer.WriteLineAsync(valueToWrite);
            }

            return true;
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

                    var attachmentString = string.Join(", " , attachmentUrls);
                    recordToAdd.Attachments = attachmentString;
                }

                records.Add(recordToAdd);
            }

            return records;
        }

        public bool CheckIfTranslationExists()
        {
            return false;
        }

        public TranslateData GetTranslationFromFile(string toLanguage, string fromLanguage, string textToTranslate)
        {
            return null;
        }

        public Dictionary<string, string> GetAllConfigurationValues()
        {
            return ConfigUtility.GetAllConfig();
        }

        public void SetConfigurationValue(string configurationName, string newConfigurationValue)
        {
            Common.Configuration.ConfigUtility.SetConfig(configurationName, newConfigurationValue);
        }

        public void SetEventListeners(bool newValue)
        {
            ConfigUtility.AllowEventListeners = newValue;
        }
    }
}

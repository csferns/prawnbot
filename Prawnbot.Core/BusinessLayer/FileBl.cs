﻿using CsvHelper;
using Discord;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Core.Model.DTOs;
using Prawnbot.Utility.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public interface IFileBL
    {
        Task<CloudBlobContainer> GetBlobContainer(string containerName);
        Task<Uri> GetUriFromBlobStoreAsync(string fileName, string containerName);
        Task<Stream> GetStreamFromBlobStoreAsync(string fileName, string containerName);
        Task<Stream> DownloadFileFromBlobStoreAsync(string fileName, string containerName);
        Task UploadFileToBlobStoreAsync(string fileName, string containerName);
        FileStream CreateLocalFileIfNotExists(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
        Task<Bunch<string>> ReadFromFileAsync(string fileName);
        void WriteToCSV(IList<CSVColumns> columns, ulong? id, string fileName);
        Task WriteToFileAsync(string valueToWrite, string fileName);
        Bunch<CSVColumns> CreateCSVList(IList<IMessage> messagesToAdd);
        bool CheckIfTranslationExists();
        Bunch<TranslateData> GetTranslationFromFile(string toLanguage, string fromLanguage, string textToTranslate);
    }

    public class FileBL : BaseBL, IFileBL
    {
        public async Task<CloudBlobContainer> GetBlobContainer(string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigUtility.BlobStoreConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            return container;
        }

        public async Task<Uri> GetUriFromBlobStoreAsync(string fileName, string containerName)
        {
            CloudBlobContainer container = await GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            return blockBlob.Uri;
        }

        public async Task<Stream> GetStreamFromBlobStoreAsync(string fileName, string containerName)
        {
            using MemoryStream stream = new MemoryStream();
            CloudBlobContainer container = await GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            await blockBlob.DownloadToStreamAsync(stream);

            return stream;
        }

        public async Task<Stream> DownloadFileFromBlobStoreAsync(string fileName, string containerName)
        {
            CloudBlobContainer container = await GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            MemoryStream stream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(stream);

            return stream;
        }

        public async Task UploadFileToBlobStoreAsync(string fileName, string containerName)
        {
            CloudBlobContainer container = await GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromFileAsync(fileName);
        }

        public FileStream CreateLocalFileIfNotExists(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            string TextFileDirectory = ConfigUtility.TextFileDirectory;

            if (!Directory.Exists(TextFileDirectory))
            {
                Directory.CreateDirectory(TextFileDirectory);
            }

            string filePath = TextFileDirectory + "\\" + fileName;

            if (!File.Exists(filePath))
            {
                using (FileStream file = File.Create(fileName)) { };
            }

            return new FileStream(filePath, fileMode, fileAccess, fileShare);
        }

        public async Task<Bunch<string>> ReadFromFileAsync(string fileName)
        {
            using (FileStream file = CreateLocalFileIfNotExists(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader reader = new StreamReader(file)) 
            {
                Bunch<string> fileLines = new Bunch<string>();

                while (!reader.EndOfStream)
                {
                    fileLines.Add(await reader.ReadLineAsync());
                }

                return fileLines;
            };
        }

        public void WriteToCSV(IList<CSVColumns> columns, ulong? id, string fileName)
        {
            using (FileStream fileStream = CreateLocalFileIfNotExists(fileName, FileMode.Truncate, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            using (CsvWriter csv = new CsvWriter(writer))
            {
                fileStream.Position = fileStream.Length;
                csv.WriteRecords(columns);
            }
        }

        public async Task WriteToFileAsync(string valueToWrite, string fileName)
        {
            using (FileStream fileStream = CreateLocalFileIfNotExists(fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                await writer.WriteLineAsync(valueToWrite);
            }
        }

        public Bunch<CSVColumns> CreateCSVList(IList<IMessage> messagesToAdd)
        {
            Bunch<CSVColumns> records = new Bunch<CSVColumns>();

            for (int message = 0; message < messagesToAdd.Count(); message++)
            {
                CSVColumns recordToAdd = new CSVColumns
                {
                    MessageID = messagesToAdd[message].Id,
                    Author = messagesToAdd[message].Author.Username,
                    AuthorIsBot = messagesToAdd[message].Author.IsBot,
                    MessageContent = messagesToAdd[message].Content,
                    Timestamp = messagesToAdd[message].Timestamp
                };

                if (messagesToAdd[message].Attachments.Count() > 0)
                {
                    recordToAdd.Attachments = string.Join(", ", messagesToAdd[message].Attachments);
                }

                records.Add(recordToAdd);
            }

            return records;
        }

        public bool CheckIfTranslationExists()
        {
            return false;
        }

        public Bunch<TranslateData> GetTranslationFromFile(string toLanguage, string fromLanguage, string textToTranslate)
        {
            return new Bunch<TranslateData>();
        }
    }
}

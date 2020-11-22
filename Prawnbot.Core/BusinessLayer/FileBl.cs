using CsvHelper;
using Discord;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Core.Model.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public class FileBL : BaseBL, IFileBL
    {
        private readonly IConfigUtility configUtility;

        public FileBL(IConfigUtility configUtility)
        {
            this.configUtility = configUtility;
        }

        public FileStream CreateLocalFileIfNotExists(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            string TextFileDirectory = configUtility.TextFileDirectory;

            if (!Directory.Exists(TextFileDirectory))
            {
                Directory.CreateDirectory(TextFileDirectory);
            }

            string filePath = TextFileDirectory + "\\" + fileName;

            if (!File.Exists(filePath))
            {
                using (FileStream fs = File.Create(filePath))
                {
                }
            }

            return new FileStream(filePath, fileMode, fileAccess, fileShare);
        }

        public FileStream WriteToCSV(IList<CSVColumns> columns, string fileName)
        {
            using (FileStream fileStream = CreateLocalFileIfNotExists(fileName, FileMode.Truncate, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            using (CsvWriter csv = new CsvWriter(writer))
            {
                csv.WriteHeader(typeof(CSVColumns));
                csv.WriteRecords(columns);
            }

            return File.OpenRead(configUtility.TextFileDirectory + "\\" + fileName);
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
                    MessageSource = messagesToAdd[message].Source.ToString(),
                    Author = messagesToAdd[message].Author.Username,
                    WasSentByBot = messagesToAdd[message].Author.IsBot,
                    IsPinned = messagesToAdd[message].IsPinned,
                    MessageContent = messagesToAdd[message].Content,
                    Timestamp = messagesToAdd[message].Timestamp
                };

                recordToAdd.AttachmentCount = messagesToAdd[message].Attachments.Count();

                if (messagesToAdd[message].Attachments.Any())
                {
                    recordToAdd.Attachments = messagesToAdd[message].Attachments.FirstOrDefault().Url;
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

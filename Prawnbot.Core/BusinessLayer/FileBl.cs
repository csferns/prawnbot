using CsvHelper;
using Discord;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Core.Model.DTOs;
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

        public FileStream WriteToCSV(HashSet<CSVColumns> columns, string fileName)
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

        public HashSet<CSVColumns> CreateCSVList(HashSet<IMessage> messagesToAdd)
        {
            HashSet<CSVColumns> records = new HashSet<CSVColumns>(messagesToAdd.Count);

            foreach (IMessage message in messagesToAdd)
            {
                CSVColumns recordToAdd = new CSVColumns
                {
                    MessageID = message.Id,
                    MessageSource = message.Source.ToString(),
                    Author = message.Author.Username,
                    WasSentByBot = message.Author.IsBot,
                    IsPinned = message.IsPinned,
                    MessageContent = message.Content,
                    Timestamp = message.Timestamp
                };

                recordToAdd.AttachmentCount = message.Attachments.Count();

                if (message.Attachments.Any())
                {
                    recordToAdd.Attachments = message.Attachments.FirstOrDefault().Url;
                }

                records.Add(recordToAdd);
            }

            return records;
        }

        public bool CheckIfTranslationExists()
        {
            return false;
        }

        public HashSet<TranslateData> GetTranslationFromFile(string toLanguage, string fromLanguage, string textToTranslate)
        {
            return new HashSet<TranslateData>();
        }
    }
}

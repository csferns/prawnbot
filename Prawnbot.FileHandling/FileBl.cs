using CsvHelper;
using Discord;
using Prawnbot.Common.Configuration;
using Prawnbot.Common.DTOs;
using Prawnbot.Common.DTOs.API.Translation;
using Prawnbot.Core.Custom.Collections;
using Prawnbot.FileHandling.Interfaces;
using Prawnbot.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.FileHandling
{
    public class FileBL : BaseBL, IFileBL
    {
        private static FileStream LogFile;

        private const string LogFileName = "EventLogs";
        private const string LogFileExtension = ".txt";

        private static void GetLogFileIfNotExists()
        {
            if (LogFile == null)
            {
                string directory = ConfigUtility.TextFileDirectory;

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string filePath = directory + "\\" + LogFileName + LogFileExtension;

                LogFile = File.OpenWrite(filePath);
            }

            // Used to make sure that the line is written at the end of the file
            LogFile.Position = LogFile.Length;
        }

        public FileStream CreateLocalFileIfNotExists(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            string path = CreateLocalFileIfNotExists(fileName);

            return File.Open(path, fileMode, fileAccess, fileShare);
        }

        public string CreateLocalFileIfNotExists(string fileName)
        {
            string TextFileDirectory = ConfigUtility.TextFileDirectory;

            if (!Directory.Exists(TextFileDirectory))
            {
                Directory.CreateDirectory(TextFileDirectory);
            }

            string filePath = TextFileDirectory + "\\" + fileName;

            if (!File.Exists(filePath))
            {
                using (FileStream fs = File.Create(filePath))
                {
                    fs.Close();
                }
            }

            return filePath;
        }

        public async Task<Bunch<string>> ReadFromFileAsync(string fileName)
        {
            string path = CreateLocalFileIfNotExists(fileName);

            var fileLines = await File.ReadAllLinesAsync(path, Encoding.Default);

            return fileLines.ToBunch();
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

            return File.OpenRead(ConfigUtility.TextFileDirectory + "\\" + fileName);
        }

        public async Task WriteToFileAsync(string valueToWrite, string fileName)
        {
            using (FileStream fileStream = CreateLocalFileIfNotExists(fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                await writer.WriteLineAsync(valueToWrite);
            }
        }

        public async Task WriteToLogFileAsync(string valueToWrite)
        {
            GetLogFileIfNotExists();

            StreamWriter writer = new StreamWriter(LogFile);
            await writer.WriteLineAsync(valueToWrite);

            writer.Flush();
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

using Discord;
using Prawnbot.Core.File;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.Log
{
    public static class Logging
    {
        public static async Task PopulateEventLog(LogMessage message)
        {
            string folderPath = $"{Environment.CurrentDirectory}\\Logs";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string filePath = $"{folderPath}\\EventLogs.txt";
            if (!System.IO.File.Exists(filePath)) System.IO.File.Create(filePath);

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    await writer.WriteLineAsync(message.ToString(timestampKind: DateTimeKind.Local));
                }

                Console.WriteLine(message.ToString(timestampKind: DateTimeKind.Local));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task PopulateMessageLog(LogMessage message)
        {
            string folderPath = $"{Environment.CurrentDirectory}\\Logs";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string filePath = $"{folderPath}\\MessageLogs.txt";
            if (!System.IO.File.Exists(filePath)) System.IO.File.Create(filePath);

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    await writer.WriteLineAsync(message.ToString(timestampKind: DateTimeKind.Local));
                }

                Console.WriteLine(message.ToString(timestampKind: DateTimeKind.Local));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task PopulateTranslationLog(LogMessage message)
        {

        }
    }
}

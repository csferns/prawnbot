using Discord;
using System;
using System.IO;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public partial class Functions
    {
        public async Task PopulateMessageLog(LogMessage arg)
        {
            string folderPath = $"{Environment.CurrentDirectory}\\Logs";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string filePath = $"{folderPath}\\MessageLogs.txt";
            if (!File.Exists(filePath)) File.Create(filePath);

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    await writer.WriteLineAsync(arg.ToString(timestampKind: DateTimeKind.Local));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task PopulateEventLog(LogMessage arg)
        {
            string folderPath = $"{Environment.CurrentDirectory}\\Logs";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string filePath = $"{folderPath}\\EventLogs.txt";
            if (!File.Exists(filePath)) File.Create(filePath);

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    await writer.WriteLineAsync(arg.ToString(timestampKind: DateTimeKind.Local));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

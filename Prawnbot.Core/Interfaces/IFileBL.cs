using Discord;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Core.Model.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    /// <summary>
    /// Contains all the methods to deal with writing data to files
    /// </summary>
    public interface IFileBL
    {
        FileStream CreateLocalFileIfNotExists(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
        FileStream WriteToCSV(HashSet<CSVColumns> columns, string fileName);
        Task WriteToFileAsync(string valueToWrite, string fileName);
        HashSet<CSVColumns> CreateCSVList(HashSet<IMessage> messagesToAdd);
        bool CheckIfTranslationExists();
        HashSet<TranslateData> GetTranslationFromFile(string toLanguage, string fromLanguage, string textToTranslate);
    }
}

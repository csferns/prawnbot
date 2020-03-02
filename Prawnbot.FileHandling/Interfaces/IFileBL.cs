using Discord;
using Prawnbot.Common.DTOs;
using Prawnbot.Common.DTOs.API.Translation;
using Prawnbot.Core.Custom.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.FileHandling.Interfaces
{
    /// <summary>
    /// Contains all the methods to deal with writing data to files
    /// </summary>
    public interface IFileBL
    {
        FileStream CreateLocalFileIfNotExists(string fileName, FileMode fileMode, FileAccess fileAccess, FileShare fileShare);
        Task<Bunch<string>> ReadFromFileAsync(string fileName);
        FileStream WriteToCSV(IList<CSVColumns> columns, string fileName);
        Task WriteToFileAsync(string valueToWrite, string fileName);
        Bunch<CSVColumns> CreateCSVList(IList<IMessage> messagesToAdd);
        bool CheckIfTranslationExists();
        Bunch<TranslateData> GetTranslationFromFile(string toLanguage, string fromLanguage, string textToTranslate);
    }
}

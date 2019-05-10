using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.Data.Models.API
{
    public class TranslateRootobject
    {
        public TranslateData[] TranslateData { get; set; }
    }

    public class TranslateData
    {
        public Detectedlanguage detectedLanguage { get; set; }
        public Translation[] translations { get; set; }
    }

    public class Detectedlanguage
    {
        public string language { get; set; }
        public float score { get; set; }
    }

    public class Translation
    {
        public string text { get; set; }
        public string to { get; set; }
    }

    public class LanguageTranslationRoot
    {
        public Language[] Languages { get; set; }
    }

    public class Language
    {
        public LanguageDetails[] LanguageDetails { get; set; }
    }

    public class LanguageDetails
    {
        public string name { get; set; }
        public string nativeName { get; set; }
        public string dir { get; set; }
    }
}

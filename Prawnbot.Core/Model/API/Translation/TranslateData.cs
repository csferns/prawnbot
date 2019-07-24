using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.Core.Model.API.Translation
{
    public class TranslateData
    {
        public Detectedlanguage detectedLanguage { get; set; }
        public Translation[] translations { get; set; }
    }
}

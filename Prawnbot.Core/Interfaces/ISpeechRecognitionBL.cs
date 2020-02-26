using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
{
    public interface ISpeechRecognitionBL
    {
        Task Setup(AudioInStream discordAudioInStream);
    }
}

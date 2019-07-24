using Microsoft.CognitiveServices.Speech;
using Prawnbot.Utility.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public interface ISpeechRecognitionBL
    {
        Task Setup(Discord.Audio.AudioInStream discordAudioInStream);
    }

    public class SpeechRecognitionBL : BaseBL, ISpeechRecognitionBL
    {
        public async Task Setup(Discord.Audio.AudioInStream discordAudioInStream)
        {
            using (SpeechSynthesizer synth = new SpeechSynthesizer(SpeechConfig.FromEndpoint(new Uri("https://uksouth.api.cognitive.microsoft.com/sts/v1.0/issuetoken"), ConfigUtility.SpeechServicesKey)))
            using (MemoryStream streamAudio = new MemoryStream())
            {
                await synth.StartSpeakingTextAsync("");
            }
        }
    }
}

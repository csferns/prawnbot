using Discord.Audio;
using Microsoft.CognitiveServices.Speech;
using Prawnbot.Common;
using Prawnbot.Common.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public interface ISpeechRecognitionBL
    {
        Task Setup(AudioInStream discordAudioInStream);
    }

    public class SpeechRecognitionBL : BaseBL, ISpeechRecognitionBL
    {
        public async Task Setup(AudioInStream discordAudioInStream)
        {
            SpeechConfig config = SpeechConfig.FromSubscription(ConfigUtility.SpeechServicesKey, "westeurope");

            using (SpeechRecognizer recogniser = new SpeechRecognizer(config))
            {
                await recogniser.StartContinuousRecognitionAsync();
            }


            using (SpeechSynthesizer synth = new SpeechSynthesizer(SpeechConfig.FromEndpoint(new Uri(ConfigUtility.MicrosoftSpeechServicesEndpoint), ConfigUtility.SpeechServicesKey)))
            using (MemoryStream streamAudio = new MemoryStream())
            {
                await synth.StartSpeakingTextAsync("");
            }
        }
    }
}

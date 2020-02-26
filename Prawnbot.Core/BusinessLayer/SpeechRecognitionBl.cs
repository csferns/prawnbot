using Discord.Audio;
using Microsoft.CognitiveServices.Speech;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public class SpeechRecognitionBL : BaseBL, ISpeechRecognitionBL
    {
        public async Task Setup(AudioInStream discordAudioInStream)
        {
            SpeechConfig config = SpeechConfig.FromSubscription(ConfigUtility.MicrosoftSpeechServicesKey, ConfigUtility.MicrosoftSpeechServicesRegion);

            using (SpeechRecognizer recogniser = new SpeechRecognizer(config))
            {
                await recogniser.StartContinuousRecognitionAsync();
            }

            using (SpeechSynthesizer synth = new SpeechSynthesizer(SpeechConfig.FromEndpoint(new Uri(ConfigUtility.MicrosoftSpeechServicesEndpoint), ConfigUtility.MicrosoftSpeechServicesKey)))
            using (MemoryStream streamAudio = new MemoryStream())
            {
                await synth.StartSpeakingTextAsync("");
            }
        }
    }
}

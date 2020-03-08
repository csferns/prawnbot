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
        private readonly IConfigUtility configUtility;

        public SpeechRecognitionBL(IConfigUtility configUtility)
        {
            this.configUtility = configUtility;
        }

        public async Task Setup(AudioInStream discordAudioInStream)
        {
            SpeechConfig config = SpeechConfig.FromSubscription(configUtility.MicrosoftSpeechServicesKey, configUtility.MicrosoftSpeechServicesRegion);

            using (SpeechRecognizer recogniser = new SpeechRecognizer(config))
            {
                await recogniser.StartContinuousRecognitionAsync();
            }

            using (SpeechSynthesizer synth = new SpeechSynthesizer(SpeechConfig.FromEndpoint(new Uri(configUtility.MicrosoftSpeechServicesEndpoint), configUtility.MicrosoftSpeechServicesKey)))
            using (MemoryStream streamAudio = new MemoryStream())
            {
                await synth.StartSpeakingTextAsync("");
            }
        }
    }
}

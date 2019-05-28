using Discord;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Prawnbot.Core.Base;
using RestSharp.Extensions;
using System;

namespace Prawnbot.Core.SpeechRecognition
{
    public interface ISpeechRecognitionBl
    {
        void Setup();
    }

    public class SpeechRecognitionBl : BaseBl, ISpeechRecognitionBl
    {
        private static Discord.Audio.AudioOutStream discordAudioStream;
        private static SpeechSynthesizer speechSynthesizer; 

        public void Setup()
        {
            AudioOutputStream newStream = AudioOutputStream.CreatePullStream(AudioStreamFormat.GetDefaultOutputFormat());
            //discordAudioStream.CopyTo(newStream);

            speechSynthesizer = new SpeechSynthesizer(SpeechConfig.FromEndpoint(new Uri("https://uksouth.api.cognitive.microsoft.com/sts/v1.0/issuetoken"), "48d1c048b2b04e5584e2390d08f5e83f"));
        }
    }
}

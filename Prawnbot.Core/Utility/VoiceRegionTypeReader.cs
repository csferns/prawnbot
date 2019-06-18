using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Prawnbot.Core.Utility
{
    public class VoiceRegionTypeReader : TypeReader
    {
        public async override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            IVoiceRegion region = await context.Guild.GetVoiceRegionsAsync().ToAsyncEnumerable().Flatten().FirstOrDefault();
            return TypeReaderResult.FromSuccess(region);
        }
    }
}

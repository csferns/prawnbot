using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prawnbot_core
{
    public partial class Functions
    {
        public async Task UpdateRichPresence(string name, ActivityType activityType, string streamUrl)
        {
            try
            {
                await _client.SetGameAsync(name ?? null, (ActivityType)activityType == ActivityType.Streaming ? streamUrl : null, (ActivityType)activityType);
            }
            catch (Exception)
            {
                
            }

        }

        public async Task UpdateRichPresence(int delay, List<TextBox> rptextboxes, List<ComboBox> rpdropdowns, string streamUrl)
        {
            var rp = rpdropdowns.Zip(rptextboxes, (dd, tb) => new { Dropdown = dd, Textbox = tb });

            foreach (var item in rp)
            {
                await Task.Delay(delay);
                await _client.SetGameAsync(item.Textbox.Text ?? null, (ActivityType)item.Dropdown.SelectedItem == ActivityType.Streaming ? streamUrl : null, (ActivityType)item.Dropdown.SelectedItem);
            }
        }
    }
}

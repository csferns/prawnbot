using CsvHelper;
using Discord.Commands;
using Discord.WebSocket;
using prawnbot_core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public partial class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("set-alarm")]
        [Summary("sets an alarm for a user")]
        public async Task SetAlarmAsync(int timePassed, [Remainder]string alarmName = null)
        {
            string folderPath = $"{Environment.CurrentDirectory}\\Alarms\\{Context.Guild}";
            string filePath = $"{folderPath}\\alarm.csv";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            else if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            DateTime newAlarmTime = DateTime.Now.AddHours(timePassed);

            Alarm alarm = new Alarm()
            {
                AlarmName = alarmName,
                AlarmTime = newAlarmTime,
                User = Context.User.Username
            };

            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fileStream))
            using (CsvWriter csv = new CsvWriter(writer))
            {
                fileStream.Position = fileStream.Length;
                csv.WriteRecord(alarm);
            }

            await Context.Channel.SendMessageAsync($"{alarm.AlarmTime} alarm set for {alarm.User}");
        }

        [Command("display-alarms")]
        public async Task GetAlarmAsync()
        {
            string folderPath = $"{Environment.CurrentDirectory}\\Alarms\\{Context.Guild}";
            string filePath = $"{folderPath}\\alarm.csv";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
                await Context.Channel.SendMessageAsync("No alarms set!");
                return;
            }

            StringBuilder sb = new StringBuilder();

            IEnumerable<Alarm> alarmFile = null;

            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            using (StreamReader streamReader = new StreamReader(fileStream))
            using (CsvReader reader = new CsvReader(streamReader))
            {
                alarmFile = reader.GetRecords<Alarm>();
            }

            if (alarmFile == null)
            {
                await Context.Channel.SendMessageAsync("No alarms set!");
                return;
            }

            foreach (var alarm in alarmFile)
            {
                sb.Append(alarm);
            }
        }

        [Command("remove-alarm")]
        public async Task RemoveAlarmAsync()
        {
            string filePath = $"{Environment.CurrentDirectory}\\Alarms\\{Context.Guild}\\alarm.txt";


        }
    }
}

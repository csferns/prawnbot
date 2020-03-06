namespace Prawnbot.CommandEngine
{
    public enum CommandsEnum
    {
        [Command(CommandText = "/disconnect", Description = "Disconnect")]
        Disconnect = 0,
        [Command(CommandText = "/sendmessageuser", Description = "Send a message to a user", Parameters = new string[] { "{name}", "{message}" })]
        SendMessageUser = 1,
        [Command(CommandText = "/sendmessageguild", Description = "Send a message to a guild", Parameters = new string[] { "{guild name}", "{guild channel name}", "{message}" })]
        SendMessageGuild = 2,
        [Command(CommandText = "/richpresence", Description = "Sets the rich presence of the bot", Parameters = new string[] { "{activity type}", "{activity name}" }, OptionalParameters = new string[] { "{stream url}" })]
        RichPresence = 3,
        [Command(CommandText = "/help", Description = "Defines all the active commands")]
        Help = 4,
        [Command(CommandText = "/log", Description = "Opens up the folder location of the logs")]
        Log = 5,
        [Command(CommandText = "/nickname", Description = "Sets the nickname of the bot on a server", Parameters = new string[] { "{guild name}", "{new nickname}" })]
        Nickname = 6,
        [Command(CommandText = "/changeicon", Description = "Sets the icon of the bot", Parameters = new string[] { "{icon url}" })]
        ChangeIcon = 7,
        [Command(CommandText = "/removeicon", Description = "Removes the icon of the bot")]
        RemoveIcon = 8,
        [Command(CommandText = "/setstatus", Description = "Sets the status of the bot", Parameters = new string[] { "{status}" })]
        SetStatus = 9
    }
}

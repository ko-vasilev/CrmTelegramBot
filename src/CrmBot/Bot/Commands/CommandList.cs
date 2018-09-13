using System;
using System.Collections.Generic;

namespace CrmBot.Bot.Commands
{
    /// <summary>
    /// Specifies a list of registered commands.
    /// </summary>
    public static class CommandList
    {
        public static readonly IReadOnlyDictionary<string, Type> PublicCommands = new Dictionary<string, Type>()
        {
            { Start, typeof(GetAuthorizationUrlCommand) },
            { Connect, typeof(GetAuthorizationUrlCommand) },
            { DailyReport, typeof(UpdateDailyReportCommand) },
            { Jobs, typeof(GetDayJobProgressCommand) },
            { DailyReportNotificationsSubscribe, typeof(SubscribeDailyReportNotificationsCommand) },
            { DailyReportNotificationsUnsubscribe, typeof(UnSubscribeDailyReportNotificationsCommand) },
            { JobsList, typeof(GetDayJobsCommand) },
            { CheckDailyReport, typeof(CheckDailyReportExistsCommand) },
            { Help, typeof(HelpCommand) }
        };

        public const string Start = "start";

        public const string Connect = "connect";

        public const string DailyReport = "dr";

        public const string Jobs = "jobs";

        public const string DailyReportNotificationsSubscribe = "subscribedr";

        public const string DailyReportNotificationsUnsubscribe = "unsubscribedr";

        public const string JobsList = "joblist";

        public const string CheckDailyReport = "checkdr";

        public const string Help = "help";
    }

    /// <summary>
    /// Specifies a list of commands available only to admins.
    /// </summary>
    public static class AdminCommandList
    {
        public const string Broadcast = "broadcast";
    }
}

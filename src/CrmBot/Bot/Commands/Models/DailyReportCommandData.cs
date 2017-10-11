using System;

namespace CrmBot.Bot.Commands.Models
{
    /// <summary>
    /// Data associated with the daily report creation process.
    /// </summary>
    public class DailyReportCommandData
    {
        /// <summary>
        /// Date of the daily report.
        /// </summary>
        public DateTime DailyReportDate { get; set; }
    }
}

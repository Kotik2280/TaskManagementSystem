namespace TaskManagementSystem.Models
{
    public static class DateToViewConverter
    {
        public static string GetStringRepresentation(DateTime deadLine)
        {
            TimeSpan timeSpan = deadLine - DateTime.Now;

            if (timeSpan.ToString().Contains('-'))
                return "Out of time";

            if (timeSpan.Days >= 30)
                return (timeSpan.Days / 30) + "m";
            if (timeSpan.Days >= 14)
                return (timeSpan.Days / 7) + "w";
            if (timeSpan.Days >= 2)
                return timeSpan.Days + "d";
            if (timeSpan.Hours >= 3)
                return timeSpan.Hours + "h";
            if (timeSpan.Hours >= 1)
                return $"{timeSpan.Hours}h {timeSpan.Minutes}min";

            return $"{timeSpan.Minutes}min";
        }
    }
}

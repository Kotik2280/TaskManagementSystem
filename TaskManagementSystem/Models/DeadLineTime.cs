namespace TaskManagementSystem.Models
{
    public class DeadLineTime
    {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public void Deconstruct(out int days, out int hours, out int minutes)
        {
            days = Days;
            hours = Hours;
            minutes = Minutes;
        }
    }
}

namespace TaskManagementSystem.Models
{
    public class Node
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeadLine { get; set; }
        public string AuthorName { get; set; }
        public string? Responsible { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; } = 0;
    }

    public enum NodeStatus
    {
        Default,
        Accepted,
        Ejected
    }
}

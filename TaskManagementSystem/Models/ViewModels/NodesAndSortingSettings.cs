namespace TaskManagementSystem.Models.ViewModels
{
    public class NodesAndSortingSettings
    {
        public List<Node> Nodes { get; set; }
        public SortingSettings SortingSettings { get; set; }
        public NodesAndSortingSettings(List<Node> nodes, SortingSettings sortingSettings)
        {
            Nodes = nodes;
            SortingSettings = sortingSettings;
        }
    }
}

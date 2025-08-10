namespace TaskManagementSystem.Models
{
    public static class NodeSorter
    {
        public static List<Node> Sort(IEnumerable<Node> nodes, SortingSettings sortingSettings)
        {
            List<Node> sortedNodes = null;

            switch (sortingSettings.Vector)
            {
                case "By ascending":
                    switch (sortingSettings.Field)
                    {
                        case "ID":
                            sortedNodes = nodes.OrderBy(n => n.Id).ToList();
                            break;
                        case "CreateDate":
                            sortedNodes = nodes.OrderBy(n => n.CreateDate).ToList();
                            break;
                        case "Author":
                            sortedNodes = nodes.OrderBy(n => n.AuthorName).ToList();
                            break;
                        case "Title":
                            sortedNodes = nodes.OrderBy(n => n.Title).ToList();
                            break;
                        case "DeadLine":
                            sortedNodes = nodes.OrderBy(n => n.DeadLine).ToList();
                            break;
                        default:
                            break;
                    }
                    break;
                case "By descending":
                    switch (sortingSettings.Field)
                    {
                        case "ID":
                            sortedNodes = nodes.OrderByDescending(n => n.Id).ToList();
                            break;
                        case "CreateDate":
                            sortedNodes = nodes.OrderByDescending(n => n.CreateDate).ToList();
                            break;
                        case "Author":
                            sortedNodes = nodes.OrderByDescending(n => n.AuthorName).ToList();
                            break;
                        case "Title":
                            sortedNodes = nodes.OrderByDescending(n => n.Title).ToList();
                            break;
                        case "DeadLine":
                            sortedNodes = nodes.OrderByDescending(n => n.DeadLine).ToList();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    sortedNodes = nodes.OrderByDescending(n => n.Id).ToList();
                    break;
            }

            return sortedNodes;
        }
    }
}

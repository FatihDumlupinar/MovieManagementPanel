namespace MovieManagementPanel.WebApp.Models
{
    public class SaloonCreateModel
    {
        public string Name { get; set; } = "";

        public List<int> MovieIds { get; set; } = new();
    }
}

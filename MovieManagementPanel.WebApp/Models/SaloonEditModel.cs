using System.ComponentModel;

namespace MovieManagementPanel.WebApp.Models
{
    public class SaloonEditModel
    {
        public int Id { get; set; }

        [DisplayName("Salon Adı")]
        public string Name { get; set; } = "";
        
        
        [DisplayName("Filmler")]
        public List<int> MovieIds { get; set; } = new();
    }
}

using System.ComponentModel;

namespace MovieManagementPanel.WebApp.Models
{
    public class MovieCreateModel
    {
        [DisplayName("Film Adı")]
        public string Name { get; set; } = "";

        [DisplayName("Özet")]
        public string Description { get; set; } = "";

        [DisplayName("Yayın Tarihi")]
        public DateTime RealeseDate { get; set; }

        [DisplayName("Salonlar")]
        public List<int> SaloonsId { get; set; } = new();
    }
}

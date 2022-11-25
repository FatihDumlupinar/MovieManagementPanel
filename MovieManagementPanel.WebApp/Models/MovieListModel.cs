using System.ComponentModel;

namespace MovieManagementPanel.WebApp.Models
{
    public class MovieListModel
    {
        public int MovieId { get; set; }

        [DisplayName("Film Adı")]
        public string Name { get; set; } = "";

        [DisplayName("Yayın Tarihi")]
        public DateTime RealeseDate { get; set; }
        
        [DisplayName("Salonlar")]
        public string Saloons { get; set; } = "";
    }
}

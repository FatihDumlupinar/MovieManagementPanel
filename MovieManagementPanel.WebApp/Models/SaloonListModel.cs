using System.ComponentModel;

namespace MovieManagementPanel.WebApp.Models
{
    public class SaloonListModel
    {
        public int Id { get; set; }

        [DisplayName("Salon Adı")]
        public string Name { get; set; } = "";

        [DisplayName("Filmler")]
        public string Movies { get; set; } = "";
    }
}

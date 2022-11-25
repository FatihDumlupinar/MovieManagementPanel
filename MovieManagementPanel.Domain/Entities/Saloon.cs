using MovieManagementPanel.Domain.Common;

namespace MovieManagementPanel.Domain.Entities
{
    public class Saloon : BaseEntity
    {
        /// <summary>
        /// Salon adı
        /// </summary>
        public string Name { get; set; } = "";

        public virtual IList<MovieAndSaloon> MoviesAndSaloons { get; set; }

    }
}

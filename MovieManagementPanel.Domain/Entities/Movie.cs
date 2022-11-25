using MovieManagementPanel.Domain.Common;

namespace MovieManagementPanel.Domain.Entities
{
    public class Movie : BaseEntity
    {
        /// <summary>
        /// Film adı
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Film açıklaması/özeti
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Yayın tarihi (gün ay yıl)
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        public virtual IList<MovieAndSaloon> MoviesAndSaloons { get; set; }
    }
}

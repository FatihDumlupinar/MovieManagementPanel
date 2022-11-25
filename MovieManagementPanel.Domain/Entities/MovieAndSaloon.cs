using MovieManagementPanel.Domain.Common;

namespace MovieManagementPanel.Domain.Entities
{
    public class MovieAndSaloon : BaseEntity
    {
        public virtual Movie Movie { get; set; }

        public virtual Saloon Saloon { get; set; }

    }
}

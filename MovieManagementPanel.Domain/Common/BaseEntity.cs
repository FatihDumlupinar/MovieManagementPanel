using System.ComponentModel.DataAnnotations;

namespace MovieManagementPanel.Domain.Common
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int CreateUserId { get; set; }
        public DateTime CreateDate { get; set; }

        public int? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; }

        public bool IsActive { get; set; } = true;

    }
}

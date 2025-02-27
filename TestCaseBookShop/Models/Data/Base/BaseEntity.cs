using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TestCaseBookShop.Models.Data.Enum;
using TestCaseBookShop.Models.Data.Auth;

namespace TestCaseBookShop.Models.Data.Base
{
    public class BaseEntity
    {
        [Required]
        [Key]
        [Column(Order = 1)]
        public Guid ID { get; set; } = Guid.NewGuid();

        [Column(Order = 1000)]
        public Guid? CreatedByID { get; set; }

        [ForeignKey(nameof(CreatedByID))]
        public virtual User CreatedBy { get; set; }

        [Column(Order = 1010)]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column(Order = 1020)]
        public Guid? UpdatedById { get; set; }

        [ForeignKey(nameof(UpdatedById))]
        public virtual User UpdatedBy { get; set; }

        [Column(Order = 1030)]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column(Order = 1040)]
        public RecordState RecordState { get; set; }
    }
}

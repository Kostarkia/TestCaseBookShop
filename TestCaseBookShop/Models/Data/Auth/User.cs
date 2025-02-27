using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TestCaseBookShop.Models.Data.Enum;

namespace TestCaseBookShop.Models.Data.Auth
{
    public class User
    {

        [Key]
        [Required]
        public Guid ID { get; set; }

        [Required]
        [Column(TypeName = "varchar(MAX)")]
        public string UserName { get; set; }

        public byte[] Password { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string? PhoneNumber { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? LastLogin { get; set; }

        public RecordState RecordState { get; set; } = RecordState.Active;
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TestCaseBookShop.Models.Data.Enum;

namespace TestCaseBookShop.Models.Data.Dto
{
    public class UserDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BookstoreAPI.Dto
{
    public class UpdatedPermissionDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
    }
}

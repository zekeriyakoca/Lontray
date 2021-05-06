using System.ComponentModel.DataAnnotations;

namespace Lontray.Services.Identity.API.Models.ManageViewModels
{
    public record VerifyPhoneNumberViewModel
    {
        [Required]
        public string Code { get; init; }

        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; init; }
    }
}

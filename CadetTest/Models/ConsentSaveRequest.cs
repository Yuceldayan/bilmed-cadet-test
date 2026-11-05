using System.ComponentModel.DataAnnotations;

namespace CadetTest.Models
{
    // degerler IYS izin tiplerine gore
    public class ConsentSaveRequest
    {
        [Required]
        [AllowedValues("EPOSTA", "MESAJ", "ARAMA")]
        public string Type { get; set; }

        [Required]
        [StringLength(254)]
        public string Recipient { get; set; }

        [Required]
        [AllowedValues("ONAY", "RET")]
        public string Status { get; set; }

        [Required]
        [AllowedValues("BIREYSEL", "TACIR")]
        public string RecipientType { get; set; }
    }
}

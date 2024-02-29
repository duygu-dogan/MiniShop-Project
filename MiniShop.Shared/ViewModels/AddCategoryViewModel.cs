using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiniShop.Shared.ViewModels
{
    public class AddCategoryViewModel
    {
        [JsonPropertyName("Name")]
        [DisplayName("Kategori")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
        [MinLength(5, ErrorMessage = "{0} alanına uzunluğu {1} karakterden az değer girilemez.")]
        [MaxLength(50, ErrorMessage = "{0} alanına uzunluğu {1} karakterden fazla değer girilemez.")]
        public string Name { get; set; }

        [JsonPropertyName("Description")]
        [DisplayName("Açıklama")]
        //[Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
        //[MinLength(20, ErrorMessage = "{0} alanına uzunluğu {1} karakterden az değer girilemez.")]
        //[MaxLength(200, ErrorMessage = "{0} alanına uzunluğu {2} karakterden az değer girilemez.")]
        public string Description { get; set; }

        [JsonPropertyName("Url")]
        public string Url { get; set; }

        [JsonPropertyName("IsActive")]
        [DisplayName("Aktif Kategori")]
        public bool IsActive { get; set; }
    }
}

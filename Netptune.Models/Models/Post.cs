using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Netptune.Models.Enums;
using Newtonsoft.Json;

namespace Netptune.Models.Models
{
    public class Post : BaseModel
    {

        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        [StringLength(4096)]
        public string Body { get; set; }

        public PostType Type { get; set; }

    #region ForeignKeys

        [Required]
        [ForeignKey ("Project")]
        public int ProjectId { get; set; }

    #endregion

    #region NavigationProperties

        [JsonIgnore]
        public virtual Project Project { get; set; }

    #endregion
    
    }
}

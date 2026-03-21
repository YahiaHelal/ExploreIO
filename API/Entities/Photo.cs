using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; } // for photo storage
        public AppUser AppUser{ get; set; } // fully define relation with the app-user
        public int AppUserId{ get; set; }
    }
}
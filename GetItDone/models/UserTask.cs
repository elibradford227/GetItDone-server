using System.ComponentModel.DataAnnotations;

namespace GetItDone.models
{
    public class UserTask
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Ownerid { get; set; }
        [Required]
        public User Owner { get; set; }
        [Required]
        public int TaskId { get; set; }
        [Required]
        public Task Task { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }
    }
}

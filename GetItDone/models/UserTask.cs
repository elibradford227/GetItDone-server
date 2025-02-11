using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetItDone.models
{
    public class UserTask
    {
        [Required]
        public int Id { get; set; }
        [ForeignKey("Task")]

        public int TaskId { get; set; }
        [Required]
        public Task Task { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        public User User { get; set; }
    }
}

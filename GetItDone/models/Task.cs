using System.ComponentModel.DataAnnotations;

namespace GetItDone.models
{
    public class Task
    {
        [Required]
        public int Id { get; set; }
        [Required]


        [MaxLength(120)]
        public string Title { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Ownerid { get; set; }
        [Required]
        public User Owner { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public List<UserTask>? Assignees { get; set; } = new List<UserTask>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace GetItDone.models.DTOs
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string Ownerid { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public List<UserTask>? Assignees { get; set; } = new List<UserTask>();
    }
}

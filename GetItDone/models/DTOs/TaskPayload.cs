using System.ComponentModel.DataAnnotations;
using static GetItDone.Controllers.TaskController;

namespace GetItDone.models.DTOs
{
    public class TaskPayload
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Ownerid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public List<AssigneePayload> Assignees { get; set; }
    }
}

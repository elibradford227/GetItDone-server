using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GetItDone.models.DTOs
{
    public class UserTaskDTO
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public TaskDTO Task { get; set; }
        public string UserId { get; set; }
        public UserDTO User { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}

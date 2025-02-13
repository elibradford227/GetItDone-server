namespace GetItDone.models.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string? Initials { get; set; }
        public List<UserTaskDTO>? Tasks { get; set; } = new List<UserTaskDTO>();
    }
}

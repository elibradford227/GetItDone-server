using AutoMapper;
using GetItDone.models;
using GetItDone.models.DTOs;

namespace GetItDone.Data
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<models.Task, TaskDTO>();
            CreateMap<UserTask, UserTaskDTO>();

            CreateMap<User, UserDTO>()
                .ForMember(user => user.Tasks, opt => opt.MapFrom(src => src.Tasks));
        }
    }
}

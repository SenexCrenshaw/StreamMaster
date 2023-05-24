using AutoMapper;

using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile)
    {
        //if (typeof(T) == typeof(VideoStream))
        //{
        //    profile.CreateMap<VideoStream, VideoStreamDto>()
        //    .ForMember(dest => dest.ChildVideoStreams, opt => opt.MapFrom(src => src.ChildRelationships.Select(cr => cr.ChildVideoStream)));
        //}

        //else
        //{
            _ = profile.CreateMap(typeof(T), GetType(), MemberList.None);
        //}
    }
}

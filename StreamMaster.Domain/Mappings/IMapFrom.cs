using AutoMapper;

namespace StreamMaster.Domain.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile)
    {

        if (typeof(T) == typeof(SMChannel))
        {
            profile.CreateMap<SMChannel, SMChannelDto>(MemberList.None)
              .ForMember(dest => dest.SMStreams, opt => opt.MapFrom(src => src.SMStreams
              .Where(cr => cr.SMStream != null)
              .Select(cr => cr.SMStream)
              ));

            return;

        }

        if (typeof(T) == typeof(SMStream))
        {
            profile.CreateMap<SMStream, SMStreamDto>().ForMember(dest => dest.SMStreamType, opt => opt.MapFrom(src => (int)src.SMStreamType));
            return;
        }

        _ = profile.CreateMap(typeof(T), GetType(), MemberList.None);
    }
}

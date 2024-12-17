using AutoMapper;

namespace StreamMaster.Domain.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile)
    {
        if (typeof(T) == typeof(SMChannel))
        {
            profile.CreateMap<SMChannel, SMChannelDto>(MemberList.None)
            //.ForMember(dest => dest.SMStreams, opt => opt.Ignore())
            //.ForMember(dest => dest.SMChannels, opt => opt.Ignore())
            .ForMember(dest => dest.SMStreamDtos, opt => opt.MapFrom(src => src.SMStreams
                .Where(cr => cr.SMStream != null)
                .Select(cr => cr.SMStream)))
            .ForMember(dest => dest.StreamGroupIds, opt => opt.MapFrom(src => src.StreamGroups
                .Where(cr => cr.StreamGroup != null)
                .Select(cr => cr.StreamGroup.Id)))
            .ForMember(dest => dest.SMChannelDtos, opt => opt.MapFrom(src => src.SMChannels
                .Where(ch => ch.SMChannel != null)
                .Select(ch => ch.SMChannel)));

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

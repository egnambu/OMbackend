using AutoMapper;
using OMbackend.Models;
using static OMbackend.Models.PostDTO;

namespace OMbackend.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // Map Conversation to ConversationDto

            // Map Conversation to ConversationDto
            CreateMap<Conversation, ConversationDto>()
                .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ID)) // Correct mapping of ID to ConversationId
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Shop.Name)) // Map Shop.Name to Name
                .ForMember(dest => dest.IsSeen, opt => opt.MapFrom(src => src.IsSeen));


            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.ID));

            CreateMap<PostCreateDto, Post>();
            CreateMap<Post, PostResponseDto>();

        }
    }
}

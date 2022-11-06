using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using ICode.Common;
using Models.DTO;
using System.Linq;

namespace API.Mapper
{
    public class SubmissionMapperConfig: Profile
    {
        public SubmissionMapperConfig()
        {
            CreateMap<Submission, SubmissionBase>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.State == SubmitState.Success))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => getState(src.State)));
            CreateMap<Submission, SubmissionDTO>()
               .IncludeBase<Submission, SubmissionBase>()
               .ForMember(dest => dest.Problem, opt => opt.MapFrom(src => src.SubmissionDetails.First().TestCase.Problem));
            CreateMap<Submission, SubmissionResult>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.State == SubmitState.Success))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => getState(src.State)));
            CreateMap<SubmissionDetail, SubmissionDetailDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.State == SubmitState.Success))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => getState(src.State)));
            CreateMap<PagingList<Submission>, PagingList<SubmissionDTO>>();
        }

        public string getState(SubmitState state)
        {
            switch (state)
            {
                case SubmitState.Success:
                    return "Success";
                case SubmitState.WrongAnswer:
                    return "Wrong Answer";
                case SubmitState.CompilerError:
                    return "Compiler Error";
                case SubmitState.RuntimeError:
                    return "Runtime Error";
                default:
                    return null;
            }
        }
    }
}

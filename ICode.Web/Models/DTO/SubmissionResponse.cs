using CodeStudy.Models;
using System.Collections.Generic;

namespace ICode.Web.Models.DTO
{
    public class SubmissionResponse
    {
        public SubmissionDTO Submission { get; set; }
        public List<SubmissionDetailDTO> Detail { get; set; } 
    }
}

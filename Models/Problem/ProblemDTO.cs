using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class ProblemDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TagDTO> Tags { get; set; }
        public override bool Equals(object obj)
        {
            ProblemDTO problem = obj as ProblemDTO;
            if (problem == null)
            {
                return false;
            }
            else
            {
                return problem.ID == ID;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name, Description, AuthorId, CreatedAt, UpdatedAt, Tags);
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ICode.CodeExecutor.Models
{
    public class CodeExecutorRequest
    {
        public string Code { get; set; }
        public string Lang { get; set; }
        public string Input { get; set; }

        public Dictionary<string, string[]> IsValid() {
            Dictionary<string, string[]> validationResult = new Dictionary<string, string[]>();
            if (string.IsNullOrEmpty(Code)) 
            {
                validationResult.Add("code", new string[] { "Code is required." });
            }
            if (string.IsNullOrEmpty(Lang))
            {
                validationResult.Add("lang", new string[] { "Language is required." });
            }
            return validationResult;
        }
    }
}
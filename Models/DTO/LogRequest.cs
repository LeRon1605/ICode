using System;
using System.Collections.Generic;

namespace Models.DTO
{
    public class LogRequest
    {
        public string UserId { get; set; }
        public string Method { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Path { get; set; }
        public string AccessIP { get; set; }
        public DateTime AccessTime { get; set; }
        public string UserAgent { get; set; }
        public IDictionary<string, object> ActionArguments { get; set; }
        public string FormatArgument()
        {
            string result = "";
            foreach (string key in ActionArguments.Keys)
            {
                result += $"{key} = {ActionArguments[key]}" + System.Environment.NewLine;
            }
            return result;
        }
        public override string ToString()
        {
            return $@"
                UserId     = {UserId}
                AccessIP   = {AccessIP}
                AccessTime = {AccessTime.ToString("dd/MM/yyyy hh:mm:ss")}
                User-Agent = {UserAgent}
                Path       = {Path}
                Method     = {Method}
                Controller = {Controller}
                Action     = {Action}
                Arguments  = {"{"}
                        {FormatArgument()}
               {"}"}
            ";
        }
    }
}

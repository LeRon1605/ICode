using System.Security.Cryptography;
using System.Text;

namespace ICode.CodeExecutor.Utils
{
    public class UniqueID
    {
        public static string GenerateID(string txt)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(txt));
            return new Guid(hash).ToString();
        }
    }
}

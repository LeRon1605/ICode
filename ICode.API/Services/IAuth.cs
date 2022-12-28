using Data.Entity;
using ICode.Common;
using System;
using System.Linq.Expressions;

namespace Services
{
    #region interface
    public interface IAuth
    {
        Expression<Func<User, bool>> Login(string name, string password);
    }
    public interface ILocalAuth: IAuth
    {

    }
    public interface IGoogleAuth: IAuth
    {

    }
    #endregion

    #region implementation
    public class LocalAuth : ILocalAuth
    {
        public Expression<Func<User, bool>> Login(string name, string password)
        {
            string passwordHashed = Encryptor.MD5Hash(password);
            return (x => (x.Username == name || x.Email == name) && passwordHashed == x.Password && x.Type == AccountType.Local); 
        }
    }

    public class GoogleAuth : IGoogleAuth
    {
        public Expression<Func<User, bool>> Login(string email, string password)
        {
            return (user => user.Email == email && password == Constant.PASSWORD_DEFAULT && user.Type == AccountType.Google);
        }
    }
    #endregion
}

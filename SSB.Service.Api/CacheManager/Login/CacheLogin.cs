using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SSB.Service.SSBApi.CacheManager.Login
{
    public class CacheLogin
    {
        #region props
        public static Dictionary<string, TokenModel> _cache = new Dictionary<string, TokenModel>();
        #endregion
        #region ctors
        public CacheLogin()
        {
            ClearCache();
        }
        #endregion
        #region public methods
        public bool HaveSession(string token)
        {
            var session = _cache.FirstOrDefault(o => o.Key.Equals(token));
            if (session.Value != null)
            {
                TokenModel tokenModel = session.Value;
                if (tokenModel.ExpireDate <= DateTime.Now)
                    return true;
                else
                    RemoveSession(token);
            }
            return false;
        }
        public void AddSession(string token, string username)
        {
            RemoveSession(token);
            var expireTime = int.Parse(ConfigurationManager.AppSettings["ExpireTime"] ?? "10");
            _cache.Add(token, new TokenModel(username, DateTime.Now.AddHours(expireTime)));
        }
        public void RemoveSession(string token)
            => _cache.Remove(token);
        public string GetUsername(string token)
        {
            var session = _cache.FirstOrDefault(o => o.Key.Equals(token)).Value;
            return session.Username;
        }


        #endregion
        #region private methods
        private void ClearCache() {
            try
            {
                if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour <= 9)
                {
                    _cache.ForEach(c =>
                    {
                        if (c.Value.ExpireDate.Day < DateTime.Now.Day) _cache.Remove(c.Key);
                        if (c.Value.ExpireDate.Month != DateTime.Now.Month)
                            _cache.Remove(c.Key);
                    });
                }
            }
            catch (Exception)
            {

               
            }
           
        }
        #endregion
    }
    public class TokenModel
    {
        public TokenModel(string username, DateTime expireDate)
        {
            Username = username;
            ExpireDate = expireDate;
        }
        public string Username { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
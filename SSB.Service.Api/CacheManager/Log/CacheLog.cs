
using SSB.Service.SSBApi.CacheManager.Login;
using System;
using System.Collections.Generic;

namespace SSB.Service.SSBApi.CacheManager.Log
{
    public class CacheLog
    {
        #region props
        public static Dictionary<string, LogModel> _cache = new Dictionary<string, LogModel>();
        #endregion
        #region ctors
        #endregion
        #region public methods
        public void Add(string key, LogModel value)
        {
            Remove(key);
            _cache.Add(key, value);
        }
       
        public LogModel Get(string key)
        {
            LogModel value ;
            var exist = _cache.TryGetValue(key, out value);
            return exist ? value : null;
        }
        public void Remove(string key)
            =>_cache.Remove(key);

        
        #endregion

    }
    public class LogModel
    {
        public long Id { get; set; }
        public DateTime  CreateTime { get; set; }
        public string Username { get; set; }
        public static LogModel New(long id, DateTime createTime, string username)
            => new LogModel() { Id=id,CreateTime=createTime,Username=username};
    }
}
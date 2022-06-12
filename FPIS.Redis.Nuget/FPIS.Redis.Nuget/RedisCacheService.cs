using StackExchange.Redis;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FPIS.Redis.Nuget
{
    public class RedisCacheService
    {
        Lazy<ConnectionMultiplexer> conn;
        public RedisCacheService(string cacheKey)
        {
            conn = new Lazy<ConnectionMultiplexer>(ConnectionMultiplexer.Connect(cacheKey));
        }
        public T GetFromCache<T>(string keyValue)
        {
            var db = conn.Value.GetDatabase();
            try
            {
                var cacheObj = db.StringGet(keyValue);
                if (cacheObj.HasValue && (string)cacheObj != null)
                {
                    var deserializedObject = JsonConvert.DeserializeObject<T>(cacheObj);

                    if (deserializedObject != null)
                        return deserializedObject;
                }

                return default(T);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void AddToCache<T1>(T1 data, string key, DateTime expirationDate)
        {
            TimeSpan expirationTime = expirationDate.TimeOfDay;
            var db = conn.Value.GetDatabase();
            var serializedObject = JsonConvert.SerializeObject(data);

            try
            {
                db.StringSet(key, serializedObject, expirationTime);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
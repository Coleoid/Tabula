using System.Configuration;

namespace Tabula
{
    //TODO:  Git magic to leave app.config in project yet ignored for commits?

    public interface IConfigWrapper
    {
        string GetValue(string key);
    }

    public class ConfigWrapper : IConfigWrapper
    {
        public string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}

﻿using System.Collections.Generic;
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
        public virtual string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }

    public class MockConfigWrapper : ConfigWrapper
    {
        public Dictionary<string, string> Entries = new Dictionary<string, string>();

        public override string GetValue(string key)
        {
            return Entries[key];
        }
    }
}

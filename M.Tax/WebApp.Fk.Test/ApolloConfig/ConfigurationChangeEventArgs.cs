using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Model;
using System.Collections.Generic;

namespace WebApp.Fk.Test.ApolloConfig
{
    public class ConfigurationChangeEventArgs
    {
        public IEnumerable<string> ChangedKeys => Changes.Keys;
        public bool IsChanged(string key) => Changes.ContainsKey(key);
        public IConfig Config { get; }
        public IReadOnlyDictionary<string, ConfigChange> Changes { get; }
        public ConfigurationChangeEventArgs(IConfig config, IReadOnlyDictionary<string, ConfigChange> changes)
        {
            Config = config;
            Changes = changes;
        }
        public ConfigChange GetChange(string key)
        {
            Changes.TryGetValue(key, out var change);
            return change;
        }
    }
}
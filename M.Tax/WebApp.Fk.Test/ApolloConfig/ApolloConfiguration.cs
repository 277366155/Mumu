using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Model;
using System;
using System.Globalization;

namespace WebApp.Fk.Test.ApolloConfig
{
    public class ApolloConfiguration: IConfiguration
    {
        private readonly string _defaultValue = null;

        private IConfig GetConfig(params string[] namespaces)
        {
            var config = namespaces == null || namespaces.Length == 0 ?
                ApolloConfigurationManager.GetAppConfig().GetAwaiter().GetResult() :
                ApolloConfigurationManager.GetConfig(namespaces).GetAwaiter().GetResult();

            config.ConfigChanged += OnChanged;
            return config;
        }
        private void OnChanged(object sender, ConfigChangeEventArgs changeEvent)
        {
            foreach (string key in changeEvent.ChangedKeys)
            {
                ConfigChange change = changeEvent.GetChange(key);
                Console.WriteLine("Change - key: {0}, oldValue: {1}, newValue: {2}, changeType: {3}", change.PropertyName, change.OldValue, change.NewValue, change.ChangeType);
            }
        }

        public string GetValue(string key, params string[] namespaces)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            var config = GetConfig(namespaces);
            return config.GetProperty(key, _defaultValue);
        }

        public TValue GetValue<TValue>(string key, params string[] namespaces)
        {
            var value = GetValue(key, namespaces);
            return value == null ?
                default(TValue) :
                (TValue)Convert.ChangeType(value, typeof(TValue), CultureInfo.InvariantCulture);
        }

        public string GetDefaultValue(string key, string defaultValue, params string[] namespaces)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            var config = GetConfig(namespaces);
            return config.GetProperty(key, defaultValue);
        }

        public TValue GetDefaultValue<TValue>(string key, TValue defaultValue, params string[] namespaces)
        {
            var value = GetDefaultValue(key, defaultValue, namespaces);
            return value == null ?
                default(TValue) :
                (TValue)Convert.ChangeType(value, typeof(TValue), CultureInfo.InvariantCulture);
        }
    }
}
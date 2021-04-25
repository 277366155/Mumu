using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Fk.Test.ApolloConfig
{
    interface IConfiguration
    {
        ///// <summary>
        ///// 配置更改回调事件。
        ///// </summary>
        //event EventHandler<ConfigurationChangeEventArgs> ConfigChanged;

        /// <summary>
        /// 获取配置项。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="namespaces">命名空间集合</param>
        /// <returns></returns>
        string GetValue(string key, params string[] namespaces);

        /// <summary>
        /// 获取配置项。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="namespaces">命名空间集合</param>
        /// <returns></returns>
        TValue GetValue<TValue>(string key, params string[] namespaces);

        /// <summary>
        /// 获取配置项，如果值为 <see cref="null"/> 则取参数 <see cref="defaultValue"/> 值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="namespaces">命名空间集合</param>
        /// <returns></returns>
        string GetDefaultValue(string key, string defaultValue, params string[] namespaces);

        /// <summary>
        /// 获取配置项，如果值为 <see cref="null"/> 则取参数 <see cref="defaultValue"/> 值。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="namespaces">命名空间集合</param>
        /// <returns></returns>
        TValue GetDefaultValue<TValue>(string key, TValue defaultValue, params string[] namespaces);
    }
}

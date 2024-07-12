using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Middle_WathchDog
{
    public class INI_Mgr
    {
        private IConfigurationRoot _config;
        private readonly string _filePath;

        public INI_Mgr(string filePath)
        {
            _filePath = filePath;

            // 설정 파일을 빌드합니다.
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddIniFile(_filePath);

            _config = builder.Build();
        }

        // 특정 키의 값을 읽어옵니다.
        public string? GetValue(string section, string key)
        {
            string? strData = string.Empty;

            if (_config.GetSection(section)?[key] != null)
                strData = _config.GetSection(section)?[key];

            return strData;
        }


        // 특정 키에 값을 쓰거나 수정합니다.
        public void SetValue(string section, string key, string value)
        {
            // 값을 설정하기 위해 IConfigurationRoot을 다시 빌드합니다.
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddIniFile(_filePath);

            var config = builder.Build();

            // 값을 설정합니다.
            config.GetSection(section)[key] = value;

            // 변경된 설정을 파일에 다시 씁니다.
            using (var stream = new StreamWriter(_filePath))
            {
                foreach (var pair in config.AsEnumerable())
                {
                    stream.WriteLine($"{pair.Key}={pair.Value}");
                }
            }
        }
    }
}

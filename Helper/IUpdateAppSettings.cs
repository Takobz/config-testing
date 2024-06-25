using Newtonsoft.Json.Linq;

namespace ConfigTesting.Helper
{
    interface IConfigurationUpdater 
    {
        void UpdateAppSettings(string section, string key, string newValue);
    }

    public class ConfigurationUpdater : IConfigurationUpdater
    {
        private readonly string _filePath;
        private readonly IConfigurationRoot _configRoot;

        public ConfigurationUpdater(IConfigurationRoot configRoot, string filePath)
        {
            _filePath = filePath;
            _configRoot = configRoot;
        }

        //builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        public void UpdateAppSettings(string section, string key, string newValue)
        {
            var json = File.ReadAllText(_filePath);
            var jsonObj = JObject.Parse(json);

            if (jsonObj.SelectToken(section) is JObject sectionToken)
            {
                sectionToken[key] = newValue;
                File.WriteAllText(_filePath, jsonObj.ToString());
                _configRoot.Reload();
            }
            else
            {
                throw new ArgumentException($"Section '{section}' not found in configuration.");
            }
        }
    }
}

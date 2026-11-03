using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EgorkaGame.Egorka
{
    public class EgorkaSettings : ConfigurationSection, IEgorkaSettings
    {
        #region Statics

        #region Properties

        public static EgorkaSettings Instance =>
            // read http://csharpindepth.com/articles/general/singleton.aspx for more info
            Lazy.Value;

        #endregion

        #region Fields

        private static readonly Lazy<EgorkaSettings> Lazy = new Lazy<EgorkaSettings>(Load);

        #endregion

        #endregion

        #region Constructors

        private EgorkaSettings()
        {
        }

        #endregion

        #region Private methods

        private static EgorkaSettings Load()
        {
            var section = (EgorkaSettings)ConfigurationManager.GetSection("egorkaSettings");
            if (section != null)
            {
                return section;
            }

            // Test runners (MSTest v2 / vstest) execute in-proc, so ConfigurationManager
            // resolves against the runner's own config and this section is absent there.
            // Fall back to the config files deployed next to this assembly.
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (directory == null)
            {
                return null;
            }

            foreach (var configFile in Directory
                .GetFiles(directory, "*.config")
                .OrderByDescending(f => f.EndsWith(".dll.config", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    var map = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
                    var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                    var candidate = (EgorkaSettings)config.GetSection("egorkaSettings");
                    if (candidate != null)
                    {
                        return candidate;
                    }
                }
                catch (ConfigurationErrorsException)
                {
                }
            }

            return null;
        }

        #endregion Private methods

        #region Public properties
        
        [ConfigurationProperty("culture")]
        public CultureInfo CultureInfo
        {
            get => (CultureInfo)this["culture"];
            set
            {
            }
        }

        [ConfigurationProperty("IsSpeechEnabled")]
        public  bool IsSpeechEnabled => (bool)this["IsSpeechEnabled"];

        [ConfigurationProperty("SpeechVolume")]
        public int SpeechVolume => (int)this["SpeechVolume"];

        [ConfigurationProperty("SpeechRate")]
        public int SpeechRate => (int)this["SpeechRate"];

        [ConfigurationProperty("SpeechIntro")]
        public string SpeechIntro => (string)this["SpeechIntro"];

        #endregion
    }
}
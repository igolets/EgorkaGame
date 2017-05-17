using System;
using System.Configuration;
using System.Globalization;

namespace EgorkaGame.Egorka
{
    public class EgorkaSettings : ConfigurationSection, IEgorkaSettings
    {
        #region Statics

        #region Properties

        public static EgorkaSettings Instance
        {
            get
            {
                // read http://csharpindepth.com/articles/general/singleton.aspx for more info
                return Lazy.Value;
            }
        }

        #endregion

        #region Fields

        private static readonly Lazy<EgorkaSettings> Lazy = new Lazy<EgorkaSettings>(() => (EgorkaSettings)ConfigurationManager.GetSection("egorkaSettings"));

        #endregion

        #endregion

        #region Constructors

        private EgorkaSettings()
        {
        }

        #endregion

        #region Public properties
        
        [ConfigurationProperty("culture")]
        public CultureInfo CultureInfo
        {
            get
            {
                return (CultureInfo)this["culture"];
            }
            set
            {
            }
        }

        [ConfigurationProperty("IsSpeechEnabled")]
        public  bool IsSpeechEnabled
        {
            get
            {
                return (bool)this["IsSpeechEnabled"];
            }
        }

        [ConfigurationProperty("SpeechVolume")]
        public int SpeechVolume
        {
            get
            {
                return (int)this["SpeechVolume"];
            }
        }

        [ConfigurationProperty("SpeechRate")]
        public int SpeechRate
        {
            get
            {
                return (int)this["SpeechRate"];
            }
        }

        [ConfigurationProperty("SpeechIntro")]
        public string SpeechIntro
        {
            get
            {
                return (string)this["SpeechIntro"];
            }
        }

        #endregion
    }
}
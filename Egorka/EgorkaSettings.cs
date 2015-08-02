using System.Configuration;
using System.Globalization;

namespace EgorkaGame.Egorka
{
    public class EgorkaSettings : ConfigurationSection
    {
        #region Statics

        #region Properties

        public static EgorkaSettings Instance
        {
            get
            {
                return _instance ?? (_instance = (EgorkaSettings)ConfigurationManager.GetSection("egorkaSettings"));
            }
        }

        #endregion

        #region Fields

        private static EgorkaSettings _instance;

        #endregion

        #endregion

        #region Constructors

        private EgorkaSettings()
        {
        }

        #endregion

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

    }
}
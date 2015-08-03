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
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null)
                        {
                            _instance = (EgorkaSettings)ConfigurationManager.GetSection("egorkaSettings");
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Fields

        private static volatile EgorkaSettings _instance;
        private static readonly object Locker = new object();

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

        #endregion
    }
}
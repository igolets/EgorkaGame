using System;
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
                // read http://csharpindepth.com/articles/general/singleton.aspx for more info
                return Lazy.Value;
            }
        }

        #endregion

        #region Fields

        private static readonly Lazy<EgorkaSettings> Lazy = new Lazy<EgorkaSettings>(() => new EgorkaSettings());

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
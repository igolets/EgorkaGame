using System.Globalization;

namespace EgorkaGame.Egorka
{
    public interface IEgorkaSettings
    {
        CultureInfo CultureInfo
        {
            get;
        }

        bool IsSpeechEnabled
        {
            get;
        }

        int SpeechVolume
        {
            get;
        }

        int SpeechRate
        {
            get;
        }

        string SpeechIntro
        {
            get;
        }
    }
}
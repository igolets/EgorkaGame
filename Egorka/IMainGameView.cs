using System.Drawing;

namespace EgorkaGame.Egorka
{
    /// <summary>
    /// View interface implementing MVC pattern
    /// </summary>
    public interface IMainGameView
    {
        Color ScreenColor
        {
            get;
            set;
        }

        string DisplayText
        {
            get;
            set;
        }

        void PlaySound(int beepFrequency, int beepDurationInMs);

        void ReadAloud(char character);

        void SubscribeGlobalEvents();

    }
}
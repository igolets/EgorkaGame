using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EgorkaGame.Egorka
{
    /// <summary>
    /// Game controller responsible on handling game logic
    /// </summary>
    public class MainGameController
    {
        #region Consts

        private const int BeepDurationInMs = 100;

        #endregion

        #region Constructors

        public MainGameController(IMainGameView view)
        {
            _view = view;
            _view.SubscribeGlobalEvents();
        }

        #endregion

        #region Public properties

        public Dictionary<Keys, CharData> Chars
        {
            get
            {
                return _chars;
            }
        }

        #endregion

        #region Public methods

        public bool IsShouldSupressKey(Keys keyCode, bool alt, bool control)
        {
            // заблокировать кнопку Windows
            if (keyCode == Keys.LWin || keyCode == Keys.RWin)
            {
                return true;
            }
            // заблокировать Alt-Tab
            if (keyCode == Keys.Tab && alt)
            {
                return true;
            }

            // заблокировать Ctr-Esc
            if (keyCode == Keys.Escape && control)
            {
                return true;
            }
            return false;
        }

        public bool IsShouldSupressMouse(MouseButtons button)
        {
            return button == MouseButtons.Right;
        }

        public bool IsShouldSupressMouseWheel()
        {
            return true;
        }

        public void LoadKeysResources(string keysResource)
        {
            // load resources
            var lines = keysResource.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var i = 0;
            foreach (var line in lines)
            {
                var parts = line.Split('\0');
                _chars.Add((Keys)Enum.Parse(typeof(Keys), parts[0]),
                    new CharData
                    {
                        Character = parts[1][0],
                        Color = Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), parts[2])),
                        BeepFrequency = CalcKeyFrequency(i++)
                    });
            }
        }

        public void ProcessKey(Keys keyCode)
        {
            if (_chars.Keys.Contains(keyCode))
            {
                _view.ScreenColor = _chars[keyCode].Color;
                _view.DisplayText = _chars[keyCode].Character.ToString();
                _view.PlaySound(_chars[keyCode].BeepFrequency, BeepDurationInMs);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Calculates the key frequency.
        /// </summary>
        /// <param name="keyIndex">Index of the key.</param>
        /// <returns></returns>
        /// <remarks>See https://en.wikipedia.org/wiki/Piano_key_frequencies for a table and math</remarks>
        private int CalcKeyFrequency(int keyIndex)
        {
            var herz = Math.Pow(2, (((double)keyIndex - 49 + 25 /*First notable sound*/) / 12)) * 440;
            return (int)Math.Round(herz, MidpointRounding.AwayFromZero);
        }

        #endregion

        #region Fields

        private readonly Dictionary<Keys, CharData> _chars = new Dictionary<Keys, CharData>();
        private readonly IMainGameView _view;

        #endregion
    }
}
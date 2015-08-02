using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EgorkaGame.BuildDictionary
{
    public partial class KeyLoggingForm : Form
    {
        #region Constructors

        public KeyLoggingForm()
        {
            InitializeComponent();
            ActiveControl = null;
        }

        #endregion Constructors

        #region Private methods

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            _keyCode = e.KeyCode;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            var character = e.KeyChar.ToString().ToUpper()[0];
            _chars[_keyCode] = character;
            lblDicSize.Text = _chars.Count.ToString();
            lblLastPair.Text = String.Format("{0} - {1}", _keyCode, character);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _chars.Clear();
            lblDicSize.Text = lblLastPair.Text = "";
            ActiveControl = null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
            ActiveControl = null;
        }

        private bool SaveData()
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            var colorsArray = Enum.GetValues(typeof(KnownColor));
            var allColors = new KnownColor[colorsArray.Length];
            Array.Copy(colorsArray, allColors, colorsArray.Length);
            var startInd = Array.IndexOf(allColors, KnownColor.Black) + 1;

            using (var file = File.CreateText(saveFileDialog1.FileName))
            {
                foreach (var pair in _chars)
                {
                    file.WriteLine("{0}\0{1}\0{2}", pair.Key, pair.Value, allColors[startInd++]);
                }
                file.Close();
            }
            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_chars.Count > 0)
            {
                if (MessageBox.Show(
                        @"Сохранить введенные данные?",
                        @"Подтвердите закрытие",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                if (!SaveData())
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion Private methods

        #region Fields

        private Keys _keyCode;
        private readonly Dictionary<Keys, Char> _chars = new Dictionary<Keys, char>();

        #endregion Fields
    }
}
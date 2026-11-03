using System;
using System.Diagnostics;
using System.Drawing;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows.Forms;
using EgorkaGame.Egorka.Properties;
using Gma.System.MouseKeyHook;

namespace EgorkaGame.Egorka
{
    /// <summary>
    ///     Main fullscreen form, implements View
    /// </summary>
    public partial class FullScreenForm : Form, IMainGameView
    {
        #region Statics

        #region Fields

        private static readonly object Locker = new object();

        #endregion Fields

        #endregion Statics

        #region Constructors

        public FullScreenForm()
        {
            InitializeComponent();
            lblLetter.Text = string.Empty;

            _synthesizer = new SpeechSynthesizer();
            _synthesizer.SelectVoiceByHints(VoiceGender.NotSet, VoiceAge.Adult, 0, EgorkaSettings.Instance.CultureInfo);
            _synthesizer.Volume = EgorkaSettings.Instance.SpeechVolume;  // 0...100
            _synthesizer.Rate = EgorkaSettings.Instance.SpeechRate;     // -10...10
        }

        #endregion Constructors

        #region Public properties

        #region IMainGameView implementation

        public Color ScreenColor
        {
            get => BackColor;
            set => BackColor = value;
        }

        public string DisplayText
        {
            get => lblLetter.Text;
            set => lblLetter.Text = value;
        }

        #endregion IMainGameView implementation

        #endregion

        #region Private methods

        private void Form1_Load(object sender, EventArgs e)
        {
            // set up full screen mode
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            var screen = Screen.PrimaryScreen;
            Bounds = screen.Bounds;

            // to remove flickering http://stackoverflow.com/questions/9408038/flickering-on-background-colour-change
            DoubleBuffered = true;

            _controller = new MainGameController(this);
            _controller.LoadKeysResources(Resources.keys);

            if (EgorkaSettings.Instance.IsSpeechEnabled && !string.IsNullOrEmpty(EgorkaSettings.Instance.SpeechIntro))
            {
                _synthesizer.Speak(EgorkaSettings.Instance.SpeechIntro);
            }
        }

        /// <summary>
        ///     Logs the specified text to debug log.
        /// </summary>
        /// <param name="text">The text.</param>
        private void Log(string text)
        {
            if (!Debugger.IsAttached || IsDisposed)
            {
                return;
            }
            Debug.Write(text);
        }

        #region IMainGameView implementation

        bool IMainGameView.IsVoiceInProgress => _prompt != null && !_prompt.IsCompleted;


        void IMainGameView.PlaySound(int beepFrequency, int beepDurationInMs)
        {
            var soundTask = new Thread(() =>
            {
                lock (Locker)
                {
                    Console.Beep(beepFrequency, beepDurationInMs);
                }
            });
            soundTask.Start();
        }

        void IMainGameView.ReadAloud(char character)
        {
            _prompt = _synthesizer.SpeakAsync(character.ToString());
        }

        /// <summary>
        ///     Re-Subscribes to all events.
        /// </summary>
        void IMainGameView.SubscribeGlobalEvents()
        {
            Unsubscribe();
            Subscribe(Hook.GlobalEvents());
        }

        #endregion IMainGameView implementation

        #region Handle input devices

        /// <summary>
        ///     Subscribes the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        private void Subscribe(IKeyboardMouseEvents events)
        {
            _events = events;
            _events.KeyDown += OnKeyDown;
            _events.KeyUp += OnKeyUp;
            _events.KeyPress += HookManager_KeyPress;

            _events.MouseUp += OnMouseUp;
            _events.MouseClick += OnMouseClick;
            _events.MouseDoubleClick += OnMouseDoubleClick;

            _events.MouseMove += HookManager_MouseMove;

            _events.MouseWheelExt += HookManager_MouseWheelExt;

            _events.MouseDownExt += HookManager_SuppressMouse;
        }

        /// <summary>
        ///     Unsubscribes events.
        /// </summary>
        private void Unsubscribe()
        {
            if (_events == null)
            {
                return;
            }
            _events.KeyDown -= OnKeyDown;
            _events.KeyUp -= OnKeyUp;
            _events.KeyPress -= HookManager_KeyPress;

            _events.MouseUp -= OnMouseUp;
            _events.MouseClick -= OnMouseClick;
            _events.MouseDoubleClick -= OnMouseDoubleClick;

            _events.MouseMove -= HookManager_MouseMove;

            _events.MouseWheelExt -= HookManager_MouseWheelExt;

            _events.MouseDownExt -= HookManager_SuppressMouse;

            _events.Dispose();
            _events = null;
        }

        /// <summary>
        ///     Suppress mouse keys.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void HookManager_SuppressMouse(object sender, MouseEventExtArgs e)
        {
            if (_controller.IsShouldSuppressMouse(e.Button))
            {
                Log($"MouseDown \t\t {e.Button} Suppressed\n");
                e.Handled = true;
                return;
            }

            Log($"MouseDown \t\t {e.Button}\n");
        }

        /// <summary>
        ///     Called when [key down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Log($"KeyDown  \t\t {e.KeyCode}\n");

            if (_controller.IsShouldSuppressKey(e.KeyCode, e.Alt, e.Control))
            {
                e.Handled = true;
                return;
            }

            _controller.ProcessKey(e.KeyCode);
        }

        /// <summary>
        ///     Called when [key up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            Log($"KeyUp  \t\t {e.KeyCode}\n");
        }

        /// <summary>
        ///     Handles the KeyPress event of the HookManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs" /> instance containing the event data.</param>
        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            Log($"KeyPress \t\t {e.KeyChar}\n");
        }

        /// <summary>
        ///     Handles the MouseMove event of the HookManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        ///     Called when [mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Log($"MouseUp \t\t {e.Button}\n");
        }

        /// <summary>
        ///     Called when [mouse click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            Log($"MouseClick \t\t {e.Button}\n");
        }

        /// <summary>
        ///     Called when [mouse double click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            Log($"MouseDoubleClick \t\t {e.Button}\n");
        }

        /// <summary>
        ///     Hooks the manager_ mouse wheel ext.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void HookManager_MouseWheelExt(object sender, MouseEventExtArgs e)
        {
            if (_controller.IsShouldSuppressMouseWheel())
            {
                Log("Mouse Wheel Move Suppressed.\n");
                e.Handled = true;
            }
        }

        #endregion Handle input devices

        #endregion Private methods

        #region Fields

        private MainGameController _controller;
        private IKeyboardMouseEvents _events;
        private readonly SpeechSynthesizer _synthesizer;
        private Prompt _prompt;

        #endregion Fields
    }
}
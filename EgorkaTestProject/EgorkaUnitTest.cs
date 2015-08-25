using System.Drawing;
using System.Windows.Forms;
using EgorkaGame.Egorka;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace EgorkaGame.EgorkaTestProject
{
    [TestClass]
    public class EgorkaUnitTest
    {
        #region Consts

        private const string TestKeyFileResource =
            "Q\0Й\0DarkCyan\n" +
            "W\0Ц\0DarkGoldenrod\n" +
            "E\0У\0DarkGray\n" +
            "R\0К\0DarkGreen\n" +
            "T\0Е\0DarkKhaki\n" +
            "Y\0Н\0DarkMagenta\n" +
            "U\0Г\0DarkOliveGreen\n" +
            "I\0Ш\0DarkOrange\n" +
            "O\0Щ\0DarkOrchid\n" +
            "P\0З\0DarkRed\n";

        #endregion

        #region Public methods

        [TestMethod]
        public void SubscribeToEventsTest()
        {
            var view = new Mock<IMainGameView>();
            view.Setup(x => x.SubscribeGlobalEvents());
            var controller = new MainGameController(view.Object);
            view.VerifyAll();
        }


        [TestMethod]
        public void LoadKeysEmptyFileTest()
        {
            var view = new Mock<IMainGameView>();
            var controller = new MainGameController(view.Object);
            controller.LoadKeysResources("");

            controller.Chars.Count.ShouldBe(0);
        }

        [TestMethod]
        public void LoadKeysFileTest()
        {
            var view = new Mock<IMainGameView>();
            var controller = new MainGameController(view.Object);
            controller.LoadKeysResources(TestKeyFileResource);

            controller.Chars.Count.ShouldBe(10);
            var test = controller.Chars[Keys.Q];
            test.Color.ShouldBe(Color.FromKnownColor(KnownColor.DarkCyan));
            test.Character.ShouldBe('Й');
            test = controller.Chars[Keys.P];
            test.Color.ShouldBe(Color.FromKnownColor(KnownColor.DarkRed));
            test.Character.ShouldBe('З');
        }

        [TestMethod]
        public void ProcessKeyTest()
        {
            var view = new Mock<IMainGameView>();
            view.Setup(x => x.PlaySound(It.IsAny<int>(), 100));

            var controller = new MainGameController(view.Object);
            controller.LoadKeysResources(TestKeyFileResource);

            controller.ProcessKey(Keys.R);
            view.VerifyAll();
        }

        [TestMethod]
        public void CalculateFrequencyTest()
        {
            var view = new Mock<IMainGameView>();

            var controller = new MainGameController(view.Object);
            controller.LoadKeysResources(TestKeyFileResource);

            view.Setup(x => x.PlaySound(It.Is<int>(f => f == 110), 100));
            controller.ProcessKey(Keys.Q); //key#0
            view.VerifyAll();

            view.Reset();
            view.Setup(x => x.PlaySound(It.Is<int>(f => f == 117), 100));
            controller.ProcessKey(Keys.W); //key#0
            view.VerifyAll();

            view.Reset();
            view.Setup(x => x.PlaySound(It.Is<int>(f => f == 175), 100));
            controller.ProcessKey(Keys.O); //key#8
            view.VerifyAll();

            view.Reset();
            view.Setup(x => x.PlaySound(It.Is<int>(f => f == 185), 100));
            controller.ProcessKey(Keys.P); //key#9
            view.VerifyAll();
        }

        [TestMethod]
        public void SupressMouseWheelTest()
        {
            var view = new Mock<IMainGameView>();
            var controller = new MainGameController(view.Object);

            controller.IsShouldSupressMouseWheel().ShouldBe(true);
        }

        [TestMethod]
        public void SupressRightMouseClickTest()
        {
            var view = new Mock<IMainGameView>();
            var controller = new MainGameController(view.Object);

            controller.IsShouldSupressMouse(MouseButtons.Left).ShouldBe(false);
            controller.IsShouldSupressMouse(MouseButtons.Middle).ShouldBe(false);
            controller.IsShouldSupressMouse(MouseButtons.Right).ShouldBe(true);
            controller.IsShouldSupressMouse(MouseButtons.Middle).ShouldBe(false);
            controller.IsShouldSupressMouse(MouseButtons.None).ShouldBe(false);
            controller.IsShouldSupressMouse(MouseButtons.XButton1).ShouldBe(false);
            controller.IsShouldSupressMouse(MouseButtons.XButton2).ShouldBe(false);
        }

        [TestMethod]
        public void SupressTaskManagerTest()
        {
            var view = new Mock<IMainGameView>();
            var controller = new MainGameController(view.Object);

            controller.IsShouldSupressKey(Keys.Escape, true, true).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.Escape, false, true).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.Escape, true, false).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.Escape, false, false).ShouldBe(false);
        }

        [TestMethod]
        public void SupressTaskSwitchTest()
        {
            var view = new Mock<IMainGameView>();
            var controller = new MainGameController(view.Object);

            controller.IsShouldSupressKey(Keys.Tab, true, true).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.Tab, true, false).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.Tab, false, true).ShouldBe(false);
            controller.IsShouldSupressKey(Keys.Tab, false, false).ShouldBe(false);
        }

        [TestMethod]
        public void SupressWindowsKeyTest()
        {
            var view = new Mock<IMainGameView>();
            var controller = new MainGameController(view.Object);

            controller.IsShouldSupressKey(Keys.LWin, true, true).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.LWin, true, false).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.LWin, false, true).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.LWin, false, false).ShouldBe(true);

            controller.IsShouldSupressKey(Keys.RWin, true, true).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.RWin, true, false).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.RWin, false, true).ShouldBe(true);
            controller.IsShouldSupressKey(Keys.RWin, false, false).ShouldBe(true);
        }

        #endregion
    }
}
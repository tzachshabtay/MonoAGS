using AGS.API;
using AGS.Engine;
using NUnit.Framework;
using Moq;
using AGS.Engine.Desktop;

namespace Tests
{
    [TestFixture]
    public class TextBoxComponentTests
    {
        [TestCase("a", Key.A)]
        [TestCase("abcd", Key.A, Key.B, Key.C, Key.D)]
        [TestCase("efgh", Key.E, Key.A, Key.BackSpace, Key.F, Key.G, Key.B, Key.C, Key.BackSpace, Key.BackSpace, Key.H)]
        [TestCase("ijklmnop", Key.BackSpace, Key.M, Key.N, Key.O, Key.P, Key.Home, Key.I, Key.J, Key.K, Key.L)]
        [TestCase("qrstuv", Key.Right, Key.Q, Key.T, Key.Left, Key.S, Key.Home, Key.Right, Key.R, Key.End, Key.U, Key.V)]
        [TestCase("wxyz", Key.W, Key.Up, Key.F7, Key.X, Key.F1, Key.Y, Key.Z)]

        [TestCase("A", Key.ShiftLeft, Key.A)]
        [TestCase("ABCD", Key.ShiftLeft, Key.A, Key.B, Key.C, Key.D)]
        [TestCase("EFGH", Key.ShiftLeft, Key.E, Key.A, Key.BackSpace, Key.F, Key.G, Key.B, Key.C, Key.BackSpace, Key.BackSpace, Key.H)]
        [TestCase("IJKLMNOP", Key.ShiftRight, Key.BackSpace, Key.M, Key.N, Key.O, Key.P, Key.Home, Key.I, Key.J, Key.K, Key.L)]
        [TestCase("QRSTUV", Key.ShiftRight, Key.Right, Key.Q, Key.T, Key.Left, Key.S, Key.Home, Key.Right, Key.R, Key.End, Key.U, Key.V)]
        [TestCase("WXYZ", Key.ShiftRight, Key.W, Key.Up, Key.F7, Key.X, Key.F1, Key.Y, Key.Z)]

        [TestCase("12345^&*()09876%$#@!", Key.Number1, Key.Number2, Key.Number3, Key.Number4, Key.Number5, Key.ShiftLeft,
            Key.Number6, Key.Number7, Key.Number8, Key.Number9, Key.Number0, Key.LShift,
            Key.Number0, Key.Number9, Key.Number8, Key.Number7, Key.Number6, Key.RShift,
            Key.Number5, Key.Number4, Key.Number3, Key.Number2, Key.Number1)]

        [TestCase("`-=[]\\;',./~_+{}|:\"<>?", Key.Tilde, Key.Minus, Key.Plus, Key.BracketLeft, Key.BracketRight, Key.BackSlash,
            Key.Semicolon, Key.Quote, Key.Comma, Key.Period, Key.Slash, Key.LShift,
            Key.Tilde, Key.Minus, Key.Plus, Key.BracketLeft, Key.BracketRight, Key.BackSlash,
            Key.Semicolon, Key.Quote, Key.Comma, Key.Period, Key.Slash)]

        [TestCase("/*-+.0123456789/*-+.0123456789", Key.Delete, Key.KeypadDivide, Key.KeypadMultiply, Key.KeypadMinus, Key.KeypadPlus, Key.KeypadPeriod,
            Key.Keypad0, Key.Keypad1, Key.Keypad2, Key.Keypad3, Key.Keypad4, Key.Keypad5, Key.Keypad6, Key.Keypad7, Key.Keypad8, Key.Keypad9, Key.LShift,
            Key.KeypadDivide, Key.KeypadMultiply, Key.KeypadMinus, Key.KeypadPlus, Key.KeypadPeriod,
            Key.Keypad0, Key.Keypad1, Key.Keypad2, Key.Keypad3, Key.Keypad4, Key.Keypad5, Key.Keypad6, Key.Keypad7, Key.Keypad8, Key.Keypad9, Key.LShift)]

        [TestCase("", Key.A, Key.A, Key.A, Key.BackSpace, Key.BackSpace, Key.BackSpace)]
        [TestCase("aaa", Key.A, Key.A, Key.A, Key.Delete, Key.Delete, Key.Delete)]
        [TestCase("", Key.A, Key.A, Key.A, Key.Home, Key.Delete, Key.Delete, Key.Delete)]
        [TestCase("ad", Key.A, Key.B, Key.C, Key.D, Key.Home, Key.Right, Key.Delete, Key.Delete)]
        public void TextBox_RespondsToKeys_Test(string expectedText, params Key[] keys)
        {                                    
            Mock<IInput> input = new Mock<IInput>();
            AGSEvent<KeyboardEventArgs> keyDown = new AGSEvent<KeyboardEventArgs>();
            AGSEvent<KeyboardEventArgs> keyUp = new AGSEvent<KeyboardEventArgs>();
            AGSEvent<MouseButtonEventArgs> mouseDown = new AGSEvent<MouseButtonEventArgs>();
            AGSEvent<MouseButtonEventArgs> mouseDownOutside = new AGSEvent<MouseButtonEventArgs>();
            AGSEvent<MouseButtonEventArgs> mouseUp = new AGSEvent<MouseButtonEventArgs>();
            input.Setup(i => i.KeyDown).Returns(keyDown);
            input.Setup(i => i.KeyUp).Returns(keyUp);            

            Mock<IObject> entity = new Mock<IObject>();
            Mock<ITextComponent> textComponent = new Mock<ITextComponent>();
            string actualText = "";
            textComponent.Setup(t => t.Text).Returns(() => actualText);
            textComponent.SetupSet(t => t.Text = It.IsAny<string>()).Callback<string>(text => actualText = text);
            Mock<IUIEvents> uiEvents = new Mock<IUIEvents>();
            uiEvents.Setup(i => i.MouseDown).Returns(mouseDown);
            uiEvents.Setup(i => i.MouseUp).Returns(mouseUp);
            uiEvents.Setup(i => i.LostFocus).Returns(mouseDownOutside);
            Mock<IInObjectTree> inTree = new Mock<IInObjectTree>();
            entity.Setup(e => e.GetComponent<ITextComponent>()).Returns(textComponent.Object);
            entity.Setup(e => e.GetComponent<IUIEvents>()).Returns(uiEvents.Object);
            entity.Setup(e => e.GetComponent<IInObjectTree>()).Returns(inTree.Object);
            Mock<IGame> game = new Mock<IGame>();
            Mock<IGameFactory> factory = new Mock<IGameFactory>();
            Mock<IUIFactory> uiFactory = new Mock<IUIFactory>();
            Mock<IGameEvents> gameEvents = new Mock<IGameEvents>();
            gameEvents.Setup(g => g.OnBeforeRender).Returns(new Mock<IBlockingEvent<AGSEventArgs>>().Object);
            gameEvents.Setup(g => g.OnRepeatedlyExecute).Returns(new Mock<IEvent<AGSEventArgs>>().Object);
            game.Setup(g => g.Events).Returns(gameEvents.Object);
            game.Setup(g => g.Factory).Returns(factory.Object);
            factory.Setup(f => f.UI).Returns(uiFactory.Object);
            Mock<ILabel> label = new Mock<ILabel>();
            Mock<ITreeNode<IObject>> tree = new Mock<ITreeNode<IObject>>();
            Mock<IFocusedUI> focusedUi = new Mock<IFocusedUI>();
            label.Setup(l => l.TreeNode).Returns(tree.Object);
            uiFactory.Setup(u => u.GetLabel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<float>(),
                It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<ITextConfig>(), It.IsAny<bool>())).
                Returns(label.Object);
            

            AGSTextBoxComponent textbox = new AGSTextBoxComponent(new AGSEvent<AGSEventArgs>(), new AGSEvent<TextBoxKeyPressingEventArgs>(), 
                                                  input.Object, game.Object, new DesktopKeyboardState(), focusedUi.Object);
            textbox.Init(entity.Object);
            textbox.IsFocused = true;
            bool isShiftDown = false;

            foreach (var key in keys)
            {
                if (key == Key.ShiftLeft || key == Key.ShiftRight)
                {
                    isShiftDown = !isShiftDown;
                    if (!isShiftDown) { keyUp.Invoke(this, new KeyboardEventArgs(key)); continue; }
                }
                keyDown.Invoke(this, new KeyboardEventArgs(key));
            }

            Assert.AreEqual(expectedText, actualText);
        }
    }
}

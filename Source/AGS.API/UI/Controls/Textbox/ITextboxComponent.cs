namespace AGS.API
{
    /// <summary>
    /// This component allows us to create a textbox control (a UI control which allows keyboard input to print text on screen).
    /// </summary>
    [RequiredComponent(typeof(ITextComponent))]
    [RequiredComponent(typeof(IUIEvents))]
    [RequiredComponent(typeof(ITranslateComponent))]
    [RequiredComponent(typeof(IDrawableInfoComponent))]
    [RequiredComponent(typeof(IInObjectTreeComponent))]
    [RequiredComponent(typeof(IHasRoomComponent))]
    [RequiredComponent(typeof(IImageComponent))]
    [RequiredComponent(typeof(IVisibleComponent))]
    [RequiredComponent(typeof(IAnimationComponent), false)]
    [RequiredComponent(typeof(IBorderComponent), false)]
    public interface ITextBoxComponent : IComponent
    {
        /// <summary>
        /// This event can be subscribed to get notifications when keys are pressed.
        /// You can also use this event to cancel processing (or perform special processing) for specific keys.
        /// <example>
        /// Let's say, for example, that we don't want our textbox to allow spaces.
        /// First, we'll subscribe to the event. Then, if a space key is pressed, we'll cancel its processing.
        /// Also, let's say that our textbox is inside a dialog, and we want the enter key to close the dialog (and then do something with the text).
        /// We'll listen to the enter key in the same dialog to close it.
        /// <code language="lang-csharp">
        /// _myTextbox.OnPressingKey.Subscribe(onPressingKey);
        /// ...
        /// ...
        /// private void onPressingKey(TextboxKeyPressingEventArgs args)
        /// {
        ///     if (args.Key == Key.Space)
        ///     {
        ///         args.ShouldCancel = true;
        ///         return;
        ///     }
        ///     if (args.Key == Key.Enter)
        ///     {
        ///         args.ShouldCancel = true;
        ///         _myTextbox.IsFocused = false;
        ///         _myDialog.Close();
        ///         doSomethingWithText(_myTextbox.Text);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </summary>
        IBlockingEvent<TextBoxKeyPressingEventArgs> OnPressingKey { get; }

        /// <summary>
        /// Gets or sets whether the textbox is focused or not. Only when the textbox is focused it can receive input from the keyboard 
        /// (and the caret is shown).
        /// The textbox automatically gets focus when clicked, and loses focus when the user clicks somewhere else.
        /// </summary>
        bool IsFocused { get; set; }

        /// <summary>
        /// Sets the number of frames will wait for flashing the caret.
        /// The default is 10, meaning the caret will be shown for 10 frames, hidden for 10 frames, and so on.
        /// </summary>
        uint CaretFlashDelay { get; set; }

        /// <summary>
        /// An optional label that can act as a watermark.
        /// A watermark is for text which is shown in the textbox when it's empty and out of focus.
        /// An example of a watermark can be seen here: https://marketplace.visualstudio.com/items?itemName=havardhu.WatermarkTextBoxControl
        /// </summary>
        /// <value>The watermark label.</value>
        ILabel Watermark { get; set; }
    }
}

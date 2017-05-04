# GUIs

A GUI (graphical user interface, also referred to as a HUD- heads up display) is a control which is usually layered on top of the game, and allows the user to interact with the game in various ways.

In `MonoAGS` all GUI controls are based off of `IUIControl` which is basically an object (`IObject`) with a few more components sprinkled on top.
We already covered [objects](objects.md), so we will go over here on just the additional components.

## UI Common Components

### UI Events

Each UI control allows you to subscribe to UI events and respond to mouse/touch input:

#### Mouse Enter/Leave

When the users moves the mouse in/out the boundaries of the control. The boundaries of the control can be either its bounding box or pixel perfect (see [Collisions](objects.md#collisions)).
Those 2 events are good for hover effects, for example. 
The event arguments will tell you the position of the mouse.

#### Mouse Move

When the user moves the mouse inside the boundaries of the control.
The event arguments will tell you the position of the mouse.

#### Mouse Down/Up

When the user presses a mouse button/releases a mouse button inside the boundaries of the control.
The event arguments will tell you which button was pressed/release, and the position of the mouse.

#### Mouse Clicked/Double Clicked

When the user clicks (a fast press and release) a mouse button or double clicks the mouse button inside the boundaries of the control.
The event arguments will tell you which button was clicked, and the position of the mouse.

#### Lost Focus

When the user clicks a mouse button outside the boundaries of the control.

### Skinning

Skinning provides you with the ability to provide a custom look/theme to your controls.
`MonoAGS` currently comes with 2 basic themes, a blue theme (`AGSBlueSkin`) and a gray theme (`AGSGraySkin`), both used the generic `AGSColoredSkin` with a pre-defined set of rules for coloring
various aspects of the GUIs. You can use `AGSColoredSkin` to define other skins, or just implement `ISkin` for complete skinning freedom (but do look at how `AGSColoredSkin` is implemented to get an idea of how to write your skin).

Note that the 2 basic skins are currently written as a proof of concept, not as beautiful fully functional skins. If you have a passion for graphic design and wish to create a beautiful skin for the engine, please let us know.

## GUI Types

On top of the basic GUI control, several GUIs are available for use:

### Panels

A panel is a UI control which hosts other UI controls.
Note: the panel is in fact completely unnecessary, as any other UI control can host UI controls as well, but it's useful as it provides a clear intent on its use.

### Labels

A label is used for showing text, see [Labels](labels.md).

### Buttons

A button can be clicked to trigger an event (though as we saw, that's actually a property of all UI controls). The button gives you 3 animations that you need to assign, for idle/hover/pushed, which will trigger changing the button appearance based on its state, making it look like how a button usually looks.

### Checkboxes

A checkbox can be either checked or not checked. For the checkbox you need to provide 4 animations: Not checked, Not checked and hovered, Checked, Checked and hovered.

### Textboxes

A textbox allows you to edit text. You can query/set the text that appears in the box, query/set the position of the (caret)[https://en.wikipedia.org/wiki/Caret_navigation] and its flashing speed. You can also subscribe to an event which triggers on each key sent to the textbox and you can block the text from reaching the box (if you want a number only input, for example, you can only allow numbers to reach the box).

### Comboboxes 

A combobox (also referred to as a drop-down) is a text+button which when clicked, shows you a list of options to choose from. The selected option from the drop-down list appears as text next to the button. The combobox is therefore composed of several controls: A textbox for the text, a drop down button, a panel from the drop down list, and a button for each of the items in the drop down list. You can control all of those from the combobox, as well as getting/setting the selected item.

### Sliders

A slider is useful for selecting a value from a range. The user usually slides a handle across a line (which is either horizontal or vertical) to select a value. The slider is composed of an object for the handle, and an object for the background (the line). You need to assign valid objects for both, which will then be shown and the handle can be dragged by the user. You'll also need to send minimum and maximum values for the slider, and you'll be able to get/set the selected value, set whether the slider is vertical/horizontal, assign an optional label which shows the selected value, and subscribe to an event for whenever the value in the slider changes.

### Inventory Windows

An inventory window is used for displaying a character's [inventory](characters.md#inventory).
For displaying the window, you need to set the size allocated for displaying each inventory item 
in the window and assign the window to an inventory (usually the player's). You then have the ability to scroll the window up/down (usually you'll add to buttons that when clicked will call the scroll up/down functions) or specifically set the top item which is shown in the window (those are for cases when the inventory contains too many items such that not all fit in the window).

### Message Boxes

A message box is a dialog which is shown to the user with information. The dialog can also comes with buttons for selecting an action (like an "Are you sure you want to quit?" dialog, for example).
For displaying message boxes, you can use `AGSMessageBox.Display` for which you give a text and a list of buttons. It will display the text and the buttons at the bottom of the box and it returns the button which was clicked so that you'll be able to trigger the desired action. For convenience, on top of `AGSMessageBox.Display`, there are 2 built in message boxes: `AGSMessageBox.YesNo` and `AGSMessageBox.OkCancel`, which will show you a yes/no question, or a ok/cancel dialog (they both use `AGSMessageBox.Display` internally), and those will return to you a boolean value indicating whether yes/ok or no/cancel were selected.

## GUI Focus

The concept of focus for GUIs (or [modal windows](https://ux.stackexchange.com/questions/12045/what-is-a-modal-dialog-window)), is for when you want a specific GUI control to force interaction with the user, thus you don't want any of the other GUIs responding to user input at that time. For example, when showing a yes/no question to the user, you don't want the user to be able to open the inventory window.
When there's a textbox for editing text (for example, entering a save name), you don't want the keyboard input to move the character around the room.
For the first scenario, you can add a modal window component (`IModalWindowComponent`) to your control which gives you the ability to grab focus and lose focus (`AGSMessageBox` does this automatically).
For the second, keyboard preventing scenario, the `ITextBox` comes built in with this functionality. 

You can always query if there's a control/textbox which has focus by using `IGame.State.FocusedUI`, and either querying `FocusedWindow` or `FocusedTextBox`.


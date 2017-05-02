# Dialogs

Dialogs are on-going conversation between two (or more) characters.
The dialog is structured as a sequence of actions (usually the characters talking but not necessarily), followed by a list of choices for the player, where each choice branches to another sequence of actions, followed by another list of choices, and so on.

You can configure a graphic object (see [Objects](objects.md)) to serve as a background for the dialog, and you have various options for configuring the dialog options and actions.

## Dialog option

A dialog option is one choice in a list of options for the player to choose from.
For example, in a detective game, you might interrogate a suspect and get a few choices:
1. Ask for an alibi
2. "What can you tell me about the murder weapon?"
3. "Did you know the victim?"
4. Nothing more to ask

For each option you can configure the following:

### Label

Each dialog option is a text hosted in a label which you can configure to change various things, like its color, size, border, font, text color, text outline/shadow, etc (refer to the [GUIs](guis.md) section to learn more).

### Text rendering options

You can also configure different text rendering options (like text color, font, outline, etc) for when the mouse is hovering over an option, and for when you've already selected this option in the past (to maybe notify the player she/he does not need to click on this option again).
By default the normal label text is white, the hovered text is yellow and the "already selected" text is gray.

### Show Once

You can configure a dialog option to only be shown once (i.e only until selected once). Once selected, and in case the same dialog is shown again the option will be hidden.
If an option is set as "show once", then the text rendering configuration for "already selected" is irrelevant.

### Speak option

You can configure where the player will speak the text written in the option when chosen. In the example dialog above, options 2 and 3 are a good candidate for a "speak option", but options 1 and 4 do not make sense as a "speak option" as those are not quoting the detective.

### Dialog control

For each option you can set what happens after running the associated conversation (the dialog actions): you can either set to close the dialog or to switch to a new dialog (i.e a new set of options to choose from, which will continue to branch out the conversation). If neither one is configured, the dialog will show itself again after the dialog option runs out.

Each of the options, when selected, will lead to a different conversation which involves both the detective and the suspect talking, with possible animations and other script actions: those are the dialog actions.

## Dialog action

A dialog action is any action which runs as part of a sequence of actions. As this is a dialog between characters, the action is usually one of the characters saying something but it isn't always the case, it can also be an animation, or setting a variable to indicate a puzzle has completed, or anything really.

The dialog option contains convenience methods for adding speaking actions, but also generic actions, which can be either synchrnous (blockinb) or asynchronous. Each action can also return a false value, indicating that the sequence of actions should be cancelled.
Finally, each dialog action can also be disabled by the code which will skip the action and go to the next action in the sequence.

### Startup actions

Dialog actions are configured per dialog option, but they can also be configured for the dialog as a whole. Those actions will run when starting the dialog, before showing the dialog options. This is useful, for example, if you want to have the characters greeting each other and/or setting up the stage for the conversation.

## Dialog layout

Besides configuring the graphics object for the dialog background, and configuring each dialog option rendering separately, you can also implement your own custom layout for the option. This is done by implementing the `IDialogLayout` interface and then hooking it up via the `Resolver` object (like you would hook up every other interface implementation that you wish to switch with your own).

Another related configuration that you have, is whether to display the dialog options while the action sequence (i.e the conversation) is running. This is set via the `ShowWhileOptionsAreRunning` property of the dialog.

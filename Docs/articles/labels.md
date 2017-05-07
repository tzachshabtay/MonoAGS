# Labels

Text can come in many forms in adventure games: Speech, hotspot label, dialogs, buttons, messages to the user, etc.

In `MonoAGS`, text is always hosted by a label, and always comes with a text configuration object.
The label gives the ability to have a background and a border for the text, and gives context for the text alignment configuration (i.e center aligning a text will put it in the center of the label).

## Text Configuration

The text configuration object (`ITextConfig`) has the following options:

### Font

A choice of font for rendering the text. This includes the font family, font size and font style (regular/bold/italic/underline/strikethrough).

### Brush

A brush which will used for drawing the text. Usually this will be a solid brush where you'll choose a color for the brush. However, other brush are available (though for desktop only currently) like a gradient brush or a texture brush.

### Alignment

How the text will be aligned (in relation to the label), i.e centered, top-left, etc.

### Auto fitting

This gives several options regarding how to fit the label with its underlining text:

#### None

There will be no custom fitting: the user will select a label size, and the text size
will be based on the font, it's size and the text.

#### Label Should Fit Text

The label size will be ignored. The label will resize to fit the text inside it.

#### Text Should Wrap And Label Should Fit Height

The text will be resized (but only downscaled, not upscaled) to fit the label size.
This can be useful when you lay out multiple buttons when you want to have them at the same size, and have various lengths of text.

#### Text Should Fit Label

The text will be resized (but only downscaled, not upscaled) to fit the label size.
This can be useful when you lay out multiple buttons when you want to have them at the same size, and have various lengths of text.

#### Text Should Crop

The text will be cropped to fit the label size.
This is the default for textboxes.

### Outline

An outline is the same text drawn behind the original text, only slightly bigger
and usually with a different color, to provide better contrast for the rendered text.
You can provide a brush for the outline and an outline width.

### Shadow

The shadow is the same text drawn behind the original text with an offset,
giving the appearance of a shadow. You can provide a brush for the shadow, and
an X and Y offsets from the original text.

### Padding

You can set padding from the left/right/top/bottom of the label (in pixels).

## Text in low resolutions

When your game is in a low resolution, the text might appear blurry as a result of the
low resolution. Some "pixel purists" might like it as it stays true to the resolution,
but other people might prefer to have their text sharp and crisp even if the game itself
is in low resolution. If you want your text in high resolution (and don't want to put
all text object in a specific high resolution label) there are properties in the engine
for scaling the resolution for the text. Those are `GLText.TextResolutionFactorX` and `GLText.TextResolutionFactorY` which will be a factor of the game's resolution. In a future revision, we'll add support for automatically adjusting the text resolution to your physical resolution, which would save from calculating the text resolution factor needed.

## Built-in Labels

There are specific types of labels which are built in with the engine, designed to make it easier for completing common tasks.

### Hotspot Labels

Those are labels that show the hotspot name when the mouse hovers over a hotspot. `HotspotLabel` will show the name of the hotspot (i.e "Shiny Object"), and `VerbOnHotspotLabel` will show the currently used verb with the name of the hotspot (i.e "Look at Shiny Object" or "Walk to Shiny Object" or "Use Hammer on Shiny Object").

### Debug Labels

Those are labels that can you help while you develop. `FPSCounter` will show the current FPS (frame per second), and can easily show you if the game is running too slow (slower than 60 FPS usually is considered too slow).
`MousePositionLabel` will show you the current (x,y) position of the mouse cursor. `DebugLabel` (which is internally used by both `FPSCounter` and `MousePositionLabel`) lets you bind a function to a label. The function will be repeatedly called and the text result will be shown on the label.
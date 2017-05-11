# Game Interface

The game interface (`IGame`) is the main entry point to everything (or almost everything) you can do with the engine.

By calling `AGSGame.CreateEmpty` you'll get the game interface, which you can then use and pass around. You'd usually want to subscribe to the game load event where you'll load up resources and show the first screen.
Then you'll want to call `IGame.Start` for which you'll pass the game settings (name of the game, resolution, etc, see [Settings](#settings)), which will start the game (and will call the game load event).

After doing that, at any script, you can always get the `IGame` interface (assuming you don't want to pass it around) by calling `AGSGame.Game`.

The game interface has the following systems you can interact with:

## Factory

The game factory is an interface to ease up creating game elements. The `IGameFactory` interface itself contains sub-factories which specialize in specific types of elements. The sub-factories that you can access there are:

### Graphics

For loading images, [animations](animations.md), and [directional animations](animations.md#directional-animations).
For example, you can call `game.Factory.Graphics.LoadAnimationFromFolder(animationFolder)` to load an animation (that's the short version of this function, with default configurations, there's a longer version for which you can pass animation configurations).

### Sound

For loading [audio clips](audio.md).

### Inventory

For creating [inventory items](characters.md#inventory) and [inventory windows](guis.md#inventory-windows).

### UI

For creating [UIs](guis.md), like panels, labels, checkboxes, textboxes, comboboxes and sliders.

### Fonts

For loading and installing fonts.

Note: installing fonts are needed specifically for Macs, as you must install and then restart the game on a Mac for the changes to take place, therefore it's recommended to call install fonts on the fonts you want to use at the start of the game where it will automatically restart if the fonts were not already installed (on Mac only, the game won't be restarted on other platforms).

###  Objects

For creating [objects](objects.md), [characters](characters.md) and hotspot areas (those will be converted to objects which are also [hotspots](hotspots.md)).

### Rooms

For creating [rooms](rooms.md) and [edges](rooms.md#edges).

### Outfits

For creating character [outfits](characters.md#outfits).

### Dialogs

For creating [dialogs](dialogs.md) and [dialog options](dialogs.md#dialog-option).

### Resources

For loading [resources](resources.md). Usually you won't have to actually use it, as the other factories provide
higher level methods of loading your assets.

## Game State

Game state is the entry point for everything that can change in the game (when the user saves the game, we'll save the state and hopefully won't need to save anything else, because the state is the only thing that changes):
This includes the player [character](characters.md), the [rooms](rooms.md), the [UI](guis.md), the [focused UI](guis.md#gui-focus), the [cutscene](#cutscene), the [room transitions](rooms.md#room-transitions) and global variables.
It also allows you to pause/resume the game, change its global speed and change the current room.

### Cutscene

As part of the game states, you can control the current cutscene. A cutscene is a scene in the game in which user interaction is disabled. Usually, you'll want to offer the user a way of skipping the scene as well. From the game, you can mark a start of a cutscene with `ICutscene.Start` and wrap it up with `ICutscene.End` (you must have a cutscene end for every start and you must ensure that every possible branch from the start will reach the end). You can set how the user triggers skipping a cutscene. The built in defaults are "Escape key"/"Any key"/"Any key or mouse". Or you can implement your own cutscene skipping by calling `ICutscene.BeginSkip`.

Finally, you can query to see if a cutscene is currently running, and if a cutscene is currently skipping.

## Input

The input allows you to query the mouse/keyboard state (what key is pressed, what position is the mouse in, etc) and subscribe for events for the mouse/keyboard (a mouse button was pressed/released, mouse was moved, a keyboard key was pressed/released). Normally you wouldn't need to handle input here, as the available game components will do that for you (and they use the input API for this).

Additionally, you can set the cursor from the input API (the cursor is actually an [object](objects.md) so you can do with it everything you do with objects, like animating, rotating, scaling, etc). Usually you won't need to do that, the control scheme you'll choose (rotating cursors/two buttons for interact and look/verb coin) will do that for you.

## Settings

The game settings allows you to set:

### Title

The title will appear in the title window of your game (if it's not full-screen).

### Virtual Resolution

The virtual resolution in which you code your game. This is the coordinate system in which you'll move all of your objects (unless if you set a rendering layer with an [independent resolution](render-layers.md#independent-resolution)).

### Window Size

The size of the window (if the game is not showing full-screen).

### Window Border

If the game is not in full-screen, you can have a fixed border, a re-sizable window (which will allow the user to resize the window), or a hidden border: you can use a hidden border with a maximized window to have the game on full-screen without actually having your game on full-screen. 

### Window State

You can set whether your window is normal, minimized, maximized or full-screen. If the window is full-screen then the border and size settings have no effect. Note that you can combined the maximized setting with the hidden border setting to have your game be full-screen without changing to full-screen mode.

#### Full-Screen vs Maximized with hidden border

There are a lot of discussions on [what's better](https://gaming.stackexchange.com/questions/107028/is-there-a-difference-between-running-games-in-windowed-or-fullscreen-mode). Full-screen might give your game a better performance in theory, whereas maximized window might be more stable (or work on more computers). In anyway, `MonoAGS` supports both.

### Preserve Aspect Ratio

If the game is configured to preserve the aspect ratio, then in case the window is resized
and the aspect ratio is changed, the screen will be letter-boxed or pillar-boxed so the
aspect ratio for the actual content will remain the same. If not, then it will stretch to fit.

### VSync

Allows to set the [vsync](https://hardforum.com/threads/how-vsync-works-and-why-people-loathe-it.928593/) mode (synchronization of the frame update rate with screen refresh rate to prevent tearing).

### Audio Settings

The audio settings allow you to change the master volume, and to control the [cross fading](rooms.md#music-clip-to-play) between rooms.

## Game Events

The game events allow you to subscribe to important events in the game.

### Game Load Event

The game load event happens when you start your game. This is where you load your resources (at least the resources for the first room) and set the first room for the game to start in.

### On Repeatedly Execute Event

The on repeatedly execute event happens every tick and allows you to check for conditions 
and do specific actions when the conditions fulfill.
The frequency of this events depends on the FPS (frames per second). By default, if the hardware (and software) can handle it, we run at 60 FPS, meaning this event will be called 60 times per second.

IMPORTANT: As this event runs 60 times per second (by default), it can be abused and deteriorate the performance of the game.
So it's important to keep two rules:
1. Don't perform long actions on each tick.
2. Don't allocate memory on each tick.

### On Before Render Event

This event is called on every render cycle before rendering starts.
It can be used for native background drawings, or for native OpenGL calls
for setting thins up (like setting [shader](objects.md#shader) variables).

### On Screen Resize

This event is called whenever the screen is resized.

### Default Interactions

Allows setting the [default interactions](hotspots.md#default-interactions).

## Repeat

`Repeat` is a utility class for repeating actions a set number of times. `Repeat.OnceOnly(key)` is probably the most useful function there and is the equivalent of `Game.DoOnceOnly` from "classic" AGS.

```csharp
private void onInteractWithMicrophone(object sender, AGSEventArgs args) {
    if (Repeat.OnceOnly("sing_a_song")) {
        cHero.Say("Mary had a little lamb!");
    }
    else cHero.Say("Nah, I already sang earlier...");
}
```

Other similar functions there include: `Repeat.Exactly(key, number_of_times)` for doing an action exactly X times, and also `Repeat.MoreThen(key, number_of_times)` and `Repeat.LessThen(key, number_of_times)` for repeating actions more than or less than X times.

Another useful function is `Repeat.Rotate(key, actions)` which rotates around a list of actions to perform (i.e on each call it performs the next action on the list, and eventually goes back to the beginning). This can be useful when wanting to have multiple responses for an interaction which repeat themselves:

```csharp
private void someDefaultInteraction()
{
    Repeat.Rotate("default interaction",
        () => cHero.Say("I'm not going to do that."),
        () => cHero.Say("Nope, I don't think so."),
        () => cHero.Say("I really don't want to, sorry."));
}
```
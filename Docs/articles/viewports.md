# Viewports

It might be that not all of the room is shown on the screen at once (for example, a scrolling room). A viewport to the room instructs the engine on what parts of the room to show. A viewport has the following properties:

## Each viewport has:

### X,Y

The location (bottom-left) in the room from which to show the screen.

### ScaleX, ScaleY

An optional zoom in/out into the room.

### Angle

An optional rotation of the viewport.

### Camera

A camera is a script that automatically moves the viewport, usually to track the player, although you can set the camera's target to track another character/object in the room. Or, you can disable the camera to manually set values in the viewport if needed. Note that you can also code your own camera script if you want to replace the default camera (and this can be done per-room).

## Multiple Viewports

You can show more than one viewport on the screen if you want. There are plenty of use cases for why you'd want something like this: you can show a split-screen for a 2 player game, you can have a television in one room showing what's going on in another room, you can have a sophisticated animation sequence a-la "24" to show multiple going-ons at once, etc.

If you look at the game state variable, you'll notice that there's a `Viewport` property, and a `SecondaryViewports` property. The `Viewport` property refers to the main viewport of the game (the game must have this viewport) and tracks the current room. For multiple viewports, add more viewports to the `SecondaryViewports` property.

There are additional properties for each viewport which might not be very helpful if you only have one viewport (i.e the normal scenario), but are very useful for multiple viewports.

### Room Provider

The room provider provides a room for the viewport to track. 
By default, the main viewport tracks the current room of the game.
You can assign a room provider for a viewport to change the strategy of which room to show in that specific viewport. There's a built in `AGSSingleRoomProvider` which you can
use to show a single room in your secondary viewport, or you can implement your own room provider with different logic. 

### Display List Control

You can control whether to show the room objects, the global GUIs, or both (default is both). The main viewport, by default, shows both. 
For a secondary viewport, you'd probably want to hide the global GUIs.

Additionally, you also have fine grain control with the ability to show/hide specific objects for that specific viewports. So, for example, you can show the same room twice on the screen, one with the player and one without.

### Projection Box

You can control what part of the screen a viewport will be shown in. For a vertical split screen, for example, you'll configure the main viewport to have a projection box on the left side, and an additional viewport to have a projection box on the right side.

### Parent

You can assign a parent object for the viewport. If a parent object is assigned, the projection box of the viewport will be bound to the location of that object. So, if, for example, you have a television set object which shows what happens in another room (via a secondary viewport), you'll set the TV as the parent for the secondary viewport, and that way moving the TV will also move the "contents" of the TV.

### Interactive

For each viewport, you can decide if it allows interactions with its contents or not.
# Rooms

The rooms are where the game takes place. A room can be a room in a house, or an outdoor location, or anything in between.

The game can show only one room at a time. This would usually be the room where the player is, though you can change to a different room than the one the player is in if you want. Note: this is different than "Classic" AGS in which the room is explicitly tied to where the player is.

## Each room has:

### ID

Each rooms should have a unique id (string) which identifies it to the engine.

### Background

A background graphic for the room. The background is actually just a regular object so it can be animated or use any of the properties available on regular objects (refer to the [Objects](objects.md) section for more information).

### Show Player? 

A flag indicating whether the player is to be shown in the room, for hiding the player from a map, for example. This is a convenience flag as this can be achieved by simply switching the room without moving the player to another room.

### Music clip to play

An optional music clip you can play when switching to the room. If moved from another room with a music clip, there clips will be cross-faded, and you can configure the cross-fading duration and easing functions from `IGame.AudioSettings.RoomMusicCrossFading`.

### Viewport

It might be that not all of the room is shown on the screen at once (for example, a scrolling room). A viewport to the room instructs the engine on what parts of the room to show. A viewport has the following properties:
#### X,Y

The location (bottom-left) in the room from which to show the screen.

#### ScaleX, ScaleY

An optional zoom in/out into the room.

#### Angle

An optional rotation of the viewport.

#### Camera

A camera is a script that automatically moves the viewport, usually to track the player, although you can set the camera's target to track another character/object in the room. Or, you can disable the camera to manually set values in the viewport if needed. Note that you can also code your own camera script if you want to replace the default camera (and this can be done per-room).
	
### Areas

A list of areas (or regions) that exist in the room (like areas in the room that the characters can walk in). There are several types of areas and things you can do with them, refer the [Areas](areas.md) section.

### Objects

A list of objects that are placed in the room (note that unlike "Classic" AGS, objects can be moved between rooms). Refer to the [Objects](objects.md) section for more information.

### Edges

Room edges are a convenient way for scripting a room change once a player walks beyond an edge. You can set the 'X' for the left and right edge, and the 'Y' for the top and bottom edge and subscribe to events when the player crosses the edge to change the room.

### Events

Each room has specific events which you can subscribe to and code stuff to happen on those events. The available events:
#### Before fade in

After leaving the previous room, before the current room is visible.

#### After fade in

After leaving the previous room, after the current room is visible.

#### Before fade out

Before leaving the current room when it is still visible.

#### After fade out

Before leaving the current room when it is no longer visible.

### Custom Properties

Those are special properties which you can attach to a room which might be useful for special coding tasks.

## Room Transitions

When switching between rooms, you can have an optional room transition effect to switch the room.
The transition can be set from `IGame.State.RoomTransitions`, so for example you can set `game.State.RoomTransitions.Transition = AGSRoomTransitions.CrossFade()` where the built in transitions are:

- Instant (no transition)
- Fade (fade out old room, then fade in new room)
- Cross-Fade (fade out old room and fade in new room at the same time)
- Box Out (a black box scales up from the center, then scales down to reveal the new room)
- Dissolve (the old room dissolved into the new room)
- Slide (the old room slides as in a slide-show)

All of those transitions can be configured with things like the duration and the easing function(s). 
Also, you can code your custom room transition if you want by implementing the `IRoomTransition` interface.

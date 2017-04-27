# Walking

## Walk Configurations:

### Movement Linked To Animation

Should the movement speed be linked to the animation frame changes?
This is on by default, meaning that the walk step would only be performed when the walk animation frame changes.
For the walk to look right, the images need to be drawn so that a foot touching the ground moves back a 
constant amount of pixels in between frames. This amount should then be entered as the `WalkStep`, and not touched any further. 
Any speed adjustment should only done by changing the animation speed (the animation's `DelayBetweenFrames` property, 
and an optional additional delay you can set for the actual running frame).

If this is turned off, then the walk step would be performed on each frame making for a smooth movement.
Note that if the walk animation only has a single frame, then the engine will ignore the configuration
and will treat it as false.

#### How should I decide if this should be on or off?

As a rule of thumb, if the character has legs you would want this on.
If you turn this off (for a character which has legs) while the movement will look smoother, it
will also cause a gliding effect, caused by the fact that the movement is not in sync with the animation
of the moving feet.
If your character doesn't have legs (a robot, a floating ghost, etc), turning this off would make
for a smoother looking walk.

### Walk Step

The walk speed is actually the number of pixels the character moves each time he/she/it moves.
If `MovementLinkedToAnimation` is turned off, the step would be made on each frame, making for a smooth movement.
This means that for this state, the walk step is the sole 'decision maker' for the walking speed of the character.
If `MovementLinkedToAnimation` is turned on (the default), the step would be made each time the animation frame changes.
This means that for this state, the walk step should be setup once (it needs to match exactly the amount of pixels the leg moves in the drawings)
when the actual controller of the walk speed is the animation's speed (the animation's `DelayBetweenFrames` property, 
and an optional additional delay for the actual running frame).

### Adjust Walk Speed To Scale Area

Should the engine adjust the walk speed to the scaling area (i.e for a scaling area that shrinks the character to simulate a far away place, you'll want the character to walk slower).

### Debug Draw Walk Path

For debugging purposes, you might want to turn this on, it will draw lines in real-time showing where the character intends on walking to.

## Actual Walking:

For your character to walk, you should call the `WalkAsync` command. In general, you don't have to do it for when the user clicks the mouse to walk the character, whatever
input scheme you choose will handle that for you (under the hood, the input scheme uses the `WalkAsync` as well), but you'd want to walk your character (or others) during cut scenes, or 
maybe you want to have the NPCs walking about during the game. 
When you call `WalkAsync` you give it a location and the character will start to walk to that location (on walkable areas only).
The command returns a Task of bool (a boolean flag). The task allows you to wait for the walk to complete (using the `await` keyword) and the boolean flag tells you whether the walk was 
completed successfully (it might be that the walk was interrupted by another walk, or another activity, or just that your character tried to walk to a location where it cannot reach).

Here's an example:

```csharp
Task<bool> walkSuccessful = cHero.WalkAsync(oChair.Location);
await cHero.SayAsync("I'm walking to the chair!");
if (await walkSuccessful)
{
    cHero.Say("And now I'm sitting!");
    sitOnChair();
}
else
{
    cHero.Say("You know what, the chair doesn't look too comfortable, I think I'll pass.");
}
```

At any time you can call either `StopWalking` or `StopWalkingAsync` to stop the character, you can query whether the character is currently walking by looking at `IsWalking` and you can 
also query the destination that the character is trying to walk to with `WalkDestination`.
You can also call `PlaceOnWalkableArea` to place the character on a walkable area near her/him, this is useful, for example, when moving the character between the rooms when you don't want to bother
looking for the exact pixel on the screen you want your character to be located at.

 


# Animations

An animation is composed out of a list of frames where each frame shows an image. The engine then runs through the images to make it look like the object is moving (i.e a walk animation, a talk animation, a jump animation, etc).

## Animation top-level configuration

An animation can be configured from the `IAnimationConfiguration` interface with the following properties:

### Loops

How many times an animation will loop before it stops?
Or, you can put 0 which will be interpreted as an endless animation (will loop over and over till the end of time, unless you manually stop it from the code).

### Looping Style

You can select how the animation will loop: forwards, backwards, forwards and then backwards, or backwards and then forwards.

### Delay Between Frames

Gets or sets the delay between each frame.
The delay is measured in frames, so if we're running at the expected 60 FPS, a delay of 5 (the default) means each second 12 frames of animation will be shown.
Note that each frame can be configured with an additional delay. That delay will be added for this
overall delay for that specific frame, so a frame's delay is relative, while this delay can be used
as an overall speed for the entire animation.

## Animation per-frame configuration

An animation frame is actually using a `ISprite` object, which wraps around an image and gives it a lot of the capabilities that objects have. So you can have a position, scale, rotation, anchor, tint and even a custom renderer for each frame individually (those properties will be relative to the object, so if you have a frame with a position of (10, 5) it means that the frame will be offset from the rest of the object/animations by 10 pixels to the right and 5 pixels to the bottom).

Additionally, the following properties can be set per frame:

### Frame Delay

An animation frame has 2 modes of delays that you can choose from (how much time to wait before moving on to the next frame).
It's either a specific delay or a random delay with an allowed range.
A random delay with an allowed range is useful for simulating talking, for example, when the mouth 
is moving in random speeds, giving the illusion of actual talking, instead of repeating animation 
that breaks the illusion fast.

The delay is added on top of the animation's overall delay and measure in game ticks. So let's say we have an overall animation speed configured to be 5 (the default), and we have this frame delay configured to be also 5 (the default is 0) and the game is running with 60 FPS (frames per second- also the default), then the total delay for this frame will be 10 ticks, so it will run
for 1/6 of a second.

### Sound Emitter

A sound emitter can be attached to a frame, and assuming the emitter also has an audio clip attached to it, the audio clip will play when the frame is showing, from the apparent location the object is at (i.e by panning and adjusting volume, depending on how the sound emitter is configured).
This is especially useful for footsteps, for example.

Note that, for footsteps, for example, you'd probably want to set the same emitter for all directions of walking which might be tedious. Luckily, the sound emitter has convenience method which you can use which will assign it to multiple animations at once.

## Animation State Control

You can query (and if needed, change) the state of an animation.
Here are the properties you can query/control at run-time:

### Is running backwards?

Is the animation currently running forwards or backwards?

### Current frame

The current frame that is currently showing in the game. You can also access the sprite in that frame and change its properties while the game is running (like position, rotation, etc).

### Current loop

The current loop number that is running.

### Time to next frame

How much time (in game ticks: so 10 when the game is at 60FPS means 1/6 of a second) is left before the engine will move to the next frame.

### Is Paused

Pause/resume an animation (and query if it is paused or not).

## Additional Building Commands

In addition, you can clone an animation (or an individual frame), and flip an animation horizontally/vertically for easily creating similar but different animations.

## Directional Animations

Directional animations allow grouping animations for different directions together (for example, walking left and right). This is used by the character (using the [outfit](characters.md#outfits)) when choosing a directional animation: if you don't assign all of the directions, the engine will attempt to choose the best direction based on what you have assigned. So, for example, if you only assign Left and Right animations for your walk animation, and the player is attempting to walk down-right, the engine will use the Right animation.
# Tweens

Tweening is the act of animating a property between two values (also called ["inbetweening"](https://en.wikipedia.org/wiki/Inbetweening)).

`MonoAGS` comes with built in support for tweening a lot of properties, and an easy mechanism to create your own custom tweens.

## Built-in Tweens

The built in tweens can be roughly put in the following categories:

### Transform

Those change the position, scale or rotation of either objects (including GUIs, characters, etc as everything is an object), viewports, or even individual animation frames.

A few examples:
```csharp
obj.TweenX(500f, 5f); //Animate the object's "x" to be x = 500 in the span of 5 seconds, and do it linearly

obj.TweenX(500f, 5f, Ease.BounceIn); //Same animation, only use the "bounce in" for easing.

obj.Animation.Frames[0].Sprite.TweenX(500f, 5f); //Same animation, only instead of animating the object, only animate the first frame of the object's animation.

obj.TweenScaleX(2f, 3f); //Scale the object by a factor of 2 in the span of 3 seconds.

obj.TweenWidth(200f, 3f); //Scale the object to make its width be 200 pixels in the span of 3 seconds.

obj.TweenAngle(45f, 3f); //Rotate the object to an angle of 45 degrees in the span of 3 seconds.

viewport.TweenScaleX(2f, 10.5f); //Zoom in the viewport by a factor of 2 in the span of 10 and a half seconds.
```

### Color

Those change the color tint of the object, with existing methods to either modify the RGBA elements, or the HSLA elements, depending on which color scheme you want to work with. This also includes convenience methods for fading in/out objects.

A few examples:
```csharp
obj.TweenRed(255, 2f); //Animate the red component of the object's RGBA tint to be all the way red in the span of 2 seconds.

obj.TweenSaturation(1f, 5f); //Animate the object to be fully saturated (fully colored) in the span of 5 seconds.

obj.TweenOpacity(0, 2f); //Tweens the opacity of the object to be fully transparent in the span of 2 seconds.

obj.FadeOut(2f); //The same as the previous TweenOpacity call, only easier on the eyes.
```

### Audio

Audio tweens exist to animate the volume, pitch or panning of a playing sound.

A few examples:
```csharp
sound.TweenVolume(0f, 2f); //Fades out the volume of the sound in the span of 2 seconds.

sound.TweenPitch(2f, 10f); //Change the pitch of the sound to be twice as high in the span of 10 seconds.

sound.TweenPanning(-1f, 5f); //Pans the sound to left speaker in the span of 5 seconds.
```

### And More...

Even more tweens exist that don't fit into one of these above categories, like animating cropping of an object (if the object has the `ICropSelfComponent` attached to it).

## Easing

Each tween can be optionally supplied with an easing function. Easing functions specify the rate of change of a parameter over time, and so can be used to "give more life" to various animations. If you don't specify an easing function for the tween, the linear ease will be used by default, meaning the value will be changed by the same fixed amount on each tick.

`MonoAGS` comes built in with various easing functions, which you can use in all of your desired tweens. You can visualize how the easing functions work [here](http://easings.net/).

You can also create your own custom easing function by specifying a function that gets a single `float` parameter (the time elapsed, from 0 at the beginning and 1 at the end) and returns a `float` (the new "eased" time).

A few examples of using built-in ease functions or a custom function.
```csharp
obj.TweenAngle(45f, 5f, Ease.ElasticIn);

obj.TweenHeight(200f, 2f, Ease.SineInOut);

private float myCustomEase(float t)
{
    //Some fancy graph building function goes here.
}

obj.TweenGreen(0, 5f, myCustomEase);
```

## Working with a tween

All the tween methods return a `Tween` object, which allows us to work with a tween. This includes waiting for a tween to complete, chaining multiple tweens, pausing/resuming a tween, rewinding a tween, repeating a tween, querying the state of the tween and stopping it.

### Waiting for a tween

All tweens are asynchronous, which means that the next line of the code won't wait for the tween to end by default.

```csharp
obj.X = 1f;
obj.TweenX(500f, 5f);
Debug.WriteLine($"Object is at {obj.X}"); //This will print 1, not 500, because we didn't wait for the tween to complete.
```

To wait for the tween the complete, we can `await` the `Task` property of the returned tween object.

```csharp
obj.X = 1f;
await obj.TweenX(500f, 5f).Task;
Debug.WriteLine($"Object is at {obj.X}"); //This will print 500, because we waited for the tween to complete.
```

### Combining multiple tweens

Separating the act of starting the tween and waiting for a tween to complete gives us the power to combine tweens in multiple ways. So, for example, we can start 2 tweens in parallel, and then wait for both of them to complete.

```csharp
var tween1 = obj.TweenX(500f, 5f);
var tween2 = obj.TweenScaleY(2f, 3f);
await tween1.Task;
await tween2.Task;
```

Note that instead of awaiting the 2 tweens separately, we could have also used the `Task.WhenAll` convenience method. Let's do that now, but also let's combine waiting for the tweens with waiting for a non-tween asynchronous action, like walking. Because all asynchronous actions return a `Task` object, the interface with working with them is the same.

```csharp
var tween1 = obj.TweenAngle(45f, 5f);
var tween2 = viewport.TweenScaleY(2f, 4f);
await Task.Delay(1000); //Waiting for a second between tweens just to make the example a little more interesting
var tween3 = sound.TweenVolume(0f, 2f);
var task = cHero.WalkAsync(100f, 50f);

await Task.WhenAll(tween1.Task, tween2.Task, tween3.Task, task);
```

Another useful method for working with multiple asynchronous actions is `Task.WhenAny`. Instead of waiting for all tasks to complete, it will wait for the first one and return that task.

```csharp
var tween1 = obj.TweenY(100f, 5f);
var tween2 = obj.TweenX(100f, 3f);
var tween3 = viewport.TweenProjectX(2f, 10f);

var firstTask = await Task.WhenAny(tween1.Task, tween2.Task, tween3.Task);

Debug.WriteLine($"Is the first task tween x? {firstTask == tween2.Task}"); //will print "True" as tween x will be completed first after 3 seconds.
```

Except for awaiting the task object, you can also chain continuations by using the `ContinueWith` method, and query if the task was completed by using the `IsCompleted` property:

```csharp
obj.TweenX(100f, 5f).Task.ContinueWith(_ => obj.TweenY(200f, 5f));

var tween = obj.TweenAngle(45f, 3f);
Debug.WriteLine($"Is Completed? {tween.Task.IsCompleted}"); //Will print false

await tween.Task;
Debug.WriteLine($"Is Completed? {tween.Task.IsCompleted}"); //Will print true

```

### Controlling Time

You can pause a tween at any time by calling `Pause`, and resume it by calling `Resume`. You can rewind a tween to the beginning by calling `Rewind`, and even set the tween to a specific time by setting the `ElapsedTicks` (or `ElapsedSeconds`) property.

```csharp
var tween = obj.TweenX(-50f, 10f);
tween.Pause();
tween.Resume();
tween.ElapsedSeconds = 5f;
tween.Rewind();
```

### Stopping a tween

You can stop a tween at any time by calling `Stop`. The `Stop` method receives a parameter that tells the tween what to do with the tween value: should we reset it as if the tween never happened (`Rewind`), should we set the value as if the tween was completed (`Complete`) or should we leave the value as it is now (`Stay`)?

```csharp
var tween1 = obj.TweenX(-50f, 10f);
var tween2 = obj.TweenY(50f, 10f);
var tween3 = obj.TweenZ(50f, 10f);

tween1.Stop(TweenCompletion.Rewind);
tween2.Stop(TweenCompletion.Complete);
tween3.Stop(TweenCompletion.Stay);
```

### Repeating a tween

You can set your tween to repeat multiple times (or forever). When repeating a tween you can set the looping style (should it only go forwards, or forwards then backwards, etc) and an optional delay between every loop start.

```csharp
obj.TweenX(50.2f, 5f).RepeatTimes(3, LoopingStyle.Forwards); //Will repeat the tween 3 times (5 seconds for each loop, totalling 15 seconds)

obj.TweenX(50.2f, 5f).RepeatTimes(4, LoopingStyle.ForwardsBackwards, 2f); //Will repeat the tween 4 times (forwards -> backwards -> forwards -> backwards) with 2 seconds apart between each animation loop, and each loop taking 5 seconds (totalling 4 * 5 + (4 - 1) * 2 = 26 seconds).

obj.TweenX(50.2f, 5f).RepeatForever(LoopingStyle.Backwards, 3f); //Will repeat the tween forever (always going backwards from end to beginning), with 3 seconds apart between each animation loop, and each loop taking 5 seconds.
```

### Querying a tween

At each point in time of a running tween you can query it to see what it doing. `tween.State` will tell you the state of the tween: Is it playing, paused, stopped, stopping or completed?

You can query `tween.ElapsedTicks` or `tween.ElapsedSeconds` to see how much time has passed since the tween started, you can query the original input for the tween by looking at `tween.From` (the beginning value), `tween.To` (the target end value), `tween.DurationInTicks` or `tween.DurationInSeconds` to see how much time the tween was configured to run for, `tween.Easing` (the easing function), and if you're dealing with a repeating tween you can look at `tween.RepeatInfo` to get more information (and even change it while the tween is running) like how many loops have passed, how many total loops should it animate for, is it currently running forwards or backwards, what looping style should it use and the delay to wait between each loop.

## Custom Tween

If there's no built-in tween for your desired property, you can create your own custom tween for any possible numeric property, using `Tween.Run`. In fact, all of the built in tweens were written using the same `Tween.Run` function.
A custom tween for a variable "x" can be written as: 

```csharp
float x = 1f;

Tween.Run(x, 10f, val => x = val, 3f);
```

The code above will tween the value of x from 1 to 10 in the span of 3 seconds.
The third parameter for this function is a function itself which assigns the animated value back to x.

Combining `Tween.Run` with [extension methods](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods) can create friendly reusable tweens. 
For example, the `TweenX` method we showed earlier can be written as:

```csharp
public static Tween TweenX(this IObject obj, float toX, float timeInSeconds, Func<float, float> easing = null)
		{
			return Tween.Run(obj.X, toX, x => obj.X = x, timeInSeconds, easing);
		}
```

This is almost an exact duplicate of the actual built-in tween method, and it only took a few simple lines of code to create (in fact, the only difference between the built in method, is that the built in method extends `ITranslate` instead of `IObject` which guarantees that it works on anything that implements `ITranslate` which includes both `IObject` and `ISprite` so you can run it on objects and individual animation frames).

### Custom Tween- Controlling the time

All the tweens created with `Tween.Run` use the [Repeatedly Execute](game.md#on-repeatedly-execute-event) event to "visit" the tween and move it forwards (so this includes all of the built in tweens as well).
Usually this is exactly what you want. Sometimes, however, you might want to control yourself when the tween is moved forwards (for example, maybe you want to move the tween forwards every time you click a button). For this, you can use the `Tween.RunWithExternalVisit` method, which behaves like `Tween.Run` only returns one additional parameter, called `visitCallback`. The visit callback is an action that you can trigger on your own terms to move the tween to its next tick.

Here's an example of how it can be used to move the tween forwards once a second instead of once a tick:

```csharp

var tween = Tween.RunWithExternalVisit(x, 10f, val => x = val, 3f, out Action visitTween);

//Tween hasn't actually started animating yet, because it's up to us to move it forwards. So let's do it now.
while (!tween.Task.IsCompleted)
{
    visit(); //Moving the tween
    await Task.Delay(1000); //Waiting a second before the next "tick".
}

//Note that in this example, while we started the tween with "10 seconds", the tween will actually take a lot more than 10 seconds, because we only move forwards one tick a second (instead of 60 ticks a second assuming 60 FPS).
```
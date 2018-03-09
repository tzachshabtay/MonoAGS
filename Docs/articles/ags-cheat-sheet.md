# AGS Cheat Sheet

This is a "cheat sheet" for people coming in from `AGS`. The "cheat sheet" is divided into 2 sections. The first section roughly demonstrates the language differences between AGS script and c#. The second section goes over the AGS scripting API (individual functions and properties) and compares them with AGS: it shows how to do the same in `MonoAGS`, or if something is missing, and also explaining some differences between the two, with brief highlights of things that don't exist in `AGS` but do exist in `MonoAGS`.

# General Scripting Differences

C# has a lot of similarities to AGS Script, but there are also many differences. In some sense, c# is simpler than AGS script (no pointers, no script headers, the order of the scripts doesn't matter), and in another sense it's more complicated (all functions need to be wrapped in classes/structs, and there are a lot more keywords).
Let's go over the main differences:

## Writing Functions

In AGS, a common function signature might look like this:

```

function do_something(int param)
{
    // contents of function go here
}

```

The equivalent in c# would look like this:

```csharp

void do_something(int param)
{
    // contents of function go here
}

```

The only difference here is that instead of `function` we wrote `void`. `void` is a keyword that means that the function doesn't return anything.
In c#, when you write a function, you need to declare what the function returns.

So, if the AGS function returns an int, like this:

```

function do_something(int param)
{
    return 5;
}

```

The c# equivalent function will look like this:

```csharp

int do_something(int param)
{
    return 5;
}

```

Another big difference is that in AGS we can write a function anywhere in the file. In c#, the functions need to be written inside either a class or a struct, and that needs to be wrapped inside a namespace.
A namespace is used to group multiple classes together, you shouldn't usually worry about this, when you add a new code file from the editor the namespace is automatically generated and written to the file.

A c# struct is identical to the AGS struct, and a c# class represents a reference type, which basically means it's an instance that you can carry around: this is basically the replacement to pointers in AGS.
It's not critical to understand this at first, as almost always you'd want to use a class. If you want to dig deeper and understand when you'd like to use a struct, this link is a good start: https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct

A single class can contain multiple functions, and is usually used to group functions which revolve around the same purpose.
So, in our example, we'll put the `do_something` function in a class like this:

```csharp

namespace MyGame
{
    class MyClass
    {
        int do_something(int param)
        {
            return 5;
        }
    }
}

```

## Returning multiple values from a function

If you want to return multiple values from your function in AGS script, you'd have to create a struct especially for that purpose, which complicates things. In c#, you can just use tuples for this:

```csharp

(float x, float y) get_position()
{
    return (100, 200);
}

```

You can now call this function easily like this:


```csharp

(float x, float y) = get_position();
Debug.WriteLine($"X = {x}, Y = {y}");

```

## Variables & Scope

Variables in c# are declared the same way as in AGS script, with the difference that, like functions, the variables need to reside inside the class/struct.

Variables can also declared inside a function, this would make their scope local to the function, meaning that the variable will not exist once the function completes.

Here's an example:

```csharp

namespace MyGame
{
    class MyClass
    {
        string var1;
        string var2 = "Hello";

        int do_something(int param)
        {
            string var3 = "Hi";

            var1 = "aaa";
            var2 = "bbb";
            var3 = "ccc";

            return 5;
        }

        int do_something2()
        {
            var1 = "aaa";
            var2 = "bbb";
            var3 = "ccc"; //THIS DOES NOT COMPILE, var3 does not exist!
        }
    }
}

```

In the example above, `var1` and `var2` are class variables, so they can be used anywhere within the class. `var3` however, is local to the `do_something` function, so it cannot be accessed from the `do_something2` function.

## Exporting functions & variables

In AGS script, in order to be able to access a function from different script, you need to add an import to it in your exporting script header, and in order to export a variable you need to add an `export` to it and that `export` must reside in the end of the file.

In c#, there is no script header file: both functions and variables can be exported simply by prefixing them with the keyword `public`, like this:

```csharp

public string var1;

public int do_something()
{
    return 5;
}

```

## Static vs instance

Another big difference that needs to be understood, is the different between static functions/variables to instance functions/variables. In AGS, functions and variables that are declared inside a struct are instance functions/variables, but all other functions and variables that you code are static.

In c#, however, all functions/variables are instance by default, unless you explicitly make them static. The difference is that instance functions/variables are associated with instances that are generated from the class (which can be looked at like a blueprint for creating instances), while static variables belong to the class itself.

If I have a `Dog` class to represent a dog, and a `health` variable that gives a number between 0-100 to say how healthy the dog is, then `health` should be an instance variable: if I create 3 different dogs, they should have 3 different `health` states.
If, on the other hand, I have a `Utilities` class with random scripts, and among them I have a `sum_two_numbers` function, that function makes sense as a static function, there is no point in having 2 (or more) "Utilities".

In c#, to have a function (or variable) static, you prefix it with the keyword `static`.

So the 2 examples from above would look like this:

```csharp

namespace MyGame
{
    class Dog
    {
        public int Health;
    }

    class Utitilies
    {
        public static int sum_two_numbers(int x, int y)
        {
            return x + y;
        }
    }
}

```

## Importing functions & variables

In AGS, to import functions/variables you need to add an import for them in the script header.
In c#, assuming both files are using the same namespace, you can call them without adding anything specific.

How you call the functions/variables in c# depends if they are static or instance functions/variables.
For static functions/variables you need to prefix the class name and a dot before the function/variable.
For instance functions/variables you need to create a new instance of your class using the `new` keyword, assign that to a variable, and then you can call that instance's functions/variables by prefixing it with the variable name and a dot.

As an example, let's use or `Dog` and `Utilities` classes from the previous example in a different file:

```csharp

namespace MyGame
{
    class MyClass
    {
        Dog dog1 = new Dog();
        Dog dog2 = new Dog();

        void init()
        {
            dog1.Health = 10;
            dog2.Health = 20;

            int combined_health = get_dogs_combined_health();
            Debug.WriteLine($"The combined health of both dogs is: {combined_health}");
        }

        int get_dogs_combined_health()
        {
            return Utilities.sum_two_numbers(dog1.Health, dog2.Health);
        }
    }
}

```

Sometimes we want to import functions/variables from another namespace which is not our own.
For example, the `Debug.WriteLine` we used above (that writes a message to our debug console) is part of the `System.Diagnostics` namespace, as that is part of the c# standard library. The standard library is a library of common useful functions that is bundled with every c# application (and it has a lot of useful stuff in there). Because it's in a different namespace than ours, we need to explicitly import that namespace, and we do that with the `using` keyword:

```csharp

using System.Diagnostics;

namespace MyGame
{
    class MyClass
    {
        void write_hello()
        {
            Debug.WriteLine("Hello!");
        }
    }
}

```

Note that if you're using an IDE like Visual Studio to write your code, and you forgot putting the `using` section, a lot of times Visual Studio can add this automatically for you: you'll see a squigly line under your usage of a missing library (`Debug.WriteLine` in this scenario) and a lightbulb icon which, when clicked on, will give you the option to automatically add the missing `using` clause.

Sometimes we want to import functions/variables from an external library: in that case we'll want to add a reference to that library before we can add `using` clauses for its namespaces.
For example, we might want to add a library that will allow us to vibrate the mobile phone (if the game is played on a mobile phone). We can search for an available package directly from the IDE. From Visual Studio, under your project node in the solution explorer, there's a "Packages" node, right click it and select "add package". This will open a window in which you can search and install packages. If you search for "vibrate", for example, you'll see "Vibrate Plugin for Xamarin and Windows" which you can then install by clicking "Add Package".
Once you have the package installed, you can use its namespaces using the same `using` keyword we saw before (and you should read that package documentation to understand how to use it).

## Scripts

In AGS, you have a global script, room scripts, and additional scripts you can create on will. In c# there's no global script, and you are free to organize the scripts as you want them to be.
The order of the scripts (unlike AGS) does not matter.
There are no "special functions" that you can put in your script (like `game_start`) that are magically called, you have to call it yourself.

## Data types and operators

All AGS data types and operators exist in c# with the same syntax, and used the same way.
c# has additional data types and operators which do not exist in AGS.

You can see all data types here: https://docs.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/types-and-variables

You can see all operators here: https://msdn.microsoft.com/en-us/library/6a71f45d(v=vs.120).aspx

Additionally, c# has the keyword `var` which can be used when declaring the variable is it is clear what data type it has.
For example:

```csharp

var x = "hello"; //x is a string, it's clear from how x is initialized, so we can use var instead of string when declaring the variable (if we want).

```

## Arrays

The c# syntax for arrays is slightly different from the AGS syntax for dynamic arrays, in that you put the brackets next to the type and not next to the variable name:

```csharp

int[] characterHealth = new int[50];


```

Also, in c# you have lists, which is like an array only without a size limit, so you can add and remove items from the list:

```csharp

List<int> characterHealth = new List<int>();

characterHealth.Add(5);
characterHealth.Add(6);
characterHealth.Add(7);

Debug.WriteLine(characterHealth[0]); //5
Debug.WriteLine(characterHealth[1]); //6
Debug.WriteLine(characterHealth[2]); //7

characterHealth.RemoveAt(1);
Debug.WriteLine(characterHealth[0]); //5
Debug.WriteLine(characterHealth[1]); //7

```

## Conditionals and Loops

The syntax for `if`, `else`, `while` and `switch` in c# is identical to the syntax in AGS.

C# also had additional constructs for conditionals and loops: most notably the `?` operator can be used in some scenarios instead of `if`, and the `foreach` keyword can be used in some scenarios instead of `while`:

```csharp

//using an if:
string getVisibleWord()
{
    if (visible) return "visible";
    else return "invisible";
}

//using '?'
string getVisibleWord()
{
    return visible ? "visible" : "invisible";
}

//using while
int getSum(int[] numbers)
{
    int sum = 0;
    int index = 0;
    while (index < numbers.Length)
    {
        sum += numbers[index];
        index++;
    }
    return sum;
}

//using foreach
int getSum(int[] numbers)
{
    int sum = 0;
    foreach (int number in numbers)
    {
        sum += number;
    }
    return sum;
}

```

Also, c# has a wonderful querying language for collections, called LINQ, which can be used to make a lot of tasks more simpler. For example:

```csharp

//without LINQ
int getSumOfNumbersBiggerThan10(int[] numbers)
{
    int sum = 0;
    foreach (int number in numbers)
    {
        if (number > 10) sum += number;
    }
    return sum
}

//with LINQ
int getSumOfNumbersBiggerThan10(int[] numbers)
{
    return numbers.Where(number => number > 10).Sum();
}

```

You can read more on LINQ here: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/getting-started-with-linq

## String Formatting

c# has a `string.Format` function which is similar to the `String.Format` function in AGS.
c# also has support for string interpolation, though, which usually gives a better experience.

Let's go over the different of `string.Format` first:

In AGS script:

```

String posString = String.Format("The position is: %d,%d", x, y);

```

In c#:

```csharp

string posString = string.Format("The position is: {0},{1}", x, y);

```

The main different is that in AGS script you need to specify the type of the variables, where you don't have to do that in c#, but in c# you have to specify the location of the parameter in the string.

AGS also has the `%0Xd` and `%.Xf` special formatting codes for zero padding and showing decimal places. c# also has special formatting codes for a lot more scenarios. So, for example, for padding to 5 zeroes and showing 3 decimal places, we can rewrite our previous formatting like this:

```csharp

string posString = string.Format("The position is: {0:00000.###},{1:00000.###}", x, y);

```

You can read about all the special formatting codes here: https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types

AGS also uses the left bracket as a special code for a new line, in c# you can use `Environment.NewLine` instead.

### String Interpolation

String interpolation is a newer type of formatting which allows placing the parameters directly in the string. For creating an interpolated string, you should prefix the string with `$`, and then can directly put the parameters inside `{}`.
So we can rewrite our previous example like this:

```csharp

string posString = $"The position is: {x:00000.###},{y::00000.###}";

```

You can read more about interpolated strings here: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interpolated-strings

## Constants (Compilation Flags)

What in AGS script is called "constants" is actually 2 separate things in c#: constants and compilation flags. In c# constants are variables that cannot be changed ever:

```csharp

const int x = 5;
x = 6; //This is illegal -> will not compile

```

So, the AGS constant `AGS_MAX_INV_ITEMS` would have been defined as a constant in c# as well (though MonoAGS doesn't have all of the limit constants in AGS, there are no limits in MonoAGS).

On the contrary, all of the AGS constants that might or might not be defined, those are compilation flags. The AGS directive `#ifdef` has an equivalent `#if` directive in c# and `#ifndef` equivalent is `#if !`.
The `DEBUG` compilation flag that exists in AGS also exists in c#, so you can use it to do things only when debugging:

```csharp

#if DEBUG

//do something debug specific here.

#endif

```

All of the other AGS compilation flags are AGS specific and have no equivalents in MonoAGS.
Except for `#if`, c# has more directives which you can see here: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/

## Extender functions

The c# equivalent to AGS extender functions is extension methods (note that in c# a function is called a method usually, but it means the same thing).

The syntax is similar, with the exception that in c# you need to mark your method static and the class should also be marked as static (a static class is a class for which all the functions within are also static).

So, in AGS you would write:

```

function Scream(this Character*)
{
    this.Say("AAAAARRRRGGGGHHHHHH!!!!");
}

```

In c# you would write:

```csharp

public static class CharacterExtensions
{
    public static void Scream(this ICharacter character)
    {
        character.Say("AAAAARRRRGGGGHHHHHH!!!!");
    }
}

```

## noloopcheck

`noloopcheck` is something that's AGS specific and has no equivalent in c#. Or rather, the loop checks are AGS specific, you can think of all loops in c# as `noloopcheck`.

## async/await

`async/await` is a c# specific mechanism for asynchronous programming which doesn't have an equivalent in AGS.
As it's an important part of programming your game, there's a specific article devoted to `async/await`, which you can find [here](async.md).

# API Comparisons

This section goes over each API provided by AGS (based on the AGS manual), and compares against the MonoAGS API. Items marked with `?` are items that currently don't have an equivalent in `MonoAGS`.

## AudioChannel

The equivalent in `MonoAGS` would be `ISound`. Both are returned when you're playing an audio clip. The difference between AGS channel and MonoAGS sound is that a sound relates to the specific sound you're playing, it "dies" when you finished playing the sound. The channel however lives on throughout the game and can play other sounds in the future, so you can't always trust it's playing the sound you requested.

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Seek | Seek | `channel.Seek(milliseconds);` | `sound.Seek = seconds;` | Milliseconds in AGS, seconds in MonoAGS. In AGS the value is int meaning you can't get a lower resolution than milliseconds. In MonoAGS the value is float meaning you can go as low in resolution as the hardware understands.
| SetRoomLocation | ? | `channel.SetRoomLocation(x,y);` | ? | MonoAGS has the concept of a sound emitter which automatically pans the sound based on the location in the room, and can set the volume based on volume-changing areas, but nothing currently specifically exists for volume based on distance from a character.
| Stop | Stop | `channel.Stop();` | `sound.Stop();` |
| ID | SourceID | `channel.ID` | `sound.SourceID` |
| IsPlaying | HasCompleted | `if (!channel.IsPlaying)` | `if (sound.HasCompleted)` | If you want to check whether the sound you played completed playing, `MonoAGS` provides you with a better option: In AGS, `channel.IsPlaying` might return true even if your sound finished playing, because another sound is now being played on that channel.
| LengthMs | Duration | `channel.LengthMs` | `sound.Duration` |
| Panning | Panning | `channel.Panning = -100;` | `sound.Panning = -1;` | -100 - 100 in AGS, -1 - 1 in MonoAGS. In AGS the value is int (meaning you can only have 200 values) where in MonoAGS the value is float (when you can have a range as big as the hardware understands).
| PlayingClip | ? | `channel.PlayingClip` | ? | This is critical in AGS due to the fact the channel might be playing a lot of clips in its lifetime. Much less important in `MonoAGS` as you can know which clip the sound is coming from, because you're playing that sound.
| Position | Seek | `if (channel.Position == 0)` | `if (channel.Seek == 0)` | Milliseconds in AGS, seconds in MonoAGS
| Volume | Volume | `channel.Volume = 100;` | `sound.Volume = 1f;` | 0 - 100 in AGS, 0 - 1 in MonoAGS. In AGS the value is int (meaning you can only have 200 values) where in MonoAGS the value is float (when you can have a range as big as the hardware understands).

Missing in AGS but exists in MonoAGS: Pitch, Asynchronous completion API, Pause/Resume, Rewind, IsPaused, IsLooping, IsValid.

## AudioClip

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Play | Play | `clip.Play(eAudioPriorityNormal, eOnce); clip.Play(eAudioPriorityNormal, eRepeat);` | `clip.Play(false); clip.Play(true);` | There's no equivalence for audio priority currently.
| PlayFrom | Seek the sound coming back from the clip. | `clip.PlayFrom(1000);` | `var sound = clip.Play(); sound.Seek = 1;` |
| PlayQueued | ? | `clip.PlayQueued();` | ? | Note that in AGS the number of available channels is 10; In MonoAGS the number of available channels is based on what the running hardware provides, which, on modern machines is usually at least 32 (and on older machines, usually at least 16), so this feature becomes less important.
| Stop | You can query all playing sounds and stop them | `clip.Stop();` | `foreach (var sound in clip.CurrentlyPlayingSounds) sound.Stop();` |
| FileType | ? | `clip.FileType` | ? |
| IsAvailable | ? | `clip.IsAvailable` | ? |
| Type | ? | `clip.Type` | ? |

Missing in AGS but exists in MonoAGS: ID, CurrentlyPlayingSounds, Volume/Pitch/Panning (so you can change the template at runtime, not just from the editor), playing a clip while overriding default volume/pitch/panning.

## Character

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| AddInventory | Inventory.Items.Add | `cEgo.AddInventory(iKey);` | `cEgo.Inventory.Items.Add(iKey)` |
| AddWaypoint | Either use `await` or `ContinueWith` | `cSomeguy.Walk(160, 100); cSomeguy.AddWaypoint(50, 150);` | Using await: `private async Task walk() { await cSomeguy.WalkAsync(160, 100); await cSomeguy.WalkAsync(50, 150); }` (we can now call this walk method and either block (with another await) or doesn't block, it's up to us. Using ContinueWith: `cSomeguy.WalkAsync(160, 100).ContinueWith(_ => cSomeguy.WalkAsync(50, 150));` | Note what we gain using the `await` that we can't do with AGS: we can easily create an endless loop of non-blocking walking in circles: `private async void endlessWalk() { while (true) { await cSomeguy.WalkAsync(50, 150); await cSomeguy.WalkAsync(160, 100);}}`
| Animate | AnimateAsync | `cEgo.Animate(3, 1, 0, eBlock, eBackwards);` | For blocking: `await cEgo.AnimateAsync(jumpUpAnimation);`. For non-blocking, do the same just without awaiting it: `cEgo.AnimateAsync(jumpUpAnimation);`. As for delay, repeat style and direction, those are configured as part of the animation ("jumpUpAnimation" in this scenario). It can be changed at run-time before animating, if you want. For example: `jumpUpAnimation.Looping = LoopingStyle.BackwardsForwards; jumpUpAnimation.Loops = 15; jumpUpAnimation.DelayBetweenFrames = 3;` | Note that `MonoAGS` doesn't have the concepts of view and loop, just individual animations for manual animations, and directional animations for automatic animations like walk and idle.
| ChangeRoom | ChangeRoomAsync | `cEgo.ChangeRoom(4, 50, 50);` | `cEgo.ChangeRoomAsync(rLobby, 50, 50);` | Note that unlike AGS, you CAN wait for the change room to finish in your current script if you use await.
| ChangeRoomAutoPosition | ? | `cEgo.ChangeRoomAutoPosition()` | ? |
| ChangeView | Outfit | `cEgo.ChangeView(5);` | `cEgo.Outfit = outfitWithHat;` | Note that the concepts are not identical: `ChangeView` in AGS changes the walk animation, where `Outfit` in MonoAGS changes all animations in that outfit (which can be walk, idle, etc).
| FaceCharacter | FaceDirection | `cEgo.FaceCharacter(cSomeGirl, eBlock);` | Non-blocking: `cEgo.FaceDirectionAsync(cSomeGirl);`, blocking: `await cEgo.FaceDirectionAsync(cSomeGirl);` | Missing support for "turning" animation.
| FaceLocation | FaceDirection | `cEgo.FaceLocation(50, 50, eBlock);` | Non-blocking: `cEgo.FaceDirectionAsync(50, 50);`, blocking: `await cEgo.FaceDirectionAsync(50, 50);` | Missing support for "turning" animation.
| FaceObject | FaceDirection | `cEgo.FaceObject(oFridge, eBlock);` | Non-blocking: `cEgo.FaceDirectionAsync(oFridge);`, blocking: `await cEgo.FaceDirectionAsync(oFridge);` | Missing support for "turning" animation.
| FollowCharacter | Follow | `cBadGuy.FollowCharacter(cEgo);` | `cBadGuy.Follow(cEgo);` | Note that in MonoAGS you can follow more than just characters, including objects and even GUIs.
| GetAtScreenXY | IHitTest.ObjectAtMousePosition | `if (Character.GetAtScreenXY(mouse.x, mouse.y) == cEgo){}` | `if (hitTest.ObjectAtMousePosition == cEgo) {}` | Missing support for specific location checks.
| GetProperty | Properties.Ints.GetValue | `if (cEgo.GetProperty("Value") > 200) {}` | `if (cEgo.Properties.Ints.GetValue("Value") > 200) {}` |
| GetTextProperty | Properties.Strings.GetValue | `cEgo.GetTextProperty("Description");` | `cEgo.Properties.Strings.GetValue("Description");` |
| HasInventory | Inventory.Items.Contains | `if (cEgo.HasInventory(iKnife)) {}` | `if (cEgo.Inventory.Items.Contains(iKnife)) {}` |
| IsCollidingWithChar | CollidesWith | `if (cEgo.IsCollidingWithChar(cGuy) == 1) {}` | `if (cEgo.CollidesWith(cGuy.X, cGuy.Y, state.Viewport)) {}` | Note that MonoAGS supports multiple viewports so we need to pass the viewport in which we'd like to test for collisions.
| IsCollidingWithObject (character) | CollidesWith | `if (cEgo.IsCollidingWithChar(oBottle) == 1) {}` | `if (cEgo.CollidesWith(oBottle.X, oBottle.Y, state.Viewport)) {}` | Note that MonoAGS supports multiple viewports so we need to pass the viewport in which we'd like to test for collisions.
| LockView | Outfit | `cEgo.LockView(12);` | `cEgo.Outfit = swimmingOutfit;` |
| LockViewAligned | ? | `cEgo.LockViewAligned(12, 1, eAlignLeft);` | ? |
| LockViewFrame | To display a still frame, use Image, for actual locking set a different outfit | `cEgo.LockViewFrame(AGHAST, 2, 4)` | `cEgo.Image = cEgo.Outfit[Animations.Aghast].Left.Frames[4].Sprite.Image;` |
| LockViewOffset | ? | `cEgo.LockViewOffset(12, 1, -1);` | ? | Note that while there's no direct equivalent, you can change offsets for individual animation frames, so you can do that manually (at run-time if you desire), for example: `cEgo.Outfit[Animations.Walk].Left.Frames[0].Sprite.X = 5; //will offset the first left walking animation frame by 5 pixels to the right`
| LoseInventory | Inventory.Items.Remove | `cEgo.LoseInventory(iKnife);` | `cEgo.Inventory.Items.Remove(iKnife);` |
| Move (character) | set the outfit to an outfit without a walk animation | `cEgo.Move(155, 122, eBlock);` | Non-blocking: `cEgo.Outfit = idleOnlyOutfit; cEgo.WalkAsync(155, 122);`, blocking: `cEgo.Outfit = idleOnlyOutfit; await cEgo.WalkAsync(155, 122);` | No support currently for "walk anywhere"
| PlaceOnWalkableArea | PlaceOnWalkableArea | `cEgo.PlaceOnWalkableArea();` | `cEgo.PlaceOnWalkableArea();` |
| RemoveTint | Tint | `cEgo.RemoveTint();` | `cEgo.Tint = Colors.White;` |
| RunInteraction | Interactions.OnInteract(Verb).InvokeAsync | `cEgo.RunInteraction(eModeTalk);` | `cEgo.Interactions.OnInteract(Verbs.Talk).InvokeAsync();` |
| Say | SayAsync | `cEgo.Say("Hello!");` | `await cEgo.SayAsync("Hello!");` |
| SayAt | ? | `cEgo.SayAt("Hello!", 50, 50);` | ? | While there's no direct equivalent currently, this can be worked around by providing a custom implementation for `ISayLocationProvider`.
| SayBackground | SayAsync | `cEgo.SayBackground("Hello!");` | `cEgo.SayAsync("Hello!");` | There's no way in AGS to know when `SayBackground` completes. MonoAGS gives you the task completion API for this: `Task task = cEgo.SayAsync("Hello!"); ... while (!task.IsCompleted) {..}`, or simply: `Task task = cEgo.SayAsync("Hello!"); ... await task; `
| SetAsPlayer | IGameState.Player | `cEgo.SetAsPlayer();` | `state.Player = cEgo;` |
| SetIdleView | Outfit | `cEgo.SetIdleView(5);` | `cEgo.Outfit = outfitWithHat;` | Note that the concepts are not identical: `SetIdleView` in AGS changes the idle animation, where `Outfit` in MonoAGS changes all animations in that outfit (which can be walk, idle, etc).
| SetWalkSpeed | WalkStep | `cEgo.SetWalkSpeed(5, 5);` | `cEgo.WalkStep = new PointF(5, 5);` |
| StopMoving | StopWalkingAsync | `cEgo.StopMoving();` | `cEgo.StopWalkingAsync();` |
| Think | ? | `cEgo.Think("Hmmmm..");` | ? |
| Tint | Tint | `cEgo.Tint(0, 250, 0, 30, 100);` | `cEgo.Tint = Colors.Green;` or `cEgo.Tint = Color.FromRgba(0, 255, 0, 255);` or `cEgo.Tint = Color.FromHsla(200, 1, 1, 255);` or `cEgo.Tint = Color.FromHexa(59f442);` |
| UnlockView | Outfit | `cEgo.UnlockView();` | `cEgo.Outfit = defaultOutfit;` |
| Walk | WalkAsync | `cEgo.Walk(100, 100);` | For non blocking: `cEgo.WalkAsync(100, 100);`, for blocking: `await cEgo.WalkAsync(100, 100);` | No support currently for "walk anywhere"
| WalkStraight | ? | `cEgo.WalkStraight(100, 100);` | ? |
| ActiveInventory | Inventory.ActiveItem | `cEgo.ActiveInventory` | `cEgo.Inventory.ActiveItem` |
| Animating | Animation.State.IsPaused | `if (cEgo.Animating) {}` | `if (!cEgo.Animation.State.IsPaused) {}` |
| AnimationSpeed | Animation.Configuration.DelayBetweenFrames | `cEgo.AnimationSpeed = 4;` | `cEgo.Animation.Configuration.DelayBetweenFrames = 4;` |
| Baseline | Z | `cEgo.Baseline = 40;` | `cEgo.Z = 40;` |
| BlinkInterval | ? | `cEgo.BlinkInterval = 10;` | ? |
| BlinkView | ? | `cEgo.BlinkView = 10;` | ? |
| BlinkWhileThinking property | ? | `cEgo.BlinkWhileThinking = false;` | ? |
| BlockingHeight | ? | `cEgo.BlockingHeight = 20;` | ? |
| BlockingWidth | ? | `cEgo.BlockingWidth = 20;` | ? |
| Clickable | Enabled | `cEgo.Clickable = false;` | `cEgo.Enabled = false;` |
| DiagonalLoops | Simply configure your directional animation either with or without diagonal directions | `cEgo.DiagonalLoops = true;` | Nothing special needed for this to work |
| Frame | Animation.State.CurrentFrame | `cEgo.Frame` | `cEgo.Animation.State.CurrentFrame` |
| HasExplicitTint | Tint | `if (cEgo.HasExplicitTint) {}` | `if (cEgo.Tint != Colors.White) {}` |
| ID | ID | `cEgo.ID` | `cEgo.ID` |
| IdleView | Outfit[Animations.Idle] | `cEgo.IdleView` | `cEgo.Outfit[Animations.Idle]` |
| IgnoreLighting | ? | `cEgo.IgnoreLighting = 1;` | ? |
| IgnoreWalkbehinds | ? | `cEgo.IgnoreWalkbehinds = true;` | ? | Probably not really needed in MonoAGS- with the combination of render layers, Z and parent-child relationships you have the ability control rendering order more easily
| InventoryQuantity | InventoryItem.Qty | `player.InventoryQuantity[iCash.ID]` | `iCash.Qty` |
| Loop | Animation.State.CurrentLoop | `cEgo.Loop` | `cEgo.Animation.State.CurrentLoop` |
| ManualScaling | IgnoreScalingArea | `cEgo.ManualScaling = true;` | `cEgo.IgnoreScalingArea = true;` | This is not a 1-to-1 fit. In MonoAGS you can still set manual scaling to be applied onto the walkable area scaling, even if `IgnoreScalingArea` is false.
| MovementLinkedToAnimation | MovementLinkedToAnimation | `cEgo.MovementLinkedToAnimation = false;` | `cEgo.MovementLinkedToAnimation = false;` |
| Moving | IsWalking | `if (cEgo.IsMoving) {}` | `if (cEgo.IsWalking) {}` |
| Name | DisplayName | `cEgo.Name = "Bernard";` | `cEgo.DisplayName = "Bernard";` |
| NormalView | Outfit[Animations.Walk] | `cEgo.NormalView` | `cEgo.Outfit[Animations.Walk];` |
| PreviousRoom | PreviousRoom | `if (cEgo.PreviousRoom == 5) {}` | `if (cEgo.PreviousRoom == elevator) {}` | In MonoAGS, `PreviousRoom` actually provides you with access to the entire room's API, not just its ID, so you can query the room's objects, for example.
| Room | Room | `if (cEgo.Room == 5) {}` | `if (cEgo.Room == elevator) {}` | In MonoAGS, `Room` actually provides you with access to the entire room's API, not just its ID, so you can query the room's objects, for example.
| ScaleMoveSpeed | AdjustWalkSpeedToScaleArea | `cEgo.ScaleMoveSpeed = true;` | `cEgo.AdjustWalkSpeedToScaleArea = true;` |
| ScaleVolume | scalingArea.ScaleVolume | `cEgo.ScaleVolume = true;` | `scalingArea.ScaleVolume = true;` | This is not a 1-to-1 match. In AGS, scale volume scales the volume according to the scaling of the character, and it doesn't matter if the scaling was set manually or in an area. In MonoAGS, this is specfically for areas, there is no equivalent configuration for manual scaling changes currently.
| Scaling | ScaleX and ScaleY | `cEgo.ManualScaling = true; cEgo.Scaling = 200;` | `cEgo.ScaleX = 2; cEgo.ScaleY = 2;` | In AGS the range is 5 to 200, where the value must be an integer and 100 is not scaled. In MonoAGS there's no "allowed" range, the value is a float (so you can do `cEgo.ScaleX = 0.5f`) and 1 is not scaled.
| Solid | ? | `if (cEgo.Solid) {}` | ? |
| Speaking | Outfit[Animations.Speak].Animation.State.IsPaused | `if (cEgo.Speaking) {}` | `if (!cEgo.Outfit[Animations.Speak].Animation.State.IsPaused) {}` |
| SpeakingFrame | Outfit[Animations.Speak].Animation.State.CurrentFrame | `cEgo.SpeakingFrame` | `cEgo.Outfit[Animations.Speak].Animation.State.CurrentFrame` |
| SpeechAnimationDelay | Outfit[Animations.Speak].Animation.Configuration.DelayBetweenFrames | `cEgo.SpeechAnimationDelay` | `cEgo.Outfit[Animations.Speak].Animation.Configuration.DelayBetweenFrames` |
| SpeechColor | SpeechConfig.TextConfig.Brush | `cEgo.SpeechColor = 14;` | `cEgo.SpeechConfig.TextConfig.Brush = blueSolidBrush;` |
| SpeechView | Outfit[Animations.Speak] | `cEgo.SpeechView` | `cEgo.Outfit[Animations.Speak]` |
| ThinkView | Outfit[Animations.Think] | `cEgo.ThinkView` | `cEgo.Outfit[Animations.Think]` | There's nothing particular about `Think` in MonoAGS currently, but using outfit you can assign and query specific animations, so you can create a "think" animation if it fits your game.
| Transparency | Opacity | `cEgo.Transparency = 100;` | `cEgo.Opacity = 0;` | The range for AGS transparency is 0-100, the range for MonoAGS opacity is 0-255
| TurnBeforeWalking | ? | `cEgo.TurnBeforeWalking = 1;` | ? |
| View | Animation | `cEgo.View` | `cEgo.Animation` |
| WalkSpeedX | WalkStep.X | `cEgo.WalkSpeedX` | `cEgo.WalkStep.X` |
| WalkSpeedY | WalkStep.Y | `cEgo.WalkSpeedY` | `cEgo.WalkStep.Y` |
| x | X | `cEgo.x` | `cEgo.X` |
| y | Y | `cEgo.y` | `cEgo.Y` |
| z | JumpOffset.Y | `cEgo.Z = 100;` | `cEgo.JumpOffset = new PointF(0, 100);` | This requires the jump component to be added to the character.

Missing in AGS but exists in MonoAGS: asynchronous speech/walk, configuring speech background color/shadows + outlines/text brushes/borders/alignments/text skipping/portraits, hooking/customizing speech/walk/path finding, getting walk destination, face direction with left/right/etc, face direction based on where somebody else is looking, iterating/querying inventory items, subscribing/unsubscribing interaction events during the game, more configurations for following, follow objects which are not characters, query the current follow target, and as a character is an extension of object, see the list for object for more stuff.

## DateTime

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Now | Now | `DateTime.Now` | `DateTime.Now` |
| DayOfMonth | DayOfMonth | `now.DayOfMonth` | `now.DayOfMonth` |
| Hour | Hour | `now.Hour` | `now.Hour` |
| Minute | Minute | `now.Minute` | `now.Minute` |
| Month | Month | `now.Month` | `now.Month` |
| RawTime | Need to calculate | `DateTime.Now.RawTime` | `(DateTime.UtcNow - new DateTime(1970,1,1)).TotalSeconds` |
| Second | Second | `now.Second` | `now.Second` |
| Year | Year | `now.Year` | `now.Year` |

Missing in AGS but exists in MonoAGS: well, nothing here is MonoAGS specific, this is all c#. You can see all available functions here: https://msdn.microsoft.com/en-us/library/system.datetime(v=vs.110).aspx

Also note, that if you need correct handling of time zones and DST, this a recommended library which you can add to your project: https://github.com/nodatime/nodatime

## Dialog

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| DisplayOptions | ? | `dOldMan.DisplayOptions();` | ? |
| GetOptionState | option.Label.UnderlyingVisible (and optionally combine with ShowOnce if you need to compare with "off forever") | `if (dJoeExcited.GetOptionState(2) == eOptionOffForever) {}` | `dOption = dJoeExcited.Options[2]; if (!dOption.Label.UnderlyingVisible && dOption.ShowOnce) {}` |
| GetOptionText | option.Label.Text | `dJoeExcited.GetOptionText(3)` | `dJoeExcited.Options[3].Label.Text` |
| HasOptionBeenChosen | option.HasOptionBeenChosen | `dJoeExcited.HasOptionBeenChosen(3)` | `dJoeExcited.Options[3].HasOptionBeenChosen` |
| ID | ? | `dJoeExcited.ID` | ? | It doesn't seem there's any need for an id for the dialog, you can just compare with the dialog reference if you need equality checks
| OptionCount | Options.Count | `dJoeExcited.OptionCount` | `dJoeExcited.Options.Count` |
| SetOptionState | Either set option.Label.Visible or option.ShowOnce | `dJoeExcited.SetOptionState(2, eOptionOff)` | For option off/on: `dJoeExcited.Options[2].Label.Visible = false;`, for off forever, you can add: `dJoeExcited.Options[2].ShowOnce = true;` |
| ShowTextParser | ? | `if (cJoeExcited.ShowTextParser) {}` | ? |
| Start | RunAsync | `dJoeExcited.Start();` | `dJoeExcited.RunAsync();` |
| StopDialog | ? | `dJoeExcited.StopDialog();` | ? |

Missing in AGS but exists in MonoAGS: create and change dialogs at run-time, customize appearances of everything dialog related, asynchronously wait for a dialog to complete, automatic grey-out (or any desired rendering) for already selected options, show/hide dialog options when speaking, run a specific dialog option on demand, enable/disable specific dialog actions.

## DialogOptionsRenderingInfo

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| ActiveOptionID | Not Needed | `info.ActiveOptionID = 1` | N/A | This is required for custom dialog rendering in AGS, as the assumption is you get a drawing surface and natively drawing all the options, and then AGS can't do hit-tests itself so you need to worry about it. This is not required in MonoAGS, as you can provide individual rendering for the dialog options and they can still be used as hit-test targets.
| DialogToRender | ? | `info.DialogToRender` | ? | The way to do custom rendering is a bit different in MonoAGS. There's no one single hook to customize the dialogs, but you can choose on which layer you want to provide your own different implementation. So you can provide a different implementation for `IDialogLayout` (which gets the dialog graphics and options graphics and chooses how to place them), or you can provide a different implementation for each (or for specific) `IDialogOption` to change how they are rendered/behave, or you can provide a different implementation for `IDialog` to completely rewrite the dialog mechanism (but still be able to hook it up to existing dialog code). Each of those custom implementations can be either changed for all dialogs or for specific dialogs.
| Height | dialog.Graphics.Height | `info.Height` | `dialog.Graphics.Height` |
| ParserTextBoxWidth | ? | `info.ParserTextBoxWidth` | ? |
| ParserTextBoxX | ? | `info.ParserTextBoxX` | ? |
| ParserTextBoxY | ? | `info.ParserTextBoxY` | ? |
| Surface | Not needed | `info.Surface` | N/A | See notes on "ActiveOptionID" and "DialogToRender" to see why this is not needed.
| Width | dialog.Graphics.Width | `info.Width` | `dialog.Graphics.Width` |
| X | dialog.Graphics.X | `info.X` | `dialog.Graphics.X` |
| Y | dialog.Graphics.Y | `info.Y` | `dialog.Graphics.Y` |

Missing in AGS but exists in MonoAGS: The whole process for custom dialog rendering is completely different (see notes on `DialogToRender`).

## DrawingSurface

Currently nothing built in that's equivalent for this, but one could directly implement `IImageRenderer`, assign it to its object with `obj.CustomRenderer = myRenderer` and use OpenGL in that renderer implementation to do everything desired.

## DynamicSprite

The concept of dynamic sprite is not really needed in MonoAGS, as everything is dynamic by default, so you can create objects, characters, animations, etc, all in run-time. So in this section, equivalent behaviors might be found on one or more levels: bitmaps, images (container above bitmap which also adds texture information), sprites (container above image which adds abilities for run-time transforms) and objects (which contain sprites as individual animation frames or a single image).

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Create | new EmptyImage | `DynamicSprite.Create(50, 30);` | `new EmptyImage(50, 30);` |
| CreateFromBackground | LoadImage | `DynamicSprite.CreateFromBackground()` | `graphicsFactory.LoadImage(state.Room.Image.OriginalBitmap)` |
| CreateFromDrawingSurface | ? | `DynamicSprite.CreateFromDrawingSurface(surface, 0, 0, 10, 10);` | ? | While there's nothing drawing area specific, one could implement a custom renderer for doing something like this.
| CreateFromExistingSprite | LoadImage | `DynamicSprite.CreateFromExistingSprite(20);` | `graphicsFactory.LoadImage(existingImage.OriginalBitmap)` |
| CreateFromFile | LoadImage | `DynamicSprite.CreateFromFile("door.png")` | `graphicsFactory.LoadImage("door.png")` |
| CreateFromSaveGame | ? | `DynamicSprite.CreateFromSaveGame(1, 50, 50)` | ? |
| CreateFromScreenShot | ? | `DynamicSprite.CreateFromScreenShot(80, 50)` | ? |
| ChangeCanvasSize | ? | `DynamicSprite.ChangeCanvasSize(sprite.Width + 10, sprite.Height, 5, 0);` | ? | While there's nothing specific for this, this could be implemented by manual bitmap manipulations (but it's tedious): you first create a bitmap with the new size, manually get pixels from the original bitmap and set them in the new bitmap, then load an image from the new bitmap.
| CopyTransparencyMask | ? | `DynamicSprite.CopyTransparencyMask` | ? | While there's nothing specific for this, this could be implemented by manual bitmap manipulations (but it's tedious): you first create a bitmap with the new size, manually get pixels from the original bitmap and set them in the new bitmap, then load an image from the new bitmap.
| Crop | bitmap.Crop | `sprite.Crop(10, 10, sprite.Width - 10, sprite.Height - 10);` | `graphicsFactory.LoadImage(sprite.OriginalBitmap.Crop(new Rectangle(10, 10, sprite.Width - 10, sprite.Height - 10)));` | Also, instead of cropping the bitmap, one could add an `ICropSelfComponent` to the object which will crop the rendered object at run-time (without touching the bitmap).
| Delete | ? | `sprite.Delete();` | ? |
| Flip | FlipHorizontally or FlipVertically | `sprite.Flip(eFlipUpsideDown);` | `obj.FlipHorizontally();` |
| GetDrawingSurface | ? | `sprite.GetDrawingSurface()` | ? | While there's nothing drawing area specific, one could implement a custom renderer for doing something like this.
| Resize | ScaleX and ScaleY | `sprite.Resize(100, 50);` | `obj.ScaleX = 100; obj.ScaleY = 50;` |
| Rotate | Angle | `sprite.Rotate(180);` | `obj.Angle = 180;` |
| SaveToFile | bitmap.SafeToFile | `sprite.SaveToFile("abc.png");` | `sprite.OriginalBitmap.SaveToFile("abc.png");` |
| Tint | Tint | `sprite.Tint(0, 250, 0, 30, 100);` | `sprite.Tint = Colors.Green;` or `sprite.Tint = Color.FromRgba(0, 255, 0, 255);` or `sprite.Tint = Color.FromHsla(200, 1, 1, 255);` or `sprite.Tint = Color.FromHexa(59f442);` |
| ColorDepth | ? | `sprite.ColorDepth` | ? |
| Graphic | ID | `sprite.Graphic` | `image.ID` |
| Height | Height | `sprite.Height` | `image.Height` |
| Width | Width | `sprite.Width` | `image.Width` |

## File

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Open | Open | `File *output = File.Open("temp.tmp", eFileWrite);` | `var output = File.Open("temp.tmp", FileMode.Create);` |
| Close | Close | `output.Close();` | `output.Close();` |
| Delete | Delete | `File.Delete("temp.tmp");` | `File.Delete("temp.tmp");` |
| Exists | Exists | `if (File.Exists("temp.tmp")) {}` | `if (File.Exists("temp.tmp")) {}` |
| ReadInt | BinaryReader.ReadInt32 | `int number; number = input.ReadInt();` | `using (var reader = new BinaryReader(File.OpenRead("file.txt")) ) { char c = reader.ReadInt32();` } |
| ReadRawChar | BinaryReader.ReadChar | `String buffer = String.Format("%c", input.ReadRawChar());` | `using (var reader = new BinaryReader(File.OpenRead("file.txt")) ) { char c = reader.ReadChar();` } |
| ReadRawInt | BinaryReader.ReadInt32 | `int number; number = input.ReadRawInt();` | `using (var reader = new BinaryReader(File.OpenRead("file.txt")) ) { char c = reader.ReadInt32();` } |
| ReadRawLineBack | StreamReader.ReadLine | `String line = input.ReadRawLineBack();` | `using (var reader = new StreamReader("file.txt")) { string line = reader.ReadLine();`} |
| ReadStringBack | BinaryReader.ReadString | `String buffer = input.ReadStringBack();` | `using (var reader = new BinaryReader(File.OpenRead("file.txt")) ) { char c = reader.ReadInt32();` |
| WriteInt | BinaryWriter.Write | `output.WriteInt(6);` | `using (var writer = new BinaryWriter(File.Open("temp.tmp", FileMode.Create))) { writer.Write(6);` |
| WriteRawChar | BinaryWriter.Write | `output.WriteRawChar('A');` | `using (var writer = new BinaryWriter(File.Open("temp.tmp", FileMode.Create))) { writer.Write('A');` |
| WriteRawLine | StreamWriter.WriteLine | `output.WriteRawLine("My line");` | `using (var writer = new StreamWriter("file.txt")) { writer.WriteLine("My line");` } |
| WriteString | BinaryWriter.Write | `output.WriteString("test string");` | `using (var writer = new BinaryWriter(File.Open("temp.tmp", FileMode.Create))) { writer.Write("test string");` |
| EOF | BinaryReader.BaseStream.Position | `while (!output.EOF) {}` | `while (reader.BaseStream.Position != reader.BaseStream.Length)` |
| Error | try/catch | `output.WriteInt(51); if (output.Error) { Display("Error writing the data!"); }` | `try { writer.Write(51); } catch (Exception e) { AGSMessageBox.DisplayAsync($"Error while writing the data. The error message is: {e.Message}"); }` |

Missing in AGS but exists in MonoAGS: well, nothing here is MonoAGS specific, this is all c#. You can see all available functions here:
https://msdn.microsoft.com/en-us/library/system.io.file(v=vs.110).aspx
https://msdn.microsoft.com/en-us/library/system.io.binaryreader(v=vs.110).aspx
https://msdn.microsoft.com/en-us/library/system.io.binarywriter(v=vs.110).aspx
https://msdn.microsoft.com/en-us/library/system.io.streamreader(v=vs.110).aspx
https://msdn.microsoft.com/en-us/library/system.io.streamwriter(v=vs.110).aspx

## Game / Global functions

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| AbortGame | throw | `AbortGame("Error in game!");` | `throw new Exception("Error im game!");` |
| CallRoomScript | Nothing specific, but you can create a shared interfaces between your rooms and call it. | `CallRoomScript(1); ... function on_call(int value) {...}` | `public interface IOnCall { void on_call(int value); } .. public class MyRoom : IOnCall { public void on_call(int value) {...} } ... (state.Room as IOnCall)?.on_call(1);` |
| ChangeTranslation | ? | `Game.ChangeTranslation("Spanish")` | ? |
| ClaimEvent | ? | `ClaimEvent();` | ? |
| Debug | ? | `Debug(0);` | ? |
| DeleteSaveSlot | N/A | `DeleteSaveSlot(130);` | `MonoAGS` doesn't have the concept of save slots, you can just delete the save file.
| DisableInterface | ? | `DisableInterface();` | There's nothing specific currently, but you can disable all GUI controls and change the cursor.
| DoOnceOnly | Repeat.OnceOnly | `if (Game.DoOnceOnly("open cupboard")) {}` | `if Repeat.OnceOnly("open cupboard") {}` |
| EnableInterface | ? | `EnableInterface();` | There's nothing specific currently, but you can disable all GUI controls and change the cursor.
| EndCutscene | Cutscene.End | `EndCutscene();` | `state.Cutscene.End();` |
| GetColorFromRGB | Color.FromRgba | `Game.GetColorFromRGB(0, 255, 0);` | `Color.FromRgba(0, 255, 0, 255);` |
| GetFrameCountForLoop | animation.Frames.Count | `Game.GetFrameCountForLoop(SWIMMING, 2);` | `cEgo.Outfit[Animations.Swim].Left.Frames.Count` |
| GetGameOption | ? | `GetGameOption(OPT_WALKONLOOK)` | ? | There's a lot of unrelated very specific configurations for AGS here, some of them have equivalents in MonoAGS, see `SetGameOption` for more details.
| GetGameSpeed | `state.Speed` | `GetGameSpeed();` | `game.State.Speed` |
| GetLocationName | `hitTest.ObjectAtMousePosition` | `if (GetLocationName(mouse.x, mouse.y) == "Hero") {}` | `if (hitTest.ObjectAtMousePosition.DisplayName == "Hero") {}` | Currently, there's no support for getting at specific (x,y) position, but just for where the mouse is at.
| GetLocationType | `hitTest.ObjectAtMousePosition` | `if (GetLocationType(mouse.x, mouse.y) == eLocationCharacter) {}` | `if (hitTest.ObjectAtMousePosition is ICharacter) {}` | Currently, there's no support for getting at specific (x,y) position, but just for where the mouse is at.
| GetLoopCountForView | GetAllDirections | `Game.GetLoopCountForView(SWIMMING)` | `cEgo.Outfit[Animation.Swim].GetAllDirections().Count()` |
| GetRunNextSettingForLoop | ? | `Game.GetRunNextSettingForLoop(SWIMMING, 5)` | ? |
| GetSaveSlotDescription | ? | `Game.GetSaveSlotDescription(10)` | ? |
| GetTextHeight | Font.MeasureString | `GetTextHeight("The message on the GUI!", Game.NormalFont, 100)` | `AGSGameSettings.DefaultTextFont.MeasureString("The message on the GUI!", 100).Height` |
| GetTextWidth | Font.MeasureString | `GetTextWidth("Hello!", Game.NormalFont)` | `AGSGameSettings.DefaultTextFont.MeasureString("Hello!").Width` |
| GetTranslation | ? | `GetTranslation("secret")` | ? |
| GetViewFrame | Frames[index] | `Game.GetViewFrame(SWIMMING, 2, 3)` | `cEgo.Outfit[Animations.Swim].Left.Frames[3]` |
| GiveScore | ? | `GiveScore(5)` | ? | Nothing specific in MonoAGS for this, but this could be easily implemented in just a few lines: `public static class Score { public static int Score { get; private set; } public static void GiveScore(int score) { Score += score; Sounds.Score.Play();}}`
| InputBox | ? | `String name = Game.InputBox("!What is your name?");` | ? |
| IsGamePaused | state.Paused | `if (IsGamePaused()) {}` | `if (game.State.Paused) {}` |
| IsInterfaceEnabled | ? | `if (IsInterfaceEnabled()) {}` | ? | There's nothing specific for this in `MonoAGS`, but you can query (and set) enabled/disabled for individual GUI components.
| IsInteractionAvailable | checking subscriber count on the interaction event | `if (IsInteractionAvailable(mouse.x,mouse.y, eModeLookat) == 0) {}` | `if (hitTest.ObjectAtMousePosition.Interactions.OnInteract(Verbs.Look).SubscribersCount == 0) {}` |
| IsKeyPressed | input.IsKeyDown | `if (IsKeyPressed(eKeyUpArrow)) {}` | `if (game.Input.IsKeyDown(Key.Up)) {}` |
| IsTimerExpired | Stopwatch.Elapsed | `SetTimer(1, 3000); ... if (IsTimerExpired(1)) {}` | `Stopwatch myTimer = new Stopwatch(); myTimer.Start(); ... if (myTimer.Elapsed.Seconds > 3) {}` |
| IsTranslationAvailable | ? | `if (IsTranslationAvailable() == 1) {}` | ? |
| PauseGame | state.Paused | `PauseGame();` | `game.State.Paused = true;` |
| ProcessClick | invoke the interaction event | `ProcessClick(100, 50, eModeLookAt);` | `cEgo.Interactions.OnInteract(Verbs.Look).InvokeAsync(new ObjectEventArgs(oKnife));` |
| QuitGame | game.Quit | `QuitGame(0);` | `game.Quit();` | No built-in support in `MonoAGS` for "ask first", though this could be easily coded by using a message box: `if (await AGSMessageBox.YesNoAsync("Are you sure you want to quit?")) { game.Quit(); } `
| Random | MathUtils.Random().Next | `int ran = Random(2);` | `int ran = MathUtils.Random().Next(0, 2);` |
| RestartGame | SaveLoad.Restart() | `RestartGame();` | `game.SaveLoad.Restart();` |
| RestoreGameDialog | AGSSelectFileDialog.SelectFile | `RestoreGameDialog();` | `await AGSSelectFileDialog.SelectFile("Select file to load", FileSelection.FileOnly);` |
| RestoreGameSlot | SaveLoad.Load | `RestoreGameSlot(5);` | `await game.SaveLoad.LoadAsync("save.bin");` |
| RunAGSGame | Process.Start |  `RunAGSGame ("MyGame.exe", 0, 51);` | `Process.Start("MyGame.exe");` |
| SaveGameDialog | AGSSelectFileDialog.SelectFile | `SaveGameDialog();` | `await AGSSelectFileDialog.SelectFile("Select file to save", FileSelection.FileOnly);` |
| SaveGameSlot | SaveLoad.Save | `SaveGameSlot(30, "save game");` | `await game.SaveLoad.SaveAsync("save.bin");` |
| SaveScreenShot | ? | `SaveScreenshot("pic.pcx");` | ? |
| SetAmbientTint | ? | `SetAmbientTint(0, 0, 250, 30, 100);` | ? |
| SetGameOption | ? | `SetGameOption(OPT_WALKONLOOK, 1);` | ? | There's a lot of unrelated very specific configurations for AGS here, some of them have equivalents in MonoAGS: `OPT_WALKONLOOK` + `OPT_NOWALKMODE` -> Configure the "approach" component, for example: `cEgo.ApproachStyle.ApproachWhenVerb[Verbs.Look] = ApproachHotspots.AlwaysWalk`, `OPT_PIXELPERFECT` -> can be configured per entity: `cEgo.PixelPerfect(false);`, `OPT_FIXEDINVCURSOR` -> can be configured per inventory item: `iKnife.CursoreGraphics = iKnife.Graphics;`, `OPT_CROSSFADEMUSIC` -> you have several more configuration options here, for example: `var crossFade = game.AudioSettings.RoomMusicCrossFading; crossFade.FadeIn = true; crossFade.FadeOut = false; crossFade.FadeInSeconds = 5f; crossFade.EaseFadeIn = Ease.QuadIn;`, `OPT_PORTRAITPOSITION` => `cEgo.SpeechConfig.PortraitConfig.Positioning = PortraitPositioning.Alternating;`
| SetGameSpeed | `state.Speed` | `SetGameSpeed(80);` | `game.State.Speed = 80;` |
| SetMultitaskingMode | ? | `SetMultitaskingMode(1);` | ? |
| SetRestartPoint | SaveLoad.SetRestartPoint | `SetRestartPoint();` | `game.SaveLoad.SetRestartPoint();` |
| SetSaveGameDirectory | ? | `Game.SetSaveGameDirectory("My cool game saves");` | ? | `MonoAGS` does not have a "save game directory" because when you save a game you select the directory to save in.
| SetTextWindowGUI | ? | `SetTextWindowGUI(4);` | ? |
| SetTimer | Stopwatch.Start | `SetTimer(1, 3000); ... if (IsTimerExpired(1)) {}` | `Stopwatch myTimer = new Stopwatch(); myTimer.Start(); ... if (myTimer.Elapsed.Seconds > 3) {}` |
| SkipUntilCharacterStops | ? | `SkipUntilCharacterStops(EGO);` | ? |
| StartCutscene | Cutscene.Start | `StartCutscene();` | `state.Cutscene.Start();` |
| UpdateInventory | N/A | `UpdateInventory();` | N/A | Not needed
| UnPauseGame | Paused | `UnPauseGame();` | `state.Paused = false;` |
| Wait | Task.Delay or Thread.Sleep | `Wait(80);` | `await Task.Delay(80);` or `Thread.Sleep(80);` | Note, that both methods are not perfect fit-ins, as it waits milliseconds and not game loops as in `Wait`, so we'll need to add another option.
| WaitKey | ? | `WaitKey(200);` | ? |
| WaitMouseKey | ? | `WaitMouseKey(200);` | ? |
| CharacterCount | calculate yourself | `Game.CharacterCount` | `state.Rooms.Select(r => r.Objects.Count(o => o is ICharacter)).Sum())` |
| DialogCount | ? | `Game.DialogCount` | ? |
| FileName | use dotnet functions | `Game.FileName` | `Process.GetCurrentProcess().MainModule.FileName` or `Path.GetFileName(Assembly.GetEntryAssembly().Location)` |
| FontCount | ? | `Game.FontCount` | ? |
| GlobalStrings | GlobalVariables.Strings | `Game.GlobalStrings[15] = "Joe";` | `state.GlobalVariables.Strings.SetValue("ImportantCharacterName", "Joe");` |
| GUICount | state.UI.Count | `Game.GUICount` | `state.UI.Count` |
| IgnoreUserInputAfterTextTimeoutMs | ? | `Game.IgnoreUserInputAfterTextTimeoutMs = 1000;` | ? | This is currently hard-coded to 500 ms in `MonoAGS` (in `FastFingerChecker` class), and you can bypass it with a custom value like this (should be done at the very start of the game): `FastFingerChecker checker = new FastFingerChecker { FastFingerSafeBuffer = 1000 }; Resolver.Override(resolver => resolver.Builder.RegisterInstance(checker));`
| InSkippableCutscene | Cutscene.IsRunning | `if (Game.InSkippableCutscene) {}` | `if (state.Cutscene.IsRunning) {}` |
| InventoryItemCount | ? | `Game.InventoryItemCount` | ? |
| MinimumTextDisplayTimeMs | ? | `Game.MinimumTextDisplayTimeMs = 1000;` | ? | Currently hard-coded to 40 ms in `MonoAGS`.
| MouseCursorCount | ? | `Game.MouseCursorCount` | ? |
| Name | Title | `Game.Name = "My game";` | `game.Title = "My game";` |
| NormalFont | AGSGameSettings.DefaultTextFont | `Game.NormalFont = eFontSpecial;` | `AGSGameSettings.DefaultTextFont = Fonts.Special;` |
| SkippingCutscene | Cutscene.IsSkipping | `if (!Game.SkippingCutscene) {}` | `if (!state.Cutscene.IsSkipping) {}` |
| SpeechFont | AGSGameSettings.DefaultSpeechFont | `Game.SpeechFont = eFontStandard;` | `AGSGameSettings.DefaultSpeechFont = Fonts.Standard;` |
| SpriteHeight | sprite.Height | `Game.SpriteHeight[15]` | `animation.Left.Frames[3].Sprite.Height` |
| SpriteWidth | sprite.Width | `Game.SpriteWidth[15]` | `animation.Left.Frames[3].Sprite.Width` |
| TextReadingSpeed | SayConfig.TextDelay | `Game.TextReadingSpeed = 10;` | `cEgo.SayConfig.TextDelay = 100;` | Note the difference in units: in `AGS` it stands for "number of characters to read in a second", where in `MonoAGS` it stands for "number of milliseconds to wait for each character".
| TranslationFilename | ? | `if (Game.TranslationFilename == "German") {}` | ? |
| UseNativeCoordinates | N/A | `if (Game.UseNativeCoordinates) {}` | N/A | Not needed in `MonoAGS` (there is no low resolution backwards-compatible mode)
| ViewCount | ? | `Game.ViewCount` | ? |

## GUI

In AGS there's a separation between GUI and GUI controls, where GUI is a panel containing other controls.
In MonoAGS there's no distinction like this, as every control can contain other controls, however there is a "Panel" in MonoAGS which is a naked UI control without any other components added to it, which is probably the closest equivalent for AGS "GUI".

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Centre | Position yourself | `gPanel.Centre();` | `gPanel.Pivot = new PointF(0.5f, 0.5f); gPanel.X = game.Settings.VirtualResolution.Width / 2; gPanel.Y = game.Settings.VirtualResolution.Height / 2;` | The example assumes that the panel has no parent and using the default game's resolution.
| GetAtScreenXY | hitTest.ObjectAtMousePosition | `GUI.GetAtScreenXY(mouse.x, mouse.y)` | `hitTest.ObjectAtMousePosition` | Missing support for specific location checks.
| SetPosition | Location | `gPanel.SetPosition(50, 50);` | `gPanel.Location = new AGSLocation(50, 50);` |
| SetSize | BaseSize | `gPanel.setSize(100, 100);` | `gPanel.BaseSize = new SizeF(100, 100);` |
| BackgroundGraphic | Image | `gPanel.BackgroundGraphic = 5;` | `gPanel.Image = myBackgroundImage;` |
| Clickable | Enabled or ClickThrough | `gPanel.Clickable = false;` | `gPanel.Enabled = false;` or `gPanel.ClickThrough = false;` | Note the different between `Enabled` and `ClickThrough` in `MonoAGS`: `Enabled` disables the panel and all of the controls within, while `ClickThrough` disables the panel itself but still allows for inner children to respond.
| ControlCount | TreeNode.ChildrenCount | `gPanel.ControlCount` | `gPanel.TreeNode.ChildrenCount` |
| Controls | TreeNode.Children | `gPanel.Controls` | `gPanel.TreeNode.Children` |
| Height | BaseSize.Height | `gPanel.Height = 100;` | `gPanel.BaseSize = new SizeF(gPanel.BaseSize.Width, 100);` |
| ID | ID | `gPanel.ID` | `gPanel.ID` |
| Transparency | Opacity | `gPanel.Transparency = 100;` | `gPanel.Opacity = 0;` | The range for AGS transparency is 0-100, the range for MonoAGS opacity is 0-255
| Visible | Visible | `gPanel.Visible = true;` | `gPanel.Visible = true;` |
| Width | BaseSize.Width | `gPanel.Width = 100;` | `gPanel.BaseSize = new SizeF(100, gPanel.BaseSize.Height);` |
| X | X | `gPanel.X = 5;` | `gPanel.X = 5;` |
| Y | Y | `gPanel.Y = 5;` | `gPanel.Y = 5;` |
| ZOrder | Z | `gPanel.ZOrder = 5;` | `gPanel.Z = 5;` |

Missing in AGS but exists in MonoAGS: scaling and rotating panels, scrolling panels, nesting panels (or any other object) within panels (or any other object), placing GUIs as part of the world (behind non-GUIs), different resolution from the game, custom rendering (including shaders), mouse events (enter, leave, move, click, double-click, down, up, lost focus), sub-pixel positioning, skinning, and also, as panels extend objects, see objects for more stuff.

## GUI Control

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| GetAtScreenXY | hitTest.ObjectAtMousePosition | `GUIControl.GetAtScreenXY(mouse.x, mouse.y)` | `hitTest.ObjectAtMousePosition` | Missing support for specific location checks.
| AsType | as | `gIconbar.Controls[2].AsButton` | `gIconBar.TreeNode.Children[2] as IButton` |
| BringToFront | Z | `btnBigButton.BringToFront()` | `btnBigButton.Z = btnBigButton.TreeNode.Parent.TreeNode.Children.Max(c => c.Z) + 1;` |
| Clickable | ClickThrough | `btnSaveGame.Clickable = false;` | `btnSaveGame.ClickThrough = true;` |
| Enabled | Enabled | `btnSaveGame.Enabled = false;` | `btnSaveGame.Enabled = false;` |
| Height | BaseSize.Height | `btnConfirm.Height = 20;` | `btnConfirm.BaseSize = new SizeF(btnConfirm.BaseSize.Width, 20);`; |
| ID | ID | `btnConfirm.ID` | `btnConfirm.ID` |
| OwningGUI | TreeNode.Parent | `btnConfirm.OwningGUI` | `btnConfirm.TreeNode.Parent` |
| SendToBack | Z | `btnBigButton.SendToBack()` | `btnBigButton.Z = btnBigButton.TreeNode.Parent.TreeNode.Children.Min(c => c.Z) - 1;` |
| SetPosition | Location | `btnConfirm.SetPosition(40, 10);` | `btnConfirm.Location = new AGSLocation(40, 10);` |
| SetSize | BaseSize | `invMain.SetSize(160, 100);` | `invMain.BaseSize = new SizeF(160, 100);` |
| Visible | Visible | `btnSaveGame.Visible = false;` | `btnSaveGame.Visible = false;` |
| Width | BaseSize.Width | `btnConfirm.Width = 20;` | `btnConfirm.BaseSize = new SizeF(20, btnConfirm.BaseSize.Height);`; |
| X | X | `btnConfirm.X = 10;` | `btnConfirm.X = 10;` |
| Y | Y | `btnConfirm.Y = 20;` | `btnConfirm.Y = 20;` |

Missing in AGS but exists in MonoAGS: scaling and rotating controls, nesting controls (or any other object) within controls (or any other object), placing GUI controls as part of the world (behind non-GUIs), different resolution from the game, custom rendering (including shaders), mouse events (enter, leave, move, click, double-click, down, up, lost focus), sub-pixel positioning, skinning, and also, as the controls extend objects, see objects for more stuff.

## Button

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Animate | AnimateAsync | `btnDeathAnim.Animate(6, 2, 4, eRepeat);` | `btnDeathAnim.AnimateAsync(deathAnimation); //The delay & repeat for the animation is configured in the animation configuration` |
| ClipImage | image is always clipped to the button size | `btnOK.ClipImage = true;` | N/A |
| Font | TextConfig.Font | `btnOK.Font = eFontMain;` | `btnOK.TextConfig.Font = Fonts.Main;` |
| Graphic | Image | `btnPlay.Graphic` | `btnPlay.Image` |
| MouseOverGraphic | HoverAnimation | `btnPlay.MouseOverGraphic = 5;` | `btnPlay.HoverAnimation = buttonHoverAnimation;` |
| NormalGraphic | IdleAnimation | `btnPlay.NormalGraphic = 5;` | `btnPlay.IdleAnimation = buttonIdleAnimation;` |
| PushedGraphic | PushedAnimation | `btnPlay.PushedGraphic = 5;` | `btnPlay.PushedAnimation = buttonPushedAnimation;` |
| Text | Text | `btnPlay.Text = "Play";` | `btnPlay.Text = "Play";` |
| TextColor | TextConfig.Brush | `btnPlay.TextColor = 15;` | `btnPlay.TextConfig.Brush = solidWhiteBrush;` |

Missing in AGS but exists in MonoAGS: animations for button states, shadows + outlines/text brushes/alignments/auto-fitting, borders, see GUI controls for more.

## InvWindow

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| ScrollDown | ScrollDown | `invMain.ScrollDown();` | `invMain.ScrollDown();` |
| ScrollUp | ScrollUp | `invMain.ScrollUp();` | `invMain.ScrollUp();` |
| CharacterToUse | Inventory | `invMain.CharacterToUse = cJack;` | `invMain.Inventory = cJack.Inventory;` |
| ItemAtIndex | Inventory.Items[] | `item = invMain.ItemAtIndex[0];` | `item = invMain.Inventory.Items[0];` |
| ItemCount | Inventory.Items.Count | `invMain.ItemCount` | `invMain.Inventory.Items.Count` |
| ItemHeight | ItemSize.Height | `invMain.ItemHeight = 30;` | `invMain.ItemSize = new SizeF(50, 30);` |
| ItemWidth | ItemSize.Width | `invMain.ItemWidth = 50;` | `invMain.ItemSize = new SizeF(50, 30);` |
| ItemsPerRow | ItemsPerRow | `invMain.ItemsPerRow` | `invMain.ItemsPerRow` |
| RowCount | RowCount | `invMain.RowCount` | `invMain.RowCount` |
| TopItem | TopItem | `invMain.TopItem = 0;` | `invMain.TopItem = 0;` |

Missing in AGS but exists in MonoAGS: The ability to attach inventories (and show them in the inventory window) to non-characters, see GUI controls for more.

## Label

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Font | TextConfig.Font | `lblStatus.Font = eFontMain;` | `lblStatus.TextConfig.Font = Fonts.Main;` |
| Text | Text | `lblStatus.Text = "Play";` | `lblStatus.Text = "Play";` |
| TextColor | TextConfig.Brush | `lblStatus.TextColor = 15;` | `lblStatus.TextConfig.Brush = solidWhiteBrush;` |

Missing in AGS but exists in MonoAGS: shadows + outlines/text brushes/alignments/auto-fitting, borders, see GUI controls for more.

## ListBox

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| AddItem | Items.Add | `lstChoices.AddItem("Hello");` | `lstChoices.Items.Add(new AGSStringItem("Hello"));` |
| Clear | Items.Clear | `lstChoices.Clear();` | `lstChoices.Items.Clear();` |
| FillDirList | ? | `lstSaveGames.FillDirList("agssave.*");` | ? |
| FillSaveGameList | ? | `lstSaveGames.FillSaveGameList();` | ? |
| GetItemAtLocation | hitTest.ObjectAtMousePosition | `lstOptions.GetItemAtLocation(mouse.x, mouse.y)` | `hitTest.ObjectAtMousePosition` | Missing support for specific location checks.
| InsertItemAt | Items.Insert | `lstChoices.InsertItemAt(1, "Third item");` | `lstChoices.Items.Insert(1, new AGSStringItem("Third item"));` |
| RemoveItem | Items.RemoveAt | `lstChoices.RemoveItem(0);` | `lstChoices.Items.RemoveAt(0);` |
| ScrollDown | ? | `lstTest.ScrollDown();` |
| ScrollUp | ? | `lstTest.ScrollUp();` |
| Font | either set fonts on individual rows in the listbox, or set a default font in the factory | `lstSaveGames.Font = eFontSpeech;` | For individual rows: `lstSaveGames.ItemButtons[3].TextConfig.Font = Fonts.MyFont;`, a global font using a factory: `var defaultFactory = lstSaveGames.ItemButtonFactory; lstSaveGames.ItemButtonFactory = text => { var button = defaultFactory(text); button.TextConfig.Font = Fonts.MyFont; }` |
| HideBorder | Border | `lstSaveGames.HideBorder = true;` | `lstSaveGames.Border = null;` |
| HideScrollArrows | Scrolling component.Vertical/HorizontalScrollBar | `lstSaveGames.HideScrollArrows = true;` | `var scrolling = lstSaveGames.GetComponent<IScrollingComponent>(); scrolling.VerticalScrollBar = null;` |
| ItemCount | Items.Count | `lstChoices.ItemCount` | `lstChoices.Items.Count` |
| Items | Items | `lstOptions.Items[3]` | `lstOptions.Items[3]` |
| RowCount | ? | `lstOptions.RowCount` | ? | Note, that the listbox in `MonoAGS` has a smooth scrollbar, meaning it might show part of a row if the scrollbar is set just in the middle of the row.
| SaveGameSlots | ? | `lstSaveGames.SaveGameSlots[index]` | ? |
| SelectedIndex | SelectedIndex | `lstSaveGames.SelectedIndex` | `lstSaveGames.SelectedIndex` |
| TopItem | ? | `lstSaveGames.TopItem = 0;` | ? |

Missing in AGS but exists in MonoAGS: smooth scrolling for the listbox, SelectedItem, control individual appearances of rows/scrollbars/panel, allow automatic resizing of the box with minimum and maximum height, change events, search filter, see GUI controls for more.

## Slider

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| BackgroundGraphic | Graphics | `sldHealth.BackgroundGraphic = 5;` | `sldHealth.Graphics = animatedSliderBackground;` |
| HandleGraphic | HandleGraphics | `sldHealth.HandleGraphic = 6;` | `sldHealth.HandleGraphics = animatedSliderHandle;` |
| HandleOffset | Add the jump component to the handle graphics | `sldHealth.HandleOffset = 2;` | `var jump = sldHealth.HandleGraphics.AddComponent<IJumpOffsetComponent>(); jump.JumpOffset = new PointF(2, 0);` |
| Max | MaxValue | `sldHealth.Max = 200;` | `sldHealth.MaxValue = 200;` |
| Min | MinValue | `sldHealth.Min = 200;` | `sldHealth.MinValue = 200;` |
| Value | Value | `sldHealth.Value = 100;` | `sldHealth.Value = 100;` |

Missing in AGS but exists in MonoAGS: animations for slider background + handle, subscribing to slider events, non-integer values for the slider, see GUI controls for more.

## Text Box

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Font | TextConfig.Font | `txtUserInput.Font = eFontNormal;` | `txtUserInput.TextConfig.Font = Fonts.MyFont;` |
| Text | Text | `txtUserInput.Text = "Hello";` | `txtUserInput.Text = "Hello";` |
| TextColor | TextConfig.Brush | `txtUserInput.TextColor = 5;` | `txtUserInput.TextConfig.Brush = solidRedBrush;` |

Missing in AGS but exists in MonoAGS: configuring background color/shadows + outlines/text brushes/borders/alignments/auto-fitting, configure caret flashing speed, query and set the caret position, set a watermark for the textbox, focus/unfocus, subscribe to text change events on the textbox with the option to undo entered text, see GUI controls for more.

## Hotspot

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| GetAtScreenXY | IHitTest.ObjectAtMousePosition | `if (Hotspot.GetAtScreenXY(mouse.x, mouse.y) == hDoor){}` | `if (hitTest.ObjectAtMousePosition == hDoor) {}` | Missing support for specific location checks.
| GetProperty | Properties.Ints.GetValue | `if (hDoor.GetProperty("Value") > 200) {}` | `if (hDoor.Properties.Ints.GetValue("Value") > 200) {}` |
| GetTextProperty | Properties.Strings.GetValue | `hDoor.GetTextProperty("Description");` | `hDoor.Properties.Strings.GetValue("Description");` |
| RunInteraction | Interactions.OnInteract(Verb).InvokeAsync | `hDoor.RunInteraction(eModeLookat);` | `hDoor.Interactions.OnInteract(Verbs.Look).InvokeAsync();` |
| Enabled | Enabled | `hDoor.Enabled = false;` | `hDoor.Enabled = false;` |
| ID | ID | `hDoor.ID` | `hDoor.ID` |
| Name | DisplayName | `hDoor.Name` | `hDoor.DisplayName` |
| WalkToX | WalkPoint.X | `hDoor.WalkToX` | `hDoor.WalkPoint.X` |
| WalkToY | WalkPoint.Y | `hDoor.WalkToY` | `hDoor.WalkPoint.Y` |

Missing in AGS but exists in MonoAGS: Change hotspot name at run-time, change hotspot walk-point at run-time, rotating/scaling hotspot area (at run-time), and as hotspot extends object, see Object for more.

## Inventory Item

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| GetAtScreenXY | IHitTest.ObjectAtMousePosition | `if (InventoryItem.GetAtScreenXY(mouse.x, mouse.y) == iKnife){}` | `if (hitTest.ObjectAtMousePosition == iKnife.Graphics) {}` | Missing support for specific location checks.
| GetProperty | Properties.Ints.GetValue | `if (iKnife.GetProperty("Value") > 200) {}` | `if (iKnife.Graphics.Properties.Ints.GetValue("Value") > 200) {}` |
| GetTextProperty | Properties.Strings.GetValue | `iKnife.GetTextProperty("Description");` | `iKnife.Graphics.Properties.Strings.GetValue("Description");` |
| IsInteractionAvailable | checking subscriber count on the interaction event | `if (iKeyring.IsInteractionAvailable(eModeLookat) == 0) {}` | `if (iKeyring.Interactions.OnInteract(Verbs.Look).SubscribersCount == 0) {}` |
| RunInteraction | Interactions.OnInteract(Verb).InvokeAsync | `iKeyring.RunInteraction(eModeLookat);` | `iKeyring.Interactions.OnInteract(Verbs.Look).InvokeAsync();` |
| CursorGraphic | CursorGraphics | `iKey.CursorGraphic = 5;` | `iKey.CursorGraphics = animatedKeyCursor;` |
| Graphic | Graphics | `iKey.Graphic = 5;` | `iKey.Graphics = animatedKey;` |
| ID | Graphics.ID | `iKey.ID` | `iKey.Graphics.ID` |
| Name | Graphics.DisplayName | `iKey.Name` | `iKey.Graphics.DisplayName` |

Missing in AGS but exists in MonoAGS: animated inventory items (and cursors), inventory items extend objects so you can do with them everything you can do with objects (rotate, scale, etc), see Object for more.

## Maths

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| FloatToInt | Floor or Ceiling or Round followed by casting to int | `FloatToInt(10.7, eRoundNearest)` | `(int)Math.Round(10.7f)`
| IntToFloat | cast to float | `IntToFloat(myIntValue)` | `(float)myIntValue` |
| ArcCos | Acos | `float angle = Maths.ArcCos(1.0);` | `float angle = Math.Acos(1)`; |
| ArcSin | Asin | `float angle = Maths.ArcSin(0.5);` | `float angle = Math.Asin(0.5f)`; |
| ArcTan | Atan | `float angle = Maths.ArcTan(0.5);` | `float angle = Math.Atan(0.5f);` |
| ArcTan2 | Atan2 | `float angle = Maths.ArcTan2(-862.42, 78.5149);` |
| Cos | Cos | `float x = Maths.Cos(100);` | `float x = Math.Cos(100);` |
| Cosh | Cosh | `float x = Maths.Cosh(100);` | `float x = Math.Cosh(100);` |
| DegreesToRadians | MathUtils.DegreesToRadians | `float radians = Maths.DegreesToRadians(360.0);` | `float radians = MathUtils.DegreesToRadians(360);` |
| Exp | Exp | `float expValue = Maths.Exp(2.302585093);` | `float expValue = Math.Exp(2.302585093f);` |
| Log | Log | `float logVal = Maths.Log(9000.0);` | `float logVal = Math.Log(9000);` |
| Log10 | Log10 | `float logVal = Maths.Log10(9000.0);` | `float logVal = Math.Log10(9000);` |
| RadiansToDegrees | ? | `float val = Maths.RadiansToDegrees(angle);` | `float val = angle * (180f / Math.PI);` |
| RaiseToPower | Pow | `float value = Maths.RaiseToPower(4.5, 3.0);` | `float value = Math.Pow(4.5f, 3);` |
| Sin | Sin | `float value = Maths.Sin(50.0);` | `float value = Math.Sin(50);` |
| Sinh | Sinh | `float value = Maths.Sinh(50.0);` | `float value = Math.Sinh(50);` |
| Sqrt | Sqrt | `float value = Maths.Sqrt(9.0);` | `float value = Math.Sqrt(9);` |
| Tan | Tan | `float value = Maths.Tan(9.0);` | `float value = Math.Tan(9);` |
| Tanh | Tanh | `float value = Maths.Tanh(9.0);` | `float value = Math.Tan(9);` |
| Pi | PI | `Maths.Pi` | `Math.PI` |

Missing in AGS but exists in MonoAGS: well, almost nothing here is MonoAGS specific, this is all c# Math class, which you can view here: https://msdn.microsoft.com/en-us/library/system.math(v=vs.110).aspx
Also, MonoAGS has some additional useful math methods in `MathUtils` like Lerp and Clamp which can be useful.

## Mouse

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| ChangeModeGraphic | input.Cursor | `mouse.ChangeModeGraphic(eModeLookat, 120);` | `game.Input.Cursor = myLookCursor;` | Note that the AGS "ChangeModeGraphic" function changes how the mouse cursor automatically changes, and the example shown here changes the cursor manually. For automatic changes, each control scheme might have different methods that you need to call as the logic might be completely different. For the rotating cursors scheme, for example, you'd call `scheme.AddCursor(Verbs.Look, myLookCursor, true);`
| ChangeModeHotspot | change the pivot point on the cursor's object | `mouse.ChangeModeHotspot(eModeWalkTo, 10, 10);` | `walkCursor.Pivot = new PointF(0.1f, 0.1f);` | Note that the pivot point is in relative co-ordinates to the graphics, so (0.5, 0.5) is the center of the image, for example.
| ChangeModeView | input.Cursor | `mouse.ChangeModeView(eModeLookat, ROLLEYES);` | `game.Input.Cursor = myLookCursor;` | See notes on ChangeModeGraphic
| DisableMode | ? | `mouse.DisableMode(eModeWalkto);` | ? |
| EnableMode | ? | `mouse.EnableMode(eModeWalkto);` | ? |
| GetModeGraphic | ? | `mouse.GetModeGraphic(eModeWalkto);` | ? | There's nothing specific, but you can just query the specific mouse cursor that you're interested about
| IsButtonDown | LeftMouseButtonDown or RightMouseButtonDown | `if (mouse.IsButtonDown(eMouseRight)) {}` | `if (game.Input.RightMouseButtonDown)` |
| SaveCursorUntilItLeaves | use the cursor component | when hovering over "myHotspot": `mouse.SaveCursorUntilItLeaves(); mouse.Mode = eModeTalk;` | `var cursorComponent = myHotspot.AddComponent<IHasCursorComponent>(); cursorComponent.SpecialCursor = myAnimatedSpecialCursorForThisHotspot;` |
| SelectNextMode | ? | `Mouse.SelectNextMode()` | In MonoAGS, by choosing the `RotatingCursorsScheme` as your control scheme, this is already handled (and you can look at the code if you want to handle it differently)
| SetBounds | Subscribe to mouse move event and change the position of the mouse accordingly | `mouse.SetBounds(160, 100, 320, 200);` | `game.Input.MouseMove.Subscribe(args => if (args.MousePosition.XWindow > 160) OpenTK.Mouse.SetPosition(160, args.MousePosition.YWindow));` |
| SetPosition | OpenTK.Mouse.SetPosition | `mouse.SetPosition(160, 100);` | `OpenTK.Mouse.SetPosition(160, 100);` |
| Update | ? | `mouse.Update();` | ? |
| UseDefaultGraphic | ? | `mouse.UseDefaultGraphic();` | ? |
| UseModeGraphic | N/A | `mouse.UseModeGraphic(eModeWait)` | ? | This can be different depending on the control scheme you chose for the game. For the RotatingCursorsScheme for example, you'd write: `scheme.Mode = Verbs.Wait;` |
| Mode | input.Cursor | `if (mouse.Mode == eModeWalkto) {}` | `if (game.Input.Cursor == myWalkCursor) {}` | For individual control schemes, you might have the concept of "Mode", but it's not related to the mouse. In RotatingCursorsScheme, for example, you can query `if (scheme.Mode == Verbs.Walk) {}`.
| Visible | Input.Cursor.Visible | `mouse.Visible = false;` | `game.Input.Cursor.Visible = false;`

Missing in AGS but exists in MonoAGS: The cursors is just an extension of objects, so they can be manipulated in all ways an object can be manipulated (see Object for more).

## Multimedia

Currently there are no equivalents to any of the multimedia functions in MonoAGS.

## Object

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Animate  | AnimateAsync | `oRope.Animate(3, 1, 0, eBlock, eBackwards);` | For blocking: `await oRope.AnimateAsync(jumpUpAnimation);`. For non-blocking, do the same just without awaiting it: `oRope.AnimateAsync(jumpUpAnimation);`. As for delay, repeat style and direction, those are configured as part of the animation ("jumpUpAnimation" in this scenario). It can be changed at run-time before animating, if you want. For example: `jumpUpAnimation.Looping = LoopingStyle.BackwardsForwards; jumpUpAnimation.Loops = 15; jumpUpAnimation.DelayBetweenFrames = 3;` | Note that `MonoAGS` doesn't have the concepts of view and loop, just individual animations for manual animations, and directional animations for automatic animations like walk and idle.
| GetAtScreenXY  | IHitTest.ObjectAtMousePosition | `if (Object.GetAtScreenXY(mouse.x, mouse.y) == oRope){}` | `if (hitTest.ObjectAtMousePosition == oRope) {}` | Missing support for specific location checks.
| GetProperty | Properties.Ints.GetValue | `if (oRope.GetProperty("Value") > 200) {}` | `if (oRope.Properties.Ints.GetValue("Value") > 200) {}` |
| GetTextProperty | Properties.Strings.GetValue | `oRope.GetTextProperty("Description");` | `oRope.Properties.Strings.GetValue("Description");` |
| IsCollidingWithObject (character) | CollidesWith | `if (oRope.IsCollidingWithChar(oBottle) == 1) {}` | `if (oRope.CollidesWith(oBottle.X, oBottle.Y, state.Viewport)) {}` | Note that MonoAGS supports multiple viewports so we need to pass the viewport in which we'd like to test for collisions.
| MergeIntoBackground | ? | `object[3].MergeIntoBackground();` | ? |
| Move | TweenX & TweenY | `object[2].Move(125, 40, 4, eBlock);` | For blocking: `await oRope.TweenX(125, 3, Ease.Linear);`, for non-blocking do the same just without awaiting it: `oRope.TweenX(125, 3, Ease.Linear);` |
| RemoveTint | Tint | `oRope.RemoveTint();` | `oRope.Tint = Colors.White;` |
| RunInteraction | Interactions.OnInteract(Verb).InvokeAsync | `oRope.RunInteraction(eModeTalk);` | `oRope.Interactions.OnInteract(Verbs.Talk).InvokeAsync();` |
| SetPosition | Location | `oRope.SetPosition(50, 50);` | `oRope.Location = new AGSLocation(50, 50);` |
| SetView | N/A | `object[3].SetView(14);` | No need | In AGS this is a command that must come before calling "Animate" so that AGS would know which animation to run. In MonoAGS you just pass the animation object to the "Animate" function, so SetView becomes redundant.
| StopAnimating | Set an image | `oRope.StopAnimating();` | `oRope.Image = oRope.CurrentSprite.Image`; |
| StopMoving | Stop the previous tween(s) | `oRope.StopMoving();` | `tween.Stop(TweenCompletion.Stay);` |
| Tint | Tint | `oRope.Tint(0, 250, 0, 30, 100);` | `oRope.Tint = Colors.Green;` or `cEoRopego.Tint = Color.FromRgba(0, 255, 0, 255);` or `oRope.Tint = Color.FromHsla(200, 1, 1, 255);` or `oRope.Tint = Color.FromHexa(59f442);` |
| Animating | Animation.State.IsPaused | `if (oRope.Animating) {}` | `if (!oRope.Animation.State.IsPaused) {}` |
| Baseline | Z | `oRope.Baseline = 40;` | `oRope.Z = 40;` |
| BlockingHeight | ? | `oRope.BlockingHeight = 20;` | ? |
| BlockingWidth | ? | `oRope.BlockingWidth = 20;` | ? |
| Clickable | Enabled | `oRope.Clickable = false;` | `oRope.Enabled = false;` |
| Frame | Animation.State.CurrentFrame | `oRope.Frame` | `oRope.Animation.State.CurrentFrame` |
| Graphic | Image | `oRope.Graphic = 100;` | `oRope.Image = ropeImage;` |
| ID | ID | `oRope.ID` | `oRope.ID` |
| IgnoreScaling | IgnoreScalingArea | `oRope.IgnoreScaling = true;` | `oRope.IgnoreScalingArea = true;` |
| IgnoreWalkbehinds | ? | `oRope.IgnoreWalkbehinds = true;` | ? | Probably not really needed in MonoAGS- with the combination of render layers, Z and parent-child relationships you have the ability control rendering order more easily
| Loop | Animation.State.CurrentLoop | `oRope.Loop` | `oRope.Animation.State.CurrentLoop` |
| Moving | query the previous tween(s) | `if (oRope.Moving) {}` | `if (myTween.State == TweenState.Playing) {}` |
| Name | DisplayName | `oRope.Name` | `oRope.DisplayName` |
| Solid | ? | `oRope.Solid = true;` | ? |
| Transparency | Opacity | `oRope.Transparency = 100;` | `oRope.Opacity = 0;` | The range for AGS transparency is 0-100, the range for MonoAGS opacity is 0-255
| View | Animation | `oRope.View` | `oRope.Animation` |
| Visible | Visible | `oRope.Visible = true;` | `oRope.Visible = true;` |
| X | X | `oRope.X = 50;` | `oRope.X = 50.5f;` |
| Y | Y | `oRope.Y = 50;` | `oRope.Y = 50.5f;` |

Missing in AGS but exists in MonoAGS: scaling and rotating (with setting a pivot point), the ability to scale/rotate/translate individual animation frames, composition of objects (i.e nesting objects in other objects), mix & match with GUI, move between rooms, different resolution from the game, custom rendering (including shaders), sub-pixel positioning, rendering in multiple viewports, creation at run-time, selecting between pixel perfect or bounding box collision checks, objects are transitive with all other on-screen items (characters, GUIs), cropping objects, surround with borders, set hotspot text at runtime, controlling texture offset & scaling filter (per texture), subscribing to events (on pretty much anything that might change in any of the components), interactions with custom verbs, ability to extend objects with custom components, ability to replace engine implementation of components with your own (i.e implement your own collider component and provide custom collision checks, for example).

## Overlay

The whole concept of overlays in AGS is for allowing to show graphics/text at run-time. In MonoAGS, all objects can be created at run-time so the whole concept of "overlay" is not really needed.

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| CreateGraphical | Create an object | `Overlay* myOverlay = Overlay.CreateGraphical(100, 100, 300, true);` | `IObject myObj = game.Factory.Object.GetObject("id for my object"); myObj.Image = game.Factory.Graphics.LoadImageAsync("overlay.png"); myObj.X = 100; myObj.Y = 100; game.State.Room.Objects.Add(myObj);` |
| CreateTextual | Create a label | `Overlay* myOverlay = Overlay.CreateTextual(50,80,120, Game.SpeechFont, 15,"This is a text overlay");` | `ILabel myLabel = game.Factory.UI.GetLabel("id for my label", "This is a text overlay", 120, 0, 50, 80, addToUi: false); myLabel.TextConfig.AutoFit = AutoFit.TextShouldWrapAndLabelShouldFitHeight; myLabel.TextConfig.Font = AGSGameSettings.DefaultSpeechFont; game.State.Room.Objects.Add(myLabel);` | Note that when getting the label from the factory we passed "addToUi: false", this is to closely simulate the overlay behavior in AGS, where the overlay is local to the room and not global like a GUI (so instead of adding to UI, we add the label to the current room). This is still not identical behavior, as the AGS overlay gets removed when you switch to another room, and in MonoAGS the label will still be there if you return to the room (if you want the exact same behavior, subscribe to the room leave event and remove the label).
| Remove | Remove the object from the room (or from the global GUI list) | `myOverlay.Remove();` | If the object is in a specific room: `myRoom.Objects.Remove(myObj);`, and if the object is in the global GUI list: `game.State.UI.Remove(myObj);` |
| SetText | Set desired properties on the label | `myOverlay.SetText(120,Game.SpeechFont,15,"This is another text overlay");` | `myLabel.Text = "This is another text overlay"; myLabel.BaseSize = new SizeF(120, 0); myLabel.TextConfig.Font = AGSGameSettings.DefaultSpeechFont; myLabel.TextConfig.Brush = game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.Red);` |
| Valid | Check if the object is contained in room (or global GUI list) | `if (myOverlay.Valid) {}` | Checking in a specific room: `if (room.Objects.Contains(myObj) {}`, checking in global GUI list: `if (game.State.UI.Contains(myObj)) {}` |
| X | X | `myOverlay.X = 5;` | `myObj.X = 5;` |
| Y | Y | `myOverlay.Y = 5;` | `myObj.Y = 5;` |

Missing in AGS but exists in MonoAGS: no limits to the number of presented overlays, overlays are just objects so can be treated exactly the same as objects (like rotating/scaling), see the Object section for more details.

## Palette

Currently there are no equivalents to the palette in MonoAGS (if you want to implement something like this, it would probably require writing a custom shader).

## Parser

Currently there are no equivalents to the parser in MonoAGS, you'll have to program parsing logic yourself (but you can use a textbox and subscribe to key pressed events on the textbox).

## Region

Currently there are no equivalents to regions in MonoAGS.

## Room

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| AreThingsOverlapping | CollidesWith | `if (AreThingsOverlapping(1002, EGO)) {}` | `if (cEgo.CollidesWith(oBullet.X, oBullet.Y, game.State.Viewport)) {}` | Note that in MonoAGS, if the colliding objects have the pixel-perfect component enabled, the collision checks will be accurate, and not using a bounding box like in AGS.
| DisableGroundLevelAreas | manually disable areas & edges | `DisableGroundLevelAreas(0);` | `foreach (var area in myRoom.Areas) { area.Enabled = false; } foreach (var edge in myRoom.Edges) { edge.Enabled = false; } ` |
| EnableGroundLevelAreas | manually enable areas & edges | `EnableGroundLevelAreas();` | `foreach (var area in myRoom.Areas) { area.Enabled = true; } foreach (var edge in myRoom.Edges) { edge.Enabled = true; } ` |
| GetBackgroundFrame | Background.Animation.State.CurrentFrame | `if (GetBackgroundFrame()==4) {}` | `if (state.Room.Background.Animation.State.CurrentFrame == 4) {}` |
| GetDrawingSurfaceForBackground | ? | `DrawingSurface *surface = Room.GetDrawingSurfaceForBackground();` | ? | No built-in way, but you can implement a custom renderer for the background and draw whatever you want.
| GetRoomProperty | Properties.Bools/Ints/etc | `if (GetRoomProperty("CanBeAttackedHere")) {}` | `if (myRoom.Properties.Bools.GetValue("CanBeAttackedHere")) {}` |
| GetTextProperty | Properties.Strings | `Room.GetTextProperty("Description");` | `myRoom.Properties.Strings.GetValue("Description");` |
| GetScalingAt | GetMatchingAreas and then calculate it yourself | `if (GetScalingAt(player.x, player.y) == 100) {}` | `float getAreaScalingWidth(IRoom room, IObject obj) { foreach (IArea are in room.GetMatchingAreas(obj.Location.XY, obj.ID)) { IScalingArea scaleArea = area.GetComponent<IScalingArea>(); is (scaleArea == null || !scaleArea.ScaleObjectsX) continue; return scaleArea.GetScaling(scaleArea.Axis == ScalingAxis.X ? obj.Location.X : obj.Location.Y).Width; }} ... if (getAreaScalingWidth(myRoom, myObj) == 1f) {}` | Some notes on the MonoAGS example: note that it's not enough to just pass x,y to get the scaling, we also need to pass the actual object- that's because in MonoAGS areas might be configured to include/exclude specific objects. Also note that the horizontal and vertical scaling are not necessarily the same, that the scaling area has an axis which need to be accounted for (in AGS the axis is always vertical) and the value returned is the factor in which scaling is multiplied (so 100 scaling in AGS is 1 scaling in MonoAGS), and unlike AGS there are no limits to the scaling.
| GetViewportX | viewport.X | `if (GetViewportX() > 100) {}` | `if (state.Viewport.X > 100) {}` | Note that in MonoAGS you can have multiple viewports, which you can access using the "SecondaryViewports" property: `if (state.SecondaryViewports[0].X > 100) {}`
| GetViewportY | viewport.Y | ``if (GetViewportY() > 100) {}` | `if (state.Viewport.Y > 100) {}` | Note that in MonoAGS you can have multiple viewports, which you can access using the "SecondaryViewports" property: `if (state.SecondaryViewports[0].Y > 100) {}`
| GetWalkableAreaAt | GetMatchingAreas and then calculate it yourself | `if (GetWalkableAreaAt(mouse.x,mouse.y) == 0) {}` | `private IArea getWalkableAreaAt(IRoom room, IObject obj) { return room.GetMatchingAreas(obj.Location.XY, obj.ID).FirstOrDefault(area => area.GetComponent<IWalkableArea>()?.IsWalkable ?? false); } ... if (getWalkableAreaAt(myRoom, myObj) == null) {}` | Note that it's not enough to just pass x,y to get the walkable area, we also need to pass the actual object- that's because in MonoAGS areas might be configured to include/exclude specific objects.
| HasPlayerBeenInRoom | ? | `if (HasPlayerBeenInRoom(14)) {}` | ? | Note that while this is not built-in, it can be programmed easily if needed: when entering the room you're interested in tracking, you can run `Repeat.Do("playerInMySpecialRoom");`,  and then, for testing "HasPlayerBeenInRoom", you can run `if (Repeat.Current("playerInMySpecialRoom") >= 1) {}`
| ReleaseViewport | viewport.Camera.Enabled | `ReleaseViewport();` | `state.Viewport.Camera.Enabled = true;` | Note that in MonoAGS you can have multiple viewports, which you can access using the "SecondaryViewports" property: `state.SecondaryViewports[0].Camera.Enabled = true;`
| RemoveWalkableArea | area.Enabled | `RemoveWalkableArea(5);` | `myArea.Enabled = false;` | Note, that unlike AGS, the change is permanent, it does not reset when you switch rooms
| ResetRoom | ? | `ResetRoom(0);` | ? |
| RestoreWalkableArea | area.Enabled | `RestoreWalkableArea(5);` | `myArea.Enabled = true;` |
| SetAreaScaling | scalingArea.MinScaling/MaxScaling | `SetAreaScaling(5, 120, 170);` | `var scalingArea = area.GetComponent<IScalingArea>(); scalingArea.MinScaling = 1.2f; scalingArea.MaxScaling = 1.7f;` | Note that in MonoAGS the scaling value is the factor in which scaling is multiplied (so 100 scaling in AGS is 1 scaling in MonoAGS), and unlike AGS there are no limits to the scaling.
| SetBackgroundFrame | Background.Image | `SetBackgroundFrame(4);` and to get back to the animation: `SetBackgroundFrame(-1);` | `myRoom.Background.Image = myRoom.Background.Animation.Frames[4].Sprite.Image` and to get back to the animation `myRoom.Background.StartAnimation(myRoomAnimation);` | Note that there is no "4 animation frames" limit in MonoAGS like there is in AGS
| SetViewport | Disable the camera and then set viewport x/y | `SetViewport(100, 100);` | `state.Viewport.Camera.Enabled = false; state.Viewport.X = 100; state.Viewport.Y = 100;` |
| SetWalkBehindBase | Baseline | `SetWalkBehindBase (3,0);` | `var walkbehind = area.GetComponent<IWalkBehindArea>(); walkbehind.Baseline = 0;` |
| BottomEdge | Edges.Bottom.Value | `Room.BottomEdge` | `myRoom.Edges.Bottom.Value` |
| ColorDepth | ? | `Room.ColorDepth` | ? | This is probably not needed in MonoAGS, mix & match images with different color depths should work.
| Height | Limits.Height | `Room.Height` | `myRoom.Limits.Height` |
| LeftEdge | Edges.Left.Value | `Room.LeftEdge` | `myRoom.Edges.Left.Value` |
| Messages | Properties.Strings | `String description = Room.Messages[1];` | `string dsescription = myRoom.Properties.Strings.GetValue("MyRoomDescription");`
| MusicOnLoad | MusicOnLoad | `Room.MusicOnLoad` | `myRoom.MusicOnLoad` |
| ObjectCount | Objects.Count | `Room.ObjectCount` | `myRoom.Objects.Count` | Note that in MonoAGS this includes the characters in the room (as characters are also objects)
| RightEdge | Edges.Right.Value | `Room.RightEdge` | `myRoom.Edges.Right.Value` |
| TopEdge | Edges.Top.Value | `Room.TopEdge` | `myRoom.Edges.Top.Value` |
| Width | Limits.Width | `Room.Width` | `myRoom.Limits.Width` |

Missing in AGS but exists in MonoAGS: no limits on the scaling areas, no limits on the number of animation frames for the background, multiple viewports, restriction lists to include/exclude specific entities from specific areas, separate horizontal/vertical scaling for scaling areas, set the scaling axis for scaling areas, can set custom properties at run-time, can create rooms at run-time, can subscribe/unsubcribe events at run-time, set custom limits for the room (including "endless" rooms), enable/disable specific areas & edges, volume scaling areas, camera zoom areas, zoom/scale the viewport.

## Screen

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| FadeIn | ? | `FadeIn(10);` | ? |
| FadeOut | ? | `FadeOut(30);` | ? |
| FlipScreen | ? | `FlipScreen(1);` | ? |
| SetFadeColor | ? | `SetFadeColor(200, 0, 0);` | ? |
| SetNextScreenTransition | state.RoomTransitions.SetOneTimeNextTransition | `SetNextScreenTransition(eTransitionBoxout);` | `state.RoomTransitions.SetOneTimeNextTransition(AGSRoomTransitions.BoxOut());` | Note that you can give more options to the room transition in MonoAGS, for example: `AGSRoomTransitions.BoxOut(timeInSeconds: 3, easeBoxOut: Ease.CubeIn, easeBoxIn: Ease.CubeOut)`
| SetScreenTransition | state.RoomTransitions.Transition | `SetScreenTransition(eTransitionFade);` | `state.RoomTransitions.Transition = AGSRoomTransitions.Fade();` | Note that you can give more options to the room transition in MonoAGS, for example: `AGSRoomTransitions.Fade(timeInSeconds: 3.5f, easeFadeOut: Ease.CubeIn, easeFadeIn: Ease.CubeOut)`
| ShakeScreen | ShakeEffect | `ShakeScreen(5);` | `ShakeEffect effect = new ShakeEffect (); await effect.RunAsync(TimeSpan.FromSeconds(5));` |
| ShakeScreenBackground | ShakeEffect | `ShakeScreenBackground (4, 10, 80);` | `ShakeEffect effect = new ShakeEffect(strength: 0.1f, decay: 0.9f); effect.RunAsync(TimeSpan.FromSeconds(5));` |
| TintScreen | ? | `TintScreen (100, 50, 50);` | ? |

Missing in AGS but exists in MonoAGS: Customize time and easing for built in room transitions, code your own custom room transitions, shake can be applied to individual objects (not just the screen), custom shaders to generate all sorts of effects.

## String

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Append | + | `mytext = mytext.Append("World");` | `mytext = mytext + "World"`, or `mytext += "World"`, or `mytext = $"{mytext}World"` |
| AppendChar | + | `mytext = mytext.AppendChar('o');` | `mytext = mytext + 'o'`, or `mytext += 'o'`, or `mytext = $"{mytext}o"` |
| CompareTo | for direct equality, you can use "==", otherwise, CompareTo | `if (mytext.CompareTo("hello") == 0) {}` | `if (mytext == "hello") {}` or `if (mytext.CompareTo("hello") == 0) {}` | Note, in c# both CompareTo and == are case sensitive, as opposed to AGS. If you want case insensitive equality check, you can do `if (string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase)) {}`
| Copy | Copy | `String newstring = mystring.Copy();` | `String newstring = mystring.Copy();` |
| EndsWith | EndsWith | `if (myString.EndsWith("script!")) {}` | `if (myString.EndsWith("script!", false)) {}` | In c# EndsWith is case sensitive by default, as opposed to AGS which is case insensitive by default (that's the reason for the added false parameter in the c# example, to match the AGS sample).
| Format | Format, or string interpolation | `String text = String.Format("%d, %d", health, score);` | `string text = string.Format("{0}, {1}", health, score);` or `string text = $"{health}, {score}";` |
| IndexOf | IndexOf | `int result = haystack.IndexOf("a needle");` | `int result = haystack.IndexOf("a needle");` | In c# IndexOf is case sensitive by default, if you want case insensitive, you can do: `int result = haystack.IndexOf("a needle", StringComparison.InvariantCultureIgnoreCase);`
| IsNullOrEmpty | IsNullOrEmpty | `if (String.IsNullOrEmpty(myString)) {}` | `if (string.IsNullOrEmpty(myString)) {}` |
| LowerCase | ToLower | `String lowercased = mystring.LowerCase();` | `string lowercased = mystring.ToLower();` |
| Replace | Replace | `String changed = original.Replace("hello", "goodbye");` | `string changed = original.Replace("hello", "goodbye");` | In c# Replace is case sensitive, to do a case insensitive replace you can use a regular expression: `string changed = Regex.Replace(original, "hello", "goodbye");`
| ReplaceCharAt | ToCharArray + index | `String changed = mystring.ReplaceCharAt(2, 'm');` | `char[] ch = mystring.ToCharArray(); ch[2] = 'm'; string changed = new string(ch);` |
| StartsWith | StartsWith |  `if (myString.StartsWith("script!")) {}` | `if (myString.StartsWith("script!", false)) {}` | In c# StartsWith is case sensitive by default, as opposed to AGS which is case insensitive by default (that's the reason for the added false parameter in the c# example, to match the AGS sample).
| Substring | Substring | `String substring = mystring.Substring(3, 5);` | `String substring = mystring.Substring(3, 5);` |
| Truncate | Substring | `String truncated = mystring.Truncate(4);` | `string truncated = mystring.Length <= 4 ? mystring : mystring.Substring(0, 4);` |
| UpperCase | ToUpper | `String uppercased = mystring.UpperCase();` | `string uppercased = mystring.ToUpper();` |
| AsFloat | float.TryParse | `float number1; number1 = text1.AsFloat;` | `float.TryParse(text1, out float number1);` |
| AsInt | int.TryParse | `int number1; number1 = text1.AsInt;` | `int.TryParse(text1, out int number1);` |
| Chars | [] | `text.Chars[3]` | `text[3]` |
| Length | Length | `text.Length` | `text.Length` |

Missing in AGS but exists in MonoAGS: Everything here is not MonoAGS specific but c#: you can see c# string reference here: https://msdn.microsoft.com/en-us/library/system.string(v=vs.110).aspx

## System

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| AudioChannelCount | ? | `System.AudioChannelCount` | ? |
| AudioChannels | ? | `AudioChannel *channel = System.AudioChannels[2];` | ? |
| CapsLock | Device.KeyboardState.CapslockOn | `if (System.CapsLock) {}` | `if (AGSGame.Device.KeyboardState.CapslockOn) {}` |
| ColorDepth | ? | `System.ColorDepth` | ? |
| Gamma | ? | `System.Gamma` | ? |
| HardwareAcceleration | Device.GrahpicsBackend.AreShadersSupported | `if (System.HardwareAcceleration) {}` | `if (AGSGame.Device.GraphicsBackend.AreShadersSupported()) {}` |
| NumLock | ? | `if (System.NunLock) {}` | ? |
| OperatingSystem | ? | `if (System.OperatingSystem == eOSWindows) {}` | ? |
| ScreenHeight | Settings.WindowSize.Height | `System.ScreenHeight` | `game.Settings.WindowSize.Height` |
| ScreenWidth | Settings.WindowSize.Width | `System.ScreenWidth` | `game.Settings.WindowSize.Width` |
| ScrollLock | ? | `if (System.ScrollLock) {}` | ? |
| SupportsGammaControl | ? | `System.SupportsGammaControl` | ? |
| Version| ? | `System.Version` | ? |
| ViewportHeight | ? | `System.ViewportHeight` | ? |
| ViewportWidth | ? | `System.ViewportWidth` | ? |
| Volume | AudioSettings.MasterVolume | `System.Volume = 80;` | `game.AudioSettings.MasterVolume = 0.8f;` | Range in AGS is 0-100, range in MonoAGS is 0-1
| VSync | Settings.Vsync | `System.Vsync = true;` | `game.Settings.Vsync = VsyncMode.On;` |
| Windowed | Settings.WindowState | `if (Settings.Windowed) {}` | `if (game.Settings.WindowState == WindowState.Normal) {}` |

Missing in AGS but exists in MonoAGS: Change window size at runtime (including windowed or not, and bordered or not), adaptive vsync.

## Text Display / Speech

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Display | AGSMessageBox.DisplayAsync | `Display("Hello");` | `await AGSMessageBox.DisplayAsync("Hello");` |
| DisplayAt | ? | `DisplayAt (50,50,100, "This is a message");` | ? |
| DisplayAtY | ? | `DisplayAt (50, "This is a message");` | ? |
| DisplayMessage | AGSMessageBox.DisplayAsync | `DisplayMessage(220);` | `await AGSMessageBox.DisplayAsync(game.Properties.Strings.GetValue("MySpecialMessage"));` |
| DisplayMessageAtY | ? | `DisplayMessageAtY(527, 200);` | ? |
| DisplayTopBar | ? | `DisplayTopBar(25, 8, 7, "Evil wizard", "Get out of my house and never return!");` | ? |
| SetSkipSpeech | SpeechConfig.SkipText | `SetSkipSpeech(2);` | `player.SpeechConfig.SkipText = SkipText.ByTime;` | Note that the example given in MonoAGS is for setting the skip style for a specific character, not a global change like in AGS.
| SetSpeechStyle | customize to your liking | `SetSpeechStyle(eSpeechSierra);` | N/A | For the "lucas arts" style (text over character's head), do nothing, that's the default. For the "sierra" style, set a portrait in the charater's speech config: `cGraham.SpeechConfig.PortraitConfig.Portrait = grahamPortrait;`. For the "sierra with background" style, set a portrait as before, and also set BackgroundColor in the character's speech config: `cGraham.SpeechConfig.BackgroundColor = Colors.Pink;`. For "QFG4-style full screen", set the portrait object to be a full-screen size object, and for customizing where the speech text and portrait will be located, you need to provide your custom implementation for `ISayLocationProvider` and then replace the default implementation. For example, this custom implementation will place both text and portrait in the bottom-left of the screen: `public class Qfg4StyleSayLocationProvider : ISayLocationProvider { public ISayLocation GetLocation(string text, ISayConfig config) { var textLocation = new PointF(0f, 0f); var portraitLocation = new PointF(0f, 0f); return new AGSSayLocation(textLocation, portraitLocation); }}`, then to use this implementation instead of the default engine's implementation, write (before the game loads): `Resolver.Override(resolver => resolver.Builder.RegisterType<Qfg4StyleSayLocationProvider>().As<ISayLocationProvider>());`.

Missing in AGS but exists in MonoAGS: Setting "skip text" styles for specific characters, show yes/no or ok/cancel dialogs (or any dialog with a custom number of buttons), more customizations to speech style, including creating custom say functions completely.

## ViewFrame

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Flipped | ? | if (frame.Flipped) {} | ? | Note that you can currently flip a frame horizontally or vertically by doing `frame.Sprite.FlipHorizontally()`, but currently no way to query if a frame is flipped (other, perhaps, then checking if the scaling of the sprite is negative)
| Frame | Frames.IndexOf | `frame.Frame` | `animation.Frames.IndexOf(frame)` |
| Graphic | Sprite.Image.ID | `frame.Graphic` | `frame.Sprite.Image.ID` |
| LinkedAudio | SoundEmitter.AudioClip | `frame.LinkedAudio` | `frame.SoundEmitter.AudioClip` |
| Loop | N/A | `frame.Loop` | N/A | MonoAGS does not have the concept of loops in a view, so getting the loop of a frame is meaningless. You can check whether a frame is part of a particular animations by doing: `animation.Frames.Contains(frame)`.
| Speed | Delay | `frame.Speed` | `frame.Delay` |
| View | N/A | `frame.View` | N/A | MonoAGS does not have the concept of a view. The most equivalent for a view is a directional animation. For a specific directional animation, you can check if the frame exists for one of its animations by doing: `if (myDirectionalAnimation.GetAllDirections().Any(animation => animation.Frames.Contains(frame)) {}`

Missing in AGS but exists in MonoAGS: rotate/scale/translate individual animation frames (including at run-time), change the animation frame speed at run-time and allow for random animation frame delays within a range (very useful for speak animations), for the linked sound, allows to configure whether to automatically pan and adjust the volume of the sound based on the location of the animation relative to the center of the screen (for panning) and based on the location within a volume changing area.

## SCUMM_VERBCOIN_GUI

Currently there are no equivalents to verb coin GUI in MonoAGS.

## Game variables

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| abort_key | ? | `game.abort_key = 324` | ? |
| ambient_sounds_persist | ? | `game.ambient_sounds_persist = 0` | ? |
| anim_background_speed | room.Background.Animation.Configuration.DelayBetweenFrames | `game.anim_background_speed = 5;` | `game.State.Room.Background.Animation.Configuration.DelayBetweenFrames = 5;` |
| auto_use_walkto_points | ApproachStyle | `game.auto_use_walkto_points = 0;` | `player.ApproachStyle.ApproachWhenVerb[Verbs.Look] = ApproachHotspots.NeverWalk;` |
| bgspeech_game_speed | ? | `game.bgspeech_game_speed = 0;` | ? |
| bgspeech_stay_on_display | ? | `game.bgspeech_stay_on_display = 0;` | ? |
| close_mouth_end_speech_time | ? | `game.close_mouth_end_speech_time = 10;` | ? |
| debug_mode | `#if DEBUG` | `if (game.debug_mode == 1) {}` | `#if DEBUG ... #endif` |
| dialog_options_x | ? | `game.dialog_options_x = 4;` | ? |
| dialog_options_y | ? | `game.dialog_options_y = 4;` | ? |
| disable_antialiasing | you can select texture filter scaling per image | `game.disable_antialiasing = 1;` | `image.Texture.Config = new AGSTextureConfig(scaleUp: ScaleUpFilters.Nearest);` |
| following_room_timer | ? | `game.following_room_timer = 150;` | ? |
| keep_screen_during_instant_transition | ? | `game.keep_screen_during_instant_transition = 1;` | ? |
| inv_activated | Inventory.ActiveItem | `game.inv_activated` | `player.Inventory.ActiveItem` |
| inventory_greys_out | ? | `game.inventory_greys_out = 1;` | ? |
| lipsync_speed | ? | `game.lipsync_speed = 15;` | ?
| max_dialogoption_width | ? | `game.max_dialogoption_width = 180;` | ? |
| min_dialogoption_width | ? | `game.min_dialogoption_width = 0;` | ? |
| narrator_speech | ? | `game.narrator_speech = 5;` | ? |
| no_textbg_when_voice | ? | `game.no_textbg_when_voice = 0;` | ? |
| read_dialog_option_color | ? | `game.read_dialog_option_color = 5;` | ? |
| roomscript_finished | N/A | `game.roomscript_finished` | N/A | There's no need for `on_call` as there's no problem just executing functions between scripts
| score | ? | `game.score` | ? | There's no problem mimicking a score with a simple global variable
| score_sound | ? | `game.score_sound` | ? | There's no problem mimicking a score sound with a simple function that changes a global variable and plays a sound.
| screenshot_height | ? | `game.screenshot_height = 200;` | ? |
| screenshot_width | ? | `game.screenshot_width = 320;` | ? |
| show_single_dialog_option | ? | `game.show_single_dialog_option = 1;` | ? |
| sierra_inv_color | set an empty image with a color on the inventory window | `game.sierra_inv_color = 5;` | `invWindow.Image = new EmptyImage(300, 200); invWindow.Tint = Colors.Pink;` |
| skip_display | ? | `game.skip_display = 3;` | ? |
| skip_speech_specific_key | Implement custom text skipping | `game.skip_speech_specific_key = 5;` | `player.SpeechConfig.SkipText = SkipText.External;  player.OnBeforeSay.Subscribe(args => { input.KeyDown.Subscribe(keyArgs => if (keyArgs.Key == Key.PageDown) args.Skip();) });`
| speech_bubble_width | ? | `game.speech_bubble_width = 100;` | ? |
| speech_text_align | SpeechConfig.TextConfig.Alignment | `game.speech_text_align = eAlignCentre;` | `player.SpeechConfig.TextConfig.Alignment = Alignment.MiddleCenter;` |
| speech_text_gui | Either set properties in `SpeechConfig` or subscribe to `OnBeforeSay` and manipulate the label | `game.speech_text_gui = 4;` | For basic changes: `player.SpeechConfig.BackgroundColor = Colors.Pink; player.SpeechConfig.Border = AGSBorders.Solid(Colors.Red, lineWidth: 5);`, for complete control: `player.OnBeforeSay.Subscribe(args => { args.Label.Opacity = 0; args.Label.TweenOpacity(255, 3f);});` |
| text_align | SpeechConfig.TextConfig.Alignment | `game.speech_text_align = eAlignCentre;` | `AGSMessageBox.Config.TextConfig.Alignment = Alignment.MiddleCenter;` |
| text_shadow_color | SpeechConfig.TextConfig.ShadowBrush | `game.text_shadow_color = 16;` | `player.SpeechConfig.TextConfig.ShadowBrush = Brushes.SolidBlue;` |
| top_bar_XXXX | ? | `game.top_bar_bordercolor = 5;` | ? |
| total_score | ? | `game.total_score = 50;` | ? | There's no problem mimicking a score sound with a simple function that changes a global variable and plays a sound.
| used_mode | N/A | `game.used_mode` | N/A | The "used mode" is dependent on your control scheme. For a "rotating cursors" scheme, you can get `scheme.CurrentMode`
| mouse.x | input.MousePosition.XMainViewport | `mouse.x` | `game.Input.MousePosition.XMainViewport` |
| mouse.y | input.MousePosition.YMainViewport | `mouse.y` | `game.Input.MousePosition.YMainViewport` |
| palette[SLOT] | ? | `palette[0].r` | ? |
| player | state.Player | `player` | `game.State.Player` |

## Predefined global script functions

The predefined global script function in AGS are events. As there is no way of runtime event subscription in AGS, there are "magic" predefined functions in the script. In MonoAGS you can subscribe/unsubscribe events at will.

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| dialog_request | ? | `function dialog_request(int parameter) {}` | ? |
| game_start | game.Events.OnLoad | `function game_start() {}` | `game.Events.OnLoad.Subscribe(onGameStart); ... void onGameStart() {}` |
| on_event:eEventEnterRoomBeforeFadein | room.Events.OnBeforeFadeIn for specific room, or game.Events.OnRoomChanging | `function on_event (EventType event, int data) { if (event == eEventEnterRoomBeforeFadein) { int room = data; } }` | `game.Events.OnRoomChanging.Subscribe(onRoomChanging); ... void onRoomChanging() { IRoom room = game.State.Room; }` |
| on_event:eEventLeaveRoom | room.Events.OnAfterFadeOut for specific room, or game.Events.OnRoomChanging | `function on_event (EventType event, int data) { if (event == eEventLeaveRoom) { int room = data; } }` | `game.Events.OnRoomChanging.Subscribe(onRoomChanging); ... void onRoomChanging() { IRoom room = player.PreviousRoom; }` | Missing `state.PreviousRoom` so `OnRoomChanging` for getting the previous room for the player will not work if that room is changed without moving the player to that room.
| on_event:eEventGotScore | ? | `function on_event (EventType event, int data) { if (event == eEventGotScore) {} }` | ? |
| on_event:eEventGUIMouseDown | gui.MouseDown for specific gui, or subscribe to input.MouseDown and use hitTest for GUI checks | `function on_event (EventType event, int data) { if (event == eEventGUIMouseDown) { int gui = data; } }` | `game.Input.MouseDown.Subscribe(onMouseDown); ... void onMouseDown(MouseButtonEventArgs args) { var clickedObject = hitTest.ObjectAtMousePosition; }` |
| on_event:eEventGUIMouseUp | gui.MouseUp for specific gui, or subscribe to input.MouseUp and use hitTest for GUI checks | `function on_event (EventType event, int data) { if (event == eEventGUIMouseUp) { int gui = data; } }` | `game.Input.MouseUp.Subscribe(onMouseUp); ... void onMouseUp(MouseButtonEventArgs args) { var clickedObject = hitTest.ObjectAtMousePosition; }` |
| on_event:eEventAddInventory | ? | `function on_event (EventType event, int data) { if (event == eEventAddInventory) {} }` | ? |
| on_event:eEventLoseInventory | ? | `function on_event (EventType event, int data) { if (event == eEventLoseInventory) {} }` | ? |
| on_event:eEventRestoreGame | ? | `function on_event (EventType event, int data) { if (event == eEventRestoreGame) {} }` | ? |
| on_key_press | input.KeyDown | `function on_key_press (eKeyCode keycode) {}` | `input.KeyDown.Subscribe(onKeyDown); ... void onKeyDown(KeyboardEventArgs args) {}`
| on_mouse_click | input.MouseDown | `function on_mouse_click (MouseButton button) {}` | `input.MouseDown.Subscribe(onMouseDown); ... void onMouseDown(MouseButtonEventArgs args) {}` |
| repeatedly_execute | Events.OnRepeatedlyExecute | `function repeatedly_execute() {}` | `game.Events.OnRepeatedlyExecute.Subscribe(onRepeatedlyExecute); ... void onRepeatedlyExecute() {}` |
| repeatedly_execute_always | Events.OnRepeatedlyExecuteAlways | `function repeatedly_execute_always() {}` | `game.Events.OnRepeatedlyExecuteAlways.Subscribe(onRepeatedlyExecute); ... void onRepeatedlyExecute() { }` |
| unhandled_event | Events.DefaultInteractions | `function unhandled_event (int what, int type) { if (what == 1 && type == 1) {}}` | `game.Events.DefaultInteractions.OnInteract(Verbs.Look).Subscribe(args => {});` |

Missing in AGS but exists in MonoAGS: Subscribe/unsubscribe to events at runtime, mouse move and key up generic events, mouse enter/leave/up/double click/lost focus per object events

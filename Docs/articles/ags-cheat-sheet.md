# AGS Cheat Sheet

For people coming in from AGS, this is a "cheat sheet", going over the AGS functions and properties and shows how to do the same in `MonoAGS`, or if something is missing, and also explaining some differences between the two.

## AudioChannel

The equivalent in `MonoAGS` would be `ISound`. Both are returned when you're playing an audio clip. The difference between AGS channel and MonoAGS sound is that a sound relates to the specific sound you're playing, it "dies" when you finished playing the sound. The channel however lives on throughout the game and can play other sounds in the future, so you can't always trust it's playing the sound you requested.

| AGS | MonoAGS | AGS Example | MonoAGS Example | Further notes
|-----|---------|-------------|-----------------|-----------------------------------
| Seek | Seek | `channel.Seek(milliseconds);` | `sound.Seek = seconds;` | Milliseconds in AGS, seconds in MonoAGS. In AGS the value is int meaning you can't get a lower resolution than milliseconds. In MonoAGS the value is float meaning you can go as low in resolution as the hardware understands.
| SetRoomLocation | ? | `channel.SetRoomLocation(x,y);` | ? | MonoAGS has the concept of a sound emitter which automatically pans the sound based on the location in the room, and can set the volume based on volume-changing areas, but nothing currently specifically exists for volume based on distance from a character.
| Stop | Stop | `channel.Stop();` | `sound.Stop();` |
| ID | SourceID | `channel.ID` | `sound.SourceID` |
| IsPlaying | HasCompleted | `if (!channel.IsPlaying)` | `if (sound.HasCompleted)` | If you want to check whether the sound you played completed playing, `MonoAGS` provides you with a better option: In AGS, `channel.IsPlaying` might return true even if your sound finished playing, because another sound is now being played on that channel.
| LengthMs | ? | `channel.LengthMs` | ? |
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
| Name | Hotspot | `cEgo.Name = "Bernard";` | `cEgo.Hotspot = "Bernard";` |
| NormalView | Outfit[Animations.Walk] | `cEgo.NormalView` | `cEgo.Outfit[Animations.Walk];` |
| PreviousRoom | PreviousRoom | `if (cEgo.PreviousRoom == 5) {}` | `if (cEgo.PreviousRoom == elevator) {}` | In MonoAGS, `PreviousRoom` actually provides you with access to the entire room's API, not just its ID, so you can query the room's objects, for example.
| Room | Room | `if (cEgo.Room == 5) {}` | `if (cEgo.Room == elevator) {}` | In MonoAGS, `Room` actually provides you with access to the entire room's API, not just its ID, so you can query the room's objects, for example.
| ScaleMoveSpeed | AdjustWalkSpeedToScaleArea | `cEgo.ScaleMoveSpeed = true;` | `cEgo.AdjustWalkSpeedToScaleArea = true;` |
| ScaleVolume | scalingArea.ScaleVolume | `cEgo.ScaleVolume = true;` | `scalingArea.ScaleVolume = true;` | This is not a 1-to-1 match. In AGS, scale volume scales the volume according to the scaling of the character, not matter if the scaling was set manually or in an area. In MonoAGS, this is specfically for areas, there is no equivalent configuration for manual scaling changes currently.
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
| DialogToRender | ? | `info.DialogToRender` | ? | The way to do custom rendering is a bit different in MonoAGS. There's no one single hook to customize the dialogs, but you can choose on which lever you want to provide your own different implementation. So you can provide a different implementation for `IDialogLayout` (which gets the dialog graphics and options graphics and chooses how to place them), or you can provide a different implementation for each (or for specific) `IDialogOption` to change how they are rendered/behave, or you can provide a different implementation for `IDialog` to completely rewrite the dialog mechanism (but still be able to hook it up to existing dialog code). Each of those custom implementations can be either changed for all dialogs or for specific dialogs.
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
| DoOnceOnly | Repeat.Once | `if (Game.DoOnceOnly("open cupboard")) {}` | `if Repeat.Once("open cupboard") {}` |
| EnableInterface | ? | `EnableInterface();` | There's nothing specific currently, but you can disable all GUI controls and change the cursor.
| EndCutscene | Cutscene.End | `EndCutscene();` | `state.Cutscene.End();` |
| GetColorFromRGB | Color.FromRgba | `Game.GetColorFromRGB(0, 255, 0);` | `Color.FromRgba(0, 255, 0, 255);` |
| GetFrameCountForLoop | animation.Frames.Count | `Game.GetFrameCountForLoop(SWIMMING, 2);` | `cEgo.Outfit[Animations.Swim].Left.Frames.Count` |
| GetGameOption | ? | `GetGameOption(OPT_WALKONLOOK)` | ? | There's a lot of unrelated very specific configurations for AGS here, some of them have equivalents in MonoAGS, see `SetGameOption` for more details.
| GetGameSpeed | `state.Speed` | `GetGameSpeed();` | `game.State.Speed` |
| GetLocationName | `hitTest.ObjectAtMousePosition` | `if (GetLocationName(mouse.x, mouse.y) == "Hero") {}` | `if (hitTest.ObjectAtMousePosition.Hotspot == "Hero") {}` | Currently, there's no support for getting at specific (x,y) position, but just for where the mouse is at.
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
| Name | Hotspot | `hDoor.Name` | `hDoor.Hotspot` |
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
| Name | Graphics.Hotspot | `iKey.Name` | `iKey.Graphics.Hotspot` |

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
| SelectNextMode | ? | `Mouse.SelectNextMode()` | In MonoAGS, by choosing the `RotatingCursorsScheme` as your control scheme, this is already handled (and you can look at the code if you want to it differently)
| SetBounds | Subscribe to mouse move event and change the position of the mouse accordingly | `mouse.SetBounds(160, 100, 320, 200);` | `game.Input.MouseMove.Subscribe(args => if (args.MousePosition.XWindow > 160) OpenTK.Mouse.SetPosition(160, args.MousePosition.YWindow));` |
| SetPosition | OpenTK.Mouse.SetPosition | `mouse.SetPosition(160, 100);` | `OpenTK.Mouse.SetPosition(160, 100);` |
| Update | ? | `mouse.Update();` | ? |
| UseDefaultGraphic | ? | `mouse.UseDefaultGraphic();` | ? |
| UseModeGraphic | N/A | `mouse.UseModeGraphic(eModeWait)` | ? | This can be different depending on the control scheme you chose for the game. For the RotatingCursorsScheme for example, you'd write: `scheme.Mode = Verbs.Wait;` |
| Mode | input.Cursor | `if (mouse.Mode == eModeWalkto) {}` | `if (game.Input.Cursor == myWalkCursor) {}` | For individual control schemes, you might have the concept of "Mode", but it's not related to the mouse. In RotatingCursorsScheme, for example, you can query `if (scheme.Mode == Verbs.Walk) {}`.
| Visible | Input.Cursor.Visible | `mouse.Visible = false;` | `game.Input.Cursor.Visible = false;`

Missing in AGS but exists in MonoAGS: The cursors is just an extension of objects, so they can be manipulated in all ways an object can be manipulated (see Object for more).

## Multimedia

Currently no equivalents to any of the multimedia functions in MonoAGS.

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
| Name | Hotspot | `oRope.Name` | `oRope.Hotspot` |
| Solid | ? | `oRope.Solid = true;` | ? |
| Transparency | Opacity | `oRope.Transparency = 100;` | `oRope.Opacity = 0;` | The range for AGS transparency is 0-100, the range for MonoAGS opacity is 0-255
| View | Animation | `oRope.View` | `oRope.Animation` |
| Visible | Visible | `oRope.Visible = true;` | `oRope.Visible = true;` |
| X | X | `oRope.X = 50;` | `oRope.X = 50.5f;` |
| Y | Y | `oRope.Y = 50;` | `oRope.Y = 50.5f;` |

Missing in AGS but exists in MonoAGS: scaling and rotating (with setting a pivot point), the ability to scale/rotate/translate individual animation frames, composition of objects (i.e nesting objects in other objects), mix & match with GUI, move between rooms, different resolution from the game, custom rendering (including shaders), sub-pixel positioning, rendering in multiple viewports, creation at run-time, selecting between pixel perfect or bounding box collision checks, objects are transitive with all other on-screen items (characters, GUIs), cropping objects, surround with borders, set hotspot text at runtime, controlling texture offset & scaling filter (per texture), subscribing to events (on pretty much anything that might change in any of the components), interactions with custom verbs, ability to extend objects with custom components, ability to replace engine implementation of components with your own (i.e implement your own collider component and provide custom collision checks, for example).

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
| GetProperty | Ints.GetValue | `if (cEgo.GetProperty("Value") > 200) {}` | `if (cEgo.Ints.GetValue("Value") > 200) {}` |
| GetTextProperty | Strings.GetValue | `cEgo.GetTextProperty("Description");` | `cEgo.Strings.GetValue("Description");` |
| HasInventory | Inventory.Items.Contains | `if (cEgo.HasInventory(iKnife)) {}` | `if (cEgo.Inventory.Items.Contains(iKnife)) {}` |
| IsCollidingWithChar | CollidesWith | `if (cEgo.IsCollidingWithChar(cGuy) == 1) {}` | `if (cEgo.CollidesWith(cGuy.X, cGuy.Y, state.Viewport)) {}` | Note that MonoAGS supports multiple viewports so we need to pass the viewport in which we'd like to test for collisions.
| IsCollidingWithObject (character) | CollidesWith | `if (cEgo.IsCollidingWithChar(oBottle) == 1) {}` | `if (cEgo.CollidesWith(oBottle.X, oBottle.Y, state.Viewport)) {}` | Note that MonoAGS supports multiple viewports so we need to pass the viewport in which we'd like to test for collisions.
| LockView | Outfit | `cEgo.LockView(12);` | `cEgo.Outfit = swimmingOutfit;` |
| LockViewAligned | ? | `cEgo.LockViewAligned(12, 1, eAlignLeft);` | ? |
| LockViewFrame | To display a still frame, use Image, for actual locking set a different outfit | `cEgo.LockViewFrame(AGHAST, 2, 4)` | `cEgo.Image = cEgo.Outfit["AGHAST"].Left.Frames[4].Sprite.Image;` |
| LockViewOffset | ? | `cEgo.LockViewOffset(12, 1, -1);` | ? | Note that while there's no direct equivalent, you can change offsets for individual animation frames, so you can do that manually (at run-time if you desire), for example: `cEgo.Outfit["Walk"].Left.Frames[0].Sprite.X = 5; //will offset the first left walking animation frame by 5 pixels to the right`
| LoseInventory | Inventory.Items.Remove | `cEgo.LoseInventory(iKnife);` | `cEgo.Inventory.Items.Remove(iKnife);` |
| Move (character) | set the outfit to an outfit without a walk animation | `cEgo.Move(155, 122, eBlock);` | Non-blocking: `cEgo.Outfit = idleOnlyOutfit; cEgo.WalkAsync(155, 122);`, blocking: `cEgo.Outfit = idleOnlyOutfit; await cEgo.WalkAsync(155, 122);` | No support currently for "walk anywhere"
| PlaceOnWalkableArea | PlaceOnWalkableArea | `cEgo.PlaceOnWalkableArea();` | `cEgo.PlaceOnWalkableArea();` |
| RemoveTint | Tint | `cEgo.RemoveTint();` | `cEgo.Tint = Colors.White;` |
| RunInteraction | Interactions.OnInteract(Verb).InvokeAsync | `cEgo.RunInteraction(eModeTalk);` | `cEgo.Interactions.OnInteract("Speak").InvokeAsync();` |
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
| IdleView | Outfit["Idle"] | `cEgo.IdleView` | `cEgo.Outfit["Idle"]` |
| IgnoreLighting | ? | `cEgo.IgnoreLighting = 1;` | ? |
| IgnoreWalkbehinds | ? | `cEgo.IgnoreWalkbehinds = true;` | ? |
| InventoryQuantity | InventoryItem.Qty | `player.InventoryQuantity[iCash.ID]` | `iCash.Qty` |
| Loop | Animation.State.CurrentLoop | `cEgo.Loop` | `cEgo.Animation.State.CurrentLoop` |
| ManualScaling | IgnoreScalingArea | `cEgo.ManualScaling = true;` | `cEgo.IgnoreScalingArea = true;` | This is not a 1-to-1 fit. In MonoAGS you can still set manual scaling to be applied onto the walkable area scaling, even if `IgnoreScalingArea` is false.
| MovementLinkedToAnimation | MovementLinkedToAnimation | `cEgo.MovementLinkedToAnimation = false;` | `cEgo.MovementLinkedToAnimation = false;` |
| Moving | IsWalking | `if (cEgo.IsMoving) {}` | `if (cEgo.IsWalking) {}` |
| Name | Hotspot | `cEgo.Name = "Bernard";` | `cEgo.Hotspot = "Bernard";` |
| NormalView | Outfit["Walk"] | `cEgo.NormalView` | `cEgo.Outfit["Walk"];` |
| PreviousRoom | PreviousRoom | `if (cEgo.PreviousRoom == 5) {}` | `if (cEgo.PreviousRoom == elevator) {}` | In MonoAGS, `PreviousRoom` actually provides you with access to the entire room's API, not just its ID, so you can query the room's objects, for example.
| Room | Room | `if (cEgo.Room == 5) {}` | `if (cEgo.Room == elevator) {}` | In MonoAGS, `Room` actually provides you with access to the entire room's API, not just its ID, so you can query the room's objects, for example.
| ScaleMoveSpeed | AdjustWalkSpeedToScaleArea | `cEgo.ScaleMoveSpeed = true;` | `cEgo.AdjustWalkSpeedToScaleArea = true;` |
| ScaleVolume | scalingArea.ScaleVolume | `cEgo.ScaleVolume = true;` | `scalingArea.ScaleVolume = true;` | This is not a 1-to-1 match. In AGS, scale volume scales the volume according to the scaling of the character, not matter if the scaling was set manually or in an area. In MonoAGS, this is specfically for areas, there is no equivalent configuration for manual scaling changes currently.
| Scaling | ScaleX and ScaleY | `cEgo.ManualScaling = true; cEgo.Scaling = 200;` | `cEgo.ScaleX = 2; cEgo.ScaleY = 2;` | In AGS the range is 5 to 200, where the value must be an integer and 100 is not scaled. In MonoAGS there's no "allowed" range, the value is a float (so you can do `cEgo.ScaleX = 0.5f`) and 1 is not scaled.
| Solid | ? | `if (cEgo.Solid) {}` | ? |
| Speaking | Outfit["Speak"].Animation.State.IsPaused | `if (cEgo.Speaking) {}` | `if (!cEgo.Outfit["Speak"].Animation.State.IsPaused) {}` |
| SpeakingFrame | Outfit["Speak"].Animation.State.CurrentFrame | `cEgo.SpeakingFrame` | `cEgo.Outfit["Speak"].Animation.State.CurrentFrame` |
| SpeechAnimationDelay | Outfit["Speak"].Animation.Configuration.DelayBetweenFrames | `cEgo.SpeechAnimationDelay` | `cEgo.Outfit["Speak"].Animation.Configuration.DelayBetweenFrames` |
| SpeechColor | SpeechConfig.TextConfig.Brush | `cEgo.SpeechColor = 14;` | `cEgo.SpeechConfig.TextConfig.Brush = blueSolidBrush;` |
| SpeechView | Outfit["Speak"] | `cEgo.SpeechView` | `cEgo.Outfit["Speak"]` |
| ThinkView | Outfit["Think"] | `cEgo.ThinkView` | `cEgo.Outfit["Think"]` | There's nothing particular about `Think` in MonoAGS currently, but using outfit you can assign and query specific animations, so you can create a "think" animation if it fits your game.
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


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
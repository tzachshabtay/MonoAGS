# Audio

Playing music/sound effects is essential to any game.
`MonoAGS` allows you to play and control audio using audio clips and sounds.

## Audio Clips

An audio clip is loaded from a resource/file and can be played multiple times during the game.
The audio clip should be loaded using the audio factory.
You can then set its properties (volume, pitch, panning) which will be used for all sounds played
from this clip.
You can think of an audio clip as a "template" for a sound. You set the properties once (like the volume), and then every time you play it it will use that volume.

When you tell the audio clip to play, you can use the template or provide a one-time property change. You can also tell the audio clip to play a looping sound (which will loop forever until you stop it).

You can query an audio clip to see how many sounds for this clip (if any) are currently playing.

## Sounds

When you play an audio clip, you'll get back a sound. A sound is an "instance" of the audio clip which is playing right now. The same properties that you can set for an audio clip (volume, pitch, panning) can also be set for a sound, only those will affect just this current sound, not future sounds that will be played from the clip, and also you can change those properties over and over as the sound running to achieve effect (like gradually reducing the volume for a fade-out effect, for example).

You can also pause/stop/resume a sound, rewind a sound to the beginning or set the sound to a specific position within the file (by changing the "seek").
You can query and see whether the sound is looping or not and whether it's valid or not (an invalid sound might result if something is wrong with the user's computer and the engine was not able to play the sound).

Lastly, you can query the sound to see if it was already completed, and asynchronously wait for the sound to complete playing.

## Additional Concepts

Some additional concepts related to audio are discussed in other sections, but will be briefly mentioned here:

### Speech

Allows you to configure speech sounds to be automatically played when the character speaks.
See [character's speech section](characters.md#playing_the_audio_clip).

### Cross-Fading

Allows you to configure how music clips are cross-faded when moving between rooms.
See [cross-fading section in rooms](rooms.md#music_clip_to_play).

### Sound Emitter

Allows you to attach an audio clip to an object, and play a sound which will sound like it's coming from that object (via adjusting the panning and volume), and also optionally attach it to animation frames which will automatically trigger the sound (very useful for footsteps effects).
See [animation's sound emitter section](animations.md#sound_emitter), and also [scaling volume areas section](areas.md#scaling_areas).
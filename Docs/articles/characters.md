# Characters

Characters are both the player(s) and the NPCs (non player characters) that walk around on your screen (or just stand there, like many NPCs in adventure games) and interact with one another.

## The relationship between characters and objects

Note that unlike "classic" AGS, characters are objects with more stuff added to them. I.E all characters are also objects, but not all objects are characters.
This means that you can expect all properties and abilities that objects have to also belong the characters, and you can expect the syntax to be the same.
Therefore we will not cover all the stuff that the characters can do here, but just what's added on top of being an object (and you can refer to the [Objects](objects.md) section for the rest).

## Character abilities

### Walking

Allows your characters to walk around the screen. There are several things you can configure for your characters walk, so there's a dedicated section for walk configurations and abilities.
Please refer to the [Walking](walking.md) section for more details.

### Facing directions

Allows your characters to face directions: either to face a specific point on screen, or face to a specific object in the room, or simply to face left, up, up-left, etc.
This would change the current animation to use the most suited animation for the current direction (so, for example, if you face left but only has an "up", "left" or "down" animations, 
either "up" or "down" will be used).

### Following

Allows commanding a character to follow another character/object in the room. 
You can configure how the character performs a follow, to be able to accommodate different scenario (a companion, a spy or an enemy, for example).
You can configure with the time range the character will wait before commencing a new walk towards the target, what's the distance range you'd want to have your character away from the target, 
what's the probability of having your character wander off somewhere else, and whether or not the character will move between rooms if the target moves to another room.

### Approaching

Allows a character to approach an object/character. This is by default, only exists for the player character.
When an interaction event is triggered for a hotspot, if this component is available for the player (which it is by default),
then the character will approach the hotspot before the interaction logic code is triggered.
You can configure what "approach" means to you per each different interaction verb ("look", "interact with", "talk to", etc depending on which verbs you have in your game).
You can set "approach" to do nothing (`NeverWalk`), face the direction of the target without walking (`FaceOnly`), walk to the target if the target has a walk point configured (`WalkIfHaveWalkPoint`),
or always walk towards the target (`AlwaysWalk`).

### Speaking

Allows a character to speak. You'll use either `Say` or `SayAsync` for the character to speak which basically does the following:
1. Puts text on the screen for some time
2. Change the character animation to the speaking animation
3. Optionally display a portrait of the character
4. Optionally play an audio clip

#### Displaying text

You can configure how the text is rendered (color, font, etc: see [Labels](labels.md) section for more on the all the options that you have for text rendering), when the text is skipped
(by time and how much, by mouse, both, or none which means you implement text skipping yourself), a label to host the text with a border (see the section about borders) and background color,
and a text offset to offset the location of the text. That offset will be relative to the default text display location which will be selected by the engine: The engine, by default, tries to place
the text above the character and a little to the side, unless the text won't fit the screen, then it changes the location to ensure the text fits the screen.
If you don't like that behavior you can implement your own text location by implementing the `ISayLocationProvider` interface (and hooking it up, see the section about customizability).

#### Displaying portrait 

As for the portrait, you can configure the object which shows the portrait (this is a standard object, so it can has everything a normal object has, including animations, scaling, rotations, etc),
plus a strategy on where to position the portrait (SpeakerPosition- on the top, either left or right based on where the speaker is standing, Alternating- top right, then top left, then top right, etc 
or Custom which leaves the portrait positioning to you) and custom offsets for both the portrait and for the text from the portrait.

#### Playing the audio clip

In order to play an audio clip, the text you pass to the speech method should have a number at the beginning prefixed by an ampersand, for example, "&17 Hello there!".
If the number exists, the engine will look for an audio file with the first four capital letters of your character's id followed by the number. So if the line was said by "Christopher",
the engine will look for the file "CHRI17". Those files should be placed in a special folder, under Assets -> Speech -> English.
Note: In future revisions we're planning on introducing multi-language support, and also other ways of building the speech "database".
If the engine does not find the file it will not play an audio clip, and in any way the "&17" will be stripped away from the text and will not be displayed on screen. 

### Outfits

Allows a character to switch outfits.
An outfit is a collection of animations that are associated with a character.
The collection can be swapped with another collection when the character changes his look.

Say, for example, your player can wear a jacket. You can create two outfits in advance, one with a jacket
and the other without. Each outfit will have different walk, idle and speak animations (one with a jacket and 
the other without).

There walk, idle and speak animations that the engine might look for when walking, standing still and speaking.
Additionally you can add more animations to an outfit (like "Jump", or "Swim" for example), which you then need to draw per outfit, and will enable you
to call your animation from whatever outfit the character is currently using.
	
### Inventory

Allows your character to carry inventory. Those are the items that the character holds in his/her (usually) imaginary bag.
It is composed of a list of inventory items, and one active item that the character holds in his/her hand.
Those inventory items are usually displayed using an inventory window (see the GUI section).
You can add or remove items from the inventory and get or set the currently active inventory item (a user typically chooses an inventory item from  the inventory window which makes it the active inventory item, and then try to "use" that item on an object on the screen, or on another inventory item).
You can also subscribe to combination events on the inventory, which gives the 2 items that the player attempts to combine and let you code the desired behavior.
And you can also subscribe to a default combination event, for those combination attempts which doesn't have special handling, allowing you, for example, to say something like
"This doesn't make any sense, I don't to use those 2 items together" (but hopefully in a less boring way).




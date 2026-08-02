# Tasting Menu

A third-person action roguelite where the food fights back. Built solo in Unity 6.

You went down to the walk-in for more stock and didn't come back up. Below the restaurant is a kitchen that doesn't end, divided into stations that have been left running long enough that the food has taken over. You've got a knife and a pan lid. Go down.

Every run assembles its floors differently. You fight through food-themed enemies with a sword and shield, pick up loot, choose a stat upgrade between rooms, and hit a boss every fifth floor. Die and you start over on a layout you haven't seen — what you keep is knowledge of the systems, not the map.

## Why I made it

This is my final year project for GAM-604, BA (Hons) Games Development and Futures at ACM Guildford.

The game exists to answer one question: how can a real-time level generator guarantee that a floor is playable without narrowing what it's able to produce? The usual trade is that a tightly constrained generator always makes completable levels that all feel the same, and a loose one makes varied levels that sometimes can't be finished. My generator builds a complete candidate floor, audits it against four structural guarantees, and bins it and reseeds if it fails, rather than restricting what it's allowed to build in the first place. Nothing in the room pool is ever excluded; only unplayable results get thrown away.

The concept came from a gap I kept noticing. Food games are a big category, and almost all of them are cooking games: prepare, serve, manage. Nobody was making the obvious other version, where you fight it.

The full architecture write-up is in the Technical Design Document, and the design and market reasoning is in the Concept Design Document.

## Controls

Fully rebindable at runtime from the settings menu, for both keyboard and controller. Prompts update to whatever you've bound and switch to controller glyphs when you pick one up.

| Action | Keyboard | Controller |
|---|---|---|
| Move | WASD | Left stick |
| Camera | Mouse | Right stick |
| Jump / double jump | Space | A |
| Light attack | Left mouse | X |
| Heavy attack (hold to charge) | Right mouse | Y |
| Shield bash | Q | B |
| Dodge roll | Shift | Right bumper |
| Lock on / cycle target | Middle mouse / flick mouse | Right stick click / flick right stick |
| Pause | Escape | Start |

The dodge has invincibility frames through the middle of the roll but not at the start or the end, so it's a commitment rather than a panic button. Dodge just before an attack lands and you get a brief slow-motion window as a reward.

## Enemies

**Panquake** — leaps, hangs at the top of the arc, slams down with a shockwave.
**Garlic** — never attacks you. Hides behind its allies and shields them. Kill it first.
**Kernel Cannon** — static artillery, lobs shots on a visible arc. Watch where the shadow lands.
**Hot Sauce** — the boss. Four attacks picked by distance, a flamethrower, a full-rotation spin, and kamikaze ghost peppers. Phase two at half health.

## Running it

Download the build from the releases page or from itch.io, extract the whole zip, and run the executable. The `_Data` folder next to it has to stay there; extracting just the .exe won't work.

To open the project, clone it and open the folder in Unity 6. Everything it needs is committed, so there's nothing to fetch from the Asset Store.

## Built with

Unity 6, Universal Render Pipeline, C#, the Unity Input System, Cinemachine, ProBuilder.

## Credits

Player combat animations and character dummy by [Kevin Iglesias](https://assetstore.unity.com/publishers/17148). Sword and shield models from Danvil's Kit01. Everything else — code, design, level authoring, remaining art, by me.

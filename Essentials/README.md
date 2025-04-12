# Essentials

Amethyst plugin providing essential commands for server administration and player utilities.

## Commands

### Player Management
| Command                      | Description                                      | Permission          |
|------------------------------|--------------------------------------------------|---------------------|
| `who [page]`                 | Displays a list of online players.               |                     |
| `broadcast <msg>`            | Broadcasts a message to all players.            | `essentials.broadcast` |
| `slap <player> <damage>`     | Hurts a player.                                  | `essentials.slap`   |
| `kill <player>`              | Kills a player.                                  | `essentials.kill`   |
| `spawn`                      | Teleports you to spawn.                          | `essentials.spawn`  |
| `pos [player]`               | Shows yours or a player's position.              | `essentials.pos`    |

### Character Management
| Command                      | Description                                      | Permission                |
|------------------------------|--------------------------------------------------|---------------------------|
| `ssc reset <name>`           | Resets a character's SSC data.                  | `essentials.ssc.reset`    |
| `ssc savebak`                | Saves a backup of your current character.       | `essentials.ssc.savebak` |
| `ssc loadbak`                | Loads your character from a backup.             | `essentials.ssc.loadbak` |
| `ssc replace <name>`         | Replaces another character's data with yours.   | `essentials.ssc.replace` |
| `ssc clone <name>`           | Clones another character's appearance.          | `essentials.ssc.clone`   |
| `ssc restore`                | Restores your character from a backup.          | `essentials.ssc.restore` |

### Item Management
| Command                      | Description                                      | Permission                |
|------------------------------|--------------------------------------------------|---------------------------|
| `i <item> [count] [prefix]`  | Gives an item to yourself.                      | `essentials.items.item`   |
| `give <player> <item> [count] [prefix]` | Gives an item to a player.       | `essentials.items.give`   |
| `fill [player]`              | Fills inventory stacks to max.                  | `essentials.items.fill`   |

### Buff Management
| Command                      | Description                                      | Permission                |
|------------------------------|--------------------------------------------------|---------------------------|
| `buff <buff_id> [time]`      | Grants you a buff.                               | `essentials.buff`         |
| `gbuff <player> <buff_id> [time]` | Grants a player a buff.                     | `essentials.gbuff`        |
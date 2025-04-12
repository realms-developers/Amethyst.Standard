# Teleportation

Amethyst plugin for teleportation.

## Commands

| Command                                      | Description                                  | Permission                |
|----------------------------------------------|----------------------------------------------|---------------------------|
| `tp <to_player>`                             | Teleports you to another player.             | `teleportation.tp`        |
| `tphere <to_player>`                         | Teleports another player to you.             | `teleportation.tphere`    |
| `tppos <x> <y>`                              | Teleports you to a specified position.       | `teleportation.tppos`     |
| `tprequest <to_player>`, `tpr <to_player>`   | Sends a teleport request to another player.  | `teleportation.tprequest` |
| `tpaccept [from_player]`                     | Accepts a pending teleport request.          |                           |
| `tpdeny [from_player]`                       | Denies a pending teleport request.           |                           |

## Permissions

| Permission             | Description                                      |
|------------------------|--------------------------------------------------|
| `teleportation.silent` | Supresses notifications to the receiving player. |

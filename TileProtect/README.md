# TileProtect

Amethyst plugin for creating and managing protected regions.

## Basic Region Commands

| Command                                      | Description                                  | Permission                |
|----------------------------------------------|----------------------------------------------|---------------------------|
| `rg point <1\|2> [X:Y]`                     | Set a region's boundary point (or hit block) | `amethyst.management.regions` |
| `rg create <name>`                          | Create a new protected region               | `amethyst.management.regions` |
| `rg list [page]`                            | List all regions                            | `amethyst.management.regions` |
| `rg mlist <name> [page]`                    | List members of a region                    | `amethyst.management.regions` |
| `rg rm <name>`                              | Remove a region                             | `amethyst.management.regions` |

## Region Position Management

| Command                                      | Description                                  |
|----------------------------------------------|----------------------------------------------|
| `rg mvx <name> <offset>`                    | Move region's X coordinate                  |
| `rg mvy <name> <offset>`                    | Move region's Y coordinate                  |
| `rg mvx2 <name> <offset>`                   | Move region's second X coordinate           |
| `rg mvy2 <name> <offset>`                   | Move region's second Y coordinate           |
| `rg mvz <name> <offset>`                    | Move region's Z coordinate                  |
| `rg setx <name> <value>`                    | Set region's X coordinate                   |
| `rg sety <name> <value>`                    | Set region's Y coordinate                   |
| `rg setx2 <name> <value>`                   | Set region's second X coordinate            |
| `rg sety2 <name> <value>`                   | Set region's second Y coordinate            |
| `rg setz <name> <value>`                    | Set region's Z coordinate                   |

## Region Membership

| Command                                      | Description                                  |
|----------------------------------------------|----------------------------------------------|
| `rg addmember <name> <username>`            | Add a member to a region                    |
| `rg rmmember <name> <username\|*>`          | Remove member(s) from a region (* for all)   |
| `rg setowner <name> <username>`             | Set the owner of a region                   |

## Region Protection

| Command                                      | Description                                  |
|----------------------------------------------|----------------------------------------------|
| `rg protect <name> <flag> <true\|false>`    | Set protection flags for a region           |
| `rg sprotect <name> <flag> <true\|false>`   | Set super protection flags for a region     |

**Available flags:**
- `tiles` - Tile protection
- `edit` - Edit protection
- `open` - Open protection  
- `signs` - Sign protection
- `chests` - Chest protection
- `tileEntities` - Tile entity protection

## Region Messages

### Enter Messages
| Command                                      | Description                                  |
|----------------------------------------------|----------------------------------------------|
| `rg emsglist <name> [page]`                 | List enter messages                         |
| `rg emsgins <name> <text> [index]`          | Add an enter message                        |
| `rg emsgrm <name> [index]`                  | Remove enter message (-1 for all)           |

### Leave Messages  
| Command                                      | Description                                  |
|----------------------------------------------|----------------------------------------------|
| `rg lmsglist <name> [page]`                 | List leave messages                         |
| `rg lmsgins <name> <text> [index]`          | Add a leave message                         |
| `rg lmsgrm <name> [index]`                  | Remove leave message (-1 for all)           |

## Region Commands

### Enter Commands
| Command                                      | Description                                  |
|----------------------------------------------|----------------------------------------------|
| `rg eclist <name> [page]`                   | List enter commands                         |
| `rg ecins <name> <cmd> <console\|self> [perm] [index]` | Add enter command |
| `rg ecrm <name> [index]`                    | Remove enter command (-1 for all)           |

### Leave Commands
| Command                                      | Description                                  |
|----------------------------------------------|----------------------------------------------|
| `rg lclist <name> [page]`                   | List leave commands                         |
| `rg lcins <name> <cmd> <console\|self> [perm] [index]` | Add leave command |
| `rg lcrm <name> [index]`                    | Remove leave command (-1 for all)           |

## Region Flags

| Command                                      | Description                                  |
|----------------------------------------------|----------------------------------------------|
| `rg autogodmode <name> <true\|false>`       | Toggle automatic god mode in region         |
| `rg noitems <name> <true\|false>`           | Toggle item restrictions in region          |
| `rg noenemies <name> <true\|false>`         | Toggle enemy restrictions in region         |
# Groups

Amethyst module for managing player groups and permissions.

## Group Commands

| Command                                      | Description                                  | Permission                |
|----------------------------------------------|----------------------------------------------|---------------------------|
| `gm init`                                    | Initialize default groups (operator and def) | `amethyst.management.groups` |
| `gm popexclude <group1,group2...>`           | Reset all groups except specified ones      | `amethyst.management.groups` |
| `gm push <name> [permissions]`               | Create a new group                          | `amethyst.management.groups` |
| `gm pop <group1,group2...>`                  | Remove existing groups                      | `amethyst.management.groups` |
| `gm addperm <group> <permissions>`           | Add permissions to a group                  | `amethyst.management.groups` |
| `gm rmperm <group> <permissions>`            | Remove permissions from a group             | `amethyst.management.groups` |
| `gm setdef <group> <true/false>`             | Set a group as default                      | `amethyst.management.groups` |
| `gm btemp <group> <true/false>`              | Block temporary group assignment            | `amethyst.management.groups` |
| `gm parent <group> [parent]`                 | Set group parent                            | `amethyst.management.groups` |
| `gm rgb <group> <R> <G> <B>`                 | Set group color (RGB values)                | `amethyst.management.groups` |
| `gm pfx <group> [prefix]`                    | Set group prefix                            | `amethyst.management.groups` |
| `gm sfx <group> [suffix]`                    | Set group suffix                            | `amethyst.management.groups` |
| `gm usrlist <group> [page]`                  | List users in a group                       | `amethyst.management.groups` |
| `gm permlist <group> [page]`                 | List permissions in a group                 | `amethyst.management.groups` |
| `gm grlist [page]`                           | List all groups                             | `amethyst.management.groups` |

## User Commands

| Command                                      | Description                                  | Permission                |
|----------------------------------------------|----------------------------------------------|---------------------------|
| `um permedlist [page]`                       | List users with personal permissions        | `amethyst.management.groups` |
| `um ppermlist <name> [page]`                | List a user's personal permissions          | `amethyst.management.groups` |
| `um addperm <name> <permission>`            | Add permission to a user                    | `amethyst.management.groups` |
| `um rmperm <name> <permission/*>`           | Remove permission(s) from a user (* for all) | `amethyst.management.groups` |
| `um set <name> <group> [time]`              | Set user's group (with optional time limit) | `amethyst.management.groups` |
| `um unset <name> <direct/temp/all>`         | Remove user's group assignment              | `amethyst.management.groups` |
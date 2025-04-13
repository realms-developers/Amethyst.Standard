# WorldBans

Amethyst plugin for banning specific items and projectiles in the world.

## Item Ban Commands

| Command                     | Description                              | Permission              |
|-----------------------------|------------------------------------------|-------------------------|
| `itemban list [page]`       | Lists all banned items.                  | `worldbans.itemban`     |
| `itemban add <item_id>`     | Bans an item by ID.                      | `worldbans.itemban`     |
| `itemban rm <item_id>`      | Removes an item ban by ID.               | `worldbans.itemban`     |

## Projectile Ban Commands

| Command                     | Description                              | Permission              |
|-----------------------------|------------------------------------------|-------------------------|
| `projban list [page]`       | Lists all banned projectiles.            | `worldbans.projban`     |
| `projban add <proj_id>`     | Bans a projectile by ID.                 | `worldbans.projban`     |
| `projban rm <proj_id>`      | Removes a projectile ban by ID.          | `worldbans.projban`     |

## Permissions

| Permission                  | Description                                      |
|-----------------------------|--------------------------------------------------|
| `worldbans.bypass.item`     | Allows bypassing item bans.                      |
| `worldbans.bypass.proj`     | Allows bypassing projectile bans.                |
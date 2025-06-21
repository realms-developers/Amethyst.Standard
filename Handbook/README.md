# Handbook
Amethyst plugin for creating and displaying customizable help documents in pages.

## Commands
| Command | Description | Permission |
|----------------------------------------------|----------------------------------------------|---------------------------|
| `handbook edit <document> <content>` | Edit or create a handbook document | `handbook.edit` |
| `handbook remove <document>`, `handbook rm <document>` | Remove a handbook document | `handbook.remove` |
| `handbook list [page]` | List all available handbook documents | `handbook.see` |
| `handbook see <document> [page]` | View the contents of a handbook document | `handbook.see` |

## Permissions
| Permission | Description |
|------------------------|--------------------------------------------------|
| `handbook.create` | Allows creating new handbook documents (required for editing non-existent documents) |

## Configuration
### Handbook Configuration
| Property | Type | Default Value | Description |
|----------------|-----------|---------------|-----------------------------------------------------|
| `MaxLines` | int | `5` | Maximum number of lines to display per page |
| `NewLine` | string | `"\n"` | Line separator used when splitting document content into pages |
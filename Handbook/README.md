# Handbook

Amethyst plugin for creating and displaying customizable help documents.

## Usage

1. The plugin automatically creates a `Handbook` directory in your profile's path
2. A sample `handbook.ini` file is generated on first run
3. Each INI file becomes a command (filename = command name)

### Handbook File Format
```ini
; Example handbook.ini
[meta]
description = Displays a welcome message ; Command description. Supports localization
permission = handbook.admin              ; Optional permission requirement

[en-US]                                  ; Language code
content = Welcome!                       ; Displayed content

[ru-RU]
content = приветствовать!
```
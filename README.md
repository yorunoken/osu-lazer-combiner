# osu-lazer-combiner

A utility tool to merge two osu!lazer `client.realm` database files to combine game data from different installations.

Do note that this tool only copies database files, you still need to manually copy over every other file from one install to another.

## Background

When combining two osu!lazer installations, simply copying files isn't enough. osu!lazer uses a `client.realm` database file to track and manage game data. Without merging these database files, your scores, beatmaps, and skins won't appear in the game.

This tool was created to solve that problem by intelligently merging data from two `client.realm` files while avoiding duplicates.

## **IMPORTANT WARNING!!**

**PLEASE BACK UP BOTH OF YOUR `client.realm` FILES BEFORE USING THIS TOOL!**

This program modifies database files and may cause issues if something goes wrong. Always keep backups until you've confirmed everything works correctly in osu!lazer.

## How It Works

The program:

1. Reads objects from a **source** `client.realm` file (eg. your old installation)
2. Copies them to a **target** `client.realm` file (eg. your new installation)
3. Skips objects that already exist to prevent duplicates

## Prerequisites

You must have [Bun](https://bun.sh) installed on your system.

### Installing Bun

**Linux/macOS:**

```bash
curl -fsSL https://bun.sh/install | bash
```

**Windows:**

```powershell
powershell -c "irm bun.sh/install.ps1 | iex"
```

## Installation

1. Clone or download this repository
2. Install dependencies:
    ```bash
    bun install
    ```

## Usage

1. **Locate your `client.realm` files:**

    - Usually found in your osu!lazer installation folder under the `files` directory
    - Common locations:
        - Windows: `%APPDATA%\osu\client.realm`
        - macOS: `~/.local/share/osu/client.realm`
        - Linux: `~/.local/share/osu/client.realm`

2. **Back up both files** (seriously, do this!)

3. **Run the program:**

    ```bash
    bun .
    ```

4. **Follow the prompts:**

    - Enter the path to your **source** file (data will be copied FROM here)
    - Enter the path to your **target** file (data will be copied TO here)
    - Choose whether you want detailed logs
    - Confirm you've made backups

5. **Wait for the merge to complete**

6. **Replace the `client.realm` in your new osu!lazer installation with the merged target file**

7. **Test osu!lazer** to ensure everything works before deleting your backups

## Frequently Asked Questions

### What is a `.realm` file?

Realm is a mobile database platform used by osu!lazer. It works similarly to SQL databases with tables, columns, and rows, but is optimized for performance and real-time applications.

### How safe is this tool?

This tool is open-source and contains no malicious code. It only reads from the source database and writes to the target database without overwriting existing data. However, **always keep backups** until you've verified everything works correctly.

### What if I encounter errors?

1. Make sure both `.realm` files are valid and not corrupted
2. Ensure osu!lazer is closed before running the merge
3. Check that you have read/write permissions for both files
4. If problems persist, restore your backups and [open an issue](../../issues) on GitHub

## License

This project is provided as-is without any warranty. Use at your own risk and always maintain backups.

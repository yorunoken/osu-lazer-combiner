# osu\!lazer Database Merger

A standalone tool to merge multiple osu\!lazer `client.realm` files into one.

If you have scores on your laptop and different scores on your PC (or an old backup you want to restore without wiping your current progress), this tool combines them. It intelligently handles duplicates and links everything back together.

## **!! IMPORTANT !!**

**BACK UP YOUR FILES.**
Seriously. This tool modifies database structures.

1.  Go to your osu\! data folder.
2.  Copy `client.realm` to a safe place (like your Desktop).
3.  Only then run this tool.

## Installation

Go to the **[Releases](https://github.com/yorunoken/osu-lazer-realm-combiner/releases)** page and grab the version that fits your needs:

-   **Portable (Recommended):** `~40MB`. Download, unzip, and run. No extra software needed.
-   **Tiny:** `~14MB`. Much smaller, but you **must** have the [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed on your system first.

## Usage

This is a command-line tool. You provide source files (`-s`) and an output path (`-o`).

### Windows

The easiest way is to open a terminal (cmd or PowerShell) and type the command. You can drag and drop the `.exe` and the `.realm` files into the window to paste their paths.

```powershell
# Merge a backup into a new file
./OsuRealmMerger.exe -s "C:\Backups\old.realm" -o "merged.realm"

# Merge multiple files at once
./OsuRealmMerger.exe -s "backup1.realm" -s "backup2.realm" -o "final.realm"
```

### Linux / macOS

```bash
# Make it executable if needed
chmod +x OsuRealmMerger

# Run it
./OsuRealmMerger -s ./old/client.realm -o ./new/client.realm
```

## The "Missing Files" Note

**Read this if your maps/skins are failing to load:**

This tool merges the **Database** (the index of what you have). It does **not** move the physical files (mp3s, backgrounds, skin textures).

osu\!lazer stores physical files in the `files/` directory (named by hash, like `a4/a4f1...`).

-   **If merging from a local backup:** You are probably fine.
-   **If merging from a different PC:** You must manually copy the contents of the source `files/` folder into your current installation's `files/` folder. (Skip duplicates).

## Building from Source

If you want to modify the code or build it yourself:

1.  Install the [.NET 8 SDK](https://dotnet.microsoft.com/download).
2.  Clone the repo.
3.  Use the `Makefile` (Linux/Mac) or `dotnet` command directly.

```bash
# Builds for everything
make release

# Build just for your current OS
dotnet build
```

## License

MIT. Use at your own risk.

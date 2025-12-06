# osu\!lazer Database Merger

A CLI tool to merge multiple osu\!lazer `client.realm` database files.

This is useful if you have scores scattered across different machines (e.g., Desktop and Laptop) and want to consolidate them into a single history. It handles the database merging by checking for duplicates and linking records, but you must handle the actual file assets manually.

## Requirements & Warnings

1.  **BACK UP YOUR DATA:** This tool modifies database structures directly. Corrupting your `client.realm` means losing all your local scores and collections. **Do not run this on your only copy.**
2.  **Manual Asset Copying:** This tool **only merges the database index**. It does not move the actual `.mp3` or `.osu` files.
      * After merging, you must manually copy the contents of your source `files/` folder into your destination `files/` folder.
      * Skip duplicates when prompted by your OS.
      * *If you skip this step, your maps and skins will appear in the list but fail to load.*

## Installation

Download the latest binary from the [Releases](https://github.com/yorunoken/osu-lazer-realm-combiner/releases) page.

  * **Portable:** Self-contained, no dependencies.
  * **Tiny:** Requires [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

## Usage

This is a command-line application.

**Syntax:**

```bash
./OsuRealmMerger -s <source_realm> -o <target_realm>
```

**Example (Windows Powershell):**

```powershell
# Merge a laptop backup into your main profile
./OsuRealmMerger.exe -s "D:\Backup\client.realm" -o "C:\Users\You\AppData\Roaming\osu\client.realm"
```

**Example (Linux / macOS):**

```bash
chmod +x OsuRealmMerger
./OsuRealmMerger -s ./laptop/client.realm -o ./desktop/client.realm
```

### Options

  * `-s`, `--source`: Path to the input database (the one you want to take data *from*). You can use this flag multiple times to merge several files at once.
  * `-o`, `--output`: Path to the output database (the one you want to write data *to*).

## Building from Source

Requires .NET 8 SDK.

```bash
git clone https://github.com/yorunoken/osu-lazer-realm-combiner.git
cd osu-lazer-realm-combiner

# Build for current OS
dotnet build
```

## License

MIT

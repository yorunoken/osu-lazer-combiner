# osu\!lazer Database Merger

A CLI tool to merge multiple osu\!lazer `client.realm` database files.

This is useful if you have scores scattered across different machines (e.g., Desktop and Laptop) and want to consolidate them into a single history. It handles the database merging by checking for duplicates and linking records, but you must handle the actual file assets manually.

## Requirements & Warnings

1.  **BACK UP YOUR DATA:** This tool modifies database structures directly. Corrupting your `client.realm` means losing all your local scores and collections. **Do not run this on your only copy.**
2.  **Prerequisites:** You must have the **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** installed to run this tool.
3.  **Manual Asset Copying:** This tool **only merges the database index**. It does not move the actual `.mp3` or `.osu` files.
    -   After merging, you must manually copy the contents of your source `files/` folder into your destination `files/` folder.
    -   Skip duplicates when prompted by your OS.
    -   _If you skip this step, your maps and skins will appear in the list but fail to load._

## Usage

Since there are no pre-compiled binaries, you will run the tool directly using the dotnet CLI.

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/yorunoken/osu-realm-combiner.git
    cd osu-realm-combiner
    ```

2.  **Run the merger:**
    Use `dotnet run --` followed by the arguments.

    **Syntax:**

    ```bash
    dotnet run -- -s <source_realm> -o <target_realm>
    ```

    **Example (Windows):**

    ```powershell
    dotnet run -- -s "D:\Backup\client.realm" -o "C:\Users\You\AppData\Roaming\osu\client.realm"
    ```

    **Example (Linux / macOS):**

    ```bash
    dotnet run -- -s /home/yorunoken/laptop/client.realm -o /home/yorunoken/desktop/client.realm
    ```

### Options

-   `-s`, `--source`: Path to the input database (the one you want to take data _from_). You can use this flag multiple times to merge several files at once.
-   `-o`, `--output`: Path to the output database (the one you want to write data _to_). Note: You should point this to a file that does not exist.

## License

MIT

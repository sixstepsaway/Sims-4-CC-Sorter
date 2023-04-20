# Sims-4-CC-Sorter

My first real C# project, at least the first one that's turned into something that works. 

## Requirements

[.net 5.0](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) or [later](https://dotnet.microsoft.com/en-us/download/dotnet).

## Premise 

An app that searches through Sims .package files and gets some info:

- The game version the package file is for. 
- Whether the file is broken.
- The name of the package file.
- What type of in-game item the package file has inside it.
- Whether the item is an override.

after that, it organizes the files for you and saves a lot of trouble. 

## Current Capabilities

- Searches recursively through a folder given to it and outputs a list of which packages are *not* for the game you're trying to run them in. 

## Roadmap

- Next up is identifying the type of file! Is it a bird, is it a plane? No! It's an override of the toaster oven! 

## License 

This is my first C# project worth a damn, but if you see something that's useful to you, feel free to use it in your own, just don't claim it's all yours. Link back!

## Acknowledgements 

Thanks to the following, which I have been learning from: 

- Lazy Duchess' CC Merger https://github.com/LazyDuchess/CC-Merger 
- This DBPF Editor https://github.com/noah-severyn/csDBPF 
- Delphy's Download Organizer's Source Code which made everything suddenly make sense. I'm mostly using this DBPF reader so far and retyping it myself to learn from and adjust.

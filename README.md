![enter image description here](https://img.shields.io/github/downloads/sixstepsaway/Sims-CC-Sorter/total?style=for-the-badge)        ![enter image description here](https://img.shields.io/github/issues/sixstepsaway/sims-cc-sorter?style=for-the-badge)  ![enter image description here](https://img.shields.io/github/watchers/sixstepsaway/sims-cc-sorter?style=for-the-badge)  ![enter image description here](https://img.shields.io/github/v/tag/sixstepsaway/sims-cc-sorter?style=for-the-badge) 
# Sims CC Manager

My first real C# project, at least the first one that's turned into something that works. 

Now built in Godot 4.3.

![An image of the WIP app as it is right now.](https://64.media.tumblr.com/b122d4942504d2867a9957a2a833ab4b/a24a1a2064ec77fa-b0/s1280x1920/c0167e9431ee10027d236ca1ec39349cc50e0154.pnj)

## Requirements

[.net 5.0](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) or [later](https://dotnet.microsoft.com/en-us/download/dotnet).

## Premise 

An app that searches through Sims .package files and gets some info:

- The game version the package file is for. 
- Whether the file is broken.
- The name of the package file.
- What type of in-game item the package file has inside it.
- Whether the item is an override.
- If you have the mesh required by a recolor.

after that, it organizes the files for you and saves a lot of trouble. 

All of these things will be optional, and hopefully have customization options. For example, if you want to sort your CC by type (buy, clothing, accessories) and then creators therein (buy/littledica for example) you can set that up. 

## Current Capabilities

- Searches recursively through a folder given to it and outputs a list of which packages are *not* for the game you're trying to run them in. 

## Roadmap

- NEXT: a functioning UI so you can do it all through that, rather than the command line, and get output both in the form of a log and the form of a list.
-- Immediately after that? An option to automatically move incorrect packages to folders. So, for example, move all Sims 2 packages to "SCCS_Sims 2", all broken packages to "SCCS_Broken" and so on. 
- After that: Get the name of the file from inside the package for Sims 2 packages that are named delightfully descriptive things like "23e73018f4f10e8c1f10b60a81eb19b90001.package", something which is my absolute nemesis.
- Next up is identifying the type of file! Is it a bird, is it a plane? No! It's an override of the toaster oven! 

## License 

This is my first C# project worth a damn, but if you see something that's useful to you, feel free to use it in your own, just don't claim it's all yours. Link back!

## Acknowledgements 

Thanks to the following, which I have been learning from: 

- Lazy Duchess' CC Merger https://github.com/LazyDuchess/CC-Merger 
- This DBPF Editor https://github.com/noah-severyn/csDBPF 
- Delphy's Download Organizer's Source Code which made everything suddenly make sense. I'm mostly using this DBPF reader so far and retyping it myself to learn from and adjust.


![enter image description here](https://img.shields.io/github/downloads/sixstepsaway/Sims-CC-Sorter/total?style=for-the-badge)        ![enter image description here](https://img.shields.io/github/issues/sixstepsaway/sims-cc-sorter?style=for-the-badge)  ![enter image description here](https://img.shields.io/github/watchers/sixstepsaway/sims-cc-sorter?style=for-the-badge)  ![enter image description here](https://img.shields.io/github/v/tag/sixstepsaway/sims-cc-sorter?style=for-the-badge) 
# Sims-CC-Sorter

![An image of the WIP app as it is right now.](https://pbs.twimg.com/media/FuIOMMIXsAIiH4-?format=png&name=small)

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/M4M16X1W)

I've been collecting Sims CC since before the Sims 4 came out in 2014, and because of it I have a large mixed up collection of different kinds of CC for different games, a lot of which is maybe not up to snuff compared to the more recent ones.

Because of how much I have, I don't always want all of it in my game at once, and sometimes I want different builds for different kinds of gameplay, such as SFW for YouTube videos, or historical CC or defaults for decades challenges.

It's surprisingly difficult to sift through and manage content for Sims 4. Sims 2 has Delphy's Mod Organizer, Sims 3 has CC Magic. Sims 4 has is Sims 4 Studio, a program I have quite a few problems with, and one which chokes when you try to open 10k+ files in "My CC". On top of that, you have to go through those files one by one, identifying them visually and sorting them in Windows Explorer. Nope, no good for me.

A couple of years back, I made myself a Batch file that allowed me to create symbolic links for my CC and mods collection so that I wouldn't have to duplicate everything for different builds. Unfortunately, this required more extensive sorting, so I learned Powershell to create a CC sorter based on filenames. 

It worked really well, up until the point I realized it was sorting things like chairs into the hair folders. 🙄

After that I picked up C# to create a better CC sorting application, as well as a way to bulk rename some of the stupidly named Sims 2 floor and wall files I have according to their internal title. 

Months later, here we are, and I've developed an app that lets me sort, filter and adjust my CC collection to my heart's content.

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

## Will this replace-- 

I don't know. Genuinely, I do not. My intention is not to replace CC Merger by LazyDuchess, or g0kur's Mod Organizer, but I do hope to bring Download Organizer functionality to all four games, and add some of my own useful functions along with it.

## License 

This is my first C# project worth a damn, but if you see something that's useful to you, feel free to use it in your own, just don't claim it's all yours. Link back!

## Acknowledgements 

Thanks to the following, which I have been learning from: 

- Lazy Duchess' CC Merger https://github.com/LazyDuchess/CC-Merger 
- This DBPF Editor https://github.com/noah-severyn/csDBPF 
- Delphy's Download Organizer's Source Code which made everything suddenly make sense. I'm mostly using this DBPF reader so far and retyping it myself to learn from and adjust.

Thank you also to: 

- Wanda for her unending support and help constantly.
- Tali for bundling into my hip and helping me debug my code.


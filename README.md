[![Build Status](https://travis-ci.org/tznind/Wanderer.svg?branch=master)](https://travis-ci.org/tznind/Wanderer) [![codecov](https://codecov.io/gh/tznind/Wanderer/branch/master/graph/badge.svg)](https://codecov.io/gh/tznind/Wanderer) [![Coverage Status](https://coveralls.io/repos/github/tznind/Wanderer/badge.svg?branch=master)](https://coveralls.io/github/tznind/Wanderer?branch=master) [![Total alerts](https://img.shields.io/lgtm/alerts/g/tznind/Wanderer.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/tznind/Wanderer/alerts/)



# Wanderer

Wanderer is a game built upon the __most powerful graphics engine in the world: your imagination!__

Stalk the abandoned corridors and burned out machine rooms of a city sized starship.  Kill, sneak or talk your way through problems as you forge your own story.  Will you turn each room into a killing field; gather a loyal band of allies or unite a faction in rebellion against the status quo?

![Screenshot of gameplay showing dialogue][screenshot3]

## Goals

1. Persuasion and Cunning should be as viable as Fighting
2. No dice rolls for actions (if you have Fight 10 and you fight someone with Fight 15 the outcome should always be the same)
3. Actions should have consequences (for relationships, long term injuries etc).

Technical Goals:

1. [Simple yaml/lua game files](./Resources.md)
2. [Super thin interface layer](./src/IUserinterface.cs)
3. Maximum Test coverage

![Screenshot of gameplay showing map][screenshot1]

## Download

You can [download the compiled binaries in the Releases Section](https://github.com/tznind/Wanderer/releases) for both windows and linux console.

**The game is in very early development and not really ready for playing.  Open issues for suggestions or if you like the idea and what to get involved.**

## Building

Build and publish with the `dotnet` command (requires installing [Dot Net 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1))

```bash
dotnet build
dotnet publish -r win-x64
```
__Substitute win-x64 for linux-x64 etc depending on your OS__

After publishing you can run the game from the Game project bin directory

```
cd ./Game/bin/Debug/netcoreapp3.1/win-x64/publish/
./Game.exe
```
__For linux drop the .exe extension__

## Development Goals

All narrative elements driven by the [yaml configuration files](./src/Resources/README.md).  This includes Adjectives, Factions, Rooms, Item slots etc.  The [UI layer is super thin](./src/IUserinterface.cs), consisting of only a handful of methods.

- For every [interface](./src/Actors/IActor.cs), an [abstract](./src/Actors/Actor.cs)!
- For every abstract a [blueprint](./src/Factories/Blueprints/ActorBlueprint.cs)
- For every blueprint a [factory](./src/Factories/ActorFactory.cs)
- For every line of code a [Test](./Tests/Actors/YamlActorFactoryTests.cs)!

Other design patterns include:

- Guids for everyth object in the world (that matters)
- Everything [HasStats](./src/IHasStats.cs): an actor has stats, the room has stats, his items and the items Adjectives all [HasStats](./src/IHasStats.cs).  Yes that means the Light on the end of your Torch has the capability to talk to you (isn't that awesome?!)

## Dependencies

[Terminal.Gui](https://github.com/migueldeicaza/gui.cs) - Only the most awesome console gui ever!
[CommandLineParser](https://github.com/commandlineparser/commandline) - For parsing CLI args
[YamlDotNet](https://github.com/aaubry/YamlDotNet) - Markup language parser
[JSON.Net](https://github.com/JamesNK/Newtonsoft.Json) - For saving/loading
[NLua](https://github.com/NLua/NLua) - For script files

## Class Diagram

![Overview of classes in game][classDiagram]

[classDiagram]: ./src/Overview.cd.png
[screenshot1]: ./src/Screen1.png
[screenshot2]: ./src/Screen2.png
[screenshot3]: ./src/Screen3.png

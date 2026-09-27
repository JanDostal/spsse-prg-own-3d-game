# Game description

- Name – **Star Wars: Combat Arena**
- Topic – **Lightsaber combat**
- Genre – **Action-arcade**
- Graphics type – **3D**
- Platform – **PC**

# Development info

- This game was being developed between December 2021 and January 2022
- Game was uploaded from school repository to this repository in May 2022, which was at the end in the last year of secondary school
- Revision happened in August 2023, which starts from commit [7c225fd](/../../commit/7c225fd4e3438586d2d2a61c7e988add56081778)

# Game design

- Will be played from a third person perspective, the player will have to think tactically as he has to guard his remaining health
- Player will also have to keep an eye on his own stamina and opponent's stamina, because plasma has no mass, but the lightsaber handle weighs something
- Player will have to block lightsaber attack or execute his own lightsaber attack with correct timing
- Each player's win increases the difficulty of the opponent (moving to the next game level)
- If player loses, the difficulty is reset to the beginning (going to the first game level)
- Music will be playing in the background

# Game controls

- Keyboard keys *WSAD* – Player movement
- Mouse left button – Attack
- Mouse right button – Blocking against enemy attacks
- Keyboard key *M* – Pausing game and triggering game menu

# Instructions for starting game

## Release/Production mode

- Download a setup installer from the [latest game release](../../releases/latest) and run installer

## Debug/Development mode

1. Clone this repository from the [development branch](/../../tree/development)
2. Open this Unity project through **Unity Hub**

- It is needed to have **Unity Hub** installed to run the game inside *Unity Editor* for debugging

# Instructions for developers

1. Fork the original repository
2. Clone your fork from the *development branch*
3. Open this Unity project through **Unity Hub**

- Source code can be viewed and modified using IDE **Visual Studio** and **Visual Studio Code**, which are integrated with *Unity Editor*
- Changes made in source code **are applied immediately** to *Unity Editor*

4. When changes are ready to be sent to production, **open a pull request** from the *development branch* of the **forked repository** against the *development branch* of the **original repository**
5. **Before creating the pull request**, state the type of change in the pull request description:

- **bug** – when fixing a bug
- **enhancement** – when implementing a new feature or improvement
- **breaking-change** – when implementing a major change that breaks existing behavior

6. Create the pull request

- The original repository maintainer **will review the pull request** and **merge approved changes** into the *development branch*
- When the maintainer is ready to release the changes to production, they **will create a pull request** from the *development branch* to the *main branch* and **apply the appropriate version label** (`bug`, `enhancement`, or `breaking-change`)

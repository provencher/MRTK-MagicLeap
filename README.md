# MRTK-MagicLeap

Welcome to MRTK-MagicLeap!
This is a basic MRTK extension built to bridge Magic Leap's hand tracking code, improved by the [Magic-Leap-Toolkit](https://github.com/magicleap/Magic-Leap-Toolkit-Unity), to Microsoft's [Mixed reality Toolkit](https://github.com/microsoft/MixedRealityToolkit-Unity).

Right now, feature wise, it is a bit bare, and not all Magic Leap functionality is exposed via the MRTK.

## Enviornment
- Unity 2019.4.3f1
- MRTK V2.4.1
- MLSDK 24.1
- Latest version of magic leap toolkit-unity

## What works:
- Head pose
- Hand Tracking (with palm rotations!)
- Hand Meshing (via a prefab dropped in the scene)

## What doesn't work via MRTK
- Spatial observer (spatial meshing).
- Controller interaction.
- Eye tracking.

# Getting Started
Clone this repository, and then make sure to initialize submodules.
To do this, open a command line terminal, rooted on the folder you'd like the project to be in. 
(Windows: Hold shift + right click -> Select "Open Powershell Window Here")

Then clone using this command "git clone --recurse-submodules https://github.com/provencher/MRTK-MagicLeap.git"

That should be it! Be sure to check out documentation for both Magic Leap and Mixed Reality Toolkit, if you have questions.

# Trying it out
Check out the [releases page](https://github.com/provencher/MRTK-MagicLeap/releases) for a download link to an mpk file.

# Community and support
If you'd like to discuss your issues or ideas to improve this project, please join us over on the [HoloDevs Slack](https://holodevelopersslack.azurewebsites.net/).

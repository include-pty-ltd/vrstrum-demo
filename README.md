# vrstrum-demo
Turn a tracker tracked ViewR device into a window into VR.
Unity Project saved in Unity 2018.2.1f1

VRStrum is a demo which turns a ViewR device into an actual window into VR, distorting the image displayed on the device to match the view the tracked observer.
This demo also shows how to adjust ViewR cameras at run time, and interface with the events that are triggered while ViewR runs.

Additional hardware to run VRStrum
* Tracker for ViewR device
* Tracker for observer

To set up VRStrum
* Drag the VRStrum prefab into your scene
* Set the tracked object number to that of the tracker attached to the observer in the UnityXR script attached to VRStrum/VRStrumCamera
* Set ViewR App to External Tracking mode
* Start the Unity scene
* Connect to the Unity scene with ViewR 
* Set the tracker attached to the ViewR device on ViewR
* Device should now appear as a window

[![Preview](https://pbs.twimg.com/media/DuoMgxIUwAE_CI4.jpg)](https://twitter.com/i/status/1074673490463096837)

# FitCamera2D

A self-resizing orthographic camera addon component to help with rendering at different aspect ratios.

## Usage

Add the FitCamera2D component to a Unity3D Camera in Orthographic mode. There are a few options you'll need to set in the inspector.

* Play Area Width - How wide is our "ideal" game screen?
* Play Area Height - How tall is our "ideal" game height?
* Pixels Per Unit - Match this to your sprite import settings. (Unity default is 100.)
* Update Interval - How often will the camera check to update the aspect ratio at runtime?
* Fit type - How do we match the camera scaling to the screen aspect ratio?
    * Safe Fit - Make sure that all our content is on screen, no matter what resolution the screen is using. If the aspect ratio is different from what we planned, this will lead to empty space at the top and bottom or on the sides. If you leave that area black, you get pillarboxes/letterboxes, or you can expand the background or design a decorative border to be displayed there.
    * Fit Height - Scale the screen so that our vertical size is preserved. This can lead to the pillarboxes if the screen is wider than our design, but it can also lead to content being cut off on the sides of the screen.
    * Fit Width - Scale the screen so that our horizontal size is preserved. This can lead to the letterboxes if the screen is taller than our design, but it can also lead to content being cut off on the top and bottom of the screen.

This component is intended as a runtime tool, but you'll want to test different screen resolutions in the editor. Simply change the size of the Game window in the Unity editor, then hit the "Update camera size" button in the FitCamera2D inspector.

In the Demos folder, there's a simple example scene showing how the system works. The background is intentionally created to be bigger than the design space, so you can see how sections are revealed as the resolution changes.
# Ground Fog Sample

Drag `GroundFog.prefab` into a scene and place it in the XY plane visible to the 2D camera. Move it toward the bottom of the camera view for a floor-hugging result.

The component creates its particle child automatically. Adjust `Area Size`, `Fog Color`, `Particle Size`, `Wind XY`, `Turbulence`, and `Sorting Order` on the prefab instance.

This version targets the project's URP 2D Renderer. It uses a camera-facing billboard and the `SRPDefaultUnlit` pass, so it does not require a depth texture.

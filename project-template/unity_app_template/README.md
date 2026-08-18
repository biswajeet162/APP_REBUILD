# Unity App Template

Stage 2 starter for **game** projects. Copy this folder to a named project at the repo root (e.g. `2d-game/`).

## Open in Unity

1. Install [Unity Hub](https://unity.com/download) with Editor **2022.3 LTS** or newer.
2. **Add** this folder as a project (or open `{your-project-name}/` after Stage 2 copy).
3. Let Unity import assets and generate `Library/` locally (not copied from template).

## Layout

```
Assets/
  Scenes/           # Add game scenes here
  Scripts/
    Starter.cs      # Placeholder entry behaviour
Packages/
  manifest.json
ProjectSettings/
  ProjectVersion.txt
```

## Android build

Configure **File → Build Settings → Android** in the Unity Editor. Use your own keystore for release builds. Do not copy keystores from decompiled dumps.

## Stage 3

Replace `Starter.cs` and add scenes/scripts to reconstruct owned gameplay. Do not paste IL2CPP dumps or proprietary `.so` binaries from `project/`.

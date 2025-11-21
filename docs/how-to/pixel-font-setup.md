# Pixel Font Setup for RCT-Style Text

## Current Issue
The game currently uses Monaco/Consolas which are TrueType fonts that get anti-aliased, making text look smooth and "floating" on top of the pixel art instead of blending in.

## Solution: Install a True Pixel Font

### Option 1: m5x7 (Recommended - Most RCT-like)
1. Download from: https://managore.itch.io/m5x7
2. Install the TTF file on your Mac (double-click, click "Install Font")
3. Update `PixelFont.spritefont` to use:
   ```xml
   <FontName>m5x7</FontName>
   <Size>16</Size>
   ```

### Option 2: m3x6 (Even Smaller)
1. Download from: https://managore.itch.io/m3x6
2. Same installation process
3. Update to:
   ```xml
   <FontName>m3x6</FontName>
   <Size>16</Size>
   ```

### Option 3: Press Start 2P (Classic Retro)
1. Download from: https://fonts.google.com/specimen/Press+Start+2P
2. Install on Mac
3. Update to:
   ```xml
   <FontName>Press Start 2P</FontName>
   <Size>8</Size>
   ```

### Option 4: Pixel Operator (Professional)
1. Download from: https://www.dafont.com/pixel-operator.font
2. Install on Mac
3. Update to:
   ```xml
   <FontName>Pixel Operator</FontName>
   <Size>8</Size>
   ```

## After Installing Font

1. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet build
   dotnet run
   ```

2. The MonoGame Content Pipeline will automatically use the new font when building the .xnb file

## Why This Works

Pixel fonts are designed to be rendered at specific pixel sizes with no antialiasing. When combined with our Point sampling (which we already have), they'll look crisp and pixelated just like RCT.

The key settings we've already configured:
- `TextureFormat=Color` in Content.mgcb (preserves sharp pixels)
- `SamplerState.PointClamp` in spriteBatch.Begin() (no texture filtering)
- `UseKerning=false` (no letter spacing adjustments)

## Current Fallback

Right now we're using **Monaco** (Mac's default monospace) which is better than Courier New but still not pixel art. The game will work but won't have the authentic retro look until a proper pixel font is installed.

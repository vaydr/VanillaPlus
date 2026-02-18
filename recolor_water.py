"""
Recolor water sprites from yellow to white with baby blue tint.
"""

import os
from PIL import Image
import colorsys

def recolor_to_white_baby_blue(image):
    """Convert yellow water to white with a tinge of baby blue."""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        # Convert to HLS
        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Shift hue to baby blue (around 0.55-0.58)
        h = 0.56

        # Make it lighter but keep some depth
        l = min(1.0, l * 0.5 + 0.45)  # Bright but with depth

        # Keep noticeable blue saturation
        s = s * 0.65  # Visible baby blue

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def main():
    script_dir = os.path.dirname(os.path.abspath(__file__))

    water_files = [
        "Content/Biomes/RaptureWaterStyle.png",
        "Content/Biomes/RaptureWaterStyle_Block.png",
        "Content/Biomes/RaptureWaterStyle_Slope.png",
        "Content/Biomes/RaptureWaterfallStyle.png",
    ]

    print("Recoloring water sprites to white with baby blue tint...")

    for water_file in water_files:
        full_path = os.path.join(script_dir, water_file)

        if os.path.exists(full_path):
            print(f"  Processing: {water_file}")
            img = Image.open(full_path)
            recolored = recolor_to_white_baby_blue(img)
            recolored.save(full_path)
            print(f"    Saved!")
        else:
            print(f"  WARNING: {full_path} not found")

    print("\nDone!")

if __name__ == "__main__":
    main()

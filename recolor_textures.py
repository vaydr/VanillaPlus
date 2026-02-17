"""
Recolor vanilla Terraria textures for Rapture biome.
Run this script with Python 3 and Pillow installed.

Usage: python recolor_textures.py <path_to_extracted_terraria_images>

Example: python recolor_textures.py "C:/Terraria/Content/Images"
"""

import sys
import os
from PIL import Image
import colorsys

def hue_shift_to_gold(image):
    """Shift image colors toward gold/warm tones."""
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

        # Shift toward gold (hue ~0.12)
        h = 0.12 + (h - 0.5) * 0.1
        h = h % 1.0

        # Brighten slightly
        l = min(1.0, l * 1.1 + 0.05)

        # Slight desaturation for ethereal look
        s = s * 0.75

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def main():
    if len(sys.argv) < 2:
        print("Usage: python recolor_textures.py <path_to_extracted_terraria_images>")
        print("Example: python recolor_textures.py \"C:/Terraria/Content/Images\"")
        return

    src_dir = sys.argv[1]
    mod_dir = os.path.dirname(os.path.abspath(__file__))

    # Tile mappings: source -> destination
    tiles = {
        "Tiles_1.png": "Content/Tiles/Rapture/Blisstone.png",      # Stone
        "Tiles_2.png": "Content/Tiles/Rapture/Blissgrass.png",     # Grass
        "Tiles_53.png": "Content/Tiles/Rapture/Blissand.png",      # Sand
        "Tiles_161.png": "Content/Tiles/Rapture/BlissIce.png",     # Ice
        "Tiles_397.png": "Content/Tiles/Rapture/HardenedBlissand.png",  # Hardened Sand
        "Tiles_396.png": "Content/Tiles/Rapture/Blissandstone.png",     # Sandstone
    }

    # Item mappings
    items = {
        "Item_3.png": "Content/Items/Rapture/Blisstone.png",       # Stone item
        "Item_169.png": "Content/Items/Rapture/Blissand.png",      # Sand item
        "Item_664.png": "Content/Items/Rapture/BlissIce.png",      # Ice item
        "Item_3271.png": "Content/Items/Rapture/HardenedBlissand.png",  # Hardened Sand
        "Item_3272.png": "Content/Items/Rapture/Blissandstone.png",     # Sandstone
    }

    print("Processing tiles...")
    for src_name, dst_path in tiles.items():
        src_path = os.path.join(src_dir, src_name)
        dst_full = os.path.join(mod_dir, dst_path)

        if os.path.exists(src_path):
            print(f"  {src_name} -> {dst_path}")
            img = Image.open(src_path)
            recolored = hue_shift_to_gold(img)
            os.makedirs(os.path.dirname(dst_full), exist_ok=True)
            recolored.save(dst_full)
        else:
            print(f"  WARNING: {src_path} not found")

    print("\nProcessing items...")
    for src_name, dst_path in items.items():
        src_path = os.path.join(src_dir, src_name)
        dst_full = os.path.join(mod_dir, dst_path)

        if os.path.exists(src_path):
            print(f"  {src_name} -> {dst_path}")
            img = Image.open(src_path)
            recolored = hue_shift_to_gold(img)
            os.makedirs(os.path.dirname(dst_full), exist_ok=True)
            recolored.save(dst_full)
        else:
            print(f"  WARNING: {src_path} not found")

    print("\nDone! Now remove the Texture override and PreDraw from the tile/item classes.")

if __name__ == "__main__":
    main()

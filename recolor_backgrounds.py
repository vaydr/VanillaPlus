"""
Recolor Rapture background sprites in-place.
"""

import os
import shutil
from PIL import Image
import colorsys

def recolor_cavern0(image):
    """Cavern0: dark purple -> dark yellow"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Purple hue is around 0.75-0.85 (270-310 degrees)
        # Shift purple to dark yellow/gold (hue ~0.12-0.15)
        if 0.65 < h < 0.95:  # Purple/violet range
            h = 0.12  # Gold/yellow

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_surface2(image):
    """Surface2: lavender/pinks -> light yellow, greens/blues -> white"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Skip very low saturation (true greys) - preserve them
        if s < 0.08:
            new_pixels.append((r, g, b, a))
            continue

        # Lavender/pinks/magentas (hue ~0.75-1.0 or 0.0-0.05) -> light yellow
        if h > 0.75 or h < 0.05:
            h = 0.15  # Light yellow
            l = min(1.0, l + 0.1)  # Brighten slightly
        # Greens (hue ~0.25-0.45) -> white
        elif 0.25 < h < 0.45:
            s = s * 0.1  # Desaturate to white
            l = min(1.0, l + 0.2)
        # Blues/teals (hue ~0.45-0.75) -> white
        elif 0.45 <= h <= 0.75:
            s = s * 0.1  # Desaturate to white
            l = min(1.0, l + 0.15)

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_surface1(image):
    """Surface1: light blues->white, browns->white, reds/purples->sky blue, oranges->sky blue, greens->banana"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Skip very low saturation (true greys) - preserve them
        if s < 0.08:
            new_pixels.append((r, g, b, a))
            continue

        # Browns/trunk (hue ~0.02-0.15, lower saturation or darker) -> grey-white
        if 0.02 < h < 0.15 and (s < 0.45 or l < 0.45):
            s = s * 0.05  # Desaturate to grey-white
            l = min(1.0, l + 0.15)
        # Light blues (hue ~0.5-0.7) -> white (desaturate)
        elif 0.5 < h < 0.7:
            s = s * 0.1  # Nearly desaturate to white
            l = min(1.0, l + 0.2)  # Brighten
        # Purple-reds (hue ~0.8-1.0) -> sky blue
        elif h > 0.8:
            h = 0.55  # Sky blue
        # Reds (hue ~0.0-0.04) -> sky blue
        elif h < 0.04:
            h = 0.55  # Sky blue
        # Oranges and orange-yellows (hue ~0.04-0.14) -> sky blue
        elif 0.04 <= h < 0.14:
            h = 0.55  # Sky blue
        # Greens (hue ~0.2-0.45) -> banana
        elif 0.2 < h < 0.45:
            h = 0.14  # Banana yellow

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_ice2(image):
    """Ice2: purples -> gold, pinks -> bright yellow, dark blues -> bright blues"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Skip low saturation (greys)
        if s < 0.08:
            new_pixels.append((r, g, b, a))
            continue

        # Purples (hue ~0.7-0.85) -> gold (dark-ish yellow)
        if 0.7 < h < 0.85:
            h = 0.12  # Gold
        # Pinks/magentas (hue ~0.85-1.0 or 0.0-0.05) -> bright yellow
        elif h > 0.85 or h < 0.05:
            h = 0.14  # Bright yellow
            l = min(1.0, l + 0.1)  # Brighten
        # Blues (hue ~0.45-0.72) -> brighten them significantly
        elif 0.45 < h < 0.72:
            l = min(1.0, l + 0.35)  # Brighten dark blues more aggressively

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_ice1(image):
    """Ice1: pinks -> bright gold/banana yellow"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Pink/magenta (hue ~0.85-1.0 or 0.0-0.1) -> bright gold/banana yellow
        if h > 0.8 or h < 0.1:
            h = 0.14  # Bright gold/banana yellow

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_surface3(image):
    """Surface3: blues -> white, pinks/purples -> sky blue, oranges -> yellow, greens -> banana, keep greys"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Skip greys (low saturation) - preserve them
        if s < 0.12:
            new_pixels.append((r, g, b, a))
            continue

        # Blues/teals (hue ~0.45-0.58) -> white (desaturate)
        if 0.45 < h < 0.58:
            s = s * 0.08  # Nearly desaturate to white
            l = min(1.0, l + 0.15)  # Brighten
        # Purples/violets (hue ~0.58-0.85) -> sky blue
        elif 0.58 <= h <= 0.85:
            h = 0.55  # Sky blue
        # Pinks/magentas (hue ~0.85-1.0 or 0.0-0.05) -> sky blue
        elif h > 0.85 or h < 0.05:
            h = 0.55  # Sky blue
        # Oranges (hue ~0.05-0.12) -> yellow
        elif 0.05 <= h < 0.12:
            h = 0.14  # Yellow
        # Greens (hue ~0.2-0.45) -> banana yellow
        elif 0.2 < h <= 0.45:
            h = 0.14  # Banana yellow

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_surface4(image):
    """Surface4: dark blues -> sky blue, greens -> yellow, pinks/reds -> white, brownish trunks -> white"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Skip very low saturation (true greys)
        if s < 0.08:
            new_pixels.append((r, g, b, a))
            continue

        # Brownish-green trunks (hue ~0.2-0.35, lower saturation, darker) -> white
        if 0.18 < h < 0.38 and s < 0.45 and l < 0.55:
            s = s * 0.05  # Desaturate to white
            l = min(1.0, l + 0.2)
        # Dark blues (hue ~0.55-0.72) -> sky blue
        elif 0.55 < h < 0.72:
            h = 0.55  # Sky blue
            l = min(1.0, l + 0.1)  # Brighten slightly
        # Greens/cyans (hue ~0.25-0.55) -> yellow
        elif 0.25 < h <= 0.55:
            h = 0.14  # Yellow
        # Purples (hue ~0.72-0.85) -> sky blue
        elif 0.72 <= h <= 0.85:
            h = 0.55  # Sky blue
        # Pinks/magentas/reds (hue ~0.85-1.0 or 0.0-0.08) -> white
        elif h > 0.85 or h < 0.08:
            s = s * 0.08  # Desaturate to white
            l = min(1.0, l + 0.15)

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_ug295(image):
    """UG_295: purples -> yellow, reds -> white"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Skip low saturation (greys)
        if s < 0.10:
            new_pixels.append((r, g, b, a))
            continue

        # Purples/violets (hue ~0.7-0.85) -> yellow
        if 0.7 < h < 0.88:
            h = 0.14  # Yellow
        # Reds/pinks (hue ~0.88-1.0 or 0.0-0.08) -> white
        elif h > 0.88 or h < 0.08:
            s = s * 0.08  # Desaturate to white
            l = min(1.0, l + 0.15)

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_cavern1(image):
    """Cavern1: purples -> golds, pink -> sky blue, preserve grays"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Skip grays (low saturation) - preserve them
        if s < 0.15:
            new_pixels.append((r, g, b, a))
            continue

        # Pink (hue ~0.9-1.0 or 0.0-0.05) -> sky blue (hue ~0.55)
        if h > 0.9 or h < 0.05:
            h = 0.55  # Sky blue
        # Purple/violet/blue (0.6-0.85) -> gold
        elif 0.6 < h < 0.85:
            h = 0.12  # Gold

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_ice_mapbg(image):
    """IceMapBackground: pinks/purples -> banana yellow"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Pinks/magentas/purples/violets (hue ~0.6-1.0 or 0.0-0.05) -> banana yellow
        # Catches even dark purples
        if h > 0.6 or h < 0.05:
            h = 0.14  # Banana yellow

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_underground_mapbg(image):
    """UndergroundMapBackground: pinks/purples -> banana yellow, blues -> baby blue"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Pinks/magentas/purples/violets (hue ~0.6-1.0 or 0.0-0.05) -> banana yellow
        # Catches even dark purples
        if h > 0.6 or h < 0.05:
            h = 0.14  # Banana yellow
        # Blues (hue ~0.5-0.6) -> lighter baby blue
        elif 0.5 < h <= 0.6:
            h = 0.55  # Sky/baby blue
            l = min(1.0, l + 0.25)  # Brighten significantly

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_water(image):
    """Water styles: cream yellow -> bright #ffff66 yellow"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    # Target color #ffff66 = RGB(255, 255, 102) -> HLS(0.167, 0.7, 1.0)
    target_h = 0.167  # Pure yellow hue

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Shift all yellowish colors toward #ffff66
        if 0.08 < h < 0.25 or s < 0.3:  # Yellow range or low saturation (cream)
            h = target_h  # Pure yellow hue
            s = 1.0  # Full saturation like #ffff66
            # Keep relative lightness but bias toward 0.7

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_radiant_shard(image):
    """RadiantShard: cyan -> baby blue, pink/magenta -> gold, purple -> white"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Pink/magenta (hue ~0.8-1.0 or 0.0-0.05) -> gold
        if h > 0.8 or h < 0.05:
            h = 0.12  # Gold hue
        # Purple (hue ~0.7-0.8) -> white (desaturate)
        elif 0.7 < h <= 0.8:
            s = s * 0.1  # Nearly white
            l = min(1.0, l + 0.2)
        # Cyan/teal (hue ~0.45-0.55) -> baby blue (brighten)
        elif 0.45 < h < 0.55:
            h = 0.55  # Sky/baby blue
            l = min(1.0, l + 0.1)

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_glowing_radiant_shard(image):
    """GlowingRadiantShard: same as radiant but brighter"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Pink/magenta -> bright gold
        if h > 0.8 or h < 0.05:
            h = 0.12
            l = min(1.0, l + 0.15)  # Brighter
            s = min(1.0, s * 1.2)
        # Purple -> bright white
        elif 0.7 < h <= 0.8:
            s = s * 0.05
            l = min(1.0, l + 0.3)
        # Cyan -> bright baby blue
        elif 0.45 < h < 0.55:
            h = 0.55
            l = min(1.0, l + 0.2)

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def recolor_desert_mapbg(image):
    """DesertMapBackground: cyans (not blues) -> bright yellow"""
    if image.mode != 'RGBA':
        image = image.convert('RGBA')

    pixels = list(image.getdata())
    new_pixels = []

    for r, g, b, a in pixels:
        if a == 0:
            new_pixels.append((r, g, b, a))
            continue

        h, l, s = colorsys.rgb_to_hls(r/255, g/255, b/255)

        # Cyans (hue ~0.45-0.55) -> bright yellow, leave blues (0.55-0.7) alone
        if 0.42 < h < 0.55:
            h = 0.14  # Bright yellow

        r2, g2, b2 = colorsys.hls_to_rgb(h, l, s)
        new_pixels.append((int(r2*255), int(g2*255), int(b2*255), a))

    result = Image.new('RGBA', image.size)
    result.putdata(new_pixels)
    return result

def main():
    bg_dir = os.path.join(os.path.dirname(os.path.abspath(__file__)),
                          "Content", "Biomes", "Backgrounds")

    # Process Cavern0
    cavern0_path = os.path.join(bg_dir, "RaptureCavern0.png")
    if os.path.exists(cavern0_path):
        print(f"Processing RaptureCavern0.png...")
        img = Image.open(cavern0_path)
        recolored = recolor_cavern0(img)
        recolored.save(cavern0_path)
        print("  Done!")
    else:
        print(f"  WARNING: {cavern0_path} not found")

    # Process Surface2_Close
    surf2_close = os.path.join(bg_dir, "RaptureSurface2_Close.png")
    if os.path.exists(surf2_close):
        print(f"Processing RaptureSurface2_Close.png...")
        img = Image.open(surf2_close)
        recolored = recolor_surface2(img)
        recolored.save(surf2_close)
        print("  Done!")
    else:
        print(f"  WARNING: {surf2_close} not found")

    # Process Surface2_Mid
    surf2_mid = os.path.join(bg_dir, "RaptureSurface2_Mid.png")
    if os.path.exists(surf2_mid):
        print(f"Processing RaptureSurface2_Mid.png...")
        img = Image.open(surf2_mid)
        recolored = recolor_surface2(img)
        recolored.save(surf2_mid)
        print("  Done!")
    else:
        print(f"  WARNING: {surf2_mid} not found")

    # Process Surface2_Far
    surf2_far = os.path.join(bg_dir, "RaptureSurface2_Far.png")
    if os.path.exists(surf2_far):
        print(f"Processing RaptureSurface2_Far.png...")
        img = Image.open(surf2_far)
        recolored = recolor_surface2(img)
        recolored.save(surf2_far)
        print("  Done!")
    else:
        print(f"  WARNING: {surf2_far} not found")

    # Process Surface1_Close
    surf1_close = os.path.join(bg_dir, "RaptureSurface1_Close.png")
    if os.path.exists(surf1_close):
        print(f"Processing RaptureSurface1_Close.png...")
        img = Image.open(surf1_close)
        recolored = recolor_surface1(img)
        recolored.save(surf1_close)
        print("  Done!")
    else:
        print(f"  WARNING: {surf1_close} not found")

    # Process Surface1_Mid
    surf1_mid = os.path.join(bg_dir, "RaptureSurface1_Mid.png")
    if os.path.exists(surf1_mid):
        print(f"Processing RaptureSurface1_Mid.png...")
        img = Image.open(surf1_mid)
        recolored = recolor_surface1(img)
        recolored.save(surf1_mid)
        print("  Done!")
    else:
        print(f"  WARNING: {surf1_mid} not found")

    # Process Surface1_Far
    surf1_far = os.path.join(bg_dir, "RaptureSurface1_Far.png")
    if os.path.exists(surf1_far):
        print(f"Processing RaptureSurface1_Far.png...")
        img = Image.open(surf1_far)
        recolored = recolor_surface1(img)
        recolored.save(surf1_far)
        print("  Done!")
    else:
        print(f"  WARNING: {surf1_far} not found")

    # Process Ice2
    ice2_path = os.path.join(bg_dir, "RaptureIce2.png")
    if os.path.exists(ice2_path):
        print(f"Processing RaptureIce2.png...")
        img = Image.open(ice2_path)
        recolored = recolor_ice2(img)
        recolored.save(ice2_path)
        print("  Done!")
    else:
        print(f"  WARNING: {ice2_path} not found")

    # Process Ice3
    ice3_path = os.path.join(bg_dir, "RaptureIce3.png")
    if os.path.exists(ice3_path):
        print(f"Processing RaptureIce3.png...")
        img = Image.open(ice3_path)
        recolored = recolor_ice2(img)
        recolored.save(ice3_path)
        print("  Done!")
    else:
        print(f"  WARNING: {ice3_path} not found")

    # Process Ice1
    ice1_path = os.path.join(bg_dir, "RaptureIce1.png")
    if os.path.exists(ice1_path):
        print(f"Processing RaptureIce1.png...")
        img = Image.open(ice1_path)
        recolored = recolor_ice1(img)
        recolored.save(ice1_path)
        print("  Done!")
    else:
        print(f"  WARNING: {ice1_path} not found")

    # Process Cavern1
    cavern1_path = os.path.join(bg_dir, "RaptureCavern1.png")
    if os.path.exists(cavern1_path):
        print(f"Processing RaptureCavern1.png...")
        img = Image.open(cavern1_path)
        recolored = recolor_cavern1(img)
        recolored.save(cavern1_path)
        print("  Done!")
    else:
        print(f"  WARNING: {cavern1_path} not found")

    # Process Surface3_Close
    surf3_close = os.path.join(bg_dir, "RaptureSurface3_Close.png")
    if os.path.exists(surf3_close):
        print(f"Processing RaptureSurface3_Close.png...")
        img = Image.open(surf3_close)
        recolored = recolor_surface3(img)
        recolored.save(surf3_close)
        print("  Done!")
    else:
        print(f"  WARNING: {surf3_close} not found")

    # Process Surface3_Far
    surf3_far = os.path.join(bg_dir, "RaptureSurface3_Far.png")
    if os.path.exists(surf3_far):
        print(f"Processing RaptureSurface3_Far.png...")
        img = Image.open(surf3_far)
        recolored = recolor_surface3(img)
        recolored.save(surf3_far)
        print("  Done!")
    else:
        print(f"  WARNING: {surf3_far} not found")

    # Process Surface4_Close
    surf4_close = os.path.join(bg_dir, "RaptureSurface4_Close.png")
    if os.path.exists(surf4_close):
        print(f"Processing RaptureSurface4_Close.png...")
        img = Image.open(surf4_close)
        recolored = recolor_surface4(img)
        recolored.save(surf4_close)
        print("  Done!")
    else:
        print(f"  WARNING: {surf4_close} not found")

    # Process Surface4_Mid
    surf4_mid = os.path.join(bg_dir, "RaptureSurface4_Mid.png")
    if os.path.exists(surf4_mid):
        print(f"Processing RaptureSurface4_Mid.png...")
        img = Image.open(surf4_mid)
        recolored = recolor_surface4(img)
        recolored.save(surf4_mid)
        print("  Done!")
    else:
        print(f"  WARNING: {surf4_mid} not found")

    # Process Surface4_Far
    surf4_far = os.path.join(bg_dir, "RaptureSurface4_Far.png")
    if os.path.exists(surf4_far):
        print(f"Processing RaptureSurface4_Far.png...")
        img = Image.open(surf4_far)
        recolored = recolor_surface4(img)
        recolored.save(surf4_far)
        print("  Done!")
    else:
        print(f"  WARNING: {surf4_far} not found")

    # Process UG_295
    ug295_path = os.path.join(bg_dir, "RaptureUG_295.png")
    if os.path.exists(ug295_path):
        print(f"Processing RaptureUG_295.png...")
        img = Image.open(ug295_path)
        recolored = recolor_ug295(img)
        recolored.save(ug295_path)
        print("  Done!")
    else:
        print(f"  WARNING: {ug295_path} not found")

    # Map backgrounds are in Content/Biomes (parent folder)
    mapbg_dir = os.path.join(os.path.dirname(os.path.abspath(__file__)),
                              "Content", "Biomes")

    # Process IceMapBackground
    ice_mapbg = os.path.join(mapbg_dir, "RaptureIceMapBackground.png")
    if os.path.exists(ice_mapbg):
        print(f"Processing RaptureIceMapBackground.png...")
        img = Image.open(ice_mapbg)
        recolored = recolor_ice_mapbg(img)
        recolored.save(ice_mapbg)
        print("  Done!")
    else:
        print(f"  WARNING: {ice_mapbg} not found")

    # Process UndergroundMapBackground
    ug_mapbg = os.path.join(mapbg_dir, "RaptureUndergroundMapBackground.png")
    if os.path.exists(ug_mapbg):
        print(f"Processing RaptureUndergroundMapBackground.png...")
        img = Image.open(ug_mapbg)
        recolored = recolor_underground_mapbg(img)
        recolored.save(ug_mapbg)
        print("  Done!")
    else:
        print(f"  WARNING: {ug_mapbg} not found")

    # Process DesertMapBackground
    desert_mapbg = os.path.join(mapbg_dir, "RaptureDesertMapBackground.png")
    if os.path.exists(desert_mapbg):
        print(f"Processing RaptureDesertMapBackground.png...")
        img = Image.open(desert_mapbg)
        recolored = recolor_desert_mapbg(img)
        recolored.save(desert_mapbg)
        print("  Done!")
    else:
        print(f"  WARNING: {desert_mapbg} not found")

    # Process Water styles
    water_files = [
        "RaptureWaterStyle.png",
        "RaptureWaterStyle_Block.png",
        "RaptureWaterStyle_Slope.png",
        "RaptureWaterfallStyle.png"
    ]
    for water_file in water_files:
        water_path = os.path.join(mapbg_dir, water_file)
        if os.path.exists(water_path):
            print(f"Processing {water_file}...")
            img = Image.open(water_path)
            recolored = recolor_water(img)
            recolored.save(water_path)
            print("  Done!")
        else:
            print(f"  WARNING: {water_path} not found")

    # Process RadiantShard tiles
    tiles_dir = os.path.join(os.path.dirname(os.path.abspath(__file__)),
                             "Content", "Tiles", "Rapture")

    radiant_path = os.path.join(tiles_dir, "RadiantShard.png")
    if os.path.exists(radiant_path):
        print(f"Processing RadiantShard.png...")
        img = Image.open(radiant_path)
        recolored = recolor_radiant_shard(img)
        recolored.save(radiant_path)
        print("  Done!")
    else:
        print(f"  WARNING: {radiant_path} not found")

    glowing_path = os.path.join(tiles_dir, "GlowingRadiantShard.png")
    if os.path.exists(glowing_path):
        print(f"Processing GlowingRadiantShard.png...")
        img = Image.open(glowing_path)
        recolored = recolor_glowing_radiant_shard(img)
        recolored.save(glowing_path)
        print("  Done!")
    else:
        print(f"  WARNING: {glowing_path} not found")

    # Process RadiantShard item
    items_dir = os.path.join(os.path.dirname(os.path.abspath(__file__)),
                             "Content", "Items", "Rapture")

    item_path = os.path.join(items_dir, "RadiantShard.png")
    if os.path.exists(item_path):
        print(f"Processing RadiantShard item...")
        img = Image.open(item_path)
        recolored = recolor_radiant_shard(img)
        recolored.save(item_path)
        print("  Done!")
    else:
        print(f"  WARNING: {item_path} not found")

    print("\nComplete!")

if __name__ == "__main__":
    main()

#!/usr/bin/env python3
"""Draws the PPTimer clock icons (no dependencies).

Tray icons:  python3 tools/draw-icons.py            -> PPTimer/Assets/{clock,running,alert}.png (64px)
App icon:    python3 tools/draw-icons.py --app DIR  -> DIR/icon_<size>.png for building a macOS .icns
"""
import math, os, struct, sys, zlib

DARK = (34, 34, 34)
FACES = {"clock": (245, 245, 245), "running": (52, 199, 89), "alert": (235, 64, 64)}

def write_png(path, px):
    h, w = len(px), len(px[0])
    raw = b"".join(b"\0" + bytes(c for p in row for c in p) for row in px)
    def chunk(t, d): return struct.pack(">I", len(d)) + t + d + struct.pack(">I", zlib.crc32(t + d) & 0xffffffff)
    with open(path, "wb") as f:
        f.write(b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", struct.pack(">IIBBBBB", w, h, 8, 6, 0, 0, 0))
                + chunk(b"IDAT", zlib.compress(raw, 9)) + chunk(b"IEND", b""))

def seg_dist(px, py, ax, ay, bx, by):
    dx, dy = bx - ax, by - ay
    t = max(0, min(1, ((px - ax) * dx + (py - ay) * dy) / (dx * dx + dy * dy)))
    return math.hypot(px - ax - t * dx, py - ay - t * dy)

def render(face, size, margin=0.0):
    """Clock with hands at 12 and 3, like the original 2009 icons. All geometry is in 64-unit space."""
    ss = max(2, 512 // size)  # supersampling per axis
    scale = 64 / (size * (1 - 2 * margin))
    off = size * margin
    c, r_out, r_in = 32, 30, 25.5
    hands = [((c, c), (c, c - 19)), ((c, c), (c + 17, c))]
    def shade(x, y):
        d = math.hypot(x - c, y - c)
        if d > r_out: return None
        if d > r_in or d < 4 or any(seg_dist(x, y, *a, *b) <= 2.6 for a, b in hands): return DARK
        return face
    px = []
    for y in range(size):
        row = []
        for x in range(size):
            r = g = b = a = 0
            for sy in range(ss):
                for sx in range(ss):
                    col = shade((x - off + (sx + .5) / ss) * scale, (y - off + (sy + .5) / ss) * scale)
                    if col: r += col[0]; g += col[1]; b += col[2]; a += 1
            row.append((r // a, g // a, b // a, a * 255 // (ss * ss)) if a else (0, 0, 0, 0))
        px.append(row)
    return px

if __name__ == "__main__":
    if len(sys.argv) == 3 and sys.argv[1] == "--app":
        out = sys.argv[2]
        os.makedirs(out, exist_ok=True)
        # macOS app icons leave ~10% padding around the artwork.
        for size in (16, 32, 64, 128, 256, 512, 1024):
            write_png(os.path.join(out, f"icon_{size}.png"), render(FACES["running"], size, margin=0.1))
    else:
        assets = os.path.join(os.path.dirname(__file__), "..", "PPTimer", "Assets")
        for name, face in FACES.items():
            write_png(os.path.join(assets, f"{name}.png"), render(face, 64))

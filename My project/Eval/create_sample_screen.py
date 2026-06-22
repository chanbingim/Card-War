from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "Eval" / "sample-game-screen.png"
WIDTH, HEIGHT = 1600, 900


def font(size, bold=False):
    candidates = [
        "C:/Windows/Fonts/segoeuib.ttf" if bold else "C:/Windows/Fonts/segoeui.ttf",
        "C:/Windows/Fonts/arialbd.ttf" if bold else "C:/Windows/Fonts/arial.ttf",
    ]
    for path in candidates:
        if Path(path).exists():
            return ImageFont.truetype(path, size)
    return ImageFont.load_default()


def rounded(draw, box, fill, outline=None, radius=8, width=1):
    draw.rounded_rectangle(box, radius=radius, fill=fill, outline=outline, width=width)


def text(draw, xy, value, size=24, fill=(30, 36, 42), bold=False, anchor=None):
    draw.text(xy, value, font=font(size, bold), fill=fill, anchor=anchor)


img = Image.new("RGB", (WIDTH, HEIGHT), (236, 240, 242))
draw = ImageDraw.Draw(img)

rounded(draw, (0, 0, WIDTH, HEIGHT), (236, 240, 242), radius=0)
draw.rectangle((0, 0, WIDTH, 84), fill=(28, 55, 72))
text(draw, (64, 41), "Turn Card Game - Sample Test", 32, (255, 255, 255), True, "lm")
text(draw, (1536, 42), "16:9 Preview", 20, (210, 225, 235), False, "rm")

rounded(draw, (64, 116, 430, 796), (255, 255, 255), (196, 207, 214), 8)
text(draw, (94, 156), "Stage 1 - Training Field", 29, (15, 30, 38), True)
text(draw, (94, 204), "Phase: PlayerTurn", 22)
text(draw, (94, 238), "Player HP: 30", 22)
text(draw, (94, 272), "Hand: 2/5", 22)

text(draw, (94, 336), "Quick Test", 24, (15, 30, 38), True)
rounded(draw, (94, 370, 400, 426), (35, 78, 102), radius=8)
text(draw, (247, 398), "Play Sample Test", 22, (255, 255, 255), True, "mm")
rounded(draw, (94, 442, 400, 498), (35, 78, 102), radius=8)
text(draw, (247, 470), "Sample Turn", 22, (255, 255, 255), True, "mm")
rounded(draw, (94, 514, 400, 570), (62, 91, 105), radius=8)
text(draw, (247, 542), "Resolve Combat", 22, (255, 255, 255), True, "mm")

rounded(draw, (470, 116, 1030, 796), (255, 255, 255), (196, 207, 214), 8)
text(draw, (500, 156), "Battle", 30, (15, 30, 38), True)

text(draw, (500, 218), "Monsters", 24, (15, 30, 38), True)
rounded(draw, (500, 248, 1000, 338), (246, 248, 249), (206, 216, 222), 8)
text(draw, (530, 286), "Training Slime", 24, (20, 36, 44), True)
text(draw, (970, 286), "14 / 14 HP", 22, (95, 42, 42), True, "rm")

text(draw, (500, 398), "Hand", 24, (15, 30, 38), True)
cards = [("Strike", "Damage 6", (218, 237, 246)), ("Guard", "Guard 3", (229, 241, 224))]
for idx, (title, detail, color) in enumerate(cards):
    x = 500 + idx * 250
    rounded(draw, (x, 430, x + 220, 570), color, (151, 173, 184), 8)
    text(draw, (x + 24, 468), title, 28, (20, 36, 44), True)
    text(draw, (x + 24, 512), detail, 20, (42, 56, 64))
    rounded(draw, (x + 24, 536, x + 196, 562), (35, 78, 102), radius=6)
    text(draw, (x + 110, 549), "Record", 15, (255, 255, 255), True, "mm")

text(draw, (500, 636), "Recorded Actions", 24, (15, 30, 38), True)
rounded(draw, (500, 668, 1000, 734), (246, 248, 249), (206, 216, 222), 8)
text(draw, (530, 707), "No cards recorded yet.", 22, (60, 70, 76))

rounded(draw, (1070, 116, 1536, 796), (255, 255, 255), (196, 207, 214), 8)
text(draw, (1100, 156), "Sample Flow", 30, (15, 30, 38), True)
flow = [
    "1. Start Game or Play Sample Test",
    "2. Select Stage 1",
    "3. Click card buttons to record actions",
    "4. Click Resolve Combat",
    "5. Stage clear returns to lobby",
]
for i, line in enumerate(flow):
    text(draw, (1100, 220 + i * 46), line, 22, (38, 48, 54))

text(draw, (1100, 520), "Combat Log", 24, (15, 30, 38), True)
rounded(draw, (1100, 552, 1506, 720), (246, 248, 249), (206, 216, 222), 8)
log_lines = [
    "Sample test started.",
    "Player turn started. Drew 2 cards.",
    "Sample Turn records all hand cards.",
]
for i, line in enumerate(log_lines):
    text(draw, (1130, 590 + i * 34), line, 20, (54, 65, 72))

img.save(OUT)
print(OUT)

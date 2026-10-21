from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfgen import canvas


ROOT = Path(__file__).resolve().parents[2]
IMAGE = ROOT / "Assets" / "Texture" / "Effect" / "screen-flash-impact-gold.png"
OUTPUT = ROOT / "output" / "pdf" / "screen-flash-texture-report.pdf"
OUTPUT.parent.mkdir(parents=True, exist_ok=True)

pdfmetrics.registerFont(TTFont("Malgun", r"C:\Windows\Fonts\malgun.ttf"))
pdfmetrics.registerFont(TTFont("Malgun-Bold", r"C:\Windows\Fonts\malgunbd.ttf"))

page_w, page_h = A4
c = canvas.Canvas(str(OUTPUT), pagesize=A4)
c.setTitle("Screen Flash Texture 작업 보고서")
c.setAuthor("Codex")

ink = colors.HexColor("#17191F")
gold = colors.HexColor("#D4A544")
muted = colors.HexColor("#6D7078")
line = colors.HexColor("#D8D3C8")
panel = colors.HexColor("#111722")

c.setFillColor(ink)
c.rect(0, page_h - 15 * mm, page_w, 15 * mm, fill=1, stroke=0)
c.setFillColor(colors.HexColor("#F0CB74"))
c.setFont("Malgun-Bold", 9)
c.drawString(18 * mm, page_h - 9.5 * mm, "CARD COMBAT FX LAB / VFX ASSET")

c.setFillColor(ink)
c.setFont("Malgun-Bold", 24)
c.drawString(18 * mm, page_h - 32 * mm, "Screen Flash 텍스처")
c.setFont("Malgun", 10)
c.setFillColor(muted)
c.drawString(18 * mm, page_h - 41 * mm, "카드 전투 타격 순간에 사용하는 투명 오버레이 이미지")

preview_x, preview_y = 18 * mm, page_h - 139 * mm
preview_w, preview_h = 174 * mm, 98 * mm
c.setFillColor(panel)
c.roundRect(preview_x, preview_y, preview_w, preview_h, 3 * mm, fill=1, stroke=0)
c.drawImage(str(IMAGE), preview_x, preview_y, width=preview_w, height=preview_h, preserveAspectRatio=True, anchor="c", mask="auto")

c.setFillColor(ink)
c.setFont("Malgun-Bold", 14)
c.drawString(18 * mm, page_h - 153 * mm, "에셋 사양")

specs = [
    ("파일", "Assets/Texture/Effect/screen-flash-impact-gold.png"),
    ("해상도", "1672 x 941 px / 16:9"),
    ("포맷", "PNG / RGBA / 투명 알파"),
    ("구성", "중앙 우측 백색 코어, 아이보리-골드 방사광, 가장자리 알파 감쇠"),
]

table_x = 18 * mm
row_y = page_h - 164 * mm
for label, value in specs:
    c.setStrokeColor(line)
    c.line(table_x, row_y - 6 * mm, table_x + 174 * mm, row_y - 6 * mm)
    c.setFillColor(gold)
    c.setFont("Malgun-Bold", 8)
    c.drawString(table_x, row_y, label)
    c.setFillColor(ink)
    c.setFont("Malgun", 8.5)
    c.drawString(table_x + 28 * mm, row_y, value)
    row_y -= 11 * mm

c.setFont("Malgun-Bold", 14)
c.setFillColor(ink)
c.drawString(18 * mm, page_h - 216 * mm, "Unity 적용 권장값")

tips = [
    "Texture Type: Default 또는 Sprite (2D and UI)",
    "Alpha Source: Input Texture Alpha / Alpha Is Transparency 활성화",
    "Canvas Image를 전체 화면에 배치하고 Raycast Target 비활성화",
    "Blend: Screen 또는 Additive 계열 머티리얼 권장",
    "타격 시 알파를 약 0.10-0.18초 동안 빠르게 올린 뒤 0으로 감쇠",
    "Bloom과 함께 사용할 경우 Bloom 강도는 짧게만 상승시켜 과노출 방지",
]

c.setFont("Malgun", 8.5)
c.setFillColor(muted)
tip_y = page_h - 228 * mm
for tip in tips:
    c.setFillColor(gold)
    c.circle(20 * mm, tip_y + 1.2 * mm, 0.8 * mm, fill=1, stroke=0)
    c.setFillColor(ink)
    c.drawString(24 * mm, tip_y, tip)
    tip_y -= 8 * mm

c.setFont("Malgun", 7.5)
c.setFillColor(muted)
c.drawString(18 * mm, 10 * mm, "Generated with built-in ImageGen / 2026-10-21")
c.drawRightString(page_w - 18 * mm, 10 * mm, "1")
c.save()

print(OUTPUT)

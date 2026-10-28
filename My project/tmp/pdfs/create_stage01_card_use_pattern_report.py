from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (
    PageBreak,
    Paragraph,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)


ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "output" / "pdf" / "stage01-card-use-pattern-report.pdf"
OUTPUT.parent.mkdir(parents=True, exist_ok=True)

FONT_REGULAR = Path(r"C:\Windows\Fonts\malgun.ttf")
FONT_BOLD = Path(r"C:\Windows\Fonts\malgunbd.ttf")
pdfmetrics.registerFont(TTFont("Malgun", str(FONT_REGULAR)))
pdfmetrics.registerFont(TTFont("MalgunBold", str(FONT_BOLD)))

PAGE_WIDTH, PAGE_HEIGHT = A4
NAVY = colors.HexColor("#17233B")
BLUE = colors.HexColor("#3264D6")
PALE_BLUE = colors.HexColor("#EAF0FF")
PALE_GRAY = colors.HexColor("#F4F6F9")
TEXT = colors.HexColor("#202938")
MUTED = colors.HexColor("#657087")
LINE = colors.HexColor("#D8DEEA")


def footer(canvas, document):
    canvas.saveState()
    canvas.setStrokeColor(LINE)
    canvas.line(18 * mm, 14 * mm, PAGE_WIDTH - 18 * mm, 14 * mm)
    canvas.setFont("Malgun", 8)
    canvas.setFillColor(MUTED)
    canvas.drawString(18 * mm, 9 * mm, "Stage 01 Card Use Pattern")
    canvas.drawRightString(PAGE_WIDTH - 18 * mm, 9 * mm, f"{document.page}")
    canvas.restoreState()


styles = getSampleStyleSheet()
styles.add(ParagraphStyle(
    name="KTitle",
    fontName="MalgunBold",
    fontSize=22,
    leading=30,
    textColor=NAVY,
    alignment=TA_CENTER,
    spaceAfter=8 * mm,
))
styles.add(ParagraphStyle(
    name="KSubtitle",
    fontName="Malgun",
    fontSize=10,
    leading=16,
    textColor=MUTED,
    alignment=TA_CENTER,
    spaceAfter=9 * mm,
))
styles.add(ParagraphStyle(
    name="KHeading",
    fontName="MalgunBold",
    fontSize=14,
    leading=20,
    textColor=NAVY,
    spaceBefore=4 * mm,
    spaceAfter=3 * mm,
))
styles.add(ParagraphStyle(
    name="KBody",
    fontName="Malgun",
    fontSize=9.5,
    leading=16,
    textColor=TEXT,
    spaceAfter=2.5 * mm,
))
styles.add(ParagraphStyle(
    name="KSmall",
    fontName="Malgun",
    fontSize=8.2,
    leading=12,
    textColor=TEXT,
))
styles.add(ParagraphStyle(
    name="KSmallBold",
    fontName="MalgunBold",
    fontSize=8.2,
    leading=12,
    textColor=NAVY,
))
styles.add(ParagraphStyle(
    name="KTableHeader",
    fontName="MalgunBold",
    fontSize=8.2,
    leading=12,
    textColor=colors.white,
))


def p(text, style="KBody"):
    return Paragraph(text, styles[style])


doc = SimpleDocTemplate(
    str(OUTPUT),
    pagesize=A4,
    rightMargin=18 * mm,
    leftMargin=18 * mm,
    topMargin=18 * mm,
    bottomMargin=20 * mm,
    title="Stage 01 Card Use Pattern 작업 설명",
    author="Codex",
)

story = [
    Spacer(1, 8 * mm),
    p("Stage 01 Card Use Pattern", "KTitle"),
    p("Card Use 단계용 10턴 XML 행동 데이터 작업 설명", "KSubtitle"),
]

summary_data = [
    [p("항목", "KTableHeader"), p("내용", "KTableHeader")],
    [p("대상 Stage", "KSmall"), p("Stage 1", "KSmall")],
    [p("행동 구간", "KSmall"), p("Card Use", "KSmall")],
    [p("턴 구성", "KSmall"), p("1턴부터 10턴까지 연속된 Queue", "KSmall")],
    [p("전체 행동", "KSmall"), p("17개의 CardUse 행동", "KSmall")],
    [p("반복 정책", "KSmall"), p("10턴 종료 후 1턴부터 반복", "KSmall")],
    [p("카드 누락", "KSmall"), p("해당 행동만 건너뛰고 Queue 진행", "KSmall")],
]
summary = Table(summary_data, colWidths=[38 * mm, 116 * mm], hAlign="CENTER")
summary.setStyle(TableStyle([
    ("BACKGROUND", (0, 0), (-1, 0), NAVY),
    ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
    ("BACKGROUND", (0, 1), (0, -1), PALE_BLUE),
    ("GRID", (0, 0), (-1, -1), 0.5, LINE),
    ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
    ("LEFTPADDING", (0, 0), (-1, -1), 7),
    ("RIGHTPADDING", (0, 0), (-1, -1), 7),
    ("TOPPADDING", (0, 0), (-1, -1), 6),
    ("BOTTOMPADDING", (0, 0), (-1, -1), 6),
]))
story.extend([
    summary,
    Spacer(1, 6 * mm),
    p("구현 목적", "KHeading"),
    p("AI가 Card Use 단계에 진입했을 때 현재 턴의 행동을 읽고, 같은 턴 안에서는 order 순서대로 카드 사용을 예약할 수 있도록 XML을 구성했다. 각 행동에는 실제 카드 ID, 대상 선택 정책, 표시 지연 시간, 한국어 UI 문구가 포함된다."),
    p("데이터 연결", "KHeading"),
    p("cardId는 Cards_ko.csv의 1001 타격, 1002 치유, 1003 공격 강화, 1004 방어 강화를 참조한다. targetPolicy는 공격 카드에 LowestHpEnemy, 자기 강화 및 회복 카드에 Self를 사용한다."),
    p("Queue 처리 기준", "KHeading"),
    p("Turn 노드를 number 오름차순으로 Queue에 적재한다. 현재 턴을 꺼낸 뒤 CardUse 노드를 order 오름차순으로 실행하고, 10턴까지 처리하면 repeat=true 설정에 따라 Queue를 다시 구성한다."),
    PageBreak(),
    p("10턴 행동 요약", "KHeading"),
])

turn_rows = [
    ("1", "견제 공격", "타격"),
    ("2", "공격 준비", "공격 강화 -> 타격"),
    ("3", "방어 강화", "방어 강화"),
    ("4", "연속 공격", "타격 -> 타격"),
    ("5", "체력 회복", "치유"),
    ("6", "강화 공격", "공격 강화 -> 타격"),
    ("7", "수비 정비", "방어 강화 -> 치유"),
    ("8", "집중 공격", "타격 -> 타격"),
    ("9", "공수 강화", "공격 강화 -> 방어 강화"),
    ("10", "마무리 공격", "타격 -> 타격"),
]
turn_data = [[p("턴", "KTableHeader"), p("의도", "KTableHeader"), p("CardUse 순서", "KTableHeader")]]
turn_data.extend([[p(a, "KSmall"), p(b, "KSmall"), p(c, "KSmall")] for a, b, c in turn_rows])
turn_table = Table(turn_data, colWidths=[16 * mm, 42 * mm, 96 * mm], repeatRows=1)
turn_table.setStyle(TableStyle([
    ("BACKGROUND", (0, 0), (-1, 0), BLUE),
    ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
    ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, PALE_GRAY]),
    ("GRID", (0, 0), (-1, -1), 0.45, LINE),
    ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
    ("ALIGN", (0, 1), (0, -1), "CENTER"),
    ("LEFTPADDING", (0, 0), (-1, -1), 6),
    ("RIGHTPADDING", (0, 0), (-1, -1), 6),
    ("TOPPADDING", (0, 0), (-1, -1), 5),
    ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
]))
story.extend([
    turn_table,
    Spacer(1, 5 * mm),
    p("주요 XML 속성", "KHeading"),
])

field_data = [
    [p("속성", "KTableHeader"), p("용도", "KTableHeader")],
    [p("Turn.number", "KSmall"), p("턴 Queue 순서", "KSmall")],
    [p("CardUse.order", "KSmall"), p("한 턴 안의 카드 사용 순서", "KSmall")],
    [p("cardId", "KSmall"), p("Cards_ko.csv 카드 ID", "KSmall")],
    [p("targetPolicy", "KSmall"), p("Self 또는 LowestHpEnemy 대상 선택", "KSmall")],
    [p("delaySeconds", "KSmall"), p("UI 표시 후 다음 행동까지의 대기 시간", "KSmall")],
    [p("DisplayTextKo", "KSmall"), p("Card Use UI에 노출할 한국어 설명", "KSmall")],
]
field_table = Table(field_data, colWidths=[48 * mm, 106 * mm], repeatRows=1)
field_table.setStyle(TableStyle([
    ("BACKGROUND", (0, 0), (-1, 0), NAVY),
    ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
    ("GRID", (0, 0), (-1, -1), 0.45, LINE),
    ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
    ("LEFTPADDING", (0, 0), (-1, -1), 6),
    ("RIGHTPADDING", (0, 0), (-1, -1), 6),
    ("TOPPADDING", (0, 0), (-1, -1), 5),
    ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
]))
story.extend([
    field_table,
    Spacer(1, 5 * mm),
    p("검증 결과", "KHeading"),
    p("XML 파싱 성공, 1부터 10까지 턴 번호 연속성 확인, 각 턴의 order 연속성 확인, 총 17개 행동의 cardId가 Cards_ko.csv에 존재하는지 확인했다."),
])

doc.build(story, onFirstPage=footer, onLaterPages=footer)
print(OUTPUT)

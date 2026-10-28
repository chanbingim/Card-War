from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import PageBreak, Paragraph, SimpleDocTemplate, Spacer, Table, TableStyle


ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "output" / "pdf" / "card-use-pattern-struct-report.pdf"
OUTPUT.parent.mkdir(parents=True, exist_ok=True)

pdfmetrics.registerFont(TTFont("Malgun", r"C:\Windows\Fonts\malgun.ttf"))
pdfmetrics.registerFont(TTFont("MalgunBold", r"C:\Windows\Fonts\malgunbd.ttf"))
pdfmetrics.registerFont(TTFont("Consolas", r"C:\Windows\Fonts\consola.ttf"))

NAVY = colors.HexColor("#17233B")
BLUE = colors.HexColor("#3264D6")
PALE_BLUE = colors.HexColor("#EAF0FF")
PALE_GRAY = colors.HexColor("#F4F6F9")
TEXT = colors.HexColor("#202938")
MUTED = colors.HexColor("#657087")
LINE = colors.HexColor("#D8DEEA")
PAGE_WIDTH, _ = A4


def footer(canvas, document):
    canvas.saveState()
    canvas.setStrokeColor(LINE)
    canvas.line(18 * mm, 14 * mm, PAGE_WIDTH - 18 * mm, 14 * mm)
    canvas.setFont("Malgun", 8)
    canvas.setFillColor(MUTED)
    canvas.drawString(18 * mm, 9 * mm, "Card Use Pattern Struct")
    canvas.drawRightString(PAGE_WIDTH - 18 * mm, 9 * mm, str(document.page))
    canvas.restoreState()


styles = getSampleStyleSheet()
styles.add(ParagraphStyle(name="TitleKo", fontName="MalgunBold", fontSize=22, leading=30,
                          textColor=NAVY, alignment=TA_CENTER, spaceAfter=7 * mm))
styles.add(ParagraphStyle(name="SubtitleKo", fontName="Malgun", fontSize=10, leading=16,
                          textColor=MUTED, alignment=TA_CENTER, spaceAfter=8 * mm))
styles.add(ParagraphStyle(name="HeadingKo", fontName="MalgunBold", fontSize=14, leading=20,
                          textColor=NAVY, spaceBefore=4 * mm, spaceAfter=3 * mm))
styles.add(ParagraphStyle(name="BodyKo", fontName="Malgun", fontSize=9.5, leading=16,
                          textColor=TEXT, spaceAfter=2.5 * mm))
styles.add(ParagraphStyle(name="SmallKo", fontName="Malgun", fontSize=8.1, leading=12,
                          textColor=TEXT))
styles.add(ParagraphStyle(name="HeaderKo", fontName="MalgunBold", fontSize=8.1, leading=12,
                          textColor=colors.white))
styles.add(ParagraphStyle(name="CodeBlock", fontName="Consolas", fontSize=7.8, leading=12,
                          textColor=colors.HexColor("#E7ECF6"), leftIndent=3 * mm,
                          rightIndent=3 * mm, spaceBefore=2 * mm, spaceAfter=2 * mm))


def p(text, style="BodyKo"):
    return Paragraph(text, styles[style])


def make_table(rows, widths, header_color=NAVY):
    table = Table(rows, colWidths=widths, repeatRows=1, hAlign="CENTER")
    table.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), header_color),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, PALE_GRAY]),
        ("GRID", (0, 0), (-1, -1), 0.45, LINE),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("LEFTPADDING", (0, 0), (-1, -1), 6),
        ("RIGHTPADDING", (0, 0), (-1, -1), 6),
        ("TOPPADDING", (0, 0), (-1, -1), 5),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
    ]))
    return table


doc = SimpleDocTemplate(
    str(OUTPUT), pagesize=A4,
    leftMargin=18 * mm, rightMargin=18 * mm,
    topMargin=18 * mm, bottomMargin=20 * mm,
    title="Card Use Pattern Struct 작업 설명", author="Codex",
)

story = [
    Spacer(1, 7 * mm),
    p("Card Use Pattern Struct", "TitleKo"),
    p("Stage AI XML 역직렬화 데이터 모델 작업 설명", "SubtitleKo"),
    p("구현 결과", "HeadingKo"),
    p("Stage01_CardUsePattern.xml의 모든 계층과 속성을 그대로 읽을 수 있는 직렬화 struct 세 개와 정책 enum 세 개를 추가했다. 모델은 UnityEngine에 의존하지 않으며 System.Xml.Serialization의 XmlSerializer로 사용할 수 있다."),
]

type_rows = [
    [p("타입", "HeaderKo"), p("역할", "HeaderKo")],
    [p("CardUsePatternData", "SmallKo"), p("XML 루트, Stage 및 반복 정책, Turn 목록", "SmallKo")],
    [p("CardUseTurnData", "SmallKo"), p("턴 번호, 전투 의도, CardUse 행동 목록", "SmallKo")],
    [p("CardUseActionData", "SmallKo"), p("행동 순서, 카드 ID, 대상 정책, 지연, UI 문구", "SmallKo")],
    [p("CardUsePhase", "SmallKo"), p("현재 XML의 CardUse 단계 값", "SmallKo")],
    [p("MissingCardPolicy", "SmallKo"), p("카드 누락 시 SkipAction 정책", "SmallKo")],
    [p("CardUseTargetPolicy", "SmallKo"), p("Self 및 LowestHpEnemy 대상 선택 정책", "SmallKo")],
]
story.extend([
    make_table(type_rows, [55 * mm, 99 * mm]),
    Spacer(1, 5 * mm),
    p("XML 계층 매핑", "HeadingKo"),
    p("CardUsePatternData의 Turns는 XML의 &lt;Turns&gt; 배열에 대응한다. 각 CardUseTurnData의 CardUses는 &lt;Turn&gt; 내부에 반복되는 &lt;CardUse&gt; 요소에 대응하므로, 역직렬화 후 Turns를 순회해 턴 Queue를 만들 수 있다."),
    p("정책 값 제한", "HeadingKo"),
    p("phase, missingCardPolicy, targetPolicy를 enum으로 선언하고 XmlEnum 값을 XML 문자열과 동일하게 지정했다. 지원하지 않는 정책 문자열이 들어오면 XmlSerializer가 즉시 오류를 발생시켜 잘못된 AI 데이터를 조기에 발견할 수 있다."),
    PageBreak(),
    p("필드 매핑", "HeadingKo"),
])

field_rows = [
    [p("XML", "HeaderKo"), p("C# 필드", "HeaderKo"), p("형식", "HeaderKo")],
    [p("schemaVersion", "SmallKo"), p("SchemaVersion", "SmallKo"), p("string", "SmallKo")],
    [p("patternId", "SmallKo"), p("PatternId", "SmallKo"), p("string", "SmallKo")],
    [p("stageId", "SmallKo"), p("StageId", "SmallKo"), p("int", "SmallKo")],
    [p("phase", "SmallKo"), p("Phase", "SmallKo"), p("CardUsePhase", "SmallKo")],
    [p("repeat", "SmallKo"), p("Repeat", "SmallKo"), p("bool", "SmallKo")],
    [p("missingCardPolicy", "SmallKo"), p("MissingCardPolicy", "SmallKo"), p("enum", "SmallKo")],
    [p("Turn.number", "SmallKo"), p("Number", "SmallKo"), p("int", "SmallKo")],
    [p("Turn.intent", "SmallKo"), p("Intent", "SmallKo"), p("string", "SmallKo")],
    [p("CardUse.order", "SmallKo"), p("Order", "SmallKo"), p("int", "SmallKo")],
    [p("CardUse.cardId", "SmallKo"), p("CardId", "SmallKo"), p("int", "SmallKo")],
    [p("targetPolicy", "SmallKo"), p("TargetPolicy", "SmallKo"), p("enum", "SmallKo")],
    [p("delaySeconds", "SmallKo"), p("DelaySeconds", "SmallKo"), p("float", "SmallKo")],
    [p("DisplayTextKo", "SmallKo"), p("DisplayTextKo", "SmallKo"), p("string", "SmallKo")],
]
story.extend([
    make_table(field_rows, [58 * mm, 58 * mm, 38 * mm], BLUE),
    Spacer(1, 5 * mm),
    p("역직렬화 사용 형태", "HeadingKo"),
])

code_text = (
    "var textAsset = Resources.Load&lt;TextAsset&gt;(&quot;Data/Xml/Stage01_CardUsePattern&quot;);<br/>"
    "var serializer = new XmlSerializer(typeof(CardUsePatternData));<br/>"
    "using var reader = new StringReader(textAsset.text);<br/>"
    "var pattern = (CardUsePatternData)serializer.Deserialize(reader);"
)
code_box = Table([[p(code_text, "CodeBlock")]], colWidths=[154 * mm])
code_box.setStyle(TableStyle([
    ("BACKGROUND", (0, 0), (-1, -1), NAVY),
    ("BOX", (0, 0), (-1, -1), 0.6, colors.HexColor("#2E3E60")),
    ("LEFTPADDING", (0, 0), (-1, -1), 7),
    ("RIGHTPADDING", (0, 0), (-1, -1), 7),
    ("TOPPADDING", (0, 0), (-1, -1), 6),
    ("BOTTOMPADDING", (0, 0), (-1, -1), 6),
]))
story.extend([
    code_box,
    Spacer(1, 5 * mm),
    p("검증 결과", "HeadingKo"),
    p("새 C# 파일을 독립 컴파일한 뒤 실제 Stage 1 XML을 XmlSerializer로 역직렬화했다. StageId 1, Turn 10개, CardUse 17개, Repeat true, MissingCardPolicy SkipAction 및 TargetPolicy enum 매핑을 확인했다."),
])

doc.build(story, onFirstPage=footer, onLaterPages=footer)
print(OUTPUT)

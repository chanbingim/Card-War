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
OUTPUT = ROOT / "output" / "pdf" / "generic-xml-data-loader-report.pdf"
OUTPUT.parent.mkdir(parents=True, exist_ok=True)

pdfmetrics.registerFont(TTFont("Malgun", r"C:\Windows\Fonts\malgun.ttf"))
pdfmetrics.registerFont(TTFont("MalgunBold", r"C:\Windows\Fonts\malgunbd.ttf"))
pdfmetrics.registerFont(TTFont("Consolas", r"C:\Windows\Fonts\consola.ttf"))

NAVY = colors.HexColor("#17233B")
BLUE = colors.HexColor("#3264D6")
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
    canvas.drawString(18 * mm, 9 * mm, "Generic XML Data Loader")
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
styles.add(ParagraphStyle(name="SmallKo", fontName="Malgun", fontSize=8.2, leading=12,
                          textColor=TEXT))
styles.add(ParagraphStyle(name="HeaderKo", fontName="MalgunBold", fontSize=8.2, leading=12,
                          textColor=colors.white))
styles.add(ParagraphStyle(name="CodeBlock", fontName="Consolas", fontSize=7.8, leading=12,
                          textColor=colors.HexColor("#E7ECF6"), leftIndent=2 * mm,
                          rightIndent=2 * mm))


def p(text, style="BodyKo"):
    return Paragraph(text, styles[style])


def make_table(rows, widths, color=NAVY):
    table = Table(rows, colWidths=widths, repeatRows=1, hAlign="CENTER")
    table.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), color),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, PALE_GRAY]),
        ("GRID", (0, 0), (-1, -1), 0.45, LINE),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("LEFTPADDING", (0, 0), (-1, -1), 6),
        ("RIGHTPADDING", (0, 0), (-1, -1), 6),
        ("TOPPADDING", (0, 0), (-1, -1), 5),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
    ]))
    return table


def code_box(lines):
    content = "<br/>".join(lines)
    table = Table([[p(content, "CodeBlock")]], colWidths=[154 * mm])
    table.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, -1), NAVY),
        ("BOX", (0, 0), (-1, -1), 0.6, colors.HexColor("#2E3E60")),
        ("LEFTPADDING", (0, 0), (-1, -1), 7),
        ("RIGHTPADDING", (0, 0), (-1, -1), 7),
        ("TOPPADDING", (0, 0), (-1, -1), 7),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 7),
    ]))
    return table


doc = SimpleDocTemplate(
    str(OUTPUT), pagesize=A4,
    leftMargin=18 * mm, rightMargin=18 * mm,
    topMargin=18 * mm, bottomMargin=20 * mm,
    title="Generic XML Data Loader 작업 설명", author="Codex",
)

story = [
    Spacer(1, 7 * mm),
    p("Generic XML Data Loader", "TitleKo"),
    p("XML 데이터를 struct 타입으로 읽는 Unity Resources 제네릭 로더", "SubtitleKo"),
    p("구현 목적", "HeadingKo"),
    p("XML마다 전용 파서를 반복 작성하지 않고, XmlSerializer 특성이 적용된 struct 타입을 제네릭 인자로 전달해 동일한 로딩 절차를 재사용하도록 구현했다. 현재 CardUsePatternData뿐 아니라 이후 추가되는 동적 AI 정책 struct에도 같은 로더를 사용할 수 있다."),
]

api_rows = [
    [p("API", "HeaderKo"), p("용도", "HeaderKo")],
    [p("Deserialize(string)", "SmallKo"), p("XML 문자열을 T struct로 변환하며 실패 시 예외 발생", "SmallKo")],
    [p("Deserialize(TextAsset)", "SmallKo"), p("Unity TextAsset의 text를 T struct로 변환", "SmallKo")],
    [p("TryDeserialize(...) ", "SmallKo"), p("파싱 결과와 오류 문자열을 반환하는 안전한 변환", "SmallKo")],
    [p("LoadFromResources(path)", "SmallKo"), p("Resources TextAsset 로드와 역직렬화를 한 번에 처리", "SmallKo")],
    [p("TryLoadFromResources(...) ", "SmallKo"), p("누락 파일과 XML 오류를 예외 없이 처리", "SmallKo")],
]
story.extend([
    make_table(api_rows, [64 * mm, 90 * mm]),
    Spacer(1, 5 * mm),
    p("기본 사용", "HeadingKo"),
    code_box([
        "CardUsePatternData pattern =",
        "    XmlDataLoader&lt;CardUsePatternData&gt;.LoadFromResources(",
        "        &quot;Data/Xml/Stage01_CardUsePattern&quot;);",
    ]),
    Spacer(1, 5 * mm),
    p("처리 흐름", "HeadingKo"),
    p("Resources에서 TextAsset을 찾은 뒤 XML 문자열의 DTD와 외부 엔티티 사용을 차단하고, 타입별로 캐시된 XmlSerializer를 통해 T struct를 생성한다. 결과 타입이 일치하지 않거나 XML이 잘못되면 명시적인 오류를 반환한다."),
    PageBreak(),
    p("안전한 초기화 예시", "HeadingKo"),
    code_box([
        "if (!XmlDataLoader&lt;CardUsePatternData&gt;.TryLoadFromResources(",
        "        resourcePath, out var pattern, out var error))",
        "{",
        "    Debug.LogError(error);",
        "    return;",
        "}",
    ]),
    Spacer(1, 5 * mm),
    p("예외 및 보안 처리", "HeadingKo"),
])

safety_rows = [
    [p("상황", "HeaderKo"), p("처리", "HeaderKo")],
    [p("빈 XML 문자열", "SmallKo"), p("ArgumentException 또는 Try 결과 false", "SmallKo")],
    [p("null TextAsset", "SmallKo"), p("ArgumentNullException 또는 Try 결과 false", "SmallKo")],
    [p("누락 Resources 경로", "SmallKo"), p("FileNotFoundException 또는 Try 결과 false", "SmallKo")],
    [p("손상된 XML", "SmallKo"), p("XmlSerializer 오류를 error 문자열로 전달", "SmallKo")],
    [p("DTD 또는 외부 엔티티", "SmallKo"), p("XmlReaderSettings에서 파싱 차단", "SmallKo")],
]
story.extend([
    make_table(safety_rows, [62 * mm, 92 * mm], BLUE),
    Spacer(1, 5 * mm),
    p("검증 결과", "HeadingKo"),
    p("실제 Stage01_CardUsePattern.xml을 XmlDataLoader&lt;CardUsePatternData&gt;로 읽어 Stage 1, Turn 10개, CardUse 17개를 복원했다. 별도 검증에서 손상 XML은 false와 오류 메시지를 반환했고, DTD가 포함된 XML은 보안 설정에 의해 거부됐다."),
    p("프로젝트 적용", "HeadingKo"),
    p("로더는 Assets/Scripts/Utility/XMLReader에 배치했다. Unity가 스크립트를 새로고침하면 생성된 Assembly-CSharp 프로젝트에도 자동으로 포함되며, 생성된 .csproj 파일을 수동으로 수정할 필요가 없다."),
])

doc.build(story, onFirstPage=footer, onLaterPages=footer)
print(OUTPUT)

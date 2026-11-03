from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import Image, PageBreak, Paragraph, SimpleDocTemplate, Spacer, Table, TableStyle

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / "output" / "pdf" / "game-menu-party-formation-ui-report.pdf"
OUT.parent.mkdir(parents=True, exist_ok=True)

font_path = Path("C:/Windows/Fonts/malgun.ttf")
bold_path = Path("C:/Windows/Fonts/malgunbd.ttf")
pdfmetrics.registerFont(TTFont("Malgun", str(font_path)))
pdfmetrics.registerFont(TTFont("MalgunBold", str(bold_path)))

navy = colors.HexColor("#080D24")
violet = colors.HexColor("#7664D8")
gold = colors.HexColor("#FFCA52")
teal = colors.HexColor("#40E3DF")
muted = colors.HexColor("#65708A")
light = colors.HexColor("#F2F4FA")

styles = getSampleStyleSheet()
styles.add(ParagraphStyle(name="KTitle", fontName="MalgunBold", fontSize=23, leading=30, textColor=navy, spaceAfter=10))
styles.add(ParagraphStyle(name="KSub", fontName="Malgun", fontSize=10, leading=16, textColor=muted, spaceAfter=15))
styles.add(ParagraphStyle(name="KH2", fontName="MalgunBold", fontSize=15, leading=21, textColor=navy, spaceBefore=8, spaceAfter=8))
styles.add(ParagraphStyle(name="KBody", fontName="Malgun", fontSize=9.4, leading=15, textColor=colors.HexColor("#252B3A"), spaceAfter=6))
styles.add(ParagraphStyle(name="KSmall", fontName="Malgun", fontSize=8, leading=12, textColor=muted))
styles.add(ParagraphStyle(name="KCell", fontName="Malgun", fontSize=8.2, leading=12, textColor=colors.HexColor("#252B3A")))
styles.add(ParagraphStyle(name="KCellBold", fontName="MalgunBold", fontSize=8.2, leading=12, textColor=navy))
styles.add(ParagraphStyle(name="KHeader", fontName="MalgunBold", fontSize=8.2, leading=12, textColor=colors.white))


def header_footer(canvas, doc):
    canvas.saveState()
    canvas.setFillColor(navy)
    canvas.rect(0, A4[1] - 10 * mm, A4[0], 10 * mm, fill=1, stroke=0)
    canvas.setFont("Malgun", 7.5)
    canvas.setFillColor(colors.white)
    canvas.drawString(16 * mm, A4[1] - 6.7 * mm, "GAME MENU - PARTY FORMATION UI")
    canvas.setFillColor(muted)
    canvas.drawRightString(A4[0] - 16 * mm, 9 * mm, f"2026-11-03   |   {doc.page}")
    canvas.restoreState()


def p(text, style="KBody"):
    return Paragraph(text, styles[style])


doc = SimpleDocTemplate(
    str(OUT), pagesize=A4, rightMargin=16 * mm, leftMargin=16 * mm,
    topMargin=18 * mm, bottomMargin=16 * mm, title="GameMenu Party Formation UI Report"
)
story = []
story += [p("GameMenu 파티 편성 UI 개편", "KTitle"), p("세븐나이츠 계열의 무대형 팀 편성 문법을 참고해, 기존 3인 파티 규칙을 유지하면서 시각적 위계와 조작 흐름을 재구성했다.", "KSub")]

summary = Table([
    [p("대상", "KCellBold"), p("Assets/Scenes/GameMenu.unity", "KCell")],
    [p("해상도", "KCellBold"), p("1600 x 900, 16:9 Game View", "KCell")],
    [p("핵심 흐름", "KCellBold"), p("슬롯 선택 -> 캐릭터 교체 -> A/B 테마 비교 -> 편성 완료", "KCell")],
    [p("검증 결과", "KCellBold"), p("Play 모드 클릭 흐름 통과, A/B 렌더링 및 완료 피드백 확인", "KCell")],
], colWidths=[30 * mm, 145 * mm])
summary.setStyle(TableStyle([
    ("BACKGROUND", (0,0), (0,-1), light), ("BOX", (0,0), (-1,-1), .5, colors.HexColor("#D9DEEA")),
    ("INNERGRID", (0,0), (-1,-1), .35, colors.HexColor("#E5E8F0")), ("VALIGN", (0,0), (-1,-1), "MIDDLE"),
    ("LEFTPADDING", (0,0), (-1,-1), 7), ("RIGHTPADDING", (0,0), (-1,-1), 7),
    ("TOPPADDING", (0,0), (-1,-1), 7), ("BOTTOMPADDING", (0,0), (-1,-1), 7),
]))
story += [summary, Spacer(1, 6 * mm), p("구현 변경", "KH2")]
for item in [
    "기존 카드형 3열을 무대 위 라인업 형태로 변경하고, 캐릭터 프레임과 포지션 번호를 중앙 시선축에 정렬했다.",
    "상단에는 화면 제목, 조작 안내, A/B 테마 전환, PARTY 3/3 상태를 배치해 정보 위계를 분리했다.",
    "역할과 레벨은 캐릭터 발밑으로 낮추고, 편성 완료 CTA는 우하단 고정 영역에 246 x 68 크기로 배치했다.",
    "캐릭터 슬롯과 선택 모달 버튼에 Highlighted/Pressed ColorBlock을 적용해 hover와 누름 상태를 명확히 했다.",
    "편성 완료 시 상단 안내 문구가 저장 완료 메시지로 바뀌도록 피드백을 추가했다.",
]: story.append(p("- " + item))

story += [p("Imagegen 에셋", "KH2")]
img_a = Image(str(ROOT / "Assets/Texture/UI/PartyFormation/party-stage-violet-a.png"), width=83 * mm, height=46.7 * mm)
img_b = Image(str(ROOT / "Assets/Texture/UI/PartyFormation/party-stage-teal-b.png"), width=83 * mm, height=46.7 * mm)
imgs = Table([[img_a, img_b], [p("A - 보랏빛 성역: 금색 강조와 따뜻한 마법 무대", "KSmall"), p("B - 청록빛 월야: 청록 강조와 차가운 달빛 무대", "KSmall")]], colWidths=[87 * mm, 87 * mm])
imgs.setStyle(TableStyle([("VALIGN", (0,0), (-1,-1), "TOP"), ("LEFTPADDING", (0,0), (-1,-1), 0), ("RIGHTPADDING", (0,0), (-1,-1), 4), ("TOPPADDING", (0,0), (-1,-1), 2)]))
section_bar = Table([[p("VISUAL QA - 1600 x 900", "KHeader")]], colWidths=[175 * mm], rowHeights=[9 * mm])
section_bar.setStyle(TableStyle([("BACKGROUND", (0,0), (-1,-1), navy), ("VALIGN", (0,0), (-1,-1), "MIDDLE"), ("LEFTPADDING", (0,0), (-1,-1), 8)]))
story += [imgs, PageBreak(), section_bar, Spacer(1, 5 * mm), p("Vision 검사와 수정", "KTitle")]

checks = [
    ("시각적 위계", "제목과 상태는 상단, 캐릭터는 중앙, CTA는 우하단으로 명확히 분리", "통과"),
    ("간격", "3개 슬롯의 중심 간격과 하단 정보 간격이 동일하며 16:9에서 겹침 없음", "통과"),
    ("대비/가독성", "상하단 네이비 셰이드와 흰색 본문, 금색/청록 강조로 배경 위 가독성 확보", "통과"),
    ("반응형", "상단/하단 요소는 모서리 앵커, 중앙 슬롯은 중앙 앵커를 사용", "통과"),
    ("hover/클릭", "모든 슬롯, 모달 선택지, 테마, 완료 버튼에 상태 색 적용", "통과"),
    ("모바일 사용성", "주 CTA 246 x 68, 모달 선택지 238 x 146으로 터치 영역을 크게 유지", "통과"),
    ("사용자 흐름", "슬롯 -> 모달 -> 선택 -> 테마 -> 완료 피드백 순서가 자연스럽게 동작", "통과"),
]
table_data = [[p("검사 기준", "KHeader"), p("확인 내용", "KHeader"), p("결과", "KHeader")]] + [[p(a,"KCell"),p(b,"KCell"),p(c,"KCellBold")] for a,b,c in checks]
check_table = Table(table_data, colWidths=[31 * mm, 126 * mm, 18 * mm], repeatRows=1)
check_table.setStyle(TableStyle([
    ("BACKGROUND", (0,0), (-1,0), navy), ("TEXTCOLOR", (0,0), (-1,0), colors.white),
    ("ROWBACKGROUNDS", (0,1), (-1,-1), [colors.white, light]), ("GRID", (0,0), (-1,-1), .35, colors.HexColor("#D9DEEA")),
    ("VALIGN", (0,0), (-1,-1), "TOP"), ("LEFTPADDING", (0,0), (-1,-1), 6), ("RIGHTPADDING", (0,0), (-1,-1), 6),
    ("TOPPADDING", (0,0), (-1,-1), 6), ("BOTTOMPADDING", (0,0), (-1,-1), 6),
]))
story += [check_table, Spacer(1, 5 * mm), p("수정 이력", "KH2")]
story += [
    p("1. 최초 B안에서 캐릭터 프레임은 청록으로 전환됐지만 CTA가 금색으로 남는 불일치를 발견했다."),
    p("2. 테마 적용 시 confirmButton.targetGraphic 색을 명시적으로 갱신하도록 수정했다."),
    p("3. 재실행 후 B안 CTA가 청록으로 표시되는 것을 1600 x 900 Play 모드에서 확인했다."),
]

story += [p("추출한 디자인 규칙", "KH2")]
rules = [
    [p("01", "KCellBold"), p("무대 우선", "KCellBold"), p("배경은 캐릭터를 돋보이게 하되 중앙과 하단에 충분한 음영과 여백을 둔다.", "KCell")],
    [p("02", "KCellBold"), p("정보는 발밑", "KCellBold"), p("이름, 역할, 레벨은 캐릭터 아래에 3단으로 쌓아 시선을 방해하지 않는다.", "KCell")],
    [p("03", "KCellBold"), p("CTA 고정", "KCellBold"), p("확정 행동은 우하단 고정, 보조 행동은 상단 중앙에 둔다.", "KCell")],
    [p("04", "KCellBold"), p("테마 단일 강조", "KCellBold"), p("A는 금색, B는 청록 한 색을 프레임, 받침대, CTA에 일관되게 적용한다.", "KCell")],
    [p("05", "KCellBold"), p("큰 터치 영역", "KCellBold"), p("모바일 가로 화면에서도 슬롯 전체를 클릭 영역으로 사용하고 작은 텍스트 링크를 피한다.", "KCell")],
]
rules_table = Table(rules, colWidths=[14 * mm, 35 * mm, 126 * mm])
rules_table.setStyle(TableStyle([
    ("BACKGROUND", (0,0), (0,-1), navy), ("TEXTCOLOR", (0,0), (0,-1), colors.white),
    ("GRID", (0,0), (-1,-1), .35, colors.HexColor("#D9DEEA")), ("VALIGN", (0,0), (-1,-1), "TOP"),
    ("LEFTPADDING", (0,0), (-1,-1), 6), ("RIGHTPADDING", (0,0), (-1,-1), 6),
    ("TOPPADDING", (0,0), (-1,-1), 7), ("BOTTOMPADDING", (0,0), (-1,-1), 7),
]))
story += [rules_table, Spacer(1, 5 * mm), p("검증 참고", "KH2"), p("Unity Play 모드 UI 흐름은 통과했다. 솔루션 전체 dotnet build는 기존 Firebase.App.Internal 프로젝트에서 FirebaseApp 형식을 찾지 못하는 6개 오류로 실패했으며, 이번 변경 파일과 무관한 선행 프로젝트 문제다.")]

doc.build(story, onFirstPage=header_footer, onLaterPages=header_footer)
print(OUT)

from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import Paragraph, SimpleDocTemplate, Spacer, Table, TableStyle


ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "output" / "pdf" / "deck-card-transfer-animation.pdf"
OUTPUT.parent.mkdir(parents=True, exist_ok=True)

pdfmetrics.registerFont(TTFont("Malgun", r"C:\Windows\Fonts\malgun.ttf"))
pdfmetrics.registerFont(TTFont("MalgunBold", r"C:\Windows\Fonts\malgunbd.ttf"))

styles = getSampleStyleSheet()
title = ParagraphStyle(
    "Title", parent=styles["Title"], fontName="MalgunBold", fontSize=19,
    leading=25, alignment=TA_CENTER, textColor=colors.HexColor("#17324D"),
    spaceAfter=5 * mm,
)
heading = ParagraphStyle(
    "Heading", parent=styles["Heading2"], fontName="MalgunBold", fontSize=12,
    leading=16, textColor=colors.HexColor("#17324D"), spaceBefore=3 * mm,
    spaceAfter=1.5 * mm,
)
body = ParagraphStyle(
    "Body", parent=styles["BodyText"], fontName="Malgun", fontSize=9,
    leading=14, textColor=colors.HexColor("#2A3B4D"), spaceAfter=1.5 * mm,
)
small = ParagraphStyle(
    "Small", parent=body, fontSize=8.2, leading=12,
)


def p(text, style=body):
    return Paragraph(text, style)


story = [
    p("Deck Collection 카드 이동 애니메이션", title),
    p("작성일: 2026-10-14 | Unity 6000.3.17f1 | Game View 1600 x 900"),
    p("구현 결과", heading),
    p(
        "CollectionCardView와 DeckViewEntry는 데이터를 직접 수정하지 않고 GameClientManager에 요청한다. "
        "Manager는 Collection UI 이벤트에서 시작 RectTransform과 Sprite를 받고, 데이터를 이동한 뒤 Deck UI 이벤트에서 도착 RectTransform을 받는다. "
        "두 UI 값이 준비되면 DeckCardTransferRequested를 발행하며 DeckCardTransferAnimation이 이를 구독한다. "
        "컴포넌트는 임시 이미지 생성, 목적지 숨김, UI 갱신 대기, 실제 Transform 위치 취득을 자체 처리해 임시 이미지를 곡선으로 이동시킨다. 이동 중 이미지는 축소되고 마지막 "
        "구간에서 페이드되며, 완료 또는 중단 시 DeckView 엔트리를 표시하고 임시 이미지를 제거한다."
    ),
    p("처리 흐름", heading),
]

flow_rows = [
    [p("1", heading), p("CollectionCardView 클릭", heading), p("카드 ID, 수량, 이미지 확인", heading)],
    [p("2"), p("시작 UI 요청"), p("Manager가 Collection UI 이벤트로 Transform과 Sprite 수신")],
    [p("3"), p("데이터 이동"), p("Manager에서 Collection 제거 후 Deck 추가")],
    [p("4"), p("도착 UI 요청"), p("Manager가 Deck UI 이벤트로 갱신된 Transform 수신")],
    [p("5"), p("이동 애니메이션 재생"), p("Collection 위치에서 숨겨진 DeckView 위치까지 이동")],
    [p("6"), p("DeckView 엔트리 표시"), p("Tween 완료/중단 시 엔트리 표시 및 임시 이미지 제거")],
]
flow = Table(flow_rows, colWidths=[12 * mm, 58 * mm, 95 * mm])
flow.setStyle(TableStyle([
    ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#DDEAF5")),
    ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, colors.HexColor("#F7F9FB")]),
    ("GRID", (0, 0), (-1, -1), 0.45, colors.HexColor("#B8C8D6")),
    ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
    ("ALIGN", (0, 0), (0, -1), "CENTER"),
    ("LEFTPADDING", (0, 0), (-1, -1), 6),
    ("RIGHTPADDING", (0, 0), (-1, -1), 6),
    ("TOPPADDING", (0, 0), (-1, -1), 4),
    ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
]))
story.extend([
    flow,
    p("애니메이션 설정", heading),
])

settings_rows = [
    [p("Duration"), p("0.40초", small), p("전체 이동 시간", small)],
    [p("ArcHeight"), p("120", small), p("중간 경로의 위쪽 곡선 높이", small)],
    [p("TargetScale"), p("0.55", small), p("DeckViewEntry 도착 시 이미지 크기", small)],
    [p("FadeStartRate"), p("0.65", small), p("전체 시간의 65% 지점부터 페이드", small)],
    [p("MovementEase"), p("OutCubic", small), p("빠르게 출발해 부드럽게 도착", small)],
]
settings = Table(settings_rows, colWidths=[45 * mm, 30 * mm, 90 * mm])
settings.setStyle(TableStyle([
    ("ROWBACKGROUNDS", (0, 0), (-1, -1), [colors.white, colors.HexColor("#F7F9FB")]),
    ("GRID", (0, 0), (-1, -1), 0.4, colors.HexColor("#C4D0DA")),
    ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
    ("LEFTPADDING", (0, 0), (-1, -1), 6),
    ("TOPPADDING", (0, 0), (-1, -1), 3),
    ("BOTTOMPADDING", (0, 0), (-1, -1), 3),
]))
story.extend([
    settings,
    p("변경 파일", heading),
    p(
        "- Assets/Scripts/System/GameClientData.cs: 카드 이동 요청, 데이터 변경, UI Transform 요청 이벤트 관리<br/>"
        "- Assets/Scripts/UI/Components/Animation/DeckCardTransferAnimation.cs: Manager의 최종 이동 이벤트 구독<br/>"
        "- Assets/Scripts/UI/View/DeckBuild/Collection/DeckCollectionView.cs: Collection Transform 요청 응답<br/>"
        "- Assets/Scripts/UI/View/DeckBuild/Deck/DeckView.cs: Deck Transform 요청 응답<br/>"
        "- CollectionCardView.cs / DeckViewEntry.cs: 데이터 직접 변경 대신 Manager 요청"
    ),
    p("검증", heading),
    p(
        "Unity가 새 스크립트를 프로젝트에 반영했으며 Console에 C# 컴파일 오류는 표시되지 않았다. "
        "Game View는 1600 x 900(16:9)에서 카드 컬렉션과 덱 목록이 모두 화면 안에 표시됐다. "
        "DeckBuildScene 단독 Play는 기존 GameClientManager 초기화가 없어 DeckView와 "
        "DeckCollectionView의 Awake에서 NullReference가 발생하므로 실제 클릭 재생은 정상 부트스트랩 "
        "진입 경로에서 추가 확인이 필요하다."
    ),
])

doc = SimpleDocTemplate(
    str(OUTPUT), pagesize=A4,
    leftMargin=20 * mm, rightMargin=20 * mm,
    topMargin=15 * mm, bottomMargin=15 * mm,
    title="Deck Collection 카드 이동 애니메이션",
    author="Codex",
)
doc.build(story)
print(OUTPUT)

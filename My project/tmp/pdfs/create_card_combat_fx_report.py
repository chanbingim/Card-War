from pathlib import Path
from reportlab.lib import colors
from reportlab.lib.enums import TA_LEFT, TA_CENTER
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, KeepTogether

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / "output" / "pdf" / "card-combat-fx-lab-report.pdf"
OUT.parent.mkdir(parents=True, exist_ok=True)

regular = Path(r"C:\Windows\Fonts\malgun.ttf")
bold = Path(r"C:\Windows\Fonts\malgunbd.ttf")
pdfmetrics.registerFont(TTFont("Malgun", str(regular)))
pdfmetrics.registerFont(TTFont("Malgun-Bold", str(bold)))

INK = colors.HexColor("#17181C")
GOLD = colors.HexColor("#C99632")
PALE = colors.HexColor("#F4F0E7")
RED = colors.HexColor("#9D3C34")
BLUE = colors.HexColor("#4E8291")
MUTED = colors.HexColor("#686A70")
LINE = colors.HexColor("#D8D2C5")

styles = getSampleStyleSheet()
styles.add(ParagraphStyle(name="KBody", fontName="Malgun", fontSize=9.2, leading=15, textColor=INK))
styles.add(ParagraphStyle(name="KSmall", fontName="Malgun", fontSize=7.5, leading=11, textColor=MUTED))
styles.add(ParagraphStyle(name="KTitle", fontName="Malgun-Bold", fontSize=27, leading=34, textColor=INK))
styles.add(ParagraphStyle(name="KH1", fontName="Malgun-Bold", fontSize=17, leading=23, textColor=INK, spaceAfter=8))
styles.add(ParagraphStyle(name="KH2", fontName="Malgun-Bold", fontSize=11, leading=16, textColor=INK))
styles.add(ParagraphStyle(name="KWhite", fontName="Malgun", fontSize=8.5, leading=13, textColor=colors.white))
styles.add(ParagraphStyle(name="KWhiteBold", fontName="Malgun-Bold", fontSize=13, leading=18, textColor=colors.white))
styles.add(ParagraphStyle(name="KCenter", fontName="Malgun-Bold", fontSize=9, leading=13, textColor=INK, alignment=TA_CENTER))

def header_footer(canvas, doc):
    canvas.saveState()
    canvas.setFillColor(INK)
    canvas.rect(0, A4[1]-13*mm, A4[0], 13*mm, fill=1, stroke=0)
    canvas.setFont("Malgun-Bold", 8)
    canvas.setFillColor(colors.HexColor("#E5C16F"))
    canvas.drawString(18*mm, A4[1]-8.5*mm, "CARD COMBAT FX LAB")
    canvas.setFont("Malgun", 7.5)
    canvas.setFillColor(MUTED)
    canvas.drawString(18*mm, 10*mm, "HTML INTERACTION PROTOTYPE · 2026-10-21")
    canvas.drawRightString(A4[0]-18*mm, 10*mm, f"{doc.page}")
    canvas.restoreState()

def pill(text, color=GOLD):
    t = Table([[Paragraph(text, ParagraphStyle(name=f"pill{text}", fontName="Malgun-Bold", fontSize=7, textColor=colors.white, alignment=TA_CENTER))]], colWidths=[31*mm], rowHeights=[8*mm])
    t.setStyle(TableStyle([("BACKGROUND",(0,0),(-1,-1),color),("VALIGN",(0,0),(-1,-1),"MIDDLE"),("BOX",(0,0),(-1,-1),0.4,color)]))
    return t

def feature_card(title, body, accent):
    data = [[Paragraph(title, styles["KH2"])],[Paragraph(body, styles["KSmall"])]]
    t = Table(data, colWidths=[51*mm], rowHeights=[10*mm,27*mm])
    t.setStyle(TableStyle([
        ("BACKGROUND",(0,0),(-1,-1),colors.white),("BOX",(0,0),(-1,-1),0.7,LINE),
        ("LINEABOVE",(0,0),(-1,0),3,accent),("LEFTPADDING",(0,0),(-1,-1),4*mm),("RIGHTPADDING",(0,0),(-1,-1),4*mm),
        ("TOPPADDING",(0,0),(-1,-1),3*mm),("VALIGN",(0,0),(-1,-1),"TOP")]))
    return t

doc = SimpleDocTemplate(str(OUT), pagesize=A4, rightMargin=18*mm, leftMargin=18*mm, topMargin=22*mm, bottomMargin=18*mm,
                        title="Card Combat FX Lab 구현 보고서", author="Codex")
story = []

story += [Spacer(1,12*mm), pill("IMPLEMENTATION REPORT"), Spacer(1,8*mm),
          Paragraph("카드 전투 타격 연출<br/>인터랙티브 프로토타입", styles["KTitle"]), Spacer(1,4*mm),
          Paragraph("Slay the Spire 계열 카드 전투의 손맛을 빠르게 실험할 수 있도록, 카메라 쉐이크와 타격 피드백을 브라우저에서 즉시 조절하는 HTML 도구를 구현했습니다.", styles["KBody"]), Spacer(1,10*mm)]

hero = Table([
    [Paragraph("전투 화면", styles["KWhiteBold"]), Paragraph("연출 제어와 데이터", styles["KWhiteBold"])],
    [Paragraph("16:9 전투 무대 · 카드 3종 · 체력/피해 반영 · 공격 애니메이션", styles["KWhite"]), Paragraph("쉐이크 기법 4종 · 13포인트 엔벨로프 · JSON/CSV/XML · 단일 HTML 소스 내보내기", styles["KWhite"])],
], colWidths=[86*mm,86*mm], rowHeights=[15*mm,31*mm])
hero.setStyle(TableStyle([("BACKGROUND",(0,0),(0,-1),INK),("BACKGROUND",(1,0),(1,-1),RED),("VALIGN",(0,0),(-1,-1),"TOP"),("LEFTPADDING",(0,0),(-1,-1),7*mm),("RIGHTPADDING",(0,0),(-1,-1),7*mm),("TOPPADDING",(0,0),(-1,-1),5*mm),("BOX",(0,0),(-1,-1),0.6,colors.HexColor("#4A4240"))]))
story += [hero, Spacer(1,10*mm), Paragraph("핵심 목표", styles["KH1"])]

goals = [
    ("01", "공식과 연결된 카메라 피드백", "기법을 선택하면 전용 수식과 추천 카메라 값이 실제 움직임에 적용되고 즉시 미리보기가 재생됩니다."),
    ("02", "데이터 기반 튜닝", "강도, 지속 시간, 주파수, 회전, 줌과 13개 진폭 포인트를 편집해 같은 공격의 체감 차이를 비교합니다."),
    ("03", "이식 가능한 프로필", "현재 설정, 축 편차, 파형 레이어, 엔벨로프를 JSON, CSV, XML로 저장해 Unity 구현 데이터로 활용할 수 있습니다."),
]
rows=[]
for n,t,b in goals:
    rows.append([Paragraph(n, ParagraphStyle(name=f"n{n}",fontName="Malgun-Bold",fontSize=17,textColor=GOLD)), Paragraph(f"<b>{t}</b><br/>{b}", styles["KBody"])])
goal_table=Table(rows,colWidths=[18*mm,154*mm],rowHeights=[25*mm]*3)
goal_table.setStyle(TableStyle([("VALIGN",(0,0),(-1,-1),"MIDDLE"),("LINEBELOW",(0,0),(-1,-2),0.5,LINE),("LEFTPADDING",(0,0),(-1,-1),0),("RIGHTPADDING",(0,0),(-1,-1),2*mm)]))
story += [goal_table, PageBreak()]

story += [Paragraph("구현된 기능", styles["KH1"]), Paragraph("사용자가 조절하는 값과 엔벨로프 데이터가 절차적 카메라 계산에 반영되며, 공격 실행 시 활성화된 효과만 하나의 타격 시퀀스로 합성됩니다.", styles["KBody"]), Spacer(1,6*mm)]
cards = [
    feature_card("4가지 쉐이크 기법", "각 알고리즘의 계산식, 추천 강도/시간/주파수/회전/줌, 체감 효과와 추천 사용처를 함께 제공", GOLD),
    feature_card("엔벨로프 데이터", "기본 13포인트 감쇠 곡선, 부드러운 보간, 포인트 추가/편집/삭제, SVG 실시간 그래프", RED),
    feature_card("파일 내보내기", "프로필은 JSON/CSV/XML로 저장하고, 전체 도구는 CSS와 JavaScript가 포함된 단일 HTML로 다운로드", BLUE),
]
story += [Table([cards], colWidths=[57.3*mm]*3, style=TableStyle([("VALIGN",(0,0),(-1,-1),"TOP"),("LEFTPADDING",(0,0),(-1,-1),0),("RIGHTPADDING",(0,0),(-1,-1),3*mm)])), Spacer(1,10*mm)]

story += [Paragraph("공격 시퀀스", styles["KH1"])]
flow_data=[]
steps=[("1","카드 선택"),("2","검사 돌진"),("3","타격 합성"),("4","피해 반영"),("5","카메라 복귀")]
for i,(n,label) in enumerate(steps):
    flow_data.append([Paragraph(f"<font color='#C99632'><b>{n}</b></font><br/>{label}", styles["KCenter"])])
flow=Table([sum(flow_data,[])],colWidths=[34.4*mm]*5,rowHeights=[25*mm])
flow.setStyle(TableStyle([("BACKGROUND",(0,0),(-1,-1),PALE),("BOX",(0,0),(-1,-1),0.7,LINE),("INNERGRID",(0,0),(-1,-1),0.5,LINE),("VALIGN",(0,0),(-1,-1),"MIDDLE")]))
story += [flow, Spacer(1,10*mm), Paragraph("파일 구성", styles["KH1"])]
file_rows=[
    ["dist/index.html","전투 무대, 카드, 제어 패널의 시맨틱 마크업"],
    ["dist/styles.css","반응형 레이아웃, CSS 캐릭터 아트, 키프레임 연출"],
    ["dist/app.js","다중 파형 쉐이크, 엔벨로프 편집, 직렬화/내보내기, 전투 상태"],
    ["dist/card-combat-fx-lab-standalone.html","CSS와 JavaScript를 포함해 더블클릭으로 실행되는 단일 HTML"],
    ["build-standalone.ps1","분리된 소스를 배포 가능한 단일 HTML로 재생성"],
    [".openai/hosting.json","정적 사이트 배포 디렉터리 선언"],
    ["README.md","로컬 실행법과 설정 변경 위치"],
]
ft=Table([[Paragraph(f"<b>{a}</b>",styles["KSmall"]),Paragraph(b,styles["KSmall"])] for a,b in file_rows],colWidths=[65*mm,107*mm],rowHeights=[10*mm]*7)
ft.setStyle(TableStyle([("BACKGROUND",(0,0),(0,-1),colors.HexColor("#EEE8DB")),("BOX",(0,0),(-1,-1),0.5,LINE),("INNERGRID",(0,0),(-1,-1),0.4,LINE),("VALIGN",(0,0),(-1,-1),"MIDDLE"),("LEFTPADDING",(0,0),(-1,-1),4*mm)]))
story += [ft, PageBreak()]

story += [Paragraph("검증 결과", styles["KH1"]), Paragraph("로컬 정적 서버와 1440 × 810 브라우저 뷰포트에서 기능 및 가시성을 확인했습니다.", styles["KBody"]), Spacer(1,6*mm)]
checks=[
    ("PASS","JavaScript 구문","Node.js --check 통과",GOLD),
    ("PASS","정적 서버","HTTP 200 응답 확인",BLUE),
    ("PASS","16:9 레이아웃","전투 무대와 우측 제어 패널 2열 유지",GOLD),
    ("PASS","기법 선택","4개 알고리즘별 공식/추천값/체감 정보 전환",BLUE),
    ("PASS","프로필 편집","13 → 14 포인트 추가와 그래프 갱신",GOLD),
    ("PASS","전투 상태","84 → 66 피해 반영, 초기화 시 84 복원",RED),
    ("PASS","프로필 내보내기","JSON, CSV, XML 다운로드 성공",GOLD),
    ("PASS","단일 HTML","외부 CSS/JS 의존성 없음, 다운로드 버튼 성공",BLUE),
]
check_rows=[]
for status,name,result,color in checks:
    check_rows.append([Paragraph(f"<font color='{color.hexval()}'><b>{status}</b></font>",styles["KSmall"]),Paragraph(f"<b>{name}</b>",styles["KSmall"]),Paragraph(result,styles["KSmall"])])
ct=Table(check_rows,colWidths=[24*mm,52*mm,96*mm],rowHeights=[12*mm]*8)
ct.setStyle(TableStyle([("BOX",(0,0),(-1,-1),0.6,LINE),("INNERGRID",(0,0),(-1,-1),0.4,LINE),("BACKGROUND",(0,0),(0,-1),INK),("VALIGN",(0,0),(-1,-1),"MIDDLE"),("LEFTPADDING",(0,0),(-1,-1),4*mm)]))
story += [ct, Spacer(1,10*mm), Paragraph("조작 가이드", styles["KH1"])]
guide = Table([
    [Paragraph("1", styles["KCenter"]), Paragraph("손패에서 공격 카드를 선택합니다.", styles["KBody"])],
    [Paragraph("2", styles["KCenter"]), Paragraph("쉐이크 기법을 선택해 공식, 추천값, 체감 효과를 적용하고 비교합니다.", styles["KBody"])],
    [Paragraph("3", styles["KCenter"]), Paragraph("공격 실행 또는 Space 키로 절차적 쉐이크를 재생합니다.", styles["KBody"])],
    [Paragraph("4", styles["KCenter"]), Paragraph("완성한 프로필을 JSON, CSV, XML 중 원하는 형식으로 저장합니다.", styles["KBody"])],
    [Paragraph("5", styles["KCenter"]), Paragraph("HTML 코드 받기로 전체 프로토타입을 단일 파일로 저장하고 더블클릭해 실행합니다.", styles["KBody"])],
], colWidths=[14*mm,158*mm], rowHeights=[13*mm]*5)
guide.setStyle(TableStyle([("BACKGROUND",(0,0),(0,-1),PALE),("LINEBELOW",(0,0),(-1,-2),0.5,LINE),("VALIGN",(0,0),(-1,-1),"MIDDLE"),("LEFTPADDING",(1,0),(1,-1),5*mm)]))
story += [guide, Spacer(1,9*mm), Paragraph("배포 상태", styles["KH1"]), Paragraph("사이트 프로젝트는 생성되었지만 원격 소스 푸시 권한이 승인되지 않아 공개/비공개 배포는 수행하지 않았습니다. 로컬 결과물은 완성 및 검증된 상태입니다.", styles["KBody"])]

doc.build(story, onFirstPage=header_footer, onLaterPages=header_footer)
print(OUT)

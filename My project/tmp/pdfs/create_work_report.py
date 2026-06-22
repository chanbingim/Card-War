from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.platypus import Paragraph, SimpleDocTemplate, Spacer, Table, TableStyle


ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / "output" / "pdf" / "turn-card-game-work-report.pdf"


def section(title, body):
    return [Paragraph(title, styles["Heading2"]), Paragraph(body, styles["BodyText"]), Spacer(1, 5 * mm)]


styles = getSampleStyleSheet()
styles.add(ParagraphStyle(name="Small", parent=styles["BodyText"], fontSize=8, leading=10))

doc = SimpleDocTemplate(
    str(OUT),
    pagesize=A4,
    rightMargin=18 * mm,
    leftMargin=18 * mm,
    topMargin=18 * mm,
    bottomMargin=18 * mm,
)

story = [
    Paragraph("Turn Card Game Prototype - Work Report", styles["Title"]),
    Spacer(1, 6 * mm),
]

story += section(
    "Frontend agent summary",
    "Implemented runtime UGUI flow for Start, Stage Select, Battle, recorded card actions, combat resolution, and Stage Cleared return to lobby. "
    "The UI is created automatically by GameAppBootstrap in the active scene and scales from a 1920x1080 reference resolution.",
)

story += section(
    "GameLogic agent summary",
    "Implemented ScriptableObject data models for cards, monsters, and stages. Added GameSession as a small state machine covering Start, StageSelect, PlayerTurn, ResolvingCombat, and StageCleared. "
    "The session draws two cards per player turn, trims hand size to five, records card actions, resolves player actions before monster attacks, and clears the stage when all monsters are defeated.",
)

files = [
    ["Area", "Files"],
    ["Data", "Assets/Scripts/Data/CardData.cs<br/>Assets/Scripts/Data/MonsterData.cs<br/>Assets/Scripts/Data/StageData.cs"],
    ["Game", "Assets/Scripts/Game/CardActionRecord.cs<br/>Assets/Scripts/Game/GamePhase.cs<br/>Assets/Scripts/Game/GameSession.cs"],
    ["UI", "Assets/Scripts/UI/Components/PrimaryButton.cs<br/>Assets/Scripts/UI/Screens/GameAppBootstrap.cs"],
    ["Tests", "Assets/Tests/EditMode/GameSessionTests.cs<br/>Assets/Tests/EditMode/TurnCardGame.Tests.asmdef"],
    ["Docs", "README.md<br/>output/pdf/turn-card-game-work-report.pdf"],
]
files = [[Paragraph(str(cell), styles["Small"]) for cell in row] for row in files]
table = Table(files, colWidths=[28 * mm, 124 * mm])
table.setStyle(
    TableStyle(
        [
            ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#1f3b4d")),
            ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
            ("GRID", (0, 0), (-1, -1), 0.25, colors.HexColor("#b8c2cc")),
            ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
            ("FONTNAME", (0, 1), (-1, -1), "Helvetica"),
            ("FONTSIZE", (0, 0), (-1, -1), 8),
            ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, colors.HexColor("#f5f7f9")]),
        ]
    )
)
story += [Paragraph("Changed files", styles["Heading2"]), table, Spacer(1, 6 * mm)]

story += section(
    "Verification result",
    "dotnet build Assembly-CSharp.csproj --no-restore completed successfully with 0 errors. Warnings came from Unity package cache dependencies. "
    "Unity batchmode EditMode test execution could not produce a result XML because multiple Unity editor processes were already running for the project, so those processes were left untouched.",
)

story += section(
    "Remaining risks",
    "The runtime UI has not been visually inspected inside a live 16:9 Game view during this run. The sample stage data is generated at runtime and should later be replaced with authored ScriptableObject assets. "
    "Unity Test Runner should be re-run after the active editor processes are closed or after the project reloads the new test asmdef.",
)

doc.build(story)
print(OUT)

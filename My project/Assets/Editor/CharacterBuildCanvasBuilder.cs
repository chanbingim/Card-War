using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public static class CharacterBuildCanvasBuilder
{
    static readonly Color Navy = new(0.025f,0.04f,0.1f,1), Panel = new(0.075f,0.09f,0.18f,1), Gold = new(0.96f,0.7f,0.22f,1), White = new(0.93f,0.95f,1,1);
    static readonly string[] Names = { "아리엘", "레이븐", "세라", "카인", "루나", "에린" };
    static readonly string[] Roles = { "수호 · FRONT", "공격 · MIDDLE", "지원 · BACK", "공격 · FRONT", "마법 · MIDDLE", "치유 · BACK" };
    static readonly Color[] Colors = { new(0.25f,0.58f,0.9f), new(0.78f,0.28f,0.45f), new(0.35f,0.76f,0.56f), new(0.88f,0.45f,0.18f), new(0.46f,0.34f,0.84f), new(0.24f,0.75f,0.76f) };

    [MenuItem("Tools/Haness/Build Character Party UI")]
    public static void Build()
    {
        GameObject host = Resources.FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(x => (x.name == "CharacterBuildCanvas" || x.name == "ChracterBuildCanvas") && x.scene.IsValid());
        if (!host) { Debug.LogError("CharacterBuildCanvas not found"); return; }
        GameObject misplaced = GameObject.Find("PartyBuildRoot");
        if (misplaced && misplaced.transform.parent != host.transform) Object.DestroyImmediate(misplaced);
        Transform old = host.transform.Find("PartyBuildRoot"); if (old) Object.DestroyImmediate(old.gameObject);
        GameObject root = Box("PartyBuildRoot", host.transform, Navy); Stretch(root.GetComponent<RectTransform>());
        CharacterBuildPartyUI ui = root.AddComponent<CharacterBuildPartyUI>();

        Text title = Txt("Title", root.transform, "파티 편성", 40, FontStyle.Bold, White, TextAnchor.MiddleLeft);
        Rect(title.gameObject, new(.5f,1), new(0,-42), new(1120,55), new(.5f,1));
        Text guide = Txt("Guide", root.transform, "전투에 참여할 최대 3명의 캐릭터를 선택하세요", 17, FontStyle.Normal, new(.65f,.7f,.82f), TextAnchor.MiddleLeft);
        Rect(guide.gameObject, new(.5f,1), new(0,-91), new(1120,32), new(.5f,1));
        GameObject limit = Box("PartyLimit", root.transform, new(.18f,.14f,.3f,1)); Rect(limit,new(1,1),new(-44,-50),new(170,42),new(1,1)); FillText(limit.transform,"PARTY  3 / 3",16,Gold);

        var slots = new CharacterBuildPartyUI.Slot[3]; float[] xs={-350,0,350};
        for(int i=0;i<3;i++)
        {
            GameObject card=Box($"CharacterSlot_{i+1}",root.transform,Panel); Rect(card,new(.5f,.52f),new(xs[i],5),new(300,410));
            Button b=card.AddComponent<Button>(); b.targetGraphic=card.GetComponent<Image>(); ColorBlock cb=b.colors; cb.highlightedColor=new(.15f,.18f,.34f,1); b.colors=cb;
            GameObject num=Box("Position",card.transform,Gold); Rect(num,new(0,1),new(18,-18),new(38,38),new(0,1)); FillText(num.transform,(i+1).ToString(),18,Navy);
            GameObject ring=Box("PortraitRing",card.transform,Gold); Rect(ring,new(.5f,1),new(0,-65),new(190,190),new(.5f,1));
            GameObject portrait=Box("CharacterPortrait",ring.transform,Colors[i]); Rect(portrait,new(.5f,.5f),Vector2.zero,new(174,174)); FillText(portrait.transform,Names[i][0].ToString(),70,White);
            Text name=Txt("CharacterName",card.transform,Names[i],28,FontStyle.Bold,White,TextAnchor.MiddleCenter); Rect(name.gameObject,new(.5f,0),new(0,112),new(250,40),new(.5f,0));
            Text role=Txt("Role",card.transform,Roles[i],16,FontStyle.Bold,Gold,TextAnchor.MiddleCenter); Rect(role.gameObject,new(.5f,0),new(0,76),new(250,28),new(.5f,0));
            Text level=Txt("Level",card.transform,"Lv. 30     ◆ ◆ ◆ ◆ ◆",14,FontStyle.Normal,new(.7f,.75f,.9f),TextAnchor.MiddleCenter); Rect(level.gameObject,new(.5f,0),new(0,45),new(250,26),new(.5f,0));
            Text hint=Txt("Hint",card.transform,"캐릭터를 눌러 변경",13,FontStyle.Normal,new(.52f,.58f,.72f),TextAnchor.MiddleCenter); Rect(hint.gameObject,new(.5f,0),new(0,14),new(250,24),new(.5f,0));
            slots[i]=new CharacterBuildPartyUI.Slot{button=b,nameText=name,roleText=role,portrait=portrait.GetComponent<Image>()};
        }

        GameObject bottom=Box("PartySummary",root.transform,new(.05f,.065f,.14f,1)); Rect(bottom,new(.5f,0),new(0,18),new(1120,82),new(.5f,0));
        Text formation=Txt("Formation",bottom.transform,"FORMATION   1 FRONT  ·  1 MIDDLE  ·  1 BACK",16,FontStyle.Bold,new(.72f,.78f,.9f),TextAnchor.MiddleLeft); Rect(formation.gameObject,new(0,.5f),new(26,0),new(600,44),new(0,.5f));
        GameObject done=Box("ConfirmButton",bottom.transform,Gold); Rect(done,new(1,.5f),new(-22,0),new(220,52),new(1,.5f)); done.AddComponent<Button>().targetGraphic=done.GetComponent<Image>(); FillText(done.transform,"편성 완료",20,Navy);

        GameObject selector=Box("CharacterSelector",root.transform,new(.01f,.015f,.04f,.92f)); Stretch(selector.GetComponent<RectTransform>());
        GameObject modal=Box("Modal",selector.transform,Panel); Rect(modal,new(.5f,.5f),Vector2.zero,new(840,520));
        Text st=Txt("SelectorTitle",modal.transform,"파티 캐릭터 선택",30,FontStyle.Bold,White,TextAnchor.MiddleLeft); Rect(st.gameObject,new(.5f,1),new(0,-28),new(760,50),new(.5f,1));
        Button[] options=new Button[6];
        for(int i=0;i<6;i++)
        {
            int row=i/3,col=i%3; GameObject o=Box($"CharacterOption_{i+1}",modal.transform,new(.11f,.13f,.24f,1)); Rect(o,new(.5f,1),new((col-1)*250,-112-row*172),new(220,142),new(.5f,1));
            options[i]=o.AddComponent<Button>(); options[i].targetGraphic=o.GetComponent<Image>();
            GameObject p=Box("Portrait",o.transform,Colors[i]); Rect(p,new(0,.5f),new(16,0),new(82,104),new(0,.5f)); FillText(p.transform,Names[i][0].ToString(),38,White);
            Text n=Txt("Name",o.transform,Names[i],21,FontStyle.Bold,White,TextAnchor.MiddleLeft); Rect(n.gameObject,new(0,.5f),new(112,17),new(95,32),new(0,.5f));
            Text ok=Txt("Available",o.transform,"선택 가능",13,FontStyle.Normal,Gold,TextAnchor.MiddleLeft); Rect(ok.gameObject,new(0,.5f),new(112,-17),new(95,26),new(0,.5f));
        }
        GameObject close=Box("Close",modal.transform,new(.22f,.24f,.34f,1)); Rect(close,new(1,1),new(-22,-22),new(46,46),new(1,1)); Button closeB=close.AddComponent<Button>(); closeB.targetGraphic=close.GetComponent<Image>(); FillText(close.transform,"×",27,White); closeB.onClick.AddListener(()=>selector.SetActive(false));

        SerializedObject so=new(ui); SerializedProperty sp=so.FindProperty("slots"); sp.arraySize=3;
        for(int i=0;i<3;i++){ var e=sp.GetArrayElementAtIndex(i); e.FindPropertyRelative("button").objectReferenceValue=slots[i].button; e.FindPropertyRelative("nameText").objectReferenceValue=slots[i].nameText; e.FindPropertyRelative("roleText").objectReferenceValue=slots[i].roleText; e.FindPropertyRelative("portrait").objectReferenceValue=slots[i].portrait; }
        so.FindProperty("selector").objectReferenceValue=selector; so.FindProperty("selectorTitle").objectReferenceValue=st;
        var op=so.FindProperty("options"); op.arraySize=6; for(int i=0;i<6;i++)op.GetArrayElementAtIndex(i).objectReferenceValue=options[i];
        var cp=so.FindProperty("characters"); cp.arraySize=6; for(int i=0;i<6;i++){var c=cp.GetArrayElementAtIndex(i); c.FindPropertyRelative("characterName").stringValue=Names[i]; c.FindPropertyRelative("role").stringValue=Roles[i]; c.FindPropertyRelative("color").colorValue=Colors[i];} so.ApplyModifiedPropertiesWithoutUndo();
        selector.SetActive(false); EditorSceneManager.MarkSceneDirty(host.scene); EditorSceneManager.SaveScene(host.scene); Selection.activeGameObject=root; Debug.Log("CharacterBuild party UI created.");
    }

    static GameObject UI(string n,Transform p){var g=new GameObject(n,typeof(RectTransform));g.layer=5;g.transform.SetParent(p,false);return g;}
    static GameObject Box(string n,Transform p,Color c){var g=UI(n,p);g.AddComponent<CanvasRenderer>();g.AddComponent<Image>().color=c;return g;}
    static Text Txt(string n,Transform p,string v,int s,FontStyle fs,Color c,TextAnchor a){var g=UI(n,p);g.AddComponent<CanvasRenderer>();var t=g.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=v;t.fontSize=s;t.fontStyle=fs;t.color=c;t.alignment=a;t.raycastTarget=false;return t;}
    static void FillText(Transform p,string v,int s,Color c){var t=Txt("Label",p,v,s,FontStyle.Bold,c,TextAnchor.MiddleCenter);Stretch(t.rectTransform);}
    static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
    static void Rect(GameObject g,Vector2 a,Vector2 pos,Vector2 size,Vector2? pivot=null){var r=g.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=a;r.pivot=pivot??new(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;}
}

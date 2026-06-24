#if UNITY_EDITOR
using System.Collections.Generic;
using TurnCardGame.Data;
using TurnCardGame.Game;
using TurnCardGame.UI.Screens;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurnCardGame.EditorTools
{
    public static class TurnCardGameSceneBuilder
    {
        private const string DataRoot = "Assets/Data";
        private const string SceneRoot = "Assets/Scenes";

        [MenuItem("Turn Card Game/Build Playable Scenes")]
        public static void BuildPlayableScenes()
        {
            EnsureFolders();

            CardData strike = CreateOrLoadCard("Strike", "strike", "Strike", CardEffectType.Damage, 6, "Deal 6 damage.");
            CardData guard = CreateOrLoadCard("Guard", "guard", "Guard", CardEffectType.Guard, 3, "Gain 3 guard.");
            CardData mend = CreateOrLoadCard("Mend", "mend", "Mend", CardEffectType.Heal, 4, "Restore 4 health.");

            MonsterData slime = CreateOrLoadMonster("TrainingSlime", "slime", "Training Slime", 14, 2);
            MonsterData knight = CreateOrLoadMonster("RustKnight", "knight", "Rust Knight", 20, 4);
            MonsterData sentinel = CreateOrLoadMonster("ArchiveSentinel", "sentinel", "Archive Sentinel", 28, 5);

            StageData training = CreateOrLoadStage("Stage01TrainingField", "stage_001", "Stage 1 - Training Field", new[] { slime }, new[] { strike, strike, guard, mend });
            StageData gate = CreateOrLoadStage("Stage02OldGate", "stage_002", "Stage 2 - Old Gate", new[] { knight }, new[] { strike, strike, strike, guard, mend });
            StageData archive = CreateOrLoadStage("Stage03ArchiveHall", "stage_003", "Stage 3 - Archive Hall", new[] { sentinel }, new[] { strike, strike, guard, guard, mend });

            BuildScene(SceneNames.Start, typeof(StartScreenController), new[] { training, gate, archive }, includeRuntime: true);
            BuildScene(SceneNames.StageSelect, typeof(StageSelectScreenController), new[] { training, gate, archive }, includeRuntime: false);
            BuildScene(SceneNames.Battle, typeof(BattleScreenController), new[] { training, gate, archive }, includeRuntime: false);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene($"{SceneRoot}/{SceneNames.Start}.unity", true),
                new EditorBuildSettingsScene($"{SceneRoot}/{SceneNames.StageSelect}.unity", true),
                new EditorBuildSettingsScene($"{SceneRoot}/{SceneNames.Battle}.unity", true),
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Turn Card Game playable scenes generated.");
        }

        private static void BuildScene(string sceneName, System.Type controllerType, IReadOnlyList<StageData> stages, bool includeRuntime)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = sceneName;

            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.94f, 0.96f, 0.96f);
            camera.orthographic = true;
            camera.orthographicSize = 5f;

            var controllerObject = new GameObject(sceneName + " Controller");
            controllerObject.AddComponent(controllerType);

            if (includeRuntime)
            {
                var runtimeObject = new GameObject("Game Runtime");
                GameRuntime runtime = runtimeObject.AddComponent<GameRuntime>();
                SerializedObject serializedRuntime = new SerializedObject(runtime);
                SerializedProperty stagesProperty = serializedRuntime.FindProperty("configuredStages");
                stagesProperty.arraySize = stages.Count;
                for (int i = 0; i < stages.Count; i++)
                {
                    stagesProperty.GetArrayElementAtIndex(i).objectReferenceValue = stages[i];
                }

                serializedRuntime.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.SaveScene(scene, $"{SceneRoot}/{sceneName}.unity");
        }

        private static CardData CreateOrLoadCard(string assetName, string id, string title, CardEffectType effectType, int power, string description)
        {
            string path = $"{DataRoot}/Cards/{assetName}.asset";
            CardData card = LoadOrCreate<CardData>(path);
            SerializedObject serialized = new SerializedObject(card);
            serialized.FindProperty("cardId").stringValue = id;
            serialized.FindProperty("title").stringValue = title;
            serialized.FindProperty("effectType").enumValueIndex = (int)effectType;
            serialized.FindProperty("power").intValue = power;
            serialized.FindProperty("description").stringValue = description;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(card);
            return card;
        }

        private static MonsterData CreateOrLoadMonster(string assetName, string id, string title, int maxHealth, int attackPower)
        {
            string path = $"{DataRoot}/Monsters/{assetName}.asset";
            MonsterData monster = LoadOrCreate<MonsterData>(path);
            SerializedObject serialized = new SerializedObject(monster);
            serialized.FindProperty("monsterId").stringValue = id;
            serialized.FindProperty("title").stringValue = title;
            serialized.FindProperty("maxHealth").intValue = maxHealth;
            serialized.FindProperty("attackPower").intValue = attackPower;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(monster);
            return monster;
        }

        private static StageData CreateOrLoadStage(string assetName, string id, string title, IReadOnlyList<MonsterData> monsters, IReadOnlyList<CardData> deck)
        {
            string path = $"{DataRoot}/Stages/{assetName}.asset";
            StageData stage = LoadOrCreate<StageData>(path);
            SerializedObject serialized = new SerializedObject(stage);
            serialized.FindProperty("stageId").stringValue = id;
            serialized.FindProperty("title").stringValue = title;
            SetObjectArray(serialized.FindProperty("monsters"), monsters);
            SetObjectArray(serialized.FindProperty("startingDeck"), deck);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(stage);
            return stage;
        }

        private static void SetObjectArray<T>(SerializedProperty property, IReadOnlyList<T> values) where T : Object
        {
            property.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "Data");
            EnsureFolder(DataRoot, "Cards");
            EnsureFolder(DataRoot, "Monsters");
            EnsureFolder(DataRoot, "Stages");
            EnsureFolder("Assets", "Scenes");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
#endif

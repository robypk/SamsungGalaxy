using UnityEngine;
using UnityEditor;
using System.IO;

public class CardDataBatchCreator : EditorWindow
{
    private Texture2D spriteSheet;
    private string outputFolder = "Assets/CardData";
    private int startCardId = 1;

    [MenuItem("Tools/Card Data/Batch Create CardData")]
    public static void OpenWindow()
    {
        GetWindow<CardDataBatchCreator>("CardData Batch Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Create CardData SOs", EditorStyles.boldLabel);

        spriteSheet = (Texture2D)EditorGUILayout.ObjectField(
            "Sprite Sheet",
            spriteSheet,
            typeof(Texture2D),
            false
        );

        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
        startCardId = EditorGUILayout.IntField("Start Card ID", startCardId);

        GUILayout.Space(10);

        if (GUILayout.Button("Create CardData Assets"))
        {
            CreateCardDataAssets();
        }
    }

    private void CreateCardDataAssets()
    {
        if (spriteSheet == null)
        {
            Debug.LogError("Sprite Sheet not assigned!");
            return;
        }

        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
            AssetDatabase.Refresh();
        }

        string sheetPath = AssetDatabase.GetAssetPath(spriteSheet);

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(sheetPath);

        int cardId = startCardId;
        int createdCount = 0;

        foreach (Object obj in assets)
        {
            if (obj is Sprite sprite)
            {
                CardData cardData = ScriptableObject.CreateInstance<CardData>();
                SerializedObject so = new SerializedObject(cardData);
                so.FindProperty("cardId").intValue = cardId;
                so.FindProperty("cardImage").objectReferenceValue = sprite;
                so.ApplyModifiedProperties();

                string assetPath = $"{outputFolder}/CardData_{cardId}.asset";
                AssetDatabase.CreateAsset(cardData, assetPath);

                cardId++;
                createdCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

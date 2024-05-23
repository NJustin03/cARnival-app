using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TextPrefabScript : MonoBehaviour
{
    public string Text;
    public Sprite BackgroundImage = null;

    private void OnValidate()
    {
       var TMPObject = gameObject.GetComponentInChildren<TextMeshProUGUI>();
       var BackImg = gameObject.GetComponentInChildren<Image>();

        TMPObject.text = Text;
        if (BackgroundImage == null)
        {
            BackImg.color = Color.clear;
        }
        else
        {
            BackImg.color = Color.white;
        }

        BackImg.sprite = BackgroundImage;
    }
#if UNITY_EDITOR
    [MenuItem("GameObject/ELLE/Text", false)]

    private static void Create(MenuCommand menuCommand)
    {
        var uiElementGameObject = CreateUIElement();

        GameObjectUtility.SetParentAndAlign(uiElementGameObject, Selection.activeGameObject);

        Selection.activeGameObject = uiElementGameObject;
    }

    private static GameObject CreateUIElement()
    {
        const string prefabPath = "Assets/Prefabs/Text Prefab.prefab";

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        var gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        gameObject.name = prefab.name;

        Undo.RegisterCreatedObjectUndo(gameObject, $"Created {gameObject.name}");

        return gameObject;
    }
#endif
}

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

        if (TMPObject != null)
            TMPObject.text = Text;

        if (BackImg != null)
            BackImg.color = BackgroundImage == null ? Color.clear : Color.white;

        BackImg.sprite = BackgroundImage;
    }
#if UNITY_EDITOR
    [MenuItem("GameObject/ELLE/Text Bold", false)]
    private static void CreateBoldText(MenuCommand menuCommand)
    {
        Create("Assets/Prefabs/Text (Blue, Bold) Prefab.prefab");
    }

    [MenuItem("GameObject/ELLE/Text Medium", false)]
    private static void CreateMediumText(MenuCommand menuCommand)
    {
        Create("Assets/Prefabs/Text (Blue, Medium) Prefab.prefab");
    }

    private static void Create(string prefabPath)
    {
        var uiElementGameObject = CreateUIElement(prefabPath);

        GameObjectUtility.SetParentAndAlign(uiElementGameObject, Selection.activeGameObject);

        Selection.activeGameObject = uiElementGameObject;
    }

    private static GameObject CreateUIElement(string prefabPath)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        var gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        gameObject.name = prefab.name;

        Undo.RegisterCreatedObjectUndo(gameObject, $"Created {gameObject.name}");

        return gameObject;
    }
#endif
}

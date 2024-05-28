using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TextInputPrefab : MonoBehaviour
{
    public Sprite BackgroundImage = null;
    public TMP_InputField Input;
    public string InputText;

    private void OnValidate()
    {
        var TMPObject = gameObject.GetComponentInChildren<TMP_InputField>();
        var BackImg = gameObject.GetComponentInChildren<Image>();

        if (TMPObject != null)
        TMPObject.text = InputText;

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
    [MenuItem("GameObject/ELLE/InputField", false)]
    private static void Create(MenuCommand menuCommand)
    {
        var uiElementGameObject = CreateUIElement();

        GameObjectUtility.SetParentAndAlign(uiElementGameObject, Selection.activeGameObject);

        Selection.activeGameObject = uiElementGameObject;
    }

    private static GameObject CreateUIElement()
    {
        const string prefabPath = "Assets/Prefabs/Text Input Prefab.prefab";

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        var gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        gameObject.name = prefab.name;

        Undo.RegisterCreatedObjectUndo(gameObject, $"Created {gameObject.name}");

        return gameObject;
    }
#endif
}

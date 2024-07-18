using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPrefabScript : MonoBehaviour
{
    public Sprite ButtonImage = null;
    public TextPrefabScript Text;
    public AudioClip buttonclick;
    private void OnValidate()
    {
        var Img = gameObject.GetComponentInChildren<Image>();

        if (Img == null)
        {
            Img.color = Color.clear;
        }
        else
        {
            Img.color = Color.white;
        }

        Img.sprite = ButtonImage;
    }
#if UNITY_EDITOR
    [MenuItem("GameObject/ELLE/Button", false)]

    private static void Create(MenuCommand menuCommand)
    {
        var uiElementgameObject = CreateUIElement();

        GameObjectUtility.SetParentAndAlign(uiElementgameObject, Selection.activeGameObject);

        Selection.activeGameObject = uiElementgameObject;
    }

    private static GameObject CreateUIElement()
    {
        const string prefabPath = "Assets/Prefabs/Button Prefab.prefab"; 

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        var gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        gameObject.name = prefab.name;

        Undo.RegisterCreatedObjectUndo(gameObject, $"Created {gameObject.name}");

        return gameObject;
    }
#endif
    public void PlayButtonSound()
    {
        AudioSource.PlayClipAtPoint(buttonclick, Vector3.zero, 0.05f);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// Editor script to automatically set up the Cable Sorting minigame.
/// Creates prefabs, scenes, and all GameObjects programmatically.
///
/// Instructions:
/// 1. Copy this script into Assets/Editor/CableSortingSetup.cs
/// 2. In Unity, go to Assets > Cable Sorting > Setup Minigame
/// 3. The script will create all necessary prefabs and a test scene
///
/// After running, you can:
/// - Create a new scene from CableSortTestScene
/// - Drag CableSortingMinigame.prefab into your scene
/// - Adjust positions and settings as needed
/// </summary>
public static class CableSortingSetup
{
    private const string PREFS_FOLDER = "Assets/Prefabs";
    private const string SCENES_FOLDER = "Assets/Scenes";
    private const string PICTURES_FOLDER = "Assets/Pictures";

    [MenuItem("Assets/Cable Sorting/Setup Minigame")]
    public static void SetupMinigame()
    {
        try
        {
            // Ensure folders exist
            CreateFolders();

            // Create prefabs
            CreateCablePrefab();
            CreateBinPrefab();
            CreateMinigamePrefab();

            // Create test scene
            CreateTestScene();

            AssetDatabase.Refresh();
            Debug.Log("Cable Sorting Minigame setup complete!");
            Debug.Log("1. You can now create a new scene from: " + SCENES_FOLDER + "/CableSortTestScene.unity");
            Debug.Log("2. Drag CableSortingMinigame.prefab into your scene");
            Debug.Log("3. Adjust positions and settings as needed in Inspector");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error setting up Cable Sorting Minigame: " + e.Message);
            Debug.LogError("Stack trace: " + e.StackTrace);
        }
    }

    private static void CreateFolders()
    {
        if (!AssetDatabase.IsValidFolder(PREFS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
            Debug.Log("Created folder: Prefabs");
        }
        if (!AssetDatabase.IsValidFolder(SCENES_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
            Debug.Log("Created folder: Scenes");
        }
        if (!AssetDatabase.IsValidFolder(PICTURES_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Pictures");
            Debug.Log("Created folder: Pictures");
        }
    }

    /// <summary>
    /// Creates the Cable prefab with RectTransform, Image, and CableController.
    /// Cable displays the image for its type.
    /// </summary>
    private static void CreateCablePrefab()
    {
        string prefabPath = PREFS_FOLDER + "/Cable.prefab";

        // Create root GameObject
        GameObject cableGO = new GameObject("Cable");

        // Add RectTransform
        RectTransform rectTransform = cableGO.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(90f, 45f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Add Image component for visual (cable body)
        Image image = cableGO.AddComponent<Image>();
        image.raycastTarget = true;

        // Add CableController (script component)
        cableGO.AddComponent<CableController>().SetSprites(
            LoadPortSprite(CableType.HDMI), LoadPortSprite(CableType.USBC),
            LoadPortSprite(CableType.USBA), LoadPortSprite(CableType.LAN));

        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cableGO, prefabPath);
        Debug.Log("Created Cable prefab: " + prefabPath);
        Debug.Log("  - Size: 90x45 pixels (larger hitbox)");
        Debug.Log("  - Includes: RectTransform, Image, CableController");

        UnityEngine.Object.DestroyImmediate(cableGO);
    }

    /// <summary>
    /// Creates the Bin prefab with RectTransform, Image, and BinController.
    /// Bin is a drop target displaying its port image.
    /// </summary>
    private static void CreateBinPrefab()
    {
        string prefabPath = PREFS_FOLDER + "/Bin.prefab";

        // Create root GameObject
        GameObject binGO = new GameObject("Bin");

        // Add RectTransform
        RectTransform rectTransform = binGO.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(140f, 85f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Add Image component for visual (bin background)
        Image image = binGO.AddComponent<Image>();
        image.raycastTarget = true;

        // Add BinController (script component)
        binGO.AddComponent<BinController>().SetPortSprite(LoadPortSprite(CableType.HDMI));

        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(binGO, prefabPath);
        Debug.Log("Created Bin prefab: " + prefabPath);
        Debug.Log("  - Size: 140x85 pixels (larger hitbox)");
        Debug.Log("  - Includes: RectTransform, Image, BinController");

        UnityEngine.Object.DestroyImmediate(binGO);
    }

    /// <summary>
    /// Creates the root CableSortingMinigame prefab with manager and full setup.
    /// This prefab contains everything needed for the minigame as a standalone unit.
    /// </summary>
    private static void CreateMinigamePrefab()
    {
        string prefabPath = PREFS_FOLDER + "/CableSortingMinigame.prefab";

        // Create root GameObject
        GameObject rootGO = new GameObject("CableSortingMinigame");

        // Add CableSortingManager
        CableSortingManager manager = rootGO.AddComponent<CableSortingManager>();

        // Create TableArea child (root for cables and bins)
        GameObject tableArea = new GameObject("TableArea");
        tableArea.transform.SetParent(rootGO.transform, false);
        RectTransform tableRect = tableArea.AddComponent<RectTransform>();
        tableRect.sizeDelta = new Vector2(700f, 340f);
        tableRect.anchoredPosition = new Vector2(0f, 70f);
        tableRect.anchorMin = new Vector2(0.5f, 0.5f);
        tableRect.anchorMax = new Vector2(0.5f, 0.5f);
        tableRect.pivot = new Vector2(0.5f, 0.5f);

        // Create SpawnPoints child
        GameObject spawnPoints = new GameObject("SpawnPoints");
        spawnPoints.transform.SetParent(rootGO.transform, false);
        RectTransform spawnRect = spawnPoints.AddComponent<RectTransform>();
        spawnRect.sizeDelta = new Vector2(700f, 340f);
        spawnRect.anchoredPosition = new Vector2(0f, 70f);

        // Create Bins container (positioned cleanly below table with clear spacing)
        GameObject binsContainer = new GameObject("Bins");
        binsContainer.transform.SetParent(rootGO.transform, false);
        RectTransform binsRect = binsContainer.AddComponent<RectTransform>();
        binsRect.sizeDelta = new Vector2(700f, 100f);
        binsRect.anchoredPosition = new Vector2(0f, -220f);

        // Create 4 bins positioned along bottom
        CableType[] types = { CableType.HDMI, CableType.USBC, CableType.USBA, CableType.LAN };
        Vector2[] binPositions = {
            new Vector2(-210f, 0f),   // Left
            new Vector2(-70f, 0f),    // Left-Middle
            new Vector2(70f, 0f),     // Right-Middle
            new Vector2(210f, 0f)     // Right
        };

        List<BinController> binList = new List<BinController>();

        for (int i = 0; i < 4; i++)
        {
            GameObject binObj = new GameObject($"Bin_{types[i]}");
            binObj.transform.SetParent(binsContainer.transform, false);

            RectTransform binRect = binObj.AddComponent<RectTransform>();
            binRect.sizeDelta = new Vector2(140f, 85f);
            binRect.anchoredPosition = binPositions[i];

            Image binImage = binObj.AddComponent<Image>();
            binImage.raycastTarget = true;

            BinController binController = binObj.AddComponent<BinController>();
            binController.SetAcceptedCableType(types[i]);
            binController.SetPortSprite(LoadPortSprite(types[i]));

            // Collect bins for manager
            binList.Add(binController);

            Debug.Log($"  Created Bin {types[i]} at position {binPositions[i]}");
        }

        // Create TaskCompletedBanner UI element
        GameObject bannerGO = new GameObject("TaskCompletedBanner");
        bannerGO.transform.SetParent(rootGO.transform, false);

        RectTransform bannerRect = bannerGO.AddComponent<RectTransform>();
        bannerRect.sizeDelta = new Vector2(360f, 80f);
        bannerRect.anchoredPosition = Vector2.zero;

        Image bannerImage = bannerGO.AddComponent<Image>();
        bannerImage.color = new Color(0.1f, 0.12f, 0.18f, 0.92f);

        GameObject bannerTextGO = new GameObject("BannerText");
        bannerTextGO.transform.SetParent(bannerGO.transform, false);

        RectTransform textRect = bannerTextGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text bannerText = bannerTextGO.AddComponent<Text>();
        bannerText.text = "Task completed";
        bannerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Font.CreateDynamicFontFromOSFont("Arial", 32);
        bannerText.fontSize = 32;
        bannerText.alignment = TextAnchor.MiddleCenter;
        bannerText.color = new Color(0.3f, 0.9f, 0.4f, 1f);

        bannerGO.SetActive(false);

        // Assign bins, cable prefab, and banner to manager via public methods
        manager.SetBins(binList);
        manager.SetCablePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(PREFS_FOLDER + "/Cable.prefab"));
        manager.SetTaskCompletedBanner(bannerGO);

        Debug.Log("  Bins assigned programmatically via SetBins() method");
        Debug.Log("  Cable prefab assigned via SetCablePrefab() method");

        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(rootGO, prefabPath);
        Debug.Log("Created CableSortingMinigame prefab: " + prefabPath);
        Debug.Log("  - Contains: TableArea, Bins container with 4 bins");
        Debug.Log("  - Includes: CableSortingManager with bins assigned");
        Debug.Log("  - Ready for Unity scene setup");

        UnityEngine.Object.DestroyImmediate(rootGO);
    }

    private static Sprite LoadPortSprite(CableType type)
    {
        string fileName;
        switch (type)
        {
            case CableType.HDMI: fileName = "HDMI.png"; break;
            case CableType.USBC: fileName = "USBc.png"; break;
            case CableType.USBA: fileName = "usba.png"; break;
            case CableType.LAN: fileName = "rj45.png"; break;
            default: return null;
        }
        foreach (UnityEngine.Object asset in AssetDatabase.LoadAllAssetsAtPath(PICTURES_FOLDER + "/" + fileName))
        {
            if (asset is Sprite sprite) return sprite;
        }
        return null;
    }

    /// <summary>
    /// Creates a test scene with Canvas and initial setup.
    /// This scene allows you to test the minigame immediately.
    /// </summary>
    private static void CreateTestScene()
    {
        string scenePath = SCENES_FOLDER + "/CableSortTestScene.unity";

        // Create a new scene
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Get or create Canvas
        GameObject canvasGO = null;
        Canvas canvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            canvasGO = canvas.gameObject;
            Debug.Log("Found existing Canvas");
        }
        else
        {
            canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            Debug.Log("Created new Canvas");
        }

        // Add CanvasScaler if missing
        if (canvasGO.GetComponent<CanvasScaler>() == null)
        {
            canvasGO.AddComponent<CanvasScaler>();
            Debug.Log("Added CanvasScaler");
        }

        // Add GraphicRaycaster if missing
        if (canvasGO.GetComponent<GraphicRaycaster>() == null)
        {
            canvasGO.AddComponent<GraphicRaycaster>();
            Debug.Log("Added GraphicRaycaster");
        }

        // Add or ensure EventSystem exists
        GameObject eventSystem = UnityEngine.Object.FindAnyObjectByType<EventSystem>() != null
            ? UnityEngine.Object.FindAnyObjectByType<EventSystem>().gameObject
            : new GameObject("EventSystem");

        if (eventSystem.GetComponent<EventSystem>() == null)
            eventSystem.AddComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM
        StandaloneInputModule oldModule = eventSystem.GetComponent<StandaloneInputModule>();
        if (oldModule != null)
        {
            UnityEngine.Object.DestroyImmediate(oldModule);
        }
        if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
        {
            eventSystem.AddComponent<InputSystemUIInputModule>();
            Debug.Log("Added InputSystemUIInputModule to EventSystem (New Input System active)");
        }
#else
        if (eventSystem.GetComponent<StandaloneInputModule>() == null)
        {
            eventSystem.AddComponent<StandaloneInputModule>();
            Debug.Log("Added StandaloneInputModule to EventSystem (Legacy Input Manager active)");
        }
#endif

        // Ensure a Camera exists so the scene has a rendering camera
        if (UnityEngine.Object.FindAnyObjectByType<Camera>() == null)
        {
            GameObject cameraGO = new GameObject("Main Camera");
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.orthographic = true;
            camera.orthographicSize = 500f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            Debug.Log("Added Main Camera");
        }

        // Instantiate the minigame prefab
        string minigamePath = PREFS_FOLDER + "/CableSortingMinigame.prefab";
        GameObject minigamePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(minigamePath);

        if (minigamePrefab != null)
        {
            GameObject minigameInstance = (GameObject)PrefabUtility.InstantiatePrefab(minigamePrefab);
            minigameInstance.name = "CableSortingMinigame";
            minigameInstance.transform.SetParent(canvasGO.transform, false);

            // Set anchor and position to fill the screen
            RectTransform rt = minigameInstance.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
            }

            Debug.Log("Instantiated CableSortingMinigame prefab in scene");
        }
        else
        {
            Debug.LogWarning("Could not load CableSortingMinigame.prefab. Create it first.");
        }

        // Save scene
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);

        Debug.Log("\n=== Setup Complete ===");
        Debug.Log("Scene created: " + scenePath);
        Debug.Log("\nTo test the minigame:");
        Debug.Log("1. Open 'Assets/Scenes/CableSortTestScene.unity'");
        Debug.Log("2. Press Play button in Unity Editor");
        Debug.Log("3. You can drag cables between bins");
        Debug.Log("4. Watch cables disappear on correct drops");
        Debug.Log("5. New cables spawn when you make correct matches");
    }
}

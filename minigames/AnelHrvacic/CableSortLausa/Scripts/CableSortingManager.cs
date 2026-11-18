using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// Main manager for the cable sorting minigame.
/// Spawns cables, tracks progress, handles drops, and fires completion events.
/// </summary>
public class CableSortingManager : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private int initialCableCount = 6;
    [SerializeField] private int cablesToAddOnDrop = 2;
    [SerializeField] private int maxCablesOnTable = 50;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-320f, -80f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(320f, 220f);

    [Header("Prefabs & UI")]
    [SerializeField] private GameObject cablePrefab;
    [SerializeField] private GameObject taskCompletedBanner;

    [Header("Bin References")]
    [SerializeField] private List<BinController> bins = new List<BinController>();

    // Events
    public Action OnTaskCompleted;

    // State
    private List<GameObject> activeCables = new List<GameObject>();
    private CableType[] allCableTypes = { CableType.HDMI, CableType.USBC, CableType.USBA, CableType.LAN };

    private void Awake()
    {
        EnsureInputModule();
    }

    /// <summary>
    /// Ensures that the EventSystem uses InputSystemUIInputModule if the Input System package is active.
    /// Fixes InvalidOperationException when active Input handling is set to Input System package in Player Settings.
    /// </summary>
    private void EnsureInputModule()
    {
#if ENABLE_INPUT_SYSTEM
        var standalone = UnityEngine.Object.FindAnyObjectByType<StandaloneInputModule>();
        if (standalone != null)
        {
            GameObject esGO = standalone.gameObject;
            Destroy(standalone);
            if (esGO.GetComponent<InputSystemUIInputModule>() == null)
            {
                esGO.AddComponent<InputSystemUIInputModule>();
            }
        }
#endif
    }

    /// <summary>
    /// Initializes the table by spawning the initial set of cables.
    /// </summary>
    private void Start()
    {
        CreateTaskHint();
        SpawnInitialCables();
    }

    /// <summary>
    /// Shows the task description at the top of the screen (Upper Austrian dialect).
    /// </summary>
    private void CreateTaskHint()
    {
        if (transform.Find("TaskHint") != null) return;

        GameObject hintGO = new GameObject("TaskHint");
        hintGO.transform.SetParent(transform, false);

        RectTransform rect = hintGO.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(900f, 60f);
        rect.anchoredPosition = Vector2.zero;

        Text text = hintGO.AddComponent<Text>();
        text.text = "Steck olle Kabl in de foischn Kistn";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Font.CreateDynamicFontFromOSFont("Arial", 28);
        text.fontSize = 28;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
    }

    /// <summary>
    /// Checks if a world position is within the spawn/table area.
    /// </summary>
    public bool IsInSpawnArea(Vector3 worldPosition)
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect,
                RectTransformUtility.WorldToScreenPoint(null, worldPosition),
                null,
                out localPoint);

            return localPoint.x >= spawnAreaMin.x && localPoint.x <= spawnAreaMax.x &&
                   localPoint.y >= spawnAreaMin.y && localPoint.y <= spawnAreaMax.y;
        }

        return false;
    }

    /// <summary>
    /// Spawns the initial set of cables with random types.
    /// </summary>
    private void SpawnInitialCables()
    {
        activeCables.Clear();

        for (int i = 0; i < initialCableCount; i++)
        {
            SpawnRandomCable();
        }
    }

    /// <summary>
    /// Spawns a cable with a random type at a random position.
    /// </summary>
    private GameObject SpawnRandomCable()
    {
        CableType randomType = allCableTypes[UnityEngine.Random.Range(0, allCableTypes.Length)];
        return SpawnCable(randomType);
    }

    /// <summary>
    /// Spawns a cable of a specific type at a random position on the table.
    /// </summary>
    public GameObject SpawnCable(CableType type)
    {
        if (cablePrefab == null || activeCables.Count >= maxCablesOnTable)
        {
            return null;
        }

        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject cable = Instantiate(cablePrefab, spawnPosition, Quaternion.identity);
        cable.transform.SetParent(transform, false);

        var controller = cable.GetComponent<CableController>();
        if (controller != null)
        {
            controller.SetType(type);
        }

        activeCables.Add(cable);
        return cable;
    }

    /// <summary>
    /// Gets a random spawn position within the spawn area.
    /// </summary>
    private Vector2 GetRandomSpawnPosition()
    {
        return new Vector2(
            UnityEngine.Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            UnityEngine.Random.Range(spawnAreaMin.y, spawnAreaMax.y));
    }

    /// <summary>
    /// Called by CableController when a cable is released.
    /// Determines if the drop was on a valid bin and handles accordingly.
    /// </summary>
    public bool TryDropCable(GameObject cableObject, PointerEventData eventData)
    {
        if (bins == null || bins.Count == 0) return false;

        // Check if the cable is over any bin
        foreach (var bin in bins)
        {
            if (bin == null) continue;

            if (IsOverBin(cableObject, bin, eventData))
            {
                HandleCableDropped(cableObject.GetComponent<CableController>(), bin);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if a cable is over a specific bin.
    /// Uses EventSystem RaycastAll to detect UI drop targets.
    /// </summary>
    private bool IsOverBin(GameObject cable, BinController bin, PointerEventData eventData)
    {
        var binRect = bin.GetComponent<RectTransform>();
        if (binRect == null) return false;

        if (EventSystem.current == null || eventData == null)
            return false;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == bin.gameObject)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Handles the logic when a cable is dropped on a bin.
    /// </summary>
    public void HandleCableDropped(CableController controller, BinController bin)
    {
        if (controller == null || bin == null) return;

        // Impostor task: a cable counts as "correct" only when sabotaged into a box of a different type.
        bool isCorrect = controller.GetCableType() != bin.AcceptedCableType;

        if (isCorrect)
        {
            // Wrong box (sabotage succeeded) - delete cable and check completion
            HandleCorrectDrop(controller.gameObject);
        }
        else
        {
            // Matching box (impostor slipped up) - return cable to start position and spawn 2 new cables
            HandleIncorrectDrop(controller);
        }
    }

    /// <summary>
    /// Handles a correct drop - deletes cable and checks completion.
    /// </summary>
    private void HandleCorrectDrop(GameObject cableObject)
    {
        activeCables.Remove(cableObject);
        Destroy(cableObject);
        CheckCompletion();
    }

    /// <summary>
    /// Handles an incorrect drop - cable returns to start and 2 new cables spawn on table.
    /// </summary>
    private void HandleIncorrectDrop(CableController controller)
    {
        controller.ReturnToStart();
        SpawnReplacementCables();
    }

    /// <summary>
    /// Spawns 2 new cables on the table after an incorrect drop (up to maxCablesOnTable).
    /// </summary>
    private void SpawnReplacementCables()
    {
        for (int i = 0; i < cablesToAddOnDrop; i++)
        {
            if (activeCables.Count >= maxCablesOnTable) break;
            SpawnRandomCable();
        }
    }

    /// <summary>
    /// Checks if all cables have been sorted and fires the completion event (no on-screen message).
    /// </summary>
    private void CheckCompletion()
    {
        if (activeCables.Count == 0)
        {
            OnTaskCompleted?.Invoke();
        }
    }

    /// <summary>
    /// Resets the minigame to initial state.
    /// </summary>
    public void ResetGame()
    {
        foreach (var cable in activeCables)
        {
            if (cable != null) Destroy(cable);
        }

        if (taskCompletedBanner != null)
        {
            taskCompletedBanner.SetActive(false);
        }

        SpawnInitialCables();
    }

    /// <summary>
    /// Sets the bins list programmatically (for Editor setup).
    /// </summary>
    public void SetBins(List<BinController> binList)
    {
        bins = binList;
    }

    /// <summary>
    /// Sets the cable prefab programmatically (for Editor setup).
    /// </summary>
    public void SetCablePrefab(GameObject prefab)
    {
        cablePrefab = prefab;
    }

    /// <summary>
    /// Sets the task completed banner programmatically (for Editor setup).
    /// </summary>
    public void SetTaskCompletedBanner(GameObject banner)
    {
        taskCompletedBanner = banner;
    }
}
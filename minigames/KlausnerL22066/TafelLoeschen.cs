using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Minigame "Tafel löschen" für Among HTL.
/// Der Spieler wischt die Kreide mit dem Schwamm weg. Ab 95 % sauber wird Complete() aufgerufen.
/// Die UI wird beim ersten Start automatisch im eigenen RectTransform (600 × 400) gebaut.
/// Bei jedem Start ist die Kreide zufällig neu verteilt.
/// </summary>
public class TafelLoeschen : Minigame
{
    [Header("Tafel löschen")]
    [SerializeField, Range(0.5f, 1f)] float benoetigtSauber = 0.95f;
    [SerializeField] int schwammRadius = 13;   // in Textur-Pixeln (≈ 26 px am Bildschirm)

    Text statusText;
    TafelLoeschenFlaeche flaeche;
    bool gebaut;
    bool fertig;

    public float BenoetigtSauber => benoetigtSauber;

    void OnEnable()
    {
        if (!gebaut) Bauen();
        fertig = false;
        flaeche.Neu();   // jedes Mal neue, zufällige Kreide
    }

    // ---------- Aufrufe von der Tafel ----------

    public void OnFortschritt(float sauber)
    {
        statusText.text = Mathf.FloorToInt(sauber * 100f) + " % sauber";
    }

    public void OnFertig()
    {
        if (fertig) return;
        fertig = true;
        statusText.text = "Tafel sauber!";
        Complete();
    }

    // ---------- UI aufbauen (600 × 400) ----------

    void Bauen()
    {
        gebaut = true;
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var root = (RectTransform)transform;

        ErstelleText("Titel", root, "Wisch die Tafel sauber!", 28, new Vector2(0, 170), font);

        var rahmen = ErstelleBild("Rahmen", root, new Color(0.45f, 0.28f, 0.15f), new Vector2(572, 292), new Vector2(0, -5));
        var tafel = ErstelleBild("Tafel", rahmen.transform, new Color(0.18f, 0.31f, 0.23f), new Vector2(556, 276), Vector2.zero);

        var kreide = new GameObject("Kreide", typeof(RectTransform), typeof(RawImage));
        kreide.transform.SetParent(tafel.transform, false);
        var kr = (RectTransform)kreide.transform;
        kr.anchorMin = Vector2.zero;
        kr.anchorMax = Vector2.one;
        kr.offsetMin = Vector2.zero;
        kr.offsetMax = Vector2.zero;

        var schwamm = ErstelleBild("Schwamm", root, new Color(0.95f, 0.8f, 0.25f), new Vector2(56, 34), Vector2.zero);
        schwamm.raycastTarget = false;

        statusText = ErstelleText("Status", root, "0 % sauber", 24, new Vector2(0, -175), font);

        flaeche = kreide.AddComponent<TafelLoeschenFlaeche>();
        flaeche.Init(this, schwamm.rectTransform, schwammRadius);
    }

    static Image ErstelleBild(string name, Transform parent, Color farbe, Vector2 groesse, Vector2 pos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = farbe;
        img.rectTransform.sizeDelta = groesse;
        img.rectTransform.anchoredPosition = pos;
        return img;
    }

    static Text ErstelleText(string name, Transform parent, string inhalt, int groesse, Vector2 pos, Font font)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.font = font;
        t.text = inhalt;
        t.fontSize = groesse;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleCenter;
        t.raycastTarget = false;
        t.rectTransform.sizeDelta = new Vector2(580, 40);
        t.rectTransform.anchoredPosition = pos;
        return t;
    }
}

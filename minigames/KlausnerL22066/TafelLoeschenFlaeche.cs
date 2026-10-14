using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Wischbare Kreide-Schicht für das Minigame "Tafel löschen".
/// Wird von TafelLoeschen automatisch hinzugefügt – nicht selbst auf ein Objekt ziehen.
/// </summary>
public class TafelLoeschenFlaeche : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    const int W = 278, H = 138;   // halbe Auflösung der Tafel (556 × 276)

    TafelLoeschen spiel;
    RectTransform rect, schwamm;
    Texture2D tex;
    Color32[] pixels;
    int radius, kreideGesamt, kreideUebrig;
    bool dirty, fertig;
    Vector2? letzterPunkt;

    public void Init(TafelLoeschen spiel, RectTransform schwamm, int radius)
    {
        this.spiel = spiel;
        this.schwamm = schwamm;
        this.radius = radius;
        rect = (RectTransform)transform;

        tex = new Texture2D(W, H, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        GetComponent<RawImage>().texture = tex;
    }

    /// <summary>Neue zufällige Kreide – wird bei jedem Start aufgerufen.</summary>
    public void Neu()
    {
        pixels = new Color32[W * H];
        KritzelZeichnen();

        kreideGesamt = 0;
        foreach (var p in pixels) if (p.a > 0) kreideGesamt++;
        kreideUebrig = kreideGesamt;

        fertig = false;
        letzterPunkt = null;
        schwamm.gameObject.SetActive(false);
        spiel.OnFortschritt(0f);
        dirty = true;
    }

    void KritzelZeichnen()
    {
        int anzahl = Random.Range(25, 35);
        for (int s = 0; s < anzahl; s++)
        {
            Vector2 pos = new Vector2(Random.Range(12, W - 12), Random.Range(12, H - 12));
            float winkel = Random.Range(0f, Mathf.PI * 2f);
            int schritte = Random.Range(40, 80);
            for (int i = 0; i < schritte; i++)
            {
                winkel += Random.Range(-0.6f, 0.6f);
                pos += new Vector2(Mathf.Cos(winkel), Mathf.Sin(winkel)) * 1.5f;
                pos.x = Mathf.Clamp(pos.x, 3, W - 3);
                pos.y = Mathf.Clamp(pos.y, 3, H - 3);
                var c = new Color32(240, 240, 240, (byte)Random.Range(170, 240));
                Stempel((int)pos.x, (int)pos.y, Random.Range(1, 3), c, false);
            }
        }
    }

    void Stempel(int cx, int cy, int r, Color32 farbe, bool loeschen)
    {
        int r2 = r * r;
        for (int y = Mathf.Max(0, cy - r); y <= Mathf.Min(H - 1, cy + r); y++)
        for (int x = Mathf.Max(0, cx - r); x <= Mathf.Min(W - 1, cx + r); x++)
        {
            int dx = x - cx, dy = y - cy;
            if (dx * dx + dy * dy > r2) continue;
            int i = y * W + x;
            if (loeschen)
            {
                if (pixels[i].a > 0) { pixels[i].a = 0; kreideUebrig--; }
            }
            else pixels[i] = farbe;
        }
    }

    // ---------- Eingabe ----------

    public void OnPointerDown(PointerEventData e)
    {
        letzterPunkt = null;
        if (!fertig) schwamm.gameObject.SetActive(true);
        Wischen(e);
    }

    public void OnDrag(PointerEventData e) => Wischen(e);

    public void OnPointerUp(PointerEventData e)
    {
        letzterPunkt = null;
        schwamm.gameObject.SetActive(false);
    }

    void Wischen(PointerEventData e)
    {
        if (fertig) return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, e.position, e.pressEventCamera, out var welt))
            schwamm.position = welt;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, e.position, e.pressEventCamera, out var lokal))
            return;
        Rect r = rect.rect;
        Vector2 aktuell = new Vector2((lokal.x - r.x) / r.width * W, (lokal.y - r.y) / r.height * H);

        // Lücken bei schnellen Bewegungen füllen
        Vector2 von = letzterPunkt ?? aktuell;
        int schritte = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(von, aktuell) / (radius * 0.5f)));
        for (int i = 0; i <= schritte; i++)
        {
            Vector2 p = Vector2.Lerp(von, aktuell, i / (float)schritte);
            Stempel((int)p.x, (int)p.y, radius, default, true);
        }
        letzterPunkt = aktuell;
        dirty = true;

        float sauber = kreideGesamt == 0 ? 1f : 1f - (float)kreideUebrig / kreideGesamt;
        spiel.OnFortschritt(sauber);

        if (sauber >= spiel.BenoetigtSauber)
        {
            fertig = true;
            for (int i = 0; i < pixels.Length; i++) pixels[i].a = 0;   // letzte Reste weg
            schwamm.gameObject.SetActive(false);
            spiel.OnFertig();
        }
    }

    void LateUpdate()
    {
        if (!dirty) return;
        tex.SetPixels32(pixels);
        tex.Apply(false);
        dirty = false;
    }

    void OnDestroy()
    {
        if (tex != null) Destroy(tex);
    }
}

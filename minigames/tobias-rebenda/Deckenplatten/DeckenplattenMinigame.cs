using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TobiasRebenda
{
    // Imposter-Task "Deckenplatten beschädigen":
    // Man nimmt einen Schlapfen und wirft ihn gegen die Deckenplatten,
    // bis alle heruntergefallen sind.
    // Die Oberfläche wird im Code gebaut, damit jeder Start anders ist.
    public class DeckenplattenMinigame : Minigame
    {
        [Header("Einstellungen")]
        [SerializeField] private int minPlatten = 2;
        [SerializeField] private int maxPlatten = 2;
        [SerializeField] private int minTreffer = 2;
        [SerializeField] private int maxTreffer = 4;
        [Tooltip("So schnell muss der Schlapfen an der Decke noch sein, damit es zählt")]
        [SerializeField] private float minWurfSpeed = 250f;
        [SerializeField] private float schwerkraft = 1400f;

        private const float Breite = 600f;
        private const float Hoehe = 400f;
        private const float DeckeUnten = 110f;
        private const float PlattenHoehe = 60f;
        private const float BodenOben = -170f;
        private const float SchlapfenBreite = 70f;
        private const float SchlapfenHoehe = 24f;
        private const float MaxWurfSpeed = 2000f;

        private RectTransform root;
        private RectTransform schlapfen;
        private Text fortschrittText;
        private Text hinweisText;
        private Font schrift;

        private readonly List<Deckenplatte> platten = new List<Deckenplatte>();
        private readonly List<Vector3> verlauf = new List<Vector3>(); // x, y = Position, z = Zeit
        private int kaputt;
        private bool fertig;
        private Coroutine hinweisRoutine;

        private bool gehalten;
        private Vector2 greifOffset;
        private Vector2 geschw;
        private float drehung;
        private float drehSpeed;

        private float BodenY => BodenOben + SchlapfenHoehe / 2f;

        private void OnEnable()
        {
            Aufbauen();
        }

        private void Aufbauen()
        {
            root = (RectTransform)transform;

            // alles vom letzten Mal löschen
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);

            platten.Clear();
            verlauf.Clear();
            kaputt = 0;
            fertig = false;
            gehalten = false;
            geschw = Vector2.zero;
            drehung = 0;
            drehSpeed = 0;

            if (GetComponent<RectMask2D>() == null)
                gameObject.AddComponent<RectMask2D>();

            schrift = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Wand
            Image wand = NeuesBild("Wand", root, new Color(0.62f, 0.66f, 0.7f), Vector2.zero, new Vector2(Breite, Hoehe));
            wand.raycastTarget = true;

            // Boden
            float bodenHoehe = BodenOben + Hoehe / 2f;
            NeuesBild("Boden", root, new Color(0.45f, 0.38f, 0.32f),
                new Vector2(0, -Hoehe / 2f + bodenHoehe / 2f), new Vector2(Breite, bodenHoehe));

            // Deckenrahmen hinter den Platten
            float rahmenUnten = DeckeUnten - 6f;
            float rahmenHoehe = Hoehe / 2f - rahmenUnten;
            NeuesBild("Deckenrahmen", root, new Color(0.3f, 0.3f, 0.33f),
                new Vector2(0, rahmenUnten + rahmenHoehe / 2f), new Vector2(Breite, rahmenHoehe));

            // Platten (Anzahl und Haltbarkeit zufällig)
            int anzahl = Random.Range(minPlatten, maxPlatten + 1);
            float abstand = 6f;
            float plattenBreite = (Breite - 20f - abstand * (anzahl - 1)) / anzahl;
            float y = DeckeUnten + PlattenHoehe / 2f;

            for (int i = 0; i < anzahl; i++)
            {
                float x = -Breite / 2f + 10f + plattenBreite / 2f + i * (plattenBreite + abstand);
                Vector2 groesse = new Vector2(plattenBreite, PlattenHoehe);

                // dunkles Loch, das man sieht wenn die Platte weg ist
                NeuesBild("Loch", root, new Color(0.08f, 0.08f, 0.1f), new Vector2(x, y), groesse);

                float hell = Random.Range(0.85f, 0.95f);
                Image bild = NeuesBild("Platte " + (i + 1), root, new Color(hell, hell * 0.97f, hell * 0.9f), new Vector2(x, y), groesse);
                Deckenplatte platte = bild.gameObject.AddComponent<Deckenplatte>();
                platte.Init(Random.Range(minTreffer, maxTreffer + 1));
                platten.Add(platte);
            }

            // Texte
            fortschrittText = NeuerText("Fortschritt", new Vector2(-Breite / 2f + 115f, -Hoehe / 2f + 15f), new Vector2(200f, 30f), 18, TextAnchor.MiddleLeft);
            fortschrittText.color = Color.white;
            hinweisText = NeuerText("Hinweis", new Vector2(0, 50f), new Vector2(Breite - 40f, 40f), 22, TextAnchor.MiddleCenter);
            hinweisText.text = "Schlapfen nehmen und gegen die Decke werfen!";
            TextAktualisieren();

            // Schlapfen mit zufälliger Farbe
            Color[] farben =
            {
                new Color(0.2f, 0.45f, 0.85f),
                new Color(0.9f, 0.35f, 0.55f),
                new Color(0.25f, 0.7f, 0.35f),
                new Color(0.95f, 0.6f, 0.15f),
            };
            Color farbe = farben[Random.Range(0, farben.Length)];

            Image sohle = NeuesBild("Schlapfen", root, farbe,
                new Vector2(Random.Range(-200f, 200f), BodenY), new Vector2(SchlapfenBreite, SchlapfenHoehe));
            sohle.raycastTarget = true;
            schlapfen = sohle.rectTransform;

            Image riemen = NeuesBild("Riemen", schlapfen, farbe * 0.6f + new Color(0, 0, 0, 0.4f),
                new Vector2(8f, SchlapfenHoehe / 2f), new Vector2(34f, 10f));
            riemen.raycastTarget = true;

            sohle.gameObject.AddComponent<Schlapfen>().spiel = this;
        }

        private void Update()
        {
            if (schlapfen == null) return;

            float dt = Time.unscaledDeltaTime;
            float jetzt = Time.unscaledTime;

            if (gehalten)
            {
                // Position der letzten 0.1 Sekunden merken, daraus wird beim Loslassen die Wurfgeschwindigkeit
                Vector2 p = schlapfen.anchoredPosition;
                verlauf.Add(new Vector3(p.x, p.y, jetzt));
                verlauf.RemoveAll(v => jetzt - v.z > 0.1f);
                return;
            }

            Vector2 pos = schlapfen.anchoredPosition;

            geschw.y -= schwerkraft * dt;
            pos += geschw * dt;
            drehung += drehSpeed * dt;

            // Wände links und rechts
            float rand = Breite / 2f - SchlapfenBreite / 2f;
            if (pos.x < -rand)
            {
                pos.x = -rand;
                geschw.x = Mathf.Abs(geschw.x) * 0.6f;
            }
            if (pos.x > rand)
            {
                pos.x = rand;
                geschw.x = -Mathf.Abs(geschw.x) * 0.6f;
            }

            // Decke
            if (pos.y + SchlapfenHoehe / 2f >= DeckeUnten && geschw.y > 0)
            {
                pos.y = DeckeUnten - SchlapfenHoehe / 2f;
                DeckeGetroffen(pos.x, geschw.y);
                geschw.y = -geschw.y * 0.3f;
                drehSpeed = -drehSpeed;
            }

            // Boden
            if (pos.y <= BodenY)
            {
                pos.y = BodenY;
                if (geschw.y < -200f)
                    geschw.y = -geschw.y * 0.3f; // kleiner Hüpfer
                else
                    geschw.y = 0;

                geschw.x = Mathf.MoveTowards(geschw.x, 0, 1500f * dt);
                drehSpeed = 0;
                drehung = Mathf.MoveTowardsAngle(drehung, 0, 600f * dt);
            }

            schlapfen.anchoredPosition = pos;
            schlapfen.localRotation = Quaternion.Euler(0, 0, drehung);
        }

        public void SchlapfenGenommen(PointerEventData e)
        {
            gehalten = true;
            geschw = Vector2.zero;
            drehSpeed = 0;
            verlauf.Clear();

            Vector2 maus;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(root, e.position, e.pressEventCamera, out maus))
                greifOffset = schlapfen.anchoredPosition - maus;
        }

        public void SchlapfenGezogen(PointerEventData e)
        {
            if (!gehalten) return;

            Vector2 maus;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(root, e.position, e.pressEventCamera, out maus))
                return;

            // man darf ihn nicht einfach an die Decke drücken, man muss werfen
            Vector2 pos = maus + greifOffset;
            pos.x = Mathf.Clamp(pos.x, -Breite / 2f + SchlapfenBreite / 2f, Breite / 2f - SchlapfenBreite / 2f);
            pos.y = Mathf.Clamp(pos.y, BodenY, DeckeUnten - 80f);
            schlapfen.anchoredPosition = pos;
        }

        public void SchlapfenLosgelassen()
        {
            if (!gehalten) return;
            gehalten = false;

            geschw = Vector2.zero;
            if (verlauf.Count > 0)
            {
                Vector3 alt = verlauf[0];
                float zeit = Time.unscaledTime - alt.z;
                if (zeit > 0.01f)
                    geschw = (schlapfen.anchoredPosition - new Vector2(alt.x, alt.y)) / zeit;
            }
            geschw = Vector2.ClampMagnitude(geschw, MaxWurfSpeed);
            verlauf.Clear();

            // der Schlapfen dreht sich beim Fliegen
            drehSpeed = Random.Range(-300f, 300f) - geschw.x * 0.5f;
        }

        private void DeckeGetroffen(float x, float speed)
        {
            Deckenplatte platte = PlatteBei(x);
            if (platte == null) return; // Lücke oder schon kaputt

            if (speed < minWurfSpeed)
            {
                Hinweis("Fester werfen!");
                return;
            }

            if (hinweisRoutine != null) StopCoroutine(hinweisRoutine);
            if (!fertig) hinweisText.text = "";

            platte.Treffer();

            if (platte.IstKaputt)
            {
                kaputt++;
                TextAktualisieren();

                if (kaputt >= platten.Count)
                    StartCoroutine(Geschafft());
            }
        }

        private Deckenplatte PlatteBei(float x)
        {
            foreach (Deckenplatte p in platten)
            {
                if (!p.IstKaputt && x >= p.Links && x <= p.Rechts)
                    return p;
            }
            return null;
        }

        private IEnumerator Geschafft()
        {
            fertig = true;
            hinweisText.text = "Geschafft! Die Decke ist hinüber.";
            yield return new WaitForSecondsRealtime(1f);
            Complete();
        }

        private void Hinweis(string text)
        {
            if (fertig) return;
            if (hinweisRoutine != null) StopCoroutine(hinweisRoutine);
            hinweisRoutine = StartCoroutine(HinweisZeigen(text));
        }

        private IEnumerator HinweisZeigen(string text)
        {
            hinweisText.text = text;
            yield return new WaitForSecondsRealtime(1f);
            hinweisText.text = "";
        }

        private void TextAktualisieren()
        {
            fortschrittText.text = "Platten: " + kaputt + " / " + platten.Count;
        }

        private Text NeuerText(string name, Vector2 pos, Vector2 groesse, int schriftGroesse, TextAnchor ausrichtung)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.layer = gameObject.layer;
            go.transform.SetParent(root, false);

            Text t = go.AddComponent<Text>();
            t.font = schrift;
            t.fontSize = schriftGroesse;
            t.fontStyle = FontStyle.Bold;
            t.alignment = ausrichtung;
            t.color = new Color(0.12f, 0.12f, 0.15f);
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.raycastTarget = false;

            t.rectTransform.anchoredPosition = pos;
            t.rectTransform.sizeDelta = groesse;
            return t;
        }

        public static Image NeuesBild(string name, Transform parent, Color farbe, Vector2 pos, Vector2 groesse)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.layer = parent.gameObject.layer;
            go.transform.SetParent(parent, false);

            Image bild = go.AddComponent<Image>();
            bild.color = farbe;
            bild.raycastTarget = false;

            bild.rectTransform.anchoredPosition = pos;
            bild.rectTransform.sizeDelta = groesse;
            return bild;
        }
    }
}

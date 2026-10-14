using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TobiasRebenda
{
    // Eine einzelne Deckenplatte. Bekommt bei jedem Treffer Risse und fällt am Ende runter.
    public class Deckenplatte : MonoBehaviour
    {
        public float Links { get; private set; }
        public float Rechts { get; private set; }
        public bool IstKaputt => leben <= 0;

        private int leben;
        private int maxLeben;
        private Image bild;
        private RectTransform rt;
        private Color startFarbe;
        private Vector2 startPos;

        public void Init(int treffer)
        {
            leben = treffer;
            maxLeben = treffer;

            bild = GetComponent<Image>();
            rt = (RectTransform)transform;
            startFarbe = bild.color;
            startPos = rt.anchoredPosition;

            Links = startPos.x - rt.sizeDelta.x / 2f;
            Rechts = startPos.x + rt.sizeDelta.x / 2f;
        }

        public void Treffer()
        {
            if (IstKaputt) return;

            leben--;
            StopAllCoroutines();
            rt.anchoredPosition = startPos;

            float schaden = 1f - (float)leben / maxLeben;
            bild.color = Color.Lerp(startFarbe, new Color(0.45f, 0.42f, 0.38f), schaden);
            RissHinzufuegen();
            RissHinzufuegen();

            if (IstKaputt)
                StartCoroutine(Fallen());
            else
                StartCoroutine(Wackeln());
        }

        private void RissHinzufuegen()
        {
            float b = rt.sizeDelta.x * 0.35f;
            Vector2 pos = new Vector2(Random.Range(-b, b), Random.Range(-15f, 15f));
            Image riss = DeckenplattenMinigame.NeuesBild("Riss", transform, new Color(0.2f, 0.18f, 0.16f), pos, new Vector2(Random.Range(18f, 40f), 3f));
            riss.rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-70f, 70f));
        }

        private IEnumerator Wackeln()
        {
            float t = 0;
            while (t < 0.15f)
            {
                rt.anchoredPosition = startPos + Random.insideUnitCircle * 3f;
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            rt.anchoredPosition = startPos;
        }

        private IEnumerator Fallen()
        {
            transform.SetAsLastSibling(); // vor der Stange anzeigen

            float drehung = Random.Range(-120f, 120f);
            float speed = 0;
            float t = 0;
            Vector2 pos = startPos;

            while (pos.y > -400f)
            {
                float dt = Time.unscaledDeltaTime;
                t += dt;
                speed += 1500f * dt;
                pos.y -= speed * dt;
                rt.anchoredPosition = pos;
                rt.localRotation = Quaternion.Euler(0, 0, drehung * t);
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}

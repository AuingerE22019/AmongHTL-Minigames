using UnityEngine;
using UnityEngine.EventSystems;

namespace TobiasRebenda
{
    // Gibt Klicken, Ziehen und Loslassen vom Schlapfen an das Minigame weiter
    public class Schlapfen : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public DeckenplattenMinigame spiel;

        public void OnPointerDown(PointerEventData eventData)
        {
            spiel.SchlapfenGenommen(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            spiel.SchlapfenGezogen(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            spiel.SchlapfenLosgelassen();
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Playstel
{
    public class UiJumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Inject] private Player player;

        public void OnPointerDown(PointerEventData eventData)
        {
            player.Jump();
            player.onJumpButton = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            player.ReleaseJumpButton();
        }
    }
}
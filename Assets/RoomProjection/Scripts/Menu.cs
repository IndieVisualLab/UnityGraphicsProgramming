using UnityEngine;

namespace RoomProjection
{
    [RequireComponent(typeof(CameraSwitcher))]
    public class Menu : MonoBehaviour
    {

        CameraSwitcher _cameraSwitcher;

        void Start()
        {
            _cameraSwitcher = GetComponent<CameraSwitcher>();
        }

        private void OnGUI()
        {
            GUILayout.Label("Move: WASD,↑←↓→");
            GUILayout.Label("Jump: Space");

            _cameraSwitcher.Menu();
        }
    }
}
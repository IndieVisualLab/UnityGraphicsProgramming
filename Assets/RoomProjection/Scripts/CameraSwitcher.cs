using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace RoomProjection
{
    public class CameraSwitcher : MonoBehaviour
    {

        public enum Mode
        {
            SIMULATION,
            FIRST_PERSON
        }

        public Camera _simulationCamera;
        public Camera _firstPersonCamera;
        public ThirdPersonUserControl _control;

        public Mode _mode;

        void Update()
        {
            UpdateMode();
        }

        void UpdateMode()
        {
            switch (_mode)
            {
                case Mode.SIMULATION: SetActiveCamera(_simulationCamera); break;
                case Mode.FIRST_PERSON: SetActiveCamera(_firstPersonCamera); break;
            }
        }

        void SetActiveCamera(Camera cam)
        {
            _simulationCamera.enabled = _simulationCamera == cam;
            _firstPersonCamera.enabled = _firstPersonCamera == cam;
            _control.m_Cam = cam.transform;
        }

        public void Menu()
        {
            GUILayout.Label("CameraMode:");
            _mode = (Mode)GUILayout.SelectionGrid((int)_mode, System.Enum.GetNames(typeof(Mode)), 2);
        }
    }
}
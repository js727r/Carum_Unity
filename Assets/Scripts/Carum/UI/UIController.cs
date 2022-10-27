using Carum.Interior;
using UnityEngine;
// ReSharper disable Unity.NoNullPropagation

namespace Carum.UI
{
    public class UIController : MonoBehaviour
    {
        public GameObject functionBtnPanel;

        private void Start()
        {
            functionBtnPanel?.SetActive(false);
        }

        public void ToggleFunctionPanel()
        {
            functionBtnPanel?.SetActive(!functionBtnPanel.activeSelf);
        }

        public void EnterPutMode()
        {
            ToggleFunctionPanel();
            InteriorManager.GetInstance().ChangeMode(PutMode.Pick);
        }
    }
}

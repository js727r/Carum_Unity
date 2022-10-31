using Carum.Interior;
using TMPro;
using UnityEngine;

// ReSharper disable Unity.NoNullPropagation

namespace Carum.UI
{
    public class UIController : MonoBehaviour
    {
        // singleton pattern
        private static UIController _instance = null;

        public static UIController GetInstance()
        {
            return _instance;
        }
        
        public GameObject functionBtnPanel;
        public GameObject putModePanel;

        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);
        }
        private void Start()
        {
            functionBtnPanel?.SetActive(false);
        }
        /// <summary>
        /// 기능 UI 켜고 끄기
        /// </summary>
        public void ToggleFunctionPanel()
        {
            functionBtnPanel?.SetActive(!functionBtnPanel.activeSelf);
        }
        /// <summary>
        /// 가구배치모드 진입
        /// 
        /// </summary>
        public void EnterPutMode()
        {
            InteriorManager.GetInstance().ChangeMode(PutMode.Pick);
            SetActivePutModePanel(true);
        }
        /// <summary>
        /// 가구배치모드 종료
        /// </summary>
        public void ExitPutMode()
        {
            InteriorManager.GetInstance().ChangeMode(PutMode.None);
            SetActivePutModePanel(false);
        }
        /// <summary>
        /// 배치모드 UI 켜고끄기
        /// </summary>
        /// <param name="active">표시 여부</param>
        public void SetActivePutModePanel(bool active)
        {
            functionBtnPanel?.SetActive(!active);
            putModePanel?.SetActive(active);
        }
    }
}
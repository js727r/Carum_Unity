using System;
using Carum.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

// ReSharper disable Unity.NoNullPropagation

// ReSharper disable HeapView.ObjectAllocation

namespace Carum.Interior
{
    public class InteriorManager : MonoBehaviour
    {
        // singleton pattern
        private static InteriorManager _instance = null;

        public static InteriorManager GetInstance()
        {
            return _instance;
        }


        // 현재 배치중인 인테리어
        private Interior _pickedInterior;
        public float sensitive = 5f;
        private PutMode _putMode;

        public PutMode PutMode
        {
            get => _putMode;
        }


        private Camera _camera;

        // UI
        public GameObject normalModeUI;
        public GameObject putModeUI;
        public GameObject pickedActionPanel;
        public GameObject pickedSavePanel;
        public GameObject inventoryPanel;

        // raycast 레이어 마스크
        private LayerMask _layerMask;
        public float raycastDistance = 100f;


        void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            _layerMask = LayerMask.GetMask("Interior");
            _camera = Camera.main;

            // normalModeUI.SetActive(false);
            putModeUI.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (_putMode != PutMode.None)
                DoInteriorAction();
        }

        private void EnterPutMode(PutMode mode)
        {
            // UI 활성화
            _putMode = mode;
            normalModeUI.SetActive(_putMode is PutMode.None or PutMode.Pick);
            inventoryPanel.SetActive(_putMode != PutMode.Pick && inventoryPanel.activeSelf);
            putModeUI.SetActive(true);
            pickedActionPanel.SetActive(false);
            pickedSavePanel.SetActive(_putMode != PutMode.Pick);
        }

        public void ExitPutMode()
        {
            // UI 비활성화
            _putMode = PutMode.None;
            normalModeUI.SetActive(true);
            putModeUI.SetActive(false);
            SetPickedInterior(null);
        }

        public void SetPickedInterior(Interior interior)
        {
            // 기존에 선택한 가구 레이어 변경
            if (_pickedInterior)
                _pickedInterior.gameObject.layer = LayerMask.NameToLayer("Interior");

            // 선택한 가구 변경
            if (!interior)
            {
                _pickedInterior = null;
            }
            else
            {
                _pickedInterior = interior;
                _pickedInterior.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
            pickedActionPanel.SetActive(_pickedInterior);

        }

        public void SaveInterior(bool save)
        {
            if (!_pickedInterior) return;
            SetPickedInterior(null);
            ChangeMode(save ? PutMode.Pick : PutMode.None);
        }

        private void DoInteriorAction()
        {
            if (_putMode is PutMode.MoveAxis or PutMode.Rotate)
            {
                if (_putMode is PutMode.MoveAxis)
                {
                    if (Input.GetMouseButton(0))
                    {
                        var xAxis = Input.GetAxis("Mouse X");
                        var yAxis = Input.GetAxis("Mouse Y");
                        _pickedInterior.Move(new Vector3(xAxis, 0, yAxis));
                    }

                    if (Input.GetMouseButton(1))
                    {
                        var yAxis = Input.GetAxis("Mouse Y");
                        _pickedInterior.Move(new Vector3(0, yAxis, 0));
                    }
                }
                else
                {
                    var xAxis = Input.GetAxis("Mouse X") * sensitive;
                    var yAxis = Input.GetAxis("Mouse Y") * sensitive;

                    if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
                    {
                        _pickedInterior.Rotate(new Vector3(0, 0, -xAxis));
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        _pickedInterior.Rotate(new Vector3(0, xAxis, 0));
                    }
                    else if (Input.GetMouseButton(1))
                    {
                        _pickedInterior.Rotate(new Vector3(yAxis, 0, -xAxis));
                    }
                }
            }
            else if (_putMode is PutMode.Pick or PutMode.Raycast)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hit, raycastDistance, _layerMask)) return;

                if (_putMode == PutMode.Pick)
                {
                    if (!Input.GetMouseButtonDown(0) || EventSystem.current.IsPointerOverGameObject()) return;

                    Interior interior = hit.transform.gameObject.GetComponent<Interior>();
                    SetPickedInterior(interior);
                }
                else if (_putMode == PutMode.Raycast)
                {
                    if (!Input.GetMouseButton(0) || EventSystem.current.IsPointerOverGameObject()) return;
                    _pickedInterior?.Put(hit.point);
                }
            }
        }

        public void ChangeMode(string mode)
        {
            PutMode putMode;
            try
            {
                putMode = (PutMode)Enum.Parse(typeof(PutMode), mode);
            }
            catch (Exception)
            {
                Debug.Log("변경하려는 putMode 이름을 찾을 수 없음 : " + mode);
                return;
            }

            ChangeMode(putMode);
        }

        public void ChangeMode(PutMode mode)
        {
            switch (mode)
            {
                case PutMode.None:
                    ExitPutMode();
                    break;
                default:
                    EnterPutMode(mode);
                    break;
            }
        }

        // deprecated
        private void PutInteriorByRaycast()
        {
            if (!_pickedInterior) return;

            // 클릭시 해당 지점 레이캐스트
            if (Input.GetMouseButton(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                // 맞았으면 해당 지점으로 오브젝트 이동
                if (Physics.Raycast(ray, out var hit, raycastDistance, _layerMask))
                {
                    // Debug.Log(hit.point);
                    _pickedInterior.Put(hit.point);
                }
                else
                {
                    // Debug.Log("아무것도 안맞았당");
                }
            }

            // 우클릭하고 움직이면 각도 변경
            if (Input.GetMouseButton(1))
            {
                var xAxis = Input.GetAxis("Mouse X");
                var yAxis = Input.GetAxis("Mouse Y");
                _pickedInterior.transform.Rotate(0, xAxis * sensitive, 0);
            }
        }

        public void ToggleInventory()
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

    public enum PutMode
    {
        None,
        Pick,
        Raycast,
        MoveAxis,
        Rotate
    }
}
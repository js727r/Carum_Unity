using System;
using System.Collections.Generic;
using Carum.UI;
using Carum.Util;
using Unity.Mathematics;
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

        // 가구 배치 취소 시 원래 위치
        private Vector3 _originPosition;
        private Quaternion _originQuaternion;

        private Camera _camera;

        // UI
        [Header("평상시 UI")] public GameObject normalModeUI;
        public GameObject inventoryPanel;
        [Header("배치모드 UI")] public GameObject putModeUI;
        public GameObject pickedActionPanel;
        public GameObject pickedControlPanel;
        public float raycastDistance = 100f;
        [Header("이동모드 버튼")] public Image changeModeBtnImage;
        public Sprite raycastImage;
        public Sprite moveAxisImage;

        // raycast 레이어 마스크
        private LayerMask _layerMask;

        // inventory
        [Header("인벤토리")]
        public Inventory.InventoryController _inventoryController;
        // public Transform itemView;
        // public GameObject slotObject;
        // public List<GameObject> itemSlots = new();
        
        void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            _layerMask = LayerMask.GetMask("Interior");
            _camera = Camera.main;

            inventoryPanel.SetActive(false);
            putModeUI.SetActive(false);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (_putMode == PutMode.Raycast && !EventSystem.current.IsPointerOverGameObject())
                DoInteriorAction();
        }

        void Update()
        {
            if (_putMode is PutMode.MoveAxis or PutMode.Rotate or PutMode.Pick && !EventSystem.current.IsPointerOverGameObject())
                DoInteriorAction();
        }

        /// <summary>
        /// 가구배치모드 진입.
        /// </summary>
        /// <param name="mode">진입하려는 모드.</param>
        private void EnterPutMode(PutMode mode)
        {
            _putMode = mode;
            // UI 활성화
            normalModeUI.SetActive(_putMode is PutMode.None or PutMode.Pick);
            putModeUI.SetActive(true);
            pickedActionPanel.SetActive(false);
            pickedControlPanel.SetActive(_putMode != PutMode.Pick);
            // 선택한 가구 초기화
            if (_putMode is PutMode.Pick)
                SetPickedInterior(null);
            // 기본 UI 전환
            UIController.GetInstance()?.SetActivePutModePanel(true);

            // 배치 모드 스위칭 버튼 아이콘 변경
            if (_putMode == PutMode.Raycast)
                changeModeBtnImage.sprite = raycastImage;
            else if (_putMode == PutMode.MoveAxis)
                changeModeBtnImage.sprite = moveAxisImage;
        }

        /// <summary>
        /// 가구배치모드 종료
        /// </summary>
        public void ExitPutMode()
        {
            // UI 비활성화
            _putMode = PutMode.None;
            normalModeUI.SetActive(true);
            putModeUI.SetActive(false);
            SetPickedInterior(null);
            UIController.GetInstance()?.SetActivePutModePanel(false);
        }

        /// <summary>
        /// 배치모드에서 선택한 가구 설정.
        /// </summary>
        /// <param name="interior">선택할 가구.</param>
        public void SetPickedInterior(Interior interior)
        {
            // 이전에 선택했던 가구 비활성화
            if (_pickedInterior)
            {
                _pickedInterior.gameObject.layer = LayerMask.NameToLayer("Interior");
                _pickedInterior.SetOutline(false);
            }

            // 가구 선택
            if (!interior)
            {
                _pickedInterior = null;
            }
            else
            {
                _pickedInterior = interior;
                _pickedInterior.SetOutline(true);
                var pickedInteriorTransform = _pickedInterior.transform;
                _originPosition = pickedInteriorTransform.position;
                _originQuaternion = pickedInteriorTransform.rotation;
                _pickedInterior.gameObject.layer = LayerMask.NameToLayer("PickedInterior");
            }

            // 가구선택 UI 활성화
            pickedActionPanel.SetActive(_pickedInterior);
        }

        /// <summary>
        /// 선택 가구한 가구의 배치 끝내기.
        /// </summary>
        /// <param name="save">현재 위치로 저장하거나 이동 전 위치로 되돌릴지 여부.</param>
        public void SaveInterior(bool save)
        {
            if (!_pickedInterior) return;
            if (!save)
            {
                var pickedInteriorTransform = _pickedInterior.transform;
                pickedInteriorTransform.position = _originPosition;
                pickedInteriorTransform.rotation = _originQuaternion;
            }

            SetPickedInterior(null);
            ChangeMode(PutMode.Pick);
        }

        /// <summary>
        /// 가구배치모드 터치 액션.
        /// 배치모드에 따라 조작이 바뀜.
        /// </summary>
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
                        _pickedInterior.Move(new Vector3(xAxis*sensitive, 0, yAxis*sensitive));
                    }

                    if (Input.GetMouseButton(1))
                    {
                        var yAxis = Input.GetAxis("Mouse Y");
                        _pickedInterior.Move(new Vector3(0, yAxis*sensitive, 0));
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
            else if (_putMode is PutMode.Pick)
            {
                if (!Input.GetMouseButtonDown(0)) return;

                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, raycastDistance, _layerMask))
                {
                    Interior interior = hit.transform.gameObject.GetComponent<Interior>();
                    SetPickedInterior(interior);
                }
                else
                {
                    SetPickedInterior(null);
                }
            }
            else if (_putMode is PutMode.Raycast)
            {
                if (!Input.GetMouseButton(0)) return;

                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, raycastDistance, _layerMask))
                    // _pickedInterior?.Put(hit.point);
                    _pickedInterior?.PutFromRaycast(hit);

            }
        }

        /// <summary>
        /// UI를 통한 가구배치모드 변경
        /// </summary>
        /// <param name="mode">변경할 모드. Enum값와 일치하게 작성해야함.</param>
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

        /// <summary>
        /// 스크립트를 통한 가구배치모드 변경
        /// </summary>
        /// <param name="mode">변경할 모드. Enum값와 일치하게 작성해야함.</param>
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

        /// <summary>
        /// 배치모드를 Raycast혹은 MoveAxis로 전환
        /// </summary>
        public void TogglePutMode()
        {
            if (_putMode == PutMode.Raycast)
                EnterPutMode(PutMode.MoveAxis); 
            else if (_putMode == PutMode.MoveAxis)
                EnterPutMode(PutMode.Raycast);
        }
        
        // /// <summary>
        // /// 인벤토리 켜고 끄기
        // /// </summary>
        // public void ToggleInventory()
        // {
        //     if (inventoryPanel.activeSelf)
        //         inventoryPanel.SetActive(false);
        //     else
        //         OpenInventory();
        //     // inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        // }
        /// <summary>
        /// 선택한 가구 수평으로회전
        /// </summary>
        /// <param name="angle">회전 각도</param>
        public void RotatePickedInteriorHorizon(float angle)
        {
            if (!_pickedInterior) return;
            _pickedInterior.Rotate(new Vector3(0, angle, 0));
        }
        /// <summary>
        ///  선택한 가구 수직으로 회전
        /// </summary>
        /// <param name="angle">회전 각도</param>
        public void RotatePickedInteriorVertical(float angle)
        {
            if (!_pickedInterior) return;
            _pickedInterior.Rotate(new Vector3(angle, 0, 0));
        }
        /// <summary>
        ///  선택한 가구 각도 초기화
        /// </summary>
        public void ResetPickedInteriorRotation()
        {
            if (!_pickedInterior) return;
            _pickedInterior.transform.rotation = Quaternion.Euler(0, 135, 0);
        }
        
        // /// <summary>
        // /// 보유 가구 현황 Get 요청 후 인벤토리에 설정 및 UI 띄우기
        // /// </summary>
        // public void OpenInventory()
        // {
        //     ServerConnector.GetInstance().GetInventory(InventoryCallback);
        // }
        // /// <summary>
        // /// response로부터 인벤토리에 값 할당
        // /// </summary>
        // /// <param name="json">가구 보유 정보</param>
        // private void InventoryCallback(string json)
        // {
        //     // 보유중인 가구 리스트 설정
        //     _inventoryController = JsonUtility.FromJson<Inventory.InventoryController>(json);
        //     // 가구 아이콘 오브젝트 생성(추후 가구 카테고리별로 따로 빼야함)
        //     // 가구의 수 만큼 슬롯 생성
        //     int add = _inventoryController.GetCount() - itemSlots.Count;
        //     while (--add >= 0)
        //     {
        //         GameObject newSlot = Instantiate(slotObject, Vector3.zero, quaternion.identity);
        //         newSlot.transform.SetParent(itemView);
        //         itemSlots.Add(newSlot);
        //     }
        //     // 슬롯별 아이템 설정
        //     for (int i = 0; i < _inventoryController.GetCount(); i++)
        //     {
        //         
        //     }
        //     
        //     inventoryPanel.SetActive(true);
        // }
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
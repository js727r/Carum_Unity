using System;
using System.Collections.Generic;
using Carum.Util;
using Unity.Mathematics;
using UnityEngine;

namespace Carum.Inventory
{
    public class InventoryController : MonoBehaviour
    {
        private Inventory _inventory;
        public GameObject inventoryPanel;
        public Transform itemView;
        public GameObject slotObject;
        private readonly List<InventoryItem> _items = new();


        /// <summary>
        /// 인벤토리 켜고 끄기
        /// </summary>
        public void ToggleInventory()
        {
            if (inventoryPanel.activeSelf)
                inventoryPanel.SetActive(false);
            else
                OpenInventory();
            // inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }

        /// <summary>
        /// 보유 가구 현황 Get 요청 후 인벤토리에 설정 및 UI 띄우기
        /// </summary>
        private void OpenInventory()
        {
            ServerConnector.GetInstance().GetInventory(InventoryCallback);
        }

        /// <summary>
        /// response로부터 인벤토리에 값 할당
        /// </summary>
        /// <param name="json">가구 보유 정보</param>
        private void InventoryCallback(string json)
        {
            // 보유중인 가구 리스트 설정
            _inventory = JsonUtility.FromJson<Inventory>(json);
            // 가구 아이콘 오브젝트 생성(추후 가구 카테고리별로 따로 빼야함)
            // 가구의 수 만큼 슬롯 생성
            int add = _inventory.GetCount() - _items.Count;
            while (--add >= 0)
            {
                GameObject newSlot = Instantiate(slotObject, Vector3.zero, quaternion.identity);
                newSlot.transform.SetParent(itemView);
                _items.Add(newSlot.GetComponent<InventoryItem>());
            }

            // 슬롯별 아이템 설정
            for (int i = 0; i < _items.Count; i++)
            {
                if (i < _inventory.GetCount())
                {
                    _items[i].gameObject.SetActive(true);
                    _items[i].SetItem(_inventory.furnitureList[i]);
                }
                else
                {
                    _items[i].gameObject.SetActive(false);
                }
            }

            inventoryPanel.SetActive(true);
        }
    }

    [Serializable]
    public class Inventory
    {
        public List<Furniture> furnitureList;
        public long money;
        public long furnitureCount;

        public Inventory(List<Furniture> furnitureList, long money, long furnitureCount)
        {
            this.furnitureList = furnitureList;
            this.money = money;
            this.furnitureCount = furnitureCount;
        }

        public int GetCount()
        {
            return furnitureList.Count;
        }
    }

    [Serializable]
    public class Furniture
    {
        public long id;
        public string name;
        public string resource;
        public long price;
        public string type;

        public Furniture(long id, string name, string resource, long price, string type)
        {
            this.id = id;
            this.name = name;
            this.resource = resource;
            this.price = price;
            this.type = type;
        }

        public string GetResourcePath()
        {
            return "Assets/Prefabs/Furnitures/" + resource+".prefab";
        }
    }
}
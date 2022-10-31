using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Carum.Util.RequestDto;

// ReSharper disable HeapView.ObjectAllocation

namespace Carum.Util
{
    public class ServerConnector : MonoBehaviour
    {
        private static ServerConnector _instance;

        public static ServerConnector GetInstance()
        {
            return _instance;
        }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);
        }

        private const string ServerURL = "http://localhost:8080/api";

        [SerializeField] private Token _token;

        private IEnumerator GetRequest(string url, Action<string> callback)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(url);
            if (_token != null)
            {
                uwr.SetRequestHeader("access-token", _token.accessToken);
                uwr.SetRequestHeader("refresh-token", _token.refreshToken);
            }

            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
                callback(uwr.downloadHandler.text);
            }
            else
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
        }

        private IEnumerator PostRequest(string url, string json, Action<string> callback)
        {
            var uwr = new UnityWebRequest(url, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            if (_token != null)
            {
                uwr.SetRequestHeader("access-token", _token.accessToken);
                uwr.SetRequestHeader("refresh-token", _token.refreshToken);
            }

            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
                callback(uwr.downloadHandler.text);
            }
            else
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
        }

        public void Login(string id)
        {
            User user = new(id, "1234");
            string json = JsonUtility.ToJson(user);
            StartCoroutine(PostRequest(ServerURL + "/user/login", json, SetToken));
        }

        public void SetToken(string json)
        {
            Debug.Log("it's done!");
            _token = JsonUtility.FromJson<Token>(json);
        }

        public void GetInventory(Action<string> callback)
        {
            StartCoroutine(GetRequest(ServerURL + "/inventory", callback));
        }
    }
}
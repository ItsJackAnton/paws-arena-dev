using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

public class JavaScriptManager : MonoBehaviour
{
    public static JavaScriptManager Instance;


    [DllImport("__Internal")]
    public static extern void DoShareImageToTwitter(string _image, string _text);

    [DllImport("__Internal")]
    public static extern void CopyToClipboard(string _text);

    [DllImport("__Internal")]
    public static extern void ReceiveMessage(string _text);    
    
    [DllImport("__Internal")]
    public static extern void DoAuthenticate();
    
    [DllImport("__Internal")]
    public static extern void DoPurchase(string _text);

    private bool UseMockUpData => Application.isEditor;
    private Action<AuthResponse> _authCallBack;
    private Action<bool,int> purchaseCallBack;
    private PurchaseRequest purchaseRequest = new()
    {
        Amount = 100,
        Price = 100000,
        ToAddress =  "0x4F9E97c9332380ECf4aDf75F4D30Bc05e9FB7dfe"
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShareImageToTwitter(string _image, string _text)
    {
        if (Application.isEditor)
        {
            return;
        }

        DoShareImageToTwitter(_image, _text);
    }

    public void SendMessage()
    {
        ReceiveMessage("This is a test message from Unity");
    }

    public static void DoCopyToClipboard(string _string)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            CopyToClipboard(_string);
        }
        else
        {
            GUIUtility.systemCopyBuffer = _string;
        }
    }

    public void ReceiveMessageOutside(string _message)
    {
        Debug.Log("Received message outside Unity: " + _message);
    }

    public void AuthenticateAbstract(Action<AuthResponse> _callBack)
    {
        _authCallBack = _callBack;
        if (UseMockUpData)
        {
            AuthResponse _response = new AuthResponse { DidAuth = true, WalletAddress = "0x4F9E97c9332380ECf4aDf75F4D30Bc05e9FB7dfe" };
            ReceiveAuthResponse(JsonConvert.SerializeObject(_response));
            return;
        }

        DoAuthenticate();
    }

    public void ReceiveAuthResponse(string _json)
    {
        _authCallBack?.Invoke(JsonConvert.DeserializeObject<AuthResponse>(_json));
    }

    public void PurchaseCookies(Action<bool,int> _callBack)
    {
        purchaseCallBack = _callBack;
        if (UseMockUpData)
        {
            PurchaseResponse _response = new PurchaseResponse { DidPurchase = true };
            ReceivePurchaseResponse(JsonConvert.SerializeObject(_response));
            return;
        }

        DoPurchase(JsonConvert.SerializeObject(purchaseRequest));
    }

    public void ReceivePurchaseResponse(string _json)
    {
        PurchaseResponse _response = JsonConvert.DeserializeObject<PurchaseResponse>(_json);
        purchaseCallBack?.Invoke(_response.DidPurchase, purchaseRequest.Amount);
    }
}

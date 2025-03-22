
//vracas mi json ime funkcije ReceiveAuthResponse
// public class AuthResponse
// {
//     public bool DidAuth;
//     public string WalletAddress;
// }
function authenticate()
{

}



// _data je json
// public class PurchaseRequest
// {
//     public int Amount;
//     public int Price;
//     public string ToAddress;
// }


//vracas mi json, ime funkcije je ReceivePurchaseResponse
// public class PurchaseResponse
// {
//     public bool DidPurchase;
// }


function purchase(_data)
{

}

function sendMessageToUnity(functionName, data)
{
    gameInstance.SendMessage("JavaScriptManager", functionName, data);
}
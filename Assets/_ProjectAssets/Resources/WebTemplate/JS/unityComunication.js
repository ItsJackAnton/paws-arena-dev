function receiveMessage(_text)
{
    console.log("Received from Unity: "+_text);
}

function sendMessageToUnity(_text)
{
    gameInstance.SendMessage("JavaScriptManager", "ReceiveMessageOutside", "This is a test message from JS");
}
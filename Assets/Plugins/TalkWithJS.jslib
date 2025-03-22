mergeInto(LibraryManager.library,
{
    DoShareImageToTwitter: function(image,text)
    {
        ShareImageToTwitter(UTF8ToString(image),UTF8ToString(text));
    },
    
    CopyToClipboard: function(text)
    {
       var text = UTF8ToString(textPtr);
       navigator.clipboard.writeText(text).then(function() {
            console.log('Copying to clipboard was successful!');
       }, function(err) {
            console.error('Could not copy text to clipboard: ', err);
       });
    },
    
    ReceiveMessage: function(text)
    {
        receiveMessage(UTF8ToString(text));
    }
});
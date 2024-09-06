mergeInto(LibraryManager.library,
{
    DoShareImageToTwitter: function(image,text)
    {
        ShareImageToTwitter(UTF8ToString(image),UTF8ToString(text));
    }
});
function ShareImageToTwitter(base64Image, text) {
    //// save the image to local device
    var link = document.createElement('a');
    link.setAttribute('href', 'data:image/png;base64,' + base64Image);
    link.setAttribute('download', 'PawsArena.png');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    let url = `https://twitter.com/intent/tweet?text=${text}`;
    window.open(url, '_blank');
}
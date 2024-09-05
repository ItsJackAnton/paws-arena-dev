    var firebaseConfig = {
    apiKey: "AIzaSyB-FDd8lVZC1RmSUKVOr6JaTS2BSWixt7g",
    authDomain: "pawsarena-b05de.firebaseapp.com",
    projectId: "pawsarena-b05de",
    storageBucket: "pawsarena-b05de.appspot.com",
    messagingSenderId: "416272605878",
    appId: "1:416272605878:web:1a6c4e15d156b16984fc17",
    measurementId: "G-SW7V1ZP438"
};

document.addEventListener("DOMContentLoaded", function () {
    firebase.initializeApp(firebaseConfig);
    firebase.analytics();
});

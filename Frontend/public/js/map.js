function initGoogleMaps() {
    var geocoder = new google.maps.Geocoder();
    var address = document.getElementById('address').value;
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            // Center map on location
            var map = new google.maps.Map(document.getElementById('map'), {
                center: results[0].geometry.location,
                zoom: 14
            });
            var marker = new google.maps.Marker({
                position: results[0].geometry.location,
                map: map
            });
        } else {
            console.log('Geocode was not successful for the following reason: ' + status);
        }
    });

}
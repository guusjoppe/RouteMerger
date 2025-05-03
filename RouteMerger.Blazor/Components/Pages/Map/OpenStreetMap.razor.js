window.openStreetMap = {
    initMap: function (mapId, latitude, longitude) {
        const map = L.map(mapId).setView([latitude, longitude], 13);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);

        // Store the map instance for later use
        window.openStreetMap.mapInstance = map;
    },
    addGpxLayer: function (gpxUrl) {
        const map = window.openStreetMap.mapInstance;
        if (!map) {
            console.error("Map is not initialized.");
            return;
        }

        // Add GPX layer
        new L.GPX(gpxUrl, {
            async: true,
            marker_options: {
                startIconUrl: null,
                endIconUrl: null,
            }
        }).on('loaded', function (e) {
            const bounds = e.target.getBounds();
            // Log the bounds for debug purposes
            console.log("Bounds:", bounds);
            map.fitBounds(bounds);
        }).addTo(map);
    },
    getUserLocation: function () {
        return new Promise((resolve, reject) => {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    (position) => resolve({
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude
                    }),
                    (error) => reject(error)
                );
            } else {
                reject(new Error("Geolocation is not supported by this browser."));
            }
        });
    }
};
jQuery(function () {
    'use strict';
    if (jQuery(".mapeal-container").length) {
        jQuery(".mapeal-container").mapael({
            map: {
                name: "world_countries"
            }
        });
    }
});
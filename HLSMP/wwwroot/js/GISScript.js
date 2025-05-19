$(document).ready(function () {
    console.log("GIS Script Loaded");
    $('#districtDropdown').change(function () {
        var districtId = $(this).val();
        console.log("District changed");
        $('#tehsilDropdown').empty().append('<option>Loading...</option>');
        $('#villageDropdown').empty().append('<option>-- Select Village --</option>');

        $.getJSON('/GIS/GetTehsils', { districtId: districtId }, function (data) {
            $('#debugLog').html("Tehsils loaded: " + data.length); 

            $('#tehsilDropdown').empty().append('<option value="">-- Select Tehsil --</option>');
            $.each(data, function (i, tehsil) {
                $('#tehsilDropdown').append('<option value="' + tehsil.id + '">' + tehsil.name + '</option>');
            });
        });
    });

    $('#tehsilDropdown').change(function () {
        var tehsilId = $(this).val();
        $('#villageDropdown').empty().append('<option>Loading...</option>');

        $.getJSON('/GIS/GetVillages', { tehsilId: tehsilId }, function (data) {
            $('#villageDropdown').empty().append('<option value="">-- Select Village --</option>');
            $.each(data, function (i, village) {
                $('#villageDropdown').append('<option value="' + village.id + '">' + village.name + '</option>');
            });
        });
    });

});
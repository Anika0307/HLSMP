$(document).ready(function () {
    console.log("Script loaded");

    $('#DIS_CODE').change(function () {
        var DIS_CODE = $(this).val();
        $.ajax({
            url: gisUrls.getTehsils,
            type: 'POST',
            data: { DIS_CODE: DIS_CODE },
            success: function (data) {
                var tehsilDropdown = $('#Tehsil');
                tehsilDropdown.empty();
                tehsilDropdown.append('<option value="">-- Select Tehsil --</option>');
                $.each(data, function (index, item) {
                    tehsilDropdown.append('<option value="' + item.value + '">' + item.text + '</option>');
                });
            }
        });
    });

    $('#Tehsil').change(function () {
        var tehCode = $(this).val();
        $.ajax({
            url: gisUrls.getVillages,
            type: 'POST',
            data: { TehCode: tehCode },
            success: function (data) {
                var villageDropdown = $('#Village');
                villageDropdown.empty();
                villageDropdown.append('<option value="">-- Select Village --</option>');
                $.each(data, function (index, item) {
                    villageDropdown.append('<option value="' + item.value + '">' + item.text + '</option>');
                });
            }
        });
    });

    $('#Village').change(function () {
        $.ajax({
            url: gisUrls.getVillageStages,
            type: 'POST',
            success: function (data) {
                var VillageStageDropdown = $('#VillageStage');
                VillageStageDropdown.empty();
                VillageStageDropdown.append('<option value="">-- Select Village Stage --</option>');
                $.each(data, function (index, item) {
                    VillageStageDropdown.append('<option value="' + item.value + '">' + item.text + '</option>');
                });
            }
        });
    });
});

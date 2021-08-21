//Fix column header widths on jquery dataTables
$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    //$($.fn.dataTable.tables(true)).DataTable().columns.adjust();
    $($.fn.dataTable.tables(true)).DataTable().scroller.measure();
});

function loadForm(objectID, objectIDField, objectTypeID, actionID, remoteElementID, loadIntoElementID, parentID, childID, rootPath, modalID, formID, listID, buttonClass, closeModalOnSuccess) {
    let dataToLoad = null;

    //Load the details form if no form specified
    if (!actionID) {
        actionID = "Details"
    }

    let buttonsToDisable = `${objectIDField}DisableOnCreate`;
    // Hide any edit only buttons if new record is loading into form
    if (actionID === "Create") {
        $(".CreateOnlyButton").show();
        $(".EditOnlyButton").hide();
  
        document.querySelector("." + buttonsToDisable)

        var disableOnCreateElement = document.querySelectorAll("." + buttonsToDisable);
        disableOnCreateElement.forEach(function (elem) {
            elem.disabled = true;
        });
    }
    else if (actionID === "Edit") {
        $(".CreateOnlyButton").hide();
        $(".EditOnlyButton").show();

        document.querySelector(".DisableOnCreate")

        var disableOnCreateElement = document.querySelectorAll("." + buttonsToDisable);
        disableOnCreateElement.forEach(function (elem) {
            elem.disabled = false;
        });
    }
    else {
        $(".CreateOnlyButton").hide();
        $(".EditOnlyButton").hide();

        document.querySelector(".DisableOnCreate")

        var disableOnCreateElement = document.querySelectorAll("." + buttonsToDisable);
        disableOnCreateElement.forEach(function (elem) {
            elem.disabled = false;
        });
    }

    //If object ID is found then need to edit
    if (objectID && objectID !== "0") {
        if (parentID) {
            dataToLoad = `${rootPath}/${objectTypeID}/${actionID}/${parentID}/${objectID}`;
        }
        else {
            dataToLoad = `${rootPath}/${objectTypeID}/${actionID}/${objectID}`;
        }
    }
    else if (parentID) {
        dataToLoad = `${rootPath}/${objectTypeID}/${actionID}/${parentID}`;
    }
    else {
        dataToLoad = `${rootPath}/${objectTypeID}/${actionID}`;
    }

    if (childID) {
        dataToLoad += `/${childID}`;
    }

    $.get(dataToLoad, function (data) {

    })
        .then(data => {
            var formData = $(data).find("#" + remoteElementID);
            $("#" + loadIntoElementID).html(formData);

            attachFormFunctions(objectID, objectIDField, objectTypeID, actionID, remoteElementID, loadIntoElementID, parentID, childID, rootPath, modalID, formID, listID, buttonClass, closeModalOnSuccess);

            console.log(`${objectTypeID} form data from ${dataToLoad} loaded into ${loadIntoElementID}`);
        })
        .fail(function () {
            let title = `Error Loading ${objectTypeID} Information`;
            let content = `The form ${objectTypeID} data at ${dataToLoad} returned a server error and could not be loaded`;

            doErrorModal(title, content);
        });
}

function loadList(objectTypeID, listID, parentID, childID, rootPath) {
    let dataToLoad = `${rootPath}/${objectTypeID}/`

    if (parentID) {
        dataToLoad += `${parentID}/`;
    }
    if (childID) {
        dataToLoad += `${childID}/`;
    }

    dataToLoad += `?handler=Json`;

    let listData = $("#" + listID).DataTable();

    listData.ajax.url(dataToLoad).load(null, false);

    console.log(objectTypeID + " from " + dataToLoad + " Loaded");
}

function showAlerts(objectTypeID, objectID, rootPath) {
    let dataToLoad = null;

    if (objectID && objectID !== "0") {
        dataToLoad = `${rootPath}/${objectTypeID}/${objectID}/?handler=Json&alertOnly=true`;
    }
    else {
        dataToLoad = `${rootPath}/${objectTypeID}/?handler=Json&alertOnly=true`;
    }

    $.get(dataToLoad, function (data) {

    })
        .then(data => {
            //Show any alerts from the remote page
            var alertHtml = "";
            for (var key in data) {
                if (data[key].noteText !== null) {
                    alertHtml += "<p>" + data[key].noteText + "</p>";
                }
            }

            if (alertHtml !== "") {
                doModal(`${objectTypeID} Alerts`, alertHtml);
            }

            console.log(objectTypeID + " from " + dataToLoad + " Loaded");
        })
        .fail(function () {
            let title = `Error Loading ${dataToLoad}`;
            let content = `The list at ${dataToLoad} returned a server error and could not be loaded`;

            doErrorModal(title, content);
        });
}

function attachFormFunctions(objectID, objectIDField, objectTypeID, actionID, remoteElementID, loadIntoElementID, parentID, childID, rootPath, modalID, formID, listID, buttonClass, closeModalOnSuccess) {
    var form = $("#" + formID);
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
    
    $("#" + modalID).find(".ActionButton").data("id", objectID);

    extraFormFunctions();

    form.submit(function (event) {
        event.preventDefault();

        //If existing item then update
        let mode = null;
        let submitLocation = null;
        //If object ID is found then need to edit
        if (objectID && objectID !== "0") {
            mode = "EDIT"
            if (parentID) {
                submitLocation = `${rootPath}/${objectTypeID}/${actionID}/${parentID}/${objectID}`;
            }
            else {
                submitLocation = `${rootPath}/${objectTypeID}/${actionID}/${objectID}`;
            }
        }
        else if (parentID) {
            mode = "NEW"
            submitLocation = `${rootPath}/${objectTypeID}/${actionID}/${parentID}`;
        }
        else {
            mode = "NEW"
            submitLocation = `${rootPath}/${objectTypeID}/${actionID}`;
        }

        if (childID) {
            submitLocation += `/${childID}`;
        }

        //If no unobtrusive validation errors
        if (form.valid()) {
            let formData = new FormData(document.getElementById(formID));
            $.ajax({
                type: "POST",
                url: submitLocation,
                data: formData,
                success: function (data) {
                    let hasClosedModal = false;
                    if (data.isSuccessful !== false) {
                        if (closeModalOnSuccess === true && $("#CloseFormOnSubmit").val() === "Y") {
                            hasClosedModal = true;
                            $("#" + modalID).modal("hide");
                        }
                        console.log(`Data saved to ${submitLocation}`);
                        let audio = new Audio("/sounds/confirm.wav");
                        audio.play();
                    }
                    else {
                        let title = `Error Saving Data`;

                        if (data.errorLevel === 1) {
                            doModal(title, data.errorDescription);
                        }
                        else {
                            doErrorModal(title, data.errorDescription);
                        }
                    }

                    //Now object created must switch to edit mode
                    if (hasClosedModal === false && mode === "NEW") {
                        $("#" + objectIDField).val(data.objectID);
                        $("#" + modalID).trigger("shown.bs.modal");
                    }

                    loadList(objectTypeID, listID, parentID, childID, rootPath);
                },
                error: function (error) {
                    let title = `Error Saving Data`;
                    doCrashModal(title, error);
                },
                async: true,
                cache: false,
                contentType: false,
                processData: false,
                timeout: 60000
            });
        }
    });
}

function performButtonAction(button) {
    if (button.classList.contains("CloseModal")) {
        $("#CloseFormOnSubmit").val("Y");
    }
    else {
        $("#CloseFormOnSubmit").val("N");
    }

    if (button.classList.contains("ClearID")) {
        //Clear the ID field value
        let fieldToClear = button.getAttribute("data-id");

        $("#" + fieldToClear).val("");
    }
}

function attachListFunctions(
    buttonClass
) {
    $("." + buttonClass).click(function (event) {
        objectIDField = $(this).data("selector");
        
        if (!objectIDField) {
            return true;
        }
        else {
            //Set active object ID - input field must exist
            let objectID = $(this).data("id");
            let actionID = $(this).data("path");
            let formTitle = $(this).data("loading-text");

            //If delete button pressed then object ID already set from edit form so object ID will be null in this case
            if (objectID || objectID === 0) {
                $("#" + objectIDField).val(objectID);
            }
            else {
                //Clear existing value if no new value
                $("#" + objectIDField).val("");
            }
            if (actionID) {
                $("#ActionID").val(actionID);
            }
            else {
                $("#ActionID").val("Details");
            }
            if (formTitle) {
                $("#FormTitleID").val(formTitle);
            }
        }
    });
}

function extraFormFunctions() {
    
}

function formatMoney(num, rnd, symb, decimalSep, thousSep) {
    rnd = isNaN(rnd = Math.abs(rnd)) ? 2 : rnd;
    symb = symb === undefined ? "." : symb;
    decimalSep = decimalSep === undefined ? "." : decimalSep;
    thousSep = thousSep === undefined ? "," : thousSep;

    var s = num < 0 ? "-" : "";
    var i = String(parseInt(num = Math.abs(Number(num) || 0).toFixed(rnd)));
    var j = (j = i.length) > 3 ? j % 3 : 0;

    return s + symb + (j ? i.substr(0, j) + thousSep : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousSep) + (rnd ? decimalSep + Math.abs(num - i).toFixed(rnd).slice(2) : "");
}

function fileSizeReadable(size) {
    var i = size == 0 ? 0 : Math.floor(Math.log(size) / Math.log(1024));
    return Number(size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['Bytes', 'kB', 'MB', 'GB', 'TB'][i];
}
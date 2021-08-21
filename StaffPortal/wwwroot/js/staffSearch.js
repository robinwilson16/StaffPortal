//Buttons that load content
attachListFunctions(
    "StaffRequestButton"
);

//Load data in when model is displayed
$("#ModalStaffRequest").on("shown.bs.modal", function () {
    let companyID = $("#CompanyID").val();
    let actionID = $("#ActionID").val();
    let formTitle = $("#FormTitleID").val();
    let rootPath = $("#RootPath").val();

    //If no action specified then defailt to showing the detail screen
    if (actionID == null) {
        actionID = `Details`;
    }
    if (formTitle === "") {
        formTitle = "Staff";
    }
    if (rootPath == null) {
        rootPath = ``;
    }

    $("#ModalStaffRequestLabel").find(".title").html(formTitle);

    //Set form title back to blank to default to new recond functionality
    $("#FormTitleID").val("");

    let objectID = null;
    let objectIDField = null;
    let objectTypeID = null;
    let remoteElementID = null;
    let loadIntoElementID = null;
    let parentID = null;
    let childID = null;
    let modalID = null;
    let formID = null;
    let listID = null;
    let buttonClass = null;
    let closeModalOnSuccess = null;

    objectID = null;
    objectIDField = "StaffID";
    objectTypeID = `StaffRequests`;
    remoteElementID = "StaffRequestForm";
    loadIntoElementID = "StaffRequestDetails";
    parentID = null;
    childID = null;
    modalID = "ModalStaffRequest";
    formID = "StaffRequestFormData";
    listID = "StaffRequestList";
    buttonClass = "StaffRequestButton";
    closeModalOnSuccess = true;
    loadForm(objectID, objectIDField, objectTypeID, actionID, remoteElementID, loadIntoElementID, parentID, childID, rootPath, modalID, formID, listID, buttonClass, closeModalOnSuccess);
});

$("#ModalStaffRequest").on("hidden.bs.modal", function () {
    let loadingAnim = $("#LoadingHTML").html();
    $("#StaffRequestDetails").html(loadingAnim);
});

var staffSearchButton = document.querySelectorAll(`.StaffSearchButton`);
staffSearchButton.forEach(function (button) {
    button.addEventListener('click', function (event) {
        let searchQuery = document.getElementById("StaffSearchID").value;
        getStaffSearchResults(searchQuery);
    });
});

var staffSearchBox = document.getElementById(`StaffSearchID`);
staffSearchBox.addEventListener('keyup', function (event) {
    if (event.keyCode === 13) {
        // Enter pressed
        event.preventDefault();
        // Trigger the button element with a click
        let searchQuery = this.value;
        getStaffSearchResults(searchQuery);
    }
});

async function getStaffSearchResults(searchQuery) {
    let searchString = searchQuery.replace(" ", "%20");
    searchString = searchString.replace("/", "");
    if (searchString.length === 0) {
        searchString = "xxxx";
    }
    let staffData = `/StaffDetails/${searchString}/?handler=Json`;

    let dataToLoad = staffData;

    loadData("GET", dataToLoad)
        .then(data => {
            //try {
                displayStaffSearchResults(data, searchQuery);

                console.log(dataToLoad + " Loaded");
                attachListFunctions(
                    "EditStaffSearchResultsItemButton"
                );
            //}
            //catch (e) {
            //    let title = `Error Loading Staff Search Results`;
            //    let content = `The Staff Search Results could not be loaded`;

            //    doErrorModal(title, content);
            //}
        });
}

//Buttons that load content
attachListFunctions(
    "EditStaffSearchResultsItemButton"
);

function displayStaffSearchResults(data, searchQuery) {
    let staffData = JSON.parse(data);
    let staffContent = ``;
    let numItems = staffData.length;
    let rowSeq = 1;

    if (searchQuery.length === 0) {
        staffContent += `
            <div class="row mt-3" id="StaffSearchResultsArea">
                <div class="col-md">
                    <div class="alert alert-danger" role="alert">
                        <i class="fas fa-exclamation-circle"></i> You did not enter any text into the search box above. Please enter the name or staff code then press the Search button
                    </div>
                </div>
            </div>`;
    }
    else if (numItems === 0) {
        staffContent += `
            <div class="row mt-3" id="StaffSearchResultsArea">
                <div class="col-md">
                    <div class="alert alert-warning" role="alert">
                        <i class="fas fa-info-circle"></i> No Results for "${searchQuery}"
                    </div>
                </div>
            </div>`;
    }
    else {
        staffContent += `
            <div class="row mt-3" id="StaffSearchResultsArea">
                <div class="col-md">
                    <h5><i class="fas fa-info-circle"></i> <strong><em>${numItems}</em></strong> Results Found for "${searchQuery}":</h5>
                    <table class="table table-striped table-hover">
                        <thead>
                            <tr>
                                <th scope="col"><i class="fas fa-portrait"></i> Photo</th>
                                <th scope="col"><i class="far fa-address-card"></i> Description</th>
                                <th scope="col"><i class="far fa-address-book"></i> Contact</th>
                                <th scope="col"><i class="fas fa-map-pin"></i> Location</th>
                                <th scope="col"><i class="fas fa-users"></i> Area</th>
                            </tr>
                        </thead>
                      <tbody>`;

        for (const staffMember of staffData) {
            staffContent += `
                <tr>
                    <td class="align-middle">`;

            if (staffMember.photo !== null) {
                staffContent += `
                        <img src="data:image/jpg;base64,${staffMember.photo}" class="img-fluid StaffPhoto" />`;
            }
            else {
                staffContent += `
                        <div class="NoPhoto">
                            <i class="fas fa-user"></i>
                        </div>`;
            }

            staffContent += `
                    </td>
                    <td>
                        <h5><strong>${staffMember.forename} ${staffMember.surname}</strong></h5>
                        <p><i class="fas fa-user-tag"></i> ${staffMember.postTitles}</p>
                        <p><i class="fas fa-hashtag"></i> ${staffMember.staffRef}</p>
                    </td>
                    <td>`;

            if (staffMember.ext !== null) {
                staffContent += `
                        <p><i class="fas fa-phone-square-alt"></i> Ext: ${staffMember.ext}</p>`;
            }
            if (staffMember.tel !== null) {
                staffContent += `
                        <p><i class="fas fa-phone"></i> Tel: <a href="tel:${staffMember.tel}">${staffMember.tel}</a></p>`;
            }
            if (staffMember.mobile !== null) {
                staffContent += `
                        <p><i class="fas fa-mobile-alt"></i> Mob: <a href="tel:${staffMember.mobile}">${staffMember.mobile}</a></p>`;
            }
            if (staffMember.email !== null) {
                staffContent += `
                        <p><i class="fas fa-envelope"></i> <a href="mailto:${staffMember.email}">${staffMember.email}</a></p>`;
            }
                staffContent += `
                    </td>
                    <td>
                        <p><strong><i class="fas fa-school"></i> Site: ${staffMember.siteName}</strong></p>
                        <p><i class="fas fa-home"></i> Room: ${staffMember.roomCode}</p>
                    </td>
                    <td>`;

            if (staffMember.photo !== null) {
                staffContent += `
                        <i class="fas fa-users"></i> ${staffMember.collegeLevelName}`;
            }
            else {
                staffContent += `
                        `;
            }

            rowSeq += 1;
        }

        staffContent += `
                              </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>`;
    }

    let staffSearchArea = document.getElementById(`StaffSearchResultsArea`);
    if (staffSearchArea != null) {
        staffSearchArea.remove();
    }
    document.getElementById(`SearchBoxStaff`).insertAdjacentHTML(`afterend`, staffContent);
}

function loadData(method, url) {
    //url = `${url}&${new Date().getTime()};`
    return new Promise(function (resolve, reject) {
        let xhr = new XMLHttpRequest();
        xhr.open(method, url);
        xhr.setRequestHeader("Cache-Control", "no-cache, no-store, max-age=0");
        xhr.onload = function () {
            if (this.status >= 200 && this.status < 300) {
                resolve(xhr.response);
            } else {
                reject({
                    status: this.status,
                    statusText: xhr.statusText
                });
            }
        };
        xhr.onerror = function () {
            reject({
                status: this.status,
                statusText: xhr.statusText
            });
        };
        xhr.send();
    });
}
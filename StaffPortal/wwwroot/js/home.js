getBulletinContent();
getCEOContent();

async function getBulletinContent() {
    let bulletinData = `/NewsArticles/Bulletin/?handler=Json`;

    let dataToLoad = bulletinData;

    loadData("GET", dataToLoad)
        .then(data => {
            try {
                displayNews(data, "Bulletin");

                console.log(dataToLoad + " Loaded");

                attachListFunctions(
                    "EditBulletinItemButton"
                );
            }
            catch (e) {
                let title = `Error Loading Bulletin Items`;
                let content = `The bulletin items could not be loaded`;

                doErrorModal(title, content);
            }
        });
}

async function getCEOContent() {
    let ceoData = `/NewsArticles/CEO/?handler=Json`;

    let dataToLoad = ceoData;

    loadData("GET", dataToLoad)
        .then(data => {
            try {
                displayNews(data, "CEO");

                console.log(dataToLoad + " Loaded");

                attachListFunctions(
                    "EditCEOItemButton"
                );
            }
            catch (e) {
                let title = `Error Loading CEO News Items`;
                let content = `The CEO news items could not be loaded`;

                doErrorModal(title, content);
            }
        });
}

//Buttons that load content
attachListFunctions(
    "EditBulletinItemButton"
);
attachListFunctions(
    "EditCEOItemButton"
);

function displayNews(data, newsType) {
    let newsData = JSON.parse(data);
    let newsContent = ``;

    let rowSeq = 1;

    if (newsData[0].newsID !== null) {
        for (const newsItem of newsData) {

            let createdDate = null;
            moment.locale('en-gb');
            createdDate = moment(newsItem.createdDate).calendar();

            let icon = ``;
            let iconColour = ``;

            if (newsItem.iconID === 2) {
                icon = `fas fa-bullhorn`;
                iconClass = 'IconBlue';
            }
            else if (newsItem.iconID === 3) {
                icon = `fas fa-certificate`;
                iconClass = 'IconOrange';
            }
            else if (newsItem.iconID === 4) {
                icon = `fas fa-palette`;
                iconClass = 'IconPink';
            }
            else if (newsItem.iconID === 6) {
                icon = `fas fa-tools`;
                iconClass = 'IconGrey';
            }
            else {
                icon = `fas fa-bullhorn`;
            }

            newsContent += `
                <div class="row ${newsType}Item NewsBlock" data-target="${newsItem.articleLink}">
                    <div class="col-md">
                        <div class="card m-1">
                            <div class="card-body">
                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-2 my-auto">
                                            <h3 class="${iconClass}">
                                                <i class="${icon}"></i>
                                            </h3>
                                        </div>
                                        <div class="col-md-10">
                                            <div class="row">
                                                <div class="col-md">
                                                    <h5>${newsItem.title}</h5>
                                                    <p>${newsItem.introText}</p>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md text-end text-muted CreatedBy">
                                                    <i class="fas fa-user"></i> ${newsItem.createdByName} &nbsp;&nbsp;&nbsp; <i class="fas fa-calendar-alt"></i> ${createdDate} &nbsp;&nbsp;&nbsp; <i class="fas fa-eye"></i> ${newsItem.numViews} &nbsp;&nbsp;&nbsp; <i class="fas fa-comments"></i> ${newsItem.numComments}
                                                </div>
                                            </div>
                                            
                                        </div>
                                    </div>
                                </div>  
                            </div>
                        </div>
                    </div>
                </div>`;

            rowSeq += 1;

        }
    }

    else {
        newsContent += `
            <div class="row">
                <div class="col-md">
                    <i class="fas fa-info-circle"></i> Content coming soon...
                </div>
            </div>`;
    }

    document.getElementById(`CollegeNews${newsType}Content`).innerHTML = newsContent;

    var newsBlocks = document.querySelectorAll(`.${newsType}Item`);
    newsBlocks.forEach(function (block) {
        block.addEventListener('click', function (event) {
            let url = this.getAttribute("data-target");
            window.open(url);
        });
    });
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
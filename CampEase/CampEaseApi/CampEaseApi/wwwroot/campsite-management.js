window.onload = function () {
    loadCampsites();

    const form = document.getElementById("campsiteForm");
    form.addEventListener("submit", saveCampsite);
};

async function loadCampsites() {
    const list = document.getElementById("campsiteList");

    list.innerHTML = "<p>營地資料載入中...</p>";

    try {
        const response = await fetch("/api/campsites");
        const campsites = await response.json();

        list.innerHTML = "";

        if (campsites.length === 0) {
            list.innerHTML = "<p>目前沒有可顯示的營地。</p>";
            return;
        }

        campsites.forEach(camp => {
            const card = document.createElement("div");
            card.className = "card";

            card.innerHTML = `
                <h3>${camp.campsiteName}</h3>
                <p><strong>營地編號：</strong>${camp.campsiteId}</p>
                <p><strong>地區：</strong>${camp.county} ${camp.regionName}</p>
                <p><strong>海拔：</strong>${camp.elevation} 公尺</p>
                <p><strong>價格：</strong>NT$ ${camp.pricePerNight} / 晚</p>
                <p><strong>容納人數：</strong>${camp.capacity} 人</p>
                <p><strong>狀態：</strong>${camp.status}</p>
                <p>${camp.description}</p>

                <div class="button-group">
                    <button onclick="editCampsite(${camp.campsiteId})">修改</button>
                    <button class="danger" onclick="deleteCampsite(${camp.campsiteId})">下架</button>
                </div>
            `;

            list.appendChild(card);
        });

    } catch (error) {
        console.error(error);
        list.innerHTML = "<p>營地資料載入失敗，請確認後端是否有啟動。</p>";
    }
}

async function saveCampsite(event) {
    event.preventDefault();

    const message = document.getElementById("message");

    const editingCampsiteId = document.getElementById("editingCampsiteId").value;

    const regionId = document.getElementById("regionId").value;
    const campsiteName = document.getElementById("campsiteName").value;
    const address = document.getElementById("address").value;
    const elevation = document.getElementById("elevation").value;
    const pricePerNight = document.getElementById("pricePerNight").value;
    const capacity = document.getElementById("capacity").value;
    const description = document.getElementById("description").value;
    const status = document.getElementById("status").value;

    if (!regionId || !campsiteName || !elevation || !pricePerNight || !capacity) {
        message.className = "error-message";
        message.textContent = "請完整填寫必要欄位。";
        return;
    }

    const requestBody = {
        regionId: parseInt(regionId),
        campsiteName: campsiteName,
        address: address,
        elevation: parseInt(elevation),
        pricePerNight: parseInt(pricePerNight),
        capacity: parseInt(capacity),
        description: description
    };

    let url = "/api/campsites";
    let method = "POST";

    if (editingCampsiteId) {
        url = `/api/campsites/${editingCampsiteId}`;
        method = "PUT";
        requestBody.status = status;
    }

    try {
        const response = await fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(requestBody)
        });

        const result = await response.json();

        if (response.ok) {
            message.className = "success-message";

            if (editingCampsiteId) {
                message.textContent = `營地修改成功！營地編號：${editingCampsiteId}`;
            } else {
                message.textContent = `營地新增成功！營地編號：${result.campsiteId}`;
            }

            clearForm();
            loadCampsites();

        } else {
            message.className = "error-message";
            message.textContent = result.message || "儲存失敗。";
        }

    } catch (error) {
        console.error(error);
        message.className = "error-message";
        message.textContent = "儲存失敗，請確認後端是否有啟動。";
    }
}

async function editCampsite(id) {
    const message = document.getElementById("message");

    try {
        const response = await fetch(`/api/campsites/${id}`);
        const data = await response.json();

        if (!response.ok) {
            message.className = "error-message";
            message.textContent = data.message || "找不到這個營地。";
            return;
        }

        const camp = data.campsite;

        document.getElementById("editingCampsiteId").value = camp.campsiteId;
        document.getElementById("campsiteName").value = camp.campsiteName;
        document.getElementById("address").value = camp.address;
        document.getElementById("elevation").value = camp.elevation;
        document.getElementById("pricePerNight").value = camp.pricePerNight;
        document.getElementById("capacity").value = camp.capacity;
        document.getElementById("description").value = camp.description;
        document.getElementById("status").value = camp.status;

        setRegionValue(camp.county, camp.regionName);

        document.getElementById("formTitle").textContent = "修改營地";
        message.className = "success-message";
        message.textContent = `正在修改：${camp.campsiteName}`;

        document.getElementById("campsiteForm").scrollIntoView({ behavior: "smooth" });

    } catch (error) {
        console.error(error);
        message.className = "error-message";
        message.textContent = "讀取營地資料失敗。";
    }
}

async function deleteCampsite(id) {
    const message = document.getElementById("message");

    const confirmed = confirm("確定要下架這個營地嗎？下架後它不會出現在一般營地列表中。");

    if (!confirmed) {
        return;
    }

    try {
        const response = await fetch(`/api/campsites/${id}`, {
            method: "DELETE"
        });

        const result = await response.json();

        if (response.ok) {
            message.className = "success-message";
            message.textContent = `營地已下架！營地編號：${id}`;
            loadCampsites();
        } else {
            message.className = "error-message";
            message.textContent = result.message || "下架失敗。";
        }

    } catch (error) {
        console.error(error);
        message.className = "error-message";
        message.textContent = "下架失敗，請確認後端是否有啟動。";
    }
}

function clearForm() {
    document.getElementById("editingCampsiteId").value = "";
    document.getElementById("campsiteForm").reset();
    document.getElementById("formTitle").textContent = "新增營地";
}

function setRegionValue(county, regionName) {
    const regionMap = {
        "桃園市-復興區": "1",
        "新竹縣-尖石鄉": "2",
        "苗栗縣-南庄鄉": "3",
        "苗栗縣-泰安鄉": "4",
        "南投縣-埔里鎮": "5",
        "宜蘭縣-大同鄉": "6"
    };

    const key = `${county}-${regionName}`;
    const regionId = regionMap[key];

    if (regionId) {
        document.getElementById("regionId").value = regionId;
    }
}
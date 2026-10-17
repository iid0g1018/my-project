window.onload = function () {
    loadCampsites();
};

async function loadCampsites(queryString = "") {
    const list = document.getElementById("campsiteList");

    list.innerHTML = "<p>資料載入中...</p>";

    try {
        const response = await fetch("/api/campsites" + queryString);
        const data = await response.json();

        list.innerHTML = "";

        if (data.length === 0) {
            list.innerHTML = "<p>找不到符合條件的營地。</p>";
            return;
        }

        data.forEach(camp => {
            const card = document.createElement("div");
            card.className = "card";

            card.innerHTML = `
                <h3>${camp.campsiteName}</h3>
                <p><strong>地區：</strong>${camp.county} ${camp.regionName}</p>
                <p><strong>海拔：</strong>${camp.elevation} 公尺</p>
                <p><strong>容納人數：</strong>${camp.capacity} 人</p>
                <p class="price">每晚價格：NT$ ${camp.pricePerNight}</p>
                <p>${camp.description}</p>
                <button onclick="loadCampsiteDetail(${camp.campsiteId})">查看詳情</button>
            `;

            list.appendChild(card);
        });
    } catch (error) {
        console.error(error);
        list.innerHTML = "<p>資料載入失敗，請確認後端 API 是否有啟動。</p>";
    }
}

function searchCampsites() {
    const county = document.getElementById("county").value;
    const tag = document.getElementById("tag").value;
    const maxElevation = document.getElementById("maxElevation").value;
    const maxPrice = document.getElementById("maxPrice").value;

    const params = new URLSearchParams();

    if (county) {
        params.append("county", county);
    }

    if (tag) {
        params.append("tag", tag);
    }

    if (maxElevation) {
        params.append("maxElevation", maxElevation);
    }

    if (maxPrice) {
        params.append("maxPrice", maxPrice);
    }

    const queryString = params.toString() ? "?" + params.toString() : "";

    loadCampsites(queryString);
}

function resetSearch() {
    document.getElementById("county").value = "";
    document.getElementById("tag").value = "";
    document.getElementById("maxElevation").value = "";
    document.getElementById("maxPrice").value = "";

    document.getElementById("detailSection").classList.add("hidden");

    loadCampsites();
}

async function loadCampsiteDetail(id) {
    const detailSection = document.getElementById("detailSection");
    const detail = document.getElementById("campsiteDetail");

    detailSection.classList.remove("hidden");
    detail.innerHTML = "<p>詳細資料載入中...</p>";

    try {
        const response = await fetch(`/api/campsites/${id}`);
        const data = await response.json();

        const camp = data.campsite;
        const tags = data.tags;
        const facilities = data.facilities;

        let tagHtml = "";

        tags.forEach(tag => {
            tagHtml += `<span class="tag">${tag}</span>`;
        });

        let facilityHtml = "";

        facilities.forEach(f => {
            facilityHtml += `
                <div class="facility-item">
                    <strong>${f.facilityName}</strong>
                    ：${f.quantity}　
                    <span>${f.note}</span>
                </div>
            `;
        });

        detail.innerHTML = `
            <h3>${camp.campsiteName}</h3>
            <p><strong>地址：</strong>${camp.address}</p>
            <p><strong>地區：</strong>${camp.county} ${camp.regionName}</p>
            <p><strong>海拔：</strong>${camp.elevation} 公尺</p>
            <p><strong>價格：</strong>NT$ ${camp.pricePerNight} / 晚</p>
            <p><strong>容納人數：</strong>${camp.capacity} 人</p>
            <p><strong>狀態：</strong>${camp.status}</p>
            <p>${camp.description}</p>

            <h4>營地標籤</h4>
            <div>${tagHtml}</div>

            <h4>營地設備</h4>
            <div>${facilityHtml}</div>
        `;

        detailSection.scrollIntoView({ behavior: "smooth" });

    } catch (error) {
        console.error(error);
        detail.innerHTML = "<p>詳細資料載入失敗。</p>";
    }
}
window.onload = function () {
    loadCampsiteOptions();
    loadApplications();

    const form = document.getElementById("applicationForm");
    form.addEventListener("submit", submitApplication);
};

async function loadCampsiteOptions() {
    const select = document.getElementById("campsiteId");

    try {
        const response = await fetch("/api/campsites");
        const campsites = await response.json();

        select.innerHTML = '<option value="">請選擇營地</option>';

        campsites.forEach(camp => {
            const option = document.createElement("option");
            option.value = camp.campsiteId;
            option.textContent = `${camp.campsiteName}｜${camp.county} ${camp.regionName}｜NT$ ${camp.pricePerNight}/晚`;
            select.appendChild(option);
        });

    } catch (error) {
        console.error(error);
        select.innerHTML = '<option value="">營地載入失敗</option>';
    }
}

async function submitApplication(event) {
    event.preventDefault();

    const message = document.getElementById("message");

    const campsiteId = document.getElementById("campsiteId").value;
    const startDate = document.getElementById("startDate").value;
    const endDate = document.getElementById("endDate").value;
    const peopleCount = document.getElementById("peopleCount").value;
    const note = document.getElementById("note").value;

    if (!campsiteId || !startDate || !endDate || !peopleCount) {
        message.className = "error-message";
        message.textContent = "請完整填寫申請資料。";
        return;
    }

    if (endDate <= startDate) {
        message.className = "error-message";
        message.textContent = "離開日期必須晚於入住日期。";
        return;
    }

    const requestBody = {
        userId: 1,
        campsiteId: parseInt(campsiteId),
        startDate: startDate,
        endDate: endDate,
        peopleCount: parseInt(peopleCount),
        note: note
    };

    try {
        const response = await fetch("/api/applications", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(requestBody)
        });

        const result = await response.json();

        if (response.ok) {
            message.className = "success-message";
            message.textContent = `申請成功！申請編號：${result.applicationId}`;

            document.getElementById("applicationForm").reset();

            loadApplications();
        } else {
            message.className = "error-message";
            message.textContent = result.message || "申請失敗。";
        }

    } catch (error) {
        console.error(error);
        message.className = "error-message";
        message.textContent = "送出失敗，請確認後端是否有啟動。";
    }
}

async function loadApplications() {
    const list = document.getElementById("applicationList");

    list.innerHTML = "<p>申請紀錄載入中...</p>";

    try {
        const response = await fetch("/api/applications");
        const applications = await response.json();

        list.innerHTML = "";

        if (applications.length === 0) {
            list.innerHTML = "<p>目前沒有申請紀錄。</p>";
            return;
        }

        applications.forEach(app => {
            const card = document.createElement("div");
            card.className = "card";

            card.innerHTML = `
                <h3>${app.campsiteName}</h3>
                <p><strong>申請人：</strong>${app.userName}</p>
                <p><strong>入住日期：</strong>${formatDate(app.startDate)}</p>
                <p><strong>離開日期：</strong>${formatDate(app.endDate)}</p>
                <p><strong>人數：</strong>${app.peopleCount} 人</p>
                <p><strong>狀態：</strong><span class="status ${app.status}">${translateStatus(app.status)}</span></p>
                <p><strong>備註：</strong>${app.note || "無"}</p>
            `;

            list.appendChild(card);
        });

    } catch (error) {
        console.error(error);
        list.innerHTML = "<p>申請紀錄載入失敗。</p>";
    }
}

function formatDate(dateString) {
    if (!dateString) {
        return "";
    }

    return dateString.substring(0, 10);
}

function translateStatus(status) {
    if (status === "pending") {
        return "待審核";
    }

    if (status === "approved") {
        return "已通過";
    }

    if (status === "rejected") {
        return "已拒絕";
    }

    if (status === "cancelled") {
        return "已取消";
    }

    return status;
}
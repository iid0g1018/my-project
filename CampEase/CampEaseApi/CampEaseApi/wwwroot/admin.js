window.onload = function () {
    loadApplications();
};

async function loadApplications() {
    const list = document.getElementById("applicationList");
    const message = document.getElementById("message");

    list.innerHTML = "<p>申請資料載入中...</p>";
    message.textContent = "";

    try {
        const response = await fetch("/api/applications");
        const applications = await response.json();

        list.innerHTML = "";

        if (applications.length === 0) {
            list.innerHTML = "<p>目前沒有任何營地申請。</p>";
            return;
        }

        applications.forEach(app => {
            const card = document.createElement("div");
            card.className = "card";

            card.innerHTML = `
                <h3>${app.campsiteName}</h3>
                <p><strong>申請編號：</strong>${app.applicationId}</p>
                <p><strong>申請人：</strong>${app.userName}</p>
                <p><strong>入住日期：</strong>${formatDate(app.startDate)}</p>
                <p><strong>離開日期：</strong>${formatDate(app.endDate)}</p>
                <p><strong>人數：</strong>${app.peopleCount} 人</p>
                <p><strong>目前狀態：</strong><span class="status ${app.status}">${translateStatus(app.status)}</span></p>
                <p><strong>備註：</strong>${app.note || "無"}</p>

                <div class="button-group">
                    <button onclick="updateStatus(${app.applicationId}, 'approved')">通過</button>
                    <button class="danger" onclick="updateStatus(${app.applicationId}, 'rejected')">拒絕</button>
                    <button class="secondary" onclick="updateStatus(${app.applicationId}, 'cancelled')">取消</button>
                </div>
            `;

            list.appendChild(card);
        });

    } catch (error) {
        console.error(error);
        list.innerHTML = "<p>申請資料載入失敗，請確認後端是否有啟動。</p>";
    }
}

async function updateStatus(applicationId, status) {
    const message = document.getElementById("message");

    try {
        const response = await fetch(`/api/applications/${applicationId}/status`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                status: status
            })
        });

        const result = await response.json();

        if (response.ok) {
            message.className = "success-message";
            message.textContent = `申請編號 ${applicationId} 已更新為：${translateStatus(status)}`;

            loadApplications();
        } else {
            message.className = "error-message";
            message.textContent = result.message || "狀態更新失敗。";
        }

    } catch (error) {
        console.error(error);
        message.className = "error-message";
        message.textContent = "更新失敗，請確認後端是否有啟動。";
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
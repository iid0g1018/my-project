window.onload = function () {
    loadRentalItems();
    loadRentalOrders();
};

async function loadRentalItems() {
    const list = document.getElementById("rentalItemList");
    const category = document.getElementById("category").value;

    list.innerHTML = "<p>用品資料載入中...</p>";

    let url = "/api/rental-items";

    if (category) {
        url += "?category=" + encodeURIComponent(category);
    }

    try {
        const response = await fetch(url);
        const items = await response.json();

        list.innerHTML = "";

        if (items.length === 0) {
            list.innerHTML = "<p>目前沒有符合條件的露營用品。</p>";
            return;
        }

        items.forEach(item => {
            const card = document.createElement("div");
            card.className = "card";

            card.innerHTML = `
                <h3>${item.itemName}</h3>
                <p><strong>分類：</strong>${item.category}</p>
                <p><strong>每日租金：</strong>NT$ ${item.rentPricePerDay}</p>
                <p><strong>可租數量：</strong>${item.stockAvailable} / ${item.stockTotal}</p>
                <p>${item.description}</p>

                <label>
                    租借數量：
                    <input 
                        type="number" 
                        min="0" 
                        max="${item.stockAvailable}" 
                        value="0"
                        class="quantity-input"
                        data-item-id="${item.itemId}">
                </label>
            `;

            list.appendChild(card);
        });

    } catch (error) {
        console.error(error);
        list.innerHTML = "<p>用品資料載入失敗，請確認後端是否有啟動。</p>";
    }
}

function resetRentalSearch() {
    document.getElementById("category").value = "";
    loadRentalItems();
}

async function submitRentalOrder() {
    const message = document.getElementById("message");

    const startDate = document.getElementById("startDate").value;
    const endDate = document.getElementById("endDate").value;

    if (!startDate || !endDate) {
        message.className = "error-message";
        message.textContent = "請選擇租借開始日期與歸還日期。";
        return;
    }

    if (endDate <= startDate) {
        message.className = "error-message";
        message.textContent = "歸還日期必須晚於租借開始日期。";
        return;
    }

    const quantityInputs = document.querySelectorAll(".quantity-input");

    const items = [];

    quantityInputs.forEach(input => {
        const quantity = parseInt(input.value);
        const itemId = parseInt(input.dataset.itemId);

        if (quantity > 0) {
            items.push({
                itemId: itemId,
                quantity: quantity
            });
        }
    });

    if (items.length === 0) {
        message.className = "error-message";
        message.textContent = "請至少選擇一項露營用品。";
        return;
    }

    const requestBody = {
        userId: 1,
        startDate: startDate,
        endDate: endDate,
        items: items
    };

    try {
        const response = await fetch("/api/rental-orders", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(requestBody)
        });

        const result = await response.json();

        if (response.ok) {
            message.className = "success-message";
            message.textContent = `租借訂單建立成功！訂單編號：${result.rentalOrderId}，總金額：NT$ ${result.totalAmount}`;

            loadRentalItems();
            loadRentalOrders();
        } else {
            message.className = "error-message";
            message.textContent = result.message || "租借訂單建立失敗。";
        }

    } catch (error) {
        console.error(error);
        message.className = "error-message";
        message.textContent = "送出失敗，請確認後端是否有啟動。";
    }
}

async function loadRentalOrders() {
    const list = document.getElementById("rentalOrderList");

    list.innerHTML = "<p>租借紀錄載入中...</p>";

    try {
        const response = await fetch("/api/rental-orders");
        const orders = await response.json();

        list.innerHTML = "";

        if (orders.length === 0) {
            list.innerHTML = "<p>目前沒有租借紀錄。</p>";
            return;
        }

        orders.forEach(order => {
            const card = document.createElement("div");
            card.className = "card";

            card.innerHTML = `
                <h3>${order.itemName}</h3>
                <p><strong>訂單編號：</strong>${order.rentalOrderId}</p>
                <p><strong>租借人：</strong>${order.userName}</p>
                <p><strong>租借日期：</strong>${formatDate(order.startDate)} ～ ${formatDate(order.endDate)}</p>
                <p><strong>數量：</strong>${order.quantity}</p>
                <p><strong>小計：</strong>NT$ ${order.subtotal}</p>
                <p><strong>狀態：</strong><span class="status ${order.status}">${translateRentalStatus(order.status)}</span></p>
            `;

            list.appendChild(card);
        });

    } catch (error) {
        console.error(error);
        list.innerHTML = "<p>租借紀錄載入失敗。</p>";
    }
}

function formatDate(dateString) {
    if (!dateString) {
        return "";
    }

    return dateString.substring(0, 10);
}

function translateRentalStatus(status) {
    if (status === "pending") {
        return "待審核";
    }

    if (status === "approved") {
        return "已通過";
    }

    if (status === "returned") {
        return "已歸還";
    }

    if (status === "cancelled") {
        return "已取消";
    }

    return status;
}
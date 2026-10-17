window.onload = function () {
    loadSummary();
    loadRegionStats();
    loadTagStats();
    loadRentalPopularStats();
};

async function loadSummary() {
    const container = document.getElementById("summaryCards");

    container.innerHTML = "<p>系統總覽載入中...</p>";

    try {
        const response = await fetch("/api/statistics/summary");
        const data = await response.json();

        container.innerHTML = `
            <div class="summary-card">
                <h3>${data.totalCampsites}</h3>
                <p>營地總數</p>
            </div>

            <div class="summary-card">
                <h3>${data.totalApplications}</h3>
                <p>申請總數</p>
            </div>

            <div class="summary-card">
                <h3>${data.approvedApplications}</h3>
                <p>已通過申請</p>
            </div>

            <div class="summary-card">
                <h3>${data.totalRentalOrders}</h3>
                <p>租借訂單數</p>
            </div>

            <div class="summary-card">
                <h3>NT$ ${data.totalRentalRevenue}</h3>
                <p>租借總收入</p>
            </div>
        `;

    } catch (error) {
        console.error(error);
        container.innerHTML = "<p>系統總覽載入失敗。</p>";
    }
}

async function loadRegionStats() {
    const container = document.getElementById("regionStats");

    container.innerHTML = "<p>地區統計載入中...</p>";

    try {
        const response = await fetch("/api/statistics/regions");
        const data = await response.json();

        if (data.length === 0) {
            container.innerHTML = "<p>目前沒有地區統計資料。</p>";
            return;
        }

        let html = `
            <table class="data-table">
                <thead>
                    <tr>
                        <th>縣市</th>
                        <th>地區</th>
                        <th>營地數量</th>
                        <th>平均價格</th>
                        <th>最低價格</th>
                        <th>最高價格</th>
                    </tr>
                </thead>
                <tbody>
        `;

        data.forEach(item => {
            html += `
                <tr>
                    <td>${item.county}</td>
                    <td>${item.regionName}</td>
                    <td>${item.campsiteCount}</td>
                    <td>NT$ ${Math.round(item.avgPrice)}</td>
                    <td>NT$ ${item.minPrice}</td>
                    <td>NT$ ${item.maxPrice}</td>
                </tr>
            `;
        });

        html += `
                </tbody>
            </table>
        `;

        container.innerHTML = html;

    } catch (error) {
        console.error(error);
        container.innerHTML = "<p>地區統計載入失敗。</p>";
    }
}

async function loadTagStats() {
    const container = document.getElementById("tagStats");

    container.innerHTML = "<p>標籤統計載入中...</p>";

    try {
        const response = await fetch("/api/statistics/tags");
        const data = await response.json();

        if (data.length === 0) {
            container.innerHTML = "<p>目前沒有標籤統計資料。</p>";
            return;
        }

        const maxCount = Math.max(...data.map(item => item.campsiteCount));

        let html = "";

        data.forEach(item => {
            const width = maxCount === 0 ? 0 : (item.campsiteCount / maxCount) * 100;

            html += `
                <div class="bar-row">
                    <div class="bar-label">${item.tagName}</div>
                    <div class="bar-area">
                        <div class="bar" style="width: ${width}%"></div>
                    </div>
                    <div class="bar-value">${item.campsiteCount} 個營地</div>
                </div>
            `;
        });

        container.innerHTML = html;

    } catch (error) {
        console.error(error);
        container.innerHTML = "<p>標籤統計載入失敗。</p>";
    }
}

async function loadRentalPopularStats() {
    const container = document.getElementById("rentalPopularStats");

    container.innerHTML = "<p>熱門租借用品載入中...</p>";

    try {
        const response = await fetch("/api/statistics/rental-popular");
        const data = await response.json();

        if (data.length === 0) {
            container.innerHTML = "<p>目前沒有租借用品統計資料。</p>";
            return;
        }

        let html = `
            <table class="data-table">
                <thead>
                    <tr>
                        <th>用品名稱</th>
                        <th>分類</th>
                        <th>每日租金</th>
                        <th>租借數量</th>
                        <th>租借收入</th>
                    </tr>
                </thead>
                <tbody>
        `;

        data.forEach(item => {
            html += `
                <tr>
                    <td>${item.itemName}</td>
                    <td>${item.category}</td>
                    <td>NT$ ${item.rentPricePerDay}</td>
                    <td>${item.totalQuantity}</td>
                    <td>NT$ ${item.totalRevenue}</td>
                </tr>
            `;
        });

        html += `
                </tbody>
            </table>
        `;

        container.innerHTML = html;

    } catch (error) {
        console.error(error);
        container.innerHTML = "<p>熱門租借用品載入失敗。</p>";
    }
}
USE CampEaseDB;
GO

IF NOT EXISTS (
    SELECT * FROM sys.tables WHERE name = 'RentalItems'
)
BEGIN
    CREATE TABLE RentalItems (
        item_id INT IDENTITY(1,1) PRIMARY KEY,
        item_name NVARCHAR(100) NOT NULL,
        category NVARCHAR(50) NOT NULL,
        rent_price_per_day INT NOT NULL,
        stock_total INT NOT NULL,
        stock_available INT NOT NULL,
        description NVARCHAR(300),
        status NVARCHAR(20) DEFAULT 'available'
    );
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.tables WHERE name = 'RentalOrders'
)
BEGIN
    CREATE TABLE RentalOrders (
        rental_order_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL,
        start_date DATE NOT NULL,
        end_date DATE NOT NULL,
        status NVARCHAR(20) DEFAULT 'pending',
        created_at DATETIME DEFAULT GETDATE(),

        CONSTRAINT FK_RentalOrders_Users
        FOREIGN KEY (user_id) REFERENCES Users(user_id)
    );
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.tables WHERE name = 'RentalOrderDetails'
)
BEGIN
    CREATE TABLE RentalOrderDetails (
        detail_id INT IDENTITY(1,1) PRIMARY KEY,
        rental_order_id INT NOT NULL,
        item_id INT NOT NULL,
        quantity INT NOT NULL,
        subtotal INT NOT NULL,

        CONSTRAINT FK_RentalOrderDetails_RentalOrders
        FOREIGN KEY (rental_order_id) REFERENCES RentalOrders(rental_order_id),

        CONSTRAINT FK_RentalOrderDetails_RentalItems
        FOREIGN KEY (item_id) REFERENCES RentalItems(item_id)
    );
END
GO

USE CampEaseDB;
GO

IF NOT EXISTS (
    SELECT * FROM RentalItems WHERE item_name = N'四人帳篷'
)
BEGIN
    INSERT INTO RentalItems 
    (item_name, category, rent_price_per_day, stock_total, stock_available, description, status)
    VALUES
    (N'四人帳篷', N'帳篷', 300, 10, 10, N'適合 3 到 4 人使用，附營釘與外帳。', 'available'),
    (N'雙人帳篷', N'帳篷', 220, 8, 8, N'適合情侶或兩人露營使用。', 'available'),
    (N'睡袋', N'寢具', 80, 30, 30, N'保暖睡袋，適合一般低中海拔營地。', 'available'),
    (N'防潮墊', N'寢具', 60, 25, 25, N'可隔絕地面濕氣，適合新手露營。', 'available'),
    (N'露營燈', N'照明', 50, 20, 20, N'夜間照明使用，可充電。', 'available'),
    (N'卡式爐', N'炊具', 100, 12, 12, N'適合簡易煮食，需自備瓦斯罐。', 'available'),
    (N'野餐桌', N'家具', 120, 10, 10, N'折疊式野餐桌，適合 4 人使用。', 'available'),
    (N'露營椅', N'家具', 50, 40, 40, N'折疊椅，輕量好攜帶。', 'available'),
    (N'保冷箱', N'食物保存', 100, 8, 8, N'可保存飲料與食材。', 'available'),
    (N'延長線', N'電器', 40, 15, 15, N'適合有電源營位使用。', 'available');
END
GO

SELECT * FROM RentalItems;

USE CampEaseDB;
GO

SELECT *
FROM Regions;
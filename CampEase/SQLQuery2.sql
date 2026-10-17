USE CampEaseDB;
GO

IF NOT EXISTS (
    SELECT * FROM sys.tables WHERE name = 'Users'
)
BEGIN
    CREATE TABLE Users (
        user_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(50) NOT NULL,
        email NVARCHAR(100),
        phone NVARCHAR(20),
        role NVARCHAR(20) DEFAULT 'user',
        created_at DATETIME DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.tables WHERE name = 'CampApplications'
)
BEGIN
    CREATE TABLE CampApplications (
        application_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL,
        campsite_id INT NOT NULL,
        start_date DATE NOT NULL,
        end_date DATE NOT NULL,
        people_count INT NOT NULL,
        status NVARCHAR(20) DEFAULT 'pending',
        note NVARCHAR(300),
        created_at DATETIME DEFAULT GETDATE(),

        CONSTRAINT FK_CampApplications_Users
        FOREIGN KEY (user_id) REFERENCES Users(user_id),

        CONSTRAINT FK_CampApplications_Campsites
        FOREIGN KEY (campsite_id) REFERENCES Campsites(campsite_id)
    );
END
GO

USE CampEaseDB;
GO

IF NOT EXISTS (
    SELECT * FROM Users WHERE email = 'test@example.com'
)
BEGIN
    INSERT INTO Users (name, email, phone, role)
    VALUES (N'測試使用者', 'test@example.com', '0912345678', 'user');
END
GO

SELECT * FROM Users;

USE CampEaseDB;
GO

SELECT campsite_id, campsite_name
FROM Campsites;

USE CampEaseDB;
GO

SELECT
    a.application_id,
    u.name AS user_name,
    c.campsite_name,
    a.start_date,
    a.end_date,
    a.people_count,
    a.status,
    a.note,
    a.created_at
FROM CampApplications a
JOIN Users u ON a.user_id = u.user_id
JOIN Campsites c ON a.campsite_id = c.campsite_id;
-- 建立資料庫（若不存在）
IF DB_ID('CampEaseDB') IS NULL
    CREATE DATABASE CampEaseDB;
GO

USE CampEaseDB;
GO

-- 建立 Regions 表格（若不存在）
IF OBJECT_ID('Regions', 'U') IS NULL
BEGIN
    CREATE TABLE Regions (
        region_id INT IDENTITY(1,1) PRIMARY KEY,
        county NVARCHAR(20) NOT NULL,
        region_name NVARCHAR(50) NOT NULL
    );
END

-- 建立 Campsites 表格（若不存在）
IF OBJECT_ID('Campsites', 'U') IS NULL
BEGIN
    CREATE TABLE Campsites (
        campsite_id INT IDENTITY(1,1) PRIMARY KEY,
        region_id INT NOT NULL,
        campsite_name NVARCHAR(100) NOT NULL,
        address NVARCHAR(200),
        elevation INT CHECK (elevation >= 0),
        price_per_night INT CHECK (price_per_night > 0),
        capacity INT CHECK (capacity > 0),
        description NVARCHAR(500),
        status NVARCHAR(20) DEFAULT 'available',
        created_at DATETIME DEFAULT GETDATE(),

        CONSTRAINT FK_Campsites_Regions
        FOREIGN KEY (region_id) REFERENCES Regions(region_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
    );
END

-- 建立 Tags 表格（若不存在）
IF OBJECT_ID('Tags', 'U') IS NULL
BEGIN
    CREATE TABLE Tags (
        tag_id INT IDENTITY(1,1) PRIMARY KEY,
        tag_name NVARCHAR(50) NOT NULL UNIQUE
    );
END

-- 建立 CampsiteTags 表格（若不存在）
IF OBJECT_ID('CampsiteTags', 'U') IS NULL
BEGIN
    CREATE TABLE CampsiteTags (
        campsite_id INT NOT NULL,
        tag_id INT NOT NULL,

        PRIMARY KEY (campsite_id, tag_id),

        CONSTRAINT FK_CampsiteTags_Campsites
        FOREIGN KEY (campsite_id) REFERENCES Campsites(campsite_id)
        ON DELETE CASCADE,

        CONSTRAINT FK_CampsiteTags_Tags
        FOREIGN KEY (tag_id) REFERENCES Tags(tag_id)
        ON DELETE CASCADE
    );
END

-- 建立 Facilities 表格（若不存在）
IF OBJECT_ID('Facilities', 'U') IS NULL
BEGIN
    CREATE TABLE Facilities (
        facility_id INT IDENTITY(1,1) PRIMARY KEY,
        facility_name NVARCHAR(50) NOT NULL UNIQUE
    );
END

-- 建立 CampsiteFacilities 表格（若不存在）
IF OBJECT_ID('CampsiteFacilities', 'U') IS NULL
BEGIN
    CREATE TABLE CampsiteFacilities (
        campsite_id INT NOT NULL,
        facility_id INT NOT NULL,
        quantity INT DEFAULT 1 CHECK (quantity > 0),
        note NVARCHAR(200),

        PRIMARY KEY (campsite_id, facility_id),

        CONSTRAINT FK_CampsiteFacilities_Campsites
        FOREIGN KEY (campsite_id) REFERENCES Campsites(campsite_id)
        ON DELETE CASCADE,

        CONSTRAINT FK_CampsiteFacilities_Facilities
        FOREIGN KEY (facility_id) REFERENCES Facilities(facility_id)
        ON DELETE CASCADE
    );
END

-- 插入 Regions（避免重複）
INSERT INTO Regions (county, region_name)
SELECT N'桃園市', N'復興區' WHERE NOT EXISTS (SELECT 1 FROM Regions WHERE county=N'桃園市' AND region_name=N'復興區');
INSERT INTO Regions (county, region_name)
SELECT N'新竹縣', N'尖石鄉' WHERE NOT EXISTS (SELECT 1 FROM Regions WHERE county=N'新竹縣' AND region_name=N'尖石鄉');
INSERT INTO Regions (county, region_name)
SELECT N'苗栗縣', N'南庄鄉' WHERE NOT EXISTS (SELECT 1 FROM Regions WHERE county=N'苗栗縣' AND region_name=N'南庄鄉');
INSERT INTO Regions (county, region_name)
SELECT N'苗栗縣', N'泰安鄉' WHERE NOT EXISTS (SELECT 1 FROM Regions WHERE county=N'苗栗縣' AND region_name=N'泰安鄉');
INSERT INTO Regions (county, region_name)
SELECT N'南投縣', N'埔里鎮' WHERE NOT EXISTS (SELECT 1 FROM Regions WHERE county=N'南投縣' AND region_name=N'埔里鎮');
INSERT INTO Regions (county, region_name)
SELECT N'宜蘭縣', N'大同鄉' WHERE NOT EXISTS (SELECT 1 FROM Regions WHERE county=N'宜蘭縣' AND region_name=N'大同鄉');

-- 插入 Tags（避免重複）
INSERT INTO Tags (tag_name)
SELECT N'新手友善' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'新手友善');
INSERT INTO Tags (tag_name)
SELECT N'低海拔' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'低海拔');
INSERT INTO Tags (tag_name)
SELECT N'中海拔' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'中海拔');
INSERT INTO Tags (tag_name)
SELECT N'高海拔' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'高海拔');
INSERT INTO Tags (tag_name)
SELECT N'親子友善' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'親子友善');
INSERT INTO Tags (tag_name)
SELECT N'寵物友善' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'寵物友善');
INSERT INTO Tags (tag_name)
SELECT N'星空景觀' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'星空景觀');
INSERT INTO Tags (tag_name)
SELECT N'近溪流' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'近溪流');
INSERT INTO Tags (tag_name)
SELECT N'森林系' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'森林系');
INSERT INTO Tags (tag_name)
SELECT N'平價' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'平價');
INSERT INTO Tags (tag_name)
SELECT N'安靜' WHERE NOT EXISTS (SELECT 1 FROM Tags WHERE tag_name=N'安靜');

-- 插入 Facilities（避免重複）
INSERT INTO Facilities (facility_name)
SELECT N'廁所' WHERE NOT EXISTS (SELECT 1 FROM Facilities WHERE facility_name=N'廁所');
INSERT INTO Facilities (facility_name)
SELECT N'熱水澡' WHERE NOT EXISTS (SELECT 1 FROM Facilities WHERE facility_name=N'熱水澡');
INSERT INTO Facilities (facility_name)
SELECT N'電源' WHERE NOT EXISTS (SELECT 1 FROM Facilities WHERE facility_name=N'電源');
INSERT INTO Facilities (facility_name)
SELECT N'飲水機' WHERE NOT EXISTS (SELECT 1 FROM Facilities WHERE facility_name=N'飲水機');
INSERT INTO Facilities (facility_name)
SELECT N'停車場' WHERE NOT EXISTS (SELECT 1 FROM Facilities WHERE facility_name=N'停車場');
INSERT INTO Facilities (facility_name)
SELECT N'冰箱' WHERE NOT EXISTS (SELECT 1 FROM Facilities WHERE facility_name=N'冰箱');
INSERT INTO Facilities (facility_name)
SELECT N'雨棚' WHERE NOT EXISTS (SELECT 1 FROM Facilities WHERE facility_name=N'雨棚');
INSERT INTO Facilities (facility_name)
SELECT N'Wi-Fi' WHERE NOT EXISTS (SELECT 1 FROM Facilities WHERE facility_name=N'Wi-Fi');
INSERT INTO Facilities (facility_name)
SELECT N'烤肉區' WHERE NOT EXISTS (SELECT 1 FROM Facilities WHERE facility_name=N'烤肉區');
INSERT INTO Facilities (facility_name)
SELECT N'洗手台' WHERE NOT EXISTS (SELECT 1 FROM Facilities WHERE facility_name=N'洗手台');

SELECT * FROM Regions;
SELECT * FROM Campsites;
SELECT * FROM Tags;
SELECT * FROM Facilities;

-- join
SELECT 
    c.campsite_id,
    c.campsite_name,
    r.county,
    r.region_name,
    c.elevation,
    c.price_per_night,
    c.capacity
FROM Campsites c
JOIN Regions r ON c.region_id = r.region_id;

SELECT 
    c.campsite_name,
    f.facility_name,
    cf.quantity,
    cf.note
FROM Campsites c
JOIN CampsiteFacilities cf 
    ON c.campsite_id = cf.campsite_id
JOIN Facilities f 
    ON cf.facility_id = f.facility_id
WHERE c.campsite_id = 1;

SELECT 
    c.campsite_name,
    c.elevation,
    c.price_per_night,
    t.tag_name
FROM Campsites c
JOIN CampsiteTags ct 
    ON c.campsite_id = ct.campsite_id
JOIN Tags t 
    ON ct.tag_id = t.tag_id
WHERE t.tag_name = N'新手友善';
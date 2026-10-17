USE CampEaseDB;
GO

SELECT * FROM Tags;
SELECT * FROM Facilities;

--´ú¸Õ§R°£
USE CampEaseDB;
GO

SELECT
    campsite_id,
    campsite_name,
    status
FROM Campsites
WHERE status = 'closed';
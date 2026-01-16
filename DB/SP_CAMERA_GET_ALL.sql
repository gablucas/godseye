CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_CAMERA_GET_ALL`()
BEGIN
    SELECT 
		c.Id,
		c.Name,
		c.Connection,
		c.Sector_Id AS SectorId,
		s.Name AS SectorName
	FROM CAMERA c
	LEFT JOIN SECTOR s ON c.SECTOR_ID = s.Id;
END
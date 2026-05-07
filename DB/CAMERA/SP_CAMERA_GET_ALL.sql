CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_CAMERA_GET_ALL`()
BEGIN
    SELECT 
		C.Id,
		D.Name,
        D.ID AS DeviceId,
		C.Connection,
        D.IS_ACTIVE AS IsActive,
		D.ORIGIN_SECTOR_ID AS SectorId,
		S.Name AS SectorName,
        NULL AS RoiJson
	FROM CAMERA C
    LEFT JOIN DEVICE D ON D.ID = C.DEVICE_ID
	LEFT JOIN SECTOR S ON S.ID = D.ORIGIN_SECTOR_ID
    GROUP BY
		C.ID,
        D.NAME,
        C.CONNECTION,
        D.ORIGIN_SECTOR_ID,
        S.Name,
        D.IS_ACTIVE;
END
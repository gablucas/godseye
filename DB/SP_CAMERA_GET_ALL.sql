CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_CAMERA_GET_ALL`()
BEGIN
    SELECT 
		c.Id,
		c.Name,
		c.Connection,
		c.Sector_Id AS SectorId,
		s.Name AS SectorName,
        CASE
			WHEN COUNT(F.ID) = 0 THEN NULL
            ELSE JSON_ARRAYAGG(
					JSON_OBJECT (
					"FeatureId", F.ID,
					"FeatureName", F.NAME
					)
				) 
			END AS FeaturesJson
	FROM CAMERA c
	LEFT JOIN SECTOR s ON c.SECTOR_ID = s.Id
    LEFT JOIN CAMERA_FEATURE CF ON CF.CAMERA_ID = C.ID
    LEFT JOIN FEATURE F ON F.ID = CF.FEATURE_ID
    GROUP BY
		C.ID,
        C.NAME,
        C.CONNECTION,
        C.Sector_ID,
        S.Name;
END
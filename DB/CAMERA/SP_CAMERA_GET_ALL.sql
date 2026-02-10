CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_CAMERA_GET_ALL`()
BEGIN
    SELECT 
		c.Id,
		c.Name,
		c.Connection,
        C.IS_ACTIVE AS IsActive,
		c.Sector_Id AS SectorId,
		s.Name AS SectorName,
        NULL AS RoiJson,
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
    LEFT JOIN CAMERA_FEATURE CF ON CF.CAMERA_ID = C.ID AND CF.IS_ACTIVE = 1
    LEFT JOIN FEATURE F ON F.ID = CF.FEATURE_ID
    GROUP BY
		C.ID,
        C.NAME,
        C.CONNECTION,
        C.Sector_ID,
        S.Name,
        C.IS_ACTIVE;
END
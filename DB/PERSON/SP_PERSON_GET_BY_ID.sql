CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_PERSON_GET_BY_ID`(
	IN P_PERSON_ID INT
)
BEGIN
    SELECT 
        P.ID,
        P.NAME,
        P.IMAGE_PATH AS ImagePath,
        P.ACTIVE,
        CASE 
            WHEN COUNT(S.ID) = 0 THEN NULL
			ELSE JSON_ARRAYAGG(
				JSON_OBJECT(
					'SectorId', S.ID,
					'SectorName', S.NAME
				)
			)
		END AS Sectors
    FROM PERSON P
    LEFT JOIN PERSON_SECTOR PS ON PS.PERSON_ID = P.ID
    LEFT JOIN SECTOR S ON S.ID = PS.SECTOR_ID
    WHERE P.ID = P_PERSON_ID
    GROUP BY 
        P.ID,
        P.NAME,
        P.IMAGE_PATH
	ORDER BY P.CREATED_AT DESC;
END
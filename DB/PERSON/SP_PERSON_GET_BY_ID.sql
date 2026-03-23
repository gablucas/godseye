CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_PERSON_GET_BY_ID`(
	IN P_PERSON_ID INT
)
BEGIN
    SELECT 
        P.ID,
        P.NAME,
        P.IMAGE_PATH,
        P.ACTIVE,
        JSON_OBJECT(
			'Id', S.ID,
			'Name', S.NAME
		) AS SECTOR,
        (
			SELECT JSON_OBJECT(
				'Id', AL.ID,
				'Name', AL.NAME
			)
            FROM ACCESS_LEVEL AL
            WHERE AL.ID = P.ACCESS_LEVEL_ID
        ) AS ACCESSLEVEL
    FROM PERSON P
    LEFT JOIN SECTOR S ON S.ID = P.MAIN_SECTOR_ID
    GROUP BY 
        P.ID,
        P.NAME,
        P.IMAGE_PATH,
        P.MAIN_SECTOR_ID,
        P.ACCESS_LEVEL_ID
	ORDER BY P.CREATED_AT DESC;
END
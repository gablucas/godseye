CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_PERSON_GET_ALL`()
BEGIN
    SELECT 
        P.ID,
        P.NAME,
        P.IMAGE_PATH AS ImagePath,
        P.ACTIVE,
        (
			SELECT JSON_OBJECT(
				'Id', S.ID,
				'Name', S.NAME
			)
			FROM SECTOR S
			WHERE S.ID = P.MAIN_SECTOR_ID
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
    ORDER BY P.CREATED_AT DESC;
END
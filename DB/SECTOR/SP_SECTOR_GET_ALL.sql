CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_SECTOR_GET_ALL`()
BEGIN
    SELECT 
        S.ID,
        S.NAME,
        S.ACTIVE,
        CASE 
            WHEN COUNT(C.ID) = 0 THEN NULL
            ELSE JSON_ARRAYAGG(
                JSON_OBJECT(
                    'Id', C.ID,
                    'Name', C.NAME
                )
            )
        END AS CamerasJson,
        CASE
			WHEN COUNT(NG.ID) = 0 THEN NULL
				ELSE JSON_ARRAYAGG(
					JSON_OBJECT(
						'Id', NG.ID,
						'Name', NG.NAME
					)
				)
			END AS NotificationGroupsJson
    FROM SECTOR S
    LEFT JOIN CAMERA C ON C.SECTOR_ID = S.ID
    LEFT JOIN SECTOR_NOTIFICATION_GROUP SNG ON SNG.SECTOR_ID = S.ID
    LEFT JOIN NOTIFICATION_GROUP NG ON NG.ID = SNG.NOTIFICATION_GROUP_ID
    GROUP BY S.ID, S.NAME, S.ACTIVE;
END
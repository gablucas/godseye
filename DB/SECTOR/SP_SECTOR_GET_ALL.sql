CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_SECTOR_GET_ALL`()
BEGIN
    SELECT 
        S.ID,
        S.NAME,
        S.PARENT_ID,
        S.IS_ACTIVE,
        CASE 
            WHEN COUNT(D.ID) = 0 THEN NULL
            ELSE JSON_ARRAYAGG(
                JSON_OBJECT(
                    'Id', D.ID,
                    'Name', D.NAME
                )
            )
        END AS Devices,
        CASE
			WHEN COUNT(NG.ID) = 0 THEN NULL
				ELSE JSON_ARRAYAGG(
					JSON_OBJECT(
						'Id', NG.ID,
						'Name', NG.NAME
					)
				)
			END AS NotificationGroups
    FROM SECTOR S
    LEFT JOIN DEVICE D ON D.ORIGIN_SECTOR_ID = S.ID
    LEFT JOIN SECTOR_NOTIFICATION_GROUP SNG ON SNG.SECTOR_ID = S.ID
    LEFT JOIN NOTIFICATION_GROUP NG ON NG.ID = SNG.NOTIFICATION_GROUP_ID
    GROUP BY S.ID, S.NAME, S.IS_ACTIVE;
END
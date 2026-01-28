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
                    'CameraId', C.ID,
                    'CameraName', C.NAME
                )
            )
        END AS CamerasJson
    FROM SECTOR S
    LEFT JOIN CAMERA C ON C.SECTOR_ID = S.ID
    GROUP BY S.ID, S.NAME, S.ACTIVE;
END
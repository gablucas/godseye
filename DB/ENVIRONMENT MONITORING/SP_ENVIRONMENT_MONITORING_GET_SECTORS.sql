CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_ENVIRONMENT_MONITORING_GET_SECTORS`()
BEGIN
    SELECT
        S.ID AS SectorId,
        S.NAME AS SectorName,
        COUNT(P.ID) AS TotalPerson,
        CASE
            WHEN COUNT(P.ID) = 0 THEN NULL
            ELSE JSON_ARRAYAGG(
                JSON_OBJECT(
                    'PersonId', P.ID,
                    'PersonName', P.NAME,
                    'PersonPhoto', P.IMAGE_PATH,
                    'CreatedAt', DATE_FORMAT(EM.CREATED_AT, '%Y-%m-%dT%H:%i:%s')
                )
            )
        END AS PersonJSON
    FROM SECTOR S
    LEFT JOIN CAMERA C 
        ON C.SECTOR_ID = S.ID
    LEFT JOIN ENVIRONMENT_MONITORING EM
        ON EM.ID = (
            SELECT EM2.ID
            FROM ENVIRONMENT_MONITORING EM2
            WHERE EM2.CAMERA_ID = C.ID
              AND EM2.CREATED_AT >= CURDATE()
              AND EM2.CREATED_AT < CURDATE() + INTERVAL 1 DAY
            ORDER BY EM2.CREATED_AT DESC
            LIMIT 1
        )
    LEFT JOIN PERSON P 
        ON P.ID = EM.PERSON_ID
    GROUP BY
        S.ID,
        S.NAME

    UNION ALL

    /* ===========================
       FORA DA EMPRESA
       =========================== */
    SELECT
        0 AS SectorId,
        'Fora da empresa' AS SectorName,
        COUNT(P.ID) AS TotalPeople,
        JSON_ARRAYAGG(
            JSON_OBJECT(
                'PersonId', P.ID,
                'PersonName', P.NAME,
                'PersonPhoto', P.IMAGE_PATH,
                'CreatedAt', NULL
            )
        ) AS PersonJSON
    FROM PERSON P
    WHERE NOT EXISTS (
        SELECT 1
        FROM ENVIRONMENT_MONITORING EM
        WHERE EM.PERSON_ID = P.ID
          AND EM.CREATED_AT >= CURDATE()
          AND EM.CREATED_AT < CURDATE() + INTERVAL 1 DAY
    );
END
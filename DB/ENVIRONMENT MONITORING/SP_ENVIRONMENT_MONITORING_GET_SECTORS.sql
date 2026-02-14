CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_ENVIRONMENT_MONITORING_GET_SECTORS`()
BEGIN
    -- CTE (Common Table Expression) para isolar a lógica de "Visto Hoje"
    -- Isso evita repetir esse código pesado duas vezes e melhora a performance (MySQL 8.0+)
    WITH PeopleSeenToday AS (
        SELECT
            EM1.PERSON_ID,
            EM1.CREATED_AT AS LastSeen,
            EM1.CAMERA_ID
        FROM ENVIRONMENT_MONITORING EM1
        INNER JOIN (
            SELECT
                PERSON_ID,
                MAX(CREATED_AT) AS LastSeen
            FROM ENVIRONMENT_MONITORING
            WHERE CREATED_AT >= CURDATE()
              AND CREATED_AT < CURDATE() + INTERVAL 1 DAY
            GROUP BY PERSON_ID
        ) LAST_EM
        ON LAST_EM.PERSON_ID = EM1.PERSON_ID
        AND LAST_EM.LastSeen = EM1.CREATED_AT
    )

    -- SETORES - Com e sem pessoas
    SELECT
        S.ID   AS SectorId,
        S.NAME AS SectorName,
        COUNT(P.ID) AS TotalPerson,
        CASE
            WHEN COUNT(P.ID) = 0 THEN NULL
            ELSE JSON_ARRAYAGG(
                JSON_OBJECT(
                    'PersonId', P.ID,
                    'PersonName', P.NAME,
                    'PersonPhoto', P.IMAGE_PATH,
                    'CreatedAt', DATE_FORMAT(PST.LastSeen, '%Y-%m-%dT%H:%i:%s')
                )
            )
        END AS Person
    FROM SECTOR S
    LEFT JOIN CAMERA C ON C.SECTOR_ID = S.ID
    LEFT JOIN PeopleSeenToday PST ON PST.CAMERA_ID = C.ID
    LEFT JOIN PERSON P ON P.ID = PST.PERSON_ID
    GROUP BY S.ID, S.NAME

    UNION ALL

    -- PARTE 2: Setor "Fora da Empresa" (Pessoas não vistas hoje)
    SELECT
        0 AS SectorId,
        'Fora da empresa' AS SectorName,
        COUNT(P.ID) AS TotalPerson,
        CASE
            WHEN COUNT(P.ID) = 0 THEN NULL
            ELSE JSON_ARRAYAGG(
                JSON_OBJECT(
                    'PersonId', P.ID,
                    'PersonName', P.NAME,
                    'PersonPhoto', P.IMAGE_PATH,
                    'CreatedAt', NULL -- Não foi visto hoje, então data é NULL
                )
            )
        END AS Person
    FROM PERSON P
    LEFT JOIN PeopleSeenToday PST ON PST.PERSON_ID = P.ID
    WHERE PST.PERSON_ID IS NULL -- Pega apenas quem NÃO está no "Visto Hoje"
    GROUP BY SectorId, SectorName
    
    ORDER BY TotalPerson DESC; 
END
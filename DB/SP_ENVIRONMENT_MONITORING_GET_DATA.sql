CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_ENVIRONMENT_MONITORING_GET_DATA`()
BEGIN
    SELECT JSON_OBJECT(
		'Persons', (
			SELECT JSON_ARRAYAGG(
				JSON_OBJECT (
					'Id', P.ID,
                    'Embedding', P.EMBEDDING
                )
            )
            FROM PERSON P
    ),
		'Cameras', (
			SELECT JSON_ARRAYAGG(
				JSON_OBJECT(
					'Id', C.ID,
					'Connection', C.Connection,
                    'SectorId', C.SECTOR_ID
				)
			)
			FROM CAMERA C
            WHERE CONNECTION IS NOT NULL
		)
	) AS Data;
END
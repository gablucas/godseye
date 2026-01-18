CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_GODSEYE_GET_MONITORING_DATA`()
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
                    'SectorId', C.SECTOR_ID,
                    'Features', (SELECT JSON_ARRAYAGG(
						JSON_OBJECT(
							'Id', F.ID,
                            'Name', F.Name
                        )
                    ) 
                    FROM FEATURE F
                    INNER JOIN CAMERA_FEATURE CF ON CF.FEATURE_ID = F.ID
                    WHERE CF.CAMERA_ID = C.ID
                    )
				)
			)
			FROM CAMERA C
            WHERE CONNECTION IS NOT NULL
		)
	) AS Data;
END
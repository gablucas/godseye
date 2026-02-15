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
                    INNER JOIN CAMERA_FEATURE CF ON CF.FEATURE_ID = F.ID AND CF.IS_ACTIVE = 1
                    WHERE CF.CAMERA_ID = C.ID
                    ),
                    'Roi', (
						SELECT JSON_ARRAYAGG(
							JSON_OBJECT(
								'RoiType', CR.ROI_TYPE,
                                'Coordinates', CR.COORDINATES_JSON
                            )
                        )
                        FROM CAMERA_ROI CR
                        WHERE CR.CAMERA_ID = C.ID AND CR.IS_ACTIVE = 1
                    )
				)
			)
			FROM CAMERA C
            WHERE CONNECTION IS NOT NULL AND C.IS_ACTIVE = 1
		)
	) AS Data;
END
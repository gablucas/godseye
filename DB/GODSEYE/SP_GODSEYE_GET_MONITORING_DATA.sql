CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_GODSEYE_GET_MONITORING_DATA`()
BEGIN
    SELECT JSON_OBJECT(
		'Cameras', (
			SELECT JSON_ARRAYAGG(
				JSON_OBJECT(
					'Id', C.ID,
					'Connection', C.Connection,
                    'DeviceId', C.DEVICE_ID,
                    'SectorId', C.SECTOR_ID,
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
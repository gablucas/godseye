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
				'Name', AL.NAME,
				'Sectors', (
					SELECT JSON_ARRAYAGG(
						JSON_OBJECT(
							'Id', S2.ID,
							'Name', S2.NAME,
							'RuleType', ALS.RULE_TYPE
						)
					)
					FROM ACCESS_LEVEL_SECTOR ALS
					LEFT JOIN SECTOR S2 ON S2.ID = ALS.SECTOR_ID
					WHERE ALS.ACCESS_LEVEL_ID = AL.ID
				),
				'AccessSchedule', (
					SELECT JSON_ARRAYAGG(
						JSON_OBJECT(
							'Id', ACCS.ID,
							'Name', ACCS.NAME,
							'Rules', (
								SELECT JSON_ARRAYAGG(
									JSON_OBJECT(
										'WeekDay', ACCSR.WEEKDAY,
										'StartTime', ACCSR.START_TIME,
										'EndTime', ACCSR.END_TIME
									)
								)
								FROM ACCESS_SCHEDULE_RULES ACCSR
								WHERE ACCSR.ACCESS_SCHEDULE_ID = ACCS.ID
							)
						)
					)
					FROM ACCESS_SCHEDULE ACCS
					WHERE ACCS.ID = AL.ACCESS_SCHEDULE_ID
				)
			)
            FROM ACCESS_LEVEL AL
            WHERE AL.ID = P.ACCESS_LEVEL_ID
        ) AS ACCESSLEVEL

    FROM PERSON P
    ORDER BY P.CREATED_AT DESC;
END
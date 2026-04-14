CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_ACCESS_LEVEL_GET_ALL`()
BEGIN
    SELECT
        AL.ID,
        AL.NAME,
        (
            SELECT COALESCE(
                JSON_ARRAYAGG(
                    JSON_OBJECT(
                        'Id', S.ID,
                        'Name', S.NAME,
                        'RuleType', CAST(ALS.RULE_TYPE AS UNSIGNED) 
                    )
                ), 
                JSON_ARRAY()
            )
            FROM ACCESS_LEVEL_SECTOR ALS
            LEFT JOIN SECTOR S ON S.ID = ALS.SECTOR_ID
            WHERE ALS.ACCESS_LEVEL_ID = AL.ID
        ) AS SECTORS,
        (
            SELECT JSON_OBJECT(
                'Id', ACCS.ID,
                'Name', ACCS.NAME,
                'Rules', COALESCE(
                    (
                        SELECT JSON_ARRAYAGG(
                            JSON_OBJECT(
                                'WeekDay', ACCSR.WEEKDAY,
                                'StartTime', ACCSR.START_TIME,
                                'EndTime', ACCSR.END_TIME
                            )
                        )
                        FROM ACCESS_SCHEDULE_RULE ACCSR
                        WHERE ACCSR.ACCESS_SCHEDULE_ID = ACCS.ID
                    ),
                    JSON_ARRAY()
                )
            )
            FROM ACCESS_SCHEDULE ACCS
            WHERE ACCS.ID = AL.ACCESS_SCHEDULE_ID
        ) AS SECTOR_SCHEDULE,
        (
            SELECT COALESCE(
                JSON_ARRAYAGG(
                    JSON_OBJECT(
                        'Id', R.ID,
                        'Name', R.NAME
                    )
                ), 
                JSON_ARRAY()
            )
            FROM ACCESS_LEVEL_ROUTINE ALR
            LEFT JOIN ROUTINE R ON R.ID = ALR.ROUTINE_ID
            WHERE ALR.ACCESS_LEVEL_ID = AL.ID
        ) AS ROUTINES
    FROM ACCESS_LEVEL AL;
END
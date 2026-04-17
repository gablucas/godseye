CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_ACCESS_LEVEL_GET_ALL_CACHE`()
BEGIN
    SELECT
		AL.ID,
        (
			SELECT JSON_ARRAYAGG(
				JSON_OBJECT(
					"Id", ALS.SECTOR_ID,
                    "RuleType", ALS.RULE_TYPE
                )
            )
            FROM ACCESS_LEVEL_SECTOR ALS
            WHERE ALS.ACCESS_LEVEL_ID = AL.ID
        ) AS Sectors,
        (
			SELECT JSON_ARRAYAGG(
				JSON_OBJECT(
					"Id", R.ID,
                    "Type", R.TYPE,
                    "Rules", (
						SELECT JSON_ARRAYAGG(
							JSON_OBJECT(
								"OrderIndex", RRST.ORDER_INDEX,
                                "MinTime", RRST.MIN_TIME,
                                "MaxTime", RRST.MAX_TIME
                            )
                        )
                        FROM ROUTINE_RULE_SECTOR_TRANSITION RRST
                        WHERE RRST.ROUTINE_ID = R.ID
                    )
                )
            )
            FROM ACCESS_LEVEL_ROUTINE ALR
            JOIN ROUTINE R ON R.ID = ALR.ROUTINE_ID
            WHERE ALR.ACCESS_LEVEL_ID = AL.ID
        ) AS Routines
    FROM ACCESS_LEVEL AL;
END
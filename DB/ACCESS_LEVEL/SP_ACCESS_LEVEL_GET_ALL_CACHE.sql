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
        ) AS Sectors
    FROM ACCESS_LEVEL AL;
END
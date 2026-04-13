CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_ROUTINE_GET_BY_ID`(
	IN P_ROUTINE_ID INT
)
BEGIN
    SELECT 
        R.ID,
        R.NAME,
        (R.TYPE - 1) AS Type,
        (
			SELECT JSON_ARRAYAGG(
				JSON_OBJECT(
					'OrderIndex', RRST.ORDER_INDEX,
                    'MinTime', RRST.MIN_TIME,
                    'MaxTime', RRST.MAX_TIME,
                    'SectorId', RRST.SECTOR_ID
                )
            )
            FROM ROUTINE_RULE_SECTOR_TRANSITION RRST WHERE RRST.ROUTINE_ID = P_ROUTINE_ID
        ) AS RULES
    FROM ROUTINE R
    WHERE R.ID = P_ROUTINE_ID
	ORDER BY R.CREATED_AT DESC;
END
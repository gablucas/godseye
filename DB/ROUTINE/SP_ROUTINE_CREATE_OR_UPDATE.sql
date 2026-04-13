CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_ROUTINE_CREATE_OR_UPDATE`(
	IN P_ID INT,
    IN P_NAME VARCHAR(100),
    IN P_TYPE enum('Transição de setores'),
    IN P_ROUTINE_RULES_JSON JSON
)
BEGIN
	DECLARE ERRO INT DEFAULT 0;
    DECLARE MENSAGEM TEXT DEFAULT "SUCESSO";
    DECLARE V_ID INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
	BEGIN
		GET DIAGNOSTICS CONDITION 1 
			@p1 = MESSAGE_TEXT;

		ROLLBACK;
		
		SELECT
			1 AS ERRO,
			@p1 as MENSAGEM,
			0 as Id;
	END;
    
    START TRANSACTION;
    
    IF P_ID IS NULL OR P_ID = 0 THEN
		
        INSERT INTO ROUTINE (NAME, TYPE)
        VALUES (P_NAME, P_TYPE);
        
        SET V_ID = LAST_INSERT_ID();
        
	ELSE
		UPDATE ROUTINE
		SET
			NAME = P_NAME,
            TYPE = P_TYPE
		WHERE ID = P_ID;
        
        SET V_ID = P_ID;
        
	END IF;
    
    DELETE FROM ROUTINE_RULE_SECTOR_TRANSITION
    WHERE ROUTINE_ID = V_ID;
    
    INSERT INTO ROUTINE_RULE_SECTOR_TRANSITION (ROUTINE_ID, SECTOR_ID, ORDER_INDEX, MIN_TIME, MAX_TIME)
    SELECT V_ID, jt.sector_id, jt.order_index, jt.min_time, jt.max_time
    FROM JSON_TABLE (
		P_ROUTINE_RULES_JSON,
        '$[*]' COLUMNS (
			sector_id INT PATH '$.SectorId',
            order_index INT PATH '$.OrderIndex',
            min_time TIME PATH '$.MinTime',
            max_time TIME PATH '$.MaxTime'
        )
    ) as jt;
    
    COMMIT;
    
    SELECT 
        ERRO AS Erro,
        MENSAGEM AS Mensagem,
        V_ID AS Id;
    
END
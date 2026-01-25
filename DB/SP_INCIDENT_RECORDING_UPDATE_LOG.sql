CREATE DEFINER=`root`@`localhost` PROCEDURE `SP_INCIDENT_RECORDING_UPDATE_LOG`(
    IN P_ID INT,
    IN P_PERSONS_IDS_JSON JSON,
    IN P_FILE_NAME VARCHAR(300)
)
BEGIN
    DECLARE ERRO INT DEFAULT 0;
    DECLARE MENSAGEM VARCHAR(100) DEFAULT 'SUCESSO';

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
	BEGIN
		ROLLBACK;

		SELECT 
			1 AS Erro,
			'ERRO AO INSERIR CAMERA' AS Mensagem,
			0 AS Id;
	END;

    START TRANSACTION;

        UPDATE INCIDENT_RECORDING
        SET 
			STATUS = 3,
            FILE_NAME = P_FILE_NAME, 
            UPDATE_AT = NOW()
        WHERE ID = P_ID;

		INSERT INTO INCIDENT_RECORDING_PERSON 
        (
			INCIDENT_RECORDING_ID,
			PERSON_ID, 
			SEEN_AT,
            VIDEO_OFFSET_SECONDS
        )
        SELECT 
			P_ID, 
            jt.person_id, 
            jt.seen_at,
            jt.video_offset_seconds
		FROM JSON_TABLE(
			P_PERSONS_IDS_JSON, 
            '$[*]' 
            COLUMNS (
				person_id INT PATH '$.PersonId',
				seen_at DATETIME PATH '$.SeenAt',
                video_offset_seconds DECIMAL(10,3) PATH '$.VideoOffsetSeconds'
            )
		) jt;

    COMMIT;

    SELECT 
        ERRO AS Erro,
        MENSAGEM AS Mensagem,
        P_ID AS Id;
END
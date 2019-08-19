ALTER TRIGGER TRIG_ACT_Note_Record_Add
ON ACT_NOTE_RECORD
INSTEAD OF INSERT
AS
	-- Check total and column links to the same table

	IF EXISTS(SELECT * FROM DELETED)
		-- Update
		-- Changes among ids is not permitted.
		BEGIN
		UPDATE ACT_NOTE_RECORD
		SET
			note = ins.note,
			comment = ins.comment
		FROM
			ACT_NOTE_RECORD rec
			INNER JOIN INSERTED ins ON rec.id_record = ins.id_record
			INNER JOIN ACT_NOTE_TOTAL_VALEUR tot ON tot.id_total = ins.id_total
			INNER JOIN ACT_NOTE_COLUMN col ON col.id_column = ins.id_column
			WHERE tot.id_table = col.id_table
		END
	ELSE
		-- Insert
		BEGIN
		INSERT INTO ACT_NOTE_RECORD
			SELECT
				id_column = ins.id_column,
				id_total = ins.id_total,
				note = ins.note,
				comment = ins.comment
			FROM INSERTED ins
			INNER JOIN ACT_NOTE_TOTAL_VALEUR tot ON tot.id_total = ins.id_total
			INNER JOIN ACT_NOTE_COLUMN col ON col.id_column = ins.id_column
			WHERE tot.id_table = col.id_table
		END
		
GO

ALTER TABLE ACT_NOTE_COLUMN ENABLE TRIGGER TRIG_ACT_Note_Column_Change


ALTER TRIGGER TRIG_ACT_Note_Column_Change
ON ACT_NOTE_COLUMN
AFTER INSERT, UPDATE, DELETE
AS
	-- Update nb_col_note in ACT_NOTE_TABLE on column's changes
	UPDATE ACT_NOTE_TABLE
	SET
		nb_col_note = tab.nb_col_note + COALESCE(ins.cnt, 0) - COALESCE(del.cnt, 0)
	FROM
		ACT_NOTE_TABLE As tab
		-- join each column with is_activated and is_note which needs to be updated.
		LEFT OUTER JOIN (
			SELECT id_table, COUNT(*) AS cnt
			FROM inserted
			WHERE is_activated = 1 AND is_note = 1
			GROUP BY id_table
		) AS ins ON ins.id_table = tab.id_table
		LEFT OUTER JOIN (
			SELECT id_table, COUNT(*) AS cnt
			FROM deleted
			WHERE is_activated = 1 AND is_note = 1
			GROUP BY id_table
		) AS del ON del.id_table = tab.id_table
GO

ALTER TABLE ACT_NOTE_COLUMN ENABLE TRIGGER TRIG_ACT_Note_Column_Change

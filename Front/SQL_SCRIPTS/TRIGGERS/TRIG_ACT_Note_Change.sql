ALTER TRIGGER TRIG_ACT_Note_Change
ON ACT_NOTE_RECORD
AFTER INSERT, UPDATE, DELETE
AS
	-- Update total count on note's changes
	UPDATE ACT_NOTE_TOTAL_VALEUR
	SET
		total = notes.sum_poids/tab.nb_col_note
	FROM
		ACT_NOTE_TOTAL_VALEUR As total
		-- join each total which needs to be updated.
		INNER JOIN (
			SELECT ins.id_total, id_table FROM INSERTED ins
			INNER JOIN ACT_NOTE_TOTAL_VALEUR tot ON tot.id_total = ins.id_total
			UNION
			SELECT del.id_total, id_table FROM DELETED del
			INNER JOIN ACT_NOTE_TOTAL_VALEUR tot ON tot.id_total = del.id_total
		) AS updated ON updated.id_total = total.id_total
		INNER JOIN ACT_NOTE_TABLE tab ON tab.id_table = updated.id_table
		INNER JOIN (
			SELECT
				id_total, SUM(poids) AS sum_poids
			FROM
				ACT_NOTE_RECORD rec
				INNER JOIN ACT_NOTE note ON note.id = rec.note
				INNER JOIN ACT_NOTE_COLUMN col ON col.id_column = rec.id_column
				WHERE is_activated = 1 AND is_note = 1
			GROUP BY
				id_total
		) AS notes ON notes.id_total = total.id_total
		
GO

ALTER TABLE ACT_NOTE_RECORD ENABLE TRIGGER TRIG_ACT_Note_Change

/*
------- TESTS -------
-- TABLE
DECLARE @t1 AS INT
DECLARE @t2 AS INT
INSERT INTO ACT_NOTE_TABLE (nom) VALUES ('test')
SET @t1 = SCOPE_IDENTITY()
INSERT INTO ACT_NOTE_TABLE (nom) VALUES ('test2')
SET @t2 = SCOPE_IDENTITY()

-- SELECT * FROM ACT_NOTE_TABLE

-- COLUMNS
DECLARE @c1 AS INT
DECLARE @c2 AS INT
DECLARE @c3 AS INT
DECLARE @c4 AS INT
DECLARE @c5 AS INT
INSERT INTO ACT_NOTE_COLUMN (id_TABLE, nom, is_note, is_activated)
	VALUES (@t1, 'col1', 0, 0)
SET @c1 = SCOPE_IDENTITY()
INSERT INTO ACT_NOTE_COLUMN (id_TABLE, nom, is_note, is_activated)
	VALUES (@t1, 'col2', 0, 1)
SET @c2 = SCOPE_IDENTITY()
INSERT INTO ACT_NOTE_COLUMN (id_TABLE, nom, is_note, is_activated)
	VALUES (@t1, 'col3', 1, 0)
SET @c3 = SCOPE_IDENTITY()
INSERT INTO ACT_NOTE_COLUMN (id_TABLE, nom, is_note, is_activated)
	VALUES (@t2, 'coltest1', 1, 1)
SET @c4 = SCOPE_IDENTITY()
INSERT INTO ACT_NOTE_COLUMN (id_TABLE, nom, is_note, is_activated)
	VALUES (@t2, 'coltest2', 1, 1)
SET @c5 = SCOPE_IDENTITY()


-- SELECT * FROM ACT_NOTE_COLUMN

-- TOTAL
DECLARE @v1 AS INT
SET @v1 = (SELECT TOP 1 id FROM ACT_VALEUR)
DECLARE @s1 AS INT
DECLARE @s2 AS INT
INSERT INTO ACT_NOTE_TOTAL_VALEUR (id_valeur, id_table) VALUES (@v1, @t1)
SET @s1 = SCOPE_IDENTITY()
INSERT INTO ACT_NOTE_TOTAL_VALEUR (id_valeur, id_table) VALUES (@v1, @t2)
SET @s2 = SCOPE_IDENTITY()

-- SELECT * FROM ACT_NOTE_TOTAL_VALEUR

-- RECORD
INSERT INTO ACT_NOTE_RECORD (id_column, id_total, note) VALUES (@c1, @s1, '=')
INSERT INTO ACT_NOTE_RECORD (id_column, id_total, note) VALUES (@c2, @s1, '++')
INSERT INTO ACT_NOTE_RECORD (id_column, id_total, note) VALUES (@c3, @s1, '+')
INSERT INTO ACT_NOTE_RECORD (id_column, id_total, note) VALUES (@c4, @s2, '--')
INSERT INTO ACT_NOTE_RECORD (id_column, id_total, note) VALUES (@c5, @s2, '++')

-- SELECT * FROM ACT_NOTE_RECORD

------- SELECTS -------
SELECT * FROM ACT_NOTE_TABLE
SELECT * FROM ACT_NOTE_COLUMN
SELECT * FROM ACT_NOTE_TOTAL_VALEUR
SELECT * FROM ACT_NOTE_RECORD

------- REMOVE -------
DELETE FROM ACT_NOTE_RECORD
DELETE FROM ACT_NOTE_TOTAL_VALEUR
DELETE FROM ACT_NOTE_COLUMN
DELETE FROM ACT_NOTE_TABLE
*/
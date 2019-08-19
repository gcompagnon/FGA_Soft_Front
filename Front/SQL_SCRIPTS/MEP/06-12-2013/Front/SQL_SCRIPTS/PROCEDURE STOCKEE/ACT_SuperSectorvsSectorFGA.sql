USE [E2DBFGA01]
GO
/****** Object:  StoredProcedure [dbo].[ACT_SuperSectorvsSectorFGA]    Script Date: 11/14/2013 10:47:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ACT_SuperSectorvsSectorFGA]

		@supersector As INTEGER,
		@fga As VARCHAR(50)
AS

--SELECT DISTINCT f.libelle FROM ACT_SUPERSECTOR sus 
--LEFT OUTER JOIN ACT_SECTOR s ON sus.id=s.id_supersector
--LEFT OUTER JOIN ACT_SUBSECTOR ss ON s.id = ss.id_sector
--LEFT OUTER JOIN ACT_FGA_SECTOR f ON ss.id_fga_sector = f.id
--WHERE sus.id=@supersector

select distinct fga.label from ref_security.SECTOR ss
inner join  ref_security.SECTOR s on s.id_parent=ss.id
inner join ref_security.SECTOR_TRANSCO tr on tr.id_sector1=s.id
inner join ref_security.SECTOR fga on fga.id=tr.id_sector2
where fga.class_name=@fga and ss.code=@supersector
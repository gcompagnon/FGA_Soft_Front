--Publication des indices

declare @dateInventaire  as datetime
set @dateInventaire ='31/12/2012'

declare @Indice as varchar(24)
set @Indice = 'INDEX_50'

declare @pct_15_25 as float
declare @pct_25 as float
declare @pct_IG_3_5 as float
declare @base as float

declare @Code_PROXY as varchar(24)

set @pct_15_25=(select sum(Poids_VB)from ptf_proxy where date=@dateInventaire and code_proxy='EMTX_F' and [maturité]<=date+365.25*25)
set @pct_25=(select sum(Poids_VB)from ptf_proxy where date=@dateInventaire and code_proxy='EMTX_F' and [maturité]>date+365.25*25)
set @pct_IG_3_5=(select sum(Poids_VB)from ptf_proxy where date=@dateInventaire and code_proxy='EMTXig_AG_G' and [maturité]<=date+365.25*5 and [maturité]>date+365.25*3)

set @base=(select case @Indice 
                  when 'INDEX_50' then @pct_15_25
                  when 'INDEX_186' then @pct_25
                  when 'INDEX_239' then @pct_IG_3_5
                  else 1
                  end)
set @code_PROXY = (select case @Indice 
                  when 'INDEX_50' then 'EMTX_F'
                  when 'INDEX_186' then 'EMTX_F'
                  when 'INDEX_239' then 'EMTXig_AG_G'
                  else (select code_proxy from JUMP_INDICE_PROXY where code_ptf_jump=@Indice )
                  end)

select 
	prx.date              as [date], 
	jip.name_jump         as [name],
	'FEDERIS'             as [portfolio source],
	jip.code_ptf_jump     as [portfolio code],
	jip.bank_code_jump    as [bank code],
	jip.branch_code_jump  as [branch code],
	jip.account_jump      as [account],
	'EUR'                 as [base currency],
	jot.type_jump         as [type],
	prx.Libelle_Titre     as [asset],
	prx.Devise_Titre      as [currency],
	'no_place'            as [market place],
	999                   as [telekurs code],
	code_titre            as [isin code],
	''                    as [cusip code],
	''                    as [Bloomberg code],
	''                    as [Beauchamp code],
	''                    as [Local Code],		
	1                     as [position],
	Poids_VB/@base        as [base amount],
	Poids_VB/@base        as [average absorption cost],
	0                     as [base PL],
	1                     as [change rate],
	0                     as [line PL],	
	Poids_VB/@base        as [line amount]
from ptf_proxy prx , JUMP_INDICE_PROXY jip, JUMP_OMEGA_TYPE jot
where date=@dateInventaire
and rtrim(jip.code_ptf_jump)=@Indice
and rtrim(prx.code_proxy)=@code_PROXY
and prx.type_produit<>'Cash'
and jot.type_produit=prx.Type_Produit
and (
       ([maturité]<=date+365.25*25 and @Indice='INDEX_50') 
    OR ([maturité]>date+365.25*25 and @Indice='INDEX_186') 
    OR ([maturité]<=date+365.25*5 and [maturité]>date+365.25*3 and @Indice='INDEX_239')
    OR (@indice not in ('INDEX_50','INDEX_186','INDEX_239')))
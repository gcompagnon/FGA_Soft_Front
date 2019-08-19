------------------------------------------------------------------------------------------
-- Copier le proxy d une date à une autre.
-- la date du proxy de reference est 30/04/2012 et on veut créer un proxy en date @dateTransparence en prenant comme reference celui du @dateProxyReference

declare @dateTransparence datetime
set @dateTransparence = '24/06/2015'

declare @dateProxyReference datetime
set @dateProxyReference = '29/05/2015'

-- PRE CONTROLES:
-- verifier que les dates sont différentes
if @dateTransparence <> @dateProxyReference
BEGIN
	declare @nb int
	set @nb = (Select count(*) from PTF_PARAM_PROXY where date =  @dateProxyReference)
	IF @nb = 0
	BEGIN
		PRINT 'Aucune donnee pour la date du proxy de reference (table PTF_PARAM_PROXY): ' + RTRIM(convert(char,@dateProxyReference,103))
		RETURN
	END  
	set @nb = (Select count(*) from PTF_PROXY where date =  @dateProxyReference)
	IF @nb = 0
	BEGIN
		PRINT 'Aucune donnee pour la date du proxy de reference (table PTF_PROXY): ' + RTRIM(convert(char,@dateProxyReference,103))
		RETURN
	END  
	  
	set @nb = (Select count(*) from PTF_PARAM_PROXY where date =  @dateTransparence)
	IF @nb <> 0
	BEGIN
		PRINT 'suppression sur la table (PTF_PARAM_PROXY): ' + RTRIM(convert(char,@dateTransparence,103))
		delete from PTF_PARAM_PROXY where date = @dateTransparence 
	END 
	 
	set @nb = (Select count(*) from PTF_PROXY where date =  @dateTransparence)
	IF @nb <> 0
	BEGIN
		PRINT 'suppression sur la table (PTF_PROXY): ' + RTRIM(convert(char,@dateTransparence,103))
		delete from PTF_PROXY where date = @dateTransparence 
	END 

	-- Copie des lignes du proxy en date de reference sur la date de transparence voulue
	PRINT 'Copie de la table (PTF_PARAM_PROXY) ' + RTRIM(convert(char,@dateProxyReference,103)) + ' sur : ' + RTRIM(convert(char,@dateTransparence,103))
	insert into PTF_PARAM_PROXY
	select @dateTransparence,Isin_Titre,Libellé_titre,Code_Proxy,Libellé_Proxy,Source from PTF_PARAM_PROXY where Date = @dateProxyReference
	
	PRINT 'Copie de la table (PTF_PROXY) ' + RTRIM(convert(char,@dateProxyReference,103)) + ' sur : ' + RTRIM(convert(char,@dateTransparence,103))
	insert into PTF_PROXY  --(Date ,Code_Proxy,Libelle_Proxy,code_titre,Libelle_Titre,Poids_VB,Poids_CC,Type_Produit,Devise_Titre,Secteur,Sous_Secteur,Pays,Emetteur,Rating,Groupe_Emet,Maturité,Duration,Sensibilite)
	select @dateTransparence,Code_Proxy,Libelle_Proxy,code_titre,Libelle_Titre,Poids_VB,Poids_CC,Type_Produit,Devise_Titre,Secteur,Sous_Secteur,Pays,Emetteur,Rating,Groupe_Emet,Maturité,Duration,Sensibilite 
	from PTF_PROXY where Date = @dateProxyReference

	-- maj des date de maturité (pour les titres Court terme à renseigner)
	update PTF_PROXY
	set Maturité = @dateTransparence
	where Date = @dateTransparence and Maturité < @dateTransparence

	PRINT 'Copie du proxy ' + RTRIM(convert(char,@dateProxyReference,103)) + ' sur : ' + RTRIM(convert(char,@dateTransparence,103)) + ' OK '

END


USE master
GO
CREATE DATABASE G3D
GO
USE G3D
GO
CREATE TABLE Établissement
	(
		Code NVARCHAR(6) PRIMARY KEY,
		Nom NVARCHAR(200) NOT NULL
	)
GO
INSERT INTO Établissement VALUES ('R330', 'Institut Spécialisé de Technologie Appliquée Hay Al Adarissa Fès'), ('REH0', 'Centre Mixte de Formation Professionnelle Fès')
GO
CREATE TABLE CléProduit
	(
		Id INT PRIMARY KEY IDENTITY(1, 1),
		Clé VARBINARY(100) NOT NULL UNIQUE,
		NombreUtilisations INT NULL,
		UtilisationsMaximales INT NOT NULL,
		Établissement NVARCHAR(6) FOREIGN KEY REFERENCES Établissement (Code) ON DELETE NO ACTION NULL,
		CHECK (NombreUtilisations <= UtilisationsMaximales)
	)
GO
CREATE PROC CheckClé @Clé NVARCHAR (100), @Id INT OUT, @Capacité BIT OUT
AS
BEGIN
DECLARE @NombreUtilisations INT
DECLARE @UtilisationsMaximales INT
SELECT @Id = Id, @NombreUtilisations = NombreUtilisations, @UtilisationsMaximales = UtilisationsMaximales FROM CléProduit WHERE @Clé = CONVERT(VARCHAR (100), DECRYPTBYPASSPHRASE('G3D', Clé))
IF (@NombreUtilisations != NULL AND @NombreUtilisations = @UtilisationsMaximales)
SET @Capacité = 0
ELSE
SET @Capacité = 1
END
GO
INSERT INTO CléProduit (Clé, UtilisationsMaximales) VALUES (ENCRYPTBYPASSPHRASE('G3D', 'B8E9B69DB512BE7'), 2)
GO
INSERT INTO CléProduit (Clé, UtilisationsMaximales, Établissement) VALUES (ENCRYPTBYPASSPHRASE('G3D', 'FFA9DADFD95C95F'), 3, 'R330')
GO
INSERT INTO CléProduit (Clé, UtilisationsMaximales, Établissement) VALUES (ENCRYPTBYPASSPHRASE('G3D', 'C65BCAD62C5AC7C'), 2, 'REH0')
GO
CREATE TABLE Utilisateur
	(
		Id INT IDENTITY (1,1) PRIMARY KEY,
		NomFamille NVARCHAR(30) NOT NULL,
		Prénom NVARCHAR(30) NOT NULL,
		NomUtilisateur NVARCHAR(20) NULL CHECK (LEN(NomUtilisateur) > 3),
		MotPasse VARBINARY(100) NOT NULL,
		EMail NVARCHAR(40) NOT NULL UNIQUE,
		CléProduit INT FOREIGN KEY REFERENCES CléProduit (Id) ON DELETE CASCADE,
		PhotoProfil NVARCHAR(100) NULL,
		DateInscription DATETIME NOT NULL,
		DateModification DATETIME NULL,
		UNIQUE (NomUtilisateur, EMail)
	)
GO
CREATE PROC FindUtilisateur @IsMail BIT, @Identity NVARCHAR (40), @PassWord NVARCHAR (100), @IsHere BIT OUT, @Id INT OUT
AS
BEGIN
IF (@IsMail = 1)
SELECT @IsHere = COUNT(*), @Id = Id FROM Utilisateur WHERE EMail = @Identity AND DECRYPTBYPASSPHRASE('G3D', MotPasse) = @PassWord GROUP BY Id
ELSE
SELECT @IsHere = COUNT(*), @Id = Id FROM Utilisateur WHERE NomUtilisateur = @Identity AND DECRYPTBYPASSPHRASE('G3D', MotPasse) = @PassWord GROUP BY Id
END
GO
CREATE PROC UtilisateurInsertQuery @NomFamille NVARCHAR(30), @Prénom NVARCHAR(30), @NomUtilisateur NVARCHAR(20), @MotPasse NVARCHAR(100), @EMail NVARCHAR(40), @CléProduit INT
AS
BEGIN
INSERT INTO Utilisateur (NomFamille, Prénom, NomUtilisateur, MotPasse, EMail, CléProduit, DateInscription) VALUES (@NomFamille, @Prénom, @NomUtilisateur, ENCRYPTBYPASSPHRASE('G3D', @MotPasse), @EMail, @CléProduit, GETDATE())
UPDATE CléProduit SET NombreUtilisations += 1 WHERE Id = @CléProduit
END
GO
CREATE PROC UtilisateurChangePassword @Id INT, @PassWord NVARCHAR (100)
AS
BEGIN
UPDATE Utilisateur SET MotPasse = ENCRYPTBYPASSPHRASE('G3D', @PassWord) WHERE Id = @Id
END
GO
EXEC UtilisateurInsertQuery 'LastName', 'FirstName', 'User0', '@@123User', 'Email0@exemple.com', 1
EXEC UtilisateurInsertQuery 'LastName', 'FirstName', 'User1', '@@123User', 'Email1@exemple.com', 2
GO
CREATE TABLE UtilisateurConnecté
	(
		Id INT IDENTITY (1,1) PRIMARY KEY,
		Utilisateur INT FOREIGN KEY REFERENCES Utilisateur (Id) ON DELETE CASCADE,
		ChaîneConnexion VARBINARY(100) NOT NULL,
		DateConnecté DATETIME NULL,
		DateDéconnecté DATETIME NULL
	)
GO
CREATE PROC UtilisateurConnectéInsertQuery @Id INT
AS
BEGIN
INSERT INTO UtilisateurConnecté (Utilisateur, ChaîneConnexion, DateConnecté) VALUES (@Id, ENCRYPTBYPASSPHRASE('G3D', CONVERT(VARCHAR(10), IDENT_CURRENT('dbo.UtilisateurConnecté'))), GETDATE())
SELECT CONVERT(VARCHAR (100), ChaîneConnexion, 1) FROM UtilisateurConnecté WHERE Id = (IDENT_CURRENT('dbo.UtilisateurConnecté') - 1)
END
GO
CREATE PROC UtilisateurConnectéFindQuery @ChaîneConnexion NVARCHAR (100), @Id INT OUT
AS
BEGIN
SELECT @Id = Utilisateur FROM UtilisateurConnecté WHERE Id = CONVERT(VARCHAR (100), DECRYPTBYPASSPHRASE('G3D', CONVERT(VARBINARY(100), @ChaîneConnexion, 1))) AND DateDéconnecté IS NULL
END
GO
CREATE PROC UtilisateurDéconnectéEditQuery @ChaîneConnexion NVARCHAR (100)
AS
BEGIN
UPDATE UtilisateurConnecté SET DateDéconnecté = GETDATE() WHERE Id = CONVERT(VARCHAR (100), DECRYPTBYPASSPHRASE('G3D', CONVERT(VARBINARY(100), @ChaîneConnexion, 1)))
END
GO
CREATE TABLE Niveau
	(
		NomCourt NVARCHAR(4) PRIMARY KEY,
		Nom NVARCHAR(50) NOT NULL
	)
GO
INSERT INTO Niveau VALUES ('TS', 'Technicien Spécialisé'), ('T', 'Technicien'), ('Q', 'Qualification'), ('S', 'Spécialisation')
GO
CREATE TABLE Filière
	(
		NomCourt NVARCHAR(20) PRIMARY KEY,
		Nom NVARCHAR(200) NOT NULL,
		Niveau NVARCHAR(4) FOREIGN KEY REFERENCES Niveau (NomCourt) ON DELETE NO ACTION
	)
GO
CREATE TRIGGER FilièreInsert ON Filière INSTEAD OF INSERT
AS
IF (NOT EXISTS(SELECT * FROM Filière WHERE NomCourt in (SELECT NomCourt FROM INSERTED)))
BEGIN
	INSERT INTO Filière SELECT * FROM INSERTED
END
GO
CREATE TABLE FilièreAnnée
	(
		Id INT IDENTITY(1, 1) PRIMARY KEY,
		Établissement NVARCHAR(6) FOREIGN KEY REFERENCES Établissement (Code) ON DELETE NO ACTION,
		Filière NVARCHAR(20) FOREIGN KEY REFERENCES Filière (NomCourt) ON DELETE NO ACTION,
		AnnéeFormation NCHAR(9) NOT NULL,
		Promotion NVARCHAR(20) NOT NULL,
		UNIQUE (Établissement, Filière, AnnéeFormation)
	)
GO
CREATE TRIGGER FilièreAnnéeInsert ON FilièreAnnée INSTEAD OF INSERT
AS
IF (NOT EXISTS(SELECT * FROM FilièreAnnée FA JOIN INSERTED I ON FA.Filière = I.Filière  WHERE FA.AnnéeFormation = I.AnnéeFormation AND FA.Établissement = I.Établissement))
BEGIN
	INSERT INTO FilièreAnnée SELECT Établissement, Filière, AnnéeFormation, Promotion FROM INSERTED
END
GO
CREATE PROC FilièreAnnéeSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4), @AnnéeÉtude NVARCHAR(2)
AS
BEGIN
IF @AnnéeÉtude != ''
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT FA.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = @AnnéeÉtude
ELSE
SELECT DISTINCT FA.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND AnnéeÉtude = @AnnéeÉtude
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT FA.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = @AnnéeÉtude
ELSE
SELECT DISTINCT FA.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE AnnéeÉtude = @AnnéeÉtude
END
END
ELSE
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT FA.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT FA.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT FA.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT FA.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id
END
END
END
GO
CREATE TABLE AnnéeÉtude
	(
		NomCourt NVARCHAR(2) PRIMARY KEY,
		Nom NVARCHAR(10) NOT NULL,
	)
GO
INSERT INTO AnnéeÉtude VALUES ('1A', '1ère Année'), ('2A', '2ème Année'), ('3A', '3ème Année')
GO
CREATE TABLE TypeStagiaires
	(
		NomCourt NCHAR(2) PRIMARY KEY,
		Nom NVARCHAR(20) NOT NULL
	)
GO
INSERT INTO TypeStagiaires VALUES ('CR', 'Candidat Régulier'), ('CL', 'Candidat Libre')
GO
CREATE TABLE Groupe
	(
		Id INT IDENTITY(1, 1) PRIMARY KEY,
		FilièreAnnée INT FOREIGN KEY REFERENCES FilièreAnnée (Id) ON DELETE NO ACTION,
		Numéro NVARCHAR(3) NULL,
		AnnéeÉtude NVARCHAR (2) NOT NULL FOREIGN KEY REFERENCES AnnéeÉtude (NomCourt) ON DELETE NO ACTION,
		TypeFormation NVARCHAR(20) NULL,
		TypeStagiaires NCHAR(2) NOT NULL FOREIGN KEY REFERENCES TypeStagiaires (NomCourt) ON DELETE NO ACTION,
		UNIQUE (FilièreAnnée, Numéro, AnnéeÉtude)
	)
GO
CREATE TRIGGER GroupeInsert ON Groupe INSTEAD OF INSERT
AS
IF (NOT EXISTS(SELECT * FROM Groupe G JOIN INSERTED I ON G.FilièreAnnée = I.FilièreAnnée  WHERE G.Numéro = I.Numéro AND G.AnnéeÉtude = I.AnnéeÉtude))
BEGIN
	INSERT INTO Groupe SELECT FilièreAnnée, Numéro, AnnéeÉtude, TypeFormation, TypeStagiaires FROM INSERTED
END
GO
CREATE PROC GroupeSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4), @AnnéeÉtude NVARCHAR(2)
AS
BEGIN
IF @AnnéeÉtude != ''
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT G.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = @AnnéeÉtude
ELSE
SELECT G.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND AnnéeÉtude = @AnnéeÉtude
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT G.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = @AnnéeÉtude
ELSE
SELECT * FROM Groupe WHERE AnnéeÉtude = @AnnéeÉtude
END
END
ELSE
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT G.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT G.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT G.* FROM Groupe G JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT * FROM Groupe
END
END
END
GO
CREATE TABLE Stagiaire
	(
		Cef NVARCHAR(20) PRIMARY KEY,
		NomPrénom NVARCHAR(60) NOT NULL,
		Cin NVARCHAR(10) NULL UNIQUE,
		NuméroTéléphonePremier NVARCHAR(20) NULL,
		NuméroTéléphoneDeuxième NVARCHAR(20) NULL,
		SituationActuel NVARCHAR(30) NULL CHECK (SituationActuel = 'Embauche' OR SituationActuel = 'Recherche d''emploi' OR SituationActuel = 'Poursuite de Formation')
	)
GO
CREATE TRIGGER StagiaireInsert ON Stagiaire INSTEAD OF INSERT
AS
IF (NOT EXISTS(SELECT * FROM Stagiaire S JOIN INSERTED I ON S.Cef = I.Cef ))
BEGIN
	INSERT INTO Stagiaire SELECT * FROM INSERTED
END
GO
CREATE PROC UpdateStagiaire @Cef NVARCHAR(20), @NuméroTéléphonePremier NVARCHAR(20), @NuméroTéléphoneDeuxième NVARCHAR(20), @SituationActuel NVARCHAR(30)
AS
BEGIN
UPDATE Stagiaire SET NuméroTéléphonePremier = @NuméroTéléphonePremier, NuméroTéléphoneDeuxième = @NuméroTéléphoneDeuxième, SituationActuel = @SituationActuel WHERE Cef = @Cef
END
GO
CREATE PROC StagiaireSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4), @AnnéeÉtude NVARCHAR(2)
AS
BEGIN
IF @AnnéeÉtude IS NOT NULL
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation IS NOT NULL
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = @AnnéeÉtude
ELSE
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND AnnéeÉtude = @AnnéeÉtude
END
ELSE
BEGIN
IF @FAnnéeFormation IS NOT NULL
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = @AnnéeÉtude
ELSE
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id WHERE AnnéeÉtude = @AnnéeÉtude
END
END
ELSE
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation IS NOT NULL
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement
END
ELSE
BEGIN
IF @FAnnéeFormation IS NOT NULL
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT * FROM Stagiaire
END
END
END
GO

-- DISTINCT OR NOT
CREATE TABLE StagiaireGroupe
	(
		Id INT IDENTITY(1, 1) PRIMARY KEY,
		Stagiaire NVARCHAR(20) FOREIGN KEY REFERENCES Stagiaire (Cef) ON DELETE CASCADE,
		Groupe INT FOREIGN KEY REFERENCES Groupe (Id) ON DELETE NO ACTION,
		Admis NCHAR(3) NOT NULL CHECK (Admis = 'Oui' OR Admis = 'Non'),
		Classement INT NULL,
		UNIQUE (Stagiaire, Groupe)
	)
GO
CREATE TRIGGER StagiaireGroupeInsert ON StagiaireGroupe INSTEAD OF INSERT
AS
IF (NOT EXISTS(SELECT * FROM StagiaireGroupe SG JOIN INSERTED I ON SG.Stagiaire = I.Stagiaire WHERE SG.Groupe = I.Groupe))
BEGIN
	INSERT INTO StagiaireGroupe SELECT Stagiaire, Groupe, Admis, Classement FROM INSERTED
END
GO
CREATE PROC StagiaireGroupeSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4), @AnnéeÉtude NVARCHAR(2)
AS
BEGIN
IF @AnnéeÉtude != ''
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = @AnnéeÉtude
ELSE
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND AnnéeÉtude = @AnnéeÉtude
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = @AnnéeÉtude
ELSE
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id WHERE AnnéeÉtude = @AnnéeÉtude
END
END
ELSE
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id
END
END
END
GO
CREATE TABLE StagiaireÉtatSignature
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		État NVARCHAR (10) NULL CHECK (État = 'Edité' OR État = 'Rejeté' OR État = 'Corrigé' OR État = 'Envoyé' OR État = 'Signé'),
		ÉtatDate DATETIME,
		PRIMARY KEY (StagiaireGroupe)
	)
GO
CREATE TABLE StagiaireEmbauche
	(
		Stagiaire NVARCHAR(20) FOREIGN KEY REFERENCES Stagiaire (Cef) ON DELETE NO ACTION,
		RaisonSociale NVARCHAR(100) NULL,
		DateDébut DATE NULL,
		DateInsertion DATETIME NOT NULL,
		UNIQUE (Stagiaire, RaisonSociale)
	)
GO
CREATE TABLE StagiaireRetrait
	(
		Id INT IDENTITY (1,1) PRIMARY KEY,
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		TypeDocument NVARCHAR(30) NOT NULL CHECK (TypeDocument = 'Diplôme' OR TypeDocument = 'Baccalauréat' OR TypeDocument = 'Relevé de Notes' OR TypeDocument = 'Attestation de Réussite'),
		DateRetrait DATE NULL,
		UNIQUE (StagiaireGroupe, TypeDocument, DateRetrait)
	)
GO
CREATE TABLE RetraitBaccalauréat
	(
		StagiaireRetrait INT FOREIGN KEY REFERENCES StagiaireRetrait (Id) ON DELETE NO ACTION,
		NuméroSérie NVARCHAR(30) NULL,
		UNIQUE (StagiaireRetrait)
	)
GO
CREATE TABLE RetraitDiplôme
	(
		StagiaireRetrait INT FOREIGN KEY REFERENCES StagiaireRetrait (Id) ON DELETE NO ACTION,
		NuméroSérie NVARCHAR(30) NULL,
		Cab NVARCHAR (30) NULL,
		DocumentVérification NVARCHAR(20) NULL CHECK (DocumentVérification = 'Cin' OR DocumentVérification = 'Procuration'),
		CheminDocumentVérification NVARCHAR(100) NULL,
		UNIQUE (StagiaireRetrait)
	)
GO
CREATE TABLE Edité
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateEdité DATETIME NOT NULL,
		UNIQUE (StagiaireGroupe, DateEdité)
	)
GO
CREATE TRIGGER EditéInsert ON Edité AFTER INSERT
AS
DELETE StagiaireÉtatSignature WHERE StagiaireGroupe in (SELECT StagiaireGroupe FROM INSERTED)
INSERT INTO StagiaireÉtatSignature SELECT StagiaireGroupe, 'Edité', DateEdité FROM INSERTED
GO
CREATE PROC EditéSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4)
AS
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT E.* FROM Edité E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT E.* FROM Edité E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT E.* FROM Edité E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT * FROM Edité
END
END
GO
CREATE TABLE Rejeté
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateRejeté DATETIME NOT NULL,
		Motif NVARCHAR(100) NULL,
		UNIQUE (StagiaireGroupe, DateRejeté)
	)
GO
CREATE PROC RejetéSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4)
AS
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT R.* FROM Rejeté R JOIN StagiaireGroupe SG ON R.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT R.* FROM Rejeté R JOIN StagiaireGroupe SG ON R.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT R.* FROM Rejeté R JOIN StagiaireGroupe SG ON R.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT * FROM Rejeté
END
END
GO
CREATE TRIGGER RejetéInsert ON Rejeté AFTER INSERT
AS
DELETE StagiaireÉtatSignature WHERE StagiaireGroupe in (SELECT StagiaireGroupe FROM INSERTED)
INSERT INTO StagiaireÉtatSignature SELECT StagiaireGroupe, 'Rejeté', DateRejeté FROM INSERTED
GO
CREATE TABLE Corrigé
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateCorrigé DATETIME NOT NULL,
		UNIQUE (StagiaireGroupe, DateCorrigé)
	)
GO
CREATE PROC CorrigéSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4)
AS
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT C.* FROM Corrigé C JOIN StagiaireGroupe SG ON C.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT C.* FROM Corrigé C JOIN StagiaireGroupe SG ON C.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT C.* FROM Corrigé C JOIN StagiaireGroupe SG ON C.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT * FROM Corrigé
END
END
GO
CREATE TRIGGER CorrigéInsert ON Corrigé AFTER INSERT
AS
DELETE StagiaireÉtatSignature WHERE StagiaireGroupe in (SELECT StagiaireGroupe FROM INSERTED)
INSERT INTO StagiaireÉtatSignature SELECT StagiaireGroupe, 'Corrigé', DateCorrigé FROM INSERTED
GO
CREATE TABLE Envoyé
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateEnvoyé DATETIME NOT NULL,
		UNIQUE (StagiaireGroupe, DateEnvoyé)
	)
GO
CREATE PROC EnvoyéSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4)
AS
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT E.* FROM Envoyé E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT E.* FROM Envoyé E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT E.* FROM Envoyé E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT * FROM Envoyé
END
END
GO
CREATE TRIGGER EnvoyéInsert ON Envoyé AFTER INSERT
AS
DELETE StagiaireÉtatSignature WHERE StagiaireGroupe in (SELECT StagiaireGroupe FROM INSERTED)
INSERT INTO StagiaireÉtatSignature SELECT StagiaireGroupe, 'Envoyé', DateEnvoyé FROM INSERTED
GO
CREATE TABLE Signé
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateSigné DATETIME NOT NULL,
		UNIQUE (StagiaireGroupe, DateSigné)
	)
GO
CREATE PROC SignéSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4)
AS
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT S.* FROM Signé S JOIN StagiaireGroupe SG ON S.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT S.* FROM Signé S JOIN StagiaireGroupe SG ON S.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT DISTINCT S.* FROM Signé S JOIN StagiaireGroupe SG ON S.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation
ELSE
SELECT DISTINCT * FROM Signé
END
END
GO
CREATE TRIGGER SignéInsert ON Signé AFTER INSERT
AS
DELETE StagiaireÉtatSignature WHERE StagiaireGroupe in (SELECT StagiaireGroupe FROM INSERTED)
INSERT INTO StagiaireÉtatSignature SELECT StagiaireGroupe, 'Signé', DateSigné FROM INSERTED
GO
CREATE TABLE Duplicata
	(
		Id INT IDENTITY (1,1) PRIMARY KEY,
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateNaissance DATE NOT NULL,
		Lieu NVARCHAR (200) NULL,
		NuméroSérie NVARCHAR(30) NOT NULL,
		Cab NVARCHAR (30) NOT NULL,
		ModeFormation NVARCHAR(30) NULL
	)
GO
CREATE VIEW DuplicataFiche
AS
SELECT D.Id DupId, SG.Id StagiaireGroupe, Cef, NomPrénom, Cin, DateNaissance, Lieu, NuméroSérie, Cab, NuméroTéléphonePremier, NuméroTéléphoneDeuxième, FA.Filière FilièreCourt, Numéro, ModeFormation, TypeFormation, TS.Nom AS TypeStagiaire, AnnéeFormation, N.Nom As Niveau, F.Nom AS Filière, E.Nom AS Établissement, Promotion AS [Session] FROM Duplicata D JOIN StagiaireGroupe SG ON D.StagiaireGroupe = SG.Id JOIN Stagiaire S ON SG.Stagiaire = S.Cef JOIN Groupe G ON SG.Groupe = G.Id JOIN TypeStagiaires TS ON G.TypeStagiaires = TS.NomCourt JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id JOIN Filière F ON FA.Filière = F.NomCourt JOIN Niveau N ON F.Niveau = N.NomCourt JOIN Établissement E ON FA.Établissement = E.Code
GO

CREATE PROC UpdateStagiaireGroupe @OldGroupId INT, @NewGroupId INT, @Stagiaire NVARCHAR(20), @Classement INT, @Admis NCHAR(3), @Cin NVARCHAR(10), @NomPrénom NVARCHAR(60)
AS
BEGIN
UPDATE StagiaireGroupe SET Groupe = @NewGroupId, Admis = 'Oui', Classement = @Classement WHERE Stagiaire = @Stagiaire AND Groupe = @OldGroupId
IF (@Cin != NULL OR @NomPrénom!= NULL)
UPDATE Stagiaire SET Cin = @Cin, NomPrénom = @NomPrénom WHERE Cef = @Stagiaire
END
GO

CREATE PROCEDURE GetAdmisResult @TypeStagiaires NCHAR(2), @Admis NCHAR(3)
AS
SELECT COUNT(*) AS AdmisCount, FA.Filière AS NomCourt FROM StagiaireGroupe SG join Groupe G on SG.Groupe = G.Id join FilièreAnnée FA on G.FilièreAnnée = FA.Id WHERE AnnéeÉtude = '2A' AND TypeStagiaires = @TypeStagiaires and Admis = @Admis GROUP BY FA.Filière
GO

CREATE PROCEDURE FilièreAnnéeInsertQuery @Établissement NVARCHAR(6), @Filière NVARCHAR(20), @AnnéeFormation NCHAR(9), @Id INT OUT
AS
BEGIN
INSERT INTO [dbo].[FilièreAnnée] ([Établissement], [Filière], [AnnéeFormation]) VALUES (@Établissement, @Filière, @AnnéeFormation);
SELECT @Id = Id FROM [dbo].[FilièreAnnée] WHERE [Établissement] = @Établissement AND [Filière] = @Filière AND [AnnéeFormation] = @AnnéeFormation
END
GO
CREATE PROCEDURE GroupeInsertQuery @FilièreAnnée INT, @Numéro NVARCHAR(3), @AnnéeÉtude NVARCHAR(2), @TypeFormation NVARCHAR(20), @TypeStagiaires NCHAR(2), @Id INT OUT
AS
BEGIN
INSERT INTO [dbo].[Groupe] ([FilièreAnnée], [Numéro], [AnnéeÉtude], [TypeFormation], [TypeStagiaires]) VALUES (@FilièreAnnée, @Numéro, @AnnéeÉtude, @TypeFormation, @TypeStagiaires);
SELECT @Id = Id FROM [dbo].[Groupe] WHERE [FilièreAnnée] = @FilièreAnnée AND [Numéro] = @Numéro AND [AnnéeÉtude] = @AnnéeÉtude
END
GO

CREATE VIEW DiplomeRegistreRetraitFull AS
SELECT Nom, Filière, Numéro, NomPrénom , Cin, DateRetrait, NuméroTéléphonePremier, NuméroSérie, SituationActuel, Promotion FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id JOIN Filière F ON FA.Filière = F.NomCourt JOIN StagiaireRetrait SR ON SG.Id = SR.StagiaireGroupe JOIN RetraitDiplôme RD ON SR.Id = RD.StagiaireRetrait join StagiaireÉtatSignature SES ON SG.Id = SES.StagiaireGroupe
GO
CREATE VIEW DiplomeRegistreRetrait AS
SELECT Filière, Numéro, NomPrénom , Promotion FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id JOIN Filière F ON FA.Filière = F.NomCourt
GO
CREATE VIEW SuiviDesSignatures AS
SELECT Cef, NomPrénom, Filière, Numéro, État, ÉtatDate, Promotion FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN StagiaireÉtatSignature SES ON SES.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id
GO
CREATE VIEW DiplomeBordoreu AS
SELECT S.NomPrénom, FA.Filière, G.Numéro, E.DateEnvoyé FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN StagiaireÉtatSignature SES ON SG.Id = SES.StagiaireGroupe JOIN Envoyé E ON SG.Id = E.StagiaireGroupe JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE DATEDIFF(SECOND, E.DateEnvoyé, SES.ÉtatDate) >= 0
GO
CREATE VIEW DuplicataBordoreu AS
SELECT S.NomPrénom, FA.Filière, G.Numéro, E.DateEnvoyé FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Duplicata D ON SG.Id = D.StagiaireGroupe JOIN StagiaireÉtatSignature SES ON SG.Id = SES.StagiaireGroupe JOIN Envoyé E ON SG.Id = E.StagiaireGroupe JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE DATEDIFF(SECOND, E.DateEnvoyé, SES.ÉtatDate) >= 0
GO
CREATE PROC ContactSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4)
AS
BEGIN
IF @Établissement != ''
BEGIN
IF @FAnnéeFormation != ''
SELECT S.NomPrénom, S.NuméroTéléphonePremier, S.NuméroTéléphoneDeuxième, SG.Classement, G.Numéro, FA.Filière, S.SituationActuel FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = '2A'
ELSE
SELECT S.NomPrénom, S.NuméroTéléphonePremier, S.NuméroTéléphoneDeuxième, SG.Classement, G.Numéro, FA.Filière, S.SituationActuel FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE Établissement = @Établissement AND AnnéeÉtude = '2A'
END
ELSE
BEGIN
IF @FAnnéeFormation != ''
SELECT S.NomPrénom, S.NuméroTéléphonePremier, S.NuméroTéléphoneDeuxième, SG.Classement, G.Numéro, FA.Filière, S.SituationActuel FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = '2A'
ELSE
SELECT S.NomPrénom, S.NuméroTéléphonePremier, S.NuméroTéléphoneDeuxième, SG.Classement, G.Numéro, FA.Filière, S.SituationActuel FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id WHERE AnnéeÉtude = '2A'
END
END
GO

CREATE PROC InsertFullRow @FilièreCourt NVARCHAR(20), @Filière NVARCHAR(200), @Niveau NVARCHAR(4), @ÉtablissementCourt NVARCHAR(6), @AnnéeFormation NCHAR(9), @Promotion NVARCHAR(20), @Numéro NVARCHAR(3), @AnnéeÉtude NVARCHAR(2), @TypeFormation NVARCHAR(20), @TypeStagiaires NCHAR(2), @Cef NVARCHAR(20), @NomPrénom NVARCHAR(60), @Cin NVARCHAR(10), @Admis NCHAR(3), @Classement INT
AS
BEGIN
DECLARE @IdFilièreAnnée INT
DECLARE @IdGroupe INT
INSERT INTO Filière VALUES (@FilièreCourt, @Filière, @Niveau)
INSERT INTO FilièreAnnée VALUES (@ÉtablissementCourt, @FilièreCourt, @AnnéeFormation, @Promotion)
SELECT @IdFilièreAnnée = Id FROM FilièreAnnée WHERE Établissement = @ÉtablissementCourt AND Filière = @FilièreCourt AND AnnéeFormation = @AnnéeFormation
INSERT INTO Groupe VALUES (@IdFilièreAnnée, @Numéro, @AnnéeÉtude, @TypeFormation, @TypeStagiaires)
SELECT @IdGroupe = Id FROM Groupe WHERE FilièreAnnée = @IdFilièreAnnée AND Numéro = @Numéro AND AnnéeÉtude = @AnnéeÉtude
INSERT INTO Stagiaire (Cef, NomPrénom, Cin) VALUES (@Cef, @NomPrénom, @Cin)
INSERT INTO StagiaireGroupe VALUES (@Cef, @IdGroupe, @Admis, @Classement)
END
GO


CREATE PROC InsertDuplicataRow @FilièreCourt NVARCHAR(20), @Filière NVARCHAR(200), @Niveau NVARCHAR(4), @ÉtablissementCourt NVARCHAR(6), @AnnéeFormation NCHAR(9), @Promotion NVARCHAR(20), @Numéro NVARCHAR(3), @ModeFormation NVARCHAR(20), @TypeFormation NVARCHAR(20), @TypeStagiaires NCHAR(2), @Cef NVARCHAR(20), @NomPrénom NVARCHAR(60), @Cin NVARCHAR(10), @NuméroTéléphonePremier NVARCHAR(20), @NuméroTéléphoneDeuxième NVARCHAR(20), @DateNaissance DATE, @Lieu NVARCHAR (200), @NuméroSérie NVARCHAR(30), @Cab NVARCHAR (30)
AS
BEGIN
DECLARE @IdFilièreAnnée INT
DECLARE @IdGroupe INT
DECLARE @IdStagiaireGroupe INT
INSERT INTO Filière VALUES (@FilièreCourt, @Filière, @Niveau)
INSERT INTO FilièreAnnée VALUES (@ÉtablissementCourt, @FilièreCourt, @AnnéeFormation, @Promotion)
SELECT @IdFilièreAnnée = Id FROM FilièreAnnée WHERE Établissement = @ÉtablissementCourt AND Filière = @FilièreCourt AND AnnéeFormation = @AnnéeFormation
INSERT INTO Groupe VALUES (@IdFilièreAnnée, @Numéro, '2A', @TypeFormation, @TypeStagiaires)
SELECT @IdGroupe = Id FROM Groupe WHERE FilièreAnnée = @IdFilièreAnnée AND Numéro = @Numéro AND AnnéeÉtude = '2A'
INSERT INTO Stagiaire VALUES (@Cef, @NomPrénom, @Cin, @NuméroTéléphonePremier, @NuméroTéléphoneDeuxième, NULL)
INSERT INTO StagiaireGroupe VALUES (@Cef, @IdGroupe, 'Oui', '0')
SELECT @IdStagiaireGroupe = Id FROM StagiaireGroupe WHERE Stagiaire = @Cef AND Groupe = @IdGroupe
INSERT INTO Duplicata VALUES (@IdStagiaireGroupe, @DateNaissance, @Lieu, @NuméroSérie, @Cab, @ModeFormation)
END
GO

CREATE PROC UpdateDuplicata @DuplicataId INT, @NewStagiaireGroupeId INT, @DateNaissance DATE, @Lieu NVARCHAR (200), @NuméroSérie NVARCHAR(30), @Cab NVARCHAR (30), @Cef NVARCHAR(20), @NomPrénom NVARCHAR(60), @Cin NVARCHAR(10), @NuméroTéléphonePremier NVARCHAR(20), @NuméroTéléphoneDeuxième NVARCHAR(20), @ModeFormation NVARCHAR(20)
AS
BEGIN
UPDATE Duplicata SET StagiaireGroupe = @NewStagiaireGroupeId, DateNaissance = @DateNaissance, Lieu = @Lieu, NuméroSérie = @NuméroSérie, Cab = @Cab, ModeFormation = @ModeFormation WHERE Id = @DuplicataId
UPDATE Stagiaire SET NomPrénom = @NomPrénom, Cin = @Cin,  NuméroTéléphonePremier = @NuméroTéléphonePremier, NuméroTéléphoneDeuxième = @NuméroTéléphoneDeuxième WHERE Cef = @Cef
END
GO

CREATE VIEW CheckListView AS
SELECT S.Cef, S.NomPrénom, S.Cin, G.Numéro, G.TypeStagiaires, F.NomCourt AS FilièreNomCourt, F.Nom AS FilièreNom, N.Nom AS NiveauNom, E.Code AS ÉtablissementCode, E.Nom AS ÉtablissementNom, FA.AnnéeFormation FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id JOIN Filière F ON FA.Filière = F.NomCourt JOIN Niveau N ON F.Niveau = N.NomCourt JOIN Établissement E ON FA.Établissement = E.Code WHERE G.AnnéeÉtude = '2A'
GO

CREATE PROC CheckListViewSelectQuery @Établissement NVARCHAR(6), @FAnnéeFormation NCHAR(4)
AS
BEGIN
IF @Établissement != ''
SELECT S.Cef, S.NomPrénom, S.Cin, G.Numéro, G.TypeStagiaires, F.NomCourt AS FilièreNomCourt, F.Nom AS FilièreNom, N.Nom AS NiveauNom, E.Code AS ÉtablissementCode, E.Nom AS ÉtablissementNom, FA.AnnéeFormation FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id JOIN Filière F ON FA.Filière = F.NomCourt JOIN Niveau N ON F.Niveau = N.NomCourt JOIN Établissement E ON FA.Établissement = E.Code WHERE E.Code = @Établissement AND SUBSTRING(AnnéeFormation, 1, 4) = @FAnnéeFormation AND AnnéeÉtude = '2A'
ELSE
SELECT S.Cef, S.NomPrénom, S.Cin, G.Numéro, F.NomCourt AS FilièreNomCourt, F.Nom AS FilièreNom, N.Nom AS NiveauNom, E.Code AS ÉtablissementCode, E.Nom AS ÉtablissementNom, FA.AnnéeFormation FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN FilièreAnnée FA ON G.FilièreAnnée = FA.Id JOIN Filière F ON FA.Filière = F.NomCourt JOIN Niveau N ON F.Niveau = N.NomCourt JOIN Établissement E ON FA.Établissement = E.Code WHERE SUBSTRING(AnnéeFormation, 1, 4) = '2020' AND AnnéeÉtude = '2A'
END
GO

--\\\\\\\\\\__ ARCHIVE __//////////

CREATE TABLE Archive
	(
		Id INT IDENTITY (1, 1) PRIMARY KEY,
		DateCréée DATETIME NOT NULL,
		DateModifiée DATETIME NULL,
		[Path] NVARCHAR (300) NOT NULL
	)

CREATE TABLE Note
	(
		[PathId] INT FOREIGN KEY REFERENCES Archive (Id) ON DELETE CASCADE,
		Référence NVARCHAR (100) NULL,
		Objectif NVARCHAR (100) NULL,
		[Date] Date NULL,
	)
GO

CREATE TABLE Document
	(
		[PathId] INT FOREIGN KEY REFERENCES Archive (Id) ON DELETE CASCADE,
		Document NVARCHAR (30) NOT NULL CHECK (Document = 'Bulletin' OR Document = 'Diplôme' OR Document = 'Attestation de réussite' OR Document = 'Pv'),
		AnnéeFormation NCHAR (9) NOT NULL,
		Niveau NVARCHAR (30) NOT NULL,
		AnnéeÉtude NVARCHAR (20) NOT NULL,
		Filière NVARCHAR(20) NOT NULL,
		Groupe NCHAR (3) NOT NULL
	)
GO

CREATE PROC ArchiveInsertQuery @DateCréée DATETIME, @DateModifiée DATETIME, @Path NVARCHAR (300), @Id INT OUT
AS
BEGIN
INSERT INTO Archive VALUES (@DateCréée, @DateModifiée, @Path)
SELECT @Id = Id FROM Archive WHERE DateCréée = @DateCréée AND [Path] = @Path
END
GO

CREATE PROC ArchiveUpdateQuery @Id INT, @Path NVARCHAR (300), @DateModifiée DATETIME
AS
UPDATE Archive SET [Path] = @Path, DateModifiée = @DateModifiée WHERE Id = @Id
GO

--\\\\\\\\__ END ARCHIVE __////////

--***************** FOR TESTING ******************
--DELETE FROM StagiaireÉtatSignature
--DELETE FROM Edité
--DELETE FROM Rejeté
--DELETE FROM Corrigé
--DELETE FROM Envoyé
--DELETE FROM Signé
--DELETE FROM RetraitBaccalauréat
--DELETE FROM RetraitDiplôme
--DELETE FROM StagiaireRetrait
--DELETE FROM Duplicata
--DELETE FROM StagiaireGroupe
--DELETE FROM StagiaireEmbauche
--DELETE FROM Stagiaire
--DELETE FROM Groupe
--DELETE FROM FilièreAnnée
--DELETE FROM Filière
--DELETE FROM Document
--DELETE FROM Note
--DELETE FROM Archive
--***********************************
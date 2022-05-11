USE master
GO
CREATE DATABASE G3D
GO
USE G3D
GO
CREATE TABLE �tablissement
	(
		Code NVARCHAR(6) PRIMARY KEY,
		Nom NVARCHAR(200) NOT NULL
	)
GO
INSERT INTO �tablissement VALUES ('R330', 'Institut Sp�cialis� de Technologie Appliqu�e Hay Al Adarissa F�s'), ('REH0', 'Centre Mixte de Formation Professionnelle F�s')
GO
CREATE TABLE Cl�Produit
	(
		Id INT PRIMARY KEY IDENTITY(1, 1),
		Cl� VARBINARY(100) NOT NULL UNIQUE,
		NombreUtilisations INT NULL,
		UtilisationsMaximales INT NOT NULL,
		�tablissement NVARCHAR(6) FOREIGN KEY REFERENCES �tablissement (Code) ON DELETE NO ACTION NULL,
		CHECK (NombreUtilisations <= UtilisationsMaximales)
	)
GO
CREATE PROC CheckCl� @Cl� NVARCHAR (100), @Id INT OUT, @Capacit� BIT OUT
AS
BEGIN
DECLARE @NombreUtilisations INT
DECLARE @UtilisationsMaximales INT
SELECT @Id = Id, @NombreUtilisations = NombreUtilisations, @UtilisationsMaximales = UtilisationsMaximales FROM Cl�Produit WHERE @Cl� = CONVERT(VARCHAR (100), DECRYPTBYPASSPHRASE('G3D', Cl�))
IF (@NombreUtilisations != NULL AND @NombreUtilisations = @UtilisationsMaximales)
SET @Capacit� = 0
ELSE
SET @Capacit� = 1
END
GO
INSERT INTO Cl�Produit (Cl�, UtilisationsMaximales) VALUES (ENCRYPTBYPASSPHRASE('G3D', 'B8E9B69DB512BE7'), 2)
GO
INSERT INTO Cl�Produit (Cl�, UtilisationsMaximales, �tablissement) VALUES (ENCRYPTBYPASSPHRASE('G3D', 'FFA9DADFD95C95F'), 3, 'R330')
GO
INSERT INTO Cl�Produit (Cl�, UtilisationsMaximales, �tablissement) VALUES (ENCRYPTBYPASSPHRASE('G3D', 'C65BCAD62C5AC7C'), 2, 'REH0')
GO
CREATE TABLE Utilisateur
	(
		Id INT IDENTITY (1,1) PRIMARY KEY,
		NomFamille NVARCHAR(30) NOT NULL,
		Pr�nom NVARCHAR(30) NOT NULL,
		NomUtilisateur NVARCHAR(20) NULL CHECK (LEN(NomUtilisateur) > 3),
		MotPasse VARBINARY(100) NOT NULL,
		EMail NVARCHAR(40) NOT NULL UNIQUE,
		Cl�Produit INT FOREIGN KEY REFERENCES Cl�Produit (Id) ON DELETE CASCADE,
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
CREATE PROC UtilisateurInsertQuery @NomFamille NVARCHAR(30), @Pr�nom NVARCHAR(30), @NomUtilisateur NVARCHAR(20), @MotPasse NVARCHAR(100), @EMail NVARCHAR(40), @Cl�Produit INT
AS
BEGIN
INSERT INTO Utilisateur (NomFamille, Pr�nom, NomUtilisateur, MotPasse, EMail, Cl�Produit, DateInscription) VALUES (@NomFamille, @Pr�nom, @NomUtilisateur, ENCRYPTBYPASSPHRASE('G3D', @MotPasse), @EMail, @Cl�Produit, GETDATE())
UPDATE Cl�Produit SET NombreUtilisations += 1 WHERE Id = @Cl�Produit
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
CREATE TABLE UtilisateurConnect�
	(
		Id INT IDENTITY (1,1) PRIMARY KEY,
		Utilisateur INT FOREIGN KEY REFERENCES Utilisateur (Id) ON DELETE CASCADE,
		Cha�neConnexion VARBINARY(100) NOT NULL,
		DateConnect� DATETIME NULL,
		DateD�connect� DATETIME NULL
	)
GO
CREATE PROC UtilisateurConnect�InsertQuery @Id INT
AS
BEGIN
INSERT INTO UtilisateurConnect� (Utilisateur, Cha�neConnexion, DateConnect�) VALUES (@Id, ENCRYPTBYPASSPHRASE('G3D', CONVERT(VARCHAR(10), IDENT_CURRENT('dbo.UtilisateurConnect�'))), GETDATE())
SELECT CONVERT(VARCHAR (100), Cha�neConnexion, 1) FROM UtilisateurConnect� WHERE Id = (IDENT_CURRENT('dbo.UtilisateurConnect�') - 1)
END
GO
CREATE PROC UtilisateurConnect�FindQuery @Cha�neConnexion NVARCHAR (100), @Id INT OUT
AS
BEGIN
SELECT @Id = Utilisateur FROM UtilisateurConnect� WHERE Id = CONVERT(VARCHAR (100), DECRYPTBYPASSPHRASE('G3D', CONVERT(VARBINARY(100), @Cha�neConnexion, 1))) AND DateD�connect� IS NULL
END
GO
CREATE PROC UtilisateurD�connect�EditQuery @Cha�neConnexion NVARCHAR (100)
AS
BEGIN
UPDATE UtilisateurConnect� SET DateD�connect� = GETDATE() WHERE Id = CONVERT(VARCHAR (100), DECRYPTBYPASSPHRASE('G3D', CONVERT(VARBINARY(100), @Cha�neConnexion, 1)))
END
GO
CREATE TABLE Niveau
	(
		NomCourt NVARCHAR(4) PRIMARY KEY,
		Nom NVARCHAR(50) NOT NULL
	)
GO
INSERT INTO Niveau VALUES ('TS', 'Technicien Sp�cialis�'), ('T', 'Technicien'), ('Q', 'Qualification'), ('S', 'Sp�cialisation')
GO
CREATE TABLE Fili�re
	(
		NomCourt NVARCHAR(20) PRIMARY KEY,
		Nom NVARCHAR(200) NOT NULL,
		Niveau NVARCHAR(4) FOREIGN KEY REFERENCES Niveau (NomCourt) ON DELETE NO ACTION
	)
GO
CREATE TRIGGER Fili�reInsert ON Fili�re INSTEAD OF INSERT
AS
IF (NOT EXISTS(SELECT * FROM Fili�re WHERE NomCourt in (SELECT NomCourt FROM INSERTED)))
BEGIN
	INSERT INTO Fili�re SELECT * FROM INSERTED
END
GO
CREATE TABLE Fili�reAnn�e
	(
		Id INT IDENTITY(1, 1) PRIMARY KEY,
		�tablissement NVARCHAR(6) FOREIGN KEY REFERENCES �tablissement (Code) ON DELETE NO ACTION,
		Fili�re NVARCHAR(20) FOREIGN KEY REFERENCES Fili�re (NomCourt) ON DELETE NO ACTION,
		Ann�eFormation NCHAR(9) NOT NULL,
		Promotion NVARCHAR(20) NOT NULL,
		UNIQUE (�tablissement, Fili�re, Ann�eFormation)
	)
GO
CREATE TRIGGER Fili�reAnn�eInsert ON Fili�reAnn�e INSTEAD OF INSERT
AS
IF (NOT EXISTS(SELECT * FROM Fili�reAnn�e FA JOIN INSERTED I ON FA.Fili�re = I.Fili�re  WHERE FA.Ann�eFormation = I.Ann�eFormation AND FA.�tablissement = I.�tablissement))
BEGIN
	INSERT INTO Fili�reAnn�e SELECT �tablissement, Fili�re, Ann�eFormation, Promotion FROM INSERTED
END
GO
CREATE PROC Fili�reAnn�eSelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4), @Ann�e�tude NVARCHAR(2)
AS
BEGIN
IF @Ann�e�tude != ''
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT FA.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = @Ann�e�tude
ELSE
SELECT DISTINCT FA.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND Ann�e�tude = @Ann�e�tude
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT FA.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = @Ann�e�tude
ELSE
SELECT DISTINCT FA.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE Ann�e�tude = @Ann�e�tude
END
END
ELSE
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT FA.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT FA.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT FA.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT FA.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id
END
END
END
GO
CREATE TABLE Ann�e�tude
	(
		NomCourt NVARCHAR(2) PRIMARY KEY,
		Nom NVARCHAR(10) NOT NULL,
	)
GO
INSERT INTO Ann�e�tude VALUES ('1A', '1�re Ann�e'), ('2A', '2�me Ann�e'), ('3A', '3�me Ann�e')
GO
CREATE TABLE TypeStagiaires
	(
		NomCourt NCHAR(2) PRIMARY KEY,
		Nom NVARCHAR(20) NOT NULL
	)
GO
INSERT INTO TypeStagiaires VALUES ('CR', 'Candidat R�gulier'), ('CL', 'Candidat Libre')
GO
CREATE TABLE Groupe
	(
		Id INT IDENTITY(1, 1) PRIMARY KEY,
		Fili�reAnn�e INT FOREIGN KEY REFERENCES Fili�reAnn�e (Id) ON DELETE NO ACTION,
		Num�ro NVARCHAR(3) NULL,
		Ann�e�tude NVARCHAR (2) NOT NULL FOREIGN KEY REFERENCES Ann�e�tude (NomCourt) ON DELETE NO ACTION,
		TypeFormation NVARCHAR(20) NULL,
		TypeStagiaires NCHAR(2) NOT NULL FOREIGN KEY REFERENCES TypeStagiaires (NomCourt) ON DELETE NO ACTION,
		UNIQUE (Fili�reAnn�e, Num�ro, Ann�e�tude)
	)
GO
CREATE TRIGGER GroupeInsert ON Groupe INSTEAD OF INSERT
AS
IF (NOT EXISTS(SELECT * FROM Groupe G JOIN INSERTED I ON G.Fili�reAnn�e = I.Fili�reAnn�e  WHERE G.Num�ro = I.Num�ro AND G.Ann�e�tude = I.Ann�e�tude))
BEGIN
	INSERT INTO Groupe SELECT Fili�reAnn�e, Num�ro, Ann�e�tude, TypeFormation, TypeStagiaires FROM INSERTED
END
GO
CREATE PROC GroupeSelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4), @Ann�e�tude NVARCHAR(2)
AS
BEGIN
IF @Ann�e�tude != ''
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT G.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = @Ann�e�tude
ELSE
SELECT G.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND Ann�e�tude = @Ann�e�tude
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT G.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = @Ann�e�tude
ELSE
SELECT * FROM Groupe WHERE Ann�e�tude = @Ann�e�tude
END
END
ELSE
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT G.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT G.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT G.* FROM Groupe G JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT * FROM Groupe
END
END
END
GO
CREATE TABLE Stagiaire
	(
		Cef NVARCHAR(20) PRIMARY KEY,
		NomPr�nom NVARCHAR(60) NOT NULL,
		Cin NVARCHAR(10) NULL UNIQUE,
		Num�roT�l�phonePremier NVARCHAR(20) NULL,
		Num�roT�l�phoneDeuxi�me NVARCHAR(20) NULL,
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
CREATE PROC UpdateStagiaire @Cef NVARCHAR(20), @Num�roT�l�phonePremier NVARCHAR(20), @Num�roT�l�phoneDeuxi�me NVARCHAR(20), @SituationActuel NVARCHAR(30)
AS
BEGIN
UPDATE Stagiaire SET Num�roT�l�phonePremier = @Num�roT�l�phonePremier, Num�roT�l�phoneDeuxi�me = @Num�roT�l�phoneDeuxi�me, SituationActuel = @SituationActuel WHERE Cef = @Cef
END
GO
CREATE PROC StagiaireSelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4), @Ann�e�tude NVARCHAR(2)
AS
BEGIN
IF @Ann�e�tude IS NOT NULL
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation IS NOT NULL
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = @Ann�e�tude
ELSE
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND Ann�e�tude = @Ann�e�tude
END
ELSE
BEGIN
IF @FAnn�eFormation IS NOT NULL
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = @Ann�e�tude
ELSE
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id WHERE Ann�e�tude = @Ann�e�tude
END
END
ELSE
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation IS NOT NULL
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement
END
ELSE
BEGIN
IF @FAnn�eFormation IS NOT NULL
SELECT DISTINCT S.* FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
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
CREATE PROC StagiaireGroupeSelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4), @Ann�e�tude NVARCHAR(2)
AS
BEGIN
IF @Ann�e�tude != ''
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = @Ann�e�tude
ELSE
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND Ann�e�tude = @Ann�e�tude
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = @Ann�e�tude
ELSE
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id WHERE Ann�e�tude = @Ann�e�tude
END
END
ELSE
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT SG.* FROM StagiaireGroupe SG JOIN Groupe G ON SG.Groupe = G.Id
END
END
END
GO
CREATE TABLE Stagiaire�tatSignature
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		�tat NVARCHAR (10) NULL CHECK (�tat = 'Edit�' OR �tat = 'Rejet�' OR �tat = 'Corrig�' OR �tat = 'Envoy�' OR �tat = 'Sign�'),
		�tatDate DATETIME,
		PRIMARY KEY (StagiaireGroupe)
	)
GO
CREATE TABLE StagiaireEmbauche
	(
		Stagiaire NVARCHAR(20) FOREIGN KEY REFERENCES Stagiaire (Cef) ON DELETE NO ACTION,
		RaisonSociale NVARCHAR(100) NULL,
		DateD�but DATE NULL,
		DateInsertion DATETIME NOT NULL,
		UNIQUE (Stagiaire, RaisonSociale)
	)
GO
CREATE TABLE StagiaireRetrait
	(
		Id INT IDENTITY (1,1) PRIMARY KEY,
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		TypeDocument NVARCHAR(30) NOT NULL CHECK (TypeDocument = 'Dipl�me' OR TypeDocument = 'Baccalaur�at' OR TypeDocument = 'Relev� de Notes' OR TypeDocument = 'Attestation de R�ussite'),
		DateRetrait DATE NULL,
		UNIQUE (StagiaireGroupe, TypeDocument, DateRetrait)
	)
GO
CREATE TABLE RetraitBaccalaur�at
	(
		StagiaireRetrait INT FOREIGN KEY REFERENCES StagiaireRetrait (Id) ON DELETE NO ACTION,
		Num�roS�rie NVARCHAR(30) NULL,
		UNIQUE (StagiaireRetrait)
	)
GO
CREATE TABLE RetraitDipl�me
	(
		StagiaireRetrait INT FOREIGN KEY REFERENCES StagiaireRetrait (Id) ON DELETE NO ACTION,
		Num�roS�rie NVARCHAR(30) NULL,
		Cab NVARCHAR (30) NULL,
		DocumentV�rification NVARCHAR(20) NULL CHECK (DocumentV�rification = 'Cin' OR DocumentV�rification = 'Procuration'),
		CheminDocumentV�rification NVARCHAR(100) NULL,
		UNIQUE (StagiaireRetrait)
	)
GO
CREATE TABLE Edit�
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateEdit� DATETIME NOT NULL,
		UNIQUE (StagiaireGroupe, DateEdit�)
	)
GO
CREATE TRIGGER Edit�Insert ON Edit� AFTER INSERT
AS
DELETE Stagiaire�tatSignature WHERE StagiaireGroupe in (SELECT StagiaireGroupe FROM INSERTED)
INSERT INTO Stagiaire�tatSignature SELECT StagiaireGroupe, 'Edit�', DateEdit� FROM INSERTED
GO
CREATE PROC Edit�SelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4)
AS
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT E.* FROM Edit� E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT E.* FROM Edit� E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT E.* FROM Edit� E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT * FROM Edit�
END
END
GO
CREATE TABLE Rejet�
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateRejet� DATETIME NOT NULL,
		Motif NVARCHAR(100) NULL,
		UNIQUE (StagiaireGroupe, DateRejet�)
	)
GO
CREATE PROC Rejet�SelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4)
AS
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT R.* FROM Rejet� R JOIN StagiaireGroupe SG ON R.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT R.* FROM Rejet� R JOIN StagiaireGroupe SG ON R.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT R.* FROM Rejet� R JOIN StagiaireGroupe SG ON R.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT * FROM Rejet�
END
END
GO
CREATE TRIGGER Rejet�Insert ON Rejet� AFTER INSERT
AS
DELETE Stagiaire�tatSignature WHERE StagiaireGroupe in (SELECT StagiaireGroupe FROM INSERTED)
INSERT INTO Stagiaire�tatSignature SELECT StagiaireGroupe, 'Rejet�', DateRejet� FROM INSERTED
GO
CREATE TABLE Corrig�
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateCorrig� DATETIME NOT NULL,
		UNIQUE (StagiaireGroupe, DateCorrig�)
	)
GO
CREATE PROC Corrig�SelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4)
AS
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT C.* FROM Corrig� C JOIN StagiaireGroupe SG ON C.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT C.* FROM Corrig� C JOIN StagiaireGroupe SG ON C.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT C.* FROM Corrig� C JOIN StagiaireGroupe SG ON C.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT * FROM Corrig�
END
END
GO
CREATE TRIGGER Corrig�Insert ON Corrig� AFTER INSERT
AS
DELETE Stagiaire�tatSignature WHERE StagiaireGroupe in (SELECT StagiaireGroupe FROM INSERTED)
INSERT INTO Stagiaire�tatSignature SELECT StagiaireGroupe, 'Corrig�', DateCorrig� FROM INSERTED
GO
CREATE TABLE Envoy�
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateEnvoy� DATETIME NOT NULL,
		UNIQUE (StagiaireGroupe, DateEnvoy�)
	)
GO
CREATE PROC Envoy�SelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4)
AS
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT E.* FROM Envoy� E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT E.* FROM Envoy� E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT E.* FROM Envoy� E JOIN StagiaireGroupe SG ON E.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT * FROM Envoy�
END
END
GO
CREATE TRIGGER Envoy�Insert ON Envoy� AFTER INSERT
AS
DELETE Stagiaire�tatSignature WHERE StagiaireGroupe in (SELECT StagiaireGroupe FROM INSERTED)
INSERT INTO Stagiaire�tatSignature SELECT StagiaireGroupe, 'Envoy�', DateEnvoy� FROM INSERTED
GO
CREATE TABLE Sign�
	(
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateSign� DATETIME NOT NULL,
		UNIQUE (StagiaireGroupe, DateSign�)
	)
GO
CREATE PROC Sign�SelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4)
AS
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT S.* FROM Sign� S JOIN StagiaireGroupe SG ON S.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT S.* FROM Sign� S JOIN StagiaireGroupe SG ON S.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT DISTINCT S.* FROM Sign� S JOIN StagiaireGroupe SG ON S.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation
ELSE
SELECT DISTINCT * FROM Sign�
END
END
GO
CREATE TRIGGER Sign�Insert ON Sign� AFTER INSERT
AS
DELETE Stagiaire�tatSignature WHERE StagiaireGroupe in (SELECT StagiaireGroupe FROM INSERTED)
INSERT INTO Stagiaire�tatSignature SELECT StagiaireGroupe, 'Sign�', DateSign� FROM INSERTED
GO
CREATE TABLE Duplicata
	(
		Id INT IDENTITY (1,1) PRIMARY KEY,
		StagiaireGroupe INT FOREIGN KEY REFERENCES StagiaireGroupe (Id) ON DELETE NO ACTION,
		DateNaissance DATE NOT NULL,
		Lieu NVARCHAR (200) NULL,
		Num�roS�rie NVARCHAR(30) NOT NULL,
		Cab NVARCHAR (30) NOT NULL,
		ModeFormation NVARCHAR(30) NULL
	)
GO
CREATE VIEW DuplicataFiche
AS
SELECT D.Id DupId, SG.Id StagiaireGroupe, Cef, NomPr�nom, Cin, DateNaissance, Lieu, Num�roS�rie, Cab, Num�roT�l�phonePremier, Num�roT�l�phoneDeuxi�me, FA.Fili�re Fili�reCourt, Num�ro, ModeFormation, TypeFormation, TS.Nom AS TypeStagiaire, Ann�eFormation, N.Nom As Niveau, F.Nom AS Fili�re, E.Nom AS �tablissement, Promotion AS [Session] FROM Duplicata D JOIN StagiaireGroupe SG ON D.StagiaireGroupe = SG.Id JOIN Stagiaire S ON SG.Stagiaire = S.Cef JOIN Groupe G ON SG.Groupe = G.Id JOIN TypeStagiaires TS ON G.TypeStagiaires = TS.NomCourt JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id JOIN Fili�re F ON FA.Fili�re = F.NomCourt JOIN Niveau N ON F.Niveau = N.NomCourt JOIN �tablissement E ON FA.�tablissement = E.Code
GO

CREATE PROC UpdateStagiaireGroupe @OldGroupId INT, @NewGroupId INT, @Stagiaire NVARCHAR(20), @Classement INT, @Admis NCHAR(3), @Cin NVARCHAR(10), @NomPr�nom NVARCHAR(60)
AS
BEGIN
UPDATE StagiaireGroupe SET Groupe = @NewGroupId, Admis = 'Oui', Classement = @Classement WHERE Stagiaire = @Stagiaire AND Groupe = @OldGroupId
IF (@Cin != NULL OR @NomPr�nom!= NULL)
UPDATE Stagiaire SET Cin = @Cin, NomPr�nom = @NomPr�nom WHERE Cef = @Stagiaire
END
GO

CREATE PROCEDURE GetAdmisResult @TypeStagiaires NCHAR(2), @Admis NCHAR(3)
AS
SELECT COUNT(*) AS AdmisCount, FA.Fili�re AS NomCourt FROM StagiaireGroupe SG join Groupe G on SG.Groupe = G.Id join Fili�reAnn�e FA on G.Fili�reAnn�e = FA.Id WHERE Ann�e�tude = '2A' AND TypeStagiaires = @TypeStagiaires and Admis = @Admis GROUP BY FA.Fili�re
GO

CREATE PROCEDURE Fili�reAnn�eInsertQuery @�tablissement NVARCHAR(6), @Fili�re NVARCHAR(20), @Ann�eFormation NCHAR(9), @Id INT OUT
AS
BEGIN
INSERT INTO [dbo].[Fili�reAnn�e] ([�tablissement], [Fili�re], [Ann�eFormation]) VALUES (@�tablissement, @Fili�re, @Ann�eFormation);
SELECT @Id = Id FROM [dbo].[Fili�reAnn�e] WHERE [�tablissement] = @�tablissement AND [Fili�re] = @Fili�re AND [Ann�eFormation] = @Ann�eFormation
END
GO
CREATE PROCEDURE GroupeInsertQuery @Fili�reAnn�e INT, @Num�ro NVARCHAR(3), @Ann�e�tude NVARCHAR(2), @TypeFormation NVARCHAR(20), @TypeStagiaires NCHAR(2), @Id INT OUT
AS
BEGIN
INSERT INTO [dbo].[Groupe] ([Fili�reAnn�e], [Num�ro], [Ann�e�tude], [TypeFormation], [TypeStagiaires]) VALUES (@Fili�reAnn�e, @Num�ro, @Ann�e�tude, @TypeFormation, @TypeStagiaires);
SELECT @Id = Id FROM [dbo].[Groupe] WHERE [Fili�reAnn�e] = @Fili�reAnn�e AND [Num�ro] = @Num�ro AND [Ann�e�tude] = @Ann�e�tude
END
GO

CREATE VIEW DiplomeRegistreRetraitFull AS
SELECT Nom, Fili�re, Num�ro, NomPr�nom , Cin, DateRetrait, Num�roT�l�phonePremier, Num�roS�rie, SituationActuel, Promotion FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id JOIN Fili�re F ON FA.Fili�re = F.NomCourt JOIN StagiaireRetrait SR ON SG.Id = SR.StagiaireGroupe JOIN RetraitDipl�me RD ON SR.Id = RD.StagiaireRetrait join Stagiaire�tatSignature SES ON SG.Id = SES.StagiaireGroupe
GO
CREATE VIEW DiplomeRegistreRetrait AS
SELECT Fili�re, Num�ro, NomPr�nom , Promotion FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id JOIN Fili�re F ON FA.Fili�re = F.NomCourt
GO
CREATE VIEW SuiviDesSignatures AS
SELECT Cef, NomPr�nom, Fili�re, Num�ro, �tat, �tatDate, Promotion FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Stagiaire�tatSignature SES ON SES.StagiaireGroupe = SG.Id JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id
GO
CREATE VIEW DiplomeBordoreu AS
SELECT S.NomPr�nom, FA.Fili�re, G.Num�ro, E.DateEnvoy� FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Stagiaire�tatSignature SES ON SG.Id = SES.StagiaireGroupe JOIN Envoy� E ON SG.Id = E.StagiaireGroupe JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE DATEDIFF(SECOND, E.DateEnvoy�, SES.�tatDate) >= 0
GO
CREATE VIEW DuplicataBordoreu AS
SELECT S.NomPr�nom, FA.Fili�re, G.Num�ro, E.DateEnvoy� FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Duplicata D ON SG.Id = D.StagiaireGroupe JOIN Stagiaire�tatSignature SES ON SG.Id = SES.StagiaireGroupe JOIN Envoy� E ON SG.Id = E.StagiaireGroupe JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE DATEDIFF(SECOND, E.DateEnvoy�, SES.�tatDate) >= 0
GO
CREATE PROC ContactSelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4)
AS
BEGIN
IF @�tablissement != ''
BEGIN
IF @FAnn�eFormation != ''
SELECT S.NomPr�nom, S.Num�roT�l�phonePremier, S.Num�roT�l�phoneDeuxi�me, SG.Classement, G.Num�ro, FA.Fili�re, S.SituationActuel FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = '2A'
ELSE
SELECT S.NomPr�nom, S.Num�roT�l�phonePremier, S.Num�roT�l�phoneDeuxi�me, SG.Classement, G.Num�ro, FA.Fili�re, S.SituationActuel FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE �tablissement = @�tablissement AND Ann�e�tude = '2A'
END
ELSE
BEGIN
IF @FAnn�eFormation != ''
SELECT S.NomPr�nom, S.Num�roT�l�phonePremier, S.Num�roT�l�phoneDeuxi�me, SG.Classement, G.Num�ro, FA.Fili�re, S.SituationActuel FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = '2A'
ELSE
SELECT S.NomPr�nom, S.Num�roT�l�phonePremier, S.Num�roT�l�phoneDeuxi�me, SG.Classement, G.Num�ro, FA.Fili�re, S.SituationActuel FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id WHERE Ann�e�tude = '2A'
END
END
GO

CREATE PROC InsertFullRow @Fili�reCourt NVARCHAR(20), @Fili�re NVARCHAR(200), @Niveau NVARCHAR(4), @�tablissementCourt NVARCHAR(6), @Ann�eFormation NCHAR(9), @Promotion NVARCHAR(20), @Num�ro NVARCHAR(3), @Ann�e�tude NVARCHAR(2), @TypeFormation NVARCHAR(20), @TypeStagiaires NCHAR(2), @Cef NVARCHAR(20), @NomPr�nom NVARCHAR(60), @Cin NVARCHAR(10), @Admis NCHAR(3), @Classement INT
AS
BEGIN
DECLARE @IdFili�reAnn�e INT
DECLARE @IdGroupe INT
INSERT INTO Fili�re VALUES (@Fili�reCourt, @Fili�re, @Niveau)
INSERT INTO Fili�reAnn�e VALUES (@�tablissementCourt, @Fili�reCourt, @Ann�eFormation, @Promotion)
SELECT @IdFili�reAnn�e = Id FROM Fili�reAnn�e WHERE �tablissement = @�tablissementCourt AND Fili�re = @Fili�reCourt AND Ann�eFormation = @Ann�eFormation
INSERT INTO Groupe VALUES (@IdFili�reAnn�e, @Num�ro, @Ann�e�tude, @TypeFormation, @TypeStagiaires)
SELECT @IdGroupe = Id FROM Groupe WHERE Fili�reAnn�e = @IdFili�reAnn�e AND Num�ro = @Num�ro AND Ann�e�tude = @Ann�e�tude
INSERT INTO Stagiaire (Cef, NomPr�nom, Cin) VALUES (@Cef, @NomPr�nom, @Cin)
INSERT INTO StagiaireGroupe VALUES (@Cef, @IdGroupe, @Admis, @Classement)
END
GO


CREATE PROC InsertDuplicataRow @Fili�reCourt NVARCHAR(20), @Fili�re NVARCHAR(200), @Niveau NVARCHAR(4), @�tablissementCourt NVARCHAR(6), @Ann�eFormation NCHAR(9), @Promotion NVARCHAR(20), @Num�ro NVARCHAR(3), @ModeFormation NVARCHAR(20), @TypeFormation NVARCHAR(20), @TypeStagiaires NCHAR(2), @Cef NVARCHAR(20), @NomPr�nom NVARCHAR(60), @Cin NVARCHAR(10), @Num�roT�l�phonePremier NVARCHAR(20), @Num�roT�l�phoneDeuxi�me NVARCHAR(20), @DateNaissance DATE, @Lieu NVARCHAR (200), @Num�roS�rie NVARCHAR(30), @Cab NVARCHAR (30)
AS
BEGIN
DECLARE @IdFili�reAnn�e INT
DECLARE @IdGroupe INT
DECLARE @IdStagiaireGroupe INT
INSERT INTO Fili�re VALUES (@Fili�reCourt, @Fili�re, @Niveau)
INSERT INTO Fili�reAnn�e VALUES (@�tablissementCourt, @Fili�reCourt, @Ann�eFormation, @Promotion)
SELECT @IdFili�reAnn�e = Id FROM Fili�reAnn�e WHERE �tablissement = @�tablissementCourt AND Fili�re = @Fili�reCourt AND Ann�eFormation = @Ann�eFormation
INSERT INTO Groupe VALUES (@IdFili�reAnn�e, @Num�ro, '2A', @TypeFormation, @TypeStagiaires)
SELECT @IdGroupe = Id FROM Groupe WHERE Fili�reAnn�e = @IdFili�reAnn�e AND Num�ro = @Num�ro AND Ann�e�tude = '2A'
INSERT INTO Stagiaire VALUES (@Cef, @NomPr�nom, @Cin, @Num�roT�l�phonePremier, @Num�roT�l�phoneDeuxi�me, NULL)
INSERT INTO StagiaireGroupe VALUES (@Cef, @IdGroupe, 'Oui', '0')
SELECT @IdStagiaireGroupe = Id FROM StagiaireGroupe WHERE Stagiaire = @Cef AND Groupe = @IdGroupe
INSERT INTO Duplicata VALUES (@IdStagiaireGroupe, @DateNaissance, @Lieu, @Num�roS�rie, @Cab, @ModeFormation)
END
GO

CREATE PROC UpdateDuplicata @DuplicataId INT, @NewStagiaireGroupeId INT, @DateNaissance DATE, @Lieu NVARCHAR (200), @Num�roS�rie NVARCHAR(30), @Cab NVARCHAR (30), @Cef NVARCHAR(20), @NomPr�nom NVARCHAR(60), @Cin NVARCHAR(10), @Num�roT�l�phonePremier NVARCHAR(20), @Num�roT�l�phoneDeuxi�me NVARCHAR(20), @ModeFormation NVARCHAR(20)
AS
BEGIN
UPDATE Duplicata SET StagiaireGroupe = @NewStagiaireGroupeId, DateNaissance = @DateNaissance, Lieu = @Lieu, Num�roS�rie = @Num�roS�rie, Cab = @Cab, ModeFormation = @ModeFormation WHERE Id = @DuplicataId
UPDATE Stagiaire SET NomPr�nom = @NomPr�nom, Cin = @Cin,  Num�roT�l�phonePremier = @Num�roT�l�phonePremier, Num�roT�l�phoneDeuxi�me = @Num�roT�l�phoneDeuxi�me WHERE Cef = @Cef
END
GO

CREATE VIEW CheckListView AS
SELECT S.Cef, S.NomPr�nom, S.Cin, G.Num�ro, G.TypeStagiaires, F.NomCourt AS Fili�reNomCourt, F.Nom AS Fili�reNom, N.Nom AS NiveauNom, E.Code AS �tablissementCode, E.Nom AS �tablissementNom, FA.Ann�eFormation FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id JOIN Fili�re F ON FA.Fili�re = F.NomCourt JOIN Niveau N ON F.Niveau = N.NomCourt JOIN �tablissement E ON FA.�tablissement = E.Code WHERE G.Ann�e�tude = '2A'
GO

CREATE PROC CheckListViewSelectQuery @�tablissement NVARCHAR(6), @FAnn�eFormation NCHAR(4)
AS
BEGIN
IF @�tablissement != ''
SELECT S.Cef, S.NomPr�nom, S.Cin, G.Num�ro, G.TypeStagiaires, F.NomCourt AS Fili�reNomCourt, F.Nom AS Fili�reNom, N.Nom AS NiveauNom, E.Code AS �tablissementCode, E.Nom AS �tablissementNom, FA.Ann�eFormation FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id JOIN Fili�re F ON FA.Fili�re = F.NomCourt JOIN Niveau N ON F.Niveau = N.NomCourt JOIN �tablissement E ON FA.�tablissement = E.Code WHERE E.Code = @�tablissement AND SUBSTRING(Ann�eFormation, 1, 4) = @FAnn�eFormation AND Ann�e�tude = '2A'
ELSE
SELECT S.Cef, S.NomPr�nom, S.Cin, G.Num�ro, F.NomCourt AS Fili�reNomCourt, F.Nom AS Fili�reNom, N.Nom AS NiveauNom, E.Code AS �tablissementCode, E.Nom AS �tablissementNom, FA.Ann�eFormation FROM Stagiaire S JOIN StagiaireGroupe SG ON S.Cef = SG.Stagiaire JOIN Groupe G ON SG.Groupe = G.Id JOIN Fili�reAnn�e FA ON G.Fili�reAnn�e = FA.Id JOIN Fili�re F ON FA.Fili�re = F.NomCourt JOIN Niveau N ON F.Niveau = N.NomCourt JOIN �tablissement E ON FA.�tablissement = E.Code WHERE SUBSTRING(Ann�eFormation, 1, 4) = '2020' AND Ann�e�tude = '2A'
END
GO

--\\\\\\\\\\__ ARCHIVE __//////////

CREATE TABLE Archive
	(
		Id INT IDENTITY (1, 1) PRIMARY KEY,
		DateCr��e DATETIME NOT NULL,
		DateModifi�e DATETIME NULL,
		[Path] NVARCHAR (300) NOT NULL
	)

CREATE TABLE Note
	(
		[PathId] INT FOREIGN KEY REFERENCES Archive (Id) ON DELETE CASCADE,
		R�f�rence NVARCHAR (100) NULL,
		Objectif NVARCHAR (100) NULL,
		[Date] Date NULL,
	)
GO

CREATE TABLE Document
	(
		[PathId] INT FOREIGN KEY REFERENCES Archive (Id) ON DELETE CASCADE,
		Document NVARCHAR (30) NOT NULL CHECK (Document = 'Bulletin' OR Document = 'Dipl�me' OR Document = 'Attestation de r�ussite' OR Document = 'Pv'),
		Ann�eFormation NCHAR (9) NOT NULL,
		Niveau NVARCHAR (30) NOT NULL,
		Ann�e�tude NVARCHAR (20) NOT NULL,
		Fili�re NVARCHAR(20) NOT NULL,
		Groupe NCHAR (3) NOT NULL
	)
GO

CREATE PROC ArchiveInsertQuery @DateCr��e DATETIME, @DateModifi�e DATETIME, @Path NVARCHAR (300), @Id INT OUT
AS
BEGIN
INSERT INTO Archive VALUES (@DateCr��e, @DateModifi�e, @Path)
SELECT @Id = Id FROM Archive WHERE DateCr��e = @DateCr��e AND [Path] = @Path
END
GO

CREATE PROC ArchiveUpdateQuery @Id INT, @Path NVARCHAR (300), @DateModifi�e DATETIME
AS
UPDATE Archive SET [Path] = @Path, DateModifi�e = @DateModifi�e WHERE Id = @Id
GO

--\\\\\\\\__ END ARCHIVE __////////

--***************** FOR TESTING ******************
--DELETE FROM Stagiaire�tatSignature
--DELETE FROM Edit�
--DELETE FROM Rejet�
--DELETE FROM Corrig�
--DELETE FROM Envoy�
--DELETE FROM Sign�
--DELETE FROM RetraitBaccalaur�at
--DELETE FROM RetraitDipl�me
--DELETE FROM StagiaireRetrait
--DELETE FROM Duplicata
--DELETE FROM StagiaireGroupe
--DELETE FROM StagiaireEmbauche
--DELETE FROM Stagiaire
--DELETE FROM Groupe
--DELETE FROM Fili�reAnn�e
--DELETE FROM Fili�re
--DELETE FROM Document
--DELETE FROM Note
--DELETE FROM Archive
--***********************************
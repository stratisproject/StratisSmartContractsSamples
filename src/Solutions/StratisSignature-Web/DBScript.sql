CREATE DATABASE SignatureDB;
GO 

USE [SignatureDB]
GO

Create FUNCTION [dbo].[Split] (
      @InputString                  VARCHAR(8000),
      @Delimiter                    VARCHAR(50)
)

RETURNS @Items TABLE (
      Item                          VARCHAR(8000)
)

AS
BEGIN
      IF @Delimiter = ' '
      BEGIN
            SET @Delimiter = ','
            SET @InputString = REPLACE(@InputString, ' ', @Delimiter)
      END

      IF (@Delimiter IS NULL OR @Delimiter = '')
            SET @Delimiter = ','

      DECLARE @Item           VARCHAR(8000)
      DECLARE @ItemList       VARCHAR(8000)
      DECLARE @DelimIndex     INT

      SET @ItemList = @InputString
      SET @DelimIndex = CHARINDEX(@Delimiter, @ItemList, 0)
      WHILE (@DelimIndex != 0)
      BEGIN
            SET @Item = SUBSTRING(@ItemList, 0, @DelimIndex)
            INSERT INTO @Items VALUES (@Item)

            -- Set @ItemList = @ItemList minus one less item
            SET @ItemList = SUBSTRING(@ItemList, @DelimIndex+1, LEN(@ItemList)-@DelimIndex)
            SET @DelimIndex = CHARINDEX(@Delimiter, @ItemList, 0)
      END -- End WHILE

      IF @Item IS NOT NULL -- At least one delimiter was encountered in @InputString
      BEGIN
            SET @Item = @ItemList
            INSERT INTO @Items VALUES (@Item)
      END

      -- No delimiters were encountered in @InputString, so just return @InputString
      ELSE INSERT INTO @Items VALUES (@InputString)

      RETURN

END -- End Function
GO

/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 15-Dec-19 9:31:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 15-Dec-19 9:31:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 15-Dec-19 9:31:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 15-Dec-19 9:31:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [uniqueidentifier] NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[FirstName] [varchar](100) NULL,
	[LastName] [varchar](100) NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[WalletAddress] [varchar](100) NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/****** Object:  Table [dbo].[Contacts]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contacts](
	[ContactId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[RequestedBy] [uniqueidentifier] NOT NULL,
	[FirstName] [varchar](100) NULL,
	[LastName] [varchar](100) NULL,
	[Email] [varchar](256) NULL,
 CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED 
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[Documents]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Documents](
	[Id] [uniqueidentifier] NOT NULL,
	[DocumentName] [varchar](500) NOT NULL,
	[Description] [varchar](max) NULL,
	[FileName] [varchar](500) NULL,
	[DocumentData] [varbinary](max) NULL,
	[SignedDocumentData] [varbinary](max) NULL,
	[DocumentHash] [varchar](max) NULL,
	[SignedDocumentHash] [varchar](max) NULL,
	[CreationTime] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[DocumentStatus] [bit] NOT NULL,
	[CreationTxHash] [varchar](500) NULL,
	[FinalSignTxHash] [varchar](500) NULL,
	[BlockNumber] [bigint] NULL,
 CONSTRAINT [PK_Documents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/****** Object:  Table [dbo].[Shapes]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shapes](
	[Id] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[SignerId] [uniqueidentifier] NOT NULL,
	[X] [int] NOT NULL,
	[Y] [int] NOT NULL,
 CONSTRAINT [PK_Shapes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[User_Document_Mapping]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_Document_Mapping](
	[Id] [uniqueidentifier] NOT NULL,
	[DocumentId] [uniqueidentifier] NOT NULL,
	[SignerId] [uniqueidentifier] NOT NULL,
	[TransactionHash] [varchar](max) NULL,
	[Signed] [bit] NULL,
	[BlockNumber] [bigint] NULL,
 CONSTRAINT [PK_User_Document_Mapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Contacts]  WITH CHECK ADD  CONSTRAINT [FK_Contacts_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Contacts] CHECK CONSTRAINT [FK_Contacts_AspNetUsers]
GO
ALTER TABLE [dbo].[Contacts]  WITH CHECK ADD  CONSTRAINT [FK_Contacts_AspNetUsers1] FOREIGN KEY([RequestedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Contacts] CHECK CONSTRAINT [FK_Contacts_AspNetUsers1]
GO
ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [FK_Documents_AspNetUsers] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Documents] CHECK CONSTRAINT [FK_Documents_AspNetUsers]
GO
ALTER TABLE [dbo].[Shapes]  WITH CHECK ADD  CONSTRAINT [FK_Shapes_AspNetUsers] FOREIGN KEY([SignerId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Shapes] CHECK CONSTRAINT [FK_Shapes_AspNetUsers]
GO
ALTER TABLE [dbo].[Shapes]  WITH CHECK ADD  CONSTRAINT [FK_Shapes_Documents] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Documents] ([Id])
GO
ALTER TABLE [dbo].[Shapes] CHECK CONSTRAINT [FK_Shapes_Documents]
GO
ALTER TABLE [dbo].[User_Document_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_User_Document_Mapping_AspNetUsers] FOREIGN KEY([SignerId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[User_Document_Mapping] CHECK CONSTRAINT [FK_User_Document_Mapping_AspNetUsers]
GO
ALTER TABLE [dbo].[User_Document_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_User_Document_Mapping_Documents] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Documents] ([Id])
GO
ALTER TABLE [dbo].[User_Document_Mapping] CHECK CONSTRAINT [FK_User_Document_Mapping_Documents]
GO
/****** Object:  StoredProcedure [dbo].[AddContact]    Script Date: 15-Dec-19 9:31:24 PM ******/



/****************************  StoredProcedure ************************************************/

/****** Object:  StoredProcedure [dbo].[AddContact]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddContact] @UserId      UNIQUEIDENTIFIER, 
                                   @FirstName   VARCHAR(100), 
                                   @LastName    VARCHAR(100), 
                                   @Email       VARCHAR(256), 
                                   @RequestedBy UNIQUEIDENTIFIER
AS
    BEGIN

        SET NOCOUNT OFF;
        DECLARE @NewContactId UNIQUEIDENTIFIER= NEWID();

        INSERT INTO Contacts
        (ContactId, 
         UserId, 
         RequestedBy, 
         FirstName, 
         LastName, 
         Email
        )
        VALUES
        (@NewContactId, 
         @UserId, 
         @RequestedBy, 
         @FirstName, 
         @LastName, 
         @Email
        );
        SELECT @@ROWCOUNT;
    END;
GO

/****** Object:  StoredProcedure [dbo].[AddDocument]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddDocument]

@DocumentName Varchar(500),
@FileName Varchar(500),
@Description Varchar(MAX),
@DocumentHash Varchar(MAX),
@DocumentData varbinary(MAX),
@CreatedBy uniqueidentifier,
@DocumentStatus bit,
@SignersUserIds Varchar(Max),
@DocumentId uniqueidentifier output 
	
AS
BEGIN
	
	SET NOCOUNT OFF;
	DECLARE @table table (id uniqueidentifier)

	Insert into Documents(Id, DocumentName, [Description], [FileName], DocumentHash, DocumentData, CreationTime, DocumentStatus, CreatedBy)
	Output Inserted.Id  into @table
	values (NewID(), @DocumentName, @Description, @FileName, @DocumentHash, @DocumentData, GETUTCDATE(), @DocumentStatus, @CreatedBy)

	select @DocumentId = id from @table

     DECLARE @Signer TABLE  (SignerId uniqueidentifier) 
	 insert into @Signer
	 select * from [dbo].[Split](@SignersUserIds,',')

	 DECLARE @SigenerCursor CURSOR;
		DECLARE @SignerId uniqueidentifier;
		Declare @Index int =1;
		Begin
		SET @SigenerCursor = CURSOR FOR
			select SignerId from @Signer
			
			OPEN @SigenerCursor 
			FETCH NEXT FROM @SigenerCursor 
			INTO @SignerId

			WHILE @@FETCH_STATUS = 0
			BEGIN

	             Insert into User_Document_Mapping (Id, DocumentId, SignerId, Signed) 
				 values (NEWID(), @DocumentId, @SignerId, 0)

				 Insert into Shapes (Id, DocumentId, SignerId, X, Y)
				 values (NEWID(), @DocumentId, @SignerId, CAST(CONCAT(@Index ,'00') as INT), CAST(CONCAT(@Index ,'00') as INT))

			   set @Index = @Index + 1
				
				FETCH NEXT FROM @SigenerCursor 
				INTO @SignerId 
			End
			CLOSE @SigenerCursor ;
			DEALLOCATE @SigenerCursor;
		End

END
GO

/****** Object:  StoredProcedure [dbo].[DeleteContact]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteContact] 

@ContactId UNIQUEIDENTIFIER

AS
    BEGIN
        SET NOCOUNT OFF;
        DELETE FROM Contacts
        WHERE ContactId = @ContactId;
    END;
GO
/****** Object:  StoredProcedure [dbo].[GetContacts]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetContacts] 

@UserId UNIQUEIDENTIFIER

AS
     BEGIN
	
         SET NOCOUNT ON;
         SELECT c.ContactId,
		        c.UserId,
                c.RequestedBy,
                c.FirstName,
                c.LastName,
                c.Email
         FROM Contacts c
         WHERE RequestedBy = @UserId;
     END;
GO

/****** Object:  StoredProcedure [dbo].[GetDocumentCounts]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDocumentCounts] 

@UserId Uniqueidentifier
	
AS
BEGIN
	
	SET NOCOUNT OFF;

	--awaiting my signs
	Select Count(*) as AwaitingSignCount from User_Document_Mapping Udm	
	where Udm.SignerId = @UserId AND Udm.Signed = 0
	
	Select distinct Count(*) as CompletedCount from User_Document_Mapping Udm
	Join Documents Doc on Udm.DocumentId = Doc.Id
	where (Udm.SignerId = @UserId OR  doc.CreatedBy = @UserId) AND Udm.Signed = 1

END
GO

/****** Object:  StoredProcedure [dbo].[GetDocumentDetail]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDocumentDetail] 

@DocumentId uniqueidentifier	

AS
BEGIN
	
	SET NOCOUNT OFF;

    SELECT doc.Id,
       Doc.DocumentName,
       Doc.[Description],
       Doc.[FileName],
       Doc.CreationTime,
	   Doc.DocumentHash,
       Doc.CreationTxHash,
	   Doc.DocumentStatus as [Status],
	   Doc.DocumentData as [Data],
	   Doc.SignedDocumentData as [SignedData],
	   Doc.BlockNumber,
	   Doc.FinalSignTxHash as SignTxHash,
	   Anu.FirstName + ' ' + Anu.LastName AS UploadedBy
	FROM Documents Doc
	JOIN AspNetUsers Anu ON Doc.CreatedBy= Anu.Id
	WHERE doc.Id = @DocumentId;
	
	SELECT UDM.SignerId,
	       UDM.Signed,
	       --UDM.TransactionHash,
	       ANU.FirstName,
	       ANU.LastName,
	       ANU.Email,
	       ANU.Id AS UserId
	FROM User_Document_Mapping UDM
	     JOIN AspNetUsers ANU ON UDM.SignerId = ANU.Id		 
	WHERE UDM.DocumentId = @DocumentId;

END
GO

/****** Object:  StoredProcedure [dbo].[GetDocuments]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDocuments] 

@UserId UNIQUEIDENTIFIER

AS
    BEGIN        
        SET NOCOUNT ON;
        SELECT DISTINCT 
               doc.Id, 
               doc.DocumentName, 
               doc.[Description], 
               doc.[FileName], 
               doc.DocumentHash, 
               doc.DocumentData, 
               doc.CreationTime,
               doc.CreatedBy,
               doc.DocumentStatus, 
               u.FirstName + ' ' + u.LastName AS UploadedBy
        FROM Documents doc
             JOIN User_Document_Mapping udm ON udm.DocumentId = doc.Id
             JOIN AspNetUsers u ON u.Id = doc.CreatedBy
        WHERE doc.CreatedBy = @UserId
              OR udm.SignerId = @UserId
        ORDER BY doc.CreationTime DESC;
    END;
GO

/****** Object:  StoredProcedure [dbo].[GetDocumentShapes]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDocumentShapes] 

@UserId     UNIQUEIDENTIFIER, 
@DocumentId UNIQUEIDENTIFIER

AS
    BEGIN        
        SET NOCOUNT OFF;
        SELECT Shapes.DocumentId, 
               Shapes.SignerId, 
               Shapes.X, 
               Shapes.Y
        FROM Shapes
        WHERE DocumentId = @DocumentId
              AND SignerId = @UserId;
    END;
GO

/****** Object:  StoredProcedure [dbo].[GetDocumentSigners]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDocumentSigners]
@DocumentId uniqueidentifier	
AS
BEGIN
	
	SET NOCOUNT OFF;

SELECT UDM.DocumentId, 
       ANU.Id as UserId, 
	   ANU.FirstName,
	   ANU.LastName,
       ANU.Email, 
       ANU.WalletAddress
FROM User_Document_Mapping UDM
     JOIN AspNetUsers ANU ON UDM.SignerId = ANU.Id
WHERE UDM.DocumentId = @DocumentId;

END
GO

/****** Object:  StoredProcedure [dbo].[GetUserByEmail]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUserByEmail] 

@Email VARCHAR (256)

AS
     BEGIN

         SET NOCOUNT ON;
    
         SELECT anu.Id as UserId,
                anu.Email,
                anu.UserName,
                anu.FirstName,
                anu.LastName,
                anu.PasswordHash,
                anu.WalletAddress,
				anu.PasswordHash as [Password]
         FROM AspNetUsers anu
         WHERE anu.Email = @Email;
     END;
GO

/****** Object:  StoredProcedure [dbo].[RegisterUser]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RegisterUser] 

@FirstName     VARCHAR (100),
@LastName      VARCHAR (100),
@Email         VARCHAR (256),
@Password      VARCHAR (max),
@WalletAddress VARCHAR (100)

AS
     BEGIN	
         SET NOCOUNT OFF;

         DECLARE 
		 @NewUserId UNIQUEIDENTIFIER= NEWID(),
		 @RowCount Int

		 If Not Exists(select Id from AspNetUsers where Email = @Email)
		 BEGIN  
			INSERT INTO AspNetUsers (Id, Firstname, Lastname, Email, EmailConfirmed, PasswordHash, WalletAddress, TwoFactorEnabled )
			VALUES (@NewUserId, @FirstName, @LastName, @Email, 1, @Password, @WalletAddress, 0);

			set @RowCount = @@ROWCOUNT

         IF(@RowCount > 0)
             BEGIN
                 INSERT INTO AspNetUserRoles
				 (
					UserId,
					RoleId
				 ) VALUES(
					@NewUserId,
					2
				);
             END;
		END
		ELSE
		BEGIN
			Update AspNetUsers set EmailConfirmed = 1, PasswordHash = @Password where Email = @Email
		END	 				 
		select @RowCount     
     END;
GO

/****** Object:  StoredProcedure [dbo].[SearchDocument]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SearchDocument] 

@SignedDocHash VARCHAR(MAX)

AS
     BEGIN	
         SET NOCOUNT OFF;

         SELECT doc.Id,
                doc.DocumentName,     
                doc.FileName,                
                doc.DocumentHash,
                doc.SignedDocumentHash,
                doc.CreationTime,
                doc.CreatedBy,
                doc.DocumentStatus,
                doc.CreationTxHash
         FROM Documents doc
         WHERE SignedDocumentHash = @SignedDocHash;

     END;
GO

/****** Object:  StoredProcedure [dbo].[SignDocument]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SignDocument]
@documentId    UNIQUEIDENTIFIER, 
@userId        UNIQUEIDENTIFIER, 
@SignTxHash    VARCHAR(100), 
@SignedDocHash VARCHAR(MAX)     = NULL,
@SignedDocData VARBINARY(MAX)   = NULL,
@BlockNumber   BIGINT           = NULL                                                                                                                									  
                                     
AS
    BEGIN
        SET NOCOUNT OFF;
        DECLARE @SignerCount INT, @SignedCount INT;

        IF EXISTS
        (
            SELECT Id
            FROM User_Document_Mapping
            WHERE DocumentId = @documentId
                  AND SignerId = @userId
        )
            BEGIN
                UPDATE User_Document_Mapping
                  SET 
                      TransactionHash = @SignTxHash, 
                      Signed = 1,
					  BlockNumber = @BlockNumber
                WHERE DocumentId = @documentId
                      AND SignerId = @userId;

                SET @SignerCount =
                (
                    SELECT COUNT(Id)
                    FROM User_Document_Mapping
                    WHERE DocumentId = @documentId
                );

                SET @SignedCount =
                (
                    SELECT COUNT(Udm1.Signed)
                    FROM User_Document_Mapping UDM1
                    WHERE UDM1.Signed = 1
                          AND UDM1.DocumentId = @documentId
                );

				-- update document status to signed if all signers have signed the document.
                IF(@SignerCount = @SignedCount)
                    BEGIN
                        UPDATE Documents
                          SET 
                              DocumentStatus = 1
                        WHERE Id = @documentId;
                END;

				If(@SignTxHash IS NOT NULL)
				BEGIN 
					Update Documents 
					set 
						SignedDocumentHash = @SignedDocHash, 
						SignedDocumentData = @SignedDocData, 
						FinalSignTxHash = @SignTxHash, 
						BlockNumber = @BlockNumber 
					Where 
						Id = @documentId;
				END				
        END;
    END;
GO

/****** Object:  StoredProcedure [dbo].[UpdateDocCreationHash]    Script Date: 15-Dec-19 9:31:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateDocCreationHash]  
                                              
@DocumentId   UNIQUEIDENTIFIER,
@CreationHash VARCHAR(500)

AS
    BEGIN
        --
        SET NOCOUNT OFF;
        UPDATE Documents
          SET 
              CreationTxHash = @CreationHash
        WHERE Id = @DocumentId;
    END;
GO

/** Insert Default Roles Into [AspNetRoles] Table **/

INSERT INTO [dbo].[AspNetRoles]
           ([Id]
           ,[Name])
     VALUES
           (1, 'Admin'), (2, 'Member');

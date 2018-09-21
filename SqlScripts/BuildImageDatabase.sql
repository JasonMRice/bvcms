SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO
PRINT N'Creating [dbo].[Image]'
GO
CREATE TABLE [dbo].[Image]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[bits] [varbinary] (max) NULL,
[length] [int] NULL,
[mimetype] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[secure] [bit] NULL
)

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating primary key [PK_ImageData] on [dbo].[Image]'
GO
ALTER TABLE [dbo].[Image] ADD CONSTRAINT [PK_ImageData] PRIMARY KEY CLUSTERED ([Id])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
PRINT N'Creating [dbo].[Other]'
GO
CREATE TABLE [dbo].[Other]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[created] [datetime] NOT NULL CONSTRAINT [DF_Extra_created] DEFAULT (getdate()),
[userID] [int] NOT NULL CONSTRAINT [DF_Other_userID] DEFAULT ((0)),
[first] [varbinary] (max) NULL,
[second] [varbinary] (max) NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_Extra] on [dbo].[Other]'
GO
ALTER TABLE [dbo].[Other] ADD CONSTRAINT [PK_Extra] PRIMARY KEY CLUSTERED  ([id])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'The database update succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The database update failed'
GO
DROP TABLE #tmpErrors
GO



CREATE TABLE [dbo].[Content]
(
[ContentID] [int] NOT NULL IDENTITY(1, 1),
[Title] [nvarchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ContentName] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Body] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[CreatedOn] [datetime] NULL CONSTRAINT [DF_Content_CreatedOn] DEFAULT (getdate()),
[CreatedById] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Content] ADD CONSTRAINT [PK_Content] PRIMARY KEY CLUSTERED  ([ContentID]) ON [PRIMARY]
GO

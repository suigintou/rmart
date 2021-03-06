USE [rmart]
GO
/****** Object:  Table [dbo].[Content]    Script Date: 28.06.2013 15:47:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Content](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CultureID] [int] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ContentPages] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Discussions]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Discussions](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ParentType] [int] NOT NULL,
	[ParentID] [int] NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatorIP] [varbinary](16) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_Discussions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Favorites]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Favorites](
	[PictureID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[Date] [datetime] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[History]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[History](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TargetType] [tinyint] NOT NULL,
	[TargetID] [int] NOT NULL,
	[Field] [tinyint] NOT NULL,
	[OldValue] [nvarchar](200) NULL,
	[NewValue] [nvarchar](200) NULL,
	[Date] [datetime] NOT NULL,
	[UserID] [int] NULL,
	[UserIP] [varbinary](16) NOT NULL,
	[Comment] [nvarchar](200) NULL,
	[NextEventID] [int] NULL,
 CONSTRAINT [PK_History] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Pictures]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Pictures](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FileHash] [binary](16) NOT NULL,
	[BitmapHash] [binary](16) NOT NULL,
	[Format] [tinyint] NOT NULL,
	[Width] [int] NOT NULL,
	[Height] [int] NOT NULL,
	[Resolution]  AS ([Width]*[Height]) PERSISTED,
	[FileSize] [int] NOT NULL,
	[CreatorID] [int] NULL,
	[CreatorIP] [varbinary](16) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[Score] [int] NOT NULL,
	[RatesCount] [int] NOT NULL,
	[Rating] [tinyint] NOT NULL,
	[RequiresTagging] [bit] NOT NULL,
	[CachedTagIDs] [varchar](max) NOT NULL,
	[Source] [nvarchar](2048) NULL,
 CONSTRAINT [PK_Pictures] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PictureTagRelations]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PictureTagRelations](
	[PictureID] [int] NOT NULL,
	[TagID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Rates]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Rates](
	[PictureID] [int] NOT NULL,
	[Score] [tinyint] NOT NULL,
	[UserID] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[IPAddress] [varbinary](16) NOT NULL,
	[IsActive] [bit] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Reports]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Reports](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TargetType] [tinyint] NULL,
	[TargetID] [int] NULL,
	[ReportType] [tinyint] NOT NULL,
	[Message] [nvarchar](2000) NULL,
	[CreatedBy] [int] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[CreatorIP] [varbinary](16) NOT NULL,
	[IsResolved] [bit] NOT NULL,
	[ResolvedBy] [int] NULL,
	[ResolvedAt] [datetime] NULL,
	[ResolverIP] [varbinary](16) NULL,
 CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SimilarPictures]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SimilarPictures](
	[PictureID] [int] NOT NULL,
	[SimilarPictureID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TagAliases]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TagAliases](
	[TagID] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TagRelations]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TagRelations](
	[ParentID] [int] NOT NULL,
	[ChildrenID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tags]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tags](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Type] [tinyint] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CreatorID] [int] NULL,
	[CreatorIP] [varbinary](16) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[UsageCount] [int] NOT NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tickets]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tickets](
	[UserID] [int] NOT NULL,
	[Type] [tinyint] NOT NULL,
	[Token] [char](32) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[RequestIP] [varbinary](16) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 28.06.2013 15:47:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Login] [nvarchar](30) NOT NULL,
	[PasswordHash] [binary](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](320) NOT NULL,
	[Role] [tinyint] NOT NULL,
	[IsEmailConfirmed] [bit] NOT NULL,
	[IsPrivateProfile] [bit] NOT NULL,
	[RegistrationDate] [datetime] NOT NULL,
	[RegistrationIP] [varbinary](16) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IX_Favorites_UserID_PictureID]    Script Date: 28.06.2013 15:47:10 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_Favorites_UserID_PictureID] ON [dbo].[Favorites]
(
	[UserID] ASC,
	[PictureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TagPictureRelations_PictureID_TagID]    Script Date: 28.06.2013 15:47:10 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_TagPictureRelations_PictureID_TagID] ON [dbo].[PictureTagRelations]
(
	[PictureID] ASC,
	[TagID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Rates_UserID_PictureID]    Script Date: 28.06.2013 15:47:10 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_Rates_UserID_PictureID] ON [dbo].[Rates]
(
	[PictureID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TagsRelations_ParentID_ChildrenID]    Script Date: 28.06.2013 15:47:10 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_TagsRelations_ParentID_ChildrenID] ON [dbo].[TagRelations]
(
	[ParentID] ASC,
	[ChildrenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Tickets_Token]    Script Date: 28.06.2013 15:47:10 ******/
CREATE UNIQUE CLUSTERED INDEX [IX_Tickets_Token] ON [dbo].[Tickets]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Content_Name_CultureID]    Script Date: 28.06.2013 15:47:10 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Content_Name_CultureID] ON [dbo].[Content]
(
	[Name] ASC,
	[CultureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Discussions_CreatedAtDesc]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Discussions_CreatedAtDesc] ON [dbo].[Discussions]
(
	[CreatedAt] DESC
)
INCLUDE ( 	[Status]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Discussions_Parent]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Discussions_Parent] ON [dbo].[Discussions]
(
	[ParentType] ASC,
	[ParentID] ASC,
	[CreatedAt] ASC
)
INCLUDE ( 	[Status]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Favorites_PictureID]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Favorites_PictureID] ON [dbo].[Favorites]
(
	[PictureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_History_DateDesc]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_History_DateDesc] ON [dbo].[History]
(
	[Date] DESC,
	[ID] DESC
)
INCLUDE ( 	[TargetType],
	[TargetID],
	[UserID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_History_Target]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_History_Target] ON [dbo].[History]
(
	[TargetType] ASC,
	[TargetID] ASC,
	[Date] DESC,
	[ID] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Pictures_Date]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Pictures_Date] ON [dbo].[Pictures]
(
	[CreationDate] DESC,
	[ID] DESC
)
INCLUDE ( 	[Status],
	[Rating]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Pictures_Hash]    Script Date: 28.06.2013 15:47:10 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Pictures_Hash] ON [dbo].[Pictures]
(
	[BitmapHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
/****** Object:  Index [IX_Pictures_Resolution]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Pictures_Resolution] ON [dbo].[Pictures]
(
	[Resolution] DESC,
	[CreationDate] DESC,
	[ID] DESC
)
INCLUDE ( 	[Status],
	[Rating]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Pictures_Score]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Pictures_Score] ON [dbo].[Pictures]
(
	[Score] DESC,
	[RatesCount] DESC,
	[CreationDate] DESC,
	[ID] DESC
)
INCLUDE ( 	[Status],
	[Rating]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TagPictureRelations_TagID]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_TagPictureRelations_TagID] ON [dbo].[PictureTagRelations]
(
	[TagID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Rates_Date]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Rates_Date] ON [dbo].[Rates]
(
	[Date] DESC
)
INCLUDE ( 	[IsActive]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Rates_PictureID]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Rates_PictureID] ON [dbo].[Rates]
(
	[PictureID] ASC
)
INCLUDE ( 	[IsActive]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reports_CreatedAt]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Reports_CreatedAt] ON [dbo].[Reports]
(
	[CreatedAt] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SimilarPictures_PictureID]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_SimilarPictures_PictureID] ON [dbo].[SimilarPictures]
(
	[PictureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_TagAliases_Name]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_TagAliases_Name] ON [dbo].[TagAliases]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_TagAliases_TagID_Name]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_TagAliases_TagID_Name] ON [dbo].[TagAliases]
(
	[TagID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_TagRelations_ChildrenID]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_TagRelations_ChildrenID] ON [dbo].[TagRelations]
(
	[ChildrenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Tags_Count]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Tags_Count] ON [dbo].[Tags]
(
	[UsageCount] DESC
)
INCLUDE ( 	[Name],
	[Type],
	[Status]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Tags_Date]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Tags_Date] ON [dbo].[Tags]
(
	[CreationDate] DESC
)
INCLUDE ( 	[Name],
	[Type],
	[Status]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Tags_Name]    Script Date: 28.06.2013 15:47:10 ******/
CREATE NONCLUSTERED INDEX [IX_Tags_Name] ON [dbo].[Tags]
(
	[Name] ASC
)
INCLUDE ( 	[Type],
	[Status]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Users_Email]    Script Date: 28.06.2013 15:47:10 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Users_Login]    Script Date: 28.06.2013 15:47:10 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Login] ON [dbo].[Users]
(
	[Login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Pictures] ADD  CONSTRAINT [DF_Pictures_CachedTagIDs]  DEFAULT ('') FOR [CachedTagIDs]
GO
ALTER TABLE [dbo].[Discussions]  WITH CHECK ADD  CONSTRAINT [FK_Discussions_Users_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Discussions] CHECK CONSTRAINT [FK_Discussions_Users_CreatedBy]
GO
ALTER TABLE [dbo].[Favorites]  WITH CHECK ADD  CONSTRAINT [FK_Favorites_Pictures_PictureID] FOREIGN KEY([PictureID])
REFERENCES [dbo].[Pictures] ([ID])
GO
ALTER TABLE [dbo].[Favorites] CHECK CONSTRAINT [FK_Favorites_Pictures_PictureID]
GO
ALTER TABLE [dbo].[Favorites]  WITH CHECK ADD  CONSTRAINT [FK_Favorites_Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Favorites] CHECK CONSTRAINT [FK_Favorites_Users_UserID]
GO
ALTER TABLE [dbo].[History]  WITH CHECK ADD  CONSTRAINT [FK_History_Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[History] CHECK CONSTRAINT [FK_History_Users_UserID]
GO
ALTER TABLE [dbo].[Pictures]  WITH CHECK ADD  CONSTRAINT [FK_Pictures_Users_CreatorID] FOREIGN KEY([CreatorID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Pictures] CHECK CONSTRAINT [FK_Pictures_Users_CreatorID]
GO
ALTER TABLE [dbo].[PictureTagRelations]  WITH CHECK ADD  CONSTRAINT [FK_TagPictureRelations_Pictures_PictureID] FOREIGN KEY([PictureID])
REFERENCES [dbo].[Pictures] ([ID])
GO
ALTER TABLE [dbo].[PictureTagRelations] CHECK CONSTRAINT [FK_TagPictureRelations_Pictures_PictureID]
GO
ALTER TABLE [dbo].[PictureTagRelations]  WITH CHECK ADD  CONSTRAINT [FK_TagPictureRelations_Tags_TagID] FOREIGN KEY([TagID])
REFERENCES [dbo].[Tags] ([ID])
GO
ALTER TABLE [dbo].[PictureTagRelations] CHECK CONSTRAINT [FK_TagPictureRelations_Tags_TagID]
GO
ALTER TABLE [dbo].[Rates]  WITH CHECK ADD  CONSTRAINT [FK_Rates_Pictures_PictureID] FOREIGN KEY([PictureID])
REFERENCES [dbo].[Pictures] ([ID])
GO
ALTER TABLE [dbo].[Rates] CHECK CONSTRAINT [FK_Rates_Pictures_PictureID]
GO
ALTER TABLE [dbo].[Rates]  WITH CHECK ADD  CONSTRAINT [FK_Rates_Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Rates] CHECK CONSTRAINT [FK_Rates_Users_UserID]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_Users_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_Users_CreatedBy]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_Users_ResolvedBy] FOREIGN KEY([ResolvedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_Users_ResolvedBy]
GO
ALTER TABLE [dbo].[SimilarPictures]  WITH CHECK ADD  CONSTRAINT [FK_SimilarPictures_Pictures_PictureID] FOREIGN KEY([PictureID])
REFERENCES [dbo].[Pictures] ([ID])
GO
ALTER TABLE [dbo].[SimilarPictures] CHECK CONSTRAINT [FK_SimilarPictures_Pictures_PictureID]
GO
ALTER TABLE [dbo].[SimilarPictures]  WITH CHECK ADD  CONSTRAINT [FK_SimilarPictures_Pictures_SimilarPictureID] FOREIGN KEY([SimilarPictureID])
REFERENCES [dbo].[Pictures] ([ID])
GO
ALTER TABLE [dbo].[SimilarPictures] CHECK CONSTRAINT [FK_SimilarPictures_Pictures_SimilarPictureID]
GO
ALTER TABLE [dbo].[TagAliases]  WITH CHECK ADD  CONSTRAINT [FK_TagAliases_Tags] FOREIGN KEY([TagID])
REFERENCES [dbo].[Tags] ([ID])
GO
ALTER TABLE [dbo].[TagAliases] CHECK CONSTRAINT [FK_TagAliases_Tags]
GO
ALTER TABLE [dbo].[TagRelations]  WITH CHECK ADD  CONSTRAINT [FK_TagRelations_Tags_ChildrenID] FOREIGN KEY([ChildrenID])
REFERENCES [dbo].[Tags] ([ID])
GO
ALTER TABLE [dbo].[TagRelations] CHECK CONSTRAINT [FK_TagRelations_Tags_ChildrenID]
GO
ALTER TABLE [dbo].[TagRelations]  WITH CHECK ADD  CONSTRAINT [FK_TagRelations_Tags_ParentID] FOREIGN KEY([ParentID])
REFERENCES [dbo].[Tags] ([ID])
GO
ALTER TABLE [dbo].[TagRelations] CHECK CONSTRAINT [FK_TagRelations_Tags_ParentID]
GO
ALTER TABLE [dbo].[Tickets]  WITH CHECK ADD  CONSTRAINT [FK_Tickets_Users_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Tickets] CHECK CONSTRAINT [FK_Tickets_Users_UserID]
GO

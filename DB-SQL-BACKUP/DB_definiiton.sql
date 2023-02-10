CREATE DATABASE Atletika.

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
    [id] [int] NULL,
    [userName] [nvarchar](20) NULL,
    [userPassword] [nvarchar](20) NULL,
    [admin] [int] NULL
    ) ON [PRIMARY]
    GO
    
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
    [id] [int] NULL,
    [userName] [nvarchar](20) NULL,
    [userPassword] [nvarchar](20) NULL,
    [admin] [int] NULL
    ) ON [PRIMARY]
    GO

    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE TABLE [dbo].[Trenink](
    [id] [int] NOT NULL,
    [definition] [nvarchar](4000) NULL
    ) ON [PRIMARY]
    GO
    
    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE TABLE [dbo].[Trenink_user_response](
    [id] [int] NULL,
    [definition] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    GO

    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE TABLE [dbo].[Asociace_Treninku](
    [id] [int] NULL,
    [user_id] [int] NULL,
    [trenink_id] [int] NULL,
    [date] [nvarchar](10) NULL,
    [response_id] [int] NULL
    ) ON [PRIMARY]
    GO
    
    SET ANSI_NULLS ON
    GO
    SET QUOTED_IDENTIFIER ON
    GO
CREATE TABLE [dbo].[Asociace_Trener_Uzivatel](
    [id] [int] NULL,
    [trener_id] [int] NULL,
    [user_id] [int] NULL
) ON [PRIMARY]
    GO


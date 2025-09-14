USE [order_management]
GO

/****** Object:  Table [dbo].[client]    Script Date: 11/09/2025 15:24:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[client](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NOT NULL UNIQUE,
	[email] [nvarchar](255) NOT NULL UNIQUE,
	[phone] [nvarchar](50) NOT NULL ,
	[created_at] [datetime] NOT NULL,
 CONSTRAINT [PK_client] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[client] ADD  CONSTRAINT [DF_client_created_at]  DEFAULT (getdate()) FOR [created_at]
GO



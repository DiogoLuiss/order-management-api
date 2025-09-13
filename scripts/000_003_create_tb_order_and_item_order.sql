USE [order_management]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[order_status](
    [id] SMALLINT NOT NULL,
    [description] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_order_status] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

INSERT INTO [dbo].[order_status] (id, description) VALUES
(1, 'Novo'),
(2, 'Processamento'),
(3, 'Finalizado')
GO


/****** Object:  Table [dbo].[order] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[order](
    [id] INT IDENTITY(1,1) NOT NULL,
    [client_id] INT NOT NULL,
    [order_date] DATETIME NOT NULL DEFAULT GETDATE(),
    [total_value] DECIMAL(10,2) NOT NULL DEFAULT 0,
    [status_id] SMALLINT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_order] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_order_client] FOREIGN KEY ([client_id]) REFERENCES [dbo].[client]([id]),
    CONSTRAINT [FK_order_status] FOREIGN KEY ([status_id]) REFERENCES [dbo].[order_status]([id])
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[item_order] ******/
CREATE TABLE [dbo].[item_order](
    [id] INT IDENTITY(1,1) NOT NULL,
    [order_id] INT NOT NULL,
    [product_id] INT NOT NULL,
    [quantity] INT NOT NULL,
    [unit_price] DECIMAL(10,2) NOT NULL,
    CONSTRAINT [PK_item_order] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_item_order_order] FOREIGN KEY ([order_id]) REFERENCES [dbo].[order]([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_item_order_product] FOREIGN KEY ([product_id]) REFERENCES [dbo].[product]([id])
) ON [PRIMARY]
GO

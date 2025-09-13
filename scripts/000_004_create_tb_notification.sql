CREATE TABLE [dbo].[notification](
    [id] INT IDENTITY(1,1) NOT NULL,
    [order_id] INT NOT NULL,
    [message] NVARCHAR(255) NOT NULL,
    [created_at] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_notification] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_notification_order] FOREIGN KEY ([order_id]) REFERENCES [dbo].[order]([id])
) ON [PRIMARY]

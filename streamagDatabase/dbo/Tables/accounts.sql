CREATE TABLE [dbo].[accounts] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [username]     NCHAR (10)       NULL,
    [password]     NCHAR (50)       NULL,
    [isbanned]     BIT              NULL,
    [created]      DATETIME         NOT NULL,
    [subscription] DATETIME         NULL,
    [role]         INT              NULL,
    [email]        VARCHAR (50)     NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);




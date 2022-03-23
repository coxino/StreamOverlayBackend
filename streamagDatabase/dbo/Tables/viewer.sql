CREATE TABLE [dbo].[viewer] (
    [Id]           VARCHAR (50) NOT NULL,
    [username]     VARCHAR (50) NULL,
    [inventory]    INT          NULL,
    [email]        VARCHAR (50) NULL,
    [ipadress]     VARCHAR (50) NULL,
    [creationTime] DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


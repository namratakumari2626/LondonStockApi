IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Stocks] (
    [Id] bigint NOT NULL IDENTITY,
    [TickerSymbol] nvarchar(10) NOT NULL,
    [AveragePrice] decimal(18,4) NOT NULL,
    [TotalVolume] decimal(18,4) NOT NULL,
    [LastUpdated] datetime2 NOT NULL,
    CONSTRAINT [PK_Stocks] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Trades] (
    [Id] bigint NOT NULL IDENTITY,
    [StockId] bigint NOT NULL,
    [Price] decimal(18,4) NOT NULL,
    [Quantity] decimal(18,4) NOT NULL,
    [BrokerId] nvarchar(50) NOT NULL,
    [TradedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Trades] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Trades_Stocks] FOREIGN KEY ([StockId]) REFERENCES [Stocks] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Stocks_TickerSymbol] ON [Stocks] ([TickerSymbol]);
GO

CREATE INDEX [IX_Trades_StockId] ON [Trades] ([StockId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251229155313_InitialCreate', N'8.0.22');
GO

COMMIT;
GO


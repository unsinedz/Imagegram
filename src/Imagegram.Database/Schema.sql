create table [dbo].[Accounts] (
    [Id] uniqueidentifier not null,
    [Name] nvarchar(100) not null,
    primary key ([Id])
);

create table [dbo].[Posts] (
    [Id] uniqueidentifier not null,
    [ImageUrl] varchar(100) not null,
    [CreatorId] uniqueidentifier not null,
    [CreatedAt] datetime2 not null,
    [ItemCursor] rowversion not null,
	[CommentsCount] int not null default 0,
    primary key ([Id]),
    foreign key ([CreatorId]) references [dbo].[Accounts]([Id]) on delete cascade,
	index [IX_Posts_CreatorId] nonclustered ([CreatorId]),
);

create table [dbo].[Comments] (
    [Id] uniqueidentifier not null,
    [Content] nvarchar(500) not null,
    [PostId] uniqueidentifier not null,
    [CreatorId] uniqueidentifier not null,
    [CreatedAt] datetime2 not null,
    [ItemCursor] rowversion not null,
    primary key ([Id]),
    foreign key ([CreatorId]) references [dbo].[Accounts]([Id]) on delete cascade,
	index [IX_Comments_PostId] nonclustered ([PostId]),
    index [IX_Comments_CreatorId] nonclustered ([CreatorId])
);
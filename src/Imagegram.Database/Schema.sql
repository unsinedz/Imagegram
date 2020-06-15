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

go;

create procedure [dbo].[spDeleteAccount]
    @accountId uniqueidentifier not null
as
begin
    begin transaction DeleteAccount;

    -- update Posts metadata
    with [PostDecreaseCommentsCount] as (
        select p.[Id] as [PostId], count(c.[Id]) as [CommentsCount]
        from [dbo].[Posts] p
        inner join [dbo].[Comments] c on p.[Id] = c.[PostId]
        where c.[CreatorId] = @accountId and p.[CreatorId] <> @accountId
        group by p.[Id]
    )
    update p
    set [CommentsCount] = p.[CommentsCount] - pdcc.[CommentsCount]
    from [dbo].[Posts] p
    inner join [PostDecreaseCommentsCount] pdcc on p.[Id] = pdcc.[PostId];

    -- delete account
    delete from [dbo].[Accounts]
    where [Id] = @accountId;

    commit transaction DeleteAccount;
end
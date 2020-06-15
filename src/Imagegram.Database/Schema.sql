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
    foreign key ([CreatorId]) references [dbo].[Accounts]([Id]) on delete cascade
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
	index [IX_Comments_PostId nonclustered] ([PostId])
);

go

create type [dbo].[udtIds] as Table(Id uniqueidentifier not null);

go

create procedure [dbo].[spSelectLastPostComments]
    @commentLimit int,
	@postIds [dbo].[udtIds] readonly
as
begin
    select [PostId]
    	,[Id]
    	,[Content]
        ,[CreatorId]
    	,[CreatedAt]
		,[ItemCursor]
    from (
    	select p.[Id] as [PostId]
    		,c.[Id] as [Id]
    		,c.[Content] as [Content]
    	    ,c.[CreatorId] as [CreatorId]
    		,c.[CreatedAt] as [CreatedAt]
			,c.[ItemCursor]
    		,row_number() over (partition by p.[Id] order by c.[CreatedAt] desc) as [CommentRank]
    	from @postIds p
    	inner join [dbo].[Comments] c on p.[Id] = c.[PostId]
    	) ranks
    where [CommentRank] <= @commentLimit
end;

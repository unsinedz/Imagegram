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
    [VersionCursor] rowversion not null,
    primary key ([Id]),
    foreign key ([CreatorId]) references [dbo].[Accounts]([Id]) on delete cascade
);

create table [dbo].[Comments] (
    [Id] uniqueidentifier not null,
    [Content] nvarchar(500) not null,
    [PostId] uniqueidentifier not null,
    [CreatorId] uniqueidentifier not null,
    [CreatedAt] datetime2 not null,
    [VersionCursor] rowversion not null,
    primary key ([Id]),
    foreign key ([CreatorId]) references [dbo].[Accounts]([Id]) on delete cascade
);

create type [dbo].[udtIds] as Table(Id uniqueidentifier not null);

create procedure [dbo].[spSelectLatestPosts]
	@limit int = null,
	@previousPostCursor rowversion = null
as
begin
	declare @where nvarchar(100) = ''
	if @previousPostCursor is not null
		set @where = ' where p.[VersionCursor] > ' + convert(nvarchar(30), convert(binary(8), @previousPostCursor), 1)

	declare @limitClause nvarchar(20) = ''
	if @limit > 0
		set @limitClause = ' top (' + cast(@limit as nvarchar(10)) + ')'

	declare @sql nvarchar(1000)
	set @sql = 'select' + @limitClause + ' p.[Id]
			,p.[ImageUrl]
			,p.[CreatorId]
			,p.[CreatedAt]
			,CONVERT(bigint, p.[VersionCursor]) as [VersionCursor]
		from [dbo].[Posts] p
		left join [dbo].[Comments] c on c.[PostId]= p.[Id]'
		+ @where +
		' group by p.[Id]
			,p.[ImageUrl]
			,p.[CreatorId]
			,p.[CreatedAt]
			,p.[VersionCursor]
		order by count(c.[Id]) desc';

	exec sp_executesql @sql
end

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
		,[VersionCursor]
    from (
    select p.[Id] as [PostId]
    	,c.[Id] as [Id]
    	,c.[Content] as [Content]
        ,c.[CreatorId] as [CreatorId]
    	,c.[CreatedAt] as [CreatedAt]
		,c.[VersionCursor]
    	,row_number() over (partition by p.[Id] order by c.[CreatedAt] desc) as [CommentRank]
    from @postIds p
    inner join [dbo].[Comments] c on p.[Id] = c.[PostId]
    ) ranks
    where [CommentRank] <= @commentLimit
end;

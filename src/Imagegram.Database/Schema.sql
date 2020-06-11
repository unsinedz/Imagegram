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
    primary key ([Id]),
    foreign key ([CreatorId]) references [dbo].[Accounts]([Id]) on delete cascade
);

create table [dbo].[Comments] (
    [Id] uniqueidentifier not null,
    [Content] nvarchar(500) not null,
    [PostId] uniqueidentifier not null,
    [CreatorId] uniqueidentifier not null,
    [CreatedAt] datetime2 not null,
    primary key ([Id]),
    foreign key ([CreatorId]) references [dbo].[Accounts]([Id]) on delete cascade
);

create procedure spSelectPostsWithLastComments
    @lastCount int
as
begin
    select PostId
    	,ImageUrl
    	,CreatorId
    	,CreatedAt
    	,CommentId
    	,CommentContent
    	,CommentCreatedAt
    from (
    select p.Id as PostId
    	,p.ImageUrl
    	,p.CreatorId
    	,p.CreatedAt
    	,c.Id as CommentId
    	,c.Content as CommentContent
    	,c.CreatedAt as CommentCreatedAt
    	,row_number() over (partition by p.Id order by count(c.Id), c.CreatedAt desc) as CommentRank
    from Posts p
    left join Comments c on p.Id = c.PostId
    group by p.Id, p.ImageUrl, p.CreatorId, p.CreatedAt, c.Id, c.Content, c.CreatedAt
    ) ranks
    where CommentRank <= @lastCount
end;
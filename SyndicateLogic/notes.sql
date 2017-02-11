use Team
select top 1000 * from app.Logs order by id desc -- last logs
select * from rss.feeds where id not in (select feedid from rss.Articles group by feedid ) -- feeds with no articles
select top 20 feedid, count(1) c from rss.Articles group by feedid order by c desc -- feeds with most articles
select top 100 len(summary) l, * from rss.articles order by l desc -- longest articles
select * from rss.feeds where active = 0 and id in (select feedid from rss.Articles group by feedid )

select top 100 * from rss.shingles order by id desc
select top 100 summary from rss.Articles where FeedID = 298 order by id desc
--truncate table app.Logs

select * from rss.feeds where Active = 0
-- Tools -> Nuget Packet Manager -> Packet Manager Console
-- Enable-Migrations
-- Add-Migration first2tables
-- Update-Database

select top 100 cast(rss20 as xml).value(N'(/item/title)[1]', N'nvarchar(MAX)') as Title,
cast(rss20 as xml).value(N'(/item/pubDate)[1]', N'nvarchar(MAX)') as PubDate,
cast(rss20 as xml).value(N'(/item/description)[1]', N'nvarchar(MAX)') as description
from Articles

--delete duplicates
select id, title, summary, row_number() over (partition by title, summary order by id) as itemNr into #dup from rss.articles 
select * from #dup where itemNr>1 order by title
delete from rss.Articles where id in (select id from #dup where itemNR > 1)
drop table #dup

select id, ArticleID, InstrumentID, row_number() over (partition by  ArticleID, InstrumentID order by id) as itemNR
into #dup from rss.ArticleRelations order by ArticleID
delete from rss.ArticleRelations where id in (select id from #dup where itemNR > 1)
select * from rss.ArticleRelations
drop table #dup

-- space occupied by tables
SELECT 
 t.NAME AS TableName,
 i.name AS indexName,
 SUM(p.rows) AS RowCounts,
 SUM(a.total_pages) AS TotalPages, 
 SUM(a.used_pages) AS UsedPages, 
 SUM(a.data_pages) AS DataPages,
 (SUM(a.total_pages) * 8) / 1024 AS TotalSpaceMB, 
 (SUM(a.used_pages) * 8) / 1024 AS UsedSpaceMB, 
 (SUM(a.data_pages) * 8) / 1024 AS DataSpaceMB
FROM 
 sys.tables t
INNER JOIN  
 sys.indexes i ON t.OBJECT_ID = i.object_id
INNER JOIN 
 sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
INNER JOIN 
 sys.allocation_units a ON p.partition_id = a.container_id
WHERE 
 t.NAME NOT LIKE 'dt%' AND
 i.OBJECT_ID > 255 AND  
 i.index_id <= 1
GROUP BY 
 t.NAME, i.object_id, i.index_id, i.name 
ORDER BY 
-- OBJECT_NAME(i.object_id) 
TotalSpaceMB desc




-- import csv
BULK INSERT app.indexComponents
FROM 'd:\install\indexComponents.csv'
WITH
(
    FIRSTROW = 1,
    FIELDTERMINATOR = ',',  --CSV field delimiter
    ROWTERMINATOR = '\n',   --Use to shift the control to next row
    TABLOCK
)

BULK INSERT app.currencies
FROM 'd:\curr4.csv'
WITH
(
    FIRSTROW = 1,
    FIELDTERMINATOR = ';',  --CSV field delimiter
    TABLOCK)

-- calculate price action
declare @Interval tinyint
set @Interval = 30
declare @date date
set @date = '2016-02-09'
declare @Ticker char(8)
set @Ticker = 'TSLA'

select @Ticker, @date, @Interval,
(min(adj_low) / (select adj_close from int.prices where ticker = @Ticker and date = @date)) low,
(max(adj_high) /(select adj_close from int.prices where ticker = @Ticker and date = @date)) high
from int.Prices where ticker = 'TSLA' and date > dateadd(d, 1, @date) and date < dateadd(d, @Interval, @date)

select * from int.Prices where ticker = @Ticker and date > @date and date < dateadd(d, @Interval, @date)
select top 100 * from fact.shingleAction where interval = 3 and down is not null

-- important shingles
use team
select *, (down + up -2) * 100 diff
from fact.shingleAction sa
join rss.Shingles s on s.id = sa.shingleid
where interval = 2 and down is not null and samples > 10
order by down + up desc

-- kinds of shingles
select kind, count(1)
from rss.shingles
group by kind
order by kind
 
select count (1) from fact.shingleAction where interval = 2
select * from rss.Shingles where kind = 7 and LastRecomputeDate is null

select * from rss.Shingles where kind = 4

--ALTER TABLE int.Companies ALTER COLUMN ticker  
--varchar(5)COLLATE Latin1_General_CI_AS NOT NULL; 
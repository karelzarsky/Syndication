--select COUNT (1) from rss.articles

--select COUNT (1) from rss.articles where Ticker is not null

--select COUNT (1) from rss.articles a
--where a.Ticker is not null and language = 'en'

--select COUNT (DISTINCT a.ID) from rss.articles a
--join int.prices p on a.Ticker = p.ticker
--where a.Ticker is not null and p.ticker is not null and language = 'en'

update rss.articles
set UseForML = 1
from int.prices
where int.prices.ticker = rss.articles.Ticker
AND rss.articles.Ticker is not null
AND int.prices.ticker is not null
AND rss.articles.language = 'en'
AND rss.articles.UseForML = 0

select * from rss.shingles where ID in
(
select s.ID from rss.shingles s
join rss.shingleUse su on s.ID = su.ShingleID
join rss.articles a on a.ID = su.ArticleID
where s.kind in (1,2)
and s.language = 'en'
and a.UseForML = 1
-- and s.UseForML = 0
and s.tokens = 1
GROUP BY s.ID
HAVING COUNT(1)>200
)

update rss.shingles
set UseForML = 0
where UseForML = 1

update rss.shingles
set UseForML = 1
where ID in
(
select s.ID from rss.shingles s
join rss.shingleUse su on s.ID = su.ShingleID
join rss.articles a on a.ID = su.ArticleID
where s.kind in (1,2)
and s.language = 'en'
and a.UseForML = 1
and s.UseForML = 0
and s.tokens = 1
GROUP BY s.ID
HAVING COUNT(1)>200
)

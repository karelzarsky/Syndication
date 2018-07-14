--truncate table nn.trainvalue
--truncate table nn.trainschema

INSERT INTO nn.trainSchema (NetworkID, NeuronID, Input, ShingleID)
SELECT 1, ROW_NUMBER() OVER (ORDER BY ID), 1, ID
FROM rss.shingles s
where UseForML = 1
AND (SELECT COUNT (1) FROM rss.shingleUse su JOIN rss.articles a ON a.ID = su.ArticleID WHERE a.UseForML = 1 AND su.ShingleID = s.ID) > 0

INSERT INTO nn.trainSchema (NetworkID, NeuronID, Input, ForecastDays)
SELECT 1, (SELECT MAX(NeuronID)+1 FROM nn.trainSchema) + ROW_NUMBER() OVER (ORDER BY a), 0, a
FROM (VALUES (0),(1),(2),(3),(4),(5),(6),(7)) as days (a)

INSERT INTO nn.trainValue (NetworkID, NeuronID, ArticleID, [Value])
SELECT * FROM
	(SELECT s.NetworkID NetworkID, s.NeuronID NeuronID, a.ID ArticleID,
	-1 +
	(SELECT TOP 1 CAST (adj_close AS float) FROM int.Prices WITH(NOLOCK) WHERE ticker = a.Ticker AND date >= DATEADD(d, s.ForecastDays, a.PublishedUTC) ORDER BY date) /
	(SELECT TOP 1 CAST (adj_open AS float) FROM int.prices WITH(NOLOCK) WHERE ticker = a.Ticker AND date >= a.PublishedUTC ORDER BY date) as percentChange
	FROM nn.trainSchema s
	JOIN rss.articles a ON s.Input = 0 AND a.UseForML = 1) x
WHERE percentChange IS NOT NULL

INSERT INTO nn.trainValue (NetworkID, NeuronID, ArticleID, [Value])
SELECT NetworkID, ts.NeuronID, su.ArticleID, 1 FROM rss.shingleUse su
JOIN nn.trainSchema ts ON ts.ShingleID = su.ShingleID
JOIN rss.articles a ON a.ID = su.ArticleID
WHERE a.UseForML = 1

SELECT DISTINCT ts.NeuronID, * FROM nn.trainSchema ts
LEFT JOIN nn.trainValue tv ON tv.NeuronID = ts.NeuronID
WHERE tv.NeuronID IS NULL

SELECT DISTINCT (vv1.ArticleID) FROM
(SELECT v1.* FROM nn.trainValue v1 JOIN nn.trainSchema s1 ON v1.NeuronID = s1.NeuronID WHERE s1.Input = 1) AS vv1
LEFT JOIN
(SELECT v2.* FROM nn.trainValue v2 JOIN nn.trainSchema s2 ON v2.NeuronID = s2.NeuronID WHERE s2.Input = 2) AS vv2
ON vv1.ArticleID = vv2.ArticleID
WHERE vv2.ArticleID IS NULL

DELETE FROM nn.trainValue WHERE ArticleID in
(SELECT ai.ArticleID FROM
(SELECT DISTINCT ArticleID FROM nn.trainValue v2 JOIN nn.trainSchema s2 ON v2.NeuronID = s2.NeuronID WHERE s2.Input = 1) ai
LEFT JOIN
(SELECT DISTINCT ArticleID FROM nn.trainValue v1 JOIN nn.trainSchema s1 ON v1.NeuronID = s1.NeuronID WHERE s1.Input = 0) ao
ON ai.ArticleID = ao.ArticleID
WHERE ao.ArticleID IS NULL)

DELETE FROM nn.trainValue WHERE ArticleID in
(SELECT ao.ArticleID FROM
(SELECT DISTINCT ArticleID FROM nn.trainValue v1 JOIN nn.trainSchema s1 ON v1.NeuronID = s1.NeuronID WHERE s1.Input = 0) ao
LEFT JOIN
(SELECT DISTINCT ArticleID FROM nn.trainValue v2 JOIN nn.trainSchema s2 ON v2.NeuronID = s2.NeuronID WHERE s2.Input = 1) ai
ON ai.ArticleID = ao.ArticleID
WHERE ai.ArticleID IS NULL)

SELECT COUNT (DISTINCT ArticleID), COUNT (1), CAST (COUNT(1) AS real) / COUNT (DISTINCT ArticleID), MAX(NeuronID)
FROM nn.trainValue

SELECT top 10000 NetworkID, NeuronID, ArticleID, 100 * Value from nn.trainValue
ORDER BY ArticleID, NeuronID


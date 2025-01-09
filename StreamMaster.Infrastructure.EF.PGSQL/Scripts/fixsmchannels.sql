INSERT INTO public."SMChannelStreamLinks" ("SMChannelId", "SMStreamId", "Rank")
SELECT 
    c."Id" AS "SMChannelId",
    c."BaseStreamID" AS "SMStreamId",
    0 AS "Rank"
FROM 
    public."SMChannels" c
WHERE 
    NOT EXISTS (
        SELECT 1
        FROM public."SMChannelStreamLinks" l
        WHERE l."SMChannelId" = c."Id"
          AND l."SMStreamId" = c."BaseStreamID"
    )
    AND EXISTS (
        SELECT 1
        FROM public."SMStreams" s
        WHERE s."Id" = c."BaseStreamID"
    );

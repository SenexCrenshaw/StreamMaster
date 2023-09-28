
import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { apiKey, baseHostURL } from '../settings';

const rawBaseQuery = fetchBaseQuery({ baseUrl: baseHostURL, headers: { 'x-api-key': apiKey } });

export const emptySplitApi = createApi({
  baseQuery: rawBaseQuery,
  endpoints: () => ({}),
});

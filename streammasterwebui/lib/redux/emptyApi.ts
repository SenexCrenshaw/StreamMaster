import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { baseHostURL } from '../settings';

const rawBaseQuery = fetchBaseQuery({
  baseUrl: baseHostURL,
});

export const emptySplitApi = createApi({
  baseQuery: rawBaseQuery,
  endpoints: () => ({}),
});

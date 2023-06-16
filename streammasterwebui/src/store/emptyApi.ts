import { type BaseQueryFn, type FetchArgs, type FetchBaseQueryError} from '@reduxjs/toolkit/query/react';
import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { apiKey, baseHostURL } from '../settings';

function appendQueryStringParam (args: FetchArgs | string, key: string, value: string): FetchArgs | string {
  let urlEnd = typeof args === 'string' ? args : args.url;

  if (urlEnd.indexOf('?') < 0)
    urlEnd += '?';
  else
    urlEnd += '&';

  urlEnd += `${key}=${value}`;

  return typeof args === 'string' ? urlEnd : { ...args, url: urlEnd };
}

const rawBaseQuery = fetchBaseQuery({ baseUrl: baseHostURL });
// const rawBaseQuery = fetchBaseQuery({ baseUrl: baseHostURL });

const dynamicBaseQuery: BaseQueryFn<
FetchArgs | string,
unknown,
FetchBaseQueryError
> = async (args, api, extraOptions) => {
  args = appendQueryStringParam(args, 'apiKey', apiKey);

  return rawBaseQuery(args, api, extraOptions);
};

// initialize an empty api service that we'll inject endpoints into later as needed
export const emptySplitApi = createApi({
  baseQuery: dynamicBaseQuery,
  endpoints: () => ({}),
});

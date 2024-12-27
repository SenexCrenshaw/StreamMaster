import { GetStreamConnectionMetricDatas } from '@lib/smAPI/Statistics/StatisticsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamConnectionMetricDatas = createAsyncThunk('cache/getGetStreamConnectionMetricDatas', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetStreamConnectionMetricDatas');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetStreamConnectionMetricDatas();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetStreamConnectionMetricDatas completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



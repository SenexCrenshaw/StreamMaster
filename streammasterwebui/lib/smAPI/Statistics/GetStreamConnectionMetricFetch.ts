import { GetStreamConnectionMetric } from '@lib/smAPI/Statistics/StatisticsCommands';
import { GetStreamConnectionMetricRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamConnectionMetric = createAsyncThunk('cache/getGetStreamConnectionMetric', async (param: GetStreamConnectionMetricRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetStreamConnectionMetric');
        return undefined;
    }
    Logger.debug('Fetching GetStreamConnectionMetric');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetStreamConnectionMetric(param);
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetStreamConnectionMetric completed in ${duration.toFixed(2)}ms`);
    }

    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



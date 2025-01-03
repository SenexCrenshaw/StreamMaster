import { GetStreamConnectionMetricData } from '@lib/smAPI/Statistics/StatisticsCommands';
import { GetStreamConnectionMetricDataRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamConnectionMetricData = createAsyncThunk('cache/getGetStreamConnectionMetricData', async (param: GetStreamConnectionMetricDataRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetStreamConnectionMetricData');
        return undefined;
    }
    Logger.debug('Fetching GetStreamConnectionMetricData');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetStreamConnectionMetricData(param);
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetStreamConnectionMetricData completed in ${duration.toFixed(2)}ms`);
    }

    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



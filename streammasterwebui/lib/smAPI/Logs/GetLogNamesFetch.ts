import { GetLogNames } from '@lib/smAPI/Logs/LogsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetLogNames = createAsyncThunk('cache/getGetLogNames', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetLogNames');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetLogNames();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetLogNames completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



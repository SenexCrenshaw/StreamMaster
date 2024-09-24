import { GetLogos } from '@lib/smAPI/Logos/LogosCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetLogos = createAsyncThunk('cache/getGetLogos', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetLogos');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetLogos();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetLogos completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



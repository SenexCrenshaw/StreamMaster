import { GetEPGColors } from '@lib/smAPI/EPG/EPGCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetEPGColors = createAsyncThunk('cache/getGetEPGColors', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetEPGColors');
  const fetchDebug = localStorage.getItem('fetchDebug');
 const start = performance.now();
    const response = await GetEPGColors();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetM3UFiles completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



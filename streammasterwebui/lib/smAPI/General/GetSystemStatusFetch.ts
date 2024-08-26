import { GetSystemStatus } from '@lib/smAPI/General/GeneralCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSystemStatus = createAsyncThunk('cache/getGetSystemStatus', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSystemStatus');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetSystemStatus();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetSystemStatus completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



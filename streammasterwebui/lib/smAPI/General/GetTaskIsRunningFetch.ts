import { GetTaskIsRunning } from '@lib/smAPI/General/GeneralCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetTaskIsRunning = createAsyncThunk('cache/getGetTaskIsRunning', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetTaskIsRunning');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetTaskIsRunning();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetTaskIsRunning completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



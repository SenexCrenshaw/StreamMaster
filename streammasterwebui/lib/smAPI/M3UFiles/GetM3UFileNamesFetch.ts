import { GetM3UFileNames } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGetM3UFileNames = createAsyncThunk('cache/getGetM3UFileNames', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetM3UFileNames');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetM3UFileNames();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetM3UFiles completed in ${duration.toFixed(2)}ms`);
    }

    return { param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});

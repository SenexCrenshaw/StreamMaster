import { GetIcons } from '@lib/smAPI/Icons/IconsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetIcons = createAsyncThunk('cache/getGetIcons', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetIcons');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetIcons();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetIcons completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



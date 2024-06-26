import { GetSettings } from '@lib/smAPI/Settings/SettingsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetSettings = createAsyncThunk('cache/getGetSettings', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetSettings');
    const response = await GetSettings();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



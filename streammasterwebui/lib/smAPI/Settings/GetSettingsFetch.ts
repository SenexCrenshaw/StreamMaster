import { GetSettings } from '@lib/smAPI/Settings/SettingsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSettings = createAsyncThunk('cache/getGetSettings', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetSettings');
    const response = await GetSettings();
    console.log('Fetched GetSettings',response);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


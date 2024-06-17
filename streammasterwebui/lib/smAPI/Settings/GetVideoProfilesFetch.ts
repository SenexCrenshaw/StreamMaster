import { GetVideoProfiles } from '@lib/smAPI/Settings/SettingsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetVideoProfiles = createAsyncThunk('cache/getGetVideoProfiles', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetVideoProfiles');
    const response = await GetVideoProfiles();
    console.log('Fetched GetVideoProfiles',response);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});



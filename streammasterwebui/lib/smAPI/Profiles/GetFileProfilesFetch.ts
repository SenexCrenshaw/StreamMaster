import { GetFileProfiles } from '@lib/smAPI/Profiles/ProfilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetFileProfiles = createAsyncThunk('cache/getGetFileProfiles', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetFileProfiles');
    const response = await GetFileProfiles();
    console.log('Fetched GetFileProfiles ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


